﻿//взято отсюда и немного модифицировано https://github.com/roguecode/Unity-Simple-SRT/blob/master/src/Assets/SimpleSRT/SRTParser.cs

using System;
using System.Collections.Generic;

public class SubtitlesProvider
{
    private List<SubtitleBlock> _subtitles = new List<SubtitleBlock>(0);

    public void Load(string text)
    {
        _subtitles = LoadData(text);
    }

    private List<SubtitleBlock> LoadData(string text)
    {
        var lines = text.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

        var currentState = eReadState.Index;

        var subs = new List<SubtitleBlock>();

        int currentIndex = 0;
        double currentFrom = 0, currentTo = 0;
        var currentText = string.Empty;
        for (var l = 0; l < lines.Length; l++)
        {
            var line = lines[l];

            switch (currentState)
            {
                case eReadState.Index:
                {
                    int index;
                    if (Int32.TryParse(line, out index))
                    {
                        currentIndex = index;
                        currentState = eReadState.Time;
                    }
                }
                    break;
                case eReadState.Time:
                {
                    line = line.Replace(',', '.');
                    var parts = line.Split(new[] {"-->"}, StringSplitOptions.RemoveEmptyEntries);

                    // Parse the timestamps
                    if (parts.Length == 2)
                    {
                        TimeSpan fromTime;
                        if (TimeSpan.TryParse(parts[0], out fromTime))
                        {
                            TimeSpan toTime;
                            if (TimeSpan.TryParse(parts[1], out toTime))
                            {
                                currentFrom = fromTime.TotalSeconds;
                                currentTo = toTime.TotalSeconds;
                                currentState = eReadState.Text;
                            }
                        }
                    }
                }
                    break;
                case eReadState.Text:
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
                        currentState = eReadState.Index;
                    }
                }
                    break;
            }
        }

        return subs;
    }

    public SubtitleBlock GetForTime(float time)
    {
        if (_subtitles.Count > 0)
        {
            var subtitle = _subtitles[0];

            if (time >= subtitle.To)
            {
                _subtitles.RemoveAt(0);

                if (_subtitles.Count == 0)
                    return null;

                subtitle = _subtitles[0];
            }

            if (subtitle.From > time)
                return null;

            return subtitle;
        }

        return null;
    }

    enum eReadState
    {
        Index,
        Time,
        Text
    }

    public void Clear()
    {
        _subtitles.Clear();
    }
}

public class SubtitleBlock
{
    static SubtitleBlock _blank;

    public static SubtitleBlock Blank => _blank ?? (_blank = new SubtitleBlock(0, 0, 0, string.Empty));

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