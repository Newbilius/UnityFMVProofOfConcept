using System.Collections.Generic;

public class ScenesInitializer
{
    private int KnuckleAttackCounter = 0;

    public Dictionary<SceneId, Scene> Init()
    {
        return new Dictionary<SceneId, Scene>
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
                            CaptionAction = GetKnuckleAttackText,
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
                            Caption = "Начать заново",
                            SceneId = SceneId.Start
                        }
                    },
                    scene => { KnuckleAttackCounter++; })
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
                            CaptionAction = GetKnuckleAttackText,
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
                            CaptionAction = GetKnuckleAttackHaveChainsawText,
                            SceneId = SceneId.ChainsawAttack
                        },
                        new SceneChoice
                        {
                            CaptionAction = GetKnuckleAttackText,
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
                            Caption = "Начать заново (экран успеха пока не готов)",
                            SceneId = SceneId.Start
                        }
                    })
            }
        };
    }

    private string GetKnuckleAttackHaveChainsawText()
    {
        return KnuckleAttackCounter < 2
            ? "Атаковать бензопилой"
            : "Атаковать бензопилой (просто гипотеза: возможно бензопила сильнее кастета...)";
    }

    private string GetKnuckleAttackText()
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
                return $"Пойти в рукопашную ({KnuckleAttackCounter} раз?! Это уже не смешно)";
        }
    }
}