using UnityEngine;
using HelperFunctions;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class Player : MonoBehaviour
{
    public enum State { MainMenu, Normal, Frozen, Locked };
    public static State state { get; private set; } = State.Normal;
    public static List<State> stateQueue = new List<State>();
    public static Inputs inputs;
    public static Player player;
    public static bool openMainMenu;
    public static bool closeMainMenu;
    private static float mainMenuCameraSpin = 0;

    public float speed = 5;
    public float sensitivity = 10;
    public float gravity = 0.8f;
    
    private BoxCollider boxComponent;
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private Camera cameraComponent;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Transform mainMenuView;

    [SerializeField] private GameObject fader;

    private Canvas canvas;

    private float yaw = 0;
    private float pitch = 0;
    private float vsp = 0;

    private float moveTime = 0;
    private float cameraStartY = 0;

    private void Awake()
    {
        player = this;

        Application.targetFrameRate = 60;
        #if !UNITY_WEBGL
        Cursor.lockState = CursorLockMode.Locked;
        #endif

        inputs = new Inputs();
        boxComponent = GetComponent<BoxCollider>();

        bool errored = false;

        if (!cameraContainer)
        {
            Debug.LogError("Invalid player fields! Please add the camera container transform.");
            errored = true;
        }

        if (!cameraComponent)
        {
            Debug.LogError("Invalid player fields! Please add the camera.");
            errored = true;
        }

        if (!pauseMenu)
        {
            Debug.LogError("Invalid player fields! Please add the pause menu.");
            errored = true;
        }

        if (!mainMenu)
        {
            Debug.LogError("Invalid player fields! Please add the main menu.");
            errored = true;
        }

        if (!fader)
        {
            Debug.LogError("Invalid player fields! Please add the fader.");
            errored = true;
        }
        else { FadeRoom.fader = fader; }

        if (errored) { Destroy(this); return; }
        
        cameraStartY = cameraComponent.transform.localPosition.y;
    }

    private void Start()
    {
        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }

        if (!mainMenuView)
        {
            Debug.LogWarning("Main menu view not found!");
            mainMenuView = new GameObject().transform;
            mainMenuView.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }
    }

    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }

    private Vector3 MovementVector()
    {
        if (state != State.Normal) { return Vector3.zero; }

        Vector2 movement = inputs.FindAction("General/Move").ReadValue<Vector2>();
        Vector3 inputVector = new Vector3(movement.x, 0, movement.y).normalized;
        Vector3 rotatedVector = Quaternion.Euler(0, yaw, 0) * inputVector;
        return rotatedVector;
    }

    private void Look()
    {
        Vector2 look = inputs.FindAction("General/Look").ReadValue<Vector2>();
        yaw += look.x * sensitivity * Time.deltaTime;
        pitch -= look.y * sensitivity * Time.deltaTime;
        pitch = Math.Clamp(pitch, -80, 80);
        cameraContainer.transform.rotation = Quaternion.Euler(0, yaw, 0);
        cameraComponent.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private bool SpaceFree(in BoxCollider boxComponent, in Vector3 move, out RaycastHit info, out RaycastHit infoCurved)
    {
        info = new RaycastHit(); info.distance = 0;
        infoCurved = new RaycastHit(); infoCurved.distance = 0;
        return Collisions.BoxFree(boxComponent, move, out info) &&
            Collisions.BoxFree(boxComponent, move, out infoCurved, 0.1f, "CurvedGeometry");
    }

    private bool CheckAxis(Vector3 leftoverMove, Vector3 mask)
    {
        Vector3 filteredMove = Vector3.Scale(leftoverMove, mask);

        if (SpaceFree(boxComponent, filteredMove, out RaycastHit info, out RaycastHit infoCurved))
        { transform.position += filteredMove; return false; }

        transform.position += mask * Mathf.Min(info.distance, infoCurved.distance);
        return true;
    }

    private void Move()
    {
        Vector3 move = speed * Time.deltaTime * MovementVector();

        bool inAir = SpaceFree(boxComponent, new Vector3(0, -0.2f, 0), out _, out _);
        if (vsp <= 0 && !inAir) { vsp = inputs.FindAction("General/Jump").IsPressed() ? 0.2f : 0; }
        else { vsp -= Time.deltaTime * gravity; }
        
        vsp = Mathf.Clamp(vsp, -0.5f, 0.5f);
        move.y = vsp;
        
        if (move.magnitude < 0.01f) { return; }

        //Walk above curved geometry
        Vector3 offsetVector = new Vector3(0, 0.05f, 0);
        while (!Collisions.BoxFree(boxComponent, move, out _, 0.1f, "CurvedGeometry") &&
        Collisions.BoxFree(boxComponent, offsetVector, out _))
        {
            transform.position += offsetVector;
            vsp = 0; move.y = 0;
        }

        if (SpaceFree(boxComponent, move, out RaycastHit info, out RaycastHit infoCurved)) { transform.position += move; return; }

        Vector3 leftoverMove = move;
        float scale = Mathf.Min(info.distance, infoCurved.distance) / move.magnitude;
        move *= scale;
        transform.position += move;
        leftoverMove -= move;
        
        CheckAxis(leftoverMove, Vector3.right);
        if (CheckAxis(leftoverMove, Vector3.up)) { vsp = 0; }
        CheckAxis(leftoverMove, Vector3.forward);
    }

    private void MainMenuMovement()
    {
        if (FindObjectsByType<MainMenu>().Length == 0) { Instantiate(mainMenu, canvas.transform); }

        transform.position = mainMenuView.position;
        cameraContainer.transform.localRotation = Quaternion.Euler(30, mainMenuCameraSpin, 0);
        mainMenuCameraSpin += Time.deltaTime * 3;
    }

    private void UpdateState()
    {
        FadeRoom[] faders = FindObjectsByType<FadeRoom>();
        bool effectTransition = faders.Length > 0 && faders[0].fadingOut;
        if (openMainMenu && effectTransition)
        {
            openMainMenu = false;
            state = State.MainMenu;
        }

        if (closeMainMenu && effectTransition)
        {
            closeMainMenu = false;
            state = State.Normal;
        }

        if (state == State.MainMenu) { return; }

        if (stateQueue.Contains(State.Locked)) { state = State.Locked; }
        else if (stateQueue.Contains(State.Frozen)) { state = State.Frozen; }
        else { state = State.Normal; }

        stateQueue.Clear();
    }

    private void Update()
    {
        #if UNITY_WEBGL
        if (inputs.FindAction("General/Press").IsPressed()) { Cursor.lockState = CursorLockMode.Locked; }
        #endif

        UpdateState();
        if (state == State.MainMenu) { MainMenuMovement(); return; }

        if (inputs.FindAction("General/Menu").WasPressedThisFrame() && FindObjectsByType<PauseMenu>().Length == 0)
        { Instantiate(pauseMenu, canvas.transform); return; }

        if (state != State.Locked) { Look(); }

        Vector3 startPosition = transform.position;
        Move();

        //Snap to curved geometry
        if (vsp <= 0 && !Collisions.BoxFree(boxComponent, new Vector3(0, Mathf.Min(-0.2f, vsp), 0),
        out RaycastHit snapInfo, 0.04f, "CurvedGeometry"))
        {
            transform.position += new Vector3(0, -snapInfo.distance, 0);
            vsp = 0;
        }

        bool inAir = SpaceFree(boxComponent, new Vector3(0, -0.2f, 0), out _, out _);
        bool notMoving = transform.position.x == startPosition.x && transform.position.z == startPosition.z;
        if (inAir || notMoving) { moveTime = Maths.LerpDelta(moveTime, Maths.RoundNearest(moveTime, Mathf.PI), 0.9f); }
        else { moveTime += Time.deltaTime * 10; }

        cameraComponent.transform.localPosition = new Vector3(cameraComponent.transform.localPosition.x,
            cameraStartY + Mathf.Sin(moveTime) * 0.1f, cameraComponent.transform.localPosition.z);
    }
}