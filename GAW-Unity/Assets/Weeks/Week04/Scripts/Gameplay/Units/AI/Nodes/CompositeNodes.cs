using System;
using UnityEngine;
using System.Collections.Generic;

namespace Week04
{
	namespace BehaviourTree
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

		public abstract class CompositeNode : CompositeNode<BaseNodeState>, ICompositeNode
		{
			protected CompositeNode(INode[] childNodes) : base(childNodes)
			{
			}
		}

		public class SequenceCompositeNode : CompositeNode<IteratorNodeState>
		{
			public SequenceCompositeNode(params INode[] childNodes) : base(childNodes)
			{
			}

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

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var state = context.GetState<IteratorNodeState>(this);
				state.currentIndex = -1;

				if (state.currentIndex >= 0)
				{
					var result = childNodes[state.currentIndex].Update(context);

					switch (result)
					{
						case BehaviourStatus.Success:
							childNodes[state.currentIndex].Cleanup(context);
							break;
						case BehaviourStatus.Failure:
							childNodes[state.currentIndex].Cleanup(context);
							return BehaviourStatus.Failure;
						case BehaviourStatus.Running:
							return BehaviourStatus.Running;
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
						case BehaviourStatus.Success:
							childNodes[index].Cleanup(context);
							break;
						case BehaviourStatus.Failure:
							childNodes[index].Cleanup(context);

							return BehaviourStatus.Failure;
						case BehaviourStatus.Running:
							state.currentIndex = index;

							return BehaviourStatus.Running;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				return BehaviourStatus.Success;
			}
		}

		public class SelectorCompositeNode : CompositeNode<IteratorNodeState>
		{
			public SelectorCompositeNode(params INode[] childNodes) : base(childNodes)
			{
			}

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

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var state = context.GetState<IteratorNodeState>(this);

				if (state.currentIndex >= 0)
				{
					var result = childNodes[state.currentIndex].Update(context);

					switch (result)
					{
						case BehaviourStatus.Success:
							return BehaviourStatus.Success;
						case BehaviourStatus.Failure:
							break;
						case BehaviourStatus.Running:
							return BehaviourStatus.Running;
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
					var childNode = childNodes[index];
					var result = childNode.Update(context);

					switch (result)
					{
						case BehaviourStatus.Success:
							return BehaviourStatus.Success;
						case BehaviourStatus.Failure:
							break;
						case BehaviourStatus.Running:
							return BehaviourStatus.Running;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				return BehaviourStatus.Failure;
			}
		}

	}


}
