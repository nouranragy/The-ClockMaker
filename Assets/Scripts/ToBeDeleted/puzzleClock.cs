using UnityEngine;
using DG.Tweening;

public class PuzzleClockDOTween : MonoBehaviour
{
    public AudioSource chimeAudioSource;
    public Transform hourHand;   
    public Transform minuteHand; 

    [Header("Angle Offsets")]
    public float hourAngleOffset = 0f; 
    public float minuteAngleOffset = 0f;
    
    // 10:10:00 = (10 * 3600) + (10 * 60) = 36600 seconds
    private float currentTimeInSeconds = 36600f; 
    
    private const float TARGET_TIME_SECONDS = 11700f; // 03:15:00
    private const float TWELVE_HOURS_SECONDS = 43200f; 

    private bool effectTriggeredThisCycle = false;

    void Awake()
    {
        ResetClockToTenTen();
    }

    void OnEnable()
    {
        ResetClockToTenTen();
    }

    public void ResetClockToTenTen()
    {
        currentTimeInSeconds = 36600f;
        effectTriggeredThisCycle = currentTimeInSeconds > TARGET_TIME_SECONDS;
        UpdateHandsRotation();
    }

    void Update()
    {
        float previousTime = currentTimeInSeconds;
        currentTimeInSeconds += Time.deltaTime * 60f; 

        if (currentTimeInSeconds >= TWELVE_HOURS_SECONDS)
        {
            currentTimeInSeconds -= TWELVE_HOURS_SECONDS;
            effectTriggeredThisCycle = false;
        }

        UpdateHandsRotation();

        if (!effectTriggeredThisCycle && previousTime < TARGET_TIME_SECONDS && currentTimeInSeconds >= TARGET_TIME_SECONDS)
        {
            TriggerClockEvent();
        }
    }

    void UpdateHandsRotation()
    {
        float hours = (currentTimeInSeconds / 3600f) % 12f;
        float minutes = (currentTimeInSeconds / 60f) % 60f;

        if (hourHand != null)   
            hourHand.localRotation = Quaternion.Euler(0, 0, (-hours * 30f) + hourAngleOffset);
            
        if (minuteHand != null) 
            minuteHand.localRotation = Quaternion.Euler(0, 0, (-minutes * 6f) + minuteAngleOffset);
    }

    void TriggerClockEvent()
    {
        effectTriggeredThisCycle = true;

        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.5f);

        if (chimeAudioSource != null)
        {
            chimeAudioSource.Play();
        }

        Debug.Log("Clock reached 03:15!");
    }
}