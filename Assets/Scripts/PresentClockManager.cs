using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PresentClockManager : MonoBehaviourPunCallbacks
{
    [Header("Spawn Settings")]
    [Tooltip("The character/man GameObject or Prefab that comes out of the clock.")]
    public GameObject clockmakerCharacter;
    [Tooltip("The exact position where the character emerges (e.g. Clock Door Transform).")]
    public Transform clockSpawnPoint;
    [Header("Optional Animation / Visuals")]
    [Tooltip("Optional Animator component on the Clock to play an open/emerge animation.")]
    public Animator clockAnimator;
    private const string PUZZLE_KEY = "WallPuzzleSolved";
    private void Start()
    {
        CheckPuzzleStatus();
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PUZZLE_KEY))  CheckPuzzleStatus();
    }
    private void CheckPuzzleStatus()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PUZZLE_KEY, out object isSolved))
        {
            if ((bool)isSolved)   SpawnClockmaker();
        }
    }
    private void SpawnClockmaker()
    {
        if (clockmakerCharacter == null)
        {
            Debug.LogError("[PresentClockManager] Clockmaker character GameObject is not assigned!");
            return;
        }
        if (clockmakerCharacter.activeSelf) return;
        Debug.Log("Past puzzle solved! Revealing Clockmaker in Present scene...");
        if (clockSpawnPoint != null)  clockmakerCharacter.transform.position = clockSpawnPoint.position;
        if (clockAnimator != null)   clockAnimator.SetTrigger("OpenClock");
        clockmakerCharacter.SetActive(true);
    }
}
