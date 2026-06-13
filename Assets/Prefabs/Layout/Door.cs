using UnityEngine;
using HelperFunctions;
using System;
using System.Linq;
using UnityEditor;
using System.Threading;

public class Door : MonoBehaviour
{
    public static void OpenAll()
    {
        foreach (Door door in FindObjectsByType<Door>())
        {
            door.open = true;
        }
    }

    private const int sideBottom = 0;
    private const int sideTop = 1;

    private BoxCollider playerCollider;
    private Canvas canvas;
    [SerializeField] private GameObject openDoor;
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private AudioSource lightSteam;
    [SerializeField] private AudioSource heavySteam;
    [SerializeField] private AudioSource steps;
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

        if (!lightSteam)
        {
            Debug.LogError("Invalid door fields! Please add the light steam sound.");
            errored = true;
        }

        if (!heavySteam)
        {
            Debug.LogError("Invalid door fields! Please add the heavy steam sound.");
            errored = true;
        }

        if (!steps)
        {
            Debug.LogError("Invalid door fields! Please add the steps sound.");
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
        if (!Player.player) { Debug.LogWarning("Player not found!"); }
        else
        {
            playerCollider = Player.player.GetComponent<BoxCollider>();
            if (!playerCollider) { Debug.LogWarning("Player collider not found!"); }
        }

        canvas = SingleCanvas.canvas;
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
            lightSteam.Play();
        }

        timer += Time.deltaTime;
        if (timer < 1) { return; }

        if (!bigStartParticle)
        {
            bigStartParticle = SpawnParticle(bigParticles, sideBottom);
            heavySteam.Play();
        }

        if (timer >= 2.5f && !smallEndParticle)
        {
            smallEndParticle = SpawnParticle(smallParticles, sideTop);
            lightSteam.Play();
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
        openDoor.SetActive(open);
        if (open) { OpenCutscene(); }

        if (!Player.player || !playerCollider) { return; }
        if (!open || !Collisions.BoxCollisions(openDoor.GetComponent<BoxCollider>()).Contains(playerCollider)) { return; }

        Player.stateQueue.Add(Player.State.Frozen);
        if (FindObjectsByType<FadeRoom>().Length > 0) { return; }

        steps.Play();
        GameObject fadeInstance = Instantiate(fader, canvas.transform);
        fadeInstance.GetComponent<FadeRoom>().targetScene = targetScene;
    }
}
