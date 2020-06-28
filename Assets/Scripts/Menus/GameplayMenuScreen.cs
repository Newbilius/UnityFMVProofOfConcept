using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayMenuScreen : BaseGameScreen
{
    public Button ContinueButton;
    public Button OptionsButton;
    public Button ExitButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

        ContinueButton.onClick.AddListener(ScreensNavigator.CloseGameplayMenu);
        OptionsButton.onClick.AddListener(ScreensNavigator.OpenOptionsScreen);
        ExitButton.onClick.AddListener(ScreensNavigator.GotoMainMenu);
    }

    void Update()
    {
        if (IsActiveScreen())
            UIHelpers.ReturnSelectToControl(ContinueButton);
    }

    public override void OnEscape()
    {
        if (IsActiveScreen())
            ScreensNavigator.CloseGameplayMenu();
    }
}