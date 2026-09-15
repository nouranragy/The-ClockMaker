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
    public float interactionDistance = 5f;
    [Header("Photon Sync Key")]
    [Tooltip("Unique property key for this door in Photon room properties.")]
    public string doorPropertyKey = "ClockmakerDoorOpened";
    public bool isOpen = false;
    private Transform playerTransform;
    private void Start()
    {
        PlayerMovement2D player = FindFirstObjectByType<PlayerMovement2D>();
        if (player != null) playerTransform = player.transform;
        SyncDoorStateFromRoom();
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(doorPropertyKey)) SyncDoorStateFromRoom();
    }

    private void SyncDoorStateFromRoom()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(doorPropertyKey, out object state))
        {
            if ((bool)state && !isOpen) DisappearDoor();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canBeOpenedFromThisScene)
        {
            Debug.Log("[Door] This door cannot be opened from this scene!");
            return;
        }

        if (isOpen) return;
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(playerTransform.position, transform.position);
            if (distance > interactionDistance)
            {
                Debug.Log($"[Door] Player is too far away ({distance:F1} units).");
                return;
            }
        }
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
        gameObject.SetActive(false);
    }
}
