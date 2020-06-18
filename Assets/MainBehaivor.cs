using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

//todo тут сейчас уныние в плане управления кнопками, пока тестирую саму идею
public class MainBehaivor : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public GameObject LoadingScreen;
    public GameObject ChoicesPanel;
    public Button[] Buttons;

    public Dictionary<SceneId, Scene> Scenes = new Dictionary<SceneId, Scene>
    {
        {
            SceneId.Start, new Scene("00_начальная_сцена",
                new[]
                {
                    new SceneChoice
                    {
                        Caption = "Осмотреться",
                        SceneId = SceneId.LookAround
                    },
                    new SceneChoice
                    {
                        Caption = "Пойти в рукопашку",
                        SceneId = SceneId.KnuckleAttack
                    },
                })
        },

        {
            SceneId.KnuckleAttack, new Scene("02_атака_кулаком",
                new[]
                {
                    new SceneChoice
                    {
                        Caption = "Начать сначала",
                        SceneId = SceneId.Start
                    }
                })
        },

        {
            SceneId.LookAround, new Scene("01_осмотреться",
                new[]
                {
                    new SceneChoice
                    {
                        Caption = "Обыскать балкон",
                        SceneId = SceneId.FindChainsaw
                    },
                    new SceneChoice
                    {
                        Caption = "Пойти в рукопашку",
                        SceneId = SceneId.KnuckleAttack
                    }
                })
        },

        {
            SceneId.FindChainsaw, new Scene("03_взять_бензопилу",
                new[]
                {
                    new SceneChoice
                    {
                        Caption = "Аткаовать бензопилой",
                        SceneId = SceneId.ChainsawAttack
                    },
                    new SceneChoice
                    {
                        Caption = "Пойти в рукопашку",
                        SceneId = SceneId.KnuckleAttack
                    }
                })
        },

        {
            SceneId.ChainsawAttack, new Scene("04_атака_бензопилой",
                new[]
                {
                    new SceneChoice
                    {
                        Caption = "Начать сначала (экран успеха пока не готов)",
                        SceneId = SceneId.Start
                    }
                })
        },
    };

    private SceneId CurrentSceneId = SceneId.Start;

    async Task Start()
    {
        VideoPlayer.loopPointReached += VideoCompleted;

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
        CurrentSceneId = Scenes[CurrentSceneId].Choices[buttonNumber].SceneId;
        Play(Scenes[CurrentSceneId].FileName);
    }

    void VideoCompleted(VideoPlayer vp)
    {
        ChoicesPanel.SetActive(true);
        var currentScene = Scenes[CurrentSceneId];

        Buttons[1].gameObject.SetActive(currentScene.Choices.Length != 1);
        var buttonNumber = 0;
        foreach (var choice in currentScene.Choices)
        {
            Buttons[buttonNumber].GetComponentInChildren<Text>().text = choice.Caption;
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