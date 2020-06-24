﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

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
    private Button FirstButton;

    private readonly GameScriptsProvider gameScriptsProvider = new GameScriptsProvider();
    private readonly SubtitlesProvider subtitlesProvider = new SubtitlesProvider();

    private bool GameLoaded;
    private bool VideoPaused;

    public Dictionary<int, Scene> Scenes;

    private int _сurrentSceneId;

    private int CurrentSceneId
    {
        get => _сurrentSceneId;
        set
        {
            _сurrentSceneId = value;
            if (!GameplayStatistics.ViewedScenes.Contains(value))
                GameplayStatistics.ViewedScenes.Add(value);
        }
    }

    private Scene CurrentScene => Scenes[CurrentSceneId];

    async Task Start()
    {
        Scenes = new ScenesLoader().LoadData(ScenesJsonData.text, out var currentSceneId);
        GameplayStatistics.Clear(Scenes.Count);
        CurrentSceneId = currentSceneId;

        SubtitlesText.gameObject.SetActive(false);
        ChoicesPanelParent.SetActive(false);
        VideoPlayer.loopPointReached += VideoCompleted;
        VideoPlayer.started += source => StartCoroutine(PlaySubtitles());
        VideoPlayer.playbackSpeed = Application.isEditor ? 3 : 1; //полезно для дебага

        await PlayFirstVideo(Scenes[CurrentSceneId]);
    }

    void Update()
    {
        //открыто вложенное меню? Тогда не перехватывем фокус кнопки. Некрасиво, сам понимаю, но сходу универсальнее не придумал
        if (SceneManager.sceneCount > 1)
            return;

        if (VideoPaused)
        {
            VideoPaused = false;
            VideoPlayer.Play();
        }

        UIHelpers.ReturnSelectToControl(FirstButton);
    }

    public override void OnEscape()
    {
        if (GameLoaded && SceneManager.sceneCount == 1)
        {
            if (VideoPlayer.isPlaying)
            {
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
        var choice = gameScriptsProvider.ChoicesHandle(CurrentScene.Choices[buttonNumber]);
        CurrentSceneId = choice.SceneId;

        if (CurrentScene.MusicNameOnStart != null)
            PlayMusic(CurrentScene.MusicNameOnStart);

        PlayVideo(CurrentScene.FileName);
    }

    void VideoCompleted(VideoPlayer vp)
    {
        Cursor.visible = true;
        var currentScene = gameScriptsProvider.SceneCompleteHandle(CurrentScene);

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
            var choice = gameScriptsProvider.ChoicesHandle(choiceValue);
            var button = Instantiate(ButtonPrototype);
            button.GetComponentInChildren<Text>().text = choice.Caption;
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

        //показываем варианты ответа с анимаицией
        StartCoroutine(UIHelpers.FadeIn(ChoicesPanelParent.GetComponent<CanvasGroup>(), 0.4f));
    }

    private IEnumerator PlaySubtitles()
    {
        SubtitleBlock oldSubs = null;

        while (VideoPlayer.isPlaying && Config.SubtitlesOn)
        {
            var subs = subtitlesProvider.GetForTime(VideoPlayer.time);
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
        VideoPlayer.url = GetVideoFileName(videoFileName);
        VideoPlayer.Play();
    }

    private async Task PlayFirstVideo(Scene scene)
    {
        Cursor.visible = false;

        //Все извращения в этом коде - чтобы не было мигания фона в момент подгрузки первого видео. И мне они совсем не нравятся.

        VideoPlayer.SetDirectAudioMute(0, true);
        TryLoadSubtitles(scene.FileName);
        VideoPlayer.url = GetVideoFileName(scene.FileName);
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
    }

    private void TryLoadSubtitles(string subtitlesFileName)
    {
        var fileName = Path.Combine(Application.streamingAssetsPath, subtitlesFileName + ".srt");
        if (File.Exists(fileName))
        {
            var text = File.ReadAllText(fileName);
            subtitlesProvider.Load(text);
        }
        else
            subtitlesProvider.Clear();
    }

    private static string GetVideoFileName(string videoFileName)
    {
        return Path.Combine(Application.streamingAssetsPath, videoFileName + ".mp4");
    }
}