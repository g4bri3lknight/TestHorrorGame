//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace UnityStandardAssets.Characters.FirstPerson
{
	public class HDK_Flashlight : MonoBehaviour {

	[Header ("Booleans")]						
	public bool IsWalk;
	public bool IsRun;
	public bool IsIdle;
	public bool IsDraw;
	public bool IsPutDown;

	[Header ("Animations")]						
	public GameObject ArmsAnims;
	public string WalkName;
	public string RunName;
	public string IdleName;
	public string DrawName;
	public string PutDownName;

	[Header ("Flashlight Options")]				
	public float health;
	public float MaxHealth;
    public float DecreaseSpeed;
	public Light lightSource;
	public AudioClip DrawSound;
	public AudioClip PutDownSound;
	public AudioClip ChangeBattery;
	public AudioClip NoBattery;
	public float volumeSound;
	AudioSource audio_source;
	float DrawLenght;
	float UnDrawLenght;
	public bool hasFlashlight;
    public bool usingFlashlight;
	float duration = 0.2f;
	float baseIntensity;
	float alpha;
	GameObject Player;
	bool WasNotCharge;

    [Header("Flickering Mode")]
    public bool Enabled;

    [Header ("UI")]
	GameObject FlashlightUI;
	GameObject BatteryIcon;

	[Header("Battery UI")]
	Image _20percent;
	Image _40percent;
	Image _60percent;
	Image _80percent;
	Image _100percent;

    public enum Dpad { None, Right, Left, Up, Down }
    private bool flag = true;

    void Start ()
	{
		audio_source = GetComponent<AudioSource> ();	
		DrawLenght = ArmsAnims.GetComponent<Animation> ().GetClip (DrawName).length;
		UnDrawLenght = ArmsAnims.GetComponent<Animation> ().GetClip (PutDownName).length;
		baseIntensity = lightSource.intensity;
		Player = GameObject.Find("Player");
		BatteryIcon = GameObject.Find ("BatteryIcon");
		FlashlightUI = GameObject.Find ("BatteryCounter");
		_20percent = GameObject.Find ("20%").GetComponent<Image> ();
		_40percent = GameObject.Find ("40%").GetComponent<Image> ();
		_60percent = GameObject.Find ("60%").GetComponent<Image> ();
		_80percent = GameObject.Find ("80%").GetComponent<Image> ();
		_100percent = GameObject.Find ("100%").GetComponent<Image> ();
    }

    public void HasFlashlight()
	{
		hasFlashlight = true;
	}

    public void PutdownFlashlight(int pickup) // if the value of that int is = 1 so it will draw the melee, if is = 2 it will draw the gun, if is = 0 it will be a normal putdown
    {
        StartCoroutine(Putdown(pickup));
    }

	IEnumerator Draw()
	{
		IsDraw = true;
		ShowArms ();
		audio_source.PlayOneShot (DrawSound, volumeSound);		
		yield return new WaitForSeconds (DrawLenght - 0.35f);
		IsDraw = false;
		lightSource.enabled = true;
		if (Enabled)
        {
			lightSource.gameObject.GetComponent<HDK_LightFlicker> ().enabled = true;
		}
        else 
		{
			lightSource.gameObject.GetComponent<HDK_LightFlicker> ().enabled = false;
		}
	}

    IEnumerator Putdown(int pickup)
	{
		IsPutDown = true;
		StartCoroutine (CompletePutDown ());
		yield return new WaitForSeconds (1);
		lightSource.enabled = false;
		audio_source.PlayOneShot (PutDownSound, volumeSound);
		if (Enabled)
        {
		    lightSource.gameObject.GetComponent<HDK_LightFlicker> ().enabled = false;
		}
        yield return new WaitForSeconds(1);
        if (pickup == 1)
        {
            GetComponentInChildren<HDK_WeaponsManager>().DrawMelee();
        }
        else if(pickup == 2)
        {
            GetComponentInChildren<HDK_WeaponsManager>().DrawGun();
        }
	}
	
	IEnumerator CompletePutDown()
	{
		yield return new WaitForSeconds (UnDrawLenght);
		IsPutDown = false;
		ShowOffArms ();
	}

	void ShowArms()
	{
		ArmsAnims.SetActive (true);
	}

	void ShowOffArms()
	{
		ArmsAnims.SetActive (false);
	}

    void PadControl()
    {
        if (Input.GetAxis("DpadX") == 0.0)
        {
            flag = true;
        }

        if (Input.GetAxis("DpadX") == 1f && flag)
        {
            StartCoroutine("DpadControl", Dpad.Right);
        }
        if (Input.GetAxis("DpadX") == -1f && flag)
        {
            StartCoroutine("DpadControl", Dpad.Left);
        }
        if (Input.GetAxis("DpadY") == 1f && flag)
        {
            StartCoroutine("DpadControl", Dpad.Up);
        }
        if (Input.GetAxis("DpadY") == -1f && flag)
        {
            StartCoroutine("DpadControl", Dpad.Down);

        }
    }

    IEnumerator DpadControl(Dpad value)
    {
        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool paused = HDK_PauseManager.GamePaused;
        bool inventory = HDK_InventoryManager.inventoryOpen;

        flag = false;
        yield return new WaitForSeconds(0.15f);
        if (value == Dpad.Right)
        {
            bool melee = GetComponentInChildren<HDK_WeaponsManager>().usingMelee;
            bool gun = GetComponentInChildren<HDK_WeaponsManager>().usingGun;

            if (hasFlashlight)
            {
                if (!IsDraw && !IsPutDown && !IsPutDown && !examining && !security && !reading && !paused && !inventory)
                {
                    if (usingFlashlight)
                    {
                        StartCoroutine(Putdown(0));
                    }
                    else
                    {
                        if (!melee)
                        {
                            StartCoroutine(Draw());
                        }
                        else
                        {
                            StartCoroutine(DrawAfterWeapon(1));
                        }

                        if (!gun)
                        {
                            StartCoroutine(Draw());
                        }
                        else
                        {
                            StartCoroutine(DrawAfterWeapon(2));
                        }
                    }
                }
            }
        }
        StopCoroutine("DpadControl");
    }        

    IEnumerator DrawAfterWeapon(int weapon) // weapon == 1 it's for melee, weapon == 2 it's for gun
    {
        if(weapon == 1)
        {
            GetComponentInChildren<HDK_WeaponsManager>().melee_Putdown(false,false);
            yield return new WaitForSeconds(GetComponentInChildren<HDK_WeaponsManager>().currentMelee.GetComponent<HDK_MeleeWeapon>().putDownLenght);
            StartCoroutine(Draw());
        }
        else if(weapon == 2)
        {
            GetComponentInChildren<HDK_WeaponsManager>().gun_Putdown(false,false);
            yield return new WaitForSeconds(GetComponentInChildren<HDK_WeaponsManager>().currentGun.GetComponent<HDK_FireWeapon>().putdownTime);
            StartCoroutine(Draw());
        }
    }

    public void DrawFlashlight()
    {
        bool melee = GetComponentInChildren<HDK_WeaponsManager>().usingMelee;
        bool gun = GetComponentInChildren<HDK_WeaponsManager>().usingGun;

        if (usingFlashlight)
        {
            StartCoroutine(Putdown(0));
        }
        else
        {
            if (!melee)
            {
                StartCoroutine(Draw());
            }
            else
            {
                StartCoroutine(DrawAfterWeapon(1));
            }

            if (!gun)
            {
                StartCoroutine(Draw());
            }
            else
            {
                StartCoroutine(DrawAfterWeapon(2));
            }
        }
    }

    public void Recharge()
    {
        audio_source.PlayOneShot(ChangeBattery, volumeSound);
        health += 20f;
    }

    void Update ()
    {

        PadControl();

        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool paused = HDK_PauseManager.GamePaused;
        bool inventory = HDK_InventoryManager.inventoryOpen;

        if (health == 0) 
		{
			_20percent.enabled = false;
			_40percent.enabled = false;
			_60percent.enabled = false;
			_80percent.enabled = false;
			_100percent.enabled = false;
		}
		else if (health <= 20) 		
		{
			_20percent.enabled = true;
			_40percent.enabled = false;
			_60percent.enabled = false;
			_80percent.enabled = false;
			_100percent.enabled = false;
		}
		else if (health <= 40 && health > 20) 		
		{
			_20percent.enabled = true;
			_40percent.enabled = true;
			_60percent.enabled = false;
			_80percent.enabled = false;
			_100percent.enabled = false;
		}
		else if (health <= 60 && health > 40) 		
		{
			_20percent.enabled = true;
			_40percent.enabled = true;
			_60percent.enabled = true;
			_80percent.enabled = false;
			_100percent.enabled = false;
		}
		else if (health <= 80 && health > 60) 		
		{
			_20percent.enabled = true;
			_40percent.enabled = true;
			_60percent.enabled = true;
			_80percent.enabled = true;
			_100percent.enabled = false;
		}
		else if (health <= 100 && health > 80) 		
		{
			_20percent.enabled = true;
			_40percent.enabled = true;
			_60percent.enabled = true;
			_80percent.enabled = true;
			_100percent.enabled = true;
		}

		if (usingFlashlight) 
		{
			if (Enabled) 
			{
				if (health <= 25f) {
					lightSource.gameObject.GetComponent<HDK_LightFlicker> ().enabled = false;
				} else 
				{
					lightSource.gameObject.GetComponent<HDK_LightFlicker> ().enabled = true;
				}
			}
		}

		if (hasFlashlight)
		{
			FlashlightUI.GetComponent<HDK_UIFade> ().FadeIn();
        }
		else 
		{
            FlashlightUI.GetComponent<HDK_UIFade>().FadeOut();
		}
        
		if (lightSource.isActiveAndEnabled)
        {
			if (health > 0.0f)
            {
				health -= Time.deltaTime * DecreaseSpeed;
			}
		}

		if(health < MaxHealth/4 && lightSource.enabled)
        { 
			float phi = Time.time / duration * 2 * Mathf.PI;
			float amplitude = Mathf.Cos( phi ) * (float)0.5 + baseIntensity;
			lightSource.GetComponent<Light>().intensity = amplitude + Random.Range(0.1f, 1.0f) ;
		}
		lightSource.GetComponent<Light>().color = new Color(alpha/MaxHealth, alpha/MaxHealth, alpha/MaxHealth, alpha/MaxHealth);
		alpha = health;  


		if (ArmsAnims.activeSelf)
		{
			usingFlashlight = true;
		}
		else
		{
			usingFlashlight = false;	
		}

		if (health < 20)
        {
			BatteryIcon.GetComponent<Image> ().color = Color.red;
		}
        else if (health >= 20) 
		{
			BatteryIcon.GetComponent<Image> ().color = Color.white;
		}


		if (health <= 0) 
		{
		health = 0;			
		}
        else if(health >= MaxHealth)
		{
			health  = MaxHealth;
		}

        bool melee = GetComponentInChildren<HDK_WeaponsManager>().usingMelee;
        bool gun = GetComponentInChildren<HDK_WeaponsManager>().usingGun;

        if (hasFlashlight)
		{
			if (!IsDraw && !IsPutDown && !IsPutDown && !examining && !security && !reading && !paused && !inventory) 
			{
				if (Input.GetButtonDown ("Flashlight"))
				{
					if (usingFlashlight)
                    {
						StartCoroutine(Putdown(0));
					}
                    else
					{
                        if (!melee)
                        {
                            StartCoroutine(Draw());
                        }
                        else
                        {
                            StartCoroutine(DrawAfterWeapon(1));
                        }

                        if (!gun)
                        {
                            StartCoroutine(Draw());
                        }
                        else
                        {
                            StartCoroutine(DrawAfterWeapon(2));
                        }
                    }
				}
			}
		}

	    if(Player.GetComponent<FirstPersonController>().m_CharacterController.velocity.sqrMagnitude > 0)
	    {
		    IsWalk = true;
		    IsIdle = false;
		    IsRun = false;
	    }
        else if (Player.GetComponent<FirstPersonController>().m_CharacterController.velocity.sqrMagnitude == 0)
	    {
		    IsWalk = false;
		    IsRun = false;
		    IsIdle = true;
	    }
	    if (Player.GetComponent<FirstPersonController> ().isRunning) {
		    IsWalk = false;
		    IsRun = true;
	    }
        else 
	    {
		    IsRun = false;
	    }
	    if (Player.GetComponent<FirstPersonController> ().isRunning && !Player.GetComponent<FirstPersonController> ().CanRun)
	    {
		    IsWalk = true;
		    IsRun = false;
		    IsIdle = false;
	    }
        if (reading || examining || security || paused)
        {
            IsWalk = false;
            IsRun = false;
            IsIdle = true;
        }

	    if (IsWalk)
	    {
		    ArmsAnims.GetComponent<Animation> ().CrossFade (WalkName, 0.3f, PlayMode.StopAll);
		    ArmsAnims.GetComponent<Animation> ().wrapMode = WrapMode.Loop;
	    }
	    if (IsRun) {
		    ArmsAnims.GetComponent<Animation> ().CrossFade (RunName, 0.3f, PlayMode.StopAll);
		    ArmsAnims.GetComponent<Animation> ().wrapMode = WrapMode.Loop;
	    }
	    if (IsIdle)
	    {
		    ArmsAnims.GetComponent<Animation> ().CrossFade (IdleName, 0.3f, PlayMode.StopAll);
		    ArmsAnims.GetComponent<Animation> ().wrapMode = WrapMode.Loop;
	    }
	    if (IsDraw)
	    {
		    ArmsAnims.GetComponent<Animation> ().Play (DrawName, PlayMode.StopAll);
	    }
	    if (IsPutDown)
	    {
		    ArmsAnims.GetComponent<Animation> ().Play (PutDownName, PlayMode.StopAll);
	    }
	}
}
}