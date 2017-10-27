//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class HDK_MeleeWeapon : MonoBehaviour
{

    [Header("Melee settings")]
    public string weaponName = "";
    public float meleeRange = 1.0f;
    public int damage = 60;
    public float force = 150f;
    public float cooldown = 0.1f;
    public GameObject hitPosition;
    public LayerMask layersToAffect;

    [Header("Hit particles")]
    public GameObject Default;
    public GameObject Metal;
    public GameObject Concrete;
    public GameObject Dirt;
    public GameObject Wood;
    public GameObject Blood;

    [Header("Animations")]
    public Animation weaponAnim;
    public string attackAnim = "";
    public string secondaryAttackAnim = "";
    public string putDownAnim = "";
    public string drawAnim = "";
    [HideInInspector]
    public float putDownLenght;

    [Header("Sounds")]
    public AudioClip[] meleeSounds;
    public AudioClip PutDownSound;
    public AudioClip DrawSound;
    public float soundsVolume;

    bool bodyHit;
    private float coolStuff;
    private float coolit;
    [HideInInspector]
    public bool attacking = false;
    Camera weaponCam;
    HDK_Hiding playerHiding;

    void Start()
    {
        GetComponent<AudioSource>().volume = soundsVolume;
        coolit = cooldown + 0.1f;
        putDownLenght = weaponAnim.GetClip(putDownAnim).length;
        weaponCam = GameObject.Find("WeaponsCamera").GetComponent<Camera>();
        playerHiding = GameObject.Find("Player").GetComponent<HDK_Hiding>();
    }

    void Update()
    {
        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool paused = HDK_PauseManager.GamePaused;
        bool inventory = HDK_InventoryManager.inventoryOpen;

        if (!reading && !examining && !security && !paused && !inventory && !playerHiding.Hiding)
        {
            //IF YOU WANT TO MAKE A DOUBLE ATTACK EFFECT WHEN YOU PRESS TWO DIFFERENT BUTTONS
            //WARNING: YOU NEED TO MODIFY ALL OTHER RELATED BUTTONS TO AVOID BUGS
            /*    
            if (Input.GetKeyDown(KeyCode.Mouse0) && !attacking && attackAnim != "" && !GetComponent<Animation>().isPlaying)
            {
                coolStuff = 0;
                StartCoroutine(Attack());
            }

            if (Input.GetKeyDown(KeyCode.Mouse1) && !attacking && secondaryAttackAnim != "" && !GetComponent<Animation>().isPlaying)
            {
                coolStuff = 0;
                StartCoroutine(SecondaryAttack());
            }
            */

            if (Input.GetButtonDown("WeaponShoot") && !attacking && secondaryAttackAnim != "" && !weaponAnim.isPlaying)
            {
                coolStuff = 0;
                RandomAttack();
            }
        }
		
        coolit = Mathf.Clamp01(coolit);
        coolStuff += Time.deltaTime;
		
        if (!attacking)
        {
            coolStuff = cooldown + 0.1f;
        }
		
        coolit = coolStuff / (cooldown + 0.1f);
    }

    void RandomAttack()
    {
        int attackType = Random.Range(0, 2);
        if (attackType == 0)
        {
            StartCoroutine(Attack());
        }
        else if (attackType == 1)
        {
            StartCoroutine(SecondaryAttack());
        }
    }

    IEnumerator Attack()
    {
        attacking = true;
        weaponAnim.CrossFade(attackAnim, 0.1f);
        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.05f);
        GetComponent<AudioSource>().clip = meleeSounds[Random.Range(0, meleeSounds.Length)];        
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(MeleeHit());
    }

    IEnumerator SecondaryAttack()
    {
        attacking = true;
        weaponAnim.CrossFade(secondaryAttackAnim, 0.1f);
        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.05f);
        GetComponent<AudioSource>().clip = meleeSounds[Random.Range(0, meleeSounds.Length)];
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(MeleeHit());
    }

    IEnumerator MeleeHit()
    {
        RaycastHit hit;
        Ray ray;
        ray = weaponCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(hitPosition.transform.position, hitPosition.transform.forward, out hit, meleeRange, layersToAffect.value))
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			
            if (hit.rigidbody)
            {
                hit.rigidbody.AddForceAtPosition(force * transform.forward, hit.point);
            }

            //  If you want to add an hit effect just uncomment the lines below
            //  Change the tag and hit effect as you need

            //	if(hit.transform.tag == "YOUR TAG HERE") {
            //		Instantiate(YOUR HIT EFFECT HERE, hit.point + (hit.normal * 0.1f), rotation);
            //	}

            if (hit.collider.gameObject.GetComponent<HDK_DoorBarricade>())
            {
                if (hit.collider.gameObject.GetComponent<HDK_DoorBarricade>().health > 0)
                {
                    hit.collider.SendMessage("ApplyDamage", damage);
                    SendMessageUpwards("HitMarker");
                    Instantiate(Wood, hit.point + (hit.normal * 0.1f), rotation);
                }
            }
            if (hit.collider.gameObject.GetComponentInParent<HDK_AIZombieStateMachine>())
            {
                hit.collider.gameObject.GetComponentInParent<HDK_AIZombieStateMachine>().TakeDamage(hit.point, ray.direction * 5.0f, damage, hit.rigidbody, 0);
                Instantiate(Blood, hit.point + (hit.normal * 0.1f), rotation);
                SendMessageUpwards("HitMarker");
            }
            if (hit.transform.tag == "Untagged")
            {
                Instantiate(Default, hit.point + (hit.normal * 0.1f), rotation);
            }

            if (hit.transform.tag == "Dirt")
            {
                Instantiate(Dirt, hit.point + (hit.normal * 0.1f), rotation);
            }

            if (hit.transform.tag == "Metal")
            {
                Instantiate(Metal, hit.point + (hit.normal * 0.1f), rotation);
            }

            if (hit.transform.tag == "Concrete")
            {
                Instantiate(Concrete, hit.point + (hit.normal * 0.1f), rotation);
            }

            if (hit.transform.tag == "Wood")
            {
                Instantiate(Wood, hit.point + (hit.normal * 0.1f), rotation);
            }

            if (hit.transform.tag == "Enemy")
            {
                Instantiate(Blood, hit.point + (hit.normal * 0.1f), rotation);
                hit.collider.SendMessageUpwards("GetDamage", damage, SendMessageOptions.DontRequireReceiver);
                hit.collider.SendMessageUpwards("GetHit", SendMessageOptions.DontRequireReceiver);
                SendMessageUpwards("HitMarker");
            }
            //MODIFICA
            if (hit.transform.tag == "Breakable")
            {
                //??CHE FACCIO LASCIO??
                Instantiate(Wood, hit.point + (hit.normal * 0.1f), rotation);
                hit.collider.SendMessageUpwards("DestroyIt", SendMessageOptions.DontRequireReceiver);
            }
        }		

        yield return new WaitForSeconds(cooldown);
        attacking = false;
    }
}