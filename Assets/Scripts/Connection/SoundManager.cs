using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get ; private set;}

    [Header ("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;

    [Header ("Audio Clips")]
    public AudioClip chestRattleSound;
    public AudioClip chestOpenSound;
    public AudioClip itemPickupSound;
    public AudioClip grandfatherClockSound;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if(clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    public void StopSFX()
    {
        if(sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

} 
