using UnityEngine;

public static class PlayerPrefsHelper
{
    public static bool GetBoolFromConfig(string key, bool defaultValue)
    {
        return PlayerPrefs.GetInt(key, 1).IntToBool();
    }

    public static void SaveBooleanValue(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value.BoolToInt());
    }

    private static bool IntToBool(this int value)
    {
        return value == 1;
    }

    private static int BoolToInt(this bool value)
    {
        return value ? 1 : 0;
    }
}