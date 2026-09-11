using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class WallPuzzleManager : MonoBehaviourPunCallbacks
{
    public static WallPuzzleManager Instance;
    public WallSlot wallslot1;
    public WallSlot wallslot2;
    public GameObject[] gearObjects;
    public GameObject completedClockObject;
    private const string PUZZLE_KEY = "WallPuzzleSolved";
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        CheckExistingPuzzleState();
    }
    public void CheckPuzzleCompletion()
    {
        if (wallslot1 != null && wallslot2 != null && wallslot1.isItemPlaced && wallslot2.isItemPlaced)
        {
            Hashtable props = new Hashtable { { PUZZLE_KEY, true } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(PUZZLE_KEY))
        {
            CheckExistingPuzzleState();
        }
    }
    private void CheckExistingPuzzleState()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PUZZLE_KEY, out object isSolved))
        {
            if ((bool)isSolved) ApplyPuzzleSolvedVisuals();
        }
    }
    private void ApplyPuzzleSolvedVisuals()
    {
        if (wallslot1 != null) wallslot1.gameObject.SetActive(false);
        if (wallslot2 != null) wallslot2.gameObject.SetActive(false);
        if (gearObjects != null)
        {
            foreach (GameObject gear in gearObjects)
                if (gear != null) gear.SetActive(false);
        }
        if (completedClockObject != null) completedClockObject.SetActive(true);
    }
}