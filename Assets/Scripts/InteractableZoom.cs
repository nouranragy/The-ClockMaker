using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InteractableZoom : MonoBehaviour , IPointerClickHandler
{
    [Header("Zoom View Panel")]
    public GameObject closeUpPanel;

    [Header ("Paper Mechanics")]
    public TMP_InputField paperinputField;

   public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
        OpenZoomView();
    }
    public void OpenZoomView()
    {
        if(closeUpPanel != null)
        {
            closeUpPanel.SetActive(true);

            if(paperinputField != null)
            {
                bool isPenSelected = (InventoryManager.Instance != null && InventoryManager.Instance.selectedItemID == "Pen");
                paperinputField.interactable = isPenSelected;
            }
        }
    }
     public void CloseZoomView()
    {
        if(closeUpPanel != null)
        {
            closeUpPanel.SetActive(false);
        }
    }
}
