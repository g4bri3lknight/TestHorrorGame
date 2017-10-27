//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_SecurityCamera : MonoBehaviour {

  public float rotationAmount;

        void Update()
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, (Mathf.Sin(Time.realtimeSinceStartup) * rotationAmount ) + transform.eulerAngles.y, transform.eulerAngles.z);
        }          
}