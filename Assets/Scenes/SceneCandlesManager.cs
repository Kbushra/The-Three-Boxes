using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [SerializeField] private Led[] leds;

    private List<int> symbol = new List<int>();

    private LightmapData[] lightmaps;
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

        if (errored) { Destroy(this); return; }

        lightmaps = LightmapSettings.lightmaps;
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

    private void CrypticPaintingDialogue()
    {
        crypticPainting.corrupt = realmPivot.activeSelf;
        if (!crypticPainting.corrupt) { crypticPainting.text = "Just a door..."; return; }

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
        bool interacted = Player.inputs.FindAction("General/Interact").WasPressedThisFrame();
        if (!realmPivot.activeSelf || !interacted || !realmPivot.GetComponentInParent<BlueBulb>().interactable) { return; }

        Led.ResetLedFlickers(leds);
        if (crypticPaintingFlame.activeSelf) { return; }

        bool success = false;
        Led led = leds[0];

        if (keyPaintingFlame.activeSelf && oceanPaintingFlame.activeSelf)
        {
            List<int> diamond = new List<int> { 1, 3, 5, 7 };
            success = symbol.OrderBy(el => el).SequenceEqual(diamond.OrderBy(x => x));
            led = leds[0];
        }
        else if (keyPaintingFlame.activeSelf)
        {
            List<int> keyK = new List<int> { 0, 2, 3, 4, 6, 8 };
            success = symbol.OrderBy(el => el).SequenceEqual(keyK.OrderBy(x => x));
            led = leds[1];
        }
        else if (oceanPaintingFlame.activeSelf)
        {
            List<int> boat = new List<int> { 3, 5, 6, 7, 8 };
            success = symbol.OrderBy(el => el).SequenceEqual(boat.OrderBy(x => x));
            led = leds[2];
        }
        else
        {
            success = symbol.Count == 0;
            led = leds[3];
        }

        Debug.Log( string.Join(", ", symbol.OrderBy(x => x)) );
        if (success) { led.Toggle(true); } else { led.flickerTime = 1; }
        symbol.Clear();
    }

    private void LateUpdate()
    {
        RealmTransition();
        CrypticPaintingDialogue();
        InterpretKeypad();
        KeypadLeds();
    }
}
