using HelperFunctions;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TiledMaterial : MonoBehaviour
{
    private Material materialComponent;

    void Awake()
    {
        Renderer rendererComponent = GetComponent<Renderer>();
        materialComponent = rendererComponent.material;
    }

    void Update()
    {
        Vector3 boundsSize = Quaternion.Inverse(transform.rotation) * Maths.BoundsSize(transform);
        materialComponent.mainTextureScale = new Vector2(boundsSize.x, boundsSize.z);
    }
}
