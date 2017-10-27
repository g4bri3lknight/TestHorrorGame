//Script written by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;

public class HDK_AIDamageTrigger : MonoBehaviour 
{
	// Inspector Variables
	[SerializeField] string			_parameter = "";
	[SerializeField] int			_bloodParticlesBurstAmount	=	10;
	[SerializeField] float			_damageAmount				=	0.1f;

    // Private Variables
    HDK_AIStateMachine  _stateMachine 		= null;
	Animator	   	 	_animator	 		= null;
	int			    	_parameterHash		= -1;

	// ------------------------------------------------------------
	// Name	:	Start
	// Desc	:	Called on object start-up to initialize the script.
	// ------------------------------------------------------------
	void Start()
	{
		// Cache state machine and animator references
		_stateMachine = transform.root.GetComponentInChildren<HDK_AIStateMachine> ();

		if (_stateMachine != null)
			_animator = _stateMachine.animator;

		// Generate parameter hash for more efficient parameter lookups from the animator
		_parameterHash = Animator.StringToHash (_parameter); 
	}

	// -------------------------------------------------------------
	// Name	:	OnTriggerStay
	// Desc	:	Called by Unity each fixed update that THIS trigger
	//			is in contact with another.
	// -------------------------------------------------------------
	void OnTriggerStay( Collider col )
	{
		// If we don't have an animator return
		if (!_animator)
			return;

		// If this is the player object and our parameter is set for damage
		if (col.gameObject.CompareTag ("Player") && _animator.GetFloat(_parameterHash) >0.9f)
		{
			if (GameSceneManager.instance && GameSceneManager.instance.bloodParticles) 
			{
				ParticleSystem system = GameSceneManager.instance.bloodParticles;

				// Temporary Code
				system.transform.position = transform.position;
				system.transform.rotation = Camera.main.transform.rotation;

				var settings = system.main;
				settings.simulationSpace = ParticleSystemSimulationSpace.World;
				system.Emit (_bloodParticlesBurstAmount);
			}
            FindObjectOfType<HDK_PlayerHealth>().TakeDamage(_damageAmount);
        }
	}
}