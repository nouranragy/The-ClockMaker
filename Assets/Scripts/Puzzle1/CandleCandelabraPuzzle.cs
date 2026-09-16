using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class CandleCandelabraPuzzle : MonoBehaviour
{
   [System.Serializable] 
   public class SlotRequirement
    {
        public string requiredColor;
        [HideInInspector] public bool isCorrect = false;
    }

    [Header("Puzzle Settings")]
    public List<SlotRequirement> slots;
    
    // [Header ("Darkness Overlay for this Candelabre")]
    // public GameObject darknessOverlay;

    public Light2D spotLightDarkness;

    [Header("Multi-Candelabra & Clock Settings")]
    public CandleCandelabraPuzzle otherCandelabra; 
    public GameObject clockObject;     

    public GameObject Collider;            
    [HideInInspector] public bool isCandelabraSolved = false;

       public void PlaceCandleInSlot(int index, CandleItem candle)
    {
        if(isCandelabraSolved || index < 0 || index >= slots.Count ) return;
        if(candle.candleColor == slots[index].requiredColor)
        {
            slots[index].isCorrect = true;
        }
        else
        {
            slots[index].isCorrect = false;
        }
        CheckPuzzleState();
    }
    private void CheckPuzzleState()
    {
        foreach(var slot in slots)
        {
            if(!slot.isCorrect) return;
        }

        isCandelabraSolved= true;
        Debug.Log(gameObject.name + "Solved");

        RemoveDarkness();
        if (otherCandelabra != null && otherCandelabra.isCandelabraSolved)
        {
            Debug.Log("Both Candelabras Solved! Showing Clock...");
            ShowClock();
        }
    }

    private void RemoveDarkness()
    {
        // if(darknessOverlay != null)
        // {
        //     Collider2D col = darknessOverlay.GetComponent<Collider2D>();
        //     if(col != null) col.enabled = false;

        //     SpriteRenderer sr = darknessOverlay.GetComponent<SpriteRenderer>();
        //     if(sr != null)
        //     {
        //         sr.DOFade(0f,0.8f).OnComplete(() => {darknessOverlay.SetActive(false);});
        //     }
        //     else
        //     {
        //         darknessOverlay.SetActive(false);
        //     }
        // }

        if(spotLightDarkness != null)
        {
            DOTween.To(() => spotLightDarkness.intensity,x => spotLightDarkness.intensity = x,0f,1.2f ).OnComplete(() =>{spotLightDarkness.gameObject.SetActive(false);});
            
             if(Collider!= null) Collider.SetActive(false);
 
        }
    }

    private void ShowClock()
    {
        if (clockObject != null)
        {
            clockObject.SetActive(true); 
        }
    }
}
