using UnityEngine;
using System.Collections;


namespace Week03
{
	public class Atom : MonoBehaviour
	{

		// primary
		public Vector2 position;
		public Vector2 momentum;
		public Quaternion orientation;

		// secondary
		public Vector2 angularVelocity;
		public Quaternion spin;

		// constant
		public float inertia;
		public float inverseInertia;

		private void Recalculate()
		{
			angularVelocity = momentum* inverseInertia;
		}

		private struct Derivative
		{
			public Vector2 velocity;
			public Vector2 force;
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}

}

