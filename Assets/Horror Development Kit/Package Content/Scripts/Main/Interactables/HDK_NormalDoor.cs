//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using UnityEngine;

public class HDK_NormalDoor : MonoBehaviour {

    [Header("Door Settings")]
    public SafeMode TypeOfSafe = SafeMode.Open;

    [Header("Jammed Barricades")]
    public HDK_DoorBarricade[] Barricades;
    public int barricadesValue;

    [Header("Position Settings")]
    [SerializeField]
    private Vector3 openRotation, closedRotation;
    [SerializeField]
    private float animationTime;
    [SerializeField]
    private bool isOpen = false;
    private Hashtable iTweenArgs;

    [Header("Sounds")]
    private AudioSource aSource;
    public AudioClip Open;
    public AudioClip Close;
    public AudioClip Unlocked;

    public bool performing;

    void Start()
    {
        iTweenArgs = iTween.Hash();
        iTweenArgs.Add("rotation", openRotation);
        iTweenArgs.Add("time", animationTime);
        iTweenArgs.Add("islocal", true);
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
        if (TypeOfSafe == SafeMode.Locked)
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

    public void PerformAction()
    {
        if (aSource)
        {
            if (isOpen)
            {
                aSource.clip = Close;
                aSource.Play();
            }
            else
            {
                aSource.clip = Open;
                aSource.Play();
            }
        }

        if (isOpen)
        {
            iTweenArgs["rotation"] = closedRotation;
        }
        else
        {
            iTweenArgs["rotation"] = openRotation;
        }

        isOpen = !isOpen;

        iTween.RotateTo(gameObject, iTweenArgs);

        performing = true;
        StartCoroutine(resetPerforming());
    }

    IEnumerator resetPerforming()
    {
        yield return new WaitForSeconds(animationTime);
        performing = false;        
    }
}