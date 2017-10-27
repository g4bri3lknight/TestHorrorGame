//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_SwitchableLamp : MonoBehaviour {

	[Header ("Functional / Switchable Lamps")]
	public bool isOn;
	public bool FlickLight;

	public AudioClip LampSwitchOn;
	public AudioClip LampSwitchOff;
	public float volumeSwitch;

	public MeshRenderer LampMesh;
	public Material ActivedLampMaterial;
	public Material DisabledLampMaterial;

	public Light lampLight;
	public AudioSource audioSource_switch; //AudioSource for the Switch Sounds
	public AudioSource audioSource_noise; //AudioSource for the Noise Sound


	void Update ()
	{
		if (isOn) {
			LampMesh.material = ActivedLampMaterial;			
		} else 
		{
			LampMesh.material = DisabledLampMaterial;	
		}
	}

	public void SwitchOn()
	{
		audioSource_switch.PlayOneShot (LampSwitchOn, volumeSwitch);
		lampLight.enabled = true;
		isOn = true;
		if (FlickLight) 
		{
            GetComponentInChildren<HDK_LightFlicker>().enabled = true;
            audioSource_noise.enabled = true;
		}
	}

	public void SwitchOff()
	{
		audioSource_switch.PlayOneShot (LampSwitchOff, volumeSwitch);
		lampLight.enabled = false;
		isOn = false;
        if (FlickLight)
        {
            GetComponentInChildren<HDK_LightFlicker>().enabled = false;
            audioSource_noise.enabled = false;
        }
    }
}