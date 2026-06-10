using UnityEngine;

public class SceneLayersManager : MonoBehaviour
{
    //All per layer
    [SerializeField] private GameObject[,] LEDs = new GameObject[3, 4];
    [SerializeField] private Button[,] buttons = new Button[3, 2];
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
        //todo
    }

    private void InterpretTap(Button firstTapButton, Button secondTapButton)
    {
        //todo
    }

    private void Update()
    {
        if (layer == 0)
        {
            InterpretMorse(buttons[0, 0], buttons[0, 1]);
        }
        if (layer == 1)
        {
            InterpretTap(buttons[1, 0], buttons[1, 1]);
        }
        if (layer == 2)
        {
            if (thirdLayerActivated)
            {
                InterpretTap(buttons[0, 0], buttons[0, 1]);
                InterpretMorse(buttons[1, 0], buttons[1, 1]);
            }
            else if (buttons[2, 0].holdLength > 0) { thirdLayerActivated = true; }
        }
    }
}
