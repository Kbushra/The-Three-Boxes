using UnityEngine;
using HelperFunctions;
using System;
using System.Linq;
using UnityEditor;
using System.Threading;

public class Door : MonoBehaviour
{
    private const int sideBottom = 0;
    private const int sideTop = 1;

    private Player player;
    private BoxCollider playerCollider;
    private Canvas canvas;
    [SerializeField] private GameObject openDoor;
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject fader;
    [SerializeField] private GameObject bigParticles;
    [SerializeField] private GameObject smallParticles;

    private float timer = 0;
    private GameObject smallStartParticle;
    private GameObject bigStartParticle;
    private GameObject smallEndParticle;

    public bool open = false;
    public SceneAsset targetScene;

    private void Awake()
    {
        bool errored = false;

        if (!openDoor)
        {
            Debug.LogError("Invalid door fields! Please add the open door.");
            errored = true;
        }

        if (!closedDoor)
        {
            Debug.LogError("Invalid door fields! Please add the closed door.");
            errored = true;
        }

        if (!targetScene)
        {
            Debug.LogError("Invalid door fields! Please add the target scene.");
            errored = true;
        }

        if (!fader)
        {
            Debug.LogError("Invalid door fields! Please add the fader prefab.");
            errored = true;
        }

        if (!bigParticles)
        {
            Debug.LogError("Invalid door fields! Please add the big door particles prefab.");
            errored = true;
        }

        if (!smallParticles)
        {
            Debug.LogError("Invalid door fields! Please add the small door particles prefab.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }
    }

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        playerCollider = player.GetComponent<BoxCollider>();
        canvas = SingleCanvas.canvas;
        if (!player) { Debug.LogWarning("Player not found!"); }
        if (!playerCollider) { Debug.LogWarning("Player collider not found!"); }
        if (!canvas) { Debug.LogWarning("Canvas not found!"); }
    }

    private GameObject SpawnParticle(GameObject particle, int side)
    {
        GameObject particleInstance = Instantiate(particle);
        particleInstance.transform.rotation = Quaternion.Euler(side == sideBottom ? 0 : 180, 0, 0);
        particleInstance.transform.position = Maths.OriginWorldPosition(openDoor.transform,
            new Vector3(0, side == sideBottom ? -1 : 1, 0));

        ParticleSystem.ShapeModule shape = particleInstance.GetComponent<ParticleSystem>().shape;
        shape.scale = new Vector3(Maths.BoundsSize(openDoor.transform).x + 2, 1, Maths.BoundsSize(openDoor.transform).z + 2);

        return particleInstance;
    }

    private void OpenCutscene()
    {
        if (!smallStartParticle)
        {
            smallStartParticle = SpawnParticle(smallParticles, sideBottom);
        }

        timer += Time.deltaTime;
        if (timer < 1) { return; }

        if (!bigStartParticle)
        {
            bigStartParticle = SpawnParticle(bigParticles, sideBottom);
        }

        if (timer >= 2.5f && !smallEndParticle)
        {
            smallEndParticle = SpawnParticle(smallParticles, sideTop);
        }

        if (!closedDoor) { return; }

        Maths.OriginScale(closedDoor.transform,
            new Vector3(closedDoor.transform.localScale.x, Maths.LerpDelta(closedDoor.transform.localScale.y, 0, 0.95f),
                closedDoor.transform.localScale.z), new Vector3(0, 1, 0));
        
        if (Maths.RoundNearest(closedDoor.transform.localScale.y, 0.01f) == 0)
        {
            Destroy(closedDoor);
        }
    }

    private void Update()
    {
        if (open) { OpenCutscene(); }

        if (!player || !playerCollider) { return; }
        if (!open || !Collisions.BoxCollisions(openDoor.GetComponent<BoxCollider>()).Contains(playerCollider)) { return; }

        player.stateQueue.Add(Player.State.Frozen);
        if (FindObjectsByType<Fade>().Length > 0) { return; }

        GameObject fadeInstance = Instantiate(fader, canvas.transform);
        fadeInstance.GetComponent<Fade>().targetScene = targetScene;
    }
}
