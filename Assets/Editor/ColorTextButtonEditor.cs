using UnityEditor;

[CustomEditor(typeof(ColorTextButton))]
public class ColorTextButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var colorTextButton = (ColorTextButton)target;

        serializedObject.Update();
        
        EditorGUI.BeginChangeCheck();
        colorTextButton.NormalFontSize = EditorGUILayout.IntField("Нормальный шрифт", colorTextButton.NormalFontSize);
        colorTextButton.SelectedFontSize = EditorGUILayout.IntField("Большой шрифт", colorTextButton.SelectedFontSize);

        if (EditorGUI.EndChangeCheck()) 
            EditorUtility.SetDirty(colorTextButton);

        serializedObject.ApplyModifiedProperties();
    }
}