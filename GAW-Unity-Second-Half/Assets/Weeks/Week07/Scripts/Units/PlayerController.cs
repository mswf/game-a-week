using UnityEngine;
using System.Collections;
using UnityEditorInternal;

namespace Week07
{
	public class PlayerController : Unit
	{
		[Header("Movement/Input")]

		[Range(0.5f, 20f), SerializeField]
		private float globalForceMultiplier = 1f;

		[Range(0.5f, 200f), SerializeField]
		private float directionalForceMultiplier = 1f;

		[Range(0.5f, 60f), SerializeField]
		private float jumpForce = 1f;

		private CapsuleCollider _characterCapsuleCollider;

		// Use this for early referencing
		protected override void Awake()
		{
			base.Awake();

			_rigidBody.drag = 0f;

			currentDrag = baseDrag;

			previousHeading = _transform.forward;

			_characterCapsuleCollider = _characterCollider as CapsuleCollider;
		}

		// Use this for initialization
		private void Start () 
		{
		
		}

		[Header("Physics")]
		[SerializeField]
		private Vector2 baseDrag = new Vector2(3f, 0f);
		[SerializeField]
		private Vector2 runningDrag = new Vector2(2f, 0f);
		[SerializeField, ReadOnly]
		private Vector2 currentDrag = new Vector2(0f, 0f);

		
		[HideInInspector]
		private Vector3 previousInput;
		private Vector3 previousHeading;

		private bool _isRunning;

		public override Vector3 GetCurrentHeading()
		{
			return previousHeading;
		}

		public override bool IsRunning()
		{
			return _isRunning;
		}

		private void FixedUpdate()
		{
			var dt = Time.fixedDeltaTime;

			// Polling input
			var horizontalInput = Input.GetAxis("Horizontal");
			var verticalInput = Input.GetAxis("Vertical");
			
			var directionalInput = new Vector3(horizontalInput, 0, verticalInput);

			previousInput = directionalInput;

			if (directionalInput.magnitude > 0.1f)
			{
				previousHeading = directionalInput;
			}

			var runInput = Input.GetAxis("Run");
			var jumpInput = Input.GetButtonDown("Jump");

			if (Input.GetButtonDown("Fire1"))
			{
				
	//			Debug.Log("Attacking");

				if (_weapons[0].IsReadyForAttack())
					_weapons[0].DoAttack();
				else
					_weapons[1].DoAttack();

			}

			if (jumpInput && IsGrounded())
			{
				_rigidBody.AddForce(Vector3.up * jumpForce * globalForceMultiplier * dt, ForceMode.VelocityChange);
			}




			if (directionalInput.magnitude < 0.2f)
			{
				const float maxDrag = 5f;

				currentDrag.x = Mathf.MoveTowards(currentDrag.x, maxDrag, Time.deltaTime * (maxDrag - baseDrag.x )  *5f);
			}
			else
			{
				if (runInput > 0.8f)
				{
					currentDrag.x = runningDrag.x;
					_isRunning = true;
				}
				else
				{
					currentDrag.x = baseDrag.x;
					_isRunning = false;
				}
			}


			var cap = _characterCapsuleCollider;
			
			var capsuleStart = cap.center + _transform.position;
			capsuleStart.y -= (cap.height / 2f);

			const float distanceAhead = 1.2f;


			var capStartForward = capsuleStart + directionalInput.normalized * 0.3f;

			var radius = cap.radius * 0.9f;

			DebugExtension.DebugCapsule(capStartForward,
				capStartForward + Vector3.up * (cap.height),
				Color.white, radius, dt, false);

			if (Physics.CheckCapsule(capStartForward,
				capStartForward + Vector3.up*(cap.height),
				radius, 1 << HitCollider.layerMask))
			{
				const float angle = Mathf.Deg2Rad * 45f;

				var rotatedLeft = new Vector3
				{
					x = directionalInput.x*Mathf.Cos(angle) - directionalInput.z*Mathf.Sin(angle),
					z = directionalInput.z*Mathf.Cos(angle) + directionalInput.x*Mathf.Sin(angle)
				};


				capStartForward = capsuleStart + rotatedLeft.normalized * distanceAhead;

				DebugExtension.DebugCapsule(capStartForward,
					capStartForward + Vector3.up * (cap.height),
					Color.red, radius, dt, false);

				if (!Physics.CheckCapsule(capStartForward,
					capStartForward + Vector3.up*(cap.height),
					radius, 1 << HitCollider.layerMask))
				{
					directionalInput = rotatedLeft;
				}
				else
				{
					var rotatedRight = new Vector3
					{
						x = directionalInput.x * Mathf.Cos(-angle) - directionalInput.z * Mathf.Sin(-angle),
						z = directionalInput.z * Mathf.Cos(-angle) + directionalInput.x * Mathf.Sin(-angle)
					};


					capStartForward = capsuleStart + rotatedRight.normalized * distanceAhead;

					DebugExtension.DebugCapsule(capStartForward,
						capStartForward + Vector3.up * (cap.height),
						Color.blue, radius, dt, false);

					if (!Physics.CheckCapsule(capStartForward,
						capStartForward + Vector3.up * (cap.height),
						radius, 1 << HitCollider.layerMask))
					{
						directionalInput = rotatedRight;
					}
				}
				


			}




			DebugExtension.DebugArrow(_transform.position, directionalInput.normalized * 5f, Color.gray, dt * 10f, false);

			_rigidBody.AddForce(directionalInput * directionalForceMultiplier * globalForceMultiplier * dt, ForceMode.Acceleration);



			// Applying drag
			var curVelocity = _rigidBody.velocity;


			curVelocity.x = curVelocity.x * (1f - dt * currentDrag.x);
			curVelocity.z = curVelocity.z * (1f - dt * currentDrag.x);
			// Vertical drag
			curVelocity.y = curVelocity.y * (1f - dt * currentDrag.y);
			
			_rigidBody.velocity = curVelocity;

		}

		// Update is called once per frame
		private void Update ()
		{
			_unitVisualizer.C_Update(this);


		}
	}
}
