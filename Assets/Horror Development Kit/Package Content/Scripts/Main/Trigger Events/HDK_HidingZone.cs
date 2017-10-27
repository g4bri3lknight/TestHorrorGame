//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDK_HidingZone : MonoBehaviour
{
    GameObject Player;
    public bool ShowGUI;

    void Start()
    {
        Player = GameObject.Find("Player");
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Player.GetComponent<HDK_Hiding>().OnHide(ShowGUI);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            Player.GetComponent<HDK_Hiding>().OffHide(ShowGUI);
        }
    }
}