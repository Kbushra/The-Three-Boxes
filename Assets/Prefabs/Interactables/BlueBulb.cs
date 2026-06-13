using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BlueBulb : Interactable
{
    [SerializeField] private GameObject lightObject;

    protected override void Awake()
    {
        base.Awake();

        bool errored = false;

        if (!lightObject)
        {
            Debug.LogError("Invalid blue bulb fields! Please add the light.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }
    }

    public void ToggleLight()
    {
        GetComponent<AudioSource>().Play();
        lightObject.SetActive(!lightObject.activeSelf);
    }
}
