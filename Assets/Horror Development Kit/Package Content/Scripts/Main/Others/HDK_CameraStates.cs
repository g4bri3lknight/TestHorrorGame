//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class HDK_CameraStates : MonoBehaviour {

	FirstPersonController Player;
	public HeadBobController _HeadBobController;
    bool busy;

	void Start ()
	{
		Player = GameObject.Find ("Player").GetComponent<FirstPersonController>();
	}

	void Update ()
	{
        if (Player.enabled)
        {
            //Idle
            if (Player.m_CharacterController.velocity.sqrMagnitude == 0)
                _HeadBobController._PlayerState = HeadBobController.PlayerState.Idle;

            //Walk
            if (!Player.isRunning && Player.m_CharacterController.velocity.sqrMagnitude > 0)
                _HeadBobController._PlayerState = HeadBobController.PlayerState.Walking;

            //Walk
            if (Player.isRunning && !Player.CanRun)
                _HeadBobController._PlayerState = HeadBobController.PlayerState.Walking;

            //Run
            if (Player.isRunning && Player.m_CanRun && Player.m_CharacterController.velocity.sqrMagnitude > 0)
                _HeadBobController._PlayerState = HeadBobController.PlayerState.Running;
        }
        else
        {   
            //Idle while using inventory
            if (HDK_InventoryManager.inventoryOpen || busy)
            {
                _HeadBobController._PlayerState = HeadBobController.PlayerState.Idle;
            }
        }
	}

    public void Busy(bool value)
    {
        busy = value;
    }
}