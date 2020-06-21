using System;

public class Scene
{
    public string FileName { get; }
    public SceneChoice[] Choices { get; }
    public Action<Scene> OnCompleteAction { get; }

    public string MusicNameOnStart { get; }
    public string MusicNameOnEnd { get;}

    public Scene(string fileName,
        SceneChoice[] choices,
        Action<Scene> onCompleteAction = null,
        string musicNameOnStart = null,
        string musicNameOnEnd = null)
    {
        MusicNameOnStart = musicNameOnStart;
        MusicNameOnEnd = musicNameOnEnd;
        FileName = fileName;
        Choices = choices;
        OnCompleteAction = onCompleteAction;
    }
}