using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

public static class Highscore
{
    static string FilePath => Path.Combine(UnityEngine.Application.dataPath, "Highscores.json");

    public static void AddHighscore(string username, int score)
    {
        List<Entry> entries = GetHighscores();
        entries.Add(new Entry(username, score));

        var json = JsonConvert.SerializeObject(entries);
        File.WriteAllText(FilePath, json);
    }

    public static List<Entry> GetHighscores()
    {
        string path = FilePath;
        var jsonFile = File.Exists(path) ? File.ReadAllText(path) : "";

        var entries = JsonConvert.DeserializeObject<List<Entry>>(jsonFile) ?? new();

        //sort highest to lowest
        entries.Sort((a, b) => (b.Score.CompareTo(a.Score)));
        return entries;
    }

    [Serializable]
    public record Entry(string Name, int Score);
}

//fuck Unity's outdated C# version
namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}
