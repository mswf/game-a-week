using UnityEngine;
using System.Collections;
using BehaviorTree;

namespace Week06
{
	[RequireComponent(typeof(NavMeshAgent))]
	public class Squad : BaseUnit
	{
		public Unit[] squadMembers;

		public override void Initialize(GameController gameController)
		{
			base.Initialize(gameController);

			for (int i = 0; i < squadMembers.Length; i++)
			{
				squadMembers[i].squad = this;
			}
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
