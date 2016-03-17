
namespace Week04
{
	namespace BehaviorTree
	{
	
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

	}
}
