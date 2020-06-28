using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ColorTextButton : Button
{
    private Text TextComponent { get; set; }

    public string Text
    {
        get => TextComponent?.text;
        set => TextComponent.text = value;
    }

    [SerializeField] public int NormalFontSize = 60;
    [SerializeField] public int SelectedFontSize = 80;

    protected override void Awake()
    {
        base.Awake();
        TextComponent = GetComponentInChildren<Text>();
        StartCoroutine("FontSizeAnimation");
    }

    private bool IsSelected;
    private const float AnimationSpeed = 0.01f;
    private const int StepValue = 2;

    IEnumerator FontSizeAnimation()
    {
        //возможно тут нужно указать какой-то призак того, что кнопка рисуется, что она не скрыта?
        while (true)
        {
            if (IsSelected && TextComponent.fontSize < SelectedFontSize)
                TextComponent.fontSize += StepValue;

            if (!IsSelected && TextComponent.fontSize > NormalFontSize)
                TextComponent.fontSize -= StepValue;

            yield return new WaitForSeconds(AnimationSpeed);
        }
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

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        switch (state)
        {
            case SelectionState.Pressed:
            case SelectionState.Selected:
                if (instant)
                    TextComponent.fontSize = SelectedFontSize;
                IsSelected = true;
                break;

            default:
                if (instant)
                    TextComponent.fontSize = NormalFontSize;
                IsSelected = false;
                break;
        }
    }
}