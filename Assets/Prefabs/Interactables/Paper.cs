using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Paper : Interactable
{
    private Canvas canvas;
    private AudioSource grab;
    [SerializeField] private GameObject bigPaper;

    public string text = "";

    protected override void Awake()
    {
        base.Awake();

        bool errored = false;

        if (!bigPaper)
        {
            errored = true;
            Debug.LogError("Invalid paper fields! Please add the big paper prefab.");
        }

        if (errored) { Destroy(this); return; }

        grab = GetComponent<AudioSource>();
    }

    protected override void Start()
    {
        base.Start();

        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }
    }

    public void CollectPaper()
    {
        if (FindObjectsByType<BigPaper>().Length > 0 || !canvas) { return; }

        GameObject paper = Instantiate(bigPaper, canvas.transform);
        paper.GetComponentInChildren<TextMeshProUGUI>().text = text;
        grab.Play();
    }
}
