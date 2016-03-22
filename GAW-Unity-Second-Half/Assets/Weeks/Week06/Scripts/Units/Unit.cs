using UnityEngine;
using System.Collections;

using BehaviorTree;

namespace Week06
{
	public class Unit : BaseUnit 
	{
		public const string TREE_UNIT = "TREE_UNIT";


		protected override void InitializeBehaviorTree(BehaviorState bState)
		{
			if (bState.IsTreeDefined(TREE_UNIT) == false)
			{
				_behaviorTreeEntry = bState.AddTree(TREE_UNIT,

				new EntryNode(
					new StubNode("GHello, I'm a unit")
				)


				);
			}
			else
			{
				_behaviorTreeEntry = bState.GetTree(TREE_UNIT);
			}
		}
	}
}
