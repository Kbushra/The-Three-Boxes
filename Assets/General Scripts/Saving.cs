using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Saving : MonoBehaviour
{
    public static string[] levels =
    {
        "SceneIntro",
        "SceneCrates",
        "SceneQuadrants",
        "SceneLayers",
        "SceneCandles",
        "SceneEnding"
    };

    public class Data
    {
        public int currentLevel = 0;
        public float sensitivity = 1;
    }

    public static Data saveData;
    
    public static string GetPath() { return Path.Combine(Application.persistentDataPath, "save.json"); }

    private static bool startedGame = false;

    public static void ReadSave()
    {
        if (!File.Exists(GetPath()))
        {
            saveData = new Data();
            WriteSave();
            return;
        }

        saveData = JsonUtility.FromJson<Data>(File.ReadAllText(GetPath()));
    }

    public static void WriteSave()
    {
        //Remember to do "autoSyncPersistentDataPath: true" for Web builds
        File.WriteAllText(GetPath(), JsonUtility.ToJson(saveData));
    }

    private void Awake()
    {
        if (startedGame) { return; }

        ReadSave();
        SceneManager.LoadScene(levels[Mathf.Min(saveData.currentLevel, levels.Length - 1)]);
        startedGame = true;
    }
}
