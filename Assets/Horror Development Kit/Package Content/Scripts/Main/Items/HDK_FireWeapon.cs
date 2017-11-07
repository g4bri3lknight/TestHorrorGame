//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

public enum fireMode
{
    none,
    semi,
    auto,
    burst,
    shotgun
}

public enum Ammo
{
    Magazines,
    Bullets

}

public enum Aim
{
    Simple,
    Sniper

}

public class HDK_FireWeapon : MonoBehaviour
{
    [HideInInspector]
    public fireMode currentMode = fireMode.semi;
    public fireMode firstMode = fireMode.semi;
    public fireMode secondMode = fireMode.burst;
    public Ammo ammoMode = Ammo.Magazines;
    public Aim aimMode = Aim.Simple;

    [Header("ConsoleCommands")]
    public ConsoleCommands consoleCommands;

    [Header("UI")]
    GameObject mainGunHud;
    Text text_ammoCount;
    Text text_gunName;
    Text text_fireMode;

    [Header("Weapon configuration")]
    public string WeaponName = "";
    public LayerMask layerMask;
    public int damage = 50;
    public int bulletsPerMag = 50;
    public int magazines = 5;
    private float fireRate = 0.1f;
    public float fireRateFirstMode = 0.1f;
    public float fireRateSecondMode = 0.1f;
    public float range = 250.0f;
    public float force = 200.0f;

    [Header("Accuracy Settings")]
    public float baseInaccuracyAIM = 0.005f;
    public float baseInaccuracyHIP = 1.5f;
    public float inaccuracyIncreaseOverTime = 0.2f;
    public float inaccuracyDecreaseOverTime = 0.5f;
    private float maximumInaccuracy;
    public float maxInaccuracyHIP = 5.0f;
    public float maxInaccuracyAIM = 1.0f;
    private float triggerTime = 0.05f;
    private float baseInaccuracy;

    [Header("Aiming")]
    public Vector3 aimPosition;
    public bool aiming;
    private Vector3 curVect;
    private Vector3 hipPosition = Vector3.zero;
    public float aimSpeed = 0.25f;
    public float zoomSpeed = 0.5f;
    public int FOV = 40;
    public int weaponFOV = 45;

    private float scopeTime;
    private bool inScope = false;
    public Texture scopeTexture;

    [Header("Burst Settings")]
    public int shotsPerBurst = 3;
    public float burstTime = 0.07f;

    [Header("Shotgun Settings")]
    public int pelletsPerShot = 10;

    [Header("Kickback")]
    public Transform kickGO;
    public float kickUp = 0.5f;
    public float kickSideways = 0.5f;

    [Header("Crosshair")]
    public Texture2D crosshairFirstModeHorizontal;
    public Texture2D crosshairFirstModeVertical;
    public Texture2D crosshairSecondMode;
    private float adjustMaxCroshairSize = 6.0f;

    [Header("Bulletmarks")]
    public GameObject Concrete;
    public GameObject Wood;
    public GameObject Metal;
    public GameObject Dirt;
    public GameObject Blood;
    public GameObject Untagged;

    [Header("Audio")]
    public AudioSource aSource;
    public AudioClip soundDraw;
    public AudioClip soundPutdown;
    public AudioClip soundFire;
    public AudioClip soundReload;
    public AudioClip soundEmpty;
    public AudioClip switchModeSound;
    public AudioClip attachmentUse;

    [Header("Attachments Settings")]
    public bool hasLaser;
    bool usingLaser;
    public GameObject[] laserObj;
    public bool hasTacticalLight;
    bool usingTacticalLight;
    public GameObject[] tacticallightObj;
    public bool withSilencer;

    [Header("Animation Settings")]
    public Animation weaponAnim;

    public string fireAnim = "Fire";
    [Range(0.0f, 5.0f)]
    public float fireAnimSpeed = 1.0f;

    public string drawAnim = "Draw";
    [Range(0.0f, 5.0f)]
    public float drawAnimSpeed = 1.0f;
    [Range(0.0f, 5.0f)]
    public float drawTime = 1.5f;

    public string putdownAnim = "Putdown";
    [Range(0.0f, 5.0f)]
    public float putdownAnimSpeed = 1.0f;
    [Range(0.0f, 5.0f)]
    public float putdownTime = 1.5f;

    public string reloadAnim = "Reload";
    [Range(0.0f, 5.0f)]
    public float reloadAnimSpeed = 1.0f;
    [Range(0.0f, 5.0f)]
    public float reloadTime = 1.5f;

    public string fireEmptyAnim = "FireEmpty";
    [Range(0.0f, 5.0f)]
    public float fireEmptyAnimSpeed = 1.0f;

    public string switchAnim = "SwitchAnim";
    [Range(0.0f, 5.0f)]
    public float switchAnimSpeed = 1.0f;

    [Header("Other")]
    public CharacterController fpscontroller;
    public Renderer muzzleFlash;
    public Light muzzleLight;
    public Camera mainCamera;
    public Camera wepCamera;
    public bool selected = false;

    [HideInInspector]
    public bool reloading = false;
    [HideInInspector]

    private bool canSwicthMode = true;
    private bool draw;
    private bool putdown;
    private bool playing = false;
    private bool isFiring = false;
    private bool bursting = false;
    private int m_LastFrameShot = -10;
    private float nextFireTime = 0.0f;
    private int bulletsLeft = 0;
    private RaycastHit hit;
    private float camFOV = 65.0f;

 
    private HDK_InventorySlot slotGun;

    public enum Dpad
    {
        None,
        Right,
        Left,
        Up,
        Down

    }

    private bool flag = true;
    HDK_Hiding playerHiding;

    void Start()
    {
        fpscontroller = GameObject.Find("Player").GetComponent<CharacterController>();
        text_ammoCount = GameObject.Find("ammoCount").GetComponent<Text>();
        text_fireMode = GameObject.Find("fireMode").GetComponent<Text>();
        text_gunName = GameObject.Find("weaponName").GetComponent<Text>();
        muzzleFlash.enabled = false;
        muzzleLight.enabled = false;

        slotGun = consoleCommands.slotFiregun.GetComponent<HDK_InventorySlot>();
        bulletsLeft = slotGun.AmmosQuantity;

        currentMode = firstMode;
        fireRate = fireRateFirstMode;
        aiming = false;
        /*
        if (ammoMode == Ammo.Bullets)
        {
            magazines = magazines * bulletsPerMag;
        }
        */
        playerHiding = GameObject.Find("Player").GetComponent<HDK_Hiding>();

       
    }

    void Awake()
    {
        mainGunHud = GameObject.Find("FiregunHUD");
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
        if (value == Dpad.Down)
        {
            if (!reading && !examining && !security && !paused && !inventory)
            {
                if (!reloading && !weaponAnim.IsPlaying(switchAnim))
                {
                    if (hasLaser)
                    {
                        StartCoroutine(AttachmentAction("laser"));
                    }
                    else if (hasTacticalLight)
                    {
                        StartCoroutine(AttachmentAction("tactical_light"));
                    }
                }                
            }
        }
        StopCoroutine("DpadControl");
    }

    void Update()
    {
        PadControl();

        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool paused = HDK_PauseManager.GamePaused;
        bool inventory = HDK_InventoryManager.inventoryOpen;

        if (selected)
        {
            text_gunName.text = WeaponName;
        
            slotGun.SetAmmosQuantity(bulletsLeft);
            magazines = consoleCommands.getTotalAmmosQuantityForFireGun();

            if (bulletsLeft <= bulletsPerMag / 2.5f)
            {
                text_ammoCount.color = Color.red;
            }
            else
            {
                text_ammoCount.color = Color.white;
            }

            if (firstMode != fireMode.none || secondMode != fireMode.none)
            {
                text_ammoCount.text = bulletsLeft.ToString() + " / " + magazines.ToString();
            }

            if (firstMode != fireMode.none || secondMode != fireMode.none)
            {
                text_fireMode.text = currentMode.ToString();
            }

            if (!reading && !examining && !security && !paused && !inventory && !playerHiding.Hiding)
            {
                if (Input.GetButtonDown("WeaponShoot"))
                {
                    if (currentMode == fireMode.semi)
                    {
                        FireSemi();
                    }
                    else if (currentMode == fireMode.burst)
                    {
                        StartCoroutine(FireBurst());
                    }
                    else if (currentMode == fireMode.shotgun)
                    {
                        FireShotgun();
                    }

                    if (bulletsLeft > 0)
                        isFiring = true;
                }
                
                if (Input.GetButton("WeaponShoot"))
                {
                    if (currentMode == fireMode.auto)
                    {
                        FireSemi();
                        if (bulletsLeft > 0)
                            isFiring = true;
                    }
                }
            }

            if (!reading && !examining && !security && !paused && !inventory)
            {
                if (Input.GetButtonDown("WeaponReload"))
                {
                    StartCoroutine(Reload());
                }
                
                if (Input.GetButtonDown("WeaponAttachment"))
                {
                    if (!reloading && !weaponAnim.IsPlaying(switchAnim))
                    {
                        if (hasLaser)
                        {
                            StartCoroutine(AttachmentAction("laser"));
                        }
                        else if (hasTacticalLight)
                        {
                            StartCoroutine(AttachmentAction("tactical_light"));
                        }
                    }                   
                }
            }         
        }

        if (!reading && !examining && !security && !paused && !inventory)
        {
            if (Input.GetButton("WeaponAim") && !reloading && selected && !FindObjectOfType<FirstPersonController>().isRunning)
            {
                if (!aiming)
                {
                    aiming = true;
                    curVect = aimPosition - transform.localPosition;
                    scopeTime = Time.time + aimSpeed;
                }

                if (transform.localPosition != aimPosition && aiming)
                {
                    if (Mathf.Abs(Vector3.Distance(transform.localPosition, aimPosition)) < curVect.magnitude / aimSpeed * Time.deltaTime)
                    {
                        transform.localPosition = aimPosition;
                    }
                    else
                    {
                        transform.localPosition += curVect / aimSpeed * Time.deltaTime;
                    }
                }

                if (aimMode == Aim.Sniper)
                {
                    if (Time.time >= scopeTime && !inScope)
                    {
                        inScope = true;
                        Component[] gos = GetComponentsInChildren<Renderer>();
                        foreach (var go in gos)
                        {
                            Renderer a = go as Renderer;
                            a.enabled = false;
                        }
                    }
                }
            }
            else
            {
                if (aiming)
                {
                    aiming = false;
                    inScope = false;
                    curVect = hipPosition - transform.localPosition;
                    if (aimMode == Aim.Sniper)
                    {
                        Component[] go = GetComponentsInChildren<Renderer>();
                        foreach (var g in go)
                        {
                            Renderer b = g as Renderer;
                            if (b.name != "muzzle_flash")
                                b.enabled = true;
                        }
                    }
                }

                if (Mathf.Abs(Vector3.Distance(transform.localPosition, hipPosition)) < curVect.magnitude / aimSpeed * Time.deltaTime)
                {
                    transform.localPosition = hipPosition;
                }
                else
                {
                    transform.localPosition += curVect / aimSpeed * Time.deltaTime;
                }
            }
        }

        if (aiming)
        {
            maximumInaccuracy = maxInaccuracyAIM;
            baseInaccuracy = baseInaccuracyAIM;
            if (aimMode == Aim.Sniper)
            {
                FindObjectOfType<HDK_MouseZoom>().enabled = false;
                mainCamera.fieldOfView -= FOV * Time.deltaTime / zoomSpeed;
                if (mainCamera.fieldOfView < FOV)
                {
                    mainCamera.fieldOfView = FOV;
                }
            }
            else
            {
                GetComponentInParent<TiltShift>().enabled = true;
            }

            if (GameObject.Find("Crosshair") != null)
            {
                GameObject.Find("Crosshair").GetComponent<CanvasGroup>().alpha = 0;
            }
        }
        else
        {
            maximumInaccuracy = maxInaccuracyHIP;
            baseInaccuracy = baseInaccuracyHIP;
            if (aimMode != Aim.Sniper)
            {
                GetComponentInParent<TiltShift>().enabled = false;
            }

            FindObjectOfType<HDK_MouseZoom>().enabled = true;

            if (GameObject.Find("Crosshair") != null)
            {
                GameObject.Find("Crosshair").GetComponent<CanvasGroup>().alpha = 1;
            }
        }

        if (fpscontroller.velocity.magnitude > 3.0f)
        {
            triggerTime += inaccuracyDecreaseOverTime;
        }

        if (isFiring)
        {
            triggerTime += inaccuracyIncreaseOverTime;
        }
        else
        {
            if (fpscontroller.velocity.magnitude < 3.0f)
                triggerTime -= inaccuracyDecreaseOverTime;
        }

        if (triggerTime >= maximumInaccuracy)
        {
            triggerTime = maximumInaccuracy;
        }

        if (triggerTime <= baseInaccuracy)
        {
            triggerTime = baseInaccuracy;
        }

        if (nextFireTime > Time.time)
        {
            isFiring = false;
        }

        if (Input.GetKey(KeyCode.P) && secondMode != fireMode.none && canSwicthMode)
        {
            if (currentMode != firstMode)
            {
                StartCoroutine(FirstFireMode());
            }
            else
            {
                StartCoroutine(SecondFireMode());
            }
        }
    }

    IEnumerator AttachmentAction(string attachment)
    {
        weaponAnim.Rewind(switchAnim);
        weaponAnim.Play(switchAnim);
        aSource.clip = attachmentUse;
        aSource.Play();
        yield return new WaitForSeconds(0.4f);
        if (attachment == "laser")
        {
            if (usingLaser)
            {
                usingLaser = false;
                foreach (GameObject go in laserObj)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                usingLaser = true;
                foreach (GameObject go in laserObj)
                {
                    go.SetActive(true);
                }
            }
        }
        else if (attachment == "tactical_light")
        {
            if (usingTacticalLight)
            {
                usingTacticalLight = false;
                foreach (GameObject go in tacticallightObj)
                {
                    go.SetActive(false);
                }
            }
            else
            {
                usingTacticalLight = true;
                foreach (GameObject go in tacticallightObj)
                {
                    go.SetActive(true);
                }
            }
        }
    }

    void LateUpdate()
    {
        if (withSilencer || inScope)
            return;

        if (m_LastFrameShot == Time.frameCount)
        {
            muzzleFlash.transform.localRotation = Quaternion.AngleAxis(Random.value * 360, Vector3.forward);
            muzzleFlash.enabled = true;
            muzzleLight.enabled = true;
        }
        else
        {
            muzzleFlash.enabled = false;
            muzzleLight.enabled = false;
        }
    }

    void OnGUI()
    {
        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool paused = HDK_PauseManager.GamePaused;
        bool inventory = HDK_InventoryManager.inventoryOpen;

        if (selected && !examining && !security && !reading && !paused && !inventory)
        {
            if (scopeTexture != null && inScope)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), scopeTexture, ScaleMode.StretchToFill);
            }
            else
            {
                float w = crosshairFirstModeHorizontal.width;
                float h = crosshairFirstModeHorizontal.height;
                Rect position1 = new Rect((Screen.width + w) / 2 + (triggerTime * adjustMaxCroshairSize), (Screen.height - h) / 2, w, h);
                Rect position2 = new Rect((Screen.width - w) / 2, (Screen.height + h) / 2 + (triggerTime * adjustMaxCroshairSize), w, h);
                Rect position3 = new Rect((Screen.width - w) / 2 - (triggerTime * adjustMaxCroshairSize) - w, (Screen.height - h) / 2, w, h);
                Rect position4 = new Rect((Screen.width - w) / 2, (Screen.height - h) / 2 - (triggerTime * adjustMaxCroshairSize) - h, w, h);
                if (!aiming)
                {
                    GUI.DrawTexture(position1, crosshairFirstModeHorizontal);   //Right
                    GUI.DrawTexture(position2, crosshairFirstModeVertical);     //Up
                    GUI.DrawTexture(position3, crosshairFirstModeHorizontal);   //Left
                    GUI.DrawTexture(position4, crosshairFirstModeVertical);     //Down
                }
            }
        }
    }

    IEnumerator FirstFireMode()
    {
        canSwicthMode = false;
        selected = false;
        weaponAnim.Rewind(switchAnim);
        weaponAnim.Play(switchAnim);
        aSource.clip = switchModeSound;
        aSource.Play();
        yield return new WaitForSeconds(0.6f);
        currentMode = firstMode;
        fireRate = fireRateFirstMode;
        selected = true;
        canSwicthMode = true;
    }

    IEnumerator SecondFireMode()
    {
        canSwicthMode = false;
        selected = false;
        aSource.clip = switchModeSound;
        aSource.Play();
        weaponAnim.Play(switchAnim);
        yield return new WaitForSeconds(0.6f);
        currentMode = secondMode;
        fireRate = fireRateSecondMode;
        selected = true;
        canSwicthMode = true;
    }

    void FireSemi()
    {
        if (reloading || bulletsLeft <= 0)
        {
            if (bulletsLeft == 0)
            {
                if (magazines > 0)
                {
                    SendMessage("Reload");
                }
                else
                {
                    StartCoroutine(OutOfAmmo());
                }
            }

            return;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        while (nextFireTime < Time.time)
        {
            FireOneBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator FireBurst()
    {
        int shotCounter = 0;

        if (reloading || bursting || bulletsLeft <= 0)
        {
            if (bulletsLeft <= 0)
            {
                StartCoroutine(OutOfAmmo());
            }
            yield break;
        }

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            while (shotCounter < shotsPerBurst)
            {
                bursting = true;
                shotCounter++;
                if (bulletsLeft > 0)
                {
                    FireOneBullet();
                }
                yield return new WaitForSeconds(burstTime);
            }
            nextFireTime = Time.time + fireRate;
        }
        bursting = false;
    }

    void FireShotgun()
    {
        if (reloading || bulletsLeft <= 0 || draw || putdown)
        {
            if (bulletsLeft == 0)
            {
                StartCoroutine(OutOfAmmo());
            }
            return;
        }

        int pellets = 0;

        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            while (pellets < pelletsPerShot)
            {
                FireOnePellet();
                pellets++;
            }
            bulletsLeft--;
            nextFireTime = Time.time + fireRate;
        }

        weaponAnim.Rewind(fireAnim);
        weaponAnim.Play(fireAnim);

        aSource.PlayOneShot(soundFire, 1.0f);

        m_LastFrameShot = Time.frameCount;
        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUp, Random.Range(-kickSideways, kickSideways), 0));
    }

    void FireOneBullet()
    {
        if (nextFireTime > Time.time || draw || putdown)
        {
            if (bulletsLeft <= 0)
            {
                StartCoroutine(OutOfAmmo());
            }
            return;
        }

        Vector3 dir = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * triggerTime, Random.Range(-0.01f, 0.01f) * triggerTime, 1));
        Vector3 pos = transform.parent.position;
        Ray ray;
        RaycastHit hit;
        ray = wepCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(pos, dir, out hit, range, layerMask))
        {
            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float rScale = Random.Range(0.5f, 1.0f);

            if (hit.collider.gameObject.GetComponentInParent<HDK_AIZombieStateMachine>())
            {
                hit.collider.gameObject.GetComponentInParent<HDK_AIZombieStateMachine>().TakeDamage(hit.point, ray.direction * 5.0f, damage, hit.rigidbody, 0);
                Instantiate(Blood, contact, rot);
                SendMessageUpwards("HitMarker");
            }
            if (hit.collider.gameObject.GetComponent<HDK_DoorBarricade>())
            {
                if (hit.collider.gameObject.GetComponent<HDK_DoorBarricade>().health > 0)
                {
                    hit.collider.SendMessage("ApplyDamage", damage);
                    GameObject woodMark = Instantiate(Wood, contact, rot) as GameObject;
                    woodMark.transform.localPosition += .02f * hit.normal;
                    woodMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                    woodMark.transform.parent = hit.transform;
                    SendMessageUpwards("HitMarker");
                }
            }
            if (hit.rigidbody)
            {
                hit.rigidbody.AddForceAtPosition(force * dir, hit.point);
                Debug.Log("HIT TARGET: " + hit.collider.gameObject.name);
            }   
            if (hit.collider.tag == "Concrete")
            {
                GameObject concMark = Instantiate(Concrete, contact, rot) as GameObject;
                concMark.transform.localPosition += .02f * hit.normal;
                concMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                concMark.transform.parent = hit.transform;
            }
            else if (hit.collider.tag == "Wood")
            {
                GameObject woodMark = Instantiate(Wood, contact, rot) as GameObject;
                woodMark.transform.localPosition += .02f * hit.normal;
                woodMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                woodMark.transform.parent = hit.transform;
            }
            else if (hit.collider.tag == "Metal")
            {
                GameObject metalMark = Instantiate(Metal, contact, rot) as GameObject;
                metalMark.transform.localPosition += .02f * hit.normal;
                metalMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                metalMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Dirt")
            {
                GameObject dirtMark = Instantiate(Dirt, contact, rot) as GameObject;
                dirtMark.transform.localPosition += .02f * hit.normal;
                dirtMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                dirtMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Enemy")
            {
                Instantiate(Blood, hit.point + (hit.normal * 0.1f), rot);
                hit.collider.SendMessageUpwards("GetDamage", damage, SendMessageOptions.DontRequireReceiver);
                hit.collider.SendMessageUpwards("GetHit", SendMessageOptions.DontRequireReceiver);
                SendMessageUpwards("HitMarker");
            }
            else if (hit.collider.tag == "Untagged")
            {
                GameObject def = Instantiate(Untagged, contact, rot) as GameObject;
                def.transform.localPosition += .02f * hit.normal;
                def.transform.localScale = new Vector3(rScale, rScale, rScale);
                def.transform.parent = hit.transform;
            }
            else if (hit.collider.tag == "Breakable")
            {
                hit.collider.SendMessage("DestroyIt");
            }
        }

        aSource.PlayOneShot(soundFire);
        m_LastFrameShot = Time.frameCount;

        weaponAnim[fireAnim].speed = fireAnimSpeed;
        weaponAnim.Rewind(fireAnim);
        weaponAnim.Play(fireAnim);

        kickGO.localRotation = Quaternion.Euler(kickGO.localRotation.eulerAngles - new Vector3(kickUp, Random.Range(-kickSideways, kickSideways), 0));

        bulletsLeft--;
    }

    void FireOnePellet()
    {
        Vector3 dir = gameObject.transform.TransformDirection(new Vector3(Random.Range(-0.01f, 0.01f) * triggerTime, Random.Range(-0.01f, 0.01f) * triggerTime, 1));
        Vector3 pos = transform.parent.position;

        if (Physics.Raycast(pos, dir, out hit, range, layerMask))
        {

            Vector3 contact = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            float rScale = Random.Range(0.5f, 1.0f);

            if (hit.rigidbody)
                hit.rigidbody.AddForceAtPosition(force * dir, hit.point);

            if (hit.collider.tag == "Concrete")
            {
                GameObject concMark = Instantiate(Concrete, contact, rot) as GameObject;
                concMark.transform.localPosition += .02f * hit.normal;
                concMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                concMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Enemy")
            {
                Instantiate(Blood, contact, rot);
                hit.collider.SendMessageUpwards("GetDamage", damage, SendMessageOptions.DontRequireReceiver);
                SendMessageUpwards("HitMarker");

            }
            else if (hit.collider.tag == "Wood")
            {
                GameObject woodMark = Instantiate(Wood, contact, rot) as GameObject;
                woodMark.transform.localPosition += .02f * hit.normal;
                woodMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                woodMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Metal")
            {
                GameObject metalMark = Instantiate(Metal, contact, rot) as GameObject;
                metalMark.transform.localPosition += .02f * hit.normal;
                metalMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                metalMark.transform.parent = hit.transform;

            }
            else if (hit.collider.tag == "Dirt")
            {
                GameObject dirtMark = Instantiate(Dirt, contact, rot) as GameObject;
                dirtMark.transform.localPosition += .02f * hit.normal;
                dirtMark.transform.localScale = new Vector3(rScale, rScale, rScale);
                dirtMark.transform.parent = hit.transform;

            }
            else
            {
                GameObject def = Instantiate(Untagged, contact, rot) as GameObject;
                def.transform.localPosition += .02f * hit.normal;
                def.transform.localScale = new Vector3(rScale, rScale, rScale);
                def.transform.parent = hit.transform;
            }
        }
    }

    IEnumerator OutOfAmmo()
    {
        if (reloading || playing)
            yield break;

        playing = true;
        aSource.PlayOneShot(soundEmpty, 0.3f);
        if (fireEmptyAnim != "")
        {
            weaponAnim.Rewind(fireEmptyAnim);
            weaponAnim.Play(fireEmptyAnim);
        }
        yield return new WaitForSeconds(0.2f);
        playing = false;

    }

    IEnumerator Reload()
    {
        if (reloading)
            yield break;

        if (ammoMode == Ammo.Magazines)
        {
            reloading = true;
            canSwicthMode = false;
            if (magazines > 0 && bulletsLeft != bulletsPerMag)
            {
                weaponAnim[reloadAnim].speed = reloadAnimSpeed;
                weaponAnim.Play(reloadAnim, PlayMode.StopAll);
                //weaponAnim.CrossFade(reloadAnim);
                aSource.PlayOneShot(soundReload);
                yield return new WaitForSeconds(reloadTime);
  
                if (magazines + bulletsLeft >= bulletsPerMag)
                {
                    magazines -= (bulletsPerMag - bulletsLeft);
                    bulletsLeft += (bulletsPerMag - bulletsLeft);
                }
                else
                {
                    bulletsLeft += magazines;  
                    magazines = 0;
                }

                //setta l'oggetto munizioni per l'arma in uso con la nuova quantita' - se quantita'= 0 resetta lo slot inventario
                consoleCommands.setAmmosQuantityForFireGun(magazines);
            }
           


            reloading = false;
            canSwicthMode = true;
            isFiring = false;
        }

        if (ammoMode == Ammo.Bullets)
        {
            if (magazines > 0 && bulletsLeft != bulletsPerMag)
            {
                if (magazines > bulletsPerMag)
                {
                    canSwicthMode = false;
                    reloading = true;
                    weaponAnim[reloadAnim].speed = reloadAnimSpeed;
                    weaponAnim.Play(reloadAnim, PlayMode.StopAll);
                    //weaponAnim.CrossFade(reloadAnim);
                    aSource.PlayOneShot(soundReload, 0.7f);
                    yield return new WaitForSeconds(reloadTime);
                    magazines -= bulletsPerMag - bulletsLeft;
                    bulletsLeft = bulletsPerMag;
                    canSwicthMode = true;
                    reloading = false;
                    yield break;
                }
                else
                {
                    canSwicthMode = false;
                    reloading = true;
                    weaponAnim[reloadAnim].speed = reloadAnimSpeed;
                    weaponAnim.Play(reloadAnim, PlayMode.StopAll);
                    //weaponAnim.CrossFade(reloadAnim);
                    aSource.PlayOneShot(soundReload);
                    yield return new WaitForSeconds(reloadTime);
                    var bullet = Mathf.Clamp(bulletsPerMag, magazines, bulletsLeft + magazines);
                    magazines -= (bullet - bulletsLeft);
                    bulletsLeft = bullet;
                    canSwicthMode = true;
                    reloading = false;
                    yield break;
                }
            }
        }
    }

    public void DrawGun()
    {
        mainGunHud.GetComponent<HDK_UIFade>().FadeIn();
        StartCoroutine(DrawWeapon());
    }

    IEnumerator DrawWeapon()
    {
        draw = true;
        wepCamera.fieldOfView = weaponFOV;
        canSwicthMode = false;
        aSource.clip = soundDraw;
        aSource.Play();

        weaponAnim[drawAnim].speed = drawAnimSpeed;
        weaponAnim.Rewind(drawAnim);
        weaponAnim.Play(drawAnim, PlayMode.StopAll);
        yield return new WaitForSeconds(drawTime);

        draw = false;
        reloading = false;
        canSwicthMode = true;
        selected = true;

    }

    public void PutdownGun()
    {
        mainGunHud.GetComponent<HDK_UIFade>().FadeOut();
        StartCoroutine(Deselect());
    }

    IEnumerator Deselect()
    {
        putdown = true;
        aSource.clip = soundPutdown;
        aSource.Play();
        weaponAnim[putdownAnim].speed = putdownAnimSpeed;
        weaponAnim.Rewind(putdownAnim);
        weaponAnim.Play(putdownAnim, PlayMode.StopAll);
        wepCamera.fieldOfView = weaponFOV;
        canSwicthMode = false;

        yield return new WaitForSeconds(putdownTime);

        putdown = false;
        selected = false;
        mainCamera.fieldOfView = camFOV;
        transform.localPosition = hipPosition;        
    }

    public void AddMagazines(int mags)
    {
        magazines += mags;
    }
}