using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BasicNetworkManager : MonoBehaviourPunCallbacks
{
    public string roomName = "";
    public string levelName = "";
    public bool singlePlayer = false;
    public bool multiplayerPlayer = false;
    public static BasicNetworkManager ins;
    public Transform[] position;

    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //Invoke(nameof(ConnectToMaster), 1f);

    }

    public void SingleMode()
    {
        singlePlayer = true;
        multiplayerPlayer = false;
    }

    public void MultiMode()
    {
        singlePlayer = false;
        multiplayerPlayer = true;
    }

    public void StartNetwork()
    {
        if (!singlePlayer && multiplayerPlayer)
        {
            Debug.Log("Network Manager Start");
            Invoke(nameof(ConnectToMaster), 1f);
        }
    }

    public void ConnectToMaster()
    {
        Debug.Log("Connect To Master Called");

        // Ensure all clients are synced with the scene loaded by the Master Client
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings(); // Automatic connection based on the config file
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        Debug.Log("Connected to master");
    }

    public void LoadMainScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Load the scene only if you're the master client
            PhotonNetwork.LoadLevel(roomName); // Ensure roomName matches the actual scene name
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined Lobby");
        //StartCoroutine(ConnectToRoom());
    }

    public IEnumerator ConnectToRoom()
    {
        while (!PhotonNetwork.IsConnected || !PhotonNetwork.InLobby)
        {
            Debug.Log("Not Connected to master!");
            yield return new WaitForSeconds(2);
        }

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = 8 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        Debug.Log("Joining Room.....");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log($"Master: {PhotonNetwork.IsMasterClient} | Players In Room: {PhotonNetwork.CurrentRoom.PlayerCount} | RoomName: {PhotonNetwork.CurrentRoom.Name} | Region: {PhotonNetwork.CloudRegion}");

        //Invoke("VehicleSpawn", 2);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am the master client. Starting the game.");
            StartCoroutine(WaitForAllPlayersAndLoadScene());
        }
        else
        {
            Debug.Log("I am not the master client. Waiting for the master to load the scene.");
        }
    }

    public void VehicleSpawn()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.Instantiate("Ducati", position[0].position, position[0].rotation);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.Instantiate("Ducati", position[1].position, position[1].rotation);

        }

    }

    private IEnumerator WaitForAllPlayersAndLoadScene()
    {
        // Wait until all players are connected to the room
        while (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("Waiting for more players...");
            yield return new WaitForSeconds(1f);
        }

        // Once all players have joined, load the scene
        LoadMainScene();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogError(cause);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        Debug.Log($"Player {newPlayer.NickName} has entered the room.");

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2) // If you want to start when 2 players join
        {
            LoadMainScene();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Master client has switched to: " + newMasterClient.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I am the new master client. Loading scene.");
            LoadMainScene(); // Ensure the new master loads the scene if required
        }
    }
}
