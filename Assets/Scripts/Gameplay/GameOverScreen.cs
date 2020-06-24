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
        ScenesCount.text = $"Сцен просмотрено: {GameplayStatistics.ViewedScenes.Count} из {GameplayStatistics.AllScenesCount}";
        ChoicesCount.text = $"Выборов сделано: {GameplayStatistics.ChoicesCount}";

        ReturnToMenuButton.onClick.AddListener(ScreensNavigator.GotoMainMenu);
        RestartButton.onClick.AddListener(ScreensNavigator.GoToGameplay);
    }

    void Update()
    {
        UIHelpers.ReturnSelectToControl(ReturnToMenuButton);
    }
}
