using UnityEngine;

public class SceneQuadrantsManager : MonoBehaviour
{
    [SerializeField] private Interactable[] solutionPaintings;
    private int currPaintingIndex = 0;

    private void Update()
    {
        if (currPaintingIndex >= solutionPaintings.Length || Player.inputs == null ||
        !Player.inputs.FindAction("General/Interact").WasPressedThisFrame()) { return; }

        foreach (Interactable painting in solutionPaintings)
        {
            if (!painting.interactable) { continue; }

            if (painting != solutionPaintings[currPaintingIndex] && (currPaintingIndex == 0 || painting != solutionPaintings[currPaintingIndex - 1]))
            { currPaintingIndex = 0; }

            //Not an else so if you select the first painting again your index is now 1
            if (painting == solutionPaintings[currPaintingIndex]) { currPaintingIndex++; }
            break;
        }

        if (currPaintingIndex >= solutionPaintings.Length) { Door.OpenAll(); }
    }
}
