using System;

public class SceneChoice
{
    public string Caption { private get; set; }
    public Func<string> CaptionAction { get; set; }
    public SceneId SceneId { private get; set; }
    public Func<SceneId> SceneIdAction { get; set; }

    public string GetCaption()
    {
        return CaptionAction != null ? CaptionAction() : Caption;
    }

    public SceneId GetSceneId()
    {
        return SceneIdAction?.Invoke() ?? SceneId;
    }
}