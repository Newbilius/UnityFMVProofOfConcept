using System;
using UnityEngine;
using UnityEngine.UI;

//todo добавить анимацию изменения размера вверх-вниз?
//todo нужно сделать так, что изменения в редакторе тут же перерисовывались
public class ColorTextButton : Button
{
    private Text text;

    [SerializeField] public int NormalFontSize = 60;
    [SerializeField] public int SelectedFontSize = 80;
    [SerializeField] public Color TextColor = Color.yellow;

    public event Action<int> FontSizeChanged;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (text == null)
        {
            text = GetComponentInChildren<Text>();
            text.color = TextColor;
        }

        switch (state)
        {
            case SelectionState.Normal:
                text.fontSize = NormalFontSize;
                FontSizeChanged?.Invoke(NormalFontSize);
                break;

            case SelectionState.Highlighted:
                Select();
                break;

            case SelectionState.Selected:
                text.fontSize = SelectedFontSize;
                FontSizeChanged?.Invoke(SelectedFontSize);
                break;
        }
    }
}