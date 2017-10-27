//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

	public class HDK_BrokeCamera : MonoBehaviour {

	GameObject Player;

	void Start () {
		Player = GameObject.Find ("Player");
	}

	void OnTriggerEnter (Collider col) {
		
		if (col.tag == "Player" && !Player.GetComponent<HDK_DigitalCamera>().broken && Player.GetComponent<HDK_DigitalCamera>().UsingCamera) {
			Player.GetComponent<HDK_DigitalCamera>().BrokeIt = true;
		}
	}
}