using Unity.VisualScripting;
using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    [SerializeField] private GameObject hourPivot;
    [SerializeField] private GameObject minutePivot;
    [SerializeField] private GameObject secondPivot;
    [SerializeField] private AudioSource tick1;
    [SerializeField] private AudioSource tick2;

    private int prevSecond = 0;

    private void Awake()
    {
        bool errored = false;

        if (!hourPivot)
        {
            Debug.LogError("Invalid clock fields! Please add the hour pivot.");
            errored = true;
        }

        if (!minutePivot)
        {
            Debug.LogError("Invalid clock fields! Please add the minute pivot.");
            errored = true;
        }

        if (!secondPivot)
        {
            Debug.LogError("Invalid clock fields! Please add the second pivot.");
            errored = true;
        }

        if (!tick1)
        {
            Debug.LogError("Invalid clock fields! Please add the tick1 sound.");
            errored = true;
        }

        if (!tick2)
        {
            Debug.LogError("Invalid clock fields! Please add the tick2 sound.");
            errored = true;
        }

        if (errored) { Destroy(this); return; }

        prevSecond = DateTime.Now.Second;
    }

    private void Update()
    {
        DateTime current = DateTime.Now;
        float hourAngle = -360 * current.Hour/12;
        float minuteAngle = -360 * current.Minute/60;
        float secondAngle = -360 * current.Second/60;
        hourPivot.transform.localRotation = Quaternion.Euler(0, hourAngle + minuteAngle/60 + secondAngle/3600, 0);
        minutePivot.transform.localRotation = Quaternion.Euler(0, minuteAngle + secondAngle/60, 0);
        secondPivot.transform.localRotation = Quaternion.Euler(0, secondAngle, 0);

        if (prevSecond != current.Second)
        {
            if (prevSecond % 2 == 0) { tick1.Play(); }
            else { tick2.Play(); }
        }

        prevSecond = current.Second;
    }
}
