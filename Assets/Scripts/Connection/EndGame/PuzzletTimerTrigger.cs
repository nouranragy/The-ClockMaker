using System.Collections.Generic;
using UnityEngine;

public class PuzzleTimerTrigger : MonoBehaviour
{
    public static PuzzleTimerTrigger Instance;

    [Header("Dependencies")]
    public ScoreManager scoreManager;

    [Header("Puzzle 1 Setup (Candles & Wardrobe)")]
    [Tooltip("List of item IDs/names that trigger Puzzle 1 Timer on first pickup")]
    public List<string> puzzle1StartItemIDs = new List<string>();

    [Header("Puzzle 2 Setup (Boxes & Door)")]
    [Tooltip("List of item IDs/names that trigger Puzzle 2 Timer on first pickup")]
    public List<string> puzzle2StartItemIDs = new List<string>();

    private bool isP1TimerStarted = false;
    private bool isP2TimerStarted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (scoreManager == null)
        {
            scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        }
    }

    // --- START TIMERS (ITEM PICKUPS) ---

    /// <summary>
    /// Call this whenever an item is collected/picked up in your game.
    /// </summary>
    public void OnItemCollected(string itemID)
    {
        if (scoreManager == null || string.IsNullOrEmpty(itemID)) return;

        string cleanID = itemID.Trim();

        // Check Puzzle 1 Items (Candles)
        if (!isP1TimerStarted && ContainsID(puzzle1StartItemIDs, cleanID))
        {
            isP1TimerStarted = true;
            scoreManager.StartPuzzle1();
            Debug.Log($"[Timer Trigger] Puzzle 1 Timer Started by collecting: '{itemID}'");
        }
        // Check Puzzle 2 Items (Boxes)
        else if (!isP2TimerStarted && ContainsID(puzzle2StartItemIDs, cleanID))
        {
            isP2TimerStarted = true;
            scoreManager.StartPuzzle2();
            Debug.Log($"[Timer Trigger] Puzzle 2 Timer Started by collecting: '{itemID}'");
        }
    }

    // --- STOP TIMERS (WARDROBE PASSWORD & DOOR CLICK) ---

    /// <summary>
    /// Call this when the Wardrobe password is correctly entered for Puzzle 1.
    /// </summary>
    public void OnWardrobePasswordCorrect()
    {
        if (scoreManager == null) return;

        scoreManager.SolvePuzzle1();
        Debug.Log("[Timer Trigger] Puzzle 1 Timer Stopped (Wardrobe Unlocked).");
    }

    /// <summary>
    /// Call this when the Door is clicked/opened for Puzzle 2.
    /// </summary>
    public void OnDoorOpened()
    {
        if (scoreManager == null) return;

        scoreManager.SolvePuzzle2AndOpenDoor();
        Debug.Log("[Timer Trigger] Puzzle 2 Timer Stopped & Score Panel Triggered (Door Opened).");
    }

    private bool ContainsID(List<string> list, string targetID)
    {
        foreach (string id in list)
        {
            if (!string.IsNullOrEmpty(id) && id.Trim().Equals(targetID, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}