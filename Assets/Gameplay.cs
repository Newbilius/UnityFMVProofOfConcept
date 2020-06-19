using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

//todo тут сейчас уныние в плане управления кнопками, пока тестирую саму идею
public class Gameplay : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public GameObject LoadingScreen;
    public GameObject ChoicesPanel;
    public Button[] Buttons;

    public Dictionary<SceneId, Scene> Scenes = new ScenesInitializer().Init();

    private SceneId CurrentSceneId = SceneId.Start;

    async Task Start()
    {
        ChoicesPanel.SetActive(false);
        VideoPlayer.loopPointReached += VideoCompleted;
        //VideoPlayer.playbackSpeed = 3; //полезно для дебага

        var counter = 0;
        foreach (var button in Buttons)
        {
            var counterCurrentValue = counter;
            button.onClick.AddListener(() => SelectButton(counterCurrentValue));

            counter++;
        }

        await FirstVideo(Scenes[SceneId.Start].FileName);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (VideoPlayer.isPlaying)
                VideoPlayer.Pause();
            else
                VideoPlayer.Play();
        }
    }

    void SelectButton(int buttonNumber)
    {
        ChoicesPanel.SetActive(false);
        CurrentSceneId = Scenes[CurrentSceneId].Choices[buttonNumber].GetSceneId();
        Play(Scenes[CurrentSceneId].FileName);
    }

    void VideoCompleted(VideoPlayer vp)
    {
        var currentScene = Scenes[CurrentSceneId];
        currentScene.OnCompleteAction?.Invoke(currentScene);

        ChoicesPanel.SetActive(true);

        Buttons[1].gameObject.SetActive(currentScene.Choices.Length != 1);
        var buttonNumber = 0;
        foreach (var choice in currentScene.Choices)
        {
            Buttons[buttonNumber].GetComponentInChildren<Text>().text = choice.GetCaption();
            buttonNumber++;
        }
    }

    private void Play(string name)
    {
        VideoPlayer.Pause();
        VideoPlayer.url = Path.Combine(Application.streamingAssetsPath, name + ".mp4");
        VideoPlayer.Prepare();
        VideoPlayer.Play();
    }

    private async Task FirstVideo(string name)
    {
        VideoPlayer.url = Path.Combine(Application.streamingAssetsPath, name + ".mp4");
        VideoPlayer.Prepare();
        if (!VideoPlayer.isPrepared)
            await Task.Delay(500);
        VideoPlayer.Play();
        VideoPlayer.Pause();
        VideoPlayer.frame = 1;

        await Task.Delay(500);
        LoadingScreen.SetActive(false);
        VideoCompleted(VideoPlayer);
    }
}