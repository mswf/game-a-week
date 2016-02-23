using UnityEngine;
using System.Collections.Generic;

namespace Week03
{
	public class GameManager : MonoBehaviour
	{

		// Use this for initialization
		void Start()
		{
			connections = Connection.pool.objects.ToArray();
		}

		// Update is called once per frame
		void Update()
		{
			if (connections.Length != Connection.pool.objects.Count)
			{
				connections = Connection.pool.objects.ToArray();
			}
		}

		private Connection[] connections;

		void FixedUpdate()
		{
			float dt = Time.fixedDeltaTime;
			var listCount = connections.Length;

			for (int i = 0; i < listCount; i++)
			{
				connections[i].Update(dt);
			}
		}
	}


}
