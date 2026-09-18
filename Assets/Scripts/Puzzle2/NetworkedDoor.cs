using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using ExitGames.Client.Photon;
using DG.Tweening;

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

    [Header("Open Animation")]
    [Tooltip("Y-axis rotation (degrees) the door swings to when opened.")]
    [SerializeField] private float openYRotation = -55f;
    [SerializeField] private float openDuration = 0.6f;
    [SerializeField] private Ease openEase = Ease.OutQuad;
    [Tooltip("Optional. Disabled once the door opens so the player can walk through it.")]
    [SerializeField] private Collider2D doorCollider;

    private Transform playerTransform;
    private Tween openTween;

    private void Start()
    {
        PlayerMovement2D player = FindFirstObjectByType<PlayerMovement2D>();
        if (player != null) playerTransform = player.transform;
        if (doorCollider == null) doorCollider = GetComponent<Collider2D>();
        SyncDoorStateFromRoom(notifyTimer: false);
    }

    private void OnDestroy()
    {
        openTween?.Kill();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(doorPropertyKey)) SyncDoorStateFromRoom(notifyTimer: true);
    }

    private void SyncDoorStateFromRoom(bool notifyTimer)
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(doorPropertyKey, out object state))
        {
            if ((bool)state && !isOpen)
            {
                OpenDoor();
                if (notifyTimer && PuzzleTimerTrigger.Instance != null) PuzzleTimerTrigger.Instance.OnDoorOpened();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canBeOpenedFromThisScene || isOpen) return;

        if (playerTransform != null)
        {
            float distance = Vector2.Distance(playerTransform.position, transform.position);
            if (distance > interactionDistance) return;
        }

        Hashtable props = new Hashtable { { doorPropertyKey, true } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log($"[Door] {gameObject.name} opened!");
        if (doorCollider != null) doorCollider.enabled = false;
        openTween?.Kill();
        openTween = transform.DOLocalRotate(new Vector3(0f, openYRotation, 0f), openDuration).SetEase(openEase);
    }
}