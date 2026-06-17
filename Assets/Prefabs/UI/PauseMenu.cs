using HelperFunctions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI backText;
    [SerializeField] private TextMeshProUGUI toMainText;

    private int selection = 0;
    private Canvas canvas;
    private FadeRoom faderInstance;

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

    private void Select(TextMeshProUGUI mesh, string text, int reqSelection, bool instant)
    {
        mesh.text = selection == reqSelection ? $"> {text} <" : text;
        mesh.fontSize = Maths.LerpDelta(mesh.fontSize, selection == reqSelection ? 36 : 32, instant ? 1 : 0.995f);
    }

    private void SelectAll(bool instant)
    {
        Select(backText, "BACK", 0, instant);
        Select(toMainText, "MAIN MENU", 1, instant);
    }

    private void Start()
    {
        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }

        SelectAll(true);
    }

    private void Update()
    {
        Player.stateQueue.Add(Player.State.Locked);
        if (Player.state == Player.State.MainMenu) { Destroy(gameObject); return; }

        if (faderInstance && !faderInstance.fadingOut) { SelectAll(false); return; }

        bool up = Player.inputs.FindAction("General/Up").WasPressedThisFrame();
        bool down = Player.inputs.FindAction("General/Down").WasPressedThisFrame();

        selection += up ? -1 : (down ? 1 : 0);
        selection = Maths.Mod(selection, 2);
        SelectAll(false);

        if (Player.inputs.FindAction("General/Menu").WasPressedThisFrame() ||
        Player.inputs.FindAction("General/Deny").WasPressedThisFrame()) { Destroy(gameObject); return; }

        if (!Player.inputs.FindAction("General/Confirm").WasPressedThisFrame()) { return; }

        if (selection == 0) { Destroy(gameObject); return; }
        if (selection == 1)
        {
            Player.openMainMenu = true;
            faderInstance = FadeRoom.Fade();
            faderInstance.targetSceneName = SceneManager.GetActiveScene().name;
        }
    }
}
