using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class CarSpawner : MonoBehaviourPunCallbacks
{
    public static CarSpawner instance;

    [System.Serializable]
    public class BotVehicles
    {
        public GameObject player;
        public GameObject botPlayersBike;
        public GameObject botPlayersCar;
        public int botCount = 5;
    }

    public BotVehicles botVehicles;
    public VehiclePrefabs vehiclePrefabs;

    [Header("Spawn Positions")]
    public Transform spawnOne;
    public Transform spawnTwo;
    public Transform spawnThree;
    public Transform spawnFour;
    public Transform spawnFive;
    public Transform spawnSix;

    private List<Transform> availableSpawnPositions;

    private void InitializeSpawnPositions()
    {
        availableSpawnPositions = new List<Transform> { spawnOne, spawnTwo, spawnThree, spawnFour, spawnFive, spawnSix };
    }

    private Vector3 CalcSpawnPos()
    {
        if (availableSpawnPositions.Count == 0)
        {
            Debug.LogWarning("No more available spawn positions!");
            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, availableSpawnPositions.Count);
        Transform chosenSpawn = availableSpawnPositions[randomIndex];
        availableSpawnPositions.RemoveAt(randomIndex);
        return chosenSpawn.position;
    }

    [System.Serializable]
    public class VehiclePrefabs
    {
        public Transform f1;
        public Transform ducati;
        public Transform rally;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);

        Debug.Log("Scene is loaded. Spawning vehicles...");

        InitializeSpawnPositions();
        Vector3 spawnPos = CalcSpawnPos();

        // Multiplayer spawning logic
        if (BasicNetworkManager.ins.multiplayerPlayer && !BasicNetworkManager.ins.singlePlayer)
        {
            yield return new WaitForSeconds(5);
           PhotonNetwork.Instantiate(CarSelector.Instance.vehicleName, spawnPos, spawnOne.rotation, 0);

        }

        // Single player spawning logic
        else if (BasicNetworkManager.ins.singlePlayer && !BasicNetworkManager.ins.multiplayerPlayer)
        {
            yield return new WaitForSeconds(0.1f);
            HandleVehicleSpawning(spawnPos);
        }
    }

    private void HandleVehicleSpawning(Vector3 spawnPos)
    {
        if (CarSelector.Instance.timeLaps)
        {
            SpawnPlayerLocal(spawnPos);
        }
        else if (CarSelector.Instance.raceToAce)
        {
            SpawnPlayerLocal(spawnPos);
            SpawnBotPlayers(spawnPos);
        }
    }

    private void SpawnBotPlayers(Vector3 spawnPos)
    {
        for (int i = 0; i < botVehicles.botCount; i++)
        {
            if (CarSelector.Instance.bikeBool)
            {
                Instantiate(botVehicles.botPlayersBike, CalcSpawnPos(), spawnOne.rotation);
            }
            else if (CarSelector.Instance.f1CarBool || CarSelector.Instance.rallyCarBool)
            {
                Instantiate(botVehicles.botPlayersCar, CalcSpawnPos(), spawnOne.rotation);
            }
        }
    }

    void SpawnPlayerLocal(Vector3 spawnPos)
    {
        if (CarSelector.Instance.bikeBool)
        {
            botVehicles.player = Instantiate(vehiclePrefabs.ducati.gameObject, spawnPos, spawnOne.rotation);
        }
        else if (CarSelector.Instance.f1CarBool)
        {
            botVehicles.player = Instantiate(vehiclePrefabs.f1.gameObject, spawnPos, spawnOne.rotation);
        }
        else if (CarSelector.Instance.rallyCarBool)
        {
            botVehicles.player = Instantiate(vehiclePrefabs.rally.gameObject, spawnPos, spawnOne.rotation);

        }
    }

}
