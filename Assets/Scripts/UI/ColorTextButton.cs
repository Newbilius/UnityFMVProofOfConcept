using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ColorTextButton : Button
{
    public string Text
    {
        get => TextComponent.text;
        set => TextComponent.text = value;
    }

    private Text TextComponent { get; set; }

    protected override void Awake()
    {
        base.Awake();
        TextComponent = GetComponentInChildren<Text>();
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
        var mouse = Mouse.current;
        if (mouse != null)
        {
            var mouseValue = mouse.delta.ReadValue();
            var mouseDelta = Mathf.Abs(mouseValue.x) + Mathf.Abs(mouseValue.y);
            if (mouseDelta > 2 && IsPointerInside)
                Select();
        }
    }
}