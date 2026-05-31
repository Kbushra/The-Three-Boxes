using UnityEngine;
using HelperFunctions;
using System;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float sensitivity = 10;
    public float gravity = 0.8f;

    private Inputs inputs;
    [SerializeField] private BoxCollider boxComponent;
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private Camera cameraComponent;

    private float yaw = 0;
    private float pitch = 0;
    private float vsp = 0;

    private float moveTime = 0;
    private float cameraStartY = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        #if !UNITY_WEBGL
        Cursor.lockState = CursorLockMode.Locked;
        #endif

        inputs = new Inputs();
        cameraStartY = cameraComponent.transform.localPosition.y;
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

    private void CheckAxis(Vector3 leftoverMove, Vector3 mask)
    {
        Vector3 filteredMove = Vector3.Scale(leftoverMove, mask);

        if (Collisions.BoxFree(boxComponent, filteredMove, out RaycastHit info)) { transform.position += filteredMove; return; }
        transform.position += Vector3.Scale(Maths.RepeatNum(info.distance), mask);
    }

    private void Move()
    {
        //On ground
        if (!Collisions.BoxFree(boxComponent, new Vector3(0, -0.1f, 0), out _))
        {
            if (inputs.FindAction("General/Jump").IsPressed()) { vsp = 0.2f; }
            else { vsp = 0; }
        }
        else { vsp -= Time.deltaTime * gravity; }

        //Head bump
        if (!Collisions.BoxFree(boxComponent, new Vector3(0, 0.1f, 0), out _)) { vsp = Mathf.Clamp(vsp, -0.5f, 0); }
        else { vsp = Mathf.Clamp(vsp, -0.5f, 0.5f); }

        Vector3 move = speed * Time.deltaTime * MovementVector(); move.y = vsp;
        if (move.magnitude < 0.01f) { return; }

        if (Collisions.BoxFree(boxComponent, move, out RaycastHit info)) { transform.position += move; return; }

        Vector3 leftoverMove = move;
        move *= info.distance / move.magnitude;
        transform.position += move;
        leftoverMove -= move;
        
        CheckAxis(leftoverMove, Vector3.right);
        CheckAxis(leftoverMove, Vector3.up);
        CheckAxis(leftoverMove, Vector3.forward);
    }

    private void Update()
    {
        #if UNITY_WEBGL
        if (inputs.FindAction("General/Press").IsPressed()) { Cursor.lockState = CursorLockMode.Locked; }
        #endif

        Look();

        Vector3 startPosition = transform.position;
        Move();

        if (transform.position == startPosition) { moveTime = Maths.LerpDelta(moveTime, Maths.RoundNearest(moveTime, Mathf.PI), 0.9f); }
        else { moveTime += Time.deltaTime * 10; }

        cameraComponent.transform.localPosition = new Vector3(cameraComponent.transform.localPosition.x,
            cameraStartY + Mathf.Sin(moveTime) * 0.1f, cameraComponent.transform.localPosition.z);
    }
}