//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_MeleeZoneTrigger : MonoBehaviour 
{
	void OnTriggerEnter( Collider col )
	{
        HDK_AIStateMachine machine = GameSceneManager.instance.GetAIStateMachine( col.GetInstanceID() );
		if (machine)
		{
			machine.inMeleeRange = true;
		}
	}

	void OnTriggerExit( Collider col)
	{
        HDK_AIStateMachine machine = GameSceneManager.instance.GetAIStateMachine( col.GetInstanceID() );
		if (machine)
		{
			machine.inMeleeRange = false;
		}
	}
}