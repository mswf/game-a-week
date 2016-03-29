using UnityEngine;
using System.Collections;

namespace Week07
{
	[RequireComponent(typeof(Rigidbody))]
	public abstract class Unit : MonoBehaviour
	{
		[HideInInspector]
		public Rigidbody _rigidBody;
		[HideInInspector]
		public Transform _transform;
		public Collider _characterCollider;
		public UnitVisualizer _unitVisualizer;


		public double health;


		private float _distanceToGround;

		// Use this for early referencing
		protected virtual void Awake()
		{
			GameGlobals.units.Add(this);

			_rigidBody = GetComponent<Rigidbody>();

			_distanceToGround = _characterCollider.bounds.extents.y;
			_transform = GetComponent<Transform>();
		}

		public abstract Vector3 GetCurrentHeading();
		public abstract bool IsRunning();

		public bool IsGrounded()
		{

			
			return Physics.Raycast(_transform.position + Vector3.up*0.1f, -Vector3.up, _characterCollider.contactOffset + 0.1f);
		}
	}
}
