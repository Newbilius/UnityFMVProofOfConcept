using System.Collections.Generic;

public static class GameplayStatisticss
{
    public static int AllScenesCount;
    public static List<int> ViewedScenes = new List<int>();

    public static int ChoicesCount;

    public static void Clear(int allScenesCount)
    {
        ChoicesCount = 0;
        AllScenesCount = allScenesCount;
        ViewedScenes.Clear();
    }
}