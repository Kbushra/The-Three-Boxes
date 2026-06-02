using UnityEngine;

public class Persistent : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
