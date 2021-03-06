﻿using UnityEngine;
using System.Collections;

using BehaviorTree;

namespace Week06
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class BaseUnit : MonoBehaviour
	{

		public Transform navigationTarget;

		public NavMeshAgent navMeshAgent;

		public Transform _transform;
		
		// Use this for early referencing
		protected void Awake()
		{
			navMeshAgent = GetComponent<NavMeshAgent>();

			_transform = GetComponent<Transform>();
		}

		[SerializeField, HideInInspector]
		protected GameController _gameController;

		[SerializeField, HideInInspector]
		protected BehaviorContext _behaviorContext;
		[SerializeField, HideInInspector]
		protected INode _behaviorTreeEntry;

		public const string U_SUBJECT = "U_SUBJECT";

		public const string TREE_BASE_UNIT = "TREE_BASE_UNIT";

		public virtual void Initialize(GameController gameController)
		{
			_gameController = gameController;
			
			var bState = gameController.behaviorState;

			_behaviorContext = bState.GetNewContext();

			InitializeBehaviorContext(_behaviorContext);


			InitializeBehaviorTree(bState);

		}

		protected virtual void InitializeBehaviorTree(BehaviorState bState)
		{
			if (bState.IsTreeDefined(TREE_BASE_UNIT) == false)
			{
				_behaviorTreeEntry = bState.AddTree(TREE_BASE_UNIT,

				new EntryNode(
					new PrintNode("No behavior tree defined")
				)


				);
			}
			else
			{
				_behaviorTreeEntry = bState.GetTree(TREE_BASE_UNIT);
			}
		}

		protected virtual void InitializeBehaviorContext(BehaviorContext behaviorContext)
		{
			behaviorContext[U_SUBJECT] = this;

		}

		// Use this for initialization
		private void Start () 
		{
			
		}

		// Update is called once per frame
		protected void Update ()
		{
			var dt = Time.deltaTime;

			_behaviorContext.timeLeft = dt;
			_behaviorTreeEntry.Update(_behaviorContext);
			
			
					
			//navMeshAgent.CalculatePath(navigationTarget.position, currentPath);
			//navMeshAgent.stoppingDistance = 3f;

			if (Input.GetKey(KeyCode.R))
			{
				Debug.Log("Calculating new path:");
				navMeshAgent.destination = navigationTarget.position;
				
			}


			/*
			var path = currentPath.corners;

			for (int i = 0; i < path.Length-1; i++)
			{
				DebugExtension.DebugArrow(path[i] + Vector3.up, path[i+1] - path[i], dt);
			}
			*/

		}

		public bool CanReach(Vector3 position)
		{
			navMeshAgent.destination = position;

			var status = navMeshAgent.pathStatus;

			navMeshAgent.Stop();

			if (status == NavMeshPathStatus.PathComplete)
				return true;

			return false;
		}
	}
}
