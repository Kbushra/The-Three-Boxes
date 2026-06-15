using System.Linq;
using HelperFunctions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEndingManager : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Door door;
    [SerializeField] private BoxCollider endTrigger;
    [SerializeField] private GameObject fadeEffect;
    [SerializeField] private SceneAsset targetScene;

    private BoxCollider playerCollider;
    private Canvas canvas;

    private void Start()
    {
        if (!Player.player) { Debug.LogWarning("Player not found!"); }
        else
        {
            playerCollider = Player.player.GetComponent<BoxCollider>();
            if (!playerCollider) { Debug.LogWarning("Player collider not found!"); }
        }

        canvas = SingleCanvas.canvas;
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }
    }

    private void Update()
    {
        if (button.fullHoldLength > 0) { door.open = true; }

        if (!playerCollider || !canvas || FindObjectsByType<FadeRoom>().Length > 0 ||
        Collisions.BoxCollisions(endTrigger).Contains(playerCollider)) { return; }

        Player.spawnMainMenu = true;
        Instantiate(fadeEffect, canvas.transform);
    }
}
