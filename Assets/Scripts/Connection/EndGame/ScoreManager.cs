using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class ScoreManager : MonoBehaviourPunCallbacks
{
    [Header("Dependencies")]
    public UIController uiController;
    [Header("Puzzle Target Times (For 3 Stars)")]
    public float puzzle1TargetTime = 10f;
    public float puzzle2TargetTime = 10f;
    private double p1StartTime;
    private double p2StartTime;
    private float puzzle1TimeSpent;
    private float puzzle2TimeSpent;
    private bool isP1Running = false;
    private bool isP2Running = false;
    private void Start()
    {
        if (uiController != null) uiController.HidePanel();
    }
    public void StartPuzzle1()
    {
        if (isP1Running) return;
        photonView.RPC(nameof(RPC_StartPuzzle1), RpcTarget.All, PhotonNetwork.Time);
    }
    [PunRPC]
    private void RPC_StartPuzzle1(double startTime)
    {
        p1StartTime = startTime;
        isP1Running = true;
    }
    public void SolvePuzzle1()
    {
        if (!isP1Running) return;
        photonView.RPC(nameof(RPC_SolvePuzzle1), RpcTarget.All, PhotonNetwork.Time);
    }
    [PunRPC]
    private void RPC_SolvePuzzle1(double stopTime)
    {
        isP1Running = false;
        puzzle1TimeSpent = (float)(stopTime - p1StartTime);
    }
    public void StartPuzzle2()
    {
        if (isP2Running) return;
        photonView.RPC(nameof(RPC_StartPuzzle2), RpcTarget.All, PhotonNetwork.Time);
    }
    [PunRPC]
    private void RPC_StartPuzzle2(double startTime)
    {
        p2StartTime = startTime;
        isP2Running = true;
    }
    public float GetCurrentActiveTime()
    {
        if (isP1Running) return (float)(PhotonNetwork.Time - p1StartTime);
        else if (isP2Running) return (float)(PhotonNetwork.Time - p2StartTime);
        return 0f;
    }
    public void SolvePuzzle2AndOpenDoor()
    {
        if (!isP2Running) return;
        photonView.RPC(nameof(RPC_SolvePuzzle2AndFinish), RpcTarget.All, PhotonNetwork.Time);
    }
    [PunRPC]
    private void RPC_SolvePuzzle2AndFinish(double stopTime)
    {
        isP2Running = false;
        puzzle2TimeSpent = (float)(stopTime - p2StartTime);
        FinishGame();
    }
    private void FinishGame()
    {
        string p1 = PhotonNetwork.PlayerList.Length > 0 ? PhotonNetwork.PlayerList[0].NickName : "Player 1";
        string p2 = PhotonNetwork.PlayerList.Length > 1 ? PhotonNetwork.PlayerList[1].NickName : "Player 2";
        if (string.IsNullOrEmpty(p1)) p1 = "Player 1";
        if (string.IsNullOrEmpty(p2)) p2 = "Player 2";
        string combinedNames = $"{p1}, {p2}";
        LevelResult currentResult = new LevelResult()
        {
            playerNames = combinedNames,
            puzzle1TargetTime = puzzle1TargetTime,
            puzzle2TargetTime = puzzle2TargetTime,
            puzzle1CompletedTime = puzzle1TimeSpent,
            puzzle2CompletedTime = puzzle2TimeSpent
        };
        float previousBestAvg = PlayerPrefs.GetFloat("HighScore_Avg", float.MaxValue);
        bool isNewBest = currentResult.AverageTime < previousBestAvg;
        if (isNewBest)
        {
            PlayerPrefs.SetFloat("HighScore_Avg", currentResult.AverageTime);
            PlayerPrefs.SetString("HighScore_Players", combinedNames);
            PlayerPrefs.Save();
        }
        LevelResult globalBest = new LevelResult()
        {
            playerNames = PlayerPrefs.GetString("HighScore_Players", combinedNames),
            puzzle1CompletedTime = PlayerPrefs.GetFloat("HighScore_Avg", currentResult.AverageTime) / 2f,
            puzzle2CompletedTime = PlayerPrefs.GetFloat("HighScore_Avg", currentResult.AverageTime) / 2f
        };
        if (uiController != null) uiController.DisplayPanel(currentResult, globalBest, isNewBest);
    }
}