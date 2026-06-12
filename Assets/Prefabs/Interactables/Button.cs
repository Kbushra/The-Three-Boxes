using HelperFunctions;
using UnityEngine;
using UnityEngine.Events;

public class Button : Interactable
{
    private Vector3 start;
    private float offset = 0;

    public float currentHoldLength = 0;
    public float fullHoldLength = 0;

    protected override void Awake()
    {
        base.Awake();
        start = transform.position;
    }

    public void HoverButton(Renderer renderComponent)
    {
        bool interacted = Player.inputs.FindAction("General/Interact").IsPressed();
        float emission = interactable ? (interacted ? 2 : 0.15f) : 0;

        renderComponent.material.EnableKeyword("_EMISSION");
        renderComponent.material.SetColor("_EmissionColor", new Color(emission, 0, 0));
    }

    protected override void Update()
    {
        if (!Player.player || !cameraComponent || Player.state != Player.State.Normal) { return; }

        DetectHover();

        bool interacted = false;
        if (interactable) { interacted = Player.inputs.FindAction("General/Interact").IsPressed(); }

        if (interacted) { currentHoldLength += Time.deltaTime; }
        else
        {
            if (currentHoldLength > 0) { fullHoldLength = currentHoldLength; }
            currentHoldLength = 0;
        }

        offset = Maths.LerpDelta(offset, interacted ? -0.1f : 0, 0.9998f);
        Vector3 targOffset = transform.rotation * new Vector3(0, offset, 0);
        transform.position = start + targOffset;
    }
}
