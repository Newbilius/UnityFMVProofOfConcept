using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public Button BeginGameButton;
    public Button OptionsButton;
    public Button ExitButton;

    // Start is called before the first frame update
    void Start()
    {
        BeginGameButton.onClick.AddListener(ScreensNavigator.StartGameplay);

        OptionsButton.onClick.AddListener(() => { Debug.Log("111"); });

        ExitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }

    // Update is called once per frame
    void Update()
    {
        UIHelpers.ReturnSelectToControl(BeginGameButton);
    }
}