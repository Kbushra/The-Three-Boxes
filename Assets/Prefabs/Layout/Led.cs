using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Led : MonoBehaviour
{
    public float flickerTime = 0;
    public bool on = false;

    public static void FlickerLeds(Led[] leds, float time)
    {
        foreach (Led led in leds) { led.flickerTime = time; }
    }

    public static bool LedsFlickering(Led[] leds)
    {
        foreach (Led led in leds) { if (led.flickerTime > 0) { return true; } }
        return false;
    }

    public static void ResetLedFlickers(Led[] leds)
    {
        foreach (Led led in leds) { if (led.flickerTime > 0) { led.Toggle(false); led.flickerTime = 0; } }
    }

    public static bool LedsOn(Led[] leds)
    {
        foreach (Led led in leds) { if (!led.on) { return false; } }
        return true;
    }

    public void Toggle(bool toggleOn)
    {
        on = toggleOn;

        Renderer renderComponent = GetComponent<Renderer>();
        float emission = on ? 25 : 0;
        renderComponent.material.EnableKeyword("_EMISSION");
        renderComponent.material.SetColor("_EmissionColor", new Color(emission, 0, 0));
    }

    private void Update()
    {
        if (flickerTime <= 0) { return; }

        float interval = 0.02f;
        int prevInterval = (int)(flickerTime / interval);
        flickerTime -= Time.deltaTime;
        int currInterval = (int)(flickerTime / interval);

        if (flickerTime <= 0) { Toggle(false); return; }
        if (prevInterval == currInterval) { return; }

        Toggle(Random.value < 0.5f);
    }
}
