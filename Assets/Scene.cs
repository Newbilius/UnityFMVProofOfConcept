public class Scene
{
    public string FileName { get; set; }
    public SceneChoice[] Choices { get; set; }

    public Scene(string fileName, SceneChoice[] choices)
    {
        FileName = fileName;
        Choices = choices;
    }
}