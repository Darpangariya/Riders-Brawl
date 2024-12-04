using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelector : MonoBehaviour
{
    public static CarSelector Instance;
    public bool f1CarBool = false;
    public bool rallyCarBool = false;
    public bool bikeBool = false;
    public bool timeLaps = false;
    public bool raceToAce = false;
    public string vehicleName = string.Empty;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectF1Car()
    {
        f1CarBool = true;
        rallyCarBool = false;
        bikeBool = false;
        vehicleName = "F1";
    }

    public void SelectRallyCar()
    {
        rallyCarBool = true;
        f1CarBool = false;
        bikeBool = false;
        vehicleName = "Rally";
    }

    public void SelectBike()
    {
        f1CarBool = false;
        rallyCarBool = false;
        bikeBool = true;
        vehicleName = "Ducati";
    }

    public void SetTimeLapsMode()
    {
        timeLaps = true;
    }

    public void SetRaceToAceMode()
    {
        raceToAce = true;
    }
}
