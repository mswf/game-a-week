using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;


namespace Week03
{
	[System.Serializable]
	public class Connection
	{

		public static ObjectPool<Connection> pool = new ObjectPool<Connection>();

		public Atom a;
		public Atom b;

		public float currentStress;
		public float maxStress;

		public float preferredDistance = 20f;

		public float tightness = 1000f;
		public float damping = 200f;

		private const int MaxNeighbours = 12;

		public static void Connect(Atom start, Atom end)
		{
			if (start.neighbours.Contains(end) || start == end)
				return;

			var connection = pool.GetObject();

			start.neighbours.Add(end);
			end.neighbours.Add(start);
			
			start.connections.Add(connection);
			end.connections.Add(connection);

			connection.a = start;
			connection.b = end;
		}

		public static void Disconnect(Connection connection)
		{
			connection.a.neighbours.Remove(connection.b);
			connection.b.neighbours.Remove(connection.a);

			connection.a.connections.Remove(connection);
			connection.b.connections.Remove(connection);

			connection.a = null;
			connection.b = null;

			pool.PutObject(connection);
		}

		public static void DisconnectAllNeighbours(Atom atom)
		{
			var tempConnections = atom.connections.ToArray();

			for (var index = 0; index < tempConnections.Length; index++)
			{
				Disconnect(tempConnections[index]);
			}
		}

		public void Update(float dt)
		{
			if (a == null)
				return;

			var distance = (Vector2)(a.transform.position - b.transform.position);
			distance -= distance.normalized * preferredDistance;
			
			var force = -distance * tightness - (damping * (a.rigidbody2D.velocity - b.rigidbody2D.velocity));

			a.rigidbody2D.AddForce(force * dt, ForceMode2D.Force);
			b.rigidbody2D.AddForce(force * (dt * -1f), ForceMode2D.Force);

			Debug.DrawLine(a.transform.position, b.transform.position, Color.black, dt);
			/*
			Vector2 rayDirection = (b.transform.position - a.transform.position);
			Debug.DrawRay(a.transform.position, rayDirection, Color.black, dt);
			//*/

			/*
			collider.enabled = false;
			if (drawBridges)
			{


				var hitInfo = Physics2D.Raycast(transform.position, rayDirection);

				if (hitInfo)
				{

					var atomComponent = hitInfo.collider.GetComponent<Atom>();
					if (atomComponent != null && atomComponent == atom)
					{


					}
				}

			}
			collider.enabled = true;
			*/
		}

	}
}

