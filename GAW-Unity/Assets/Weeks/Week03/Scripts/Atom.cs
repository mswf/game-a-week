using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;


namespace Week03
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class Atom : MonoBehaviour
	{
		public List<Connection> connections;
		public List<Atom> neighbours;


		public new Rigidbody2D rigidbody2D;
		public new Transform transform;
		public new Collider2D collider;

		public SpriteRenderer spriteRenderer;


		private const float MinDistance = 10f;
		public bool drawBridges = false;

		public float attractorProximity = 10f;

		private Color spriteColor;

		// Use this for initialization
		void Awake()
		{
			rigidbody2D = GetComponent<Rigidbody2D>();
			transform = GetComponent<Transform>();
			collider = GetComponent<Collider2D>();

			spriteRenderer = GetComponent<SpriteRenderer>();
			spriteColor = spriteRenderer.color;

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

		void Start()
		{
			UpdateConnections();

		}


		private void Update()
		{
			if (UnityEngine.Random.value > 0.97f)
			{
				//Connection.DisconnectAllNeighbours(this);
				UpdateConnections();
			}

			spriteColor.a = (rigidbody2D.velocity.magnitude-1f)/20f;
			spriteRenderer.color = spriteColor;
		}

		private const int MinNeighbours = 3;

		private void UpdateConnections()
		{
			//Connection.DisconnectAllNeighbours(this);


			Atom[] atoms = new Atom[0];

			float colliderSize = 1.28f*2f;
			UpdateAtoms(ref atoms, transform.position, colliderSize*2f);

			if (atoms.Length < MinNeighbours)
			{
				UpdateAtoms(ref atoms, transform.position, colliderSize * 4f);

				if (atoms.Length < MinNeighbours)
				{
					UpdateAtoms(ref atoms, transform.position, colliderSize * 6f);
					if (atoms.Length < MinNeighbours)
					{
						UpdateAtoms(ref atoms, transform.position, colliderSize * 8f);
						if (atoms.Length < MinNeighbours)
						{
							UpdateAtoms(ref atoms, transform.position, colliderSize * 10f);
							if (atoms.Length < MinNeighbours)
							{
								UpdateAtoms(ref atoms, transform.position, colliderSize * 12f);
								if (atoms.Length < MinNeighbours)
								{
									UpdateAtoms(ref atoms, transform.position, colliderSize * 15f);
									if (atoms.Length < MinNeighbours)
									{
										UpdateAtoms(ref atoms, transform.position, colliderSize * 20f);

									}
								}
							}
						}
					}
				}
			}


			foreach (var atom in atoms)
			{
				Connection.Connect(this, atom);
			}
		}

		private static List<Atom> _cachedAtomList = new List<Atom>(); 

		private static void UpdateAtoms(ref Atom[] atoms, Vector3 position, float radius)
		{
			_cachedAtomList.Clear();

			//int count = atoms.Length;

			for (int index = 0; index < atoms.Length; index++)
			{
				_cachedAtomList.Add(atoms[index]);
			}

			foreach (var colliderInCircle in Physics2D.OverlapCircleAll(position, radius))
			{
				//if (count > 8)
				//{
					//break;
				//}

				var atom = colliderInCircle.GetComponent<Atom>();
				if (atom != null && !_cachedAtomList.Contains(atom))
				{
					_cachedAtomList.Add(atom);
					//count++;
				}


			}

			atoms = _cachedAtomList.ToArray();
		}

		private void OnDestroy()
		{
			Connection.DisconnectAllNeighbours(this);
		}
	}

}

