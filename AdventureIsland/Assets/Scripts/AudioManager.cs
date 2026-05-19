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
        musicSource.Play();

    }
    public void PlaySFX(AudioClip clip)
    {
        if(clip == null)
        {
            return;
        }
        sfxSource.PlayOneShot(clip);
    }

    public void Stop()
    {
        musicSource.Stop();
    }


}
