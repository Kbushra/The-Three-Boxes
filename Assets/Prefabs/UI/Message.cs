using System;
using HelperFunctions;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Message : MonoBehaviour
{
    private TextMeshProUGUI textComponent;

    public string message = "";
    public float targY = -195;
    public bool fade = false;
    public int shifts = -1;

    public bool fadeAfter = true;
    
    private float fadeAfterDelay = 3;

    private void Shift(int dir)
    {
        if (textComponent.text == "") { return; }

        Message[] messageComponents = FindObjectsByType<Message>();
        foreach (Message messageComponent in messageComponents)
        {
            if (messageComponent.shifts <= shifts) { continue; }
            messageComponent.shifts += dir;
            messageComponent.targY += dir * textComponent.preferredHeight + 5;
        }

        shifts += dir;
    }

    private void Awake()
    {
        transform.localPosition = new Vector3(0, -200, 100);
        textComponent = GetComponent<TextMeshProUGUI>();
        textComponent.color = new Color(1, 1, 1, 0);
    }

    private void Update()
    {
        if (textComponent.text == "" && message != "")
        {
            textComponent.text = message;
            Shift(1);
        }
        else if (textComponent.text != message) { textComponent.text = message; }

        transform.localPosition = new Vector3(transform.localPosition.x, Maths.LerpDelta(transform.localPosition.y, targY, 0.8f), transform.localPosition.z);

        FadeEffect[] faders = FindObjectsByType<FadeRoom>();
        if (shifts >= 1 || (faders.Length > 0 && !faders[0].fadingOut)) { fade = true; }
        float targAlpha = fade ? 0 : 1;
        float time = shifts == 0 && !fade ? 0.5f : (shifts == 1 || fade ? 0.99f : 0.998f);
        textComponent.color = new Color(1, 1, 1, Maths.LerpDelta(textComponent.color.a, targAlpha, time));

        if (fadeAfter)
        {
            fadeAfterDelay -= Time.deltaTime;
            if (fadeAfterDelay <= 0) { fade = true; }
        }

        if (fade && Maths.NearEquals(textComponent.color.a, 0, 0.005f))
        {
            Shift(-1);
            Destroy(gameObject);
        }
    }
}
