using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Image))]
public class FadeRoom : FadeEffect
{
    public string targetSceneName;
    private bool transitioned = false;
    public static GameObject fader;

    protected override void Update()
    {
        base.Update();
        if (fadingOut && !transitioned)
        {
            SceneManager.LoadScene(targetSceneName);
            transitioned = true;
        }
    }

    public static FadeRoom Fade()
    {
        if (!fader || !SingleCanvas.canvas.transform) { return null; }

        FadeRoom[] faders = FindObjectsByType<FadeRoom>();
        for (int i = 0; i < faders.Length; i++)
        {
            if (i > 0) { Destroy(faders[i].gameObject); }
        }

        FadeRoom faderInstance = faders.Length > 0 ? faders[0] : Instantiate(fader, SingleCanvas.canvas.transform).GetComponent<FadeRoom>();
        return faderInstance;
    }
}
