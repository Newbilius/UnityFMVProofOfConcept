﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public Dictionary<SceneId, Scene> Scenes = new ScenesInitializer().Init();

    private SceneId CurrentSceneId = SceneId.Start;
    private Button FirstButton;

    private Scene CurrentScene => Scenes[CurrentSceneId];

    async Task Start()
    {
        //лень было разбираться, где в опциях включить это для всех tier :_:
        QualitySettings.vSyncCount = 1;

        ChoicesPanelParent.SetActive(false);
        VideoPlayer.loopPointReached += VideoCompleted;
        VideoPlayer.playbackSpeed = Application.isEditor ? 3 : 1; //полезно для дебага

        await FirstVideo(Scenes[SceneId.Start]);
    }

    void Update()
    {
        //чтобы нельзя было кликнуть на пустом месте и выбрать "никакой" вариант в диалоге
        if (FirstButton != null && EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(FirstButton.gameObject);
    }

    void SelectButton(int buttonNumber)
    {
        FirstButton = null;
        EventSystem.current.SetSelectedGameObject(null);

        ChoicesPanelParent.SetActive(false);
        CurrentSceneId = CurrentScene.Choices[buttonNumber].GetSceneId();

        if (CurrentScene.MusicNameOnStart != null)
            ChangeMusic(CurrentScene.MusicNameOnStart);

        Play(CurrentScene.FileName);
    }

    void VideoCompleted(VideoPlayer vp)
    {
        Cursor.visible = true;
        CurrentScene.OnCompleteAction?.Invoke(CurrentScene);

        if (CurrentScene.MusicNameOnEnd != null)
            ChangeMusic(CurrentScene.MusicNameOnEnd);

        foreach (Transform child in ChoicesPanel.transform)
            Destroy(child.gameObject);

        ChoicesPanelParent.SetActive(true);

        var buttonNumber = 0;
        var buttons = new List<Button>();
        foreach (var choice in CurrentScene.Choices)
        {
            var button = Instantiate(ButtonPrototype);
            button.GetComponentInChildren<Text>().text = choice.GetCaption();
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
        StartCoroutine(DoFade(ChoicesPanelParent.GetComponent<CanvasGroup>(), 0.4f));
    }

    //todo а без корутин и издевательства над IEnumerator никак?
    private IEnumerator DoFade(CanvasGroup canvasGroup, float duration)
    {
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, counter / duration);

            yield return null;
        }
    }

    private void ChangeMusic(string name)
    {
        //todo делать кроссфейд между композициями?
        MusicAudioSource.clip = Resources.Load<AudioClip>("Music/" + name);
        MusicAudioSource.Play();
    }

    private void Play(string name)
    {
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
        VideoPlayer.SetDirectAudioMute(0, true);
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

    private string GetFileName(string name)
    {
        return Path.Combine(Application.streamingAssetsPath, name + ".mp4");
    }
}