using System;
using TMPro;
using UnityEngine;

public class CarLapCounter : MonoBehaviour
{
    // Checkpoint and lap tracking
    int passedCheckPointNumber = 0;
    public int numberOfPassedCheckPoints = 0;
    public int lapsCompleted = 0;
    public int lapsToComplete = 1;
    public bool isRaceCompleted = false;

    // UI Elements
    public TMP_Text carPositionText;
    public Transform positionTextPanel;
    [SerializeField] private TMP_Text[] timeText; // Time display text array

    // Timing
    public float bestCheckPointTime = 0;
    public float bestLapTime = 0;
    public float currentLapTime = 0;
    public float currentCheckPointTime = 0.0f;
    public float timeAtLastPassedCheckPoint = 0;
    public float totalRaceTime = 0.0f;

    // Race start timer
    public bool raceStarted = false;
    public bool isPlayerVehicle = false; // Set this to true only for the player’s vehicle

    // Car position tracking
    private int carPosition = 0;

    // Events
    public event Action<CarLapCounter> OnPassCheckPoint;

    void Start()
    {
        CheckPoint[] checkpoints = FindObjectsOfType<CheckPoint>();

        if (checkpoints.Length > 0)
        {
            checkpoints[0].ActivateCheckPoint(); // Start with the first checkpoint
        }

        // Register this car with the RaceManager
        RaceManager.Instance.RegisterPlayer(this);
    }

    private void Update()
    {
        if (TimeCountdown.instance.raceStarted)
        {
            UpdateLapTimes();
            if (CarSelector.Instance.bikeBool)
            {
                MyBikeControll vc = GetComponent<MyBikeControll>();
                vc.activeControl = true;
            }
            else if (!CarSelector.Instance.bikeBool && CarSelector.Instance.f1CarBool)
            {
                VehicleControl vc = GetComponent<VehicleControl>();
                vc.activeControl = true;
            }
        }
    }

    private void UpdateLapTimes()
    {
        if (isRaceCompleted) return; // Stop updating times if race is completed

        currentLapTime += Time.deltaTime;
        currentCheckPointTime += Time.deltaTime;
        totalRaceTime += Time.deltaTime;
        timeText[0].text = $"Time: {FormatTime(currentLapTime)}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint") && !isRaceCompleted)
        {
            CheckPoint checkPoint = other.GetComponent<CheckPoint>();

            if (passedCheckPointNumber + 1 == checkPoint.checkPointNumbers)
            {
                HandleCheckpoint(checkPoint);
            }
        }
    }

    private void HandleCheckpoint(CheckPoint checkPoint)
    {
        passedCheckPointNumber = checkPoint.checkPointNumbers;
        numberOfPassedCheckPoints++;
        timeAtLastPassedCheckPoint = Time.time;

        // Update checkpoint and lap time
        if (bestCheckPointTime == 0 || bestCheckPointTime >= currentCheckPointTime)
        {
            bestCheckPointTime = currentCheckPointTime;
            timeText[1].text = $"Best Time: {bestCheckPointTime:0.000}";
        }

        if (checkPoint.isFinishLine)
        {
            passedCheckPointNumber = 0;
            lapsCompleted++;
            if (bestLapTime == 0 || bestLapTime >= currentLapTime)
                bestLapTime = currentLapTime;

            if (lapsCompleted >= lapsToComplete)
            {
                isRaceCompleted = true;
                timeText[3].text = $"Total Lap Time: {FormatTime(totalRaceTime)}";
                totalRaceTime = 0;

                // Notify RaceManager
                RaceManager.Instance.PlayerFinished(this);
            }

            timeText[2].text = $"Lap Time: {FormatTime(bestLapTime)}";
            currentLapTime = 0;
        }

        currentCheckPointTime = 0;
        OnPassCheckPoint?.Invoke(this); // Trigger event
        ShowCarPosition();
    }

    private void ShowCarPosition()
    {
        carPositionText.text = "Pos: " + carPosition.ToString();
    }

    public void SetCarPosition(int position)
    {
        carPosition = position;
    }

    public int GetLapsCompleted() => lapsCompleted;
    public float GetBestLapTime() => bestLapTime;
    public float GetTotalRaceTime() => totalRaceTime;
    public float GetTimeAtLastCheckPoint()
    {
        return timeAtLastPassedCheckPoint;
    }

    public int NumbersOfCheckPointPassed()
    {
        return numberOfPassedCheckPoints;
    }
    private string FormatTime(float time)
    {
        return $"{Mathf.FloorToInt(time / 60)}:{time % 60:00.000}";
    }
}
