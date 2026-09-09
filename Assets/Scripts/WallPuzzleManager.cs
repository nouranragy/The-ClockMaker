using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class WallPuzzleManager : MonoBehaviour
{
    public static WallPuzzleManager Instance;
    [Header("Wall Slots")]
    public WallSlot wallslot1;
    public WallSlot wallslot2;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void CheckPuzzleCompletion()
    {
        if (wallslot1 != null && wallslot2 != null && wallslot1.isItemPlaced && wallslot2.isItemPlaced)
        {
            Debug.Log("PAST PUZZLE SOLVED! Setting Photon Room Property...");
            Hashtable props = new Hashtable
            {
                { "WallPuzzleSolved", true }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }
}