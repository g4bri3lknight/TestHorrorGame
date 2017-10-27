//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class SwayWeapon : MonoBehaviour {
public float amount = 0.02f;
public float maxAmount = 0.03f;
public float Smooth = 3;
public float SmoothRotation = 2;
public float tiltAngle = 25;
 
private Vector3 def;
 
void Start (){
    def = transform.localPosition;
}
 
void Update (){
        float factorX = -Input.GetAxis("Mouse X") * amount;
        float factorY = -Input.GetAxis("Mouse Y") * amount;
       
        if (factorX > maxAmount)
            factorX = maxAmount;
       
        if (factorX < -maxAmount)
                factorX = -maxAmount;
 
        if (factorY > maxAmount)
                factorY = maxAmount;
       
        if (factorY < -maxAmount)
                factorY = -maxAmount;
               
 
        Vector3 Final = new Vector3(def.x+factorX, def.y+factorY, def.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Final, Time.deltaTime * Smooth);
       
             
        float tiltAroundZ= Input.GetAxis("Mouse X") * tiltAngle;
        float tiltAroundX= Input.GetAxis("Mouse Y") * tiltAngle;
		Quaternion target= Quaternion.Euler (tiltAroundX, 0, tiltAroundZ);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target,Time.deltaTime * SmoothRotation);    
}
}