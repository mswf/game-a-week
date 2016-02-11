using UnityEngine;

namespace Week01
{
	[RequireComponent(typeof(MeshFilter))]
	public class MeshDeformer : MonoBehaviour
	{
		private Mesh _deformingMesh;
		private Vector3[] _originalVertices, _displacedVertices;

		private Vector3[] _vertexVelocities;

		public float springForce = 20f;
		public float damping = 5f;

		private float _uniformScale = 1f;

		// Use this for initialization
		private void Start()
		{
			_deformingMesh = GetComponent<MeshFilter>().mesh;
			_originalVertices = _deformingMesh.vertices;
			_displacedVertices = new Vector3[_originalVertices.Length];

			for (int i = 0; i < _originalVertices.Length; i++)
			{
				_displacedVertices[i] = _originalVertices[i];
			}

			_vertexVelocities = new Vector3[_originalVertices.Length];

		}

		// Update is called once per frame
		private void Update()
		{
			_uniformScale = transform.localScale.x;

			for (int i = 0; i < _displacedVertices.Length; i++)
			{
				UpdateVertex(i);
			}
			_deformingMesh.vertices = _displacedVertices;
			_deformingMesh.RecalculateNormals();
		}

		private void UpdateVertex(int i)
		{
			Vector3 velocity = _vertexVelocities[i];
			Vector3 displacement = _displacedVertices[i] - _originalVertices[i];
			displacement *= _uniformScale;
			velocity -= displacement*springForce*Time.deltaTime;
			velocity *= 1f - damping*Time.deltaTime;
			_vertexVelocities[i] = velocity;

			_displacedVertices[i] += velocity* (Time.deltaTime*_uniformScale);
		}

		public void AddDeformingForce(Vector3 point, float force)
		{	
			
			Debug.DrawLine(Camera.main.transform.position, point);

			point = transform.InverseTransformPoint(point);

			for (int i = 0; i < _displacedVertices.Length; i++)
			{
				AddForceToVertex(i, point, force);
			}
		}

		private void AddForceToVertex(int i, Vector3 point, float force)
		{
			Vector3 pointToVertex = _displacedVertices[i] - point;
			pointToVertex *= _uniformScale;
			float attenuatedForce = force/(1f + pointToVertex.sqrMagnitude);
			float velocity = attenuatedForce*Time.deltaTime;
			_vertexVelocities[i] += pointToVertex.normalized*velocity;
		}
	}

}

