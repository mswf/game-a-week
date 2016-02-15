using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using DG.Tweening;
using UnityEditor;
using UnityEngine.Assertions.Comparers;


namespace Week02
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour
	{
		public Transform playerTarget;

		public float moveSpeed = 2f;
		public float rotationSpeed = 1f;

		public Tweener movementTween;

		private Rigidbody _rigidbody;

		// Use this for initialization
		void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.maxAngularVelocity = float.PositiveInfinity;

			DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

		}

		private void OnDrawGizmosSelected()
		{
			if (Application.isPlaying)
			{
				float scale = 1f;
	//			Gizmos.color = Color.yellow;

//				Gizmos.DrawRay(transform.position, _rigidbody.velocity * scale * 2f);


				Gizmos.color = Color.black;
				
				Gizmos.DrawRay(transform.position, (Quaternion.Euler(0,45f,0) * _curMousePosition) * scale);

			}


		}

		private Vector3 _prevMousePosition;

		private Vector3 _curMousePosition;
		

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

			if (Input.GetMouseButton(0))
			{
				_curMousePosition += new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));




				playerTarget.position = transform.position + GetCurrentCameraRotation() * _curMousePosition;

			}

			if (Input.GetMouseButtonUp(0))
			{

				_rigidbody.angularVelocity = Vector3.zero;
				_rigidbody.velocity = Vector3.zero;

				Vector3 targetEndpoint = transform.position + GetCurrentCameraRotation() * _curMousePosition;

				playerTarget.position = targetEndpoint;

				//_rigidbody.AddForce(Quaternion.Euler(0, 45f, 0) * _curMousePosition.normalized * moveSpeed, ForceMode.VelocityChange);

				var offsetThisFrame = Quaternion.Euler(0, 135f, 0) * _curMousePosition;
	

				_rigidbody.AddTorque(offsetThisFrame * rotationSpeed, ForceMode.VelocityChange);


				if (false)
				{
					if (movementTween != null)
					{
						movementTween.Complete();
					}

					movementTween = transform.DOMove(targetEndpoint, (_curMousePosition).magnitude / moveSpeed, false)
						//.SetEase(Ease.InOutQuad);
						.SetEase(Ease.Linear);
				}

				_curMousePosition = Vector3.zero;

			}

			
		}

		private Quaternion GetCurrentCameraRotation()
		{
			return Quaternion.Euler(0, 45f, 0);
		}
	}


}

