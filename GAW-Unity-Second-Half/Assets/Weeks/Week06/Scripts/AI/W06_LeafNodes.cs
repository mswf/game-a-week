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

			if (potentialClues.Length == 0)
				return BehaviorStatus.Failure;


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


	
	public class IsClueWorthInvestigating : LeafNode
	{
		// Is it's type within threshold of the squad's alert level 
			// -> Don't investigate a broken twig if we're at ease
		public readonly ContextIndex _clueVar;

		public IsClueWorthInvestigating(ContextIndex clueVar)
		{
			this._clueVar = clueVar;

		}
		
		// Is it still fresh enough (a sound fades down to a level)
		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			return BehaviorStatus.Success;
		}
	}
	
	public class CanReach : LeafNode
	{
		public readonly ContextIndex _subjectVar;
		public readonly ContextIndex _targetVar;

		public CanReach (ContextIndex subjectVar, ContextIndex targetVar)
		{
			this._subjectVar = subjectVar;
			this._targetVar = targetVar;
		}

		// (Unit, MonoBehavior)
		// Can unit with pathfinder reach the monobehavior?
		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var subject = (BaseUnit) context[_subjectVar];
			var target = (MonoBehaviour) context[_targetVar];

			if (subject.CanReach(target.transform.position))
			{
				return BehaviorStatus.Success;	
			}

			return BehaviorStatus.Failure;
		}
	}

	public class MoveTo : LeafNode
	{
		public readonly ContextIndex _subjectVar;
		public readonly ContextIndex _targetVar;

		public MoveTo(ContextIndex subjectVar, ContextIndex targetVar)
		{
			this._subjectVar = subjectVar;
			this._targetVar = targetVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			//return BehaviorStatus.Success;
			

			var subject = (BaseUnit)context[_subjectVar];
			var target = (MonoBehaviour)context[_targetVar];

			subject.navMeshAgent.destination = target.transform.position;

			subject.navMeshAgent.Resume();

			return BehaviorStatus.Success;
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
