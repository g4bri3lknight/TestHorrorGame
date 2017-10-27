//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson
{
public class HDK_Stamina : MonoBehaviour 
{

	[Header ("Stamina Settings")]
	public float speedDecrease;
	public float speedIncrease;
	public float stamina;
	GameObject BarRect;
	FirstPersonController Player;
    bool busy;
    public AudioClip BreathSound;
    public AudioSource Breath_AudioSource;

    void Start()
	{
		BarRect = GameObject.Find ("_Stamina");
		Player = GameObject.Find ("Player").GetComponent<FirstPersonController>();
        Breath_AudioSource.clip = BreathSound;
        Breath_AudioSource.Play();
    }

    void Update()
    {
        BarRect.transform.localScale = new Vector3(stamina, 1, 1);
        bool canrun = HDK_Footsteps.CanRun;
        bool backwards =  GetComponent<FirstPersonController>().Backwards;

        float difference = 100f - stamina;
        if(stamina <= 20)
        {
            Breath_AudioSource.volume = difference / 100f;
        }
        else
        {
            Breath_AudioSource.volume -= Time.deltaTime;
        }

        if (Player.isRunning && Player.m_CanRun && !busy) {
        stamina -= Time.deltaTime * speedDecrease;
        } else
        {
            stamina += Time.deltaTime * speedIncrease;
        }

        if (stamina <= 0)
        {
            stamina = 0;
            Player.GetComponent<FirstPersonController>().m_CanRun = false;
        }

    if (backwards)
    {
        Player.m_CanRun = false;
        }
        else
        {
            if (stamina >= 20 && canrun)
            {
                Player.m_CanRun = true;
            }
        }

        if (stamina >= 100)
        {
            stamina = 100;
        }

        if (!busy)
        {
        if (Player.isRunning)
        {
            BarRect.GetComponentInParent<CanvasGroup>().alpha += Time.deltaTime;
        }
        else
        {
            BarRect.GetComponentInParent<CanvasGroup>().alpha -= Time.deltaTime / 2;
        }
        }

        if(busy)
        {
            BarRect.GetComponentInParent<CanvasGroup>().alpha -= Time.deltaTime / 2;
        }
    }

    public void Busy( bool busy_value)
    {
        busy = busy_value;
    }
}
}