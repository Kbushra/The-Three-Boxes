using HelperFunctions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI backText;
    [SerializeField] private TextMeshProUGUI toMainText;
    [SerializeField] private GameObject fadeEffect;

    private int selection;
    private Canvas canvas;

    private void Awake()
    {
        bool errored = false;

        if (!backText)
        {
            Debug.LogError("Invalid pause menu fields! Please add back text.");
            errored = true;
        }

        if (!toMainText)
        {
            Debug.LogError("Invalid pause menu fields! Please add to main text.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }
    }

    private void Start()
    {
        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }
    }

    private void Update()
    {
        Player.stateQueue.Add(Player.State.Locked);

        FadeEffect[] fadeEffects = FindObjectsByType<FadeEffect>();
        if (fadeEffects.Length > 0)
        {
            if (fadeEffects[0].transitioned) { Destroy(gameObject); }
            if (!fadeEffects[0].fadingOut) { return; }
        }

        if (Player.inputs.FindAction("General/Menu").WasPressedThisFrame()) { Destroy(gameObject); return; }

        bool up = Player.inputs.FindAction("General/Up").WasPressedThisFrame();
        bool down = Player.inputs.FindAction("General/Down").WasPressedThisFrame();
        Debug.Log($"{(up ? -1 : (down ? 1 : 0))} - {selection}");
        selection += up ? -1 : (down ? 1 : 0);
        selection = Maths.Mod(selection, 2);

        backText.text = selection == 0 ? "> BACK <" : "BACK";
        toMainText.text = selection == 1 ? "> MAIN MENU <" : "MAIN MENU";

        if (!Player.inputs.FindAction("General/Interact").WasPressedThisFrame()) { return; }

        if (selection == 0) { Destroy(gameObject); return; }
        if (selection == 1) { Player.spawnMainMenu = true; Instantiate(fadeEffect, canvas.transform); }
    }
}
