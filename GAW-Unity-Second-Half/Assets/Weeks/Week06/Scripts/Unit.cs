using UnityEngine;
using System.Collections;

namespace Week06
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class Unit : MonoBehaviour
	{

		public Transform navigationTarget;

		protected NavMeshAgent navMeshAgent;

		public NavMeshPath currentPath;

		// Use this for early referencing
		private void Awake()
		{
			navMeshAgent = GetComponent<NavMeshAgent>();
			currentPath = new NavMeshPath();
		}

		// Use this for initialization
		private void Start () 
		{
			
		}
		
		// Update is called once per frame
		private void Update ()
		{
			var dt = Time.deltaTime;

			if (Input.GetKeyDown(KeyCode.R))
			{
				Debug.Log("Calculating new path:");
			//	var result = 

				

				//Debug.Log(result);
			}
			
			navMeshAgent.CalculatePath(navigationTarget.position, currentPath);

			var path = currentPath.corners;

			for (int i = 0; i < path.Length; i++)
			{
				DebugExtension.DebugPoint(path[i], Color.black, 1f, dt);
	//			DebugExtension.DebugArrow(path[i], path[i+1], dt);
			}
		//	if (path.Length > 0)
			//	DebugExtension.DebugArrow(path[path.Length-1], navigationTarget.position, dt);


		}
	}
}
