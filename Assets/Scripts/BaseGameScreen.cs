using UnityEngine;

public class BaseGameScreen : MonoBehaviour
{
    public InputActions InputActions;

    void Awake()
    {
        InputActions = new InputActions();
        InputActions.Main.Escape.performed += _ => OnEscape();
    }

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