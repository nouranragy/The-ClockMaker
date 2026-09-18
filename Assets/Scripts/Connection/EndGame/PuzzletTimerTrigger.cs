using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class PuzzleTimerTrigger : MonoBehaviourPunCallbacks
{
    public static PuzzleTimerTrigger Instance { get; private set; }

    [Header("Timer Configuration")]
    [Tooltip("Total duration of the timer in seconds for each puzzle.")]
    public float timerDuration = 300f;

    [Header("State")]
    public bool isTimerRunning = false;
    public float timeRemaining;

    public List<string> Puzzle1itemIds = new List<string>();
    public List<string> Puzzle2itemIds = new List<string>();

    private const string TIMER_STARTED_KEY = "PuzzleTimerStarted";
    private const string TIMER_START_TIME_KEY = "PuzzleTimerStartTime";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        timeRemaining = timerDuration;
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        // Sync remaining time directly using Photon's network time
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TIMER_START_TIME_KEY, out object startTimeObj))
        {
            double startTime = (double)startTimeObj;
            double elapsedTime = PhotonNetwork.Time - startTime;
            timeRemaining = Mathf.Max(0f, timerDuration - (float)elapsedTime);
        }
        else
        {
            timeRemaining -= Time.deltaTime;
        }

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isTimerRunning = false;
            OnTimerExpired();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(TIMER_STARTED_KEY))
        {
            bool started = (bool)propertiesThatChanged[TIMER_STARTED_KEY];
            isTimerRunning = started;

            if (started)
            {
                Debug.Log("[PuzzleTimerTrigger] Network timer started!");
            }
            else
            {
                Debug.Log("[PuzzleTimerTrigger] Network timer stopped!");
            }
        }
    }

    #region Trigger Event Handlers

    public void OnItemCollected()
    {
        OnItemPickedUp();
    }

    public void OnItemCollected(object item)
    {
        OnItemPickedUp();
    }

    public void OnItemCollected(string itemID)
    {
        Debug.Log($"[PuzzleTimerTrigger] Item picked up: {itemID}");

        if (Puzzle1itemIds.Contains(itemID))
        {
            TriggerTimerStart(1);
        }
        else if (Puzzle2itemIds.Contains(itemID))
        {
            TriggerTimerStart(2);
        }
        else
        {
            TriggerTimerStart(1);
        }
    }

    public void OnItemPickedUp()
    {
        Debug.Log("[PuzzleTimerTrigger] Item picked up/collected! Triggering timer start...");
        TriggerTimerStart(1);
    }

    public void OnWardrobePasswordCorrect()
    {
        Debug.Log("[PuzzleTimerTrigger] Wardrobe password correct! Stopping Puzzle 1 timer...");

        ScoreManager score = ScoreManager.Instance ?? Object.FindFirstObjectByType<ScoreManager>();
        if (score != null) score.SolvePuzzle1();

        StopTimer();
    }

    public void OnDoorOpened()
    {
        Debug.Log("[PuzzleTimerTrigger] Door opened! Stopping Puzzle 2 timer...");
        StopTimer();
    }

    #endregion

    #region Timer Logic

    public void TriggerTimerStart(int puzzleNumber = 1)
    {
        // DO NOT restart if the timer is already running for the active puzzle
        if (isTimerRunning) return;

        timeRemaining = timerDuration;

        if (PhotonNetwork.InRoom)
        {
            Hashtable props = new Hashtable
            {
                { TIMER_STARTED_KEY, true },
                { TIMER_START_TIME_KEY, PhotonNetwork.Time }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        isTimerRunning = true;

        ScoreManager score = ScoreManager.Instance ?? Object.FindFirstObjectByType<ScoreManager>();
        if (score != null)
        {
            if (puzzleNumber == 1)
            {
                score.StartPuzzle1();
            }
            else if (puzzleNumber == 2)
            {
                score.StartPuzzle2();
            }
        }
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log("[PuzzleTimerTrigger] Timer stopped!");

        if (PhotonNetwork.InRoom)
        {
            Hashtable props = new Hashtable { { TIMER_STARTED_KEY, false } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    public void StopTimer(object data)
    {
        StopTimer();
    }

    private void OnTimerExpired()
    {
        Debug.LogWarning("[PuzzleTimerTrigger] Time is up!");
    }

    #endregion
}