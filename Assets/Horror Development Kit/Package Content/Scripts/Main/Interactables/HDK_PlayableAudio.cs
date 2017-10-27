//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_PlayableAudio : MonoBehaviour
{
    [Header("Playable Audio")]
    public bool isPlaying;
    public float AudioVolume;
    public AudioSource SourceAudio;
    public AudioClip[] Sounds; //Sounds or sound to play

    void Update()
    {
        if (SourceAudio.isPlaying)
        {
            isPlaying = true;
        }
        else
        {
            isPlaying = false;
        }
    }

    public void PlaySound()
    {
        SourceAudio.clip = Sounds[Random.Range(0, Sounds.Length)];
        SourceAudio.volume = AudioVolume;
        SourceAudio.Play();
    }    
}