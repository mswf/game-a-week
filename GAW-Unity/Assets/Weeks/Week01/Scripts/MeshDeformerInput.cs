using UnityEngine;

namespace Week01
{
	[RequireComponent(typeof(Camera))]
	public class MeshDeformerInput : MonoBehaviour
	{
		
		public float force = 10f;
		public float forceOffset = 0.1f;

		private Camera _camera;

		// Use this for initialization
		private void Start()
		{
			_camera = GetComponent<Camera>();
		}

		// Update is called once per frame
		private void Update()
		{
			if (Input.GetMouseButton(0))
			{
				HandleInput();
			}
		}

		private void HandleInput()
		{
			var inputRay = _camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(inputRay, out hit))
			{
				var deformer = hit.collider.GetComponent<MeshDeformer>();

				if (deformer)
				{
					var point = hit.point;
					point += hit.normal*forceOffset;
					deformer.AddDeformingForce(point, force);
				}
			}
		}
	}
}
