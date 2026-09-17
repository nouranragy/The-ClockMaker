using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using DG.Tweening;


public class WardrobePuzzle : MonoBehaviour , IPointerClickHandler
{
    [Header ("UI Elements")]
    public TMP_InputField passwordInput;
    public GameObject openWardrobeUI;
    public GameObject closedWardrobeObject;

    [Header ("Password Settings")]
    public string correctPassword = "1234";

    [Header ("puzzle item (inside Wardrobe)")]
    public Transform paperObject;
    public Transform gearObject;

    private bool isUnlocked;

    private Vector3 paperOriginalScale;
    private Vector3 paperOriginalPos;
    private Vector3 gearOriginalScale;
    private Vector3 gearOriginalPos;
    private SpriteRenderer openWardrobeSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(passwordInput != null)
        {
            passwordInput.onValueChanged.AddListener(OnPasswordTyped);
            passwordInput.gameObject.SetActive(false);
        }

        if (openWardrobeUI != null)
        {
            openWardrobeSprite = openWardrobeUI.GetComponent<SpriteRenderer>();
        }

        if (paperObject != null)
        {
            paperOriginalScale = paperObject.localScale;
            paperOriginalPos = paperObject.position;
            paperObject.gameObject.SetActive(false); 
        }

        if (gearObject != null)
        {
            gearOriginalScale = gearObject.localScale;
            gearOriginalPos = gearObject.position;
            gearObject.gameObject.SetActive(false);
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
            // if(openWardrobeUI != null) openWardrobeUI.SetActive(true);
            if (closedWardrobeObject != null) closedWardrobeObject.SetActive(false);
           
            openWardrobeWithTween();
        }
    }

    private void openWardrobeWithTween()
    {
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.StopSFX();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.chestOpenSound);
        }
         if(openWardrobeUI != null){
             openWardrobeUI.SetActive(true);
            if(openWardrobeSprite != null)
            {
              Color color = openWardrobeSprite.color;
                color.a = 0f;
                openWardrobeSprite.color = color;

                
                openWardrobeSprite.DOFade(1f, 0.3f);
            }
         openWardrobeUI.transform.localScale = Vector3.one * 0.7f;
            
            openWardrobeUI.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
         }
        AnimateWorldItem(paperObject, paperOriginalPos, paperOriginalScale, 0.3f, 0.1f);
        AnimateWorldItem(gearObject, gearOriginalPos, gearOriginalScale, 0.3f, 0.25f);
    }
    private void AnimateWorldItem(Transform itemTransform, Vector3 targetPos, Vector3 targetScale, float offsetY, float delay)
    {
        if (itemTransform == null) return;

        itemTransform.gameObject.SetActive(true);

     
        Vector3 startPos = targetPos + new Vector3(0, offsetY, 0);

        itemTransform.position = startPos;
        itemTransform.localScale = Vector3.zero;

      
        itemTransform.DOMove(targetPos, 0.4f).SetDelay(delay).SetEase(Ease.OutBack);
        itemTransform.DOScale(targetScale, 0.4f).SetDelay(delay).SetEase(Ease.OutBack);
    }
    public void ClosePanel()
    {
        if(passwordInput != null) passwordInput.gameObject.SetActive(false);
        if(openWardrobeUI != null)openWardrobeUI.SetActive(false);
    }
}
