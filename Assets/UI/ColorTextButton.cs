using System;
using UnityEngine;
using UnityEngine.UI;

//todo цвет тоже давать возможность менять из настроек
//todo добавить анимацию изменения размера вверх-вниз?
public class ColorTextButton : Button
{
    private Text text;

    [SerializeField] public int NormalFontSize = 60;

    [SerializeField] public int SelectedFontSize = 80;

    public event Action<int> FontSizeChanged;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (text == null)
        {
            text = GetComponentInChildren<Text>();
            text.color = Color.yellow;
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