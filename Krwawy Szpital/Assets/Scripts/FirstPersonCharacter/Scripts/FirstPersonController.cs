using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
	[RequireComponent(typeof (CharacterController))]
	[RequireComponent(typeof (AudioSource))]
	public class FirstPersonController : MonoBehaviour
	{
		

		[SerializeField] private float m_WalkSpeed;

		[SerializeField] private float m_RunSpeed;
		[SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;

		[SerializeField] private float m_StealthSpeed;
		[SerializeField] [Range(0f, 1f)] private float m_StealthstepLenghten;

		[SerializeField] private float m_JumpSpeed;
		[SerializeField] private float m_StickToGroundForce;
		[SerializeField] private float m_GravityMultiplier;
		[SerializeField] private MouseLook m_MouseLook;
		[SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
		[SerializeField] private float m_StepInterval;
		[SerializeField] private AudioClip[] m_FootstepSounds;    
		[SerializeField] private AudioClip m_JumpSound;           
		[SerializeField] private AudioClip m_LandSound;





        public bool m_IsCrouching;
        public float audibilityForCrouch;
        public bool m_IsWalking;
        public float audibilityForWalk;
        public bool m_IsRunning;
        public float audibilityForRun;

        public float audibility;
        public float m_speed;

        private Camera m_Camera;
		public bool m_Jump;
		private Vector2 m_Input;
		private Vector3 m_MoveDir = Vector3.zero;
		private CharacterController m_CharacterController;
		private CollisionFlags m_CollisionFlags;
		private bool m_PreviouslyGrounded;
		private Vector3 m_OriginalCameraPosition;
		private float m_StepCycle;
		private float m_NextStep;
		public bool m_Jumping;
		private AudioSource m_AudioSource;
		private bool isAlive;
		private Aspect aspect;

		private void Start()
		{
			m_CharacterController = GetComponent<CharacterController>();
			m_Camera = Camera.main;
			m_OriginalCameraPosition = m_Camera.transform.localPosition;
			m_StepCycle = 0f;
			m_NextStep = m_StepCycle/2f;
			m_Jumping = false;
			m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
			isAlive = true;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
				SceneManager.LoadScene("Main");

			if (isAlive)
			{
				RotateView();

				if (!m_Jump)
				{
					m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
				}



                if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
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
			}
		}


		private void PlayLandingSound()
		{
			m_AudioSource.clip = m_LandSound;
			m_AudioSource.Play();
			m_NextStep = m_StepCycle + .5f;
		}


		private void FixedUpdate()
		{
			if (isAlive)
			{
				GetInput();
				Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

				RaycastHit hitInfo;
				Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
								   m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
				desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

				m_MoveDir.x = desiredMove.x* m_speed;
				m_MoveDir.z = desiredMove.z* m_speed;


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
					m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
				}
				m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

				ProgressStepCycle();
				UpdateCameraPosition();

				m_MouseLook.UpdateCursorLock();
			}
		}


		private void PlayJumpSound()
		{
			m_AudioSource.clip = m_JumpSound;
			m_AudioSource.Play();
		}


		private void ProgressStepCycle()
		{
			if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
			{
				m_StepCycle += (m_CharacterController.velocity.magnitude + (m_speed * (m_IsWalking ? 1f : m_RunstepLenghten)))*
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
			// pick & play a random footstep sound from the array,
			// excluding sound at index 0
			int n = Random.Range(1, m_FootstepSounds.Length);
			m_AudioSource.clip = m_FootstepSounds[n];
			m_AudioSource.PlayOneShot(m_AudioSource.clip);
			// move picked sound to index 0 so it's not picked next time
			m_FootstepSounds[n] = m_FootstepSounds[0];
			m_FootstepSounds[0] = m_AudioSource.clip;
		}



        private void UpdateCameraPosition()
        {
            Vector3 newCameraPosition = m_Camera.transform.localPosition;

            //sytuacja gdy gracz porusza siê na ziemi
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();

                if (m_IsCrouching && newCameraPosition.y != (m_OriginalCameraPosition.y / 2))
                    newCameraPosition.y *= 0.5f;
                if (!m_IsCrouching && newCameraPosition.y == (m_OriginalCameraPosition.y / 2))
                    newCameraPosition.y *= 2;
            }
            else if (m_CharacterController.isGrounded) // gdy gracz nie porusza siê i jest na ziemi(stoi)
            {
                if (m_IsCrouching && newCameraPosition.y != (m_OriginalCameraPosition.y / 2))
                    newCameraPosition.y *= 0.5f;
                if (!m_IsCrouching && newCameraPosition.y == (m_OriginalCameraPosition.y / 2) && m_JumpBob.Offset() == 0)
                    newCameraPosition.y *= 2;
            }
            else //sytuacja gdy gracz nie porusza siê lub jest w powietrzu
            {
                if (m_JumpBob.Offset() > 0)
                    Debug.Log(m_JumpBob.Offset());
                
                newCameraPosition.y = m_OriginalCameraPosition.y - (m_JumpBob.Offset() <= 1 ? m_JumpBob.Offset() : 1);
            }

            if (newCameraPosition.y < 0.8 && !m_IsCrouching)
                newCameraPosition.y = m_OriginalCameraPosition.y;

            m_Camera.transform.localPosition = newCameraPosition;

        }


        private void GetInput()
		{
			float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
			float vertical = CrossPlatformInputManager.GetAxis("Vertical");

			bool waswalking = m_IsWalking;
            bool RunningBefore = m_IsRunning;
            bool CrouchingBefore = m_IsCrouching;

            m_IsRunning = Input.GetKey(KeyCode.LeftShift);
            m_IsCrouching = Input.GetKey(KeyCode.LeftControl);
            m_IsWalking = !m_IsCrouching && !m_IsRunning && m_CharacterController.velocity.magnitude > 0;

            if (!m_IsRunning && !m_IsCrouching)
            {
                m_speed = m_WalkSpeed;
                audibility = audibilityForWalk;
            }

            //œmieszne rzeczy siê dziej¹ przy jednoczesnym wciœnieciu shift i ctrl
            //mo¿e warto zmieniæ na obs³uge key up/down
            if (m_IsRunning && !RunningBefore && !m_IsCrouching)
            {
                m_speed = m_RunSpeed;
                audibility = audibilityForRun;
            }

            if (m_IsCrouching && !CrouchingBefore)
            {
                m_speed = m_StealthSpeed;
                audibility = audibilityForCrouch;
            }
            if (!m_IsRunning && !m_IsCrouching && !m_IsWalking)
                audibility = 0f;

            m_Input = new Vector2(horizontal, vertical);

			// normalize input if it exceeds 1 in combined length:
			if (m_Input.sqrMagnitude > 1)
			{
				m_Input.Normalize();
			}
		}


		private void RotateView()
		{
			if (isAlive)
			{
				m_MouseLook.LookRotation(transform, m_Camera.transform);
			}
			
		}


		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if(isAlive)
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

		}

		public void DieOnCollisionWithMonster()
		{
            if(isAlive)
            {
                isAlive = false;
                audibility = 0;
                GetComponent<Aspect>().aspectName = Aspect.aspect.DeadBody;
                for (int i = 0; i < 100; i++)
                {
                    transform.position -= new Vector3(0, 0.005f, 0);
                    transform.Rotate(-0.3f, 0, -0.3f);
                }
            }
		}
	}
}
