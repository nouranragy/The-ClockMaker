using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PuzzleTimerTrigger : MonoBehaviourPunCallbacks
{
    public static PuzzleTimerTrigger Instance { get; private set; }

    [Header("Timer Configuration")]
    [Tooltip("Total duration of the timer in seconds.")]
    public float timerDuration = 300f;

    [Header("State")]
    public bool isTimerRunning = false;
    public float timeRemaining;

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

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
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

            // Sync running state across all network clients
            isTimerRunning = started;

            if (started)
            {
                Debug.Log("[PuzzleTimerTrigger] Timer started across network!");
            }
            else
            {
                Debug.Log("[PuzzleTimerTrigger] Timer stopped across network!");
            }
        }
    }

    #region Trigger Event Handlers

    /// <summary>
    /// Overload for calls from PickupItem.cs without parameters.
    /// </summary>
    public void OnItemCollected()
    {
        OnItemPickedUp();
    }

    /// <summary>
    /// Overload for calls from PickupItem.cs passing generic objects or IDs.
    /// </summary>
    public void OnItemCollected(object item)
    {
        OnItemPickedUp();
    }

    /// <summary>
    /// Overload specifically accepting string arguments (e.g., itemID).
    /// </summary>
    public void OnItemCollected(string itemID)
    {
        OnItemPickedUp();
    }

    /// <summary>
    /// Triggered when an item is picked up in the scene.
    /// </summary>
    public void OnItemPickedUp()
    {
        Debug.Log("[PuzzleTimerTrigger] Item picked up/collected! Triggering timer start...");
        TriggerTimerStart();
    }

    /// <summary>
    /// Triggered when the wardrobe puzzle password is typed correctly.
    /// </summary>
    public void OnWardrobePasswordCorrect()
    {
        Debug.Log("[PuzzleTimerTrigger] Wardrobe password correct! Triggering timer start...");
        TriggerTimerStart();

        ScoreManager score = ScoreManager.Instance ?? Object.FindAnyObjectByType<ScoreManager>();
        if (score != null) score.SolvePuzzle1();
    }

    /// <summary>
    /// Triggered when the networked door is opened.
    /// </summary>
    public void OnDoorOpened()
    {
        Debug.Log("[PuzzleTimerTrigger] Door opened! Triggering timer event...");
        TriggerTimerStart();
    }

    #endregion

    #region Timer Logic

    public void TriggerTimerStart()
    {
        if (isTimerRunning) return;

        if (PhotonNetwork.InRoom)
        {
            Hashtable props = new Hashtable
            {
                { TIMER_STARTED_KEY, true },
                { TIMER_START_TIME_KEY, PhotonNetwork.Time }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        StartTimerLocal();
    }

    private void StartTimerLocal()
    {
        isTimerRunning = true;
        Debug.Log("[PuzzleTimerTrigger] Timer started!");
    }

    /// <summary>
    /// Stops the timer locally and updates Photon Room properties.
    /// </summary>
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

    /// <summary>
    /// Overload for scripts calling StopTimer with parameters.
    /// </summary>
    public void StopTimer(object data)
    {
        StopTimer();
    }

    private void OnTimerExpired()
    {
        Debug.LogWarning("[PuzzleTimerTrigger] Time is up!");
        // Add game over / timeout handling here
    }

    #endregion
}