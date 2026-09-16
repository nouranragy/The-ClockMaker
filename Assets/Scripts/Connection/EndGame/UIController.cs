using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UIController : MonoBehaviourPunCallbacks
{
    public GameObject endPanelContainer;
    public Button exitButton;

    [Header("Top Panel")]
    public TMP_Text averageScoreLabel;
    public TMP_Text playerNamesLabel;
    public GameObject newBestIndicator;
    public GameObject[] averageStars;

    [Header("Puzzle 1 Details")]
    public TMP_Text puzzle1TargetLabel;
    public TMP_Text puzzle1CompletedLabel;
    public GameObject[] puzzle1Stars;

    [Header("Puzzle 2 Details")]
    public TMP_Text puzzle2TargetLabel;
    public TMP_Text puzzle2CompletedLabel;
    public GameObject[] puzzle2Stars;

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

    public void DisplayPanel(LevelResult currentSession, LevelResult globalBest, bool isNewBest)
    {
        endPanelContainer.SetActive(true);

        if (playerNamesLabel != null) playerNamesLabel.text = currentSession.playerNames;
        if (averageScoreLabel != null) averageScoreLabel.text = $"AVERAGE SCORE ({currentSession.AverageTime:F1}s)";
        if (newBestIndicator != null) newBestIndicator.SetActive(isNewBest);

        SetStars(averageStars, currentSession.AverageStars);

        if (puzzle1CompletedLabel != null) puzzle1CompletedLabel.text = $"COMPLETED IN {currentSession.puzzle1CompletedTime:F1}s";
        SetStars(puzzle1Stars, currentSession.Puzzle1Stars);

        if (puzzle2CompletedLabel != null) puzzle2CompletedLabel.text = $"COMPLETED IN {currentSession.puzzle2CompletedTime:F1}s";
        SetStars(puzzle2Stars, currentSession.Puzzle2Stars);

        if (globalBest != null)
        {
            if (highPlayerNamesLabel != null) highPlayerNamesLabel.text = globalBest.playerNames;
            if (highAverageScoreLabel != null) highAverageScoreLabel.text = $"({globalBest.AverageTime:F1}s)";
        }
    }

    private void SetStars(GameObject[] starArray, int starsToDisplay)
    {
        if (starArray == null) return;

        for (int i = 0; i < starArray.Length; i++)
        {
            if (starArray[i] == null) continue;

            Image starImage = starArray[i].GetComponent<Image>();
            if (starImage != null && goldStarSprite != null && emptyStarSprite != null)
            {
                starArray[i].SetActive(true);
                starImage.sprite = (i < starsToDisplay) ? goldStarSprite : emptyStarSprite;
            }
            else
            {
                starArray[i].SetActive(i < starsToDisplay);
            }
        }
    }

    public void OnExitButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("exit game");
            }
            else
            {
                PhotonNetwork.LeaveRoom();
            }
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