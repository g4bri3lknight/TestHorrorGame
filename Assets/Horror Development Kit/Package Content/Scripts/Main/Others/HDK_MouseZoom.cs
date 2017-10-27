//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class HDK_MouseZoom : MonoBehaviour {

	public float normal;
	public float zoom;
	public float smooth;
	public bool isZoomed;
	public bool canZoom;
    Camera main_camera;
    Camera weapon_camera;
	GameObject zoomIcon;

	public void ZoomOut()
	{
		isZoomed = false;
		canZoom = false;
	}

	void Start ()
    {
		main_camera = GameObject.Find ("Camera").GetComponent<Camera>();
        weapon_camera = GameObject.Find("WeaponsCamera").GetComponent<Camera>();
        zoomIcon = GameObject.Find ("icon_zooming");
	}

	void Update ()
    {
		var d = Input.GetAxis("Mouse ScrollWheel");
	
		if (!GameObject.Find("Player").GetComponent<HDK_DigitalCamera>().UsingCamera)
        {
			if (canZoom)
            {
				if (d > 0f)
				{
					isZoomed = true;
				}
				else if (d < 0f)
				{
					isZoomed = false;
				}
                else if (Input.GetButtonDown("EyeZoom"))
                {
                    isZoomed = !isZoomed;
                }
			}

			if (isZoomed)
            {
				main_camera.fieldOfView = Mathf.Lerp (main_camera.fieldOfView, zoom, Time.deltaTime * smooth);
                weapon_camera.fieldOfView = Mathf.Lerp(weapon_camera.fieldOfView, zoom, Time.deltaTime * smooth);
                zoomIcon.GetComponent<HDK_UIFade>().FadeIn();
            }
            else
            {            
				main_camera.fieldOfView = Mathf.Lerp (main_camera.fieldOfView, normal, Time.deltaTime * smooth);
                weapon_camera.fieldOfView = Mathf.Lerp(weapon_camera.fieldOfView, normal, Time.deltaTime * smooth);
                zoomIcon.GetComponent<HDK_UIFade>().FadeOut();

            }
		}
	}
}