//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(AudioSource))]
public class HDK_Jumpscare2D : MonoBehaviour {

	[Header ("Jumpscare 2D")]
    public Sprite jumpScareSprite;                          //GUI Sprite you want to use as jumpscare
    public Sprite nullSprite;                               //A null sprite to show when the jumpscare is off
    public Vector2 spritePosition;                          //Position of the sprite
    public Vector2 spriteScale = new Vector3(1.0f, 1.0f);   //Scale of the sprite
    public Color spriteColor = Color.white;                 //Color of the sprite
    public AudioClip JumpscareSound;                        //Jumpscare sound to play, it's not needed
    public bool deactivateColliderAfterCollision;           //Do you want that it works just one time?
    public float showFor;                                   //For how long the jumpscare sprite should be active?
    bool active;                                            //Is the Jumpscare actived?
    private GameObject jumpScareObj;
    private Image jumpScareSpriteRenderer;
    GameObject Player;
    public bool ShakeEffect;
    public float ShakeValue;

    void Start()
    {
        Player = GameObject.Find("Player");
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (deactivateColliderAfterCollision)
            {
                gameObject.GetComponent<Collider>().enabled = false;
            }

            jumpScareObj = GameObject.Find("Jumpscare_sprite");
            jumpScareSpriteRenderer = jumpScareObj.GetComponent<Image>();

            if (!active)
            {
                jumpScareObj.GetComponent<Transform>().localPosition = new Vector3(spritePosition.x, spritePosition.y, 0.3f);
                jumpScareObj.GetComponent<Transform>().localScale = new Vector3(spriteScale.x, spriteScale.y, 1.0f);
                jumpScareSpriteRenderer.sprite = jumpScareSprite;
                jumpScareSpriteRenderer.color = spriteColor;
                active = true;
                if (ShakeEffect)
                {
                    Player.SendMessage("Shake", ShakeValue);
                }
                StartCoroutine("HideSprite");
                if (JumpscareSound != null)
                {
                    gameObject.GetComponent<AudioSource>().PlayOneShot(JumpscareSound);
                }
            }
        }
    }

    IEnumerator HideSprite()
    {
        yield return new WaitForSeconds(showFor);
        active = false;
        jumpScareSpriteRenderer.sprite = nullSprite;
    }
}