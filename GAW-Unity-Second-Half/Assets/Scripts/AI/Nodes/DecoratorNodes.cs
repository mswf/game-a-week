
using System;

using Color = UnityEngine.Color;

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

	public class EntryNode : DecoratorNode<EntryNodeState>
	{
		public EntryNode(INode firstNode, string index) : base(firstNode)
		{
			this.childNode = firstNode;

			if (index != null)
			{
				BehaviorTreeGlobals.behaviorTrees.Add(index, new WeakReferenceT<INode>(this));
			}
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

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var nodeState = context.GetState<EntryNodeState>(this);

			if (nodeState.previousEntryStatus != BehaviorStatus.Running)
				childNode.Initialize(context);

			var result = childNode.Update(context);

			if (result != BehaviorStatus.Running)
				childNode.Cleanup(context);

			nodeState.previousEntryStatus = result;

			return result;
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

	public class EditorRegionDecoratorNode : DecoratorNode
	{
		public readonly Color regionColor;
		public readonly string label;

		public readonly bool startExpanded;

		public EditorRegionDecoratorNode(INode childNode, Color regionColor, string label = "Group", bool startExpanded = true, string treeIndex = null) : base(childNode)
		{
			regionColor.a = 0.5f;
			this.regionColor = regionColor;
			this.label = label;
			this.startExpanded = startExpanded;

			if (treeIndex != null)
			{
				BehaviorTreeGlobals.behaviorTrees.Add(treeIndex, new WeakReferenceT<INode>(this));
			}
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			return childNode.Update(context);
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

