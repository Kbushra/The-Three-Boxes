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

        if (Player.openMainMenu || Player.state == Player.State.MainMenu || !playerCollider || !canvas ||
        Collisions.BoxCollisions(endTrigger).Contains(playerCollider)) { return; }

        Player.openMainMenu = true;
        FadeRoom faderInstance = FadeRoom.Fade();
        faderInstance.targetSceneName = SceneManager.GetActiveScene().name;

        Saving.saveData.currentLevel = 6;
    }
}
