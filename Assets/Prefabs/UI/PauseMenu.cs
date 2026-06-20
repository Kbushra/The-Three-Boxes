using HelperFunctions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI backText;
    [SerializeField] private TextMeshProUGUI optionsText;
    [SerializeField] private TextMeshProUGUI toMainText;
    [SerializeField] private TextMeshProUGUI sensitivityText;
    [SerializeField] private AudioClip select;

    private int[] selection = { 0, 0 };
    private int page = 0;
    private float sensitivityDelay = 0;

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

    private void Select(TextMeshProUGUI mesh, string text, int fontSize, int selectIndex, int reqSelection, bool instant)
    {
        bool selected = page >= selectIndex && selection[selectIndex] == reqSelection;
        mesh.text = selected ? $"> {text} <" : text;
        mesh.fontSize = Maths.LerpDelta(mesh.fontSize, selected ? fontSize : fontSize - 4, instant ? 1 : 0.995f);
    }

    private void SelectAll(bool instant)
    {
        Select(backText, "BACK", 32, 0, 0, instant);
        Select(optionsText, "OPTIONS", 32, 0, 1, instant);
        Select(toMainText, "MAIN MENU", 32, 0, 2, instant);
        Select(sensitivityText, $"SENSITIVITY ({Maths.RoundNearest(Saving.saveData.sensitivity, 0.05f).ToString("F2")})", 24, 1, 0, instant);

        float[] columns = { -240, 0, 240 };
        float mainColumn = Maths.LerpDelta(backText.transform.localPosition.x, page == 0 ? columns[1] : columns[0], instant ? 1 : 0.995f);
        float optionsColumn = Maths.LerpDelta(sensitivityText.transform.localPosition.x, page == 0 ? columns[2] : columns[1], instant ? 1 : 0.995f);

        TextMeshProUGUI[] mainTexts = { backText, optionsText, toMainText };
        foreach (TextMeshProUGUI mainText in mainTexts)
        {
            mainText.transform.localPosition = new Vector3(mainColumn,
                mainText.transform.localPosition.y, mainText.transform.localPosition.z);
        }

        sensitivityText.transform.localPosition = new Vector3(optionsColumn,
            sensitivityText.transform.localPosition.y, sensitivityText.transform.localPosition.z);
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
        bool left = Player.inputs.FindAction("General/Left").WasPressedThisFrame();
        bool right = Player.inputs.FindAction("General/Right").WasPressedThisFrame();
        bool leftHeld = Player.inputs.FindAction("General/Left").IsPressed();
        bool rightHeld = Player.inputs.FindAction("General/Right").IsPressed();

        selection[page] += up ? -1 : (down ? 1 : 0);
        selection[page] = Maths.Mod(selection[page], page == 0 ? 3 : 1);
        SelectAll(false);

        if (Player.inputs.FindAction("General/Menu").WasPressedThisFrame()) { Destroy(gameObject); Saving.WriteSave(); return; }

        if (Player.inputs.FindAction("General/Deny").WasPressedThisFrame())
        {
            page--;
            Saving.WriteSave();
            if (page < 0) { Destroy(gameObject); return; }
        }

        if (page == 1)
        {
            sensitivityDelay -= Time.deltaTime;
            if (left) { Saving.saveData.sensitivity -= 0.1f; sensitivityDelay = 0.5f; }
            if (right) { Saving.saveData.sensitivity += 0.1f; sensitivityDelay = 0.5f; }

            if (sensitivityDelay <= 0)
            {
                if (leftHeld) { Saving.saveData.sensitivity -= Time.deltaTime; }
                if (rightHeld) { Saving.saveData.sensitivity += Time.deltaTime; }
            }

            Saving.saveData.sensitivity = Mathf.Clamp(Saving.saveData.sensitivity, 0.05f, 5);
        }
        else { sensitivityDelay = 0; }

        if (!Player.inputs.FindAction("General/Confirm").WasPressedThisFrame()) { return; }
        if (page != 0) { return; }
        
        if (Player.player) { AudioSource.PlayClipAtPoint(select, Player.player.transform.position); }

        if (selection[0] == 0) { Destroy(gameObject); return; }
        else if (selection[0] == 1) { page++; }
        else if (selection[0] == 2)
        {
            Player.openMainMenu = true;
            faderInstance = FadeRoom.Fade();
            faderInstance.targetSceneName = SceneManager.GetActiveScene().name;
        }
    }
}
