//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class HDK_WithEmission : MonoBehaviour {

	[Header ("Raycast Highlight")]
	public bool rayed;				//Do you raycasted the object?
	GameObject mesh;				//The mesh of the object
	public Material normal_mat;		//The normal material of the object	
	public Material raycasted_mat;  //The Highlighted material of the object
    GameObject rayedObj;
    HDK_RaycastManager rayScript;

    void Start()
	{
		mesh = gameObject;
        rayScript = GameObject.Find("Player").GetComponentInChildren<HDK_RaycastManager>();
    }

	void Update () {

        if (rayScript)
        {
            if (rayScript.raycasted_obj == null)
            {
                rayed = false;
            }
            else
            {
                if (rayScript.raycasted_obj != this.gameObject)
                {
                    rayed = false;
                }
            }
        }

		if (rayed) 
		{
			mesh.GetComponent<MeshRenderer> ().material = raycasted_mat;
		} else 
		{
			mesh.GetComponent<MeshRenderer> ().material = normal_mat;
		}

	}
}
