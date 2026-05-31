using System;
using TMPro;
using UnityEngine;

public class Paper : Interactable
{
    [SerializeField] private Canvas canvasComponent;
    [SerializeField] private GameObject bigPaper;
    [SerializeField] private AudioSource grab;
    public string text = "";

    public void CollectPaper()
    {
        if (FindObjectsByType<BigPaper>().Length > 0) { return; }

        GameObject paper = Instantiate(bigPaper, canvasComponent.transform);
        paper.GetComponentInChildren<TextMeshProUGUI>().text = text;
        grab.Play();
    }
}
