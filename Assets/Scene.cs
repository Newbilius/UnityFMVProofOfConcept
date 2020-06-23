public class Scene
{
    public string FileName { get; }
    public string SceneCode { get; }
    public SceneChoice[] Choices { get; set; }

    public string MusicNameOnStart { get; }
    public string MusicNameOnEnd { get; }

    public Scene(string fileName,
        string sceneCode,
        SceneChoice[] choices,
        string musicNameOnStart = null,
        string musicNameOnEnd = null)
    {
        FileName = fileName;
        Choices = choices ?? new SceneChoice[0];
        SceneCode = sceneCode;
        MusicNameOnStart = musicNameOnStart;
        MusicNameOnEnd = musicNameOnEnd;
    }
}