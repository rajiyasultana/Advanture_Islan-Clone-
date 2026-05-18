using UnityEngine;

public static class SoundManager
{
    private static AudioSource backgroundSound;
    public enum SoundType
    {
        backGround,
        PlayerAttack,
        jump,
        itemCollect,
        eggItemCollect,
        enemyDeath,
        playerDeath
    }

    public static void InitializeBackgroundSound()
    {
        GameObject backgroundSoundGameObject = new GameObject("BackgroundSound");
        backgroundSound = backgroundSoundGameObject.AddComponent<AudioSource>();

        backgroundSound.loop = true;
        backgroundSound.playOnAwake = false;
        backgroundSound.volume = 0.5f;

        Object.DontDestroyOnLoad(backgroundSoundGameObject);
    }

    public static void PlayBackgroundSound(bool forceRestart = false)
    {
        if (backgroundSound == null)
        {
            InitializeBackgroundSound();
        }

        backgroundSound.clip = GetAudioClip(SoundType.backGround);
        if (!forceRestart && backgroundSound.isPlaying && backgroundSound.clip == backgroundSound.clip)
            return;
        
        backgroundSound.Play();
    }

    public static void StopBackgroundSound()
    {
        if (backgroundSound != null && backgroundSound.isPlaying)
        {
            backgroundSound.Stop();
        }
    }
    public static void PlaySound(SoundType soundType)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(soundType));
        audioSource.volume = 0.5f;
    }

    private static AudioClip GetAudioClip(SoundType soundType)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.instance.soundAudioClipArray)
        {
            if (soundAudioClip.soundType == soundType)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogWarning("Sound type " + soundType + " not found!");
        return null;
    }

}
