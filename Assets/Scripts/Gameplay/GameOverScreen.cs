using UnityEngine.UI;

public class GameOverScreen : BaseGameScreen
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

    protected override void Update()
    {
        base.Update();
        UIHelpers.ReturnSelectToControl(ReturnToMenuButton);
    }
}
