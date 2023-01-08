using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;
    public static AudioController instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(Audio audio)
    {
        audioSource.volume = audio.volume + Random.Range(-audio.randomVolume, audio.randomVolume);
        audioSource.pitch = audio.pitch + Random.Range(-audio.randomPitch, audio.randomPitch);
        audioSource.PlayOneShot(audio.clip);
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }


    [Serializable]
    public class Audio
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 0.8f;
        [Range(0f, 0.5f)] public float randomVolume = 0.1f;
        [Range(0f, 1f)] public float pitch = 1f;
        [Range(0f, 0.5f)] public float randomPitch = 0.1f;
    }
}