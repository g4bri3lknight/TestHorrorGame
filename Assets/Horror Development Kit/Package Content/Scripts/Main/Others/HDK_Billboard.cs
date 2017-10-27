//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_Billboard : MonoBehaviour
{
    Camera cameraToLookAt;

    void Update()
    {
        if (GameObject.Find("Player") != null)
        {
            cameraToLookAt = GameObject.Find("Camera").GetComponent<Camera>();
            Vector3 v = cameraToLookAt.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt(cameraToLookAt.transform.position - v);
        }
    }
}