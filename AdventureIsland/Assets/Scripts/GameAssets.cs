using System;
using UnityEngine;


public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public static GameAssets instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            
            }
            return _instance;
        }
    }

    public SoundAudioClip[] soundAudioClipArray;

    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.SoundType soundType;
        public AudioClip audioClip;
    }
}
