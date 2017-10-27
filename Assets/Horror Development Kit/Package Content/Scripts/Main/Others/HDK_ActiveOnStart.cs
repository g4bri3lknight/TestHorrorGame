//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDK_ActiveOnStart : MonoBehaviour {

    public GameObject[] ToActiveObjects;

	void Start () {

        foreach(GameObject obj in ToActiveObjects)
        {
            obj.SetActive(true);
        }
	}
}
