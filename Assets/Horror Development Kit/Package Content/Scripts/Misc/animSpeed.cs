//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class animSpeed : MonoBehaviour {

	[Header ("Animation Speed")]
	public float speed = 0.2f;
	public string clipName = "";
	
	void Start()
	{
		gameObject.GetComponent<Animation>()[ clipName ].speed = speed;
	}
}
