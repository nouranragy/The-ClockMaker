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
        // Detach from parent so DontDestroyOnLoad works
        transform.SetParent(null);

        // Singleton check: If an instance already exists, destroy this duplicate IMMEDIATELY
        if (Instance != null && Instance != this)
        {
            Debug.Log("[ScoreManager] Duplicate ScoreManager detected on scene load. Destroying duplicate.");
          PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
        {
            pv.ViewID = 0; // إزالة الـ ID المكرر حتى لا يتعارض مع الشبكة
        }
            Destroy(gameObject);
            return; // Exit early so PhotonView registration doesn't run on the duplicate!
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // if (GetComponent<PhotonView>() != null)
        // {
        //     PhotonNetwork.RegisterPhotonView(GetComponent<PhotonView>());
        // }
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
        // Re-bind the UIController every time any scene loads
        FindAndAssignUIController();

        if (uiController != null)
        {
            uiController.HidePanel();
        }

        // Reset timer states when returning to Lobby or gameplay room
        ResetPuzzleState();
    }

    private void FindAndAssignUIController()
    {
        // Search including inactive game objects
        uiController = Object.FindFirstObjectByType<UIController>(FindObjectsInactive.Include);

        if (uiController != null)
        {
            Debug.Log("[ScoreManager] UIController successfully found and assigned!");
        }
        else
        {
            // Only log a warning if we are in the gameplay scene, not the Lobby/Exit scenes
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene != "Lobby" && currentScene != "exit game")
            {
                Debug.LogWarning($"[ScoreManager] Could not find UIController in scene: {currentScene}");
            }
        }
    }

    public void ResetPuzzleState()
    {
        isP1Running = false;
        isP2Running = false;
        p1StartTime = 0;
        p2StartTime = 0;
        puzzle1TimeSpent = 0;
        puzzle2TimeSpent = 0;
        Debug.Log("[ScoreManager] State successfully reset for next play session.");
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
        if (isP2Running && p2StartTime > 0)
        {
            puzzle2TimeSpent = (float)(stopTime - p2StartTime);
        }
        else
        {
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
        if (uiController == null)
        {
            FindAndAssignUIController();
        }

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

        // Safety fallback: Ensure we re-locate the UIController before displaying
        if (uiController == null)
        {
            FindAndAssignUIController();
        }

        if (uiController != null)
        {
            uiController.gameObject.SetActive(true);
            uiController.DisplayPanel(currentResult, globalBest, isNewBest);
        }
        else
        {
            Debug.LogError("[ScoreManager] UIController reference is missing in scene! Cannot show end panel.");
        }
    }
}