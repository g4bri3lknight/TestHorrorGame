//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_StabilizeKickback : MonoBehaviour
{
    public float returnSpeed = 2.0f;
    public Transform myTransform;

    void LateUpdate()
    {
        myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
    }
}