using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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


    [Header("Multi-Candelabra & Clock Settings")]
    public CandleCandelabraPuzzle otherCandelabra; 
    public GameObject clockObject;                 
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
        if (otherCandelabra != null && otherCandelabra.isCandelabraSolved)
        {
            Debug.Log("Both Candelabras Solved! Showing Clock...");
            ShowClock();
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
