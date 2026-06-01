using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitManager : MonoBehaviour
{
    public SceneAsset targetScene;

    void Awake()
    {
        bool errored = false;

        if (!targetScene)
        {
            Debug.LogError("Invalid scene init manager fields! Please add the target scene.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }
    }

    void Start()
    {
        if (!targetScene) { return; }
        SceneManager.LoadScene(targetScene.name);
    }
}
