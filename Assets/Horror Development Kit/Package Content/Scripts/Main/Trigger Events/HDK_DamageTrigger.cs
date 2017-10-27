//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public enum DamageType
{
    DamageOnEnter,
    ConstantDamage,
    InstaKill
};

public class HDK_DamageTrigger : MonoBehaviour {

    public DamageType TypeOfDamage;
    HDK_PlayerHealth PlayerHealth;
    public float DamageOnEnterValue;
    public float ConstantDamageMultiplier;
    bool decreaseOverTime;

	void Start ()
    {
        PlayerHealth = GameObject.Find ("Player").GetComponent<HDK_PlayerHealth>();
	}

    void Update()
    {
        if (decreaseOverTime)
        {
            PlayerHealth.Health -= Time.deltaTime * ConstantDamageMultiplier;
        }
    }

    void OnTriggerEnter (Collider col)
    {		
		if (col.tag == "Player" && PlayerHealth != null)
        {
            switch (TypeOfDamage)
            {
                case DamageType.DamageOnEnter: PlayerHealth.FallingDamage(DamageOnEnterValue); decreaseOverTime = false;
                break;

                case DamageType.ConstantDamage: decreaseOverTime = true; 
                break;

                case DamageType.InstaKill: PlayerHealth.Health = 0f; decreaseOverTime = false;
                break;
            }
		}
	}

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && PlayerHealth != null)
        {
            decreaseOverTime = false;
        }
    }
}