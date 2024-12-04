using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{

    public List<CarLapCounter> carLapCounters = new List<CarLapCounter>();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(10);
        CarLapCounter[] carLapCounterArray = FindObjectsOfType<CarLapCounter>();

        carLapCounters = carLapCounterArray.ToList<CarLapCounter>();

        foreach (CarLapCounter lapCounter in carLapCounters)
            lapCounter.OnPassCheckPoint += OnPassedCheckPoint;

    }

   void OnPassedCheckPoint(CarLapCounter carLapCounter)
    {
        carLapCounters = carLapCounters.OrderByDescending(s => s.NumbersOfCheckPointPassed()).ThenBy(s => s.GetTimeAtLastCheckPoint()).ToList();

        int carPosition = carLapCounters.IndexOf(carLapCounter) + 1;

        carLapCounter.SetCarPosition(carPosition);
    }
}
