using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Image))]
public class FadeRoom : FadeEffect
{
    public SceneAsset targetScene;
    private bool transitioned = false;

    protected override void Update()
    {
        base.Update();
        if (fadingOut && !transitioned)
        {
            SceneManager.LoadScene(targetScene.name);
            transitioned = true;
        }
    }
}
