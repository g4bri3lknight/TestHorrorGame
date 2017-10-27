//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HDK_ExamineRotation : MonoBehaviour
{
    [Header("Examine Object Rotation")]
    public Transform target;
    public float speedy;
    public float speedx;
    private float rootx;
    private float rooty;
 //   public float ZoomScrollSpeed = 0.5f;

    void Update()
    {
        rooty += Input.GetAxis("Mouse Y") * speedy;
        rootx += Input.GetAxis("Mouse X") * speedx;
        rooty = Mathf.Clamp(rooty, -360, 360);
        target.eulerAngles = new Vector3(rooty, -rootx, 0);

     /*   if (target.GetComponent<HDK_ExaminableObjectSettings>().canZoom)
        {
            float MaxValue = target.GetComponent<HDK_ExaminableObjectSettings>().MaxZoom;
            float MinValue = target.GetComponent<HDK_ExaminableObjectSettings>().MinZoom;

            GetComponent<Camera>().fieldOfView += Input.GetAxis("Mouse ScrollWheel") * ZoomScrollSpeed;
            GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, MaxValue, MinValue);
        }*/
    }
}