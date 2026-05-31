using UnityEngine;
using HelperFunctions;
using System.Linq;
using UnityEditor;

public class Door : MonoBehaviour
{
    [SerializeField] private Collider playerCollider;
    [SerializeField] private BoxCollider openBoxComponent;
    [SerializeField] private Transform closedTransformComponent;

    public bool open = false;
    public SceneAsset targetScene;

    void Update()
    {
        if (open && closedTransformComponent)
        {
            Maths.OriginScale(closedTransformComponent,
                new Vector3(closedTransformComponent.localScale.x, Maths.LerpDelta(closedTransformComponent.localScale.y, 0, 0.8f),
                    closedTransformComponent.localScale.z), new Vector3(0, 1, 0));
            if (Maths.RoundNearest(closedTransformComponent.localScale.y, 0.01f) == 0) { Destroy(closedTransformComponent.gameObject); }
        }

        if (!open || !Collisions.BoxCollisions(openBoxComponent).Contains(playerCollider)) { return; }

        Player.stateQueue.Add(Player.State.Frozen);
    }
}
