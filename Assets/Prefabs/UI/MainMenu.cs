using HelperFunctions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playText;
    [SerializeField] private TextMeshProUGUI levelSelectText;
    [SerializeField] private TextMeshProUGUI[] levelTexts;
    [SerializeField] private SceneAsset[] levelScenes;
    private string[] levelNames = { "INTRO", "CRATES", "PAINTINGS", "BUTTONS", "REALMS", "ENDING" };

    private int[] selection = { 0, 0 };
    private int page = 0;
    private Canvas canvas;

    private void Awake()
    {
        bool errored = false;

        if (!playText)
        {
            Debug.LogError("Invalid pause menu fields! Please add back text.");
            errored = true;
        }

        if (!levelSelectText)
        {
            Debug.LogError("Invalid pause menu fields! Please add to main text.");
            errored = true;
        }

        if (levelTexts.Length == 0 || !(levelTexts.Length == levelScenes.Length && levelScenes.Length == levelNames.Length))
        {
            Debug.LogError("Invalid pause menu fields! Please have the same amount of level options as names and scenes, and have at least one level.");
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
        Select(playText, "PLAY", 64, 0, 0, instant);
        Select(levelSelectText, "LEVELS", 32, 0, 1, instant);
        for (int i = 0; i < levelTexts.Length; i++)
        {
            Select(levelTexts[i], levelNames[i], 24, 1, i, instant);
        }

        float[] columns = { -240, 0, 240 };
        float mainColumn = Maths.LerpDelta(playText.transform.localPosition.x, page == 0 ? columns[1] : columns[0], instant ? 1 : 0.995f);
        float levelColumn = Maths.LerpDelta(levelTexts[0].transform.localPosition.x, page == 0 ? columns[2] : columns[1], instant ? 1 : 0.995f);

        playText.transform.localPosition = new Vector3(mainColumn,
            playText.transform.localPosition.y, playText.transform.localPosition.z);
        levelSelectText.transform.localPosition = new Vector3(mainColumn,
            levelSelectText.transform.localPosition.y, levelSelectText.transform.localPosition.z);
        
        for (int i = 0; i < levelTexts.Length; i++)
        {
            levelTexts[i].transform.localPosition = new Vector3(levelColumn,
                levelTexts[i].transform.localPosition.y, levelTexts[i].transform.localPosition.z);
        }
    }

    private void Start()
    {
        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }

        SelectAll(true);
    }

    private void Update()
    {
        if (Player.state != Player.State.MainMenu) { Destroy(gameObject); return; }
        
        FadeRoom[] faders = FindObjectsByType<FadeRoom>();
        if (faders.Length != 0 && !faders[0].fadingOut) { SelectAll(false); return; }

        bool up = Player.inputs.FindAction("General/Up").WasPressedThisFrame();
        bool down = Player.inputs.FindAction("General/Down").WasPressedThisFrame();
        selection[page] += up ? -1 : (down ? 1 : 0);
        selection[page] = Maths.Mod(selection[page], page == 0 ? 2 : levelTexts.Length);
        SelectAll(false);

        if (Player.inputs.FindAction("General/Deny").WasPressedThisFrame()) { page = 0; selection[1] = 0; }

        if (!Player.inputs.FindAction("General/Confirm").WasPressedThisFrame()) { return; }

        if (page == 0)
        {
            if (selection[0] == 0)
            {
                Player.closeMainMenu = true;
                FadeRoom faderInstance = FadeRoom.Fade();
                faderInstance.targetSceneName = SceneManager.GetActiveScene().name;
            }
            else { page++; }
        }
        else
        {
            FadeRoom faderInstance = FadeRoom.Fade();
            faderInstance.targetSceneName = levelScenes[selection[1]].name;
            faderInstance.spd = 2;
        }
    }
}
