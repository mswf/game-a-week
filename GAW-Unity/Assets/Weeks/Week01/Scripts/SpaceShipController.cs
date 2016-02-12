using UnityEngine;
using System.Collections;


namespace Week01
{
	[RequireComponent(typeof(Rigidbody))]
	public class SpaceShipController : MonoBehaviour
	{

		public Transform cameraTransform;
		public Transform cameraHolder;
		public Transform cameraParent;

		public ParticleSystem _exhaustParticleSystem;


		public float moveSpeed = 10f;

		public float maxRadiansDelta = 1f;
		public float maxMagnitudeDelta = 1f;

		private Rigidbody _rigidbody;
		private float _currentAcceleration = 0f;
		private ParticleSystem.EmitParams exhaustEmitParams;
		public MeshRenderer shipMeshRenderer;
		private Color _initialExhaustColor;

		private const int ExhaustMaterialIndex = 2;

		// Use this for initialization
		void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.maxAngularVelocity = Mathf.Infinity;
			exhaustEmitParams = new ParticleSystem.EmitParams();
			

			_initialExhaustColor = shipMeshRenderer.materials[ExhaustMaterialIndex].GetColor("_EmissionColor");

			//_rigidbody.ma
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				if (Cursor.lockState == CursorLockMode.Locked)
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
				}
				else
					Cursor.lockState = CursorLockMode.Locked;
			}

			if (Input.GetKey(KeyCode.W) || Input.GetMouseButton(0))
			{
				//Cursor.lockState = CursorLockMode.Locked;

				MoveForward();

			}

			UpdateRotation();

			cameraParent.localPosition = transform.localPosition;



			var curColor = Color.white;
			curColor.a = Mathf.Lerp(0.01f, 1f, _currentAcceleration);
			//exhaustEmitParams.startColor = curColor;

			_exhaustParticleSystem.startColor = curColor;
			
			var newColor = Color.Lerp(_initialExhaustColor, Color.white, _currentAcceleration);

			shipMeshRenderer.materials[ExhaustMaterialIndex].SetColor("_EmissionColor", newColor);

			_currentAcceleration *= 0.5f;
		}



		private void MoveForward()
		{
			var cameraForward = cameraTransform.transform.forward;
			var currentForward = transform.forward;

			var targetForward = Vector3.RotateTowards(currentForward, cameraForward, maxRadiansDelta * Time.deltaTime, maxMagnitudeDelta  * Time.deltaTime);

	//		transform.Translate();
			//Log.Steb();


			var alignmentWithTarget = Vector3.Dot(targetForward, cameraForward)*0.5f + 0.5f;
			_currentAcceleration = alignmentWithTarget;

			_rigidbody.AddRelativeForce((transform.InverseTransformDirection(currentForward)) * alignmentWithTarget * moveSpeed * Time.deltaTime, ForceMode.Acceleration);

			var targetRotation = new Quaternion();
			targetRotation.SetLookRotation(targetForward);

			_rigidbody.MoveRotation(targetRotation);


		}

		private void UpdateRotation()
		{


	
		}
	}


}
