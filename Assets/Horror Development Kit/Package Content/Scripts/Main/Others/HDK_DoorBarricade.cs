//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDK_DoorBarricade : MonoBehaviour
{

    [Header("Barricade Settings")]
    public int health;
    public GameObject jammingDoor;

    [Header("Destruction Settings")]
    public bool destroyAfterTime;
    public float destroyTimer;

    [Header("Sounds")]
    public AudioClip destroySound;
    public AudioClip[] impactSounds;
    public float impactAudioModifier = 1.0f;
    AudioSource aSource;

    private void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            StartCoroutine(DestroyBarricade());
        }
    }

    IEnumerator DestroyBarricade()
    {
        if (jammingDoor.GetComponent<HDK_NormalDoor>())
        {
            jammingDoor.GetComponent<HDK_NormalDoor>().barricadesValue -= 1;
        }
        else if (jammingDoor.GetComponent<HDK_DraggableDoor>())
        {
            jammingDoor.GetComponent<HDK_DraggableDoor>().barricadesValue -= 1;
        }
        aSource.clip = destroySound;
        aSource.Play();
        GetComponent<Rigidbody>().isKinematic = false;
        if (destroyAfterTime)
        {
            yield return new WaitForSeconds(destroyTimer);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        int randomImpact = Random.Range(0, impactSounds.Length);
        aSource.PlayOneShot(impactSounds[randomImpact], col.relativeVelocity.magnitude * 0.1f * impactAudioModifier);
    }
}