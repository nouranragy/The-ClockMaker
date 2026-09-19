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
    [SerializeField] private float openYRotation = -55f;
    [SerializeField] private float openDuration = 0.6f;
    [SerializeField] private Ease openEase = Ease.OutQuad;
    [SerializeField] private Collider2D doorCollider;

    private Transform playerTransform;
    private Tween openTween;

    public bool isPuzzleSolved = false;

    private void Start()
    {
        PlayerMovement2D player = FindFirstObjectByType<PlayerMovement2D>();
        if (player != null) playerTransform = player.transform;
        if (doorCollider == null) doorCollider = GetComponent<Collider2D>();

        SyncDoorStateFromRoom();
    }

    private void OnDestroy()
    {
        openTween?.Kill();
    }

    public void SetPuzzleSolved()
    {
        isPuzzleSolved = true;
        Debug.Log("[NetworkedDoor] Puzzle condition met! Door is now unlockable by click.");
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(doorPropertyKey)) SyncDoorStateFromRoom();
    }

    private void SyncDoorStateFromRoom()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(doorPropertyKey, out object state))
        {
            if ((bool)state && !isOpen)
            {
                OpenDoor();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool isPuzzleSolvedInRoom = false;

        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("WallPuzzleSolved", out object solved))
        {
            isPuzzleSolvedInRoom = (bool)solved;
        }

        if (!isPuzzleSolved && !isPuzzleSolvedInRoom)
        {
            Debug.Log("[Door] Cannot open yet! Solve the gear puzzle first.");
            return;
        }

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

        // 1. إيقاف التايمر باسم الدالة الصحيح
        if (PuzzleTimerTrigger.Instance != null)
        {
            PuzzleTimerTrigger.Instance.StopTimer();
        }

        // 2. إرسال إنهاء اللغز لـ ScoreManager من الـ MasterClient أو محلياً للسنقل
       ScoreManager score = ScoreManager.Instance ?? Object.FindFirstObjectByType<ScoreManager>(FindObjectsInactive.Include);
    if (score != null)
    {
        // score.SolvePuzzle1();
        score.SolvePuzzle2AndOpenDoor();
    }
    else
    {
        Debug.LogError("[Door] Could not find ScoreManager in Scene!");
    }

        // 3. أنيميشن الباب
        if (doorCollider != null) doorCollider.enabled = false;
        openTween?.Kill();
        openTween = transform.DOLocalRotate(new Vector3(0f, openYRotation, 0f), openDuration).SetEase(openEase);
    }
    public void CompletePuzzle()
    {
        ScoreManager sm = ScoreManager.Instance;
        if (sm == null)
        {
            sm = Object.FindFirstObjectByType<ScoreManager>();
        }

        if (sm != null)
        {
            sm.SolvePuzzle2AndOpenDoor();
        }
        else
        {
            Debug.LogError("[Door] ScoreManager is still missing in the scene!");
        }
    }
}