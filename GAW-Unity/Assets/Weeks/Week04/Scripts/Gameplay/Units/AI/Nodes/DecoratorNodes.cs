
using System;

namespace Week04
{
	namespace BehaviourTree
	{
		public abstract class DecoratorNode<StateType> : Node<StateType>, IDecoratorNode where StateType : BaseNodeState, new()
		{
			public INode childNode;

			protected DecoratorNode(INode childNode)
			{
				this.childNode = childNode;
			}

			public override void Initialize(BehaviorContext context)
			{
				base.Initialize(context);
				childNode.Initialize(context);
			}

			public override void Cleanup(BehaviorContext context)
			{
				base.Cleanup(context);
				childNode.Cleanup(context);
			}

			public INode getChildNode()
			{
				return childNode;
			}
		}

		public abstract class DecoratorNode : DecoratorNode<BaseNodeState>
		{
			protected DecoratorNode(INode childNode) : base(childNode)
			{
			}
		}

		public class EntryNode : DecoratorNode
		{
			//public Node firstNode;


			public EntryNode(INode firstNode) : base(firstNode)
			{
				this.childNode = firstNode;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				throw new NotImplementedException();
			}

			private BehaviourStatus _prevStatus;

			public void UpdateTree(float dt, BehaviorContext context)
			{
				context.timeLeft = dt;

				if (_prevStatus != BehaviourStatus.Running)
					childNode.Initialize(context);

				var result = childNode.Update(context);

				if (result != BehaviourStatus.Running)
					childNode.Cleanup(context);

				_prevStatus = result;
			}
		}

		public class InverterDecoratorNode : DecoratorNode
		{
			public InverterDecoratorNode(INode childNode) : base(childNode)
			{
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var result = childNode.Update(context);

				switch (result)
				{
					case BehaviourStatus.Success:
						return BehaviourStatus.Failure;
					case BehaviourStatus.Failure:
						return BehaviourStatus.Success;
					case BehaviourStatus.Running:
						return BehaviourStatus.Running;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		public class SucceederDecoratorNode : DecoratorNode
		{
			public SucceederDecoratorNode(INode childNode) : base(childNode)
			{
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				childNode.Update(context);
				return BehaviourStatus.Success;
			}
		}

		public class RepeatUntilFailDecoratorNode : DecoratorNode
		{
			public RepeatUntilFailDecoratorNode(INode childNode) : base(childNode)
			{
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var result = childNode.Update(context);

				if (result == BehaviourStatus.Failure)
					return BehaviourStatus.Success;

				return BehaviourStatus.Running;

			}
		}
	}
}
