using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private Camera cameraComponent;
    [SerializeField] private Collider colliderComponent;
    [SerializeField] private Renderer[] renderComponents = new Renderer[0];

    [SerializeField] private UnityEvent<Renderer> onHover;
    [SerializeField] private UnityEvent onInteract;
    public bool interactable = false;
    
    protected virtual void Awake()
    {
        bool errored = false;

        if (!colliderComponent)
        {
            Debug.LogError("Invalid interactable fields! Please add the collider.");
            errored = true;
        }

        if (renderComponents.Length == 0)
        {
            Debug.LogError("Invalid interactable fields! Please add renderers.");
            errored = true;
        }

        if (onHover == null) { Debug.LogWarning("Missing hover event!"); }
        if (onInteract == null) { Debug.LogWarning("Missing interact event!"); }

        if (errored) { Destroy(this); return; }
    }

    protected virtual void Start()
    {
        cameraComponent = FindAnyObjectByType<Camera>();
        if (!Player.player) { Debug.LogWarning("Player not found!"); }
        if (!cameraComponent) { Debug.LogWarning("Camera not found!"); }
    }

    protected void DetectInteract()
    {
        bool interacted = Player.inputs.FindAction("General/Interact").WasPressedThisFrame();
        if (interacted) { onInteract?.Invoke(); }
    }

    public void HoverURPLit(Renderer renderComponent)
    {
        float emission = interactable ? 0.05f : 0;
        renderComponent.material.EnableKeyword("_EMISSION");
        renderComponent.material.SetColor("_EmissionColor", new Color(emission, emission, emission));
    }

    public void HoverSprite(Renderer renderComponent)
    {
        float tint = interactable ? 1 : 0.75f;
        renderComponent.material.SetColor("_Color", new Color(tint, tint, tint));
    }

    protected virtual void Update()
    {
        if (!Player.player || !cameraComponent || Player.state != Player.State.Normal) { return; }

        Physics.Raycast(cameraComponent.transform.position, cameraComponent.transform.forward, out RaycastHit hit, 3);
        interactable = hit.collider == colliderComponent;
        
        foreach (Renderer renderComponent in renderComponents)
        {
            onHover?.Invoke(renderComponent);
        }
        
        if (interactable) { DetectInteract(); }
    }
}
