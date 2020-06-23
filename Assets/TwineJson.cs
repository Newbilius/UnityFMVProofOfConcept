[System.Serializable]
public class TwineJson
{
    public int startnode;
    public TwineJsonNode[] passages;
}

[System.Serializable]
public class TwineJsonNode
{
    public int pid;
    public string text;
    public string name;
    public TwineJsonNodeLink[] links;
}

[System.Serializable]
public class TwineJsonNodeLink
{
    public int pid;
    public string name;
    public string link;
}