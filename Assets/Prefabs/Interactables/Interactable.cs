using System;
using UnityEngine;
using HelperFunctions;
using System.Linq;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Camera cameraComponent;
    [SerializeField] private Collider colliderComponent;
    [SerializeField] private GameObject interactPrompt;

    [SerializeField] private UnityEvent onInteract;
    
    protected void DetectInteract()
    {
        bool interacted = Player.inputs.FindAction("General/Interact").WasPressedThisFrame();
        if (interacted) { onInteract?.Invoke(); }
    }

    protected virtual void Update()
    {
        if (Player.state != Player.State.Normal) { return; }

        Physics.Raycast(cameraComponent.transform.position, cameraComponent.transform.forward, out RaycastHit hit, 3);
        interactPrompt.SetActive(hit.collider == colliderComponent);
        if (interactPrompt.activeSelf) { DetectInteract(); }
    }
}
