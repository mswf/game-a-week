using UnityEngine;
using System.Collections;

namespace Week04
{
	namespace BehaviourTree
	{
	
		public class BaseNodeState
		{
			public BehaviourStatus previousStatus;
			public float timeSinceStatusChange;


		}

		public class IteratorNodeState : BaseNodeState
		{
			public int currentIndex;
		}

	}
}
