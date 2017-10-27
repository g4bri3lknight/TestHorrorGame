//Script written by Unity and edited by Giovanni Cartella - giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
		[SerializeField] private bool m_IsWalking;
        [SerializeField] public float m_WalkSpeed;
		[SerializeField] public float m_RunSpeed;
        [SerializeField] private float climbSpeed = 3.0f;
        [SerializeField] private float climbRate = 0.5f;
        private float climbDownThreshold = -0.4f;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] public AudioClip[] m_FootstepSounds;     
        [SerializeField] private AudioClip[] m_LadderSounds;
        [SerializeField] public float footstep_volume;
		[SerializeField] public float jump_land_volume;
        [SerializeField] private AudioClip m_JumpSound;    
        [SerializeField] private AudioClip m_LandSound;
        
        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
		public CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        
        //Custom player variables
        public bool isRunning;
        public bool m_CanRun;
        public bool Backwards;
        bool onJump;

        //Falling Effect
        private float fallDistance;
        public bool falling = false;
        bool applyDam = true;
        private float fallingDamageThreshold;
        float fallDamageMultiplier;
        private Vector3 currentPosition;
        private Vector3 lastPosition;
        private float highestPoint;
        private float normalFDTreshold = 5;
        HDK_PlayerHealth healthScript;
        public Transform fallEffect;
        public Transform fallEffectItem;
        HeadBobController hbScript;
        
        //Ladder
        public bool m_onLadder = false;
        private bool useLadder = true;
        private Vector3 climbDirection = Vector3.up;
        private Vector3 lateralMove = Vector3.zero;
        private Vector3 ladderMovement = Vector3.zero;
        private Rigidbody rigbody;
        private GameObject LadderObject;
        private float CamRot;
        private float playTime = 0.0f;

        public bool CanRun
		{
			get { return m_CanRun; }
			set { m_CanRun = value; }
		}

        // Use this for initialization
        private void Start()
        {
            rigbody = GetComponent<Rigidbody>();
            m_CharacterController = GetComponent<CharacterController>();
            hbScript = GetComponentInChildren<HeadBobController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            healthScript = GetComponent<HDK_PlayerHealth>();
            fallDamageMultiplier = healthScript.FallingDamageMultiplier;
        }

        private void LateUpdate()
        {
            lastPosition = currentPosition;
        }

        IEnumerator FallCamera(Vector3 d, Vector3 dw, float ta)
        {
            Quaternion s = fallEffect.localRotation;
            Quaternion sw = fallEffectItem.localRotation;
            Quaternion e = fallEffect.localRotation * Quaternion.Euler(d);
            float r = 1.0f / ta;
            float t = 0.0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime * r;
                fallEffect.localRotation = Quaternion.Slerp(s, e, t);
                fallEffectItem.localRotation = Quaternion.Slerp(sw, e, t);
                yield return null;
            }
            hbScript.enabled = true;
        }

        private void Update()
        {
            if (m_Input.y < 0)
            {
                Backwards = true;
            }else
            {
                Backwards = false;
            }


            if (m_onLadder || falling || GetComponent<HDK_Hiding>().Hiding)
            {
                m_CanRun = false;
            }

            if(!m_CanRun && isRunning)
            {
                isRunning = false;
            }

            if (m_CharacterController.isGrounded)
            {
                if (falling)
                {
                    fallingDamageThreshold = normalFDTreshold;
                    falling = false;
                    fallDistance = highestPoint - currentPosition.y;
                    if (fallDistance > fallingDamageThreshold && applyDam)
                    {
                        ApplyFallingDamage(fallDistance);
                    }
                    StartCoroutine(FallCamera(new Vector3(7, Random.Range(-1.0f, 1.0f), 0), new Vector3(3, Random.Range(-0.5f, 0.5f), 0), 0.15f));
                }
            }
            else
            {
                currentPosition = transform.position;
                hbScript.enabled = false;
                if (currentPosition.y > lastPosition.y && !m_onLadder)
                {
                    highestPoint = transform.position.y;
                    falling = true;
                }

                if (!falling && !m_onLadder)
                {
                    highestPoint = transform.position.y;
                    falling = true;
                }
            }


            if (m_CharacterController.velocity.sqrMagnitude > 0 &&  Input.GetButton("Run") && CanRun)
            {
                isRunning = true;
			}

			if (m_CharacterController.velocity.sqrMagnitude == 0)
			{
					isRunning = false;
			}


            if (m_CharacterController.velocity.sqrMagnitude > 0 && !Input.GetButton("Run"))
            {
					isRunning = false;
			}
            
            if (!m_Jump && !m_onLadder)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded && !m_onLadder)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            if (m_onLadder)
            {
                rigbody.useGravity = false;
                rigbody.isKinematic = true;
                LadderUpdate();
            }
            else
            {
                LadderObject = null;
                rigbody.useGravity = true;
                rigbody.isKinematic = true;
            }
        }

        void ApplyFallingDamage(float fallDistance)
        {
            healthScript.FallingDamage(System.Convert.ToInt32(fallDistance * fallDamageMultiplier));
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
			m_AudioSource.volume = jump_land_volume;  
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        private void FixedUpdate()
        {
			float speed;
            GetInput(out speed);
            if (!m_onLadder)
            {
                Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;                
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f, ~0, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;

                if (m_CharacterController.isGrounded)
                {
                    m_MoveDir.y = -m_StickToGroundForce;

                    if (m_Jump)
                    {
                        m_MoveDir.y = m_JumpSpeed;
                        PlayJumpSound();
                        m_Jump = false;
                        m_Jumping = true;
                    }
                }
                else
                {
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                }
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

                ProgressStepCycle(speed);
            }
            UpdateCameraPosition(speed);
        }

        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
			m_AudioSource.volume = jump_land_volume;          
			m_AudioSource.Play();
        }
        
        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }
        
        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip, footstep_volume);
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }
        
		private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
         //   return;
            
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }

        void GetInput(out float speed)
        {
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

            #if !MOBILE_INPUT
            m_IsWalking = !Input.GetButton("Run");
	
            #endif         
			speed = m_IsWalking ? m_WalkSpeed : m_CanRun ? m_RunSpeed : m_WalkSpeed;
            m_Input = new Vector2(horizontal, vertical);
            
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
            
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {				
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        //When on Ladder Trigger
        private void OnTriggerStay(Collider ladder)
        {
            if (ladder.tag == "Ladder" && useLadder)
            {
                LadderObject = ladder.gameObject;
                m_onLadder = true;
                applyDam = false;
            }
        }

        //Ladder Trigger Exit
        private void OnTriggerExit(Collider ladder)
        {
            if (ladder.tag == "Ladder")
            {
                m_onLadder = false;
                useLadder = true;
                applyDam = true;
            }
        }

        //Ladder Movement
        private void LadderUpdate()
        {
            CamRot = m_Camera.transform.forward.y;
            if (m_onLadder)
            {
                Vector3 verticalMove;
                verticalMove = climbDirection.normalized;
                verticalMove *= Input.GetAxis("Vertical");
                verticalMove *= (CamRot > climbDownThreshold) ? 1 : -1;
                lateralMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                lateralMove = transform.TransformDirection(lateralMove);
                ladderMovement = verticalMove + lateralMove;
                m_CharacterController.Move(ladderMovement * climbSpeed * Time.deltaTime);

                if (Input.GetAxis("Vertical") == 1 && !(m_AudioSource.isPlaying) && Time.time >= playTime)
                {
                    PlayLadderSound();
                }

                if (Input.GetKey(KeyCode.Space))
                {
                    useLadder = false;
                    m_onLadder = false;
                    LadderObject = null;
                }
            }
        }

        //Ladder Footsteps
        void PlayLadderSound()
        {
            int s = Random.Range(0, m_LadderSounds.Length);
            m_AudioSource.clip = m_LadderSounds[s];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            playTime = Time.time + climbRate;
        }
    }
}
