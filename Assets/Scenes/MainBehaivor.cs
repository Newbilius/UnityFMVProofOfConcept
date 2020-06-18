using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class MainBehaivor : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject LoadingScreen;
    public GameObject Content;
    public GameObject Button_Template;

    async Task Start()
    {
        await FirstVideo("Level1/01_осмотреться");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.G))
        {
            Play("game");
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            Play("Level1/01_осмотреться");
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            Play("Level1/01_05_сразу пойти к двери");
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Play("Level1/01_to_02_подойти_к_броне");
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            Play("Level1/02_03_взять_броню");
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            Play("Level1/02_04_отвернуться от брони");
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Pause();
            else
                videoPlayer.Play();
        }
    }

    private void Play(string name)
    {
        //        videoPlayer.clip = Resources.Load<VideoClip>(name);
        videoPlayer.Pause();
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, name + ".mp4");
        videoPlayer.Prepare();
        videoPlayer.Play();
    }

    private async Task FirstVideo(string name)
    {
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, name + ".mp4");
        videoPlayer.Prepare();
        if (!videoPlayer.isPrepared)
            await Task.Delay(500);
        videoPlayer.Play();
        videoPlayer.Pause();
        videoPlayer.frame = 0;

        await Task.Delay(500);
        LoadingScreen.SetActive(false);
    }
}