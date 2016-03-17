
using System;

namespace Week04
{
	namespace BehaviorTree
	{
		public abstract class CompositeNode<StateType> : Node<StateType>, ICompositeNode where StateType : BaseNodeState, new()
		{
			public INode[] childNodes;

			protected CompositeNode(params INode[] childNodes)
			{
				this.childNodes = childNodes;
			}

			public INode[] getChildNodes()
			{
				return childNodes;
			}
		}

		public abstract class CompositeNode : CompositeNode<BaseNodeState>
		{
			protected CompositeNode(INode[] childNodes) : base(childNodes) 
			{}
		}

		public class SequenceCompositeNode : CompositeNode<IteratorNodeState>
		{
			public SequenceCompositeNode(params INode[] childNodes) : base(childNodes)
			{}

			public override void Initialize(BehaviorContext context)
			{
				base.Initialize(context);

				var state = context.GetState<IteratorNodeState>(this);
				state.currentIndex = -1;
			}

			public override void Cleanup(BehaviorContext context)
			{
				base.Cleanup(context);

				var state = context.GetState<IteratorNodeState>(this);
				state.currentIndex = -1;
			}

			public override BehaviorStatus UpdateTick(BehaviorContext context)
			{
				var state = context.GetState<IteratorNodeState>(this);
				
				if (state.currentIndex >= 0)
				{
					var result = childNodes[state.currentIndex].Update(context);
					switch (result)
					{
						case BehaviorStatus.Success:
							childNodes[state.currentIndex].Cleanup(context);
							state.currentIndex++;
							break;
						case BehaviorStatus.Failure:
							childNodes[state.currentIndex].Cleanup(context);
							return BehaviorStatus.Failure;
						case BehaviorStatus.Running:
							return BehaviorStatus.Running;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else
				{
					state.currentIndex = 0;
				}

				for (int index = state.currentIndex; index < childNodes.Length; index++)
				{
					childNodes[index].Initialize(context);

					var result = childNodes[index].Update(context);
					switch (result)
					{
						case BehaviorStatus.Success:
							childNodes[index].Cleanup(context);
							break;
						case BehaviorStatus.Failure:
							childNodes[index].Cleanup(context);

							return BehaviorStatus.Failure;
						case BehaviorStatus.Running:
							state.currentIndex = index;

							return BehaviorStatus.Running;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				return BehaviorStatus.Success;
			}
		}

		public class SelectorCompositeNode : CompositeNode<IteratorNodeState>
		{
			public SelectorCompositeNode(params INode[] childNodes) : base(childNodes)
			{}

			public override void Initialize(BehaviorContext context)
			{
				base.Initialize(context);

				var state = context.GetState<IteratorNodeState>(this);
				state.currentIndex = -1;
			}

			public override void Cleanup(BehaviorContext context)
			{
				base.Cleanup(context);

				var state = context.GetState<IteratorNodeState>(this);
				state.currentIndex = -1;
			}

			public override BehaviorStatus UpdateTick(BehaviorContext context)
			{
				var state = context.GetState<IteratorNodeState>(this);

				if (state.currentIndex >= 0)
				{
					var result = childNodes[state.currentIndex].Update(context);
					switch (result)
					{
						case BehaviorStatus.Success:
							childNodes[state.currentIndex].Cleanup(context);
							return BehaviorStatus.Success;
						case BehaviorStatus.Failure:
							childNodes[state.currentIndex].Cleanup(context);
							state.currentIndex++;
							break;
						case BehaviorStatus.Running:
							return BehaviorStatus.Running;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else
				{
					state.currentIndex = 0;
				}

				for (int index = state.currentIndex; index < childNodes.Length; index++)
				{
					childNodes[index].Initialize(context);

					var result = childNodes[index].Update(context);
					switch (result)
					{
						case BehaviorStatus.Success:
							childNodes[index].Cleanup(context);
							return BehaviorStatus.Success;
						case BehaviorStatus.Failure:
							childNodes[index].Cleanup(context);
							break;
						case BehaviorStatus.Running:
							state.currentIndex = index;
							return BehaviorStatus.Running;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				return BehaviorStatus.Failure;
			}
		}


	}
}
