using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cameraComponent;
    private Vector3 initialAngles;

    private void Awake()
    {
        initialAngles = transform.eulerAngles;
    }

    private void Start()
    {
        cameraComponent = FindAnyObjectByType<Camera>();
        if (!cameraComponent) { Debug.LogWarning("Camera not found!"); }
    }

    private void Look()
    {
        if (!cameraComponent) { return; }
        transform.LookAt(cameraComponent.transform);
        transform.rotation *= Quaternion.Euler(initialAngles);
        transform.rotation *= Quaternion.Euler(0, 180, 0);
    }

    private void OnEnable()
    {
        Look();
    }

    private void Update()
    {
        Look();
    }
}
