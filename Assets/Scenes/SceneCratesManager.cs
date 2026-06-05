using System.Linq;
using HelperFunctions;
using UnityEngine;

public class SceneCratesManager : MonoBehaviour
{
    [SerializeField] private BoxCollider jumpCollider;
    [SerializeField] private GameObject message;
    private Message messageInstance;

    private Player player;
    private BoxCollider playerCollider;
    private Canvas canvas;

    private int step = 0;

    private void Awake()
    {
        bool errored = false;

        if (!jumpCollider)
        {
            Debug.LogError("Invalid scene manager fields! Please add jump collider.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }
    }

    private void Start()
    {
        player = Player.player;
        if (!player) { Debug.LogWarning("Player not found!"); }
        else
        {
            playerCollider = player.GetComponent<BoxCollider>();
            if (!playerCollider) { Debug.LogWarning("Player collider not found!"); }
        }

        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }
    }

    private void ManageSteps()
    {
        if (!player || !canvas) { return; }

        if (step == 0 || step == 1)
        {
            if (Player.inputs.FindAction("General/Jump").IsPressed())
            {
                if (messageInstance) { messageInstance.fade = true; }
                step = 2;
                return;
            }

            if (step == 1 || !Collisions.BoxCollisions(jumpCollider).Contains(playerCollider)) { return; }

            step = 1;
            messageInstance = Instantiate(message, canvas.transform).GetComponent<Message>();
            messageInstance.message = "SPACE to jump";
        }
    }

    private void ManagePuzzle()
    {
        //Todo
    }

    private void Update()
    {
        ManageSteps();
        ManagePuzzle();
    }
}
