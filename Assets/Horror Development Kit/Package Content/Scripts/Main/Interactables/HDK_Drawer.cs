//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDK_Drawer : MonoBehaviour {

    [Header("Drawer Settings")]
    public SafeMode TypeOfSafe = SafeMode.Open;

    [Header ("Position Settings")]
    [SerializeField]
    private Vector3 openPosition, closedPosition;
    [SerializeField]
    private float animationTime;
    [SerializeField]
    private bool isOpen = false;
    private Hashtable iTweenArgs;

    [Header ("Sounds")]
    private AudioSource aSource;
    public AudioClip Open;
    public AudioClip Close;
    public AudioClip Unlocked;

    public bool performing;

    void Start()
    {
        iTweenArgs = iTween.Hash();
        iTweenArgs.Add("position", openPosition);
        iTweenArgs.Add("time", animationTime);
        iTweenArgs.Add("islocal", true);
        aSource = GetComponent<AudioSource>();
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
            }else
            {
                aSource.clip = Open;
                aSource.Play();
            }
        }

        if (isOpen)
        {
            iTweenArgs["position"] = closedPosition;
        }
        else
        {
            iTweenArgs["position"] = openPosition;
        }

        isOpen = !isOpen;

        iTween.MoveTo(gameObject, iTweenArgs);
        performing = true;
        StartCoroutine(resetPerforming());
    }

    IEnumerator resetPerforming()
    {
        yield return new WaitForSeconds(animationTime);
        performing = false;
    }
}