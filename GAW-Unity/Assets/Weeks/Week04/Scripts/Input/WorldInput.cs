using UnityEngine;
using System.Collections;
//using UnityEngine.EventSystems;

namespace Week04
{
	[RequireComponent(typeof(Camera))]
	public class WorldInput : MonoBehaviour
	{
		private Camera _camera;

		public float inputWorldSize = 2f;

		// Use this for early referencing
		private void Awake()

		{
			Globals.worldInput = this;

			_camera = GetComponent<Camera>();
		}

		// Use this for initialization
		private void Start () 
		{
		
		}
		
		// Update is called once per frame
		private void Update () 
		{
			var touches = Input.touches;

			if (touches.Length > 0)
			{
				Debug.LogError("Registering: " + touches.Length + " touch points");

				if (touches.Length == 1)
				{
					ProcessInputPoint(touches[0].position);
				}
				else
				{
					ProcessInputPoint(Input.mousePosition);
				}
			}
			else
			{
				if (Input.GetMouseButton(0))
				{
					ProcessInputPoint(Input.mousePosition);
				}
			}
		}



		private void ProcessInputPoint(Vector3 inputPosition)
		{
			var dt = Time.deltaTime;

			inputPosition.z = 10f;
			var worldDirection = _camera.ScreenToWorldPoint(inputPosition) - transform.position;

			var ray = new Ray(transform.position, worldDirection);
			RaycastHit rayCastHit;

			if (Physics.Raycast(ray, out rayCastHit))
			{
				Debug.DrawRay(ray.origin, ray.direction*rayCastHit.distance, Color.grey, dt);
				DebugExtension.DebugPoint(rayCastHit.point, Color.blue, 4f, dt);
				DebugExtension.DebugWireSphere(rayCastHit.point, Color.blue, inputWorldSize, dt, true);

			}
			else
			{
				Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, dt);

			}
			
			foreach (var colliderInCircle in Physics.OverlapSphere(rayCastHit.point, inputWorldSize))
			{
			/*
				var rigidBody = colliderInCircle.GetComponent<Rigidbody2D>();

				var offset = rigidBody.position - worldPoint2D;

				foreach (var VARIABLE in colliderInCircle.GetComponent<Atom>().connections)
				{
					VARIABLE.lifeTimeLeft += dt * (offset.magnitude - maxRippleSize);
				}

				rigidBody.AddForce(offset.normalized * (offset.magnitude - maxRippleSize) * 50f * -1f);

				var color = new Color(0, 1, 0, (offset.magnitude / maxRippleSize) * -.9f + 1f);

				Debug.DrawRay(worldPoint, offset, color, dt);


				*/
			}

		}

		private void ProcessScrollPoint(Vector3 inputPosition)
		{
			
		}
	}
}
