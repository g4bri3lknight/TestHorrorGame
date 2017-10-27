using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{

    [Header("Chest Settings")]
    public SafeMode TypeOfSafe = SafeMode.Locked;


    [Header("Option")]
    public bool isLocked = true;
    public Animation OpenAnimation;


    [Header("Sounds")]
    public AudioClip JammedAudioClip;
    public AudioClip LockedAudioClip;
    public AudioClip UnlockedAudioClip;
    public AudioClip OpenAudioClip;


    private AudioSource aSource;

    // Use this for initialization
    void Start()
    {
        aSource = GetComponent<AudioSource>();
    }
	
    // Update is called once per frame
    void Update()
    {

    }

    public void PerformJammedAudio()
    {
        if (TypeOfSafe == SafeMode.Jammed)
        {
            aSource.clip = JammedAudioClip;
            aSource.Play();
        }
    }

    public void PerformLockedAudio()
    {
        if (TypeOfSafe == SafeMode.Locked)
        {
            aSource.clip = LockedAudioClip;
            aSource.Play();
        }
    }

    public void UnlockDoor()
    {
        if (TypeOfSafe == SafeMode.Locked)
        {
            isLocked = false;
            aSource.clip = UnlockedAudioClip;
            aSource.Play();
        }
    }

    public void PerformOpenAnimation()
    {
        if (!isLocked)
        {
            aSource.clip = OpenAudioClip;
            OpenAnimation.Play();
            aSource.Play();
            GetComponent<Collider>().enabled = false;
            TypeOfSafe = SafeMode.Open;
        }
    }
}
