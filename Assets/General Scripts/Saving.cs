using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Saving : MonoBehaviour
{
    public SceneAsset[] levels;

    public class Data
    {
        public int currentLevel = 0;
    }

    public static Data saveData;
    
    public static string Path() { return Application.persistentDataPath + "/save.json"; }

    private static bool startedGame = false;

    public static void ReadSave()
    {
        if (!File.Exists(Path()))
        {
            saveData = new Data();
            WriteSave();
            return;
        }

        saveData = JsonUtility.FromJson<Data>(File.ReadAllText(Path()));
    }

    public static void WriteSave()
    {
        File.WriteAllText(Path(), JsonUtility.ToJson(saveData));
    }

    private void Awake()
    {
        if (startedGame) { return; }

        ReadSave();
        SceneManager.LoadScene(levels[Mathf.Min(saveData.currentLevel, levels.Length - 1)].name);
        startedGame = true;
    }
}
