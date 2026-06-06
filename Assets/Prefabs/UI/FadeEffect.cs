using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Image))]
public class FadeEffect : MonoBehaviour
{
    private Image imageComponent;

    public bool fadingOut = false;
    public float spd = 1;
    public float hold = 0;

    protected virtual void Awake()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0);
    }

    protected virtual void Update()
    {
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b,
            imageComponent.color.a + (fadingOut ? -1 : 1) * Time.deltaTime * spd);

        if (!fadingOut && imageComponent.color.a >= 1)
        {
            hold -= Time.deltaTime;
            if (hold <= 0) { fadingOut = true; }
        }
        else if (fadingOut && imageComponent.color.a <= 0) { Destroy(gameObject); }
    }
}
