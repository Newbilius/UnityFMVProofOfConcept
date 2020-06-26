using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//todo нужно сделать так, что изменения в редакторе тут же перерисовывались
public class ColorTextButton : Button
{
    public Text Text { get; private set; }

    [SerializeField] public int NormalFontSize = 60;
    [SerializeField] public int SelectedFontSize = 80;

    protected override void Awake()
    {
        base.Start();
        Text = GetComponentInChildren<Text>();
    }

    private bool IsPointerInside { get; set; }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Select();
        IsPointerInside = true;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        IsPointerInside = false;
    }

    void OnGUI()
    {
        var mouseValue = Mouse.current.delta.ReadValue();
        var mouseDelta = Mathf.Abs(mouseValue.x) + Mathf.Abs(mouseValue.y);
        if (mouseDelta > 2 && IsPointerInside)
            Select();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        switch (state)
        {
            case SelectionState.Pressed:
            case SelectionState.Selected:
                Text.fontSize = SelectedFontSize;
                break;

            default:
                Text.fontSize = NormalFontSize;
                break;
        }
    }
}