using UnityEngine;

public class SceneEndingManager : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Door door;

    private void Update()
    {
        if (button.fullHoldLength > 0) { door.open = true; }
    }
}
