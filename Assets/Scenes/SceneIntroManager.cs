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
            if (Player.state == Player.State.Normal && (move.x != 0 || move.y != 0))
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
            messageInstance.message = "WASD/ARROWS to move, CURSOR to look, ALT+ENTER/F4/F11 to toggle fullscreen";
            messageInstance.fadeAfter = false;
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
            messageInstance.fadeAfter = false;
        }
    }

    private void ManagePuzzle()
    {
        BigPaper[] papers = FindObjectsByType<BigPaper>();
        if (papers.Length == 0 || !papers[0].fading) { return; }

        Door.OpenAll();
    }

    private void Update()
    {
        if (Player.state == Player.State.MainMenu) { return; }
        ManageSteps();
        ManagePuzzle();   
    }
}
