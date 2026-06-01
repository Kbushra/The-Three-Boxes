using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private Player player;
    private Camera cameraComponent;
    [SerializeField] private Collider colliderComponent;
    [SerializeField] private GameObject interactPrompt;

    [SerializeField] private UnityEvent onInteract;
    
    protected virtual void Awake()
    {
        bool errored = false;

        if (!colliderComponent)
        {
            Debug.LogError("Invalid interactable fields! Please add the collider.");
            errored = true;
        }

        if (!interactPrompt)
        {
            Debug.LogError("Invalid interactable fields! Please add the interact object.");
            errored = true;
        }

        if (onInteract == null) { Debug.LogWarning("Missing interact event!"); }

        if (errored) { Destroy(this); return; }
    }

    protected virtual void Start()
    {
        player = FindAnyObjectByType<Player>();
        cameraComponent = FindAnyObjectByType<Camera>();
        if (!player) { Debug.LogWarning("Player not found!"); }
        if (!cameraComponent) { Debug.LogWarning("Camera not found!"); }
    }

    protected void DetectInteract()
    {
        bool interacted = player.inputs.FindAction("General/Interact").WasPressedThisFrame();
        if (interacted) { onInteract?.Invoke(); }
    }

    protected virtual void Update()
    {
        if (!player || !cameraComponent || player.state != Player.State.Normal) { return; }

        Physics.Raycast(cameraComponent.transform.position, cameraComponent.transform.forward, out RaycastHit hit, 3);
        interactPrompt.SetActive(hit.collider == colliderComponent);
        if (interactPrompt.activeSelf) { DetectInteract(); }
    }
}
