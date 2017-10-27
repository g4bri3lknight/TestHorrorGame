using UnityEngine;
using System.Collections;

// ----------------------------------------------------------------------
// Class	:	AIStateMachineLink
// Desc		:	Should be used as the base class for any
//				StateMachineBehaviour that needs to communicate with
//				its AI State Machine;
// ----------------------------------------------------------------------
public class HDK_AIStateMachineLink : StateMachineBehaviour 
{
	// The AI State Machine reference
	protected HDK_AIStateMachine _stateMachine;
	public HDK_AIStateMachine stateMachine { set{ _stateMachine = value;}}
}
