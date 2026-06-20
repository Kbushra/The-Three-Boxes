using System.Linq;
using UnityEngine;

public class SceneLayersManager : MonoBehaviour
{
    [SerializeField] private Led[] firstLeds = new Led[4];
    [SerializeField] private Led[] secondLeds = new Led[4];
    [SerializeField] private Button[] firstButtons = new Button[2];
    [SerializeField] private Button[] secondButtons = new Button[2];
    [SerializeField] private Button swapButton;
    [SerializeField] private Door[] doors = new Door[3];

    private bool[] layersCompleted = new bool[4];
    private bool swapActivated = false;

    private string[] morse = { "", "", "", "" };
    private int currMorseChar = 0;

    private int firstTap = 0;
    private int secondTap = 0;
    private float firstTapWait = 0;
    private float secondTapWait = 0;

    private void Awake()
    {
        bool errored = false;

        if (firstLeds.Length == 0)
        {
            Debug.LogError("Invalid scene layer manager fields! Please add first layer leds.");
            errored = true;
        }

        if (secondLeds.Length == 0)
        {
            Debug.LogError("Invalid scene layer manager fields! Please add second layer leds.");
            errored = true;
        }

        if (firstButtons.Length < 2)
        {
            Debug.LogError("Invalid scene layer manager fields! Please add 2 first layer buttons.");
            errored = true;
        }

        if (secondButtons.Length < 2)
        {
            Debug.LogError("Invalid scene layer manager fields! Please add 2 second layer buttons.");
            errored = true;
        }

        if (!swapButton)
        {
            Debug.LogError("Invalid scene layer manager fields! Please add the swap button.");
            errored = true;
        }

        if (doors.Length < 3)
        {
            Debug.LogError("Invalid scene layer manager fields! Please add 3 doors.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }
    }

    private void InterpretMorse(Button signalButton, Button breakButton, int layerIndex)
    {
        if (layersCompleted[layerIndex])
        {
            signalButton.fullHoldLength = 0;
            breakButton.fullHoldLength = 0;
            return;
        }

        if (signalButton.fullHoldLength > 0)
        {
            morse[currMorseChar] += signalButton.fullHoldLength < 0.25f ? '.' : '-';
            signalButton.fullHoldLength = 0;
        }

        if (breakButton.fullHoldLength > 0)
        {
            currMorseChar++;
            breakButton.fullHoldLength = 0;
        }
    }

    private void MorseLeds(string[] morseReq, Led[] leds, int layerIndex)
    {
        if (layersCompleted[layerIndex] || Led.LedsFlickering(leds)) { return; }

        for (int i = 0; i < Mathf.Min(currMorseChar, leds.Length); i++) { leds[i].Toggle(true); }

        if (currMorseChar >= leds.Length)
        {
            bool correct = true;
            for (int i = 0; i < Mathf.Min(morse.Length, morseReq.Length, leds.Length); i++)
            {
                if (morse[i] != morseReq[i])
                {
                    leds[i].flickerTime = 1;
                    correct = false;
                }
            }

            if (correct) { layersCompleted[layerIndex] = true; }
            else { Led.ToggleLEDs(leds, false); }

            morse = new string[4] { "", "", "", "" };
            currMorseChar = 0;
        }
    }

    private void InterpretTap(Button firstButton, Button secondButton, int layerIndex)
    {
        if (layersCompleted[layerIndex])
        {
            firstButton.fullHoldLength = 0;
            secondButton.fullHoldLength = 0;
            return;
        }

        if (firstButton.fullHoldLength > 0)
        {
            firstTap++;
            firstButton.fullHoldLength = 0;
        }

        if (firstTap > 0 && firstButton.currentHoldLength == 0) { firstTapWait += Time.deltaTime; } else { firstTapWait = 0; }

        if (secondButton.fullHoldLength > 0)
        {
            secondTap++;
            secondButton.fullHoldLength = 0;
        }

        if (secondTap > 0 && secondButton.currentHoldLength == 0) { secondTapWait += Time.deltaTime; } else { secondTapWait = 0; }
    }

    private void TapLeds(int firstTapReq, int secondTapReq, Led[] leds, int layerIndex)
    {
        if (layersCompleted[layerIndex] || Led.LedsFlickering(leds)) { return; }

        for (int i = 0; i < leds.Length; i++)
        {
            leds[i].Toggle(
                (i < leds.Length/4 && firstTap > 0) ||
                (i >= leds.Length/4 && i < leds.Length/2 && firstTapWait > 1) ||
                (i >= leds.Length/2 && i < leds.Length*3/4 && secondTap > 0) ||
                (i >= leds.Length*3/4 && secondTapWait > 1)
            );
        }

        if (firstTapWait > 1 && secondTapWait > 1)
        {
            bool firstCorrect = firstTap == firstTapReq;
            bool secondCorrect = secondTap == secondTapReq;

            if (!firstCorrect) { leds[0].flickerTime = 1; leds[1].flickerTime = 1; }
            if (!secondCorrect) { leds[2].flickerTime = 1; leds[3].flickerTime = 1; }

            if (firstCorrect && secondCorrect) { layersCompleted[layerIndex] = true; }
            else { Led.ToggleLEDs(leds, false); }

            firstTap = 0;
            secondTap = 0;
        }
    }

    private void Update()
    {
        if (swapButton.fullHoldLength > 0 && !swapActivated)
        {
            swapActivated = true;
            Led.ToggleLEDs(firstLeds, false);
            Led.ToggleLEDs(secondLeds, false);
        }

        if (!swapActivated)
        {
            InterpretMorse(firstButtons[0], firstButtons[1], 0);
            MorseLeds(new string[4] { ".--.", "---", "...", "-" }, firstLeds, 0);
            InterpretTap(secondButtons[0], secondButtons[1], 1);
            TapLeds(3, 5, secondLeds, 1);
        }
        else
        {
            InterpretTap(firstButtons[0], firstButtons[1], 2);
            TapLeds(2, 3, firstLeds, 2);
            InterpretMorse(secondButtons[0], secondButtons[1], 3);
            MorseLeds(new string[4] { "-", ".-", ".--.", "..." }, secondLeds, 3);
        }

        doors[0].open = layersCompleted[0];
        doors[1].open = layersCompleted[1];
        doors[2].open = layersCompleted[2] && layersCompleted[3];
    }
}
