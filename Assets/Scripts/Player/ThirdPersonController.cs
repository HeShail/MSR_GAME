using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using TMPro;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class ThirdPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 2.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 5.335f;
		[Tooltip("How fast the character turns to face movement direction")]
		[Range(0.0f, 0.3f)]
		public float RotationSmoothTime = 0.12f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 7.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.50f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.4f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = 20.0f;
		[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
		public float CameraAngleOverride = 0.0f;
		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;

		// cinemachine
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _animationBlend;
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		// animation IDs
		private int _animIDSpeed;
		private int _animIDGrounded;
		private int _animIDJump;
		private int _animIDMotionSpeed;
		private int _animFallSpeed;
		private int _animIDDeath;
		private int _animIDPick;
		private int _animIDLanding;
		private int _animIDStop;
		private int _animIDAimRot;


		private Animator _animator;
		private CharacterController _controller;
		private PlayerInputs _input;
		private Cinemachine.CinemachineImpulseSource RunImpulseSource;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool _hasAnimator;
		[Space(10)]
		public GameObject body;

		[Header("Minimap")]
		public Transform minimapCamera;

		[Space(10)]
		[Header("Atributes")]

		public TextMeshProUGUI healthText;
		[SerializeField] private float health=100.0f;
		[SerializeField] private bool isDead=false;
		[SerializeField] private float pickUpAnimTime = 2.6f;
		[SerializeField] private float sensitivity = 1f;
		[SerializeField] private bool detecting_Pickeable=false;
		[SerializeField] private bool targetable = true;
		private bool tillGrounded=true;
		private bool isRunning = false;
		private bool isWalking = false;
		private bool healDisabled = false;
		private float _aimRot;

		[Space(10)]
		[Header("Caída libre")]
		[SerializeField] private bool landing=false;
		[SerializeField] private bool softBind=false;
		[SerializeField] private bool hardBind = false;
		[SerializeField] private float flyingTime = 0f;
		[SerializeField] private float recoveryTime = 0f;
		[SerializeField] private float landingAnimTime = 1f;
		private float landingVerticalVelocity = -11.0f;
		private const float LIGHTFALL_DMG = 1f;
		private const float MORTALFALL_DMG = 2f;
		private float jumpTemp = 0f;
		private bool isFalling;

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			RunImpulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
			_hasAnimator = body.TryGetComponent(out _animator);
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<PlayerInputs>();
			softBind = false;
			hardBind = false;
			landing = false;
			isFalling = false;
			isRunning = false;
			isWalking = false;
			_aimRot = 0f;
			AssignAnimationIDs();
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
			recoveryTime = 0f;
		}

		private void Update()
		{
			if (health > 0f) healthText.text = "VIDA : " + Mathf.Round(health);
			else healthText.text = "VIDA : 0";
			GroundedCheck();
			if (!isDead)
            {
				Move();
				JumpAndGravity();
				if (!healDisabled) HealthRegen();

				if (health <= 0f)
				{
					isDead = true;
					_animator.SetBool(_animIDDeath, true);
				}

            }
            else
            {
				if (health >= 0.1f)
				{
					isDead = false;
					_animator.SetBool(_animIDDeath, false);
				}
			}

			//Pick up
			if (_input.interact && Grounded && detecting_Pickeable)
			{
				if ((_hasAnimator) && !hardBind )
				{
					_animator.SetTrigger(_animIDPick);
					StartCoroutine("Inspect");

				}
				detecting_Pickeable = false;
			}

			if (minimapCamera != null) minimapCamera.transform.position = transform.position;
			CameraRotation();

		}

        /// <summary> Metodo oculto que inicializa los parametros del animator del personaje a variables declaradas. </summary>
        private void AssignAnimationIDs()
		{
			_animIDSpeed = Animator.StringToHash("speed");
			_animIDGrounded = Animator.StringToHash("Grounded");
			_animIDJump = Animator.StringToHash("Jump");
			_animFallSpeed = Animator.StringToHash("FallSpeed");
			_animIDDeath = Animator.StringToHash("Dead");
			_animIDPick = Animator.StringToHash("Pick");
			_animIDLanding = Animator.StringToHash("Landing");
			_animIDStop = Animator.StringToHash("Stop");
			_animIDAimRot = Animator.StringToHash("AimRot");
		}
		/// <summary> Metodo vacio que gestiona el control de camara (rotaciones). </summary>
		private void CameraRotation()
		{
			// if there is an input and camera position is not fixed
			if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
			{
				_cinemachineTargetYaw += _input.look.x * Time.deltaTime * sensitivity;
				_cinemachineTargetPitch += _input.look.y * Time.deltaTime * sensitivity;
			}

			// clamp our rotations so our values are limited 360 degrees
			_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
			_aimRot = _cinemachineTargetPitch;
			_animator.SetFloat(_animIDAimRot, _aimRot);

			// Cinemachine will follow this target
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
			if (minimapCamera != null) minimapCamera.transform.rotation = Quaternion.Euler(0.0f, _cinemachineTargetYaw, 0.0f);
		}
		/// <summary> Metodo oculto que regenera la vida del personaje. </summary>
		private void HealthRegen()
        {
			if (health < 100) health+= 0.05f;
			else health = 100.0f;
        }

		/// <summary> Corutina que suspende la autocuracion. </summary>
		/// <remarks> Empleada para las grandes caidas. </remarks>
		IEnumerator DisableHeal()
        {
			healDisabled = true;
			yield return new WaitForSeconds(4.0f);
			healDisabled = false;

		}
		/// <summary> Funcion publica que restablece el estado predeterminado del jugador al ser electrocutado. </summary>
		public void Electrocute()
        {
			health = 0f;
			_animator.SetBool(_animIDDeath, true);
			MoveBind();
		}
		/// <summary> Funcion publica que restablece el estado predeterminado del jugador al caer en combate/mision. </summary>
		public void Respawn()
        {
			health = 100f;
			UnbindMovement();
			_animator.SetBool(_animIDDeath, false);

			//GameObject.Find("GameManager").GetComponent<PlayerRespawn>().DeathRespawn();
        }

		/// <summary> Metodo oculto encargado de manejar el desplazamiento del personaje. </summary>
		private void Move()
		{
			float targetSpeed;
			//Hardbind reduces character movement to 0
			if (!hardBind)targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed; else targetSpeed = 0.0f;
			if (_input.sprint && targetSpeed > 2f) isRunning = true; else isRunning = false;

			//if (GetComponent<ThirdPersonShooterController>().GetWeapon() > 0) targetSpeed = MoveSpeed;
			if (GetComponent<ThirdPersonShooterController>().GetAimStatus()) targetSpeed = MoveSpeed;
			if (GetComponent<ThirdPersonShooterController>().IsMovingBackwards()) targetSpeed = targetSpeed - 2f;
			if (GetComponent<ThirdPersonShooterController>().IsMovingBackwards() && _input.sprint) targetSpeed = MoveSpeed - 2f;
			//if ((GetComponent<ThirdPersonShooterController>().GetAimStatus() && Input.GetKey(KeyCode.A)) ||
			//	(GetComponent<ThirdPersonShooterController>().GetAimStatus() && Input.GetKey(KeyCode.D))) targetSpeed = MoveSpeed - 1f;

				// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

				// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
				// if there is no input, set the target speed to 0
				if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			//Softbind locks character movement to last speed in mid air
			if (!softBind)
            {
				// accelerate or decelerate to target speed
				if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
				{
					// creates curved result rather than a linear one giving a more organic speed change
					// note T in Lerp is clamped, so we don't need to clamp our speed
					_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

					// round speed to 3 decimal places
					_speed = Mathf.Round(_speed * 1000f) / 1000f;
				}
				else
                {
					_speed = targetSpeed;
                }
				_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
			}

			//HardBind locks character rotation and reduce movement to zero
			if (!hardBind)
            {
				// normalise input direction
				Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

				// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
				// if there is a move input rotate player when the player is moving
				if (_input.move != Vector2.zero)
				{
					_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
					float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
					if (gameObject.GetComponent<ThirdPersonShooterController>().GetAimStatus() == false)
					// rotate to face input direction relative to camera position
					transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
				}
			}
			


			Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

			// move the player
			 _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                //_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }

			if (targetSpeed > 5f && Grounded)
            {
				RunImpulseSource.GenerateImpulse();
            }


		}

		//private void JumpAndGravity()
		//{
		//	if (Grounded)
		//	{
		//		if (softBind && landing)
		//              {
		//			if (_verticalVelocity <= landingVerticalVelocity)
		//			{
		//				StartCoroutine("JumpBind");
		//				Invoke("JumpUnlock", 1.0f);
		//			}
		//			else JumpUnlock();
		//			flyingTime = 0f;

		//		}
		//		//isBind = false;

		//		// reset the fall timeout timer

		//		_fallTimeoutDelta = FallTimeout;

		//		// update animator if using character
		//		if (_hasAnimator)
		//		{
		//			_animator.SetBool(_animIDJump, false);
		//			_animator.SetFloat(_animFallSpeed, _verticalVelocity);
		//		}

		//		// stop our velocity dropping infinitely when grounded
		//		if (_verticalVelocity < 0.0f)
		//		{
		//			_verticalVelocity = -2f;

		//		}
		//		if (_input.jump)
		//		{
		//			// update animator if using character
		//			if ((_hasAnimator) && !landing)
		//			{
		//				_animator.SetBool(_animIDJump, true);
		//			}


		//              }
		//              //Jump

		//              if (_input.jump && _jumpTimeoutDelta <= 0.0f)
		//              {
		//                  update animator if using character

		//                  if ((_hasAnimator) && !landing)
		//                  {
		//                      _animator.SetBool(_animIDJump, true);
		//                  }
		//                  the square root of H * -2 * G = how much velocity needed to reach desired height
		//                 _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);


		//              }

		//              // jump timeout
		//              if (_jumpTimeoutDelta >= 0.0f)
		//		{
		//			_jumpTimeoutDelta -= Time.deltaTime;
		//		}
		//	}
		//	else
		//	{
		//		landing = true;
		//		softBind = true;
		//		if (!tillGrounded) stateAnim.SetTrigger("Fall");
		//		// reset the jump timeout timer
		//		_jumpTimeoutDelta = JumpTimeout;

		//		// fall timeout
		//		if (_fallTimeoutDelta >= 0.0f)
		//		{
		//			_fallTimeoutDelta -= Time.deltaTime;
		//		}

		//		// if we are not grounded, do not jump
		//		_input.jump = false;
		//		// update animator if using character

		//		flyingTime += Time.deltaTime;
		//	}

		//	// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		//	if (_verticalVelocity < _terminalVelocity)
		//	{
		//		_verticalVelocity += Gravity * Time.deltaTime;
		//	}
		//}

		/// <summary> Metodo oculto que trata el salto y gravedad del personaje. </summary>
		private void JumpAndGravity()
		{
			_animator.SetFloat("jumpTemp", jumpTemp);
			if (Grounded)
			{
				jumpTemp = 0f;
				if (softBind && landing)
				{
					recoveryTime = 0.5f;
					if (_verticalVelocity <= landingVerticalVelocity)
					{
						StartCoroutine("JumpBind");
						Invoke("JumpUnlock", 1.0f);
						recoveryTime += 1f;
					}
					else JumpUnlock();
					flyingTime = 0f;
				}
                if (isFalling && !landing && tillGrounded)_animator.SetTrigger("fall");
				isFalling = false;
				softBind = false;

				if (Input.GetKeyDown(KeyCode.Space)&& recoveryTime <= 0f && !hardBind)
				{
					// update animator if using character
					if ((_hasAnimator))
					{
						_animator.SetTrigger(_animIDJump);
					}
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

				}

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;

                }
            }else
            {
				softBind = true;
                landing = true;
                if (!tillGrounded)isFalling = true;
				jumpTemp+=Time.deltaTime;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
			recoveryTime -= Time.deltaTime;
		}

		/// <summary> Funcion publica que limita la habilidad de detectar un objeto respecto a un valor (status). </summary>
		public void SendPickeable(bool status)
        {
			detecting_Pickeable = status;
        }

		/// <summary> Corutina encargada de limitar el movimiento del personaje al recoger objeto. </summary>
		IEnumerator Inspect()
        {
			hardBind = true;
			yield return new WaitForSeconds(pickUpAnimTime);
			hardBind = false;
		}

		/// <summary> Funcion publica que restringe completamente el movimiento del personaje del jugador. </summary>
		public void MoveBind()
        {
			landing = true;
			hardBind = true;
			softBind = true;
        }

		/// <summary> Funcion publica que libera completamente el movimiento del personaje del jugador. </summary>
		public void UnbindMovement()
        {
			landing = false;
			hardBind = false;
			softBind = false;
		}

		/// <summary> Funcion vacia que anula las restricciones de movimiento tras caida. </summary>
		void JumpUnlock()
        {
			softBind = false;
			landing = false;
        }

		/// <summary> Corutina que trata el daño y los efectos de caida de alura. </summary>
		IEnumerator JumpBind()
		{
			_animator.SetBool(_animIDLanding, landing);
			landing = true;
			softBind = false;
			hardBind = true;
			if (flyingTime <= LIGHTFALL_DMG)
            {
				health -= 20.0f;
				StartCoroutine("DisableHeal");

			}
			else if ((flyingTime <= MORTALFALL_DMG) &&(flyingTime > LIGHTFALL_DMG))
            {
				health -= 40.0f;
				StartCoroutine("DisableHeal");
			}
			else if (flyingTime > MORTALFALL_DMG)
			{
				health = -1.0f;
            }
			yield return new WaitForSeconds(landingAnimTime);
			landing = false;
			_animator.SetBool(_animIDLanding, landing);
			hardBind = false;
		}

		/// <summary> Funcion publica que modifica la variable de deteccion hacia el personaje jugador. </summary>
		public void ChangeTargetableStatus(bool state)
        {
			targetable = state;
        }

		/// <summary> ¿El personaje jugador es alcanzable? . </summary>
		public bool GetTargetableStatus()
        {
			return targetable;
        }

		/// <summary> Funcion privada que detecta el contacto con el suelo. </summary>
		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
			tillGrounded = Physics.CheckSphere(spherePosition, GroundedRadius+0.3f, GroundLayers, QueryTriggerInteraction.Ignore);
			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDGrounded, tillGrounded);
			}
		}

		/// <summary> Metodo estatico que establece limite al giro de camara. </summary>
		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		/// <summary> Funcion publica que asigna un valor a la sensibilidad de la camara de jugador. </summary>
		public void SetSensitivity(float value)
		{
			sensitivity = value;
		}

		/// <summary> ¿El personaje esta corriendo? </summary>
		public bool IsRunning()
        {
			return isRunning;
        }

		/// <summary> ¿El personaje esta andando? </summary>
		public bool IsWalking()
		{
			return isWalking;
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			
			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}

	}
}