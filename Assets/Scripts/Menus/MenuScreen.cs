//если выкинуть эту строчки - билд перестаёт собираться
using UnityEngine;
//
using UnityEngine.UI;

public class MenuScreen : BaseGameScreen
{
    public Button BeginGameButton;
    public Button OptionsButton;
    public Button ExitButton;

    void Start()
    {
        var currentResolution = Config.Resolution;
        if (currentResolution != null 
            && currentResolution.Width != 0 
            && currentResolution.Height != 0)
        {
            Screen.SetResolution(currentResolution.Width,
                currentResolution.Height,
                Config.IsFullscreen);
        }

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

    protected override void Update()
    {
        base.Update();
        if (IsActiveScreen())
            UIHelpers.ReturnSelectToControl(BeginGameButton);
    }
}