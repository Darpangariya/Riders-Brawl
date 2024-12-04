using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    private List<CarLapCounter> allPlayers = new List<CarLapCounter>();
    private List<CarLapCounter> finishedPlayers = new List<CarLapCounter>();

    [SerializeField] private RaceScoreboard scoreboard; // Reference to a UI element for showing results (optional)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Register a new player in the race.
    /// </summary>
    public void RegisterPlayer(CarLapCounter player)
    {
        if (!allPlayers.Contains(player))
        {
            allPlayers.Add(player);
        }
    }

    /// <summary>
    /// Handle logic when a player finishes the race.
    /// </summary>
    public void PlayerFinished(CarLapCounter player)
    {
        if (!finishedPlayers.Contains(player))
        {
            finishedPlayers.Add(player);

            Debug.Log($"Player {player.name} finished! Position: {finishedPlayers.Count}");

            // Optionally update a scoreboard or UI here
            if (scoreboard != null)
            {
                scoreboard.UpdateScoreboard(finishedPlayers);
            }
        }

        // Check if all players have finished the race
        if (finishedPlayers.Count == allPlayers.Count)
        {
            Debug.Log("All players have finished the race!");
            EndRace();
        }
    }

    /// <summary>
    /// Called when the race ends.
    /// </summary>
    private void EndRace()
    {
        Debug.Log("Race completed!");
        // You can add logic to handle race-end events, like showing final results.
        if (scoreboard != null)
        {
            scoreboard.ShowFinalResults(allPlayers);
        }
    }
}
