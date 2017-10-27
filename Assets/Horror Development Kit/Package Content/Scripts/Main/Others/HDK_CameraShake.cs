//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_CameraShake : MonoBehaviour {

	float shakeStrength;
	public float shake = 1;
    public Vector3 originalPosition;

    public void Shake(float value)
    {
        shake = value;
        shakeStrength = value;
    }

	void LateUpdate()
	{
		Camera.main.transform.localPosition = originalPosition + (Random.insideUnitSphere * shake);
		
		shake = Mathf.MoveTowards(shake, 0, Time.deltaTime * shakeStrength);
		
		if(shake == 0)
		{
			Camera.main.transform.localPosition = originalPosition;
		}
	}
}