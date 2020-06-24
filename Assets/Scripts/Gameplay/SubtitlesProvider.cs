//взято отсюда и немного модифицировано https://github.com/roguecode/Unity-Simple-SRT/blob/master/src/Assets/SimpleSRT/SRTParser.cs

using System;
using System.Collections.Generic;

public class SubtitlesProvider
{
    private List<SubtitleBlock> subtitles = new List<SubtitleBlock>(0);

    public void Load(string text)
    {
        subtitles = LoadData(text);
    }

    private List<SubtitleBlock> LoadData(string text)
    {
        var lines = text.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

        var currentState = SubtitleDataReadState.Index;

        var subs = new List<SubtitleBlock>();

        int currentIndex = 0;
        double currentFrom = 0, currentTo = 0;
        var currentText = string.Empty;
        for (var l = 0; l < lines.Length; l++)
        {
            var line = lines[l];

            switch (currentState)
            {
                case SubtitleDataReadState.Index:
                {
                    if (int.TryParse(line, out var index))
                    {
                        currentIndex = index;
                        currentState = SubtitleDataReadState.Time;
                    }
                }
                    break;
                case SubtitleDataReadState.Time:
                {
                    line = line.Replace(',', '.');
                    var parts = line.Split(new[] {"-->"}, StringSplitOptions.RemoveEmptyEntries);

                    // Parse the timestamps
                    if (parts.Length == 2)
                    {
                        if (TimeSpan.TryParse(parts[0], out var fromTime))
                        {
                            if (TimeSpan.TryParse(parts[1], out var toTime))
                            {
                                currentFrom = fromTime.TotalSeconds;
                                currentTo = toTime.TotalSeconds;
                                currentState = SubtitleDataReadState.Text;
                            }
                        }
                    }
                }
                    break;
                case SubtitleDataReadState.Text:
                {
                    if (currentText != string.Empty)
                        currentText += "\r\n";

                    currentText += line;

                    // When we hit an empty line, consider it the end of the text
                    if (string.IsNullOrEmpty(line) || l == lines.Length - 1)
                    {
                        // Create the SubtitleBlock with the data we've aquired 
                        subs.Add(new SubtitleBlock(currentIndex, currentFrom, currentTo, currentText));

                        // Reset stuff so we can start again for the next block
                        currentText = string.Empty;
                        currentState = SubtitleDataReadState.Index;
                    }
                }
                    break;
            }
        }

        return subs;
    }

    public SubtitleBlock GetForTime(double time)
    {
        if (subtitles.Count > 0)
        {
            var subtitle = subtitles[0];

            if (time >= subtitle.To)
            {
                subtitles.RemoveAt(0);

                if (subtitles.Count == 0)
                    return null;

                subtitle = subtitles[0];
            }

            return subtitle.From > time ? null : subtitle;
        }

        return null;
    }

    enum SubtitleDataReadState
    {
        Index,
        Time,
        Text
    }

    public void Clear()
    {
        subtitles.Clear();
    }
}

public class SubtitleBlock
{
    public int Index { get; }
    public double Length { get; }
    public double From { get; }
    public double To { get; }
    public string Text { get; }

    public SubtitleBlock(int index, double from, double to, string text)
    {
        Index = index;
        From = from;
        To = to;
        Length = to - from;
        Text = text;
    }
}