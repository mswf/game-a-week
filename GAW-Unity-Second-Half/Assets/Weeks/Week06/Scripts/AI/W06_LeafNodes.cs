using UnityEngine;
using System.Collections;

using BehaviorTree;

using ContextIndex = System.String;
using Stack_Object = System.Collections.Generic.Stack<System.Object>;

namespace Week06.BehaviorTree
{
	public class FindVisualClues : LeafNode
	{
		public readonly string _subjectVar;
		public readonly string _targetStack;

		public FindVisualClues(ContextIndex subjectVar, ContextIndex targetStack)
		{
			this._subjectVar = subjectVar;
			this._targetStack = targetStack;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var unit = (Unit) context[_subjectVar];
			var targetStack = (Stack_Object) context[_targetStack];

			var potentialClues = unit.GetPotentialClues();

			if (targetStack.Count == 0)
			{
				targetStack = new Stack_Object(potentialClues);
				context[_targetStack] = targetStack;
			}
			else
			{
				for (var i = 0; i < potentialClues.Length; i++)
				{
					targetStack.Push(potentialClues[i]);
				}
			}

			return BehaviorStatus.Success;
		}


		public override float GetGUIPropertyHeight()
		{
			return 4f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.Label("Subject:");
			GUILayout.TextField(_subjectVar.ToString());
			GUILayout.Label("Stack:");
			GUILayout.TextField(_targetStack.ToString());
		}
	}


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
