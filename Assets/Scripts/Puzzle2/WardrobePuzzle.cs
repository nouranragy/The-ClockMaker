using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class WardrobePuzzle : MonoBehaviour , IPointerClickHandler
{
    [Header ("UI Elements")]
    public TMP_InputField passwordInput;
    public GameObject openWardrobeUI;
    public GameObject closedWardrobeObject;

    [Header ("Password Settings")]
    public string correctPassword = "1234";

    private bool isUnlocked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(passwordInput != null)
        {
            passwordInput.onValueChanged.AddListener(OnPasswordTyped);
            passwordInput.gameObject.SetActive(false);
        }
    }

     public void OnPointerClick(PointerEventData eventData)
    {
        if (isUnlocked)
        {
            if(openWardrobeUI != null) openWardrobeUI.SetActive(true);
        }
        else
        {
            if(passwordInput != null)
            {
                passwordInput.gameObject.SetActive(true);
                passwordInput.text = "";
                passwordInput.ActivateInputField();
            }
        }
    }
    private void OnPasswordTyped(string typedText)
    {
        if(isUnlocked) return;

        if(typedText == correctPassword)
        {
            Debug.Log("Password Correct! ");
            isUnlocked = true;

            passwordInput.gameObject.SetActive(false);
            if(openWardrobeUI != null) openWardrobeUI.SetActive(true);
            if (closedWardrobeObject != null) closedWardrobeObject.SetActive(false);
        }
    }

    public void ClosePanel()
    {
        if(passwordInput != null) passwordInput.gameObject.SetActive(false);
        if(openWardrobeUI != null)openWardrobeUI.SetActive(false);
    }
}
