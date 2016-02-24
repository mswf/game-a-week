using UnityEngine;
using System.Collections;

namespace Week03
{
	[RequireComponent(typeof(Camera))]
	public class RippleInput : MonoBehaviour
	{

		private Camera _camera;

		public float minRippleSize;
		public float maxRippleSize;

		

		// Use this for initialization
		void Start()
		{
			_camera = GetComponent<Camera>();
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButton(0))
			{
				ProcessInputPoint(Input.mousePosition);
			}

			foreach (var touch in Input.touches)
			{
				ProcessInputPoint(touch.position);
			}
		}

		private void ProcessInputPoint(Vector3 inputPosition)
		{
			var dt = Time.deltaTime;

			inputPosition.z = -transform.position.z;
			var worldPoint = _camera.ScreenToWorldPoint(inputPosition);
			DebugExtension.DebugPoint(worldPoint, Color.blue, 4f, dt);
			//	Debug.DrawLine(transform.position, worldPoint, Color.green, 2f);
			DebugExtension.DebugCircle(worldPoint, Vector3.forward, Color.blue, minRippleSize, dt);

			DebugExtension.DebugCircle(worldPoint, Vector3.forward, Color.blue, maxRippleSize, dt);

			var worldPoint2D = (Vector2)worldPoint;
			foreach (var colliderInCircle in Physics2D.OverlapCircleAll(worldPoint2D, maxRippleSize))
			{

				var rigidBody = colliderInCircle.GetComponent<Rigidbody2D>();

				var offset = rigidBody.position - worldPoint2D;
				rigidBody.AddForce(offset.normalized * (offset.magnitude - maxRippleSize) * 50f * -1f);

				var color = new Color(0, 1, 0, (offset.magnitude / maxRippleSize) * -.9f + 1f);

				Debug.DrawRay(worldPoint, offset, color, dt);


			}

		}
	}
}


