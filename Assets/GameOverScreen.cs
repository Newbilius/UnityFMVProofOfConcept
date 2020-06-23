using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text ScenesCount;
    public Text ChoicesCount;

    public Button ReturnToMenuButton;
    public Button RestartButton;

    void Start()
    {
        ScenesCount.text = $"Сцен просмотрено: {GameplayStatisticss.ViewedScenes.Count} из {GameplayStatisticss.AllScenesCount}";
        ChoicesCount.text = $"Выборов сделано: {GameplayStatisticss.ChoicesCount}";

        ReturnToMenuButton.onClick.AddListener(ScreensNavigator.GotoMenu);
        RestartButton.onClick.AddListener(ScreensNavigator.StartGameplay);
    }

    void Update()
    {
        UIHelpers.ReturnSelectToControl(ReturnToMenuButton);
    }
}
