using UnityEngine;

public class SceneLayersManager : MonoBehaviour
{
    [SerializeField] private GameObject[] firstLEDs = new GameObject[4];
    [SerializeField] private GameObject[] secondLEDs = new GameObject[4];
    [SerializeField] private Button[] firstButtons = new Button[2];
    [SerializeField] private Button[] secondButtons = new Button[2];
    [SerializeField] private Button swapButton;
    [SerializeField] private Door[] doors = new Door[3];

    private int layer = 0;
    private string decodedMorse = "";
    private int firstTap = 0;
    private int secondTap = 0;
    private int firstTapWait = 0;
    private int secondTapWait = 0;
    private bool thirdLayerActivated = false;

    private void InterpretMorse(Button signalButton, Button breakButton)
    {
        
    }

    private void MorseLEDs(string morse, GameObject[] LEDs)
    {
        
    }

    private void InterpretTap(Button firstTapButton, Button secondTapButton)
    {
        
    }

    private void TapLEDs(int firstTapReq, int secondTapReq, GameObject[] LEDs)
    {
        
    }

    private void Update()
    {
        if (layer == 0)
        {
            InterpretMorse(firstButtons[0], firstButtons[1]);
            MorseLEDs(decodedMorse, firstLEDs);
        }
        if (layer == 1)
        {
            InterpretTap(secondButtons[0], secondButtons[1]);
            TapLEDs(3, 5, secondLEDs);
        }
        if (layer == 2)
        {
            if (thirdLayerActivated)
            {
                InterpretTap(firstButtons[0], firstButtons[1]);
                TapLEDs(2, 3, firstLEDs);

                InterpretMorse(secondButtons[0], secondButtons[1]);
                MorseLEDs(decodedMorse, secondLEDs);
            }
            else if (swapButton.holdLength > 0) { thirdLayerActivated = true; }
        }
    }
}
