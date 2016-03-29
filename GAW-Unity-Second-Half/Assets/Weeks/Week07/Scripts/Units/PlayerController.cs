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

		// Use this for early referencing
		protected override void Awake()
		{
			base.Awake();

			_rigidBody.drag = 0f;

			currentDrag = baseDrag;

			previousHeading = _transform.forward;
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
