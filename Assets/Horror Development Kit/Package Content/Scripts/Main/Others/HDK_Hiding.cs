//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDK_Hiding : MonoBehaviour {

    GameObject GUI_Indicator;
    public bool Hiding;

    private void Start()
    {
        GUI_Indicator = GameObject.Find("icon_hiding");
    }

    public void OnHide (bool withGUI)
    {
        Hiding = true;
        if (withGUI)
        {
            GUI_Indicator.GetComponent<HDK_UIFade>().FadeIn();
        }      
	}

    public void OffHide (bool withGUI)
    {
        Hiding = false;
        if (withGUI)
        {
            GUI_Indicator.GetComponent<HDK_UIFade>().FadeOut();
        }
    }
}