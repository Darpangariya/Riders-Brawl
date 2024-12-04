using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool isFinishLine = false;
    public bool isStartLine = false;

    public int checkPointNumbers = 1;

    public CheckPoint nextCheckPoint;

    // Enable this checkpoint
    public void ActivateCheckPoint()
    {
        gameObject.SetActive(true);
    }

    // Disable this checkpoint
    public void DeactivateCheckPoint()
    {
        gameObject.SetActive(false);
    }
}
