//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class HDK_SoundJumpscare : MonoBehaviour {

	[Header ("Sound Jumpscare")]
    private AudioSource audio_source;               //Audio source where the sound will be played
    public AudioClip JumpscareSound;                //Jumpscare sound to play
    public float JumpscareVolume;                   //jumpscare sound's volume
    public bool deactivateColliderAfterCollision;   //Do you want that it works just one time?
    float audio_lenght;                             //The lenght of the jumpscare sound
    bool active;                                    //Is the Jumpscare actived?
    GameObject Player;
    public bool ShakeEffect;
    public float ShakeValue;

    void Start()
    {
        audio_lenght = JumpscareSound.length;
        audio_source = this.GetComponent<AudioSource>();
        Player = GameObject.Find("Player");
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Player")
        {

            if (deactivateColliderAfterCollision)
            {
                gameObject.GetComponent<Collider>().enabled = false;
            }

            if (!active)
            {
                if (ShakeEffect)
                {
                    Player.SendMessage("Shake", ShakeValue);
                }
                audio_source.PlayOneShot(JumpscareSound, JumpscareVolume);
                active = true;
                StartCoroutine(DisableActived());
            }
        }
    }

    IEnumerator DisableActived()
    {
        yield return new WaitForSeconds(audio_lenght);
        active = false;
    }
}
