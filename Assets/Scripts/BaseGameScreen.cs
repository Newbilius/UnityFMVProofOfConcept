using UnityEngine;
using UnityEngine.InputSystem;

public class BaseGameScreen : MonoBehaviour
{
    public InputActions InputActions;
    protected bool IsMouseMode;
    private int CurrentScreenNumber;

    void Awake()
    {
        CurrentScreenNumber = ScreensNavigator.ScreenNumberCounter;
        InputActions = new InputActions();
        InputActions.Main.Escape.performed += _ => OnEscape();
        InputActions.Main.AnyAction.performed += _ => AnyKeyPressed();
    }

    protected bool IsActiveScreen()
    {
        return CurrentScreenNumber == ScreensNavigator.ScreenNumberCounter;
    }

    public void AnyKeyPressed()
    {
        IsMouseMode = false;
        Cursor.visible = false;
    }

    private bool needToCallOnResume;

    void OnGUI()
    {
        if (!IsActiveScreen())
            needToCallOnResume = true;

        if (IsActiveScreen() && needToCallOnResume)
        {
            needToCallOnResume = false;
            OnResume();
        }

        if (!IsMouseMode)
        {
            var mouse = Mouse.current;
            if (mouse != null)
            {
                var mouseValue = mouse.delta.ReadValue();
                var mouseDelta = Mathf.Abs(mouseValue.x) + Mathf.Abs(mouseValue.y);
                if (mouseDelta > 2)
                {
                    IsMouseMode = true;
                    Cursor.visible = CanShowMouseCursor;
                }
            }
        }
    }

    protected virtual bool CanShowMouseCursor => true;

    public virtual void OnEscape() { }
    public virtual void OnResume() { }

    void OnEnable()
    {
        InputActions.Enable();
    }

    void OnDisable()
    {
        InputActions.Disable();
    }
}