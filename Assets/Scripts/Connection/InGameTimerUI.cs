using UnityEngine;
using TMPro;

public class InGameTimerUI : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text timerText;
    public string timerPrefix = "";

    private void Start()
    {
        if (timerText == null) timerText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (timerText == null || PuzzleTimerTrigger.Instance == null) return;

        // Reads the actual countdown time from PuzzleTimerTrigger
        float currentDisplayTime = PuzzleTimerTrigger.Instance.timeRemaining;

        // Formats as Minutes:Seconds (e.g., 04:59s)
        int minutes = Mathf.FloorToInt(currentDisplayTime / 60f);
        int seconds = Mathf.FloorToInt(currentDisplayTime % 60f);

        timerText.text = $"{timerPrefix}{minutes:00}:{seconds:00}s";
    }
}
