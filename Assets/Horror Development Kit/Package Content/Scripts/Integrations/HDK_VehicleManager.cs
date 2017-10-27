//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

//THIS SCRIPT IS LINKED TO THE ASSET CALLED "Offroad Pickup + Animated Hands" CREATED BY "torvald-mgt"
//IS NEEDED TO INTEGRATE THAT ASSET WITH THIS ONE (HORROR DEVELOPMENT KIT)
//IF YOU WANT TO INTEGRATE IT YOU NEED TO BUY BOTH ASSETS AND FOLLOW THE DOCUMENTATION
//"Offroad Pickup + Animated Hands" -> http://u3d.as/h6X
//"Horror Development Kit" -> http://u3d.as/wgE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDK_VehicleManager : MonoBehaviour {
    
    public GameObject[] UsedGameObjects;
    public MonoBehaviour[] UsedScripts;
    public Transform spawnPlayer;
    public bool onCar = false;
    public bool OnTrigger;
    GameObject Player;
    GameObject Canvas;                             
    
	void Start ()
    {
        Player = GameObject.Find("Player");
        Canvas = GameObject.Find("Canvas");

        foreach (GameObject gameObj in UsedGameObjects)
        {
            gameObj.SetActive(false);
        }
    }    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            OnTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            OnTrigger = false;
        }
    }

    void Update()
    {
        if (OnTrigger)
        {
            if (!onCar)
            {
                if (Input.GetKeyUp(KeyCode.E))
                {
                    foreach (GameObject gameObj in UsedGameObjects)
                    {
                        gameObj.SetActive(true);
                    }
                    foreach (MonoBehaviour scripts in UsedScripts)
                    {
                        scripts.enabled = true;
                    }
                    Player.SetActive(false);
                    Canvas.SetActive(false);
                    OnTrigger = false;
                    GetComponent<BoxCollider>().enabled = false;
                    onCar = true;
                }
            }
        }
        else
        {
            if (onCar)
            {
                if (Input.GetKeyUp(KeyCode.E))
                {
                    foreach (GameObject gameObj in UsedGameObjects)
                    {
                        gameObj.SetActive(false);
                    }
                    foreach (MonoBehaviour scripts in UsedScripts)
                    {
                        scripts.enabled = false;
                    }
                    Player.SetActive(true);
                    Canvas.SetActive(true);
                    GetComponent<BoxCollider>().enabled = true;
                    Player.transform.position = spawnPlayer.position;
                    onCar = false;
                }
            }
        }       
    }
}