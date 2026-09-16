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
    [Header("Puzzle Item IDs (Inspector Configuration)")]
    public string puzzle1TriggerItemID = "first candel"; //added
    public string puzzle2TriggerItemID = "first box"; //added

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
                CheckAndStartPuzzleTimer(itemID); // added 
                return true;
            }
        }
        Debug.Log("Inventory is Full");
        return false;
    }
    private void CheckAndStartPuzzleTimer(string itemID) //added
    {
        ScoreManager scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        if (scoreManager == null) return;

        string cleanID = itemID.Trim();
        if (!string.IsNullOrEmpty(puzzle1TriggerItemID) && cleanID.Equals(puzzle1TriggerItemID.Trim(), System.StringComparison.OrdinalIgnoreCase))
        {
            scoreManager.StartPuzzle1();
            Debug.Log($"[Score System] Puzzle 1 Timer Started via Item: {itemID}");
        }
        else if (!string.IsNullOrEmpty(puzzle2TriggerItemID) && cleanID.Equals(puzzle2TriggerItemID.Trim(), System.StringComparison.OrdinalIgnoreCase))
        {
            scoreManager.StartPuzzle2();
            Debug.Log($"[Score System] Puzzle 2 Timer Started via Item: {itemID}");
        }
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
    public void ClearCurrentSlot()
    {
        if (currentSelectedSlot != null)
        {
            currentSelectedSlot.ClearSlot();
        }
        DeselctAll();
    }

    public Sprite GetSelectedSprite()
    {
        if (currentSelectedSlot != null)
        {
            return currentSelectedSlot.iconImage.sprite;
        }
        return null;
    }

}
