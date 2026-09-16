using UnityEngine;
using TMPro;
using Photon.Pun;

public class InGameTimerUI : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text timerText;
    public string timerPrefix = "Time: ";

    [Header("Dependencies")]
    public ScoreManager scoreManager;

    private void Start()
    {
        if (scoreManager == null)
        {
            scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        }

        if (timerText == null)
        {
            timerText = GetComponent<TMP_Text>();
        }
    }

    private void Update()
    {
        if (scoreManager == null || timerText == null) return;

        float currentDisplayTime = scoreManager.GetCurrentActiveTime();

        // Displays time formatted as 0.0s (e.g., "Time: 12.4s")
        timerText.text = $"{timerPrefix}{currentDisplayTime:F1}s";
    }
}
