using System.Collections.Generic;
using UnityEngine;

public class SceneCandlesManager : MonoBehaviour
{
    [SerializeField] private GameObject[] mainRealm;
    [SerializeField] private GameObject[] altRealm;
    [SerializeField] private GameObject realmPivot;
    [SerializeField] private Painting crypticPainting;

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

        crypticPainting.text = "Jf, xfp ctqy sfpdn by, clnldv wycldn mcy wapy alvcm.";
    }

    private void LateUpdate()
    {
        RealmTransition();
        CrypticPaintingDialogue();
    }
}
