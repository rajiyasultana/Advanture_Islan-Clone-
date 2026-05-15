using UnityEngine;

public static class SoundManager
{
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
    public static void PlaySound(SoundType soundType)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(soundType));
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
