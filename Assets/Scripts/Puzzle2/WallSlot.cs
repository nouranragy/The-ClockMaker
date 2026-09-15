using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class WallSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Slot Settings")]
    [Tooltip("Type the exact itemID string expected for this slot (e.g. 'gear1')")]
    public string requiredItemID = "gear1";
    [Header("Visual Display")]
    public SpriteRenderer displaySpriteRenderer;
    [Header("State")]
    public bool isItemPlaced = false;
    private PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        TryPlaceItem();
    }
    private void TryPlaceItem()
    {
        if (isItemPlaced) return;
        string selectedID = InventoryManager.Instance.selectedItemID;

        if (!string.IsNullOrEmpty(selectedID) && selectedID.Trim() == requiredItemID.Trim())  photonView.RPC(nameof(RPC_PlaceItem), RpcTarget.All, selectedID);
        else if (!string.IsNullOrEmpty(selectedID))  Debug.Log($"Wrong item selected! Expected: '{requiredItemID}', Selected: '{selectedID}'");
    }
    [PunRPC]
    private void RPC_PlaceItem(string itemID)
    {
        isItemPlaced = true;
        if (displaySpriteRenderer != null)  displaySpriteRenderer.gameObject.SetActive(true);

        if (InventoryManager.Instance != null && InventoryManager.Instance.selectedItemID == itemID)  InventoryManager.Instance.ClearCurrentSlot();
        Debug.Log($"Successfully placed '{itemID}' on the wall slot!");
        WallPuzzleManager.Instance?.CheckPuzzleCompletion();
    }
}