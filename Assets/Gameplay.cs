using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Gameplay : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public GameObject LoadingScreen;
    public GameObject ChoicesPanel;
    public Button ButtonPrototype;
    public AudioSource MusicAudioSource;

    public Dictionary<SceneId, Scene> Scenes = new ScenesInitializer().Init();

    private SceneId CurrentSceneId = SceneId.Start;

    private Scene CurrentScene => Scenes[CurrentSceneId];

    async Task Start()
    {
        QualitySettings.vSyncCount = 1;
        ChoicesPanel.SetActive(false);
        VideoPlayer.loopPointReached += VideoCompleted;
//        VideoPlayer.playbackSpeed = 3; //полезно для дебага

        await FirstVideo(Scenes[SceneId.Start].FileName,
            Scenes[SceneId.Start].MusicNameOnStart);
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
        CurrentSceneId = CurrentScene.Choices[buttonNumber].GetSceneId();

        if (CurrentScene.MusicNameOnStart != null)
            ChangeMusic(CurrentScene.MusicNameOnStart);

        Play(CurrentScene.FileName);
    }

    void VideoCompleted(VideoPlayer vp)
    {
        CurrentScene.OnCompleteAction?.Invoke(CurrentScene);

        if (CurrentScene.MusicNameOnEnd != null)
            ChangeMusic(CurrentScene.MusicNameOnEnd);

        ChoicesPanel.SetActive(true);

        foreach (Transform child in ChoicesPanel.transform)
            Destroy(child.gameObject);

        var buttonNumber = 0;
        foreach (var choice in CurrentScene.Choices)
        {
            var button = Instantiate(ButtonPrototype);
            button.GetComponentInChildren<Text>().text = choice.GetCaption();

            button.transform.SetParent(ChoicesPanel.transform, false);

            var buttonNumberCurrentValue = buttonNumber;
            button.onClick.AddListener(() => SelectButton(buttonNumberCurrentValue));
            buttonNumber++;
        }
    }

    private void ChangeMusic(string name)
    {
        //todo делать кроссфейд между композициями
        MusicAudioSource.clip = Resources.Load<AudioClip>("Music/" + name);
        MusicAudioSource.Play();
    }

    private void Play(string name)
    {
        VideoPlayer.Pause();
        VideoPlayer.url = Path.Combine(Application.streamingAssetsPath, name + ".mp4");
        VideoPlayer.Prepare();
        VideoPlayer.Play();
    }

    private async Task FirstVideo(string name,
        string musicName)
    {
        VideoPlayer.SetDirectAudioMute(0, true);
        VideoPlayer.url = Path.Combine(Application.streamingAssetsPath, name + ".mp4");
        VideoPlayer.Prepare();
        if (!VideoPlayer.isPrepared)
            await Task.Delay(100);
        VideoPlayer.Play();
        VideoPlayer.Pause();
        VideoPlayer.frame = 1;

        MusicAudioSource.clip = Resources.Load<AudioClip>("Music/" + musicName);

        await Task.Delay(500);
        VideoPlayer.SetDirectAudioMute(0, false);
        MusicAudioSource.Play();
        VideoPlayer.Play();
        LoadingScreen.SetActive(false);
    }
}