using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceScoreboard : MonoBehaviour
{
    [SerializeField] private GameObject scoreboardPanel;
    [SerializeField] private Transform scoreboardContent;
    [SerializeField] private GameObject playerEntryPrefab;

    private List<PlayerRaceData> raceResults = new List<PlayerRaceData>();

    // Show final results (end of race)
    public void ShowFinalResults(List<CarLapCounter> players)
    {
        ShowScoreboard(players); // Reuse existing functionality
    }

    // Update scoreboard dynamically during the race (e.g., lap changes)
    public void UpdateScoreboard(List<CarLapCounter> players)
    {
        ShowScoreboard(players); // Optional: Adjust to show partial updates
    }

    public void ShowScoreboard(List<CarLapCounter> players)
    {
        // Clear existing scoreboard entries
        foreach (Transform child in scoreboardContent)
        {
            Destroy(child.gameObject);
        }

        // Populate raceResults
        raceResults.Clear();
        foreach (var player in players)
        {
            var playerData = new PlayerRaceData
            {
                PlayerName = player.name,
                LapsCompleted = player.lapsCompleted,
                BestLapTime = player.bestLapTime,
                TotalRaceTime = player.totalRaceTime
            };
            raceResults.Add(playerData);
        }

        // Sort players by total race time
        raceResults.Sort((a, b) => a.TotalRaceTime.CompareTo(b.TotalRaceTime));

        // Display results on the scoreboard
        foreach (var playerData in raceResults)
        {
            GameObject entry = Instantiate(playerEntryPrefab, scoreboardContent);
            var texts = entry.GetComponentsInChildren<TMP_Text>();

            texts[0].text = playerData.PlayerName;
            texts[1].text = $"{playerData.BestLapTime:0.000}";
            texts[2].text = playerData.LapsCompleted.ToString();
            texts[3].text = $"{playerData.TotalRaceTime:0.000}";
        }

        scoreboardPanel.SetActive(true); // Show the scoreboard panel
    }

    public class PlayerRaceData
    {
        public string PlayerName;
        public int LapsCompleted;
        public float BestLapTime;
        public float TotalRaceTime;
    }

}
