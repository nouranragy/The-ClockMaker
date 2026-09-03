using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
   public static InventoryManager Instance;
   
   [Header("Slots Setup")]
   public InventorySlot[] slots;

   [Header ("Current Selection")]
   public string selectedItemID ="";
   private InventorySlot currentSelectedSlot;

   private void Awake()
    {
        if(Instance == null ) Instance = this;
        else Destroy(gameObject);
    }

    public bool AddToInventory(Sprite itemSprite, string itemID)
    {
        foreach(InventorySlot slot in slots)
        {
            if (!slot.isFull)
            {
                slot.AddItem(itemSprite , itemID);
                return true;
            }
        }
        Debug.Log("Inventory is Full");
        return false;
    }

    public void SelectItem(string itemID , InventorySlot slot)
    {

        Debug.Log($"[Click Debug] Clicked Item: '{itemID}' | Currently Selected: '{selectedItemID}'");
        string cleanItemID = itemID.Trim();
    string cleanSelectedID = selectedItemID.Trim();
        if(!string.IsNullOrEmpty(cleanSelectedID) && cleanSelectedID == cleanItemID)
        {
            DeselctAll();
           Debug.Log("[Click Debug] SUCCESS: Deselected!");
            return;
        }
         DeselctAll();

         selectedItemID = itemID;
         currentSelectedSlot = slot;
         if(currentSelectedSlot != null)
        {
            currentSelectedSlot.SetHighlight(true);
        }
Debug.Log("Selected Item in Inventory: " + selectedItemID);
Debug.Log("[Click Debug] Selected: " + selectedItemID);
     
}
public void DeselctAll()
    {
        selectedItemID = "";
        foreach(var slot in slots)
        {
            slot.SetHighlight(false);
        }
    }
}
