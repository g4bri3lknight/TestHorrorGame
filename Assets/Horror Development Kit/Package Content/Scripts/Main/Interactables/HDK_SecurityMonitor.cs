//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class HDK_SecurityMonitor : MonoBehaviour {

    [Header("Secuiry Monitor")]
    public GameObject CameraObj;
    GameObject Player;
    GameObject mainCam;
    bool hasHBob;
    bool hasPeak;
    bool hasAudioList;

    void Start ()
    {
        Player = GameObject.Find("Player");
        mainCam = GameObject.Find("Camera");
	}

    public void StartCam ()
    {
        CameraObj.SetActive(true);

        if (mainCam.GetComponent<AudioListener>() != null)
        {
            hasAudioList = true;
            mainCam.GetComponent<AudioListener>().enabled = false;
        }
        else
        {
            hasAudioList = false;
        }

        if (Player.GetComponentInChildren<HeadBobController>() != null)
        {
            hasHBob = true;
            Player.GetComponentInChildren<HeadBobController>().enabled = false;
        }
        else
        {
            hasHBob = false;
        }
        if (Player.GetComponentInChildren<HDK_Leaning>() != null)
        {
            hasPeak = true;
            Player.GetComponentInChildren<HDK_Leaning>().enabled = false;
        }
        else
        {
            hasPeak = false;
        }
        if (Player.GetComponent<HDK_Stamina>() != null)
        {
            Player.GetComponent<HDK_Stamina>().Busy(true);
        }
    }

    public void EndCam ()
    {
        CameraObj.SetActive(false);

        if (hasAudioList)
        {
            mainCam.GetComponent<AudioListener>().enabled = true;
        }

        if (hasHBob)
        {
            Player.GetComponentInChildren<HeadBobController>().enabled = true;
        }
        if (hasPeak)
        {
            Player.GetComponentInChildren<HDK_Leaning>().enabled = true;
        }
        if (Player.GetComponent<HDK_Stamina>() != null)
        {
            Player.GetComponent<HDK_Stamina>().Busy(false);
        }
    }
}
