using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using ExitGames.Client.Photon;

public class NetworkedDoor : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    [Header("Interaction Rules")]
    [Tooltip("Check this ONLY in the scene where clicking the door is allowed. Uncheck in the other scene.")]
    public bool canBeOpenedFromThisScene = true;

    [Tooltip("How close the player must be to open the door.")]
    public float interactionDistance = 2.5f;

    [Header("Photon Sync Key")]
    [Tooltip("Unique property key for this door in Photon room properties.")]
    public string doorPropertyKey = "ClockmakerDoorOpened";

    public bool isOpen = false;
    private Transform playerTransform;

    private void Start()
    {
        // Find local player in scene
        PlayerMovement2D player = FindObjectOfType<PlayerMovement2D>();
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Check if door was already opened before entering this scene
        SyncDoorStateFromRoom();
    }

    // Automatically called when any Photon Room Property updates
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(doorPropertyKey))
        {
            SyncDoorStateFromRoom();
        }
    }

    private void SyncDoorStateFromRoom()
    {
        if (PhotonNetwork.CurrentRoom != null && 
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(doorPropertyKey, out object state))
        {
            if ((bool)state && !isOpen)
            {
                DisappearDoor();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. Block interaction if this scene isn't allowed to open it
        if (!canBeOpenedFromThisScene)
        {
            Debug.Log("[Door] This door cannot be opened from this scene!");
            return;
        }

        if (isOpen) return;

        // 2. Distance check
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(playerTransform.position, transform.position);
            if (distance > interactionDistance)
            {
                Debug.Log($"[Door] Player is too far away ({distance:F1} units).");
                return;
            }
        }

        // 3. Update Photon Room Property across both scenes
        Hashtable props = new Hashtable
        {
            { doorPropertyKey, true }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private void DisappearDoor()
    {
        isOpen = true;
        Debug.Log($"[Door] {gameObject.name} disappeared!");

        // Disables the sprite and collider so it vanishes and can't be clicked/blocked anymore
        gameObject.SetActive(false);
    }
}
