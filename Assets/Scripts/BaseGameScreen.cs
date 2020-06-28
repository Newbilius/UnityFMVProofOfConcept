using UnityEngine;
using UnityEngine.InputSystem;

public class BaseGameScreen : MonoBehaviour
{
    public InputActions InputActions;
    protected bool IsMouseMode = true;
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

    public virtual void AnyKeyPressed()
    {
        IsMouseMode = false;
        Cursor.visible = false;
    }

    void OnGUI()
    {
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

    public virtual void OnEscape()
    {
    }

    void OnEnable()
    {
        InputActions.Enable();
    }

    void OnDisable()
    {
        InputActions.Disable();
    }
}