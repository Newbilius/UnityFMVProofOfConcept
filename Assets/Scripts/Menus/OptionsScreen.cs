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

    // Start is called before the first frame update
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
        ChangeSubtitlesToggleButton.Text.text = $"Субтитры: {(SubtitlesOn ? "Включены" : "Выключены")}";
    }

    // Update is called once per frame
    void Update()
    {
        UIHelpers.ReturnSelectToControl(ChangeSubtitlesToggleButton);
    }
}