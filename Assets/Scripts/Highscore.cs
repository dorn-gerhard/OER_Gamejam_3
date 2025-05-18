using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class Highscore : MonoBehaviour
{
    static string FilePath => Path.Combine(UnityEngine.Application.persistentDataPath, Filename);
    [SerializeField] TMP_Text HighscorePlayers;
    [SerializeField] TMP_Text HighscoreScores;

    const string Filename = "Highscores.json";
    
    void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            WebGLFileLoader.Init(this, Filename);
    }

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

    public static void AddHighscore(string username, int score)
    {
        List<Entry> entries = GetHighscores();
        
        if (score != 0)
            entries.Add(new Entry(username, score));

        var json = JsonConvert.SerializeObject(entries);
        File.WriteAllText(FilePath, json);
    }

    public static List<Entry> GetHighscores()
    {
        var jsonFile = GetSaveFile();
        var entries = JsonConvert.DeserializeObject<List<Entry>>(jsonFile) ?? new();

        //sort highest to lowest
        entries.Sort((a, b) => (b.Score.CompareTo(a.Score)));
        return entries;
    }

    private static string GetSaveFile()
    {
        //local highscores
        if (File.Exists(FilePath))
            return File.ReadAllText((FilePath));

        //load pre-filled highscore for extra motivation
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            return WebGLFileLoader.GetContents();
        
        var fullPath = Path.Combine(Application.streamingAssetsPath, Filename);
        return File.ReadAllText(fullPath);
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

//for WebGL, files need to be streamed into, direct loading of files is not possible
public static class WebGLFileLoader
{
    private static string _cachedContents;
    private static bool _isLoaded;

    public static void Init(MonoBehaviour context, string filename)
    {
        if (_isLoaded) return;
        context.StartCoroutine(LoadFile(filename));
    }

    public static string GetContents()
    {
        return _isLoaded ? _cachedContents : "";
    }

    private static IEnumerator LoadFile(string filename)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filename);
        using UnityWebRequest uwr = UnityWebRequest.Get(path);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            _cachedContents = uwr.downloadHandler.text;
            _isLoaded = true;
        }
        else
        {
            Debug.LogError($"[WebGLFileLoader] Failed to load file: {uwr.error}");
        }
    }
}
