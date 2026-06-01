using UnityEngine;
using HelperFunctions;
using System;
using System.Linq;
using UnityEditor;

public class Door : MonoBehaviour
{
    private Player player;
    private BoxCollider playerCollider;
    [SerializeField] private BoxCollider openBoxComponent;
    [SerializeField] private Transform closedTransformComponent;

    public bool open = false;
    public SceneAsset targetScene;

    private void Awake()
    {
        bool errored = false;

        if (!openBoxComponent)
        {
            Debug.LogError("Invalid door fields! Please add the open door box collider.");
            errored = true;
        }

        if (!closedTransformComponent)
        {
            Debug.LogError("Invalid door fields! Please add the closed door transform.");
            errored = true;
        }

        if (!targetScene)
        {
            Debug.LogError("Invalid door fields! Please add the target scene.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        playerCollider = player.GetComponent<BoxCollider>();
        if (!player) { Debug.LogWarning("Player not found!"); }
        if (!playerCollider) { Debug.LogWarning("Player collider not found!"); }
    }

    private void Update()
    {
        if (open && closedTransformComponent)
        {
            Maths.OriginScale(closedTransformComponent,
                new Vector3(closedTransformComponent.localScale.x, Maths.LerpDelta(closedTransformComponent.localScale.y, 0, 0.8f),
                    closedTransformComponent.localScale.z), new Vector3(0, 1, 0));
            if (Maths.RoundNearest(closedTransformComponent.localScale.y, 0.01f) == 0) { Destroy(closedTransformComponent.gameObject); }
        }

        if (!player || !playerCollider) { return; }
        if (!open || !Collisions.BoxCollisions(openBoxComponent).Contains(playerCollider)) { return; }

        player.stateQueue.Add(Player.State.Frozen);
    }
}
