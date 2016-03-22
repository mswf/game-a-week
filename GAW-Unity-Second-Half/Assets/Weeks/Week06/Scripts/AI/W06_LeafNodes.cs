using UnityEngine;
using System.Collections;

using BehaviorTree;

using ContextIndex = System.String;

namespace Week06.BehaviorTree
{
	public class FollowSquadNode : LeafNode
	{

		public readonly string _subjectVar;
		public FollowSquadNode(ContextIndex subjectVar)
		{
			this._subjectVar = subjectVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var subjectUnit = (Unit) context[_subjectVar];

			subjectUnit.MoveWithSquad();

			context.timeLeft = 0f;

			return BehaviorStatus.Success;
		}
	}
}
