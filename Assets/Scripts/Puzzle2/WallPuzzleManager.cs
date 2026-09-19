using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class WallPuzzleManager : MonoBehaviourPunCallbacks
{
    public static WallPuzzleManager Instance;

    public GameObject wallPuzzleParentContainer;
    public WallSlot wallslot1;
    public WallSlot wallslot2;
    public GameObject[] gearObjects;
    public GameObject completedClockObject;
    private const string PUZZLE_KEY = "WallPuzzleSolved";

    private bool hasPlayedSolvedSound = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
     public override void OnEnable()
    {
        base.OnEnable();
        CheckChestUnlockedStatus();
        CheckExistingPuzzleState();
    }
    public override void OnDisable ()
    {
        base.OnDisable();
    }

    private void Start()
    {
        if(wallPuzzleParentContainer != null)
        {
            wallPuzzleParentContainer.SetActive(false);
        }
        CheckChestUnlockedStatus();
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
        Debug.Log($"<color=yellow>[WallPuzzleManager] Room Properties Changed! Checking keys...</color>");
        if (propertiesThatChanged.ContainsKey(MainChest.CHEST_UNLOCKED_KEY))
        {
            Debug.Log("<color=green>[WallPuzzleManager] CHEST_UNLOCKED_KEY received from Network!</color>");
            CheckChestUnlockedStatus();
        }
        if (propertiesThatChanged.ContainsKey(PUZZLE_KEY))
        {
            CheckExistingPuzzleState();
        }
    }

    private void CheckChestUnlockedStatus()
    {
        if(PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom != null ){
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MainChest.CHEST_UNLOCKED_KEY, out object isUnlocked))
        {
            Debug.Log($"[WallPuzzleManager] Value of Chest Unlocked is: {isUnlocked}");
            if((bool)isUnlocked && wallPuzzleParentContainer != null)
            {
                wallPuzzleParentContainer.SetActive(true);
                Debug.Log($"[WallPuzzleManager] Value of Chest Unlocked is: {isUnlocked}");
            }  
        
           
        }
         else
        {
            Debug.LogWarning("[WallPuzzleManager] Key 'MainChestUnlocked' was not found in Room Properties!");
        }  
    }
        
        else
    {
        Debug.LogError("[WallPuzzleManager] PhotonNetwork.CurrentRoom is NULL! Not connected to room?");
    }
    }
    private void CheckExistingPuzzleState()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PUZZLE_KEY, out object isSolved))
        {
            if ((bool)isSolved)
            {
               ApplyPuzzleSolvedVisuals();

               NetworkedDoor door = FindFirstObjectByType<NetworkedDoor>();
                if (door != null)
                {
                    door.SetPuzzleSolved();
                }

                if (!hasPlayedSolvedSound)
                {
                    hasPlayedSolvedSound = true;
                if(SoundManager.Instance != null)
                {
                 SoundManager.Instance.PlaySFX(SoundManager.Instance.grandfatherClockSound);
                }
                }
            } 
             
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