using UnityEngine;
using UnityEngine.EventSystems;


public class InteractableZoom : MonoBehaviour , IPointerClickHandler
{
    [Header("Zoom View Panel")]
    public GameObject closeUpPanel;

   public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clock");
        OpenZoomView();
    }
    public void OpenZoomView()
    {
        if(closeUpPanel != null)
        {
            closeUpPanel.SetActive(true);
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
