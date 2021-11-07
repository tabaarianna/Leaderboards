using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public GameObject leaderBoardCanvas;
    public GameObject[] leaderBoardEntries;

    public static Leaderboard instance;

    private void Awake()
    {
        instance = this;
    }

    public void OnLoggedIn()
    {
        leaderBoardCanvas.SetActive(true);
        DisplayLeaderboard();
    }
    public void DisplayLeaderboard()
    {
        GetLeaderboardRequest getLeaderboardRequest = new GetLeaderboardRequest
        {
            StatisticName = "FastestTime",
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(getLeaderboardRequest,
            results => UpdateLeaderboardUI(results.Leaderboard),
            error => Debug.Log(error.ErrorMessage)
        );
    }

    void UpdateLeaderboardUI(List<PlayerLeaderboardEntry> leaderboard)
    {
        for (int x = 0; x < leaderBoardEntries.Length; x++)
        {
            leaderBoardEntries[x].SetActive(x < leaderboard.Count);

            if (x >= leaderboard.Count) continue;

            leaderBoardEntries[x].transform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = (leaderboard[x].Position + 1) + ". " + leaderboard[x].DisplayName;
            leaderBoardEntries[x].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = (-(float)leaderboard[x].StatValue * 0.001f).ToString("F2");
        }
    }

    public void SetLeaderboardEntry(int newScore)
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest
        {
            FunctionName = "UpdateHighScore",
            FunctionParameter = new { score = newScore }
        };

        PlayFabClientAPI.ExecuteCloudScript(request,
            result => DisplayLeaderboard(),
            error => Debug.Log(error.ErrorMessage)
        );
    }
}



