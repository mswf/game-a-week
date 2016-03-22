using UnityEngine;
using System.Collections;

using BehaviorTree;
using Week06.BehaviorTree;

namespace Week06
{
	public class Unit : BaseUnit 
	{
		public const string TREE_UNIT = "TREE_UNIT";

		public Squad squad;

		protected override void InitializeBehaviorTree(BehaviorState bState)
		{
			if (bState.IsTreeDefined(TREE_UNIT) == false)
			{
				_behaviorTreeEntry = bState.AddTree(TREE_UNIT,

				new EntryNode(
					new FollowSquadNode(U_SUBJECT)
				)


				);
			}
			else
			{
				_behaviorTreeEntry = bState.GetTree(TREE_UNIT);
			}
		}

		public void MoveWithSquad()
		{
			navMeshAgent.destination = squad._transform.position;
		}
	}
}
