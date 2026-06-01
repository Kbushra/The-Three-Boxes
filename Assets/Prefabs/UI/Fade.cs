using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Image))]
public class Fade : MonoBehaviour
{
    private Image imageComponent;
    private bool fadingOut = false;

    public SceneAsset targetScene;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.color = new Color(1, 1, 1, 0);
    }

    private void Update()
    {
        imageComponent.color = new Color(1, 1, 1, imageComponent.color.a + (fadingOut ? -1 : 1) * Time.deltaTime / 2);

        if (!fadingOut && imageComponent.color.a >= 1) { fadingOut = true; SceneManager.LoadScene(targetScene.name); }
        else if (fadingOut && imageComponent.color.a <= 0) { Destroy(gameObject); }
    }
}
