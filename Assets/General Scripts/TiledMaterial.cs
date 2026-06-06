using HelperFunctions;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(Renderer))]
public class TiledMaterial : MonoBehaviour
{
    enum Axis { X, Y, Z };

    private Material materialComponent;

    [SerializeField] private bool useTransform = true;

    [ShowIf(nameof(useTransform))]
    [SerializeField] private Axis excludeAxis = Axis.Y;
    [ShowIf(nameof(useTransform))]
    [SerializeField] private float transformScale = 1;

    [HideIf(nameof(useTransform))]
    [SerializeField] private float xscale = 1;
    [HideIf(nameof(useTransform))]
    [SerializeField] private float yscale = 1;

    void Awake()
    {
        Renderer rendererComponent = GetComponent<Renderer>();
        materialComponent = rendererComponent.material;
    }

    void Update()
    {
        Vector2 texSize = new Vector2(xscale, yscale);

        if (useTransform)
        {
            Vector3 scale = transform.lossyScale;
            switch (excludeAxis)
            {
                case Axis.X: scale.x = 0; break;
                case Axis.Y: scale.y = 0; break;
                case Axis.Z: scale.z = 0; break;
            }

            scale = Vector3.Scale(Maths.MeshSize(transform) * transformScale, scale);

            switch (excludeAxis)
            {
                case Axis.X: texSize = new Vector2(scale.y, scale.z); break;
                case Axis.Y: texSize = new Vector2(scale.x, scale.z); break;
                case Axis.Z: texSize = new Vector2(scale.x, scale.y); break;
            }
        }

        materialComponent.mainTextureScale = texSize;
    }
}
