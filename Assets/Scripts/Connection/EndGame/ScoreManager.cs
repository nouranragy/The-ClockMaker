using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance;

    [Header("Dependencies")]
    public UIController uiController;

    [Header("Puzzle Target Times (3 Min Each = 180s)")]
    public float puzzle1TargetTime = 180f;
    public float puzzle2TargetTime = 180f;

    private double p1StartTime;
    private double p2StartTime;
    private float puzzle1TimeSpent;
    private float puzzle2TimeSpent;
    private bool isP1Running = false;
    private bool isP2Running = false;

    private void Awake()
    {
        transform.SetParent(null);

        if (GetComponent<PhotonView>() != null)
        {
            PhotonNetwork.RegisterPhotonView(GetComponent<PhotonView>());
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        uiController = Object.FindFirstObjectByType<UIController>(FindObjectsInactive.Include);

        if (uiController != null) uiController.HidePanel();
        if (scene.name == "Lobby") ResetPuzzleState();
    }

    public void ResetPuzzleState()
    {
        isP1Running = false;
        isP2Running = false;
        p1StartTime = 0;
        p2StartTime = 0;
        puzzle1TimeSpent = 0;
        puzzle2TimeSpent = 0;
    }

    // --- PUZZLE 1 ---
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
        Debug.Log($"[ScoreManager] Puzzle 1 Started at {p1StartTime}");
    }

    public void SolvePuzzle1()
    {
        if (!isP1Running) return;
        photonView.RPC(nameof(RPC_SolvePuzzle1), RpcTarget.All, PhotonNetwork.Time);
    }

    [PunRPC]
    private void RPC_SolvePuzzle1(double stopTime)
    {
        if (isP1Running)
        {
            puzzle1TimeSpent = (float)(stopTime - p1StartTime);
            isP1Running = false;
        }
        Debug.Log($"[ScoreManager] Puzzle 1 Solved in: {puzzle1TimeSpent}s");
    }

    // --- PUZZLE 2 ---
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
        Debug.Log($"[ScoreManager] Puzzle 2 Started at {p2StartTime}");
    }

    public void SolvePuzzle2AndOpenDoor()
    {
        photonView.RPC(nameof(RPC_SolvePuzzle2AndFinish), RpcTarget.All, PhotonNetwork.Time);
    }

    [PunRPC]
    private void RPC_SolvePuzzle2AndFinish(double stopTime)
    {
        // Strictly calculate Puzzle 2 independently from p2StartTime
        if (isP2Running && p2StartTime > 0)
        {
            puzzle2TimeSpent = (float)(stopTime - p2StartTime);
        }
        else
        {
            // Safeguard: If StartPuzzle2 was never triggered, measure time spent on Puzzle 2
            puzzle2TimeSpent = (float)(stopTime - (p1StartTime + puzzle1TimeSpent));
        }

        isP2Running = false;
        Debug.Log($"[ScoreManager] Puzzle 2 Solved in: {puzzle2TimeSpent}s");

        FinishGame();
    }

    public float GetCurrentActiveTime()
    {
        if (isP1Running) return Mathf.Max(0f, puzzle1TargetTime - (float)(PhotonNetwork.Time - p1StartTime));
        if (isP2Running) return Mathf.Max(0f, puzzle2TargetTime - (float)(PhotonNetwork.Time - p2StartTime));
        return 0f;
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
            puzzle2CompletedTime = PlayerPrefs.GetFloat("HighScore_Avg", currentResult.AverageTime) / 2f,
            puzzle1TargetTime = puzzle1TargetTime,
            puzzle2TargetTime = puzzle2TargetTime
        };

        if (uiController == null)
        {
            uiController = Object.FindFirstObjectByType<UIController>(FindObjectsInactive.Include);
        }

        if (uiController != null)
        {
            uiController.gameObject.SetActive(true);
            uiController.DisplayPanel(currentResult, globalBest, isNewBest);
        }
        else
        {
            Debug.LogError("[ScoreManager] UIController reference is missing in scene!");
        }
    }
}