using UnityEngine;
using System.Collections;
using BehaviorTree;

namespace Week06
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class Squad : BaseUnit 
	{
		public override void Initialize(GameController gameController)
		{
			base.Initialize(gameController);
		}

		public const string TREE_SQUAD = "TREE_SQUAD";

		protected override void InitializeBehaviorTree(BehaviorState bState)
		{
			if (bState.IsTreeDefined(TREE_SQUAD) == false)
			{
				_behaviorTreeEntry = bState.AddTree(TREE_SQUAD,

				new EntryNode(
					new StubNode("GHello, I'm a squad")
				)


				);
			}
			else
			{
				_behaviorTreeEntry = bState.GetTree(TREE_SQUAD);
			}
		}

	}
}
