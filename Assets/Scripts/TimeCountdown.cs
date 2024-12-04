using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCountdown : MonoBehaviour
{
    public static TimeCountdown instance;
    public Light[] signalLights;
    public float startTime;
    bool lightsInitialized;
    public bool raceStarted;


    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        Timer();
    }

    public void Timer()
    {
        startTime = Mathf.Max(0, startTime - Time.deltaTime); // Reduce start time and prevent negative values
        int sec = Mathf.FloorToInt(startTime); // Convert time to seconds

        if (!lightsInitialized)
        {
            InitializeLights(); // Ensure lights are initialized only once
            lightsInitialized = true;
        }

        switch (sec)
        {
            case 3:
                Debug.Log("3 seconds remaining");
                SetLights(true, true, true); // Turn on all lights
                break;

            case 2:
                Debug.Log("2 seconds remaining");
                SetLights(false, true, true); // Only second and third lights
                break;

            case 1:
                Debug.Log("1 second remaining");
                SetLights(false, false, true); // Only the third light
                break;

            case 0:
                if (!raceStarted)
                {
                    StartRace(); // Trigger race start logic
                    raceStarted = true;
                }
                break;
        }
    }

    // Start race logic: handles enabling controls
    private void StartRace()
    {
        Debug.Log("Race Started!");
        SetLights(true, true, true); // Turn on all lights
    }

    private void InitializeLights()
    {
        foreach (Light c in signalLights)
        {
            c.enabled = false; // Turn off all lights initially
        }
    }

    // Set specific lights on/off
    private void SetLights(bool light1, bool light2, bool light3)
    {
        signalLights[0].enabled = light1;
        signalLights[1].enabled = light2;
        signalLights[2].enabled = light3;
    }
}
