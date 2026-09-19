using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance {get ; private set;}

    [Header ("Audio Sources")]
    public AudioSource sfxSource;
    public  AudioSource bgmSource;

    [Header ("Audio Clips")]
    public AudioClip chestRattleSound;
    public AudioClip chestOpenSound;
    public AudioClip itemPickupSound;
    public AudioClip grandfatherClockSound;

    public AudioClip backgroundMusic;

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
    private void Start()
    {
        
        if (bgmSource != null && backgroundMusic != null && !bgmSource.isPlaying)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.loop = true;
            bgmSource.Play();
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
