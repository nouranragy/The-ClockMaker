using UnityEngine;

public class DoorController : MonoBehaviour
{
    public ScoreManager scoreManager;

    public void OpenDoors()
    {

        if (scoreManager != null) scoreManager.SolvePuzzle2AndOpenDoors();
    }
}