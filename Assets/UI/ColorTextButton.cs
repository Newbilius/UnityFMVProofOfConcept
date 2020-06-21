using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ColorTextButton : Button
{
    private Text text;

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
                text.fontSize = 60;
                break;

            case SelectionState.Highlighted:
                Select();
                break;

            case SelectionState.Selected:
                text.fontSize = 78;
                break;
        }
    }
}