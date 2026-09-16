using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIController : MonoBehaviourPunCallbacks
{
    public GameObject endPanelContainer;
    public Button exitButton;

    [Header("Top Panel (Average Score)")]
    public TMP_Text averageScoreLabel;
    public TMP_Text playerNamesLabel;
    public GameObject newBestIndicator;
    public Image[] averageStars;

    [Header("Puzzle 1 Details")]
    public TMP_Text puzzle1TargetLabel;
    public TMP_Text puzzle1CompletedLabel;
    public Image[] puzzle1Stars;

    [Header("Puzzle 2 Details")]
    public TMP_Text puzzle2TargetLabel;
    public TMP_Text puzzle2CompletedLabel;
    public Image[] puzzle2Stars;

    [Header("High Score Panel")]
    public TMP_Text highPlayerNamesLabel;
    public TMP_Text highAverageScoreLabel;

    [Header("Star Sprites")]
    public Sprite goldStarSprite;
    public Sprite emptyStarSprite;

    private void Start()
    {
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
    }

    public void DisplayPanel(LevelResult current, LevelResult best, bool isNewBest)
    {
        if (endPanelContainer != null) endPanelContainer.SetActive(true);

        if (playerNamesLabel != null) playerNamesLabel.text = current.playerNames;
        if (averageScoreLabel != null) averageScoreLabel.text = $"({current.AverageTime:F1}s)";
        if (newBestIndicator != null) newBestIndicator.SetActive(isNewBest);

        SetStars(averageStars, current.AverageStars);

        // Puzzle 1 UI
        if (puzzle1TargetLabel != null) puzzle1TargetLabel.text = $"({current.puzzle1TargetTime:F1}s)";
        if (puzzle1CompletedLabel != null) puzzle1CompletedLabel.text = $"({current.puzzle1CompletedTime:F1}s)";
        SetStars(puzzle1Stars, current.Puzzle1Stars);

        // Puzzle 2 UI
        if (puzzle2TargetLabel != null) puzzle2TargetLabel.text = $"({current.puzzle2TargetTime:F1}s)";
        if (puzzle2CompletedLabel != null) puzzle2CompletedLabel.text = $"({current.puzzle2CompletedTime:F1}s)";
        SetStars(puzzle2Stars, current.Puzzle2Stars);

        // Highest Score UI
        if (best != null)
        {
            if (highPlayerNamesLabel != null) highPlayerNamesLabel.text = best.playerNames;
            if (highAverageScoreLabel != null) highAverageScoreLabel.text = $"({best.AverageTime:F1}s)";
        }
    }

    private void SetStars(Image[] starImages, int starCount)
    {
        if (starImages == null) return;

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue;

            if (goldStarSprite != null && emptyStarSprite != null)
            {
                starImages[i].sprite = (i < starCount) ? goldStarSprite : emptyStarSprite;
            }
            else
            {
                starImages[i].gameObject.SetActive(i < starCount);
            }
        }
    }

    public void OnExitButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("exit game");
        }
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("exit game");
    }

    public void HidePanel()
    {
        if (endPanelContainer != null) endPanelContainer.SetActive(false);
    }
}