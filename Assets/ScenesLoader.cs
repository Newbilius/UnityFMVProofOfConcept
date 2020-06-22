using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ScenesLoader
{
    readonly Regex nameIdRegex = new Regex("\\((.*?)\\)", RegexOptions.Compiled);

    public Dictionary<int, Scene> Init(string jsonData, out int startSceneId)
    {
        var dictionary = new Dictionary<int, Scene>(0);

        var data = JsonUtility.FromJson<TwineJson>(jsonData);
        startSceneId = data.startnode;

        foreach (var sceneData in data.passages)
        {
            var textData = sceneData.text.Split('\n');

            string musicNameOnStart = null;
            string musicNameOnEnd = null;

            foreach (var textDataLine in textData)
            {
                if (textDataLine.Contains("$$"))
                    musicNameOnEnd = textDataLine.Replace("$$", "");
                else if (textDataLine.Contains("$"))
                    musicNameOnStart = textDataLine.Replace("$", "");
            }

            var sceneId = "";
            var result = nameIdRegex.Matches(sceneData.name);
            if (result.Count > 0 && result[0].Groups.Count > 0)
                sceneId = result[0].Groups[1].Value;

            var scene = new Scene(textData[0],
                sceneId,
                sceneData.links.Select(x => new SceneChoice
                {
                    Caption = x.name,
                    SceneId = x.pid
                }).ToArray(),
                musicNameOnStart,
                musicNameOnEnd);

            dictionary.Add(sceneData.pid, scene);
        }

        return dictionary;
    }
}