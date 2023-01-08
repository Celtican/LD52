using System;
using UnityEngine;

public class PlaySoundOnStart : MonoBehaviour
{
    public AudioController.Audio soundToPlay;

    private void Start()
    {
        AudioController.instance.PlaySound(soundToPlay);
    }
}