using UnityEngine;
using HelperFunctions;
using System;

public class Player : MonoBehaviour
{
    public float speed = 10;
    public float sensitivity = 10;
    public float gravity = 1;

    private Inputs inputs;
    private Rigidbody rigidbodyComponent;
    private Camera cameraComponent;

    private float yaw = 0;
    private float pitch = 0;

    private void Awake()
    {
        inputs = new Inputs();
        rigidbodyComponent = GetComponent<Rigidbody>();
        cameraComponent = GetComponentInChildren<Camera>();
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
        transform.rotation = Quaternion.Euler(0, yaw, 0);
        cameraComponent.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void Move()
    {
        RaycastHit info;
        Vector3 move = speed * Time.deltaTime * MovementVector();
        if (move.magnitude < 0.01f) { return; }

        float originalX = move.x;
        float originalZ = move.z;

        if (!HelperFunctions.Collision.MoveFree(rigidbodyComponent, move, out info))
        {
            move *= info.distance / move.magnitude;
            if (!HelperFunctions.Collision.MoveFree(rigidbodyComponent, new Vector3(originalX, 0, move.z), out info))
            {
                move.x += info.distance;
            }
            else { move.x = originalX; }

            if (!HelperFunctions.Collision.MoveFree(rigidbodyComponent, new Vector3(move.x, 0, originalZ), out info))
            {
                move.z += info.distance;
            }
            else { move.z = originalZ; }
        }

        rigidbodyComponent.MovePosition(rigidbodyComponent.position + move);
    }

    private void Update()
    {
        Look();
        Move();
    }
}
