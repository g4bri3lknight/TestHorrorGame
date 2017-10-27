//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {
    
	[Header ("Auto-destroy settings")]
    public float destroyAfter = 15.0f;

    void Start() {
        Destroy(gameObject, destroyAfter);		//THE GAMEOBJECT WHERE YOU WILL ATTACH 
    }
}
