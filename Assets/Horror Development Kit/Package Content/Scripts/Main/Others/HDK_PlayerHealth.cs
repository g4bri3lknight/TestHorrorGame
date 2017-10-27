//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class HDK_PlayerHealth : MonoBehaviour {

	[Header ("Health Settings")]
	public float Health = 100;
	public float maxHealth = 100;
	public bool Regeneration;
	public float RegenerationSpeed;
    float fillValue;

    [Header ("Falling Damage")]
    public AudioSource Falling_audioSource;
    public float FallingDamageMultiplier;
    float vol;

    [Header("Heart SFX")]
    public AudioClip HeartSound;
    public AudioSource Heart_audioSource;

    [Header("Hurt SFX")]
    public AudioClip[] HurtsSounds;
    public AudioSource Hurts_audioSource;
    public float Hurts_volume;

    [Header ("Dead Replacement")]
	public GameObject deadReplacement;

    Animator HUD_Health;
    GameObject HUDicon;
    CharacterController controller;
    PostProcessingProfile m_Profile;
    float vignetteEffect;

    void Start () 
	{
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
		HUDicon = GameObject.Find ("Circle");
		HUD_Health = GameObject.Find("Heart").GetComponent<Animator>();
        Heart_audioSource.clip = HeartSound;
        Heart_audioSource.Play();
        m_Profile = GetComponentInChildren<PostProcessingBehaviour>().profile;
    }

    void Update () 
	{
        if (Health == 100)
        {
            vignetteEffect = 0;
            HUD_Health.speed = 1f;
        }
        else if(Health > 70 && Health < 90)
        {
            vignetteEffect = 0.2f;
            HUD_Health.speed = 1.5f;
        }
        else if (Health > 50 && Health < 70)
        {
            vignetteEffect = 0.3f;
            HUD_Health.speed = 2f;
        }
        else if (Health > 25 && Health < 50)
        {
            vignetteEffect = 0.4f;
            HUD_Health.speed = 2.5f;
        }
        else if (Health > 0 && Health < 25)
        {
            vignetteEffect = 0.5f;
            HUD_Health.speed = 3f;
        }

        if (Regeneration)
        {
			Health += Time.deltaTime * RegenerationSpeed;
		}

        var vignette = m_Profile.vignette.settings;
        vignette.smoothness = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup) * vignetteEffect) + 0.3f;
        m_Profile.vignette.settings = vignette;
           
        float difference = maxHealth - Health;              
        Heart_audioSource.volume = difference / 100f;  

		if (Health >= maxHealth) {
			Health = maxHealth;
		} else if (Health <= 0) {
			Health = 0;
			Die();
		}

        fillValue = Health / 100f;
		HUDicon.GetComponent<Image> ().fillAmount = fillValue;

        //Falling sound effect
        float y = -controller.velocity.y;

        if (y > 5)
        {
            vol = Mathf.Lerp(vol, 1.0f, Time.deltaTime / 2);
        }
        else
        {
            vol = Mathf.Lerp(vol, 0.0f, Time.deltaTime * 3);
        }

        Falling_audioSource.volume = vol;
        Falling_audioSource.pitch = 1 + vol;
        //
    }

	void Die()
	{
		Instantiate(deadReplacement, transform.position, transform.rotation);
		Destroy(gameObject);
		Destroy(GameObject.Find("Canvas"));
	}

    public void TakeDamage(float amount)
    {
        Health = Mathf.Max(Health - (amount * Time.deltaTime), 0.0f);
        fillValue = (Health / 100.0f);
        Hurts_audioSource.clip = HurtsSounds[Random.Range(0, HurtsSounds.Length)];
        Hurts_audioSource.volume = Hurts_volume;
        Hurts_audioSource.Play();
        if(Health <= 0)
        {
            Die();
        }
    }

    public void FallingDamage(float amount)
    {
        Health -= amount;
        Hurts_audioSource.clip = HurtsSounds[Random.Range(0, HurtsSounds.Length)];
        Hurts_audioSource.volume = Hurts_volume;
        Hurts_audioSource.Play();
    }
}