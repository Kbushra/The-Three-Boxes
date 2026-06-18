using System.Collections.Generic;
using System.Linq;
using HelperFunctions;
using UnityEngine;
using UnityEngine.LowLevel;

public class SceneCandlesManager : MonoBehaviour
{
    [SerializeField] private GameObject[] mainRealm;
    [SerializeField] private GameObject[] altRealm;
    [SerializeField] private GameObject realmPivot;
    [SerializeField] private Painting crypticPainting;

    [SerializeField] private GameObject keyPaintingFlame;
    [SerializeField] private GameObject oceanPaintingFlame;
    [SerializeField] private GameObject crypticPaintingFlame;

    [SerializeField] private Button[] keypad;
    [SerializeField] private Led[] keypadLeds = new Led[4];

    [SerializeField] private Button timedButton;

    [SerializeField] private Button finalButton;

    [SerializeField] private Door[] doors = new Door[3];

    private List<int> symbol = new List<int>();
    public bool keypadSuccess = false;
    private int[] times = { 0, 0, 0, 0 };
    public bool timedSuccess = false;

    private LightmapData[] lightmaps;
    private LightProbes lightProbes;
    private Dictionary<Renderer, int> rendererIndices = new Dictionary<Renderer, int>();

    private void Awake()
    {
        bool errored = false;

        if (!realmPivot)
        {
            Debug.LogError("Invalid scene candle manager fields! Please add the realm pivot.");
            errored = true;
        }

        if (!crypticPainting)
        {
            Debug.LogError("Invalid scene candle manager fields! Please add the cryptic painting.");
            errored = true;
        }

        if (!keyPaintingFlame || !oceanPaintingFlame || !crypticPaintingFlame)
        {
            Debug.LogError("Invalid scene candle manager fields! Please add the painting flames.");
            errored = true;
        }

        if (keypadLeds.Length < 4)
        {
            Debug.LogError("Invalid scene candle manager fields! Please add four keypad LEDs.");
            errored = true;
        }

        if (doors.Length < 3)
        {
            Debug.LogError("Invalid scene candle manager fields! Please add three doors.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }

        lightmaps = LightmapSettings.lightmaps;
        lightProbes = LightmapSettings.lightProbes;
    }

    private void Start()
    {
        Renderer[] renderers = FindObjectsByType<Renderer>();
        foreach (Renderer renderer in renderers) { rendererIndices[renderer] = renderer.lightmapIndex; }
    }

    private void RealmTransition()
    {
        foreach (GameObject mainRealmObj in mainRealm)
        {
            mainRealmObj.SetActive(!realmPivot.activeSelf);
        }

        foreach (GameObject altRealmObj in altRealm)
        {
            altRealmObj.SetActive(realmPivot.activeSelf);
        }
        
        LightmapSettings.lightmaps = !realmPivot.activeSelf ? lightmaps : null;
        LightmapSettings.lightProbes = !realmPivot.activeSelf ? lightProbes : null;
        Renderer[] renderers = FindObjectsByType<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (!rendererIndices.ContainsKey(renderer))
            {
                if (renderer.lightmapIndex == -1) { continue; }
                rendererIndices[renderer] = renderer.lightmapIndex;
            }

            renderer.lightmapIndex = !realmPivot.activeSelf ? rendererIndices[renderer] : -1;
        }
    }

    private void CrypticPaintingKeypadDialogue()
    {
        if (!crypticPainting.corrupt) { return; }

        if (crypticPaintingFlame.activeSelf)
        {
            //The keypad craves for symbols, submitted in blue fire. Both realms must use the same pattern.
            crypticPainting.text = "Mcy kyxgtn eitqyj sfi jxbwfaj, jpwblmmyn ld wapy sliy. Wfmc iytabj bpjm pjy mcy jtby gtmmyid.";
            return;
        }

        if (keyPaintingFlame.activeSelf && oceanPaintingFlame.activeSelf)
        {
            //A key, and an ocean, the water as your retreat. The shine in the waves seem like a diamond.
            crypticPainting.text = "T kyx, tdn td feytd, mcy rtmyi tj xfpi iymiytm. Mcy jcldy ld mcy rtqyj jyyb alky t nltbfdn.";
            return;
        }

        if (keyPaintingFlame.activeSelf)
        {
            //A key, your way out. The first letter resonates in your mind.
            crypticPainting.text = "T kyx, xfpi rtx fpm. Mcy slijm aymmyi iyjfdtmyj ld xfpi bldn.";
            return;
        }

        if (oceanPaintingFlame.activeSelf)
        {
            //The boiling ocean, crossable not even by a crafted boat.
            crypticPainting.text = "Mcy wflaldv feytd, eifjjtway dfm yqyd wx t eitsmyn wftm.";
            return;
        }

        //Emptiness fills you.
        crypticPainting.text = "Ybgmldyjj slaaj xfp.";
    }

    private void InterpretKeypad()
    {
        for (int i = 0; i < keypad.Length; i++)
        {
            if (keypad[i].fullHoldLength > 0)
            {
                if (!symbol.Contains(i)) { symbol.Add(i); }
                keypad[i].fullHoldLength = 0;
            }
        }
    }

    private void KeypadLeds()
    {
        if (!realmPivot.activeSelf) { return; }

        Led.ResetLedFlickers(keypadLeds);
        if (crypticPaintingFlame.activeSelf) { symbol.Clear(); return; }

        bool success = false;
        Led led = keypadLeds[0];

        if (keyPaintingFlame.activeSelf && oceanPaintingFlame.activeSelf)
        {
            List<int> diamond = new List<int> { 1, 3, 5, 7 };
            success = symbol.OrderBy(el => el).SequenceEqual(diamond.OrderBy(x => x));
            led = keypadLeds[0];
        }
        else if (keyPaintingFlame.activeSelf)
        {
            List<int> keyK = new List<int> { 0, 2, 3, 4, 6, 8 };
            success = symbol.OrderBy(el => el).SequenceEqual(keyK.OrderBy(x => x));
            led = keypadLeds[1];
        }
        else if (oceanPaintingFlame.activeSelf)
        {
            List<int> boat = new List<int> { 3, 5, 6, 7, 8 };
            success = symbol.OrderBy(el => el).SequenceEqual(boat.OrderBy(x => x));
            led = keypadLeds[2];
        }
        else
        {
            success = symbol.Count == 0;
            led = keypadLeds[3];
        }

        if (success) { led.Toggle(true); } else { led.flickerTime = 1; }
        symbol.Clear();

        keypadSuccess = Led.LedsOn(keypadLeds);
    }

    private void CrypticPaintingTimedDialogue()
    {
        if (!crypticPainting.corrupt) { return; }

        if (!crypticPaintingFlame.activeSelf)
        {
            //A button, held for the total of all three times. Each time randomly chosen through blue fire.
            crypticPainting.text = "T wpmmfd, cyan sfi mcy mfmta fs taa mciyy mlbyj. Ytec mlby itdnfbax ecfjyd mcifpvc wapy sliy.";
            return;
        }

        int timedIndex = 0;
        string paintingText = "";
        if (keyPaintingFlame.activeSelf && oceanPaintingFlame.activeSelf)
        {
            //The key and ocean choose
            paintingText = "Mcy kyx tdn feytd ecffjy";
            timedIndex = 0;
        }
        else if (keyPaintingFlame.activeSelf) 
        {
            //The key chooses
            paintingText = "Mcy kyx ecffjyj";
            timedIndex = 1;
        }
        else if (oceanPaintingFlame.activeSelf)
        {
            //The ocean chooses
            paintingText = "Mcy feytd ecffjyj";
            timedIndex = 2;
        }
        else
        {
            //The void chooses
            paintingText = "Mcy qfln ecffjyj";
            timedIndex = 3;
        }

        //ONE, TWO, THREE
        string[] possibleTimes = { "FDY", "MRF", "MCIYY" };
        int randIndex = (int)Mathf.Floor(Random.value * 2.99f);
        crypticPainting.text = $"{paintingText}: {possibleTimes[randIndex]}";
        times[timedIndex] = randIndex + 1;
    }

    private void Update()
    {
        bool interacted = Player.inputs.FindAction("General/Interact").WasPressedThisFrame();

        crypticPainting.corrupt = realmPivot.activeSelf;
        if (!crypticPainting.corrupt) { crypticPainting.text = "Just a door..."; }

        if (!keypadSuccess)
        {
            CrypticPaintingKeypadDialogue();
            return;
        }

        if (!timedSuccess)
        {
            if (interacted && crypticPainting.interactable) { CrypticPaintingTimedDialogue(); }
            return;
        }

        if (crypticPainting.corrupt)
        {
            //Just a door...
            crypticPainting.text = "Opjm t nffi...";
        }
    }

    private void LateUpdate()
    {
        bool interacted = Player.inputs.FindAction("General/Interact").WasPressedThisFrame();
        bool swappedRealm = interacted && realmPivot.GetComponentInParent<BlueBulb>().interactable;

        RealmTransition();
        
        if (!keypadSuccess)
        {
            InterpretKeypad();
            if (swappedRealm) { KeypadLeds(); }
            return;
        }

        doors[0].open = true;

        if (!timedSuccess)
        {
            float totalTime = times.Aggregate((prev, el) => prev + el);
            if (!times.Contains(0) && Maths.NearEquals(timedButton.fullHoldLength, totalTime, 1)) { timedSuccess = true; }
            return;
        }

        doors[1].open = true;

        if (finalButton.fullHoldLength > 0) { doors[2].open = true; }
    }
}
