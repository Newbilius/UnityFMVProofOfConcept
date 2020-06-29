using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsScreen : BaseGameScreen
{
    public Button SaveButton;
    public Button CancelButton;
    public ColorTextButton ChangeSubtitlesToggleButton;
    private bool SubtitlesOn;

    public override void OnEscape()
    {
        ScreensNavigator.CloseOptionsScreen();
    }

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);

        SubtitlesOn = Config.SubtitlesOn;

        SaveButton.onClick.AddListener(() =>
        {
            Config.SubtitlesOn = SubtitlesOn;
            ScreensNavigator.CloseOptionsScreen();
        });

        CancelButton.onClick.AddListener(ScreensNavigator.CloseOptionsScreen);
        ChangeSubtitlesToggleButton.onClick.AddListener(() =>
        {
            SubtitlesOn = !SubtitlesOn;
            ChangeSubtitlesStateText();
        });

        ChangeSubtitlesStateText();
    }

    private void ChangeSubtitlesStateText()
    {
        ChangeSubtitlesToggleButton.Text = $"Субтитры: {(SubtitlesOn ? "Включены" : "Выключены")}";
    }

    protected override void Update()
    {
        base.Update();
        if (IsActiveScreen())
            UIHelpers.ReturnSelectToControl(ChangeSubtitlesToggleButton);
    }
}