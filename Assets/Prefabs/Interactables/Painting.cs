using UnityEngine;

public class Painting : Interactable
{
    private Canvas canvas;
    [SerializeField] private GameObject message;

    [SerializeField] private AudioSource select;
    [SerializeField] private AudioSource[] corrupts;

    public bool corrupt = false;
    public string text = "";

    protected override void Awake()
    {
        base.Awake();

        bool errored = false;

        if (!message)
        {
            errored = true;
            Debug.LogError("Invalid painting fields! Please add the message prefab.");
        }

        if (!select)
        {
            errored = true;
            Debug.LogError("Invalid painting fields! Please add the select SFX.");
        }

        if (corrupts.Length == 0)
        {
            errored = true;
            Debug.LogError("Invalid painting fields! Please add at least one corrupt SFX.");
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

        if (corrupt)
        {
            int randSfx = (int)Mathf.Floor(Random.value * (corrupts.Length - 0.01f));
            corrupts[randSfx].Play();
        }
        else { select.Play(); }
        
        Message messageInstance = Instantiate(message, canvas.transform).GetComponent<Message>();
        messageInstance.message = text;
    }
}
