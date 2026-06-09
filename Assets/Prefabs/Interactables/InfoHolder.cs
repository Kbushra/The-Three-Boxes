using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InfoHolder : Interactable
{
    private Canvas canvas;
    [SerializeField] private GameObject message;

    public string text = "";

    protected override void Awake()
    {
        base.Awake();

        bool errored = false;

        if (!message)
        {
            errored = true;
            Debug.LogError("Invalid info holder fields! Please add the message prefab.");
        }

        if (errored) { Destroy(this); return; }
    }

    protected override void Start()
    {
        base.Start();

        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }
    }

    public void SpawnMessage()
    {
        if (!canvas) { return; }

        Message messageInstance = Instantiate(message, canvas.transform).GetComponent<Message>();
        messageInstance.message = text;
    }
}
