// ReSharper disable RedundantUsingDirective
using UnityEngine;
// ReSharper restore RedundantUsingDirective
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreen : BaseGameScreen
{
    public Button BeginGameButton;
    public Button OptionsButton;
    public Button ExitButton;

    void Start()
    {
        BeginGameButton.onClick.AddListener(ScreensNavigator.GoToGameplay);

        OptionsButton.onClick.AddListener(ScreensNavigator.OpenOptionsScreen);

        ExitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

    void Update()
    {
        //открыто вложенное меню? Тогда не перехватывем фокус кнопки. Некрасиво, сам понимаю, но сходу универсальнее не придумал
        if (SceneManager.sceneCount > 1)
            return;
        UIHelpers.ReturnSelectToControl(BeginGameButton);
    }
}