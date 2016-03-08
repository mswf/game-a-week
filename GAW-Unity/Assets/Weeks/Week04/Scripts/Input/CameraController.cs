using UnityEngine;
using System.Collections;

namespace Week04
{
	[RequireComponent(typeof(Camera))]
	public class CameraController : MonoBehaviour
	{

		public const float DefaultScrollSpeed = 40f;
		
		private Camera _camera;

		// Use this for early referencing
		private void Awake()
		{
			Globals.cameraController = this;

			_camera = GetComponent<Camera>();
			_camera.fieldOfView = _camera.fieldOfView;
		}

		private void OnEnable()
		{
			Globals.cameraController = this;
		}

		// Use this for initialization
		private void Start () 
		{

		}
		
		// Update is called once per frame
		public void UpdateInput () 
		{
			if (Input.GetKey(KeyCode.A))
			{
				Scroll(DefaultScrollSpeed * Time.deltaTime * -1f);
			}
			if (Input.GetKey(KeyCode.D))
			{
				Scroll(DefaultScrollSpeed*Time.deltaTime);
			}
		}

		public void Scroll(float amount)
		{
			transform.Translate(amount, 0,0);
		}
	}
}
