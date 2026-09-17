using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;
using Photon.Pun;
using ExitGames.Client.Photon;

public class MainChest : MonoBehaviour, IPointerClickHandler
{

    [Header ("Slots Configuration")]
    public ChestSlotButton slot1;
    public ChestSlotButton slot2;

    [Header ("Box Objects")]
    public GameObject closedBox;
    public GameObject openBox;
   [Header ("Animation Settings")]
    public float animDuration = 0.35f;

   public const string CHEST_UNLOCKED_KEY = "MainChestUnlocked";

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
        if(SoundManager.Instance != null)
        {
            SoundManager.Instance.StopSFX();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.chestRattleSound);
        }
        closedBox.transform.DOComplete();
        closedBox.transform.DOShakePosition(0.4f, strength: new Vector3(0.15f, 0, 0), vibrato:10, randomness:90).OnComplete(()=>
        {
          if(!isUnlocked&& SoundManager.Instance != null )
            {
                SoundManager.Instance.StopSFX();
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

        if (PhotonNetwork.InRoom)
        {
            Hashtable props = new Hashtable {{ CHEST_UNLOCKED_KEY, true}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            Debug.Log("[MainChest] Chest Opened! Sent signal to reveal Wall Puzzle in Past.");
        }
       if(SoundManager.Instance != null)
        {
            SoundManager.Instance.StopSFX();
            SoundManager.Instance.PlaySFX(SoundManager.Instance.chestOpenSound);
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
