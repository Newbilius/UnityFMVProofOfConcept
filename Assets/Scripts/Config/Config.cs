using UnityEngine;

public static class Config
{
    private static string SubtitlesOnKey = "SubtitlesOn";
    private static string SelectedResolutionKey = "SelectedResolution";
    private static string IsFullscreenKey = "Fullscreen";

    private static bool? subtitlesOn;
    private static bool? isFullscreenKey;

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

    public static bool IsFullscreen
    {
        get
        {
            if (!isFullscreenKey.HasValue)
                isFullscreenKey = PlayerPrefsHelper.GetBoolFromConfig(IsFullscreenKey, true);
            return isFullscreenKey.Value;
        }
        set
        {
            PlayerPrefsHelper.SaveBooleanValue(IsFullscreenKey, value);
            isFullscreenKey = value;
        }
    }

    public static SelectedResolution Resolution
    {
        get
        {
            var selectedResolutionString = PlayerPrefs.GetString(SelectedResolutionKey);
            return string.IsNullOrEmpty(selectedResolutionString)
                ? null
                : JsonUtility.FromJson<SelectedResolution>(selectedResolutionString);
        }
        set => PlayerPrefs.SetString(SelectedResolutionKey, JsonUtility.ToJson(value));
    }

    [System.Serializable]
    public class SelectedResolution
    {
        public int Width;
        public int Height;

        protected bool Equals(SelectedResolution other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SelectedResolution) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Width * 397) ^ Height;
            }
        }
    }
}