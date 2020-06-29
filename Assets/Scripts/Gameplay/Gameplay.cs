using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.UI;

//todo очень хочется прибраться - тут намешано сразу несколько логик (проигрывание и пропуск видео, работа с субтитрами, вариантами и т.п.)
public class Gameplay : BaseGameScreen
{
    public VideoPlayer VideoPlayer;
    public GameObject LoadingScreen;
    public GameObject ChoicesPanelParent;
    public GameObject ChoicesPanel;
    public Button ButtonPrototype;
    public AudioSource MusicAudioSource;
    public TextAsset ScenesJsonData;
    public Text SubtitlesText;
    public CanvasGroup SkipText;
    public Dictionary<int, Scene> Scenes;

    private readonly GameScriptsProvider GameScriptsProvider = new GameScriptsProvider();
    private readonly SubtitlesProvider SubtitlesProvider = new SubtitlesProvider();

    private Button FirstButton;
    private bool GameLoaded;
    private bool VideoPaused;
    private bool VideoInProgress;

    private int _currentSceneId;

    private int CurrentSceneId
    {
        get => _currentSceneId;
        set
        {
            _currentSceneId = value;
            if (!GameplayStatistics.ViewedScenes.Contains(value))
                GameplayStatistics.ViewedScenes.Add(value);
        }
    }

    protected override bool CanShowMouseCursor => !VideoPlayer.isPlaying;

    private Scene CurrentScene => Scenes[CurrentSceneId];

    async Task Start()
    {
        Scenes = new ScenesLoader().LoadData(ScenesJsonData.text, out var currentSceneId);
        GameplayStatistics.Clear(Scenes.Count);
        CurrentSceneId = currentSceneId;

        SkipText.alpha = 0;

        SubtitlesText.gameObject.SetActive(false);
        ChoicesPanelParent.SetActive(false);
        VideoPlayer.loopPointReached += _ => VideoCompleted();
        VideoPlayer.started += source =>
        {
            InputActions.Main.Skip.Enable();
            StartCoroutine(PlaySubtitles());
        };

        VideoPlayer.playbackSpeed = Application.isEditor ? 3 : 1; //полезно для дебага

        await PlayFirstVideo(Scenes[CurrentSceneId]);

        InputActions.Main.Skip.started += context
            => StartCoroutine(UIHelpers.FadeIn(SkipText, 0.3f));

        InputActions.Main.Skip.canceled += context =>
        {
            if (SkipText.alpha > 0)
                StartCoroutine(UIHelpers.FadeOut(SkipText, 0.3f));
        };

        InputActions.Main.Skip.performed += async context =>
        {
            if (VideoPlayer.isPlaying)
            {
                VideoPlayer.frame = (long) VideoPlayer.frameCount;
                SkipText.alpha = 0;

                //костыль на случай если пропустить видео ОЧЕНЬ неудачно - так, что не отработает VideoCompleted
                await Task.Delay(200);
                if (VideoInProgress) 
                    VideoCompleted();
            }
        };
    }

    protected override void Update()
    {
        base.Update();
        if (!IsActiveScreen() || !GameLoaded)
            return;

        if (VideoPaused)
        {
            InputActions.Main.Skip.Enable();
            VideoPaused = false;
            VideoPlayer.Play();
            Cursor.visible = CanShowMouseCursor;
        }

        UIHelpers.ReturnSelectToControl(FirstButton);
    }

    public override void OnEscape()
    {
        if (GameLoaded && IsActiveScreen())
        {
            if (VideoPlayer.isPlaying)
            {
                InputActions.Main.Skip.Disable();
                VideoPlayer.Pause();
                VideoPaused = true;
            }

            ScreensNavigator.OpenGameplayMenu();
        }
    }

    void SelectButton(int buttonNumber)
    {
        GameplayStatistics.ChoicesCount++;
        FirstButton = null;
        EventSystem.current.SetSelectedGameObject(null);

        ChoicesPanelParent.SetActive(false);
        var choice = GameScriptsProvider.ChoicesHandle(CurrentScene.Choices[buttonNumber]);
        CurrentSceneId = choice.SceneId;

        if (CurrentScene.MusicNameOnStart != null)
            PlayMusic(CurrentScene.MusicNameOnStart);

        PlayVideo(CurrentScene.FileName);
    }

    void VideoCompleted()
    {
        VideoInProgress = false;
        InputActions.Main.Skip.Disable();
        Cursor.visible = IsMouseMode;
        var currentScene = GameScriptsProvider.SceneCompleteHandle(CurrentScene);

        if (currentScene.MusicNameOnEnd != null)
            PlayMusic(currentScene.MusicNameOnEnd);

        foreach (Transform child in ChoicesPanel.transform)
            Destroy(child.gameObject);

        if (currentScene.Choices.Length == 0)
        {
            ScreensNavigator.GoToGameOver();
            return;
        }

        ChoicesPanelParent.SetActive(true);

        var buttonNumber = 0;
        var buttons = new List<Button>();
        foreach (var choiceValue in currentScene.Choices)
        {
            var choice = GameScriptsProvider.ChoicesHandle(choiceValue);
            var button = Instantiate(ButtonPrototype);
            var textComponent = button.GetComponentInChildren<Text>();
            textComponent.text = choice.Caption;
            textComponent.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            buttons.Add(button);

            button.transform.SetParent(ChoicesPanel.transform, false);

            var buttonNumberCurrentValue = buttonNumber;
            button.onClick.AddListener(() => SelectButton(buttonNumberCurrentValue));

            if (buttonNumber == 0)
            {
                FirstButton = button;
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }

            buttonNumber++;
        }

        //делаем цикличный выбор кнопок кнопками вверх-вниз и командами геймпада
        if (buttons.Count > 1)
        {
            buttonNumber = 0;
            buttons[0].navigation = new Navigation
            {
                selectOnUp = buttons.Last(),
                selectOnDown = buttons[1],
                mode = Navigation.Mode.Explicit
            };

            buttons.Last().navigation = new Navigation
            {
                selectOnUp = buttons[buttons.Count - 2],
                selectOnDown = buttons[0],
                mode = Navigation.Mode.Explicit
            };

            foreach (var button in buttons)
            {
                if (buttonNumber == 0)
                {
                    buttonNumber++;
                    continue;
                }

                if (buttonNumber == buttons.Count - 1)
                {
                    buttonNumber++;
                    continue;
                }

                button.navigation = new Navigation
                {
                    selectOnUp = buttons[buttonNumber - 1],
                    selectOnDown = buttons[buttonNumber + 1],
                    mode = Navigation.Mode.Explicit
                };

                buttonNumber++;
            }
        }

        //показываем варианты ответа с анимацией
        StartCoroutine(UIHelpers.FadeIn(ChoicesPanelParent.GetComponent<CanvasGroup>(), 0.4f));
    }

    private IEnumerator PlaySubtitles()
    {
        SubtitleBlock oldSubs = null;

        while (VideoPlayer.isPlaying && Config.SubtitlesOn)
        {
            var subs = SubtitlesProvider.GetForTime(VideoPlayer.time);
            if (subs != oldSubs)
            {
                oldSubs = subs;
                if (subs != null)
                {
                    SubtitlesText.text = subs.Text;
                    SubtitlesText.gameObject.SetActive(true);
                }
                else
                    SubtitlesText.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.05f);
        }

        SubtitlesText.gameObject.SetActive(false);
    }

    private void PlayMusic(string audioFileName)
    {
        if (audioFileName == "NULL")
            MusicAudioSource.Stop();
        else
        {
            MusicAudioSource.clip = Resources.Load<AudioClip>("Music/" + audioFileName);
            MusicAudioSource.Play();
        }
    }

    private void PlayVideo(string videoFileName)
    {
        TryLoadSubtitles(videoFileName);
        Cursor.visible = false;
        VideoPlayer.clip = GetVideoFile(videoFileName);
        VideoPlayer.Play();
        VideoInProgress = true;
    }

    private async Task PlayFirstVideo(Scene scene)
    {
        Cursor.visible = false;

        //Все извращения в этом коде - чтобы не было мигания фона в момент подгрузки первого видео, и мне это совсем не нравится

        VideoPlayer.SetDirectAudioMute(0, true);
        TryLoadSubtitles(scene.FileName);
        VideoPlayer.clip = GetVideoFile(scene.FileName);
        VideoPlayer.Prepare();
        if (!VideoPlayer.isPrepared)
            await Task.Delay(100);
        VideoPlayer.Play();
        VideoPlayer.Pause();
        VideoPlayer.frame = 1;

        PlayMusic(scene.MusicNameOnStart);

        await Task.Delay(500);
        VideoPlayer.SetDirectAudioMute(0, false);
        MusicAudioSource.Play();
        VideoPlayer.Play();
        LoadingScreen.SetActive(false);
        GameLoaded = true;
        VideoInProgress = true;
    }

    private void TryLoadSubtitles(string subtitlesFileName)
    {
        var subtitleText = Resources.Load<TextAsset>($"Video/{subtitlesFileName}");
        if (subtitleText != null)
            SubtitlesProvider.Load(subtitleText.text);
        else
            SubtitlesProvider.Clear();
    }

    private static VideoClip GetVideoFile(string videoFileName)
    {
        return Resources.Load<VideoClip>("Video/" + videoFileName);
    }
}