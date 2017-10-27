//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(AudioSource))]
public class HDK_AnimatedJumpscare : MonoBehaviour {

    [Header("Animated jumpscare")]
    public GameObject AnimatedObject;				//Animated object to active during the jumpscare
    public string ObjectAnimation;					//The name of the animation attached to the "AnimatedObject"
    public string MainAnimation;					//The name of the animation attached to the main GameObject
    public AudioClip JumpscareSound;				//Jumpscare sound to play, it's not needed
    public MotionBlur CameraEffect;                 //Camera effect to enable during the jumpscare
    public float Time;                              //The jumpscare duration
    public bool deactivateColliderAfterCollision;   //Do you want to disable the Jumpscare after playing it one time?
    bool active;                                    //Is the Jumpscare actived?
    GameObject Player;
    public bool ShakeEffect;
    public float ShakeValue;

    void Start()
    {
        AnimatedObject.GetComponent<Animation>().Stop(ObjectAnimation);            //Stop the looping animation on the animated object
        Player = GameObject.Find("Player");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!active)
            {
                active = true;
                AnimatedObject.GetComponent<Animation>().Play(ObjectAnimation);       //Play the animation of the object
                GetComponent<Animation>().Play(MainAnimation);                        //Play the animation of the main object
                if (ShakeEffect)
                {
                    Player.SendMessage("Shake", ShakeValue);
                }
                if (JumpscareSound != null)
                {
                    GetComponent<AudioSource>().clip = JumpscareSound;                //Set the jumpscare sound
                    GetComponent<AudioSource>().Play();                               //Play the jumpscare sound
                }
                if (CameraEffect != null)
                {
                    CameraEffect.enabled = true;                                      //Enable the camera effect
                }
                StartCoroutine(ScaredWait());                                         //Calling the final coroutine
            }

            if (deactivateColliderAfterCollision)
            {
                gameObject.GetComponent<Collider>().enabled = false;
            }
        }
    }

    IEnumerator ScaredWait()
    {
        yield return new WaitForSeconds(Time);
        if (CameraEffect)
        {
            CameraEffect.enabled = false;                                               //Disabling the camera effect
        }
        active = false;
    }
}