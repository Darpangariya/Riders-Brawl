using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RaceFinishedUI : MonoBehaviour
{

    [SerializeField] Transform finishPanel;
    [SerializeField] Text finishPosition;
    [SerializeField] CarLapCounter carLapCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FinishRacePanel();
    }

    void FinishRacePanel()
    {
        if (carLapCounter.isRaceCompleted)
        {
            finishPanel.gameObject.SetActive(true);
            finishPosition.text = "You Finished the race at" + carLapCounter.carPositionText.text + "position";
        }
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine("ReturnToMainMenuCO");
    }

    public void Restart()
    {
        StartCoroutine("RestartCO");
    }

    public void QuitGame()
    {
        StartCoroutine("QuitGameCO");
    }

    IEnumerator ReturnToMainMenuCO()
    {
        yield return new WaitForSeconds(2);
        Application.LoadLevel(0);
    }

    IEnumerator RestartCO()
    {
        yield return new WaitForSeconds(2);
        Application.LoadLevel(2);
    }

    IEnumerator QuitGameCO()
    {
        yield return new WaitForSeconds(2);
        Application.Quit();
    }

}
