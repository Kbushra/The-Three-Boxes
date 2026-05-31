using UnityEngine;

public class Scene1Manager : MonoBehaviour
{
    void Update()
    {
        BigPaper[] papers = FindObjectsByType<BigPaper>();
        if (papers.Length == 0 || !papers[0].fading) { return; }

        foreach (Door door in FindObjectsByType<Door>())
        {
            door.open = true;
        }
    }
}
