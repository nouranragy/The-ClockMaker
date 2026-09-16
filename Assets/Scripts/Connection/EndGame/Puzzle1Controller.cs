using UnityEngine;

public class Puzzle1Controller : MonoBehaviour
{
    public ScoreManager scoreManager;

    public void CompletePuzzle1()
    {
        if (scoreManager != null)
        {
            scoreManager.SolvePuzzle1();
        }
    }
}
