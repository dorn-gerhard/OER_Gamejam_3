using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;


public class Highscore : MonoBehaviour
{
    static string FilePath => Path.Combine(UnityEngine.Application.dataPath, "Highscores.json");
    [SerializeField] TMP_Text HighscorePlayers;
    [SerializeField] TMP_Text HighscoreScores;

    // Start is called before the first frame update
    void Start()
    {
        List<Entry> highscores = GetHighscores();

        HighscorePlayers.text = "";
        HighscoreScores.text = "";

        foreach (var highscore in highscores)
        {
            HighscorePlayers.text += highscore.Name + "\n";
            HighscoreScores.text += highscore.Score + "\n";
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
