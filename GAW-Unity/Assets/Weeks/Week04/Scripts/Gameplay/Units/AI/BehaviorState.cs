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

			public int timesCalled;

			public int timesSuccess;
			public int timesFailure;
			public int timesRunning;

		}

		public class IteratorNodeState : BaseNodeState
		{
			public int currentIndex;
		}

	}
}
