//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

namespace UnityStandardAssets.ImageEffects{
public class HDK_DigitalCamera : MonoBehaviour {

    [Header ("Digital Camera")]
    public AudioClip CameraOn;
    public AudioClip[] FoleySound;
    AudioSource audio_s;
    public GameObject CameraUI;
    GameObject mainCam;
    GameObject anim;
    GameObject Player;
    public float sounds_volume;
    Animation NoItemsHands;
    bool foleying;
    public static bool canUse;
    public bool HasCamera;
    public bool UsingCamera;
    bool TakeCamera;
    bool PutOutCamera;

    [Header("Zoom")]
    public float normal;
    public float zoom;
    public float smooth;
    public bool isZoomed;
    public bool canZoom;
    Camera main_camera;
    Camera weapons_camera;
    public AudioClip brokenZoom;
    public AudioClip ZoomIn;
    public AudioClip ZoomOut;
    AudioSource zoomAudioSource;
    bool zooming;
    public float lenght;
    public Slider lineIndicator;

    [Header("Broken Camera")]
    public AudioClip broke_sound;
    public AudioSource BrokenNoise;
    public GameObject brokenGUI;
    public GlitchEffect camera_effect;
    public bool broken;
    public bool BrokeIt;

    [Header("Night Vision")]
    public bool nv_canUse;
    public bool nv_Enabled = false;
    public float nv_Life;
    public float nv_MaxLife;
    public float nv_SpeedIncrease;
    public float nv_SpeedDecrease;
    public AudioClip nv_on;
    public AudioClip nv_off;
    public AudioClip nv_batterydead;
    public AudioSource nv_audiosource;
    private bool nv_soundplayed = true;
    private bool nv_batterysoundplayed = true;
    public float nv_soundvolume;
    private bool nv_turnOn = false;
    public Image nv_LifeGUI;        

    public enum Dpad { None, Right, Left, Up, Down }
    private bool flag = true;

    void Start()
	{
        anim = GameObject.Find ("CamHolder");
        Player = GameObject.Find ("Player");
        canUse = true;
        audio_s = GameObject.Find("audio_Camera").GetComponent<AudioSource> ();
        main_camera = GameObject.Find ("Camera").GetComponent<Camera>();
        weapons_camera = GameObject.Find("WeaponsCamera").GetComponent<Camera>();
        zoomAudioSource = GameObject.Find("audio_CameraZoom").GetComponent<AudioSource>();                    
    }

    IEnumerator TakeCam()
	{
		Player.GetComponent<HDK_MouseZoom> ().SendMessage ("ZoomOut");
		anim.GetComponent<Animation> ().Play ("CamFoley", PlayMode.StopAll);
        foleying = true;
		audio_s.clip = FoleySound [Random.Range (0, FoleySound.Length)];
		audio_s.volume = sounds_volume;
		audio_s.Play ();
		yield return new WaitForSeconds (2.5f);
		audio_s.PlayOneShot (CameraOn, sounds_volume);
		CameraUI.SetActive (true);
        CameraUI.GetComponent<CanvasGroup>().alpha = 1;
        UsingCamera = true;
        foleying = false;        
        weapons_camera.GetComponent<Fisheye> ().enabled = true;
        if (broken)
        {
            brokenGUI.GetComponent<CanvasGroup>().alpha = 1;
            BrokenNoise.gameObject.SetActive(true);
            brokenGUI.SetActive (true);
            weapons_camera.GetComponent<GlitchEffect> ().enabled = true;
            canZoom = false;
        }
        else
        {
            canZoom = true;
        }
         
        if (nv_Enabled)
        {
            weapons_camera.GetComponent<ColorCorrectionCurves>().enabled = true;
            nv_Enabled = true;
        }
    }

	IEnumerator PutDownCam()
	{			
		if (isZoomed)
        {
            isZoomed = false;
		}
        canZoom = false;
		Player.GetComponent<HDK_MouseZoom> ().enabled = true;
		Player.GetComponent<HDK_MouseZoom> ().canZoom = true;
		anim.GetComponent<Animation> ().Play ("CamFoley", PlayMode.StopAll);
        foleying = true;
        audio_s.clip = FoleySound [Random.Range (0, FoleySound.Length)];
		audio_s.volume = sounds_volume;
		audio_s.Play ();
		yield return new WaitForSeconds (2.5f);
		UsingCamera = false;
        foleying = false;
        CameraUI.SetActive (false);
        weapons_camera.GetComponent<Fisheye> ().enabled = false;        
		GetComponent<HDK_MouseZoom> ().canZoom = true;        
        if (broken)
        {
			brokenGUI.SetActive (false);
            weapons_camera.GetComponent<GlitchEffect> ().enabled = false;
            BrokenNoise.gameObject.SetActive(false);
        }
        if (nv_Enabled)
        {
            weapons_camera.GetComponent<ColorCorrectionCurves>().enabled = false;
            nv_Enabled = false;
        }
	}

    public void CameraUse(bool Draw)
    {
        if (Draw)
        {
            TakeCamera = true;
        }
        else
        {
            PutOutCamera = true;
        }
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
        if (value == Dpad.Left)
        {
            if (HasCamera)
            {
                if (canUse)
                {
                    if (!examining && !security && !reading && !foleying && !paused && !inventory)
                    {
                            if (!UsingCamera)
                            {
                                TakeCamera = true;
                            }
                            if (UsingCamera)
                            {
                                PutOutCamera = true;
                            }
                    }
                }
            }
        }
        if (value == Dpad.Up)
        {
            if (UsingCamera && nv_canUse && !examining && !security && !reading && !paused && !inventory)
            {
                if (nv_Enabled)
                {
                    nv_turnOn = false;
                }
                else
                {
                    nv_turnOn = true;
                }
                nv_soundplayed = false;                    
            }
        }
        StopCoroutine("DpadControl");
    }

    void Update ()
    {
        PadControl();

        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool paused = HDK_PauseManager.GamePaused;
        bool inventory = HDK_InventoryManager.inventoryOpen;
            
        if (isZoomed)
        {
            main_camera.fieldOfView = Mathf.Lerp(main_camera.fieldOfView, zoom, Time.deltaTime * smooth);
            weapons_camera.fieldOfView = Mathf.Lerp(main_camera.fieldOfView, normal, Time.deltaTime * smooth);
        }
        else
        {
            main_camera.fieldOfView = Mathf.Lerp(main_camera.fieldOfView, normal, Time.deltaTime * smooth);
            weapons_camera.fieldOfView = Mathf.Lerp(main_camera.fieldOfView, normal, Time.deltaTime * smooth);
        }

        if (zooming || foleying)
        {
            if (!isZoomed)
            {
                lineIndicator.value -= Time.deltaTime * lenght;
            }
            else
            {
                lineIndicator.value += Time.deltaTime * lenght;
            }
        }

        if (BrokeIt)
        {
			broken = true;
			BrokeIt = false;
			audio_s.PlayOneShot (broke_sound, sounds_volume);
			brokenGUI.SetActive (true);
			camera_effect.enabled = true;
            BrokenNoise.gameObject.SetActive(true);
            if (isZoomed)
            {
                isZoomed = false;
                canZoom = false;
			}
		}

		if (TakeCamera) 
		{
			TakeCamera = false;
			StartCoroutine(TakeCam ());
		}

		if (PutOutCamera) 
		{
			PutOutCamera = false;
			StartCoroutine(PutDownCam());
		}
        
        if (HasCamera) 
		{
            if (canUse)
            {
                if (!examining && !security && !reading && !foleying && !paused && !inventory)
                {
                    if (Input.GetButtonUp("DigitalCamera"))
                    {
                        if (!UsingCamera)
                        {
                            TakeCamera = true;
                        }
                        if (UsingCamera)
                        {
                            PutOutCamera = true;
                        }
                    }
                }
            }
		}
                
        if (nv_turnOn)
        {
            if (UsingCamera && !examining && !security)
            {
                    weapons_camera.GetComponent<ColorCorrectionCurves>().enabled = true;
                nv_Enabled = true;
            }

            if (!nv_soundplayed)
            {
                nv_audiosource.PlayOneShot(nv_on);
                nv_audiosource.volume = nv_soundvolume;
                nv_soundplayed = true;
            }
        }

        if (!nv_turnOn)
        {
            weapons_camera.GetComponent<ColorCorrectionCurves>().enabled = false;
            nv_Enabled = false;
            if (!nv_soundplayed || !nv_batterysoundplayed)
            {
                nv_audiosource.volume = nv_soundvolume;
                if (nv_batterysoundplayed)
                {
                    nv_audiosource.PlayOneShot(nv_off);
                    nv_soundplayed = true;
                }
                else
                {
                    nv_audiosource.PlayOneShot(nv_batterydead);
                    nv_batterysoundplayed = true;
                }
            }
        }

        if (UsingCamera && nv_canUse && !examining && !security && !reading && !paused && !inventory)
        {
            if (Input.GetButtonUp("NightMode"))
            {
                if (nv_Enabled)
                {
                    nv_turnOn = false;
                }
                else
                {
                    nv_turnOn = true;
                }
                nv_soundplayed = false;
            }
        }

        if (UsingCamera && !examining && !security && !reading && !paused && !inventory && !zooming && canZoom)
        {
            if (Input.GetButtonDown("DigitalCameraZoom"))
            {
                if (!broken)
                {
                    if (isZoomed)
                    {
                        StartCoroutine(zoomming());
                        zoomAudioSource.clip = ZoomOut;
                        zoomAudioSource.volume = 1;
                        zoomAudioSource.Play();
                        isZoomed = false;
                    }
                    else
                    {
                        StartCoroutine(zoomming());
                        zoomAudioSource.clip = ZoomIn;
                        zoomAudioSource.volume = 1;
                        zoomAudioSource.Play();
                        isZoomed = true;
                    }
                }
                else
                {
                    zoomAudioSource.clip = brokenZoom;
                    zoomAudioSource.volume = 0.3f;
                    zoomAudioSource.Play();
                }
            }
        }
        else if (UsingCamera && !examining && !security && !reading && !paused && !inventory && !zooming)
        {
            if (Input.GetButtonDown("DigitalCameraZoom"))
            {
                if (broken)
                {
                    zoomAudioSource.PlayOneShot(brokenZoom);
                }
            }
        }

        if (nv_Life <= 0)
        {
            nv_Life = 0;
            nv_turnOn = false;
            nv_batterysoundplayed = false;
        }
        else if (nv_Life >= nv_MaxLife)
        {
            nv_Life = nv_MaxLife;
        }

        float nvhealth = nv_Life / 100f;
        nv_LifeGUI.fillAmount = nvhealth;

        if (HasCamera)
        {
            if (!nv_Enabled)
            {                    
                nv_Life += Time.deltaTime * nv_SpeedIncrease;
            }
            else
            {
                nv_Life -= Time.deltaTime * nv_SpeedDecrease;
            }
        }
    }

    IEnumerator zoomming()
    {
        zooming = true;
        yield return new WaitForSeconds(lenght);
        zooming = false;
    }
}
}