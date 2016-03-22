using UnityEngine;

namespace BehaviorTree
{
	public abstract class BehaviorStateMonobehavior : MonoBehaviour 
	{
		public abstract BehaviorState GetBehaviorState();
	}
}
