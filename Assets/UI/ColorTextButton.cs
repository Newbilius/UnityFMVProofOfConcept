using UnityEngine;
using UnityEngine.UI;

public class ColorTextButton : Button
{
    private Text text;

    [SerializeField] public int NormalFontSize = 60;

    [SerializeField] public int SelectedFontSize = 80;

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
                break;

            case SelectionState.Highlighted:
                Select();
                break;

            case SelectionState.Selected:
                text.fontSize = SelectedFontSize;
                break;
        }
    }
}