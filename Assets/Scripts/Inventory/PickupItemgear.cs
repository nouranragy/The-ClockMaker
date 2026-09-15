using UnityEngine;
using UnityEngine.EventSystems;

// We added IPointerClickHandler here!
public class PickupItemgear : MonoBehaviour, IPointerClickHandler 
{
    [Header("Item Info")] 
    public string itemID = "Pen";
    public Sprite itemSprite;

    // This replaces OnMouseDown()
    public void OnPointerClick(PointerEventData eventData)
    {
        CollectItem();
    }

    public void CollectItem()
    {
        Debug.Log("CollectItem executed for " + itemID);
        if(itemSprite == null)
        {
            Debug.LogError("Item Sprite is missing on  " + gameObject.name);
            return;
        }

        bool added = InventoryManager.Instance.AddToInventory(itemSprite , itemID);
        if (added)
        {
            gameObject.SetActive(false);
        }
    }
}
