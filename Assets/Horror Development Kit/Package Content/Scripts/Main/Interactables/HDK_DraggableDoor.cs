//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;

public enum SafeMode { Open, Locked, Jammed }

public class HDK_DraggableDoor : MonoBehaviour {

    [Header("Door Settings")]
    public SafeMode TypeOfSafe = SafeMode.Open;

    [Header("Jammed Barricades")]
    public HDK_DoorBarricade[] Barricades;
    public int barricadesValue;

    [Header("Sounds")]
    AudioSource aSource;
    public AudioClip Unlocked;

    private void Start()
    {
        aSource = GetComponent<AudioSource>();
        if (TypeOfSafe == SafeMode.Jammed)
        {
            barricadesValue = Barricades.Length;
        }
    }

    private void Update()
    {
        if (TypeOfSafe == SafeMode.Jammed)
        {
            if (barricadesValue == 0)
            {
                TypeOfSafe = SafeMode.Open;
            }
        }
    }

    public void UnlockDoor()
    {
        if(TypeOfSafe == SafeMode.Locked)
        {
            TypeOfSafe = SafeMode.Open;
            aSource.clip = Unlocked;
            aSource.Play();
        }
    }

    public void PlayAudioClip(AudioClip sound)
    {
        aSource.clip = sound;
        aSource.Play();
    }
}