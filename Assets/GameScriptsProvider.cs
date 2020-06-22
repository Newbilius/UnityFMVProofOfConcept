using System;
using System.Collections.Generic;
using System.Linq;

public class GameScriptsProvider
{
    private static int KnuckleAttackCounter = 0;

    public SceneChoice ChoicesHandle(SceneChoice choice)
    {
        if (!ChoicesHandlers.TryGetValue(choice.Caption, out var choiceHandler))
            return choice;

        var newChose = new SceneChoice
        {
            Caption = choice.Caption,
            SceneId = choice.SceneId
        };
        choiceHandler(newChose);
        return newChose;
    }

    public Scene SceneCompleteHandle(Scene scene)
    {
        if (!SceneCompleteActions.TryGetValue(scene.SceneId, out var sceneHandler))
            return scene;

        var newScene = new Scene(scene.FileName,
            scene.SceneId,
            scene.Choices,
            scene.MusicNameOnStart,
            scene.MusicNameOnEnd);

        sceneHandler(newScene);
        return newScene;
    }

    private readonly Dictionary<string, Action<SceneChoice>> ChoicesHandlers
        = new Dictionary<string, Action<SceneChoice>>
        {
            {
                "HAND",
                choice => { choice.Caption = GetKnuckleAttackText(); }
            },
            {
                "ChainsawAtack",
                choice => { choice.Caption = GetKnuckleAttackHaveChainsawText(); }
            },
            {
                "BAD_ENGING",
                choice => { choice.Caption = "Секретная концовка"; }
            }
        };

    private readonly Dictionary<string, Action<Scene>> SceneCompleteActions
        = new Dictionary<string, Action<Scene>>
        {
            {
                "HAND_ATACK",
                scene => { KnuckleAttackCounter++; }
            },
            {
                "BEGIN",
                scene => { KnuckleAttackCounter = 0; }
            },
            {
                "RESTART",
                scene =>
                {
                    if (KnuckleAttackCounter < 3)
                    {
                        scene.Choices = scene.Choices.Where(x => x.Caption != "BAD_ENGING").ToArray();
                    }
                }
            }
        };

    private static string GetKnuckleAttackText()
    {
        switch (KnuckleAttackCounter)
        {
            case 0:
                return "Пойти в рукопашную";
            case 1:
                return "Пойти в рукопашную (опять?)";
            case 2:
                return "Пойти в рукопашную (серьёзно?!)";
            case 3:
                return "Пойти в рукопашную (да ладно?!!!)";
            default:
                return $"Пойти в рукопашную ({KnuckleAttackCounter} раз?!)";
        }
    }

    private static string GetKnuckleAttackHaveChainsawText()
    {
        return KnuckleAttackCounter < 4
            ? "Атаковать бензопилой"
            : "Атаковать бензопилой (может попробовать?)";
    }
}