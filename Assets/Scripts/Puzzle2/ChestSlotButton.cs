using UnityEngine;
using UnityEngine.UI;

public class ChestSlotButton : MonoBehaviour
{
   [Header ("Slot Requirements")]
   public string requiredChestId;
   public Image slotDisplayImage;
   public Transform spawnPoint;

   [Header ("Main Chest reference")]
   public MainChest mainChest ;

   private string currentPlacedChestID = "";

   void Start()
    {
        
        if (slotDisplayImage != null)
        {
            slotDisplayImage.gameObject.SetActive(false);
        }
    }
  private GameObject currentSpawnedChest;

  public void OnSlotClicked()
    {
        if(!string.IsNullOrEmpty(currentPlacedChestID)) return;

        string selectedID = InventoryManager.Instance.selectedItemID;

        if (!string.IsNullOrEmpty(selectedID))
        {
            
            currentPlacedChestID = selectedID;
            if(slotDisplayImage != null)
            {
                Sprite chestSprite = InventoryManager.Instance.GetSelectedSprite();
                if(chestSprite != null)
                {
                    slotDisplayImage.sprite = chestSprite;
                    slotDisplayImage.gameObject.SetActive(true);
                }

            }
            // InventoryManager.Instance.ClearSelectedItem();
            if (mainChest != null)
            {
                mainChest.CheckPuzzleCompletion();
            }
        }
    }

    public bool IsCorrectlyPlaced()
    {
        return currentPlacedChestID.Trim() == requiredChestId.Trim();
    }
}
