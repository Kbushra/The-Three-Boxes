using HelperFunctions;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TiledMaterial : MonoBehaviour
{
    private Material materialComponent;
    [SerializeField] private bool useTransform = true;
    [SerializeField] private float transformScale = 1;
    [SerializeField] private float xscale = 1;
    [SerializeField] private float yscale = 1;

    void Awake()
    {
        Renderer rendererComponent = GetComponent<Renderer>();
        materialComponent = rendererComponent.material;
    }

    void Update()
    {
        Vector3 boundsSize = new Vector3(xscale, 0, yscale);
        if (useTransform) { boundsSize = Vector3.Scale(Maths.MeshSize(transform) * transformScale, transform.lossyScale); }
        materialComponent.mainTextureScale = new Vector2(boundsSize.x, boundsSize.z);
        Debug.Log(boundsSize);
    }
}
