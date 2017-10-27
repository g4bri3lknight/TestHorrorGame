//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------
// Class	:	AISensor
// Desc		:	Notifies the parent AIStateMachine of any threats that
//				enter its trigger via the AIStateMachine's OnTriggerEvent
//				method.
// ----------------------------------------------------------------------
public class HDK_AISensor : MonoBehaviour 
{
	// Private
	private HDK_AIStateMachine _parentStateMachine	=	null;
	public HDK_AIStateMachine parentStateMachine { set{ _parentStateMachine = value; }}

	void OnTriggerEnter( Collider col )
	{
		if (_parentStateMachine!=null)
			_parentStateMachine.OnTriggerEvent ( AITriggerEventType.Enter,col );
	}

	void OnTriggerStay( Collider col )
	{
		if (_parentStateMachine!=null)
			_parentStateMachine.OnTriggerEvent ( AITriggerEventType.Stay, col );
	}

	void OnTriggerExit( Collider col )
	{
		if (_parentStateMachine!=null)
			_parentStateMachine.OnTriggerEvent ( AITriggerEventType.Exit,  col );
	}
}