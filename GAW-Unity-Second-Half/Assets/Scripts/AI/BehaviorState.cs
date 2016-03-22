

using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
	public class BehaviorState
	{
		public static BehaviorState GetBehaviorStateInScene()
		{
			var behaviorStateHolder = GameObject.FindObjectOfType<BehaviorStateMonobehavior>();

			return behaviorStateHolder.GetBehaviorState();
		}
		
		public Dictionary<string, WeakReferenceT<INode>> behaviorTrees;
		public List<WeakReferenceT<BehaviorContext>> behaviorContexts;

		public BehaviorState()
		{
			behaviorTrees = new Dictionary<string, WeakReferenceT<INode>>();
			behaviorContexts = new List<WeakReferenceT<BehaviorContext>>();
		}

		public BehaviorContext GetNewContext()
		{
			var newContext = new BehaviorContext();

			behaviorContexts.Add(new WeakReferenceT<BehaviorContext>(newContext));

			return newContext;
		}

		public bool IsTreeDefined(string behaviorTreeKey)
		{
			if (behaviorTrees.ContainsKey(behaviorTreeKey))
			{
				return behaviorTrees[behaviorTreeKey].IsAlive;
			}

			return false;
		}

		public INode GetTree(string behaviorTreeKey)
		{
			return behaviorTrees[behaviorTreeKey].Target;
		}

		public INode AddTree(string behaviorTreeKey, INode behaviorTree)
		{
			if (behaviorTrees.ContainsKey(behaviorTreeKey))
			{
				if (behaviorTrees[behaviorTreeKey].IsAlive == false)
				{
					behaviorTrees[behaviorTreeKey] = new WeakReferenceT<INode>(behaviorTree);
					behaviorTree.SetBehaviorState(this);
				}
				else
				{
					Debug.LogError("Tried add tree that already exists with key: " + behaviorTreeKey);
				}
			}
			else
			{
				behaviorTrees.Add(behaviorTreeKey, new WeakReferenceT<INode>(behaviorTree));
			}

			return behaviorTree;
		}
	}

	public class BaseNodeState
	{
		public BehaviorStatus previousStatus;
		public float timeSinceStatusChange;

		public int timesCalled;

		public int timesSuccess;
		public int timesFailure;
		public int timesRunning;

	}

	public class IteratorNodeState : BaseNodeState
	{
		public int currentIndex;
	}

	public class EntryNodeState : BaseNodeState
	{
		public BehaviorStatus previousEntryStatus;

	}

}
