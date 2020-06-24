using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayMenuScreen : MonoBehaviour
{
    public Button ContinueButton;
    public Button OptionsButton;
    public Button ExitButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

        ContinueButton.onClick.AddListener(ScreensNavigator.CloseGameplayMenuScreen);
        OptionsButton.onClick.AddListener(ScreensNavigator.OpenOptionsScreen);
        ExitButton.onClick.AddListener(ScreensNavigator.GotoMenu);
    }

    void Update()
    {
        //открыто вложенное меню? Тогда не перехватывем фокус кнопки. Некрасиво, сам понимаю, но сходу универсальнее не придумал
        if (SceneManager.sceneCount > 2)
            return;
        UIHelpers.ReturnSelectToControl(ContinueButton);

        if (UIHelpers.EscapeOrStartButtonPressed())
            ScreensNavigator.CloseGameplayMenuScreen();
    }
}
