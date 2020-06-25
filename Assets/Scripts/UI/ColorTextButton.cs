using System;
using UnityEngine;
using UnityEngine.UI;

//todo нужно сделать так, что изменения в редакторе тут же перерисовывались
public class ColorTextButton : Button
{
    public Text Text { get; private set; }

    [SerializeField] public int NormalFontSize = 60;
    [SerializeField] public int SelectedFontSize = 80;

    public event Action<int> FontSizeChanged;

    protected override void Awake()
    {
        base.Start();
        Text = GetComponentInChildren<Text>();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        switch (state)
        {
            case SelectionState.Normal:
                Text.fontSize = NormalFontSize;
                FontSizeChanged?.Invoke(NormalFontSize);
                break;

            case SelectionState.Highlighted:
                Select();
                break;

            case SelectionState.Selected:
                Text.fontSize = SelectedFontSize;
                FontSizeChanged?.Invoke(SelectedFontSize);
                break;
        }
    }
}