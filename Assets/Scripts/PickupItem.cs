using UnityEngine;
using UnityEngine.EventSystems;


public class PickupItem : MonoBehaviour
{
   [Header("Item Info")] 
   public string itemID = "Pen";
   public Sprite itemSprite;

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
