using UnityEngine;
using UnityEngine.UI;

public class CandleSlotInteractable : MonoBehaviour
{
   [Header("Slots Setup")]
   public int slotIndex;
   public CandleCandelabraPuzzle puzzleManager;

   [Header("Candle Prefab to Spawn")]
   public GameObject PinkCandlePrefab;
   public GameObject blueCandlePrefab;
   public GameObject greenCandlePrefab;
   public GameObject YellowCandlePrefab;
   public GameObject BrowenCandlePrefab;
   public GameObject BabyblueCandlePrefab;



  public void OnSlotClicked()
    {
        if(InventoryManager.Instance == null || string.IsNullOrEmpty(InventoryManager.Instance.selectedItemID))
        {
            Debug.Log("No candle selected in Inventory!");
            return;
        }

        string selectedItem = InventoryManager.Instance.selectedItemID.Trim();
        if (selectedItem.EndsWith("Candle"))
        {
            GameObject candlePrefab = GetCandlePrefab(selectedItem);
            if(candlePrefab != null)
            {
                GameObject spawnedCandle = Instantiate(candlePrefab , transform);
                spawnedCandle.transform.localPosition = Vector3.zero;
                spawnedCandle.transform.localRotation = Quaternion.identity;

                CandleItem candleItem = spawnedCandle.GetComponent<CandleItem>();

                if(puzzleManager != null && candleItem != null)
                {
                    puzzleManager.PlaceCandleInSlot(slotIndex, candleItem);
                }

                InventoryManager .Instance.DeselctAll();

                Button slotButton = GetComponent<Button>();
                if (slotButton != null) slotButton.interactable = false;
            

            }
        }

    } 
    private GameObject GetCandlePrefab(string itemName)
    {
        if (itemName == "PinkCandle") return PinkCandlePrefab;
        if (itemName == "BlueCandle") return blueCandlePrefab;
        if (itemName == "GreenCandle") return greenCandlePrefab;
        if (itemName == "BabyBlueCandle") return BabyblueCandlePrefab;
        if (itemName == "YellowCandle") return YellowCandlePrefab;
        if (itemName == "BrowenCandle") return BrowenCandlePrefab;

        return null;
    }
}

