using UnityEngine;

public class Scene1Manager : MonoBehaviour
{
    private int step = 0;
    private float controlTimer = 0;
    
    private Canvas canvas;
    private Message messageInstance;
    [SerializeField] private GameObject message;

    private void Start()
    {
        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }
    }

    private void ManageSteps()
    {
        if (!canvas) { return; }

        if (step == 0 || step == 1)
        {
            Vector2 move = Player.inputs.FindAction("General/Move").ReadValue<Vector2>();
            if (move.x != 0 || move.y != 0)
            {
                if (messageInstance) { messageInstance.fade = true; }

                controlTimer = 0;
                step = 2;
                return;
            }

            controlTimer += Time.deltaTime;
            if (controlTimer < 1 || step == 1) { return; }
            
            step = 1;
            messageInstance = Instantiate(message, canvas.transform).GetComponent<Message>();
            messageInstance.message = "WASD/Arrows to move, CURSOR to look";
        }

        if (step == 2 || step == 3)
        {
            Paper paper = FindObjectsByType<Paper>()[0];
            if (!paper.interactable) { return; }

            bool interacted = Player.inputs.FindAction("General/Interact").WasPressedThisFrame();
            if (interacted)
            {
                messageInstance.fade = true;
                step = 4;
                return;
            }

            if (step == 3) { return; }
        
            step = 3;
            messageInstance = Instantiate(message, canvas.transform).GetComponent<Message>();
            messageInstance.message = "E to interact";
        }
    }

    private void ManagePuzzle()
    {
        BigPaper[] papers = FindObjectsByType<BigPaper>();
        if (papers.Length == 0 || !papers[0].fading) { return; }

        foreach (Door door in FindObjectsByType<Door>())
        {
            door.open = true;
        }
    }

    private void Update()
    {
        ManageSteps();
        ManagePuzzle();   
    }
}
