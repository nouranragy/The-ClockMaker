using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
   public Image iconImage;
   public Image highlightOutLine;
   public bool isFull = false;
   [HideInInspector] public string itemID;

   public void AddItem(Sprite itemSprite, string id)
    {
        iconImage.sprite = itemSprite;
        iconImage.gameObject.SetActive(true);
        itemID = id;
        isFull = true;
    }

    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.gameObject.SetActive(false);
        itemID = "";
        isFull = false;
    }

    public void OnSlotClicked()
    {
        if(!isFull) return;
        InventoryManager.Instance.SelectItem(itemID , this);
    }

    public void SetHighlight(bool active)
    {
        if(highlightOutLine != null)
        {
            highlightOutLine.gameObject.SetActive(active);
        }
    }
}
