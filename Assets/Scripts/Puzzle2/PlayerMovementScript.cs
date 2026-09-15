using UnityEngine;
using UnityEngine.InputSystem; // Added New Input System namespace

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.1f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        // 1. Check for left mouse click using the New Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Ignore click if clicking over UI
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            targetPosition = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            isMoving = true;
        }

        // 2. Smoothly move toward target position
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) <= stoppingDistance)
            {
                isMoving = false;
            }
        }
    }

    public void MoveToPosition(Vector3 destination)
    {
        targetPosition = new Vector3(destination.x, destination.y, transform.position.z);
        isMoving = true;
        
    }
}