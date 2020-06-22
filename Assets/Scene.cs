public class Scene
{
    public string FileName { get; }
    public string SceneId { get; }
    public SceneChoice[] Choices { get; set; }

    public string MusicNameOnStart { get; }
    public string MusicNameOnEnd { get;}

    public Scene(string fileName,
        string sceneId,
        SceneChoice[] choices,
        string musicNameOnStart = null,
        string musicNameOnEnd = null)
    {
        FileName = fileName;
        Choices = choices;
        SceneId = sceneId;
        MusicNameOnStart = musicNameOnStart;
        MusicNameOnEnd = musicNameOnEnd;
    }
}