using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
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
        
        PlayBG();
        
    }

    public void PlayBG()
    {
        musicSource.clip = backgroundMusic;
        musicSource.volume = 0.5f;
        musicSource.pitch = 1f;
        musicSource.Play();

    }
    public void PlaySFX(AudioClip clip)
    {
        if(clip == null)
        {
            return;
        }
        sfxSource.PlayOneShot(clip);
        sfxSource.volume = 0.5f;
    }

    public void SetMusicPitch(float pitch)
    {
        musicSource.pitch = pitch;
    }

    public void Stop()
    {
        musicSource.Stop();
    }


}
