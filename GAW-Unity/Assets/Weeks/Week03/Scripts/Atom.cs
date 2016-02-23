using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;


namespace Week03
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Atom : MonoBehaviour
	{
		public Atom[] atoms;

		public float tightness = 0.5f;
		public float damping = 0.5f;


		public new Rigidbody2D rigidbody2D;
		public new Transform transform;
		public new Collider2D collider;

		private const float MinDistance = 10f;

		public bool drawBridges = false;

		public float attractorProximity = 10f;


		// Use this for initialization
		void Start()
		{
			rigidbody2D = GetComponent<Rigidbody2D>();
			transform = GetComponent<Transform>();
			collider = GetComponent<Collider2D>();

			UpdateAtoms(ref atoms, transform.position, attractorProximity);
			/*
			foreach (var atom in GameObject.FindObjectsOfType<Atom>())
			{
				if (atom != this)
				{
					atoms.Add(atom);
				}
			}
			*/


			//rigidbody2D.centerOfMass = Vector2.down;
		}

		private static readonly Color FadedWhite = new Color(1,1,1,0.1f);

		private static List<Atom> _cachedAtomList = new List<Atom>(); 

		private static void UpdateAtoms(ref Atom[] atoms, Vector3 position, float radius)
		{
			_cachedAtomList.Clear();

			int count = atoms.Length;

			for (int index = 0; index < atoms.Length; index++)
			{
				_cachedAtomList.Add(atoms[index]);
			}

			foreach (var colliderInCircle in Physics2D.OverlapCircleAll(position, radius))
			{
				if (count > 8)
				{
					break;
				}

				var atom = colliderInCircle.GetComponent<Atom>();
				if (atom != null && !_cachedAtomList.Contains(atom))
				{
					_cachedAtomList.Add(atom);
					count++;
				}


			}

			atoms = _cachedAtomList.ToArray();
		}


		private void FixedUpdate()
		{
			var dt = Time.fixedDeltaTime;

			//DebugExtension.DebugCircle(transform.position, Vector3.forward, FadedWhite, attractorProximity, dt);

			//UpdateAtoms(ref atoms, transform.position, attractorProximity);


			foreach (var atom in atoms)
			{
				var distance = (Vector2)(transform.position - atom.transform.position);
				distance -= distance.normalized* MinDistance;
				var force = -distance * tightness - (damping * (rigidbody2D.velocity - atom.rigidbody2D.velocity));

				rigidbody2D.AddForce(force * dt, ForceMode2D.Force);


				collider.enabled = false;
				if (drawBridges) 
				{
					Vector2 rayDirection = (atom.transform.position - transform.position);
					

					var hitInfo = Physics2D.Raycast(transform.position, rayDirection);
					Debug.DrawRay(transform.position, rayDirection, Color.black, dt);

					if (hitInfo)
					{

						var atomComponent = hitInfo.collider.GetComponent<Atom>();
						if (atomComponent != null && atomComponent == atom)
						{

							
						}
					}

				}
				collider.enabled = true;
			}




		}

		// Update is called once per frame
		void Update()
		{

		}
	}

}

