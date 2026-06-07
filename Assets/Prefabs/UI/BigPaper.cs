using UnityEngine;
using HelperFunctions;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Animations;

[RequireComponent(typeof(Image))]
public class BigPaper : MonoBehaviour
{
    private Image imageComponent;
    [SerializeField] private TextMeshProUGUI textComponent;
    private float targAlpha = 1;
    private float targZRot = 0;
    private float targY = 0;

    public bool fading = false;

    private void Awake()
    {
        bool errored = false;

        if (!textComponent)
        {
            Debug.LogError("Invalid UI paper fields! Please add the text element.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }

        imageComponent = GetComponent<Image>();
        imageComponent.color = new Color(1, 1, 1, 0);
        textComponent.color = new Color(0, 0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 15);
        transform.localPosition = new Vector3(0, -500, 0);
    }

    private void Start()
    {
        if (!Player.player) { Debug.LogWarning("Player not found!"); }
    }

    private void Update()
    {
        imageComponent.color = new Color(1, 1, 1, Maths.LerpDelta(imageComponent.color.a, targAlpha, 0.98f));
        textComponent.color = new Color(0, 0, 0, imageComponent.color.a);
        transform.localRotation = Quaternion.Euler(0, 0, Maths.LerpAngleDelta(transform.localEulerAngles.z, targZRot, 0.9f));
        transform.localPosition = new Vector3(0, Maths.LerpDelta(transform.localPosition.y, targY, 0.9f), 0);

        bool interacted = Player.player ? Player.inputs.FindAction("General/Interact").WasPressedThisFrame() : false;
        if (imageComponent.color.a >= 0.99f && !fading && interacted)
        {
            fading = true;
            targAlpha = 0;
            targZRot = -15;
            targY = 500;
        }

        if (!fading && Player.player) { Player.stateQueue.Add(Player.State.Locked); }
        if (imageComponent.color.a <= 0.01f && fading) { Destroy(gameObject); }
    }
}
