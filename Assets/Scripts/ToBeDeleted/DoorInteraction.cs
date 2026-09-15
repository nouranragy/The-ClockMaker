using UnityEngine;
using UnityEngine.EventSystems;

public class DoorInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("Door Settings")]
    [Tooltip("How close the player must be to open the door.")]
    public float interactionDistance = 2.0f;
    [Header("Visuals / Animation")]
    public SpriteRenderer doorSpriteRenderer;
    public Sprite openDoorSprite;
    public Animator doorAnimator;
    [Header("State")]
    public bool isOpen = false;
    private Transform playerTransform;
    private void Start()
    {
        PlayerMovement2D player = Object.FindFirstObjectByType<PlayerMovement2D>();
        if (player != null)   playerTransform = player.transform;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        TryOpenDoor();
    }
    private void TryOpenDoor()
    {
        if (isOpen)
        {
            Debug.Log("[Door] Door is already open!");
            return;
        }
        if (playerTransform == null)
        {
            Debug.LogError("[Door] Player not found in scene!");
            return;
        }
        float distance = Vector2.Distance(playerTransform.position, transform.position);
        if (distance <= interactionDistance)   OpenDoor();
        else
        {
            Debug.Log($"[Door] Player is too far away ({distance:F1} units). Move closer!");
            PlayerMovement2D movement = playerTransform.GetComponent<PlayerMovement2D>();
            if (movement != null)  movement.MoveToPosition(transform.position);
        }
    }
    public void OpenDoor()
    {
        isOpen = true;
        Debug.Log("[Door] Door Opened!");
        if (doorAnimator != null)  doorAnimator.SetTrigger("Open");
        else if (doorSpriteRenderer != null && openDoorSprite != null)  doorSpriteRenderer.sprite = openDoorSprite;
    }
}