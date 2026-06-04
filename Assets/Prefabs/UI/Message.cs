using System;
using HelperFunctions;
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

        if (shifts >= 1) { fade = true; }
        float targAlpha = fade ? 0 : 1;
        float time = shifts == 0 && !fade ? 0.5f : (shifts == 1 || fade ? 0.9f : 0.98f);
        textComponent.color = new Color(1, 1, 1, Maths.LerpDelta(textComponent.color.a, targAlpha, time));

        if (fade && textComponent.color.a <= 0.01f)
        {
            Shift(-1);
            Destroy(gameObject);
        }
    }
}
