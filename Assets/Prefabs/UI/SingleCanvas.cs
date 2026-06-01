using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class SingleCanvas : Persistent
{
    public static Canvas canvas;

    protected override void Awake()
    {
        if (!canvas) { canvas = GetComponent<Canvas>(); }
        else { Destroy(gameObject); return; }
        
        base.Awake();
    }
}
