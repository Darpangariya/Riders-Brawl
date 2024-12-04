using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Image progressBar;
    public GameObject camera;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CityMap()
    {
        Debug.Log("City");
        StartCoroutine(CityMapC());
    }

    public void RaceTrack()
    {
        Debug.Log("RaceTrack");
        //StartCoroutine(RaceTrackC());
    }

    public void CityNight()
    {
        StartCoroutine(CityNightC());
    }

    public void QuitGame()
    {
        Debug.LogError("Working");
        Application.Quit();
    }

    private IEnumerator CityMapC()
    {
        if (BasicNetworkManager.ins.multiplayerPlayer && !BasicNetworkManager.ins.singlePlayer)
        {
            BasicNetworkManager.ins.roomName = "City";
            yield return new WaitForSeconds(5);
            StartCoroutine(BasicNetworkManager.ins.ConnectToRoom());
        }
        else if (!BasicNetworkManager.ins.multiplayerPlayer && BasicNetworkManager.ins.singlePlayer)
        {
            StartCoroutine(LoadSceneWithProgress("City"));
        }
    }

    public void RaceTrackC()
    {
        if (BasicNetworkManager.ins.multiplayerPlayer && !BasicNetworkManager.ins.singlePlayer)
        {
            BasicNetworkManager.ins.roomName = "RaceTrack";
            //yield return new WaitForSeconds(5);
            StartCoroutine(BasicNetworkManager.ins.ConnectToRoom());
        }
        else if (!BasicNetworkManager.ins.multiplayerPlayer && BasicNetworkManager.ins.singlePlayer)
        {
            StartCoroutine(LoadSceneWithProgress("RaceTrack"));
        }
    }

    private IEnumerator CityNightC()
    {
        if (BasicNetworkManager.ins.multiplayerPlayer && !BasicNetworkManager.ins.singlePlayer)
        {
            BasicNetworkManager.ins.roomName = "CityNight";
            yield return new WaitForSeconds(5);
            StartCoroutine(BasicNetworkManager.ins.ConnectToRoom());
        }
        else if (!BasicNetworkManager.ins.multiplayerPlayer && BasicNetworkManager.ins.singlePlayer)
        {
            StartCoroutine(LoadSceneWithProgress("CityNight"));
        }
    }

    private IEnumerator LoadSceneWithProgress(string sceneName)
    {
        progressBar.fillAmount = 0;

        // Use Unity's AsyncOperation to get the actual loading progress
        //AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
        //while (!operation.isDone)
        //{
        //    float progress = Mathf.Clamp01(operation.progress / 0.9f);
        //    progressBar.fillAmount = progress;
        //    yield return null;
        //}
    }
}
