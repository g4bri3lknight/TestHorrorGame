//Script written by Inan Evin
//Support: giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HeadBobController : MonoBehaviour {
	
	
	public enum SelectSettings {WalkSettings, RunSettings, JumpSettings};		// Choose which settings to edit from the inspector.
	public SelectSettings _SelectSettings;							
	
	public enum PlayerState {Idle, Walking, Running, OnJump};					// Temporary PlayerStates.
	public PlayerState _PlayerState;

	public enum WalkBobStyle {OnlyPosition, OnlyRotation, Both};				// Choose what to affect on the target transform while walking.
	public WalkBobStyle _WalkBobStyle;

	public enum RunBobStyle {OnlyPosition, OnlyRotation, Both};					// Choose what to affect on the target transform while running.
	public RunBobStyle _RunBobStyle;
	
	public Vector3 v3_WALK_POS_Amounts;											// Intensities of position interpolation while walking.
	public Vector3 v3_WALK_POS_Speeds;											// Speeds of position interpolation while walking.

	public float f_WALK_POS_Smooth;												// How fast will the position be interpolated while walking?

	public Vector3 v3_WALK_EUL_Amounts;											// Intensities of rotation interpolation while walking.
	public Vector3 v3_WALK_EUL_Speeds;											// Speeds of rotation interpolation while walking.

	public float f_WALK_EUL_Smooth;												// How fast will the rotation be interpolated while walking?

	
	public Vector3 v3_RUN_POS_Amounts;											// Intensities of position interpolation while running.
	public Vector3 v3_RUN_POS_Speeds;											// Speeds of position interpolation while running.

	public float f_RUN_POS_Smooth;												// How fast will the position be interpolated while running?
	
	public Vector3 v3_RUN_EUL_Amounts;											// Intensities of rotation interpolation while running.
	public Vector3 v3_RUN_EUL_Speeds;											// Speeds of rotation interpolation while running.

	public float f_RUN_EUL_Smooth;												// How fast will the rotation be interpolated while running?

	public float f_ResetSpeed;													// How fast will the target transform reset to it's original position and/or rotation when no movement is present.
	
	public Transform t_Target;													// Target transform to affect.
	
	
	public Vector3 v3_JUMPSTART_MaximumIncrease;								// What is the upper limit of the target transform while interpolating it along the X rotation axis? (When player jumps)
	public float f_JUMPSTART_InterpolationSpeed;								// What is the speed of the above interpolation?
	
	public Vector3 v3_JUMPEND_MaximumDecrease;									// What is the lower limit of the target transform while interpolating it along the X rotation axis? (When player hits back to the ground)
	public float f_JUMPEND_InterpolationSpeed;									// What is the speed of the above interpolation?
	public float f_JUMPEND_ResetSpeed;											// How fast will the target transform rotate to it's original position rotation after going down when the player hits back to the ground.
	
	private bool b_JumpCheck = false;											// Flag for jump.
	private bool b_StateChanged = false;										// Flag for state change.
	private bool b_PositionAffected;											// Controls whether to interpolate the position or not while resetting.
	private bool b_RotationAffected;											// Controls whether to interpolate the rotation or not while resetting.
	
	private Coroutine co_JumpStart;
	private Coroutine co_JumpEnd;
	private Coroutine co_Reset;

	private Vector3 v3_InitialTargetEUL;										// Initial target rotation.
	private Vector3 v3_InitialTargetPOS;										// Initial target position.
	
	void Start()
	{
		// Store initial rotation and position of the target transform.
		v3_InitialTargetEUL = t_Target.localEulerAngles;
		v3_InitialTargetPOS = t_Target.localPosition;

		// Set the initial state to Idle.
		_PlayerState = PlayerState.Idle;

		// Set the position & rotation controller booleans according to the user choice of affection for interpolation on the target transform.
		if(_WalkBobStyle == WalkBobStyle.OnlyPosition || _WalkBobStyle == WalkBobStyle.Both || _RunBobStyle == RunBobStyle.OnlyPosition || _RunBobStyle == RunBobStyle.Both)
			b_PositionAffected = true;

		if(_WalkBobStyle == WalkBobStyle.OnlyRotation || _WalkBobStyle == WalkBobStyle.Both || _RunBobStyle == RunBobStyle.OnlyRotation || _RunBobStyle == RunBobStyle.Both)
			b_RotationAffected = true;
	}
	
	void Update()
	{
		// If we are walking and not on air.
		if(_PlayerState == PlayerState.Walking && !b_JumpCheck)
		{
			// If reset coroutine is present, stop it, run for one frame only.
			if(b_StateChanged)
			{
				b_StateChanged = false;

				if(co_Reset != null)
					StopCoroutine(co_Reset);
			}

			WalkBob();
		}
			
		else if(_PlayerState == PlayerState.Running && !b_JumpCheck)
		{
			// If reset coroutine is present, stop it, run for one frame only.
			if(b_StateChanged)
			{
				b_StateChanged = false;
				
				if(co_Reset != null)
					StopCoroutine(co_Reset);
			}

			RunBob();
		}
		else if(_PlayerState == PlayerState.Idle && !b_StateChanged)
		{
			// Reset the necessary parts for target transform if state is changed to idling.
			b_StateChanged = true;
			
			if(co_Reset != null)
				StopCoroutine(co_Reset);
			
			co_Reset = StartCoroutine(IEReset());
		}
	}

	void WalkBob()
	{
		if(_WalkBobStyle == WalkBobStyle.OnlyPosition)
		{
			// Declare & initialize the inner components of the target vector. 
			float posTargetX = Mathf.Sin (Time.time * v3_WALK_POS_Speeds.x) * v3_WALK_POS_Amounts.x;
			float posTargetY = Mathf.Sin (Time.time * v3_WALK_POS_Speeds.y) * v3_WALK_POS_Amounts.y;
			float posTargetZ = Mathf.Sin (Time.time * v3_WALK_POS_Speeds.z) * v3_WALK_POS_Amounts.z;

			// Declare & initialize the target vector.
			Vector3 posTarget = new Vector3(posTargetX, posTargetY, posTargetZ);

			// Apply the target vector using linear interpolation.
			t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_WALK_POS_Smooth);
		}
		else if(_WalkBobStyle == WalkBobStyle.OnlyRotation)
		{
			float eulTargetX = Mathf.Sin (Time.time * v3_WALK_EUL_Speeds.x) * v3_WALK_EUL_Amounts.x;
			float eulTargetY = Mathf.Sin (Time.time * v3_WALK_EUL_Speeds.y) * v3_WALK_EUL_Amounts.y;
			float eulTargetZ = Mathf.Sin (Time.time * v3_WALK_EUL_Speeds.z) * v3_WALK_EUL_Amounts.z;
			Vector3 eulTarget = new Vector3(eulTargetX, eulTargetY, eulTargetZ);

			t_Target.localRotation = Quaternion.Lerp (t_Target.localRotation, Quaternion.Euler(eulTarget), Time.deltaTime * f_WALK_EUL_Smooth);

		}
		else
		{
			float posTargetX = Mathf.Sin (Time.time * v3_WALK_POS_Speeds.x) * v3_WALK_POS_Amounts.x;
			float posTargetY = Mathf.Sin (Time.time * v3_WALK_POS_Speeds.y) * v3_WALK_POS_Amounts.y;
			float posTargetZ = Mathf.Sin (Time.time * v3_WALK_POS_Speeds.z) * v3_WALK_POS_Amounts.z;
			Vector3 posTarget = new Vector3(posTargetX, posTargetY, posTargetZ);

			float eulTargetX = Mathf.Sin (Time.time * v3_WALK_EUL_Speeds.x) * v3_WALK_EUL_Amounts.x;
			float eulTargetY = Mathf.Sin (Time.time * v3_WALK_EUL_Speeds.y) * v3_WALK_EUL_Amounts.y;
			float eulTargetZ = Mathf.Sin (Time.time * v3_WALK_EUL_Speeds.z) * v3_WALK_EUL_Amounts.z;
			Vector3 eulTarget = new Vector3(eulTargetX, eulTargetY, eulTargetZ);

			t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_WALK_POS_Smooth);
			t_Target.localRotation = Quaternion.Lerp (t_Target.localRotation, Quaternion.Euler(eulTarget), Time.deltaTime * f_WALK_EUL_Smooth);
		}
	}

	void RunBob()
	{
		if(_RunBobStyle == RunBobStyle.OnlyPosition)
		{
			// Declare & initialize the inner components of the target vector. 
			float posTargetX = Mathf.Sin (Time.time * v3_RUN_POS_Speeds.x) * v3_RUN_POS_Amounts.x;
			float posTargetY = Mathf.Sin (Time.time * v3_RUN_POS_Speeds.y) * v3_RUN_POS_Amounts.y;
			float posTargetZ = Mathf.Sin (Time.time * v3_RUN_POS_Speeds.z) * v3_RUN_POS_Amounts.z;

			// Declare & initialize the target vector.
			Vector3 posTarget = new Vector3(posTargetX, posTargetY, posTargetZ);

			// Apply the target vector using linear interpolation.
			t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_RUN_POS_Smooth);
		}
		else if(_RunBobStyle == RunBobStyle.OnlyRotation)
		{
			float eulTargetX = Mathf.Sin (Time.time * v3_RUN_EUL_Speeds.x) * v3_RUN_EUL_Amounts.x;
			float eulTargetY = Mathf.Sin (Time.time * v3_RUN_EUL_Speeds.y) * v3_RUN_EUL_Amounts.y;
			float eulTargetZ = Mathf.Sin (Time.time * v3_RUN_EUL_Speeds.z) * v3_RUN_EUL_Amounts.z;
			Vector3 eulTarget = new Vector3(eulTargetX, eulTargetY, eulTargetZ);

			t_Target.localRotation = Quaternion.Lerp (t_Target.localRotation, Quaternion.Euler(eulTarget), Time.deltaTime * f_RUN_EUL_Smooth);
		}
		else
		{
			float posTargetX = Mathf.Sin (Time.time * v3_RUN_POS_Speeds.x) * v3_RUN_POS_Amounts.x;
			float posTargetY = Mathf.Sin (Time.time * v3_RUN_POS_Speeds.y) * v3_RUN_POS_Amounts.y;
			float posTargetZ = Mathf.Sin (Time.time * v3_RUN_POS_Speeds.z) * v3_RUN_POS_Amounts.z;
			Vector3 posTarget = new Vector3(posTargetX, posTargetY, posTargetZ);

			float eulTargetX = Mathf.Sin (Time.time * v3_RUN_EUL_Speeds.x) * v3_RUN_EUL_Amounts.x;
			float eulTargetY = Mathf.Sin (Time.time * v3_RUN_EUL_Speeds.y) * v3_RUN_EUL_Amounts.y;
			float eulTargetZ = Mathf.Sin (Time.time * v3_RUN_EUL_Speeds.z) * v3_RUN_EUL_Amounts.z;
			Vector3 eulTarget = new Vector3(eulTargetX, eulTargetY, eulTargetZ);

			t_Target.localPosition = Vector3.Lerp(t_Target.localPosition, posTarget, Time.deltaTime * f_RUN_POS_Smooth);
			t_Target.localRotation = Quaternion.Lerp (t_Target.localRotation, Quaternion.Euler(eulTarget), Time.deltaTime * f_RUN_EUL_Smooth);
		}
	}

	
	public void JumpStarted()
	{
		// If jump end coroutine is present, stop it then start jump start coroutine.
		if(co_JumpEnd != null)
			StopCoroutine(co_JumpEnd);
		
		co_JumpStart = StartCoroutine(IEJumpStart());
	}
	
	public void JumpEnded()
	{
		// If jump start coroutine is present, stop it then start jump end coroutine.
		if(co_JumpStart != null)
			StopCoroutine(co_JumpStart);
		
		co_JumpEnd = StartCoroutine(IEJumpEnd());
	}
	
	IEnumerator IEJumpStart()
	{
		// Set the bool to true so that walk or run movements won't be applied.
		b_JumpCheck = true;
		
		float i = 0.0f;
		float rate = f_JUMPSTART_InterpolationSpeed;
		// Create the target vector, apply maximum increase amounts for it according to the current rotation of the target transform.
		Vector3 targetEUL = new Vector3(t_Target.localEulerAngles.x - v3_JUMPSTART_MaximumIncrease.x, t_Target.localEulerAngles.y - v3_JUMPSTART_MaximumIncrease.y, t_Target.localEulerAngles.z - v3_JUMPSTART_MaximumIncrease.z);
		// Store the current rotation to use inside the loop, so that it would be used as a starting point.
		Quaternion currentQ = t_Target.localRotation;
		// Transform the targe vector into a quaternion.
		Quaternion targetQ = Quaternion.Euler(targetEUL);
		
		while(i < 1.0f)	// Run the coroutine for a desired time which is altered by the speed given.
		{
			i += Time.deltaTime * rate;
			t_Target.localRotation = Quaternion.Lerp (currentQ, targetQ, i);
			yield return null;
		}
	}
	
	IEnumerator IEJumpEnd()
	{
		float i = 0.0f;
		float rate = f_JUMPEND_InterpolationSpeed;
		// Create the target vector, opposite to the one one that was created on jump start, apply maximum increase amounts for it according to the current rotation of the target transform.
		Vector3 targetEUL = new Vector3(v3_InitialTargetEUL.x + v3_JUMPEND_MaximumDecrease.x, v3_InitialTargetEUL.y + v3_JUMPEND_MaximumDecrease.y, v3_InitialTargetEUL.z + v3_JUMPEND_MaximumDecrease.z);
		// Store the current rotation to use inside the loop, so that it would be used as a starting point.
		Quaternion currentQ = t_Target.localRotation;
		// Transform the targe vector into a quaternion.
		Quaternion targetQ = Quaternion.Euler(targetEUL);
		
		while(i < 1.0f)	// Run the coroutine for a desired time which is altered by the speed given.
		{
			i += Time.deltaTime * rate;
			t_Target.rotation = Quaternion.Lerp (currentQ, targetQ, i);
			yield return null;
		}

		// Reset the i & rate values according to the reset speed. Now the camera was rotated down on the previous loop, it will be rotated back on the loop below.
		i = 0;
		rate = f_JUMPEND_ResetSpeed;
		currentQ = t_Target.localRotation;
		targetQ = Quaternion.Euler(v3_InitialTargetEUL);
		
		while(i < 1.0f)	// Run the coroutine for a desired time which is altered by the speed given.
		{
			i += Time.deltaTime * rate;
			t_Target.localRotation = Quaternion.Lerp (currentQ, targetQ, i);
			yield return null;
		}

		// Set the boolean to false so that walk & run movements can be applied.
		b_JumpCheck = false;
	}

	IEnumerator IEReset()
	{
		float i = 0.0f;
		float rate = f_ResetSpeed;

		// Store the current rotation to use inside the loop as an initial point for linear interpolation.
		Quaternion currentQ = t_Target.localRotation;
		// Store the current position to use inside the loop as an initial point for linear interpolation.
		Vector3 currentPos = t_Target.localPosition;

		while(i < 1.0f)	// Run the coroutine for a desired time which is altered by the speed given.
		{
			// Apply position resetting if the position was altered, and rotation resetting if the rotation was altered.
			i += Time.deltaTime * rate;
			if(b_RotationAffected)
				t_Target.localRotation = Quaternion.Lerp (currentQ, Quaternion.Euler(v3_InitialTargetEUL), i);
			if(b_PositionAffected)
				t_Target.localPosition = Vector3.Lerp (currentPos, v3_InitialTargetPOS, i);

			yield return null;
		}

		b_JumpCheck = false;
	}
}