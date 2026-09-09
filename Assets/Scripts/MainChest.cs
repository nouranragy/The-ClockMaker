using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;

public class MainChest : MonoBehaviour, IPointerClickHandler
{

    [Header ("Slots Configuration")]
    public ChestSlotButton slot1;
    public ChestSlotButton slot2;

    [Header ("Box Objects")]
    public GameObject closedBox;
    public GameObject openBox;

    [Header ("Audio & Effects")]
    public AudioSource audioSource;
    public AudioClip rattleSound;
    public AudioClip openSound;


    [Header ("Animation Settings")]
    public float animDuration = 0.35f;

    private bool isUnlocked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(openBox != null)openBox.SetActive(false);
        if(closedBox != null) closedBox.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       if(isUnlocked) return;
       PlayRattleAnimation();
    }

    private void PlayRattleAnimation()
    {
        if(audioSource != null && rattleSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(rattleSound);
        }
        closedBox.transform.DOComplete();
        closedBox.transform.DOShakePosition(0.4f, strength: new Vector3(0.15f, 0, 0), vibrato:10, randomness:90).OnComplete(()=>
        {
          if(audioSource != null && !isUnlocked)
            {
                audioSource.Stop();
            } 
        });
    }

    public void CheckPuzzleCompletion()
    {
        if(isUnlocked) return;
        if(slot1 != null && slot2!= null)
        {
            if(slot1.IsCorrectlyPlaced() && slot2.IsCorrectlyPlaced())
            {
                UnLockAndOpen();
            }
        }

    }

   private void UnLockAndOpen()
    {
        isUnlocked = true;
        if (audioSource != null)
        {
            audioSource.Stop();

            if (openSound != null)
            {
                AudioSource.PlayClipAtPoint(openSound, Camera.main.transform.position);
            }
            else
            {
                Debug.LogWarning("openSound clip is missing in Inspector!");
            }
        }
        closedBox.transform.DOPunchScale(new Vector3(0.15f, -0.15f, 0), 0.15f, 5, 1).OnComplete(() =>
        {
            closedBox.SetActive(false);
            openBox.SetActive(true);
            openBox.transform.localScale = Vector3.zero;
            openBox.transform.DOScale(Vector3.one , animDuration).SetEase(Ease.OutBack);
        });
    }
}
