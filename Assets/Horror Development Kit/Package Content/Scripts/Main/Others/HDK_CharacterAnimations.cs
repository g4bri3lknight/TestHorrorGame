//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

public class HDK_CharacterAnimations : MonoBehaviour {

	public float speed = 10.0F;
	public Animator[] BodyAnimators;
    public GameObject[] BodyObj;
    FirstPersonController controller;
    bool canJump;
    public float jumpReset;

    void Start()
    {
        foreach (GameObject obj in BodyObj)
        {
            obj.SetActive(true);
        }
        controller = GetComponent<FirstPersonController>();
        canJump = true;
    }
    
    void Update() {

        bool paused = HDK_PauseManager.GamePaused;
        bool examining = HDK_RaycastManager.ExaminingObject;
        bool security = HDK_RaycastManager.UsingSecurityCam;
        bool reading = HDK_RaycastManager.ReadingPaper;
        bool backwards = controller.Backwards;
        bool ladder = controller.m_onLadder;

        float translation = Input.GetAxis("Vertical") * speed;

        foreach (Animator anim in BodyAnimators)
        {
            if (!paused && !examining && !security && !reading && !ladder)
            {
                if (!backwards)
                {
                    if (translation != 0 && Input.GetKey(KeyCode.LeftShift))
                    {
                        anim.SetBool("IsRunning", true);
                    }
                    else
                    {
                        anim.SetBool("IsRunning", false);
                    }

                    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey("d"))
                    {
                        anim.SetBool("IsRunningRight", true);
                    }
                    else
                    {
                        anim.SetBool("IsRunningRight", false);
                    }

                    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey("a"))
                    {
                        anim.SetBool("IsRunningLeft", true);
                    }
                    else
                    {
                        anim.SetBool("IsRunningLeft", false);
                    }
                }              

                if (Input.GetKey("d"))
                {
                    anim.SetBool("IsWalkingRight", true);
                }
                else
                {
                    anim.SetBool("IsWalkingRight", false);
                }

                if (Input.GetKey("a"))
                {
                    anim.SetBool("IsWalkingLeft", true);
                }
                else
                {
                    anim.SetBool("IsWalkingLeft", false);
                }

                if (translation != 0)
                {
                    anim.SetBool("IsWalking", true);
                }
                else
                {
                    anim.SetBool("IsWalking", false);
                }

                if (Input.GetButtonDown("Jump") && canJump && controller.m_CharacterController.isGrounded)
                {
                    anim.SetBool("IsJumping", true);
                    canJump = false;
                    StartCoroutine(resetJump());                 
                }
                else
                {
                    anim.SetBool("IsJumping", false);
                }
            }
            else if (!paused || !examining || !security || !reading)
            {
                anim.SetBool("IsRunning", false);
                anim.SetBool("IsRunningRight", false);
                anim.SetBool("IsRunningLeft", false);
                anim.SetBool("IsWalkingRight", false);
                anim.SetBool("IsWalkingLeft", false);
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsJumping", false);
            }
        }

        if (ladder)
        {
            foreach (GameObject obj in BodyObj)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject obj in BodyObj)
            {
                obj.SetActive(true);
            }
        }
    }

    IEnumerator resetJump()
    {
        yield return new WaitForSeconds(jumpReset);
        canJump = true;
    }
}