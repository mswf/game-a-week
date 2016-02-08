using UnityEngine;
using System.Collections;

namespace Week01
{
	[RequireComponent(typeof(Camera))]
	public class MeshDeformerInput : MonoBehaviour
	{
		
		public float force = 10f;
		public float forceOffset = 0.1f;

		private Camera camera;

		// Use this for initialization
		void Start()
		{
			camera = GetComponent<Camera>();
		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetMouseButton(0))
			{
				HandleInput();
			}
		}

		private void HandleInput()
		{
			Ray inputRay = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(inputRay, out hit))
			{
				MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();

				if (deformer)
				{
					Vector3 point = hit.point;
					point += hit.normal*forceOffset;
					deformer.AddDeformingForce(point, force);
				}
			}
		}
	}
}
