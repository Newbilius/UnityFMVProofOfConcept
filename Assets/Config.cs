public static class Config
{
    private static string SubtitlesOnKey = "SubtitlesOn";

    private static bool? subtitlesOn;

    public static bool SubtitlesOn
    {
        get
        {
            if (!subtitlesOn.HasValue)
                subtitlesOn = PlayerPrefsHelper.GetBoolFromConfig(SubtitlesOnKey, true);
            return subtitlesOn.Value;
        }
        set
        {
            PlayerPrefsHelper.SaveBooleanValue(SubtitlesOnKey, value);
            subtitlesOn = value;
        }
    }
}