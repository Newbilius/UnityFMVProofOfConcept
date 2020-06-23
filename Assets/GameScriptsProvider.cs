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
        if (!SceneCompleteActions.TryGetValue(scene.SceneCode, out var sceneHandler))
            return scene;

        var newScene = new Scene(scene.FileName,
            scene.SceneCode,
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
                    if (KnuckleAttackCounter < 4)
                        scene.Choices = scene.Choices.Where(x => x.SceneCode != "BAD_ENDING").ToArray();
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
                return $"Пойти в рукопашную ({KnuckleAttackCounter} раз?!!!!!)";
        }
    }
}