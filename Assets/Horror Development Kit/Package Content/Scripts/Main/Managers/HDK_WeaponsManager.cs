//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using Lean.Localization;

public class HDK_WeaponsManager : MonoBehaviour
{
    [Header("Flashlight Infos")]
    public bool usingFlashlight;
    public bool hasFlashlight;

    [Header("Melee Infos")]
    public bool usingMelee;
    public bool hasMelee;
    public GameObject currentMelee;

    [Header("Gun Infos")]
    public bool usingGun;
    public bool hasGun;
    public GameObject currentGun;

    [Header("Animations")]
    public string WalkName = "";
    public string RunName = "";
    public string IdleName = "";
    bool IsWalk;
    bool IsIdle;
    bool IsRun;

    [Header("Hitmarker")]
    public bool enableHitmarker;
    public Texture hitmarker;
    private float alphaHit;
    public AudioClip hitSound;

    GameObject Player;
    AudioSource audioSource;

    bool animatingMelee;
    bool animatingGun;
    bool animatingFlashlight;

    private void Start()
    {
        Player = GameObject.Find("Player");
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool paused = HDK_PauseManager.GamePaused;
        bool inventory = HDK_InventoryManager.inventoryOpen;
        usingFlashlight = Player.GetComponent<HDK_Flashlight>().usingFlashlight;
        hasFlashlight = Player.GetComponent<HDK_Flashlight>().hasFlashlight;

        if (alphaHit > 0)
            alphaHit -= Time.deltaTime;

        if (currentMelee != null)
        {
            hasMelee = true;
        }
        else
        {
            hasMelee = false;
        }

        if (currentGun != null)
        {
            hasGun = true;
        }
        else
        {
            hasGun = false;
        }

        if (hasMelee)
        {
            if (currentMelee.GetComponent<HDK_MeleeWeapon>().weaponAnim.IsPlaying(currentMelee.GetComponent<HDK_MeleeWeapon>().putDownAnim) ||
                currentMelee.GetComponent<HDK_MeleeWeapon>().weaponAnim.IsPlaying(currentMelee.GetComponent<HDK_MeleeWeapon>().drawAnim))
            {
                animatingMelee = true;
            }
            else
            {
                animatingMelee = false;
            }
        }
        if (hasGun)
        {
            if (currentGun.GetComponent<HDK_FireWeapon>().weaponAnim.IsPlaying(currentGun.GetComponent<HDK_FireWeapon>().drawAnim) ||
                currentGun.GetComponent<HDK_FireWeapon>().weaponAnim.IsPlaying(currentGun.GetComponent<HDK_FireWeapon>().putdownAnim))
            {
                animatingGun = true;
            }
            else
            {
                animatingGun = false;
            }
        }
        if (hasFlashlight)
        {
            if (Player.GetComponent<HDK_Flashlight>().ArmsAnims.GetComponent<Animation>().IsPlaying(Player.GetComponent<HDK_Flashlight>().DrawName) ||
                Player.GetComponent<HDK_Flashlight>().ArmsAnims.GetComponent<Animation>().IsPlaying(Player.GetComponent<HDK_Flashlight>().PutDownName))
            {
                animatingFlashlight = true;
            }
            else
            {
                animatingFlashlight = false;
            }
        }

        if (!animatingGun && !animatingFlashlight && !animatingMelee && !examining && !security && !reading && !paused && !inventory)
        {
            if (Input.GetButtonDown("MeleeSwitch"))
            {
                if (!hasMelee)
                {
                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("NoMeleeEquipped"));
                }
                else if (hasMelee)
                {
                    if (usingMelee)
                    {
                        StartCoroutine(PutdownMelee(false, false));
                    }
                    else if (!usingMelee)
                    {
                        if (usingFlashlight)
                        {
                            Player.GetComponent<HDK_Flashlight>().PutdownFlashlight(1);
                        }
                        else if (usingGun)
                        {
                            gun_Putdown(true, false);
                        }
                        else if (!usingFlashlight && !usingGun)
                        {
                            DrawMelee();
                        }
                    }
                }
            }
            
            if (Input.GetButtonDown("FiregunSwitch"))
            {
                if (!hasGun)
                {
                    Player.GetComponent<HDK_UITextManager>().SendMessage("ShowTextInfo", LeanLocalization.GetTranslationText("NoFireGunEquipped"));
                }
                else if (hasGun)
                {
                    if (usingGun)
                    {
                        StartCoroutine(PutdownGun(false, false));
                    }
                    else if (!usingGun)
                    {
                        if (usingFlashlight)
                        {
                            Player.GetComponent<HDK_Flashlight>().PutdownFlashlight(2);
                        }
                        else if (usingMelee)
                        {
                            melee_Putdown(true, false);
                        }
                        else if (!usingFlashlight && !usingMelee)
                        {
                            DrawGun();
                        }
                    }
                }
            }
        }

        if (Player.GetComponent<FirstPersonController>().m_CharacterController.velocity.sqrMagnitude > 0)
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
        if (Player.GetComponent<FirstPersonController>().isRunning)
        {
            IsWalk = false;
            IsRun = true;
        }
        else
        {
            IsRun = false;
        }
        if (Player.GetComponent<FirstPersonController>().isRunning && !Player.GetComponent<FirstPersonController>().CanRun)
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

        if (hasGun && !currentGun.GetComponent<HDK_FireWeapon>().aiming)
        {
            if (IsWalk)
            {
                GetComponent<Animation>().CrossFade(WalkName, 0.3f, PlayMode.StopAll);
                GetComponent<Animation>().wrapMode = WrapMode.Loop;
            }
            if (IsRun)
            {
                GetComponent<Animation>().CrossFade(RunName, 0.3f, PlayMode.StopAll);
                GetComponent<Animation>().wrapMode = WrapMode.Loop;
            }
            if (IsIdle)
            {
                GetComponent<Animation>().CrossFade(IdleName, 0.3f, PlayMode.StopAll);
                GetComponent<Animation>().wrapMode = WrapMode.Loop;
            }
        }
        else
        {
            if (IsWalk || IsRun || IsIdle)
            {
                GetComponent<Animation>().CrossFade("static", 0.3f, PlayMode.StopAll);
                GetComponent<Animation>().wrapMode = WrapMode.Loop;
            }
        }              
    }

    //MELEE
    public IEnumerator PutdownMelee(bool drawgun, bool removemelee)
    {
        if (usingMelee)
        {
            currentMelee.GetComponent<Animation>().Play(currentMelee.GetComponent<HDK_MeleeWeapon>().putDownAnim);
            audioSource.clip = currentMelee.GetComponent<HDK_MeleeWeapon>().PutDownSound;
            audioSource.volume = currentMelee.GetComponent<HDK_MeleeWeapon>().soundsVolume;
            audioSource.Play();
            yield return new WaitForSeconds(currentMelee.GetComponent<HDK_MeleeWeapon>().putDownLenght);
            currentMelee.SetActive(false);
            usingMelee = false;
            if (drawgun)
            {
                DrawGun();
            }
            if (removemelee)
            {
                currentMelee = null;
            }
        }
        else
        {
            if (removemelee)
            {
                currentMelee = null;
            }
        }
    }

    public void DrawMelee()
    {
        usingMelee = true;
        currentMelee.SetActive(true);
        currentMelee.GetComponent<Animation>().Play(currentMelee.GetComponent<HDK_MeleeWeapon>().drawAnim);
        audioSource.clip = currentMelee.GetComponent<HDK_MeleeWeapon>().DrawSound;
        audioSource.volume = currentMelee.GetComponent<HDK_MeleeWeapon>().soundsVolume;
        audioSource.Play();
    }

    public void melee_Putdown(bool drawgun, bool removemelee)
    {
        StartCoroutine(PutdownMelee(drawgun, removemelee));
    }

    public void SetMelee(GameObject melee)
    {
        currentMelee = melee;
    }
    //

    //GUN
    public IEnumerator PutdownGun(bool drawmelee, bool removegun)
    {
        if (usingGun)
        {
            currentGun.GetComponent<HDK_FireWeapon>().PutdownGun();
            yield return new WaitForSeconds(currentGun.GetComponent<HDK_FireWeapon>().putdownTime);
            currentGun.SetActive(false);
            usingGun = false;
            if (drawmelee)
            {
                DrawMelee();
            }
            if (removegun)
            {
                currentGun = null;
            }
        }
        else
        {
            if (removegun)
            {
                currentGun = null;
            }
        }
    }

    public void DrawGun()
    {
        usingGun = true;
        currentGun.SetActive(true);
        currentGun.GetComponent<HDK_FireWeapon>().DrawGun();
    }

    public void gun_Putdown(bool drawmelee, bool removegun)
    {
        StartCoroutine(PutdownGun(drawmelee, removegun));
    }

    public void SetWeapon(GameObject gun)
    {
        currentGun = gun;
    }
    //

    //HITMARKER
    public void HitMarker()
    {
        StartCoroutine(DrawCrosshair());        
    }

    IEnumerator DrawCrosshair()
    {
        yield return new WaitForSeconds(0.05f);
        alphaHit = 1.0f;
        audioSource.PlayOneShot(hitSound, 0.2f);
    }

    void OnGUI()
    {
        GUI.color = new Color(1.0f, 1.0f, 1.0f, alphaHit);
        GUI.DrawTexture(new Rect((Screen.width / 2) - 16, (Screen.height / 2) - 16, 32, 32), hitmarker);
    }
    //

    //ADD GUN AMMO
    public void AddAmmos(int ammo)
    {
        currentGun.GetComponent<HDK_FireWeapon>().AddMagazines(ammo);
    }
    //
}