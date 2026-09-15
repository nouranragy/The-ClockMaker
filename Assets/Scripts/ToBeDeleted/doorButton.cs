using UnityEngine;

public class DoorInteraction2D : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject keypadCanvas;
    
    private void Start()
    {
        if (keypadCanvas != null) keypadCanvas.SetActive(false);
    }
    public void OpenKeypadUI()
    {
        if (keypadCanvas != null) keypadCanvas.SetActive(true);
    }
    public void CloseKeypadUI()
    {
        if (keypadCanvas != null) 
            keypadCanvas.SetActive(false);
    }
    private void OnMouseDown()
    {
        Debug.Log("Door clicked!"); //+
        OpenKeypadUI();
    }
}
