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

public class Gameplay : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public GameObject LoadingScreen;
    public GameObject ChoicesPanelParent;
    public GameObject ChoicesPanel;
    public Button ButtonPrototype;
    public AudioSource MusicAudioSource;
    public TextAsset ScenesJsonData;
    public Text SubtitlesText;
    private readonly GameScriptsProvider gameScriptsProvider = new GameScriptsProvider();
    private readonly SubtitlesProvider subtitlesProvider = new SubtitlesProvider();
    private Button FirstButton;

    public Dictionary<int, Scene> Scenes;

    private int _сurrentSceneId;

    private int CurrentSceneId
    {
        get => _сurrentSceneId;
        set
        {
            _сurrentSceneId = value;
            if (!GameplayStatus.ViewedScenes.Contains(value))
                GameplayStatus.ViewedScenes.Add(value);
        }
    }

    private Scene CurrentScene => Scenes[CurrentSceneId];

    async Task Start()
    {
        Scenes = new ScenesLoader().Init(ScenesJsonData.text, out var currentSceneId);
        GameplayStatus.ChoicesCount = 0;
        GameplayStatus.ScenesCount = Scenes.Count;
        GameplayStatus.ViewedScenes.Clear();
        CurrentSceneId = currentSceneId;

        //лень было разбираться, где в опциях включить это для всех tier :_:
        QualitySettings.vSyncCount = 1;

        SubtitlesText.gameObject.SetActive(false);
        ChoicesPanelParent.SetActive(false);
        VideoPlayer.loopPointReached += VideoCompleted;
        VideoPlayer.started += source => StartCoroutine(PlaySubtitles());
        VideoPlayer.playbackSpeed = Application.isEditor ? 3 : 1; //полезно для дебага

        await FirstVideo(Scenes[CurrentSceneId]);
    }

    void Update()
    {
        //чтобы нельзя было кликнуть на пустом месте и выбрать "никакой" вариант в диалоге
        if (FirstButton != null && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(FirstButton.gameObject);
    }

    void SelectButton(int buttonNumber)
    {
        GameplayStatus.ChoicesCount++;
        FirstButton = null;
        EventSystem.current.SetSelectedGameObject(null);

        ChoicesPanelParent.SetActive(false);
        var choice = gameScriptsProvider.ChoicesHandle(CurrentScene.Choices[buttonNumber]);
        CurrentSceneId = choice.SceneId;

        if (CurrentScene.MusicNameOnStart != null)
            ChangeMusic(CurrentScene.MusicNameOnStart);

        Play(CurrentScene.FileName);
    }

    void VideoCompleted(VideoPlayer vp)
    {
        Cursor.visible = true;
        var currentScene = gameScriptsProvider.SceneCompleteHandle(CurrentScene);

        if (currentScene.MusicNameOnEnd != null)
            ChangeMusic(currentScene.MusicNameOnEnd);

        foreach (Transform child in ChoicesPanel.transform)
            Destroy(child.gameObject);

        if (currentScene.Choices.Length == 0)
        {
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
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
        StartCoroutine(FadeIn(ChoicesPanelParent.GetComponent<CanvasGroup>(), 0.4f));
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, counter / duration);

            yield return null;
        }
    }

    private IEnumerator PlaySubtitles()
    {
        SubtitleBlock oldSubs = null;

        while (VideoPlayer.isPlaying)
        {
            var subs = subtitlesProvider.GetForTime((float) VideoPlayer.time);
            if (subs != oldSubs)
            {
                oldSubs = subs;
                if (subs != null)
                {
                    SubtitlesText.text = subs.Text;
                    SubtitlesText.gameObject.SetActive(true);
                }
                else
                {
                    SubtitlesText.gameObject.SetActive(false);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        SubtitlesText.gameObject.SetActive(false);
    }

    private void ChangeMusic(string name)
    {
        //возможно стоит сделать кроссфейд между композициями
        MusicAudioSource.clip = Resources.Load<AudioClip>("Music/" + name);
        MusicAudioSource.Play();
    }

    private void Play(string name)
    {
        TryLoadSubtitles(name);
        Cursor.visible = false;
        VideoPlayer.Pause();
        VideoPlayer.url = GetFileName(name);
        VideoPlayer.Prepare();
        VideoPlayer.Play();
    }

    private async Task FirstVideo(Scene scene)
    {
        Cursor.visible = false;

        //все извращения в этом коде - чтобы не было мигания фона в момент подгрузки первого видео
        //и мне они совсем не нравятся
        VideoPlayer.SetDirectAudioMute(0, true);
        TryLoadSubtitles(scene.FileName);
        VideoPlayer.url = GetFileName(scene.FileName);
        VideoPlayer.Prepare();
        if (!VideoPlayer.isPrepared)
            await Task.Delay(100);
        VideoPlayer.Play();
        VideoPlayer.Pause();
        VideoPlayer.frame = 1;

        MusicAudioSource.clip = Resources.Load<AudioClip>("Music/" + scene.MusicNameOnStart);

        await Task.Delay(500);
        VideoPlayer.SetDirectAudioMute(0, false);
        MusicAudioSource.Play();
        VideoPlayer.Play();
        LoadingScreen.SetActive(false);
    }

    private void TryLoadSubtitles(string name)
    {
        var fileName = Path.Combine(Application.streamingAssetsPath, name + ".srt");
        if (File.Exists(fileName))
        {
            var text = File.ReadAllText(fileName);
            subtitlesProvider.Load(text);
        }
        else
            subtitlesProvider.Clear();
    }

    private string GetFileName(string name)
    {
        return Path.Combine(Application.streamingAssetsPath, name + ".mp4");
    }
}