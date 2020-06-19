using System;

public class Scene
{
    public string FileName { get; }
    public SceneChoice[] Choices { get; }
    public Action<Scene> OnCompleteAction { get; }

    public Scene(string fileName,
        SceneChoice[] choices,
        Action<Scene> onCompleteAction = null)
    {
        FileName = fileName;
        Choices = choices;
        OnCompleteAction = onCompleteAction;
    }
}