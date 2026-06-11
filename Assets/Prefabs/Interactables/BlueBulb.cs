using UnityEngine;

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
        lightObject.SetActive(!lightObject.activeSelf);
    }
}
