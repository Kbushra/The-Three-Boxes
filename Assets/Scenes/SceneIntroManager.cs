using UnityEngine;

public class Scene1Manager : MonoBehaviour
{
    private int controlStep = 0;
    private int controlMessageStep = 0;
    private float controlTimer = 0;
    
    private Canvas canvas;
    private Message messageInstance;
    [SerializeField] private GameObject message;

    private void Start()
    {
        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }
    }

    private void controlMessages()
    {
        if (!canvas) { return; }

        if (controlStep == 0)
        {
            Vector2 move = Player.inputs.FindAction("General/Move").ReadValue<Vector2>();
            if (move.x != 0 || move.y != 0)
            {
                if (messageInstance) { messageInstance.fade = true; }

                controlTimer = 0;
                controlStep = 1;
                controlMessageStep = 2;
                return;
            }

            controlTimer += Time.deltaTime;
            if (controlTimer < 1 || controlMessageStep > 0) { return; }
            
            controlMessageStep = 1;
            messageInstance = Instantiate(message, canvas.transform).GetComponent<Message>();
            messageInstance.message = "WASD to move, CURSOR to look";
        }

        if (controlStep == 1)
        {
            Paper paper = FindObjectsByType<Paper>()[0];
            if (!paper.interactable) { return; }

            bool interacted = Player.inputs.FindAction("General/Interact").WasPressedThisFrame();
            if (interacted)
            {
                messageInstance.fade = true;
                controlStep = 2;
                return;
            }

            if (controlMessageStep > 2) { return; }
        
            controlMessageStep = 3;
            messageInstance = Instantiate(message, canvas.transform).GetComponent<Message>();
            messageInstance.message = "E to interact";
        }
    }

    private void Update()
    {
        controlMessages();

        BigPaper[] papers = FindObjectsByType<BigPaper>();
        if (papers.Length == 0 || !papers[0].fading) { return; }

        foreach (Door door in FindObjectsByType<Door>())
        {
            door.open = true;
        }
    }
}
