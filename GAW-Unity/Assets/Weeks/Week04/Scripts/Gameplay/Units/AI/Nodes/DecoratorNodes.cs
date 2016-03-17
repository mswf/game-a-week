
using System;

namespace Week04
{
	namespace BehaviorTree
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
			{}
		}

		public class EntryNode : DecoratorNode
		{
			// TODO: Store this state in the context
			private BehaviorStatus _prevStatus;

			public EntryNode(INode firstNode) : base(firstNode)
			{
				this.childNode = firstNode;
			}

			public override BehaviorStatus UpdateTick(BehaviorContext context)
			{
				throw new NotImplementedException();
			}

			public void UpdateTree(float dt, BehaviorContext context)
			{
				context.timeLeft = dt;

				if (_prevStatus != BehaviorStatus.Running)
					childNode.Initialize(context);

				var result = childNode.Update(context);

				if (result != BehaviorStatus.Running)
					childNode.Cleanup(context);

				_prevStatus = result;
			}

		}

		public class InverterDecoratorNode : DecoratorNode
		{
			public InverterDecoratorNode(INode childNode) : base(childNode)
			{}

			public override BehaviorStatus UpdateTick(BehaviorContext context)
			{
				var result = childNode.Update(context);

				switch (result)
				{
					case BehaviorStatus.Success:
						return BehaviorStatus.Failure;
					case BehaviorStatus.Failure:
						return BehaviorStatus.Success;
					case BehaviorStatus.Running:
						return BehaviorStatus.Running;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

		}

		public class SucceederDecoratorNode : DecoratorNode
		{
			public SucceederDecoratorNode(INode childNode) : base(childNode)
			{}

			public override BehaviorStatus UpdateTick(BehaviorContext context)
			{
				childNode.Update(context);
				return BehaviorStatus.Success;
			}

		}

		public class FailerDecoratorNode : DecoratorNode
		{
			public FailerDecoratorNode(INode childNode) : base(childNode)
			{}

			public override BehaviorStatus UpdateTick(BehaviorContext context)
			{
				childNode.Update(context);
				return BehaviorStatus.Failure;
			}

		}

		public class RepeatUntilFailDecoratorNode : DecoratorNode
		{
			public RepeatUntilFailDecoratorNode(INode childNode) : base(childNode)
			{}

			public override BehaviorStatus UpdateTick(BehaviorContext context)
			{
				var result = BehaviorStatus.Running;

				while (result != BehaviorStatus.Failure)
				{
					if (result == BehaviorStatus.Success)
						childNode.Cleanup(context);

					result = childNode.Update(context);
				}

				return BehaviorStatus.Success;
			}

		}
	}
}
