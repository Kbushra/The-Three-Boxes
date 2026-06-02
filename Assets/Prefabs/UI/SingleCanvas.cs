using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class SingleCanvas : MonoBehaviour
{
    public static Canvas canvas;

    private void Awake()
    {
        if (!canvas) { canvas = GetComponent<Canvas>(); }
        else { Destroy(gameObject); return; }
    }
}
