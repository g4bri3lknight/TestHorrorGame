//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDK_OpenLinkOnClick : MonoBehaviour {

    [Header("URL Settings")]
    public string URL;
    public float Delay;

    public void StartLink()
    {
        StartCoroutine("OpenURL");
    }

    IEnumerator OpenURL()
    {
        if (Delay != 0)
        {
            yield return new WaitForSeconds(Delay);
            Application.OpenURL(URL);
        }
        else
        {
            Application.OpenURL(URL);
        }
    }
}
