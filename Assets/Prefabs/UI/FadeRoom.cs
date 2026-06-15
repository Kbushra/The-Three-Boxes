using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Image))]
public class FadeRoom : FadeEffect
{
    public SceneAsset targetScene;
    public string targetSceneName;

    protected override void Update()
    {
        base.Update();
        if (fadingOut && transitioned)
        {
            SceneManager.LoadScene(targetScene ? targetScene.name : targetSceneName);
        }
    }
}
