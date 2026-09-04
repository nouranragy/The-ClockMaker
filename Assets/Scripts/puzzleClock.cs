using UnityEngine;
using DG.Tweening;

public class PuzzleClockDOTween : MonoBehaviour
{
    public AudioSource chimeAudioSource;
    public Transform hourHand;   
    public Transform minuteHand; 
    private bool puzzleTriggered = false;
    private float currentTimeInSeconds = 0f;
    private const float TARGET_TIME_SECONDS = 11700f; 
    void Update()
    {
        if (puzzleTriggered) return;
        currentTimeInSeconds += Time.deltaTime * 60f; 
       float hours = (currentTimeInSeconds / 3600f) % 12f;
       float minutes = (currentTimeInSeconds / 60f) % 60f;
       if (hourHand != null)   hourHand.localRotation = Quaternion.Euler(0, 0, -hours * 30f);
       if (minuteHand != null) minuteHand.localRotation = Quaternion.Euler(0, 0, -minutes * 6f);
       if (currentTimeInSeconds >= TARGET_TIME_SECONDS) TriggerClockEvent();
    }
    void TriggerClockEvent()
    {
        puzzleTriggered = true;
        if (hourHand != null) hourHand.localRotation = Quaternion.Euler(0, 0, -97.5f);
        if (minuteHand != null) minuteHand.localRotation = Quaternion.Euler(0, 0, -90f);

        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f);
        if (chimeAudioSource != null)  chimeAudioSource.Play();
        Debug.Log("Clock reached 03:15!");
    }
}