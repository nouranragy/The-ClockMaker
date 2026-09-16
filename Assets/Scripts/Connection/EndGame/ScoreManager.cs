using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class ScoreManager : MonoBehaviourPunCallbacks
{
    public UIController uiController;

    public float puzzle1TargetTime = 30f;
    public float puzzle2TargetTime = 45f;

    private float levelStartTime;
    private float puzzle1TimeSpent;
    private float puzzle2TimeSpent;

    private void Start()
    {
        if (uiController != null)
        {
            uiController.HidePanel();
        }

        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_StartTimer), RpcTarget.All, (float)PhotonNetwork.Time);
        }
    }

    [PunRPC]
    private void RPC_StartTimer(float networkStartTime)
    {
        levelStartTime = networkStartTime;
    }

    public void SolvePuzzle1()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        float p1Time = (float)(PhotonNetwork.Time - levelStartTime);
        photonView.RPC(nameof(RPC_Puzzle1Completed), RpcTarget.All, p1Time);
    }

    [PunRPC]
    private void RPC_Puzzle1Completed(float p1TimeSpent)
    {
        puzzle1TimeSpent = p1TimeSpent;
    }

    public void SolvePuzzle2AndOpenDoors()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        float totalTime = (float)(PhotonNetwork.Time - levelStartTime);
        float p2Time = totalTime - puzzle1TimeSpent;

        photonView.RPC(nameof(RPC_FinishGame), RpcTarget.All, puzzle1TimeSpent, p2Time);
    }

    [PunRPC]
    private void RPC_FinishGame(float p1Time, float p2Time)
    {
        string player1 = PhotonNetwork.PlayerList.Length > 0 ? PhotonNetwork.PlayerList[0].NickName : "Player 1";
        string player2 = PhotonNetwork.PlayerList.Length > 1 ? PhotonNetwork.PlayerList[1].NickName : "Player 2";
        string combinedNames = $"{player1}, {player2}";

        LevelResult result = new LevelResult()
        {
            playerNames = combinedNames,
            puzzle1TargetTime = puzzle1TargetTime,
            puzzle2TargetTime = puzzle2TargetTime,
            puzzle1CompletedTime = p1Time,
            puzzle2CompletedTime = p2Time
        };

        float bestAvg = PlayerPrefs.GetFloat("HighScore_Avg", float.MaxValue);
        bool isNewBest = result.AverageTime < bestAvg;

        if (isNewBest)
        {
            PlayerPrefs.SetFloat("HighScore_Avg", result.AverageTime);
            PlayerPrefs.SetString("HighScore_Players", combinedNames);
            PlayerPrefs.Save();
        }

        LevelResult globalBest = new LevelResult()
        {
            playerNames = PlayerPrefs.GetString("HighScore_Players", combinedNames),
            puzzle1CompletedTime = PlayerPrefs.GetFloat("HighScore_Avg", result.AverageTime),
            puzzle2CompletedTime = PlayerPrefs.GetFloat("HighScore_Avg", result.AverageTime)
        };

        uiController.DisplayPanel(result, globalBest, isNewBest);
    }
}