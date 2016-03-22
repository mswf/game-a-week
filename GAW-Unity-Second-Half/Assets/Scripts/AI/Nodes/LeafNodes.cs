
//#define SAFE_MODE

using UnityEngine;

using ContextIndex = System.String;
using Stack_Object = System.Collections.Generic.Stack<System.Object>;


namespace BehaviorTree
{
	public abstract class LeafNode<StateType> : Node<StateType>, ILeafNode where StateType : BaseNodeState, new()
	{
		public override void SetBehaviorState(BehaviorState behaviorState)
		{
			this.behaviorState = behaviorState;
		}
	}

	public abstract class LeafNode : LeafNode<BaseNodeState>
	{}


	public class PrintNode : LeafNode
	{
		private readonly string _messageToPrint;

		public PrintNode(string messageToPrint)
		{
			this._messageToPrint = messageToPrint;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			Debug.Log(_messageToPrint);

			return BehaviorStatus.Success;
		}

		public override float GetGUIPropertyHeight()
		{
			return 1f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.TextField(_messageToPrint.ToString());
		}
	}

	public class PrintVarNode : LeafNode
	{
		private readonly ContextIndex _varToPrint;

		public PrintVarNode(ContextIndex varToPrint)
		{
			this._varToPrint = varToPrint;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var toPrint = context[_varToPrint];
			if (toPrint == null)
				Debug.Log("null");
			else
				Debug.Log(toPrint.ToString());

			return BehaviorStatus.Success;
		}

		public override float GetGUIPropertyHeight()
		{
			return 1f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.TextField(_varToPrint.ToString());
		}
	}

	public class PushToStack : LeafNode
	{
		private readonly ContextIndex _stackVar;
		private readonly ContextIndex _itemVar;

		public PushToStack(ContextIndex stackVar, ContextIndex itemVar)
		{
			this._stackVar = stackVar;
			this._itemVar = itemVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var stack = context[_stackVar] as Stack_Object;

			if (stack == null)
			{
				stack = new Stack_Object();
				context[_stackVar] = stack;
			}

			var objectToPush = context[_itemVar];
			if (objectToPush == null)
			{
				Debug.Log("Tried to push nonexistent var '" + _itemVar + "' to stack '" + _stackVar + "'");
				return BehaviorStatus.Success;
			}

			stack.Push(objectToPush);

			return BehaviorStatus.Success;
		}

		public override float GetGUIPropertyHeight()
		{
			return 3f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.TextField(_stackVar.ToString());
			GUILayout.Label("Target:");
			GUILayout.TextField(_itemVar.ToString());
		}
	}

	public class PopFromStack : LeafNode
	{
		private readonly ContextIndex _stackVar;
		private readonly ContextIndex _itemVar;

		public PopFromStack(ContextIndex stackVar, ContextIndex itemVar)
		{
			this._stackVar = stackVar;
			this._itemVar = itemVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var stack = context[_stackVar] as Stack_Object;

			if (stack == null || stack.Count == 0)
			{
				return BehaviorStatus.Failure;
			}

			var objectToPop = stack.Pop();
			if (objectToPop == null)
			{
				Debug.Log("Tried to pop nonexistent var '" + _itemVar + "' to stack '" + _stackVar + "'");
				return BehaviorStatus.Failure;
			}

			context[_itemVar] = objectToPop;
			return BehaviorStatus.Success;
		}

		public override float GetGUIPropertyHeight()
		{
			return 3f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.TextField(_stackVar.ToString());
			GUILayout.Label("Target:");
			GUILayout.TextField(_itemVar.ToString());
		}
	}

	public class IsStackEmpty : LeafNode
	{
		private readonly ContextIndex _stackVar;

		public IsStackEmpty(ContextIndex stackVar)
		{
			this._stackVar = stackVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var stack = context[_stackVar] as Stack_Object;

			if (stack == null)
				return BehaviorStatus.Success;

			if (stack.Count == 0)
				return BehaviorStatus.Success;
			
			return BehaviorStatus.Failure;
		}

		public override float GetGUIPropertyHeight()
		{
			return 1f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.TextField(_stackVar.ToString());
		}
	}

	public class ClearStack : LeafNode
	{
		private readonly ContextIndex _stackVar;

		public ClearStack(ContextIndex stackVar)
		{
			this._stackVar = stackVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var stack = context[_stackVar] as Stack_Object;

			if (stack == null)
				return BehaviorStatus.Success;

			stack.Clear();

			return BehaviorStatus.Success;
		}

		public override float GetGUIPropertyHeight()
		{
			return 1f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.TextField(_stackVar.ToString());
		}

	}
	/*
	public class ContainsUnit : LeafNode
	{
		private ContextIndex _unitVar;

		public ContainsUnit(ContextIndex unitVar)
		{
			this._unitVar = unitVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var unit = context[_unitVar] as BaseUnit;

			if (unit == null)
				return BehaviorStatus.Failure;

			return BehaviorStatus.Success;
		}

		public override float GetGUIPropertyHeight()
		{
			return 2f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.Label("Subject:");
			GUILayout.TextField(_unitVar.ToString());
		}

	}

	public abstract class UnitToUnitInteraction : LeafNode
	{
		protected ContextIndex _subjectVar;
		protected ContextIndex _targetVar;

		public UnitToUnitInteraction(ContextIndex subjectVar, ContextIndex targetVar)
		{
			this._subjectVar = subjectVar;
			this._targetVar = targetVar;
		}

		public override float GetGUIPropertyHeight()
		{
			return 4f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.Label("Subject:");
			GUILayout.TextField(_subjectVar.ToString());
			GUILayout.Label("Target:");
			GUILayout.TextField(_targetVar.ToString());
		}

	}

	public class CanTargetUnit : UnitToUnitInteraction
	{
		public CanTargetUnit(ContextIndex subjectVar, ContextIndex targetVar) : base(subjectVar, targetVar) { }

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var subjectUnit = context[_subjectVar] as BaseUnit;
			var targetUnit = context[_targetVar] as BaseUnit;

			if (subjectUnit == null || targetUnit == null)
				return BehaviorStatus.Failure;

			if (subjectUnit.CanTarget(targetUnit))
				return BehaviorStatus.Success;

			return BehaviorStatus.Failure;
		}

	}

	public class ShouldTargetUnit : UnitToUnitInteraction
	{
		public ShouldTargetUnit(ContextIndex subjectVar, ContextIndex targetVar) : base(subjectVar, targetVar) { }

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var subjectUnit = context[_subjectVar] as BaseUnit;
			var targetUnit = context[_targetVar] as BaseUnit;

			if (subjectUnit == null || targetUnit == null)
				return BehaviorStatus.Failure;

			if (subjectUnit.ShouldTarget(targetUnit))
				return BehaviorStatus.Success;

			return BehaviorStatus.Failure;
		}

	}

	public class MoveToUnit : UnitToUnitInteraction
	{
		public MoveToUnit(ContextIndex subjectVar, ContextIndex targetVar) : base(subjectVar, targetVar)
		{}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var subjectUnit = context[_subjectVar] as BaseUnit;
			var targetUnit = context[_targetVar] as BaseUnit;

			if (subjectUnit == null || targetUnit == null)
				return BehaviorStatus.Failure;

			var subjectPosition = subjectUnit.GetUnitPositionX();
			var targetPosition = targetUnit.GetUnitPositionX();

			var distanceToTarget = targetPosition - subjectPosition;
			var signedDistance = Mathf.Sign(distanceToTarget);
			var maxTravelDistance = subjectUnit.movementSpeed * context.timeLeft;

			if (distanceToTarget * signedDistance > maxTravelDistance)
			{
				subjectUnit.MoveUnitPosition(maxTravelDistance * signedDistance);
				context.timeLeft = 0f;
				return BehaviorStatus.Running;
			}

			var fraction = (distanceToTarget * signedDistance) / maxTravelDistance;
			context.timeLeft -= context.timeLeft * fraction;
			return BehaviorStatus.Success;
		}

	}

	public class AttackUnit : UnitToUnitInteraction
	{
		public AttackUnit(ContextIndex subjectVar, ContextIndex targetVar) : base(subjectVar, targetVar)
		{}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var subjectUnit = context[_subjectVar] as BaseUnit;
			var targetUnit = context[_targetVar] as BaseUnit;

			if (subjectUnit == null || targetUnit == null)
				return BehaviorStatus.Failure;

			if (subjectUnit.IsReadyForAttack() == false)
				return BehaviorStatus.Running;

			DebugExtension.DebugArrow(subjectUnit.GetUnitPosition(), 
					new Vector3(targetUnit.GetUnitPositionX() - subjectUnit.GetUnitPositionX(), 0f), Color.red, 1f, false);

			subjectUnit.AttackUnit(targetUnit);

			return BehaviorStatus.Success;
		}
	}

	public class MoveNode : LeafNode
	{
		private readonly ContextIndex _subjectVar;

		public MoveNode(ContextIndex subjectVar)
		{
			this._subjectVar = subjectVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var subjectUnit = context[_subjectVar] as BaseUnit;

			if (subjectUnit == null)
				return BehaviorStatus.Failure;

			if (Mathf.Abs(context.timeLeft) < Mathf.Epsilon)
				return BehaviorStatus.Failure;

			subjectUnit.MoveUnitPosition(subjectUnit.movementSpeed * context.timeLeft);
			context.timeLeft = 0f;

			return BehaviorStatus.Success;
		}
	}

	public class IsUnitInRange : UnitToUnitInteraction
	{
		public IsUnitInRange(string subjectVar, string targetVar) : base(subjectVar, targetVar) { }

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var subjectUnit = context[_subjectVar] as BaseUnit;
			var targetUnit = context[_targetVar] as BaseUnit;

			if (subjectUnit == null || targetUnit == null)
				return BehaviorStatus.Failure;


			if (subjectUnit.IsInRange(targetUnit))
				return BehaviorStatus.Success;

			return BehaviorStatus.Failure;
		}

	}

	public class FindTargets : LeafNode
	{
		private readonly ContextIndex _subjectVar;
		private readonly ContextIndex _targetStack;

		private readonly ContextIndex _rangeVar;
		private readonly float? _range;

		public FindTargets(ContextIndex subjectVar, ContextIndex targetStack, float range)
		{
			this._subjectVar = subjectVar;
			this._targetStack = targetStack;
			this._range = range;
		}

		public FindTargets(ContextIndex subjectVar, ContextIndex targetStack, ContextIndex rangeVar)
		{
			this._subjectVar = subjectVar;
			this._targetStack = targetStack;

			this._rangeVar = rangeVar;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			float range;
			if (_range.HasValue)
				range = _range.Value;
			else
				range = (context[_rangeVar] as float?).GetValueOrDefault(5f);

			var unit = context[_subjectVar] as BaseUnit;

#if SAFE_MODE
			if (unit == null)
			{
				Debug.Log("Unit with key " + _subjectVar.ToString() + " does not exist as an unit");
				return BehaviourStatus.Failure;
			}
#endif

			var targets = unit.GetUnitsWithinCircularRange(range);

			if (targets.Length == 0)
				return BehaviorStatus.Failure;

			var stack = context[_targetStack] as Stack_Object;

			if (stack == null || stack.Count == 0)
			{
				stack = new Stack_Object(targets);
				context[_targetStack] = stack;
			}
			else
			{
				for (var i = 0; i < targets.Length; i++)
				{
					stack.Push(targets[i]);
				}
			}

			return BehaviorStatus.Success;
		}


		public override float GetGUIPropertyHeight()
		{
			return 6f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.Label("Subject:");
			GUILayout.TextField(_subjectVar.ToString());
			GUILayout.Label("Stack:");
			GUILayout.TextField(_targetStack.ToString());
			GUILayout.Label("Range:");
			if (_range.HasValue)
				GUILayout.TextField(_range.Value.ToString());
			else
				GUILayout.TextField(_rangeVar.ToString());
		}
	}
	*/
	public class IsNullNode : LeafNode
	{
		private readonly ContextIndex _variable;

		public IsNullNode(ContextIndex variable)
		{
			this._variable = variable;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var variableToCheck = context[_variable];

			if (variableToCheck == null)
				return BehaviorStatus.Success;

			return BehaviorStatus.Failure;
		}

		public override float GetGUIPropertyHeight()
		{
			return 1f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.TextField(_variable.ToString());
		}

	}

	public class SetToNullNode : LeafNode
	{
		private readonly ContextIndex _variable;

		public SetToNullNode(ContextIndex variable)
		{
			this._variable = variable;
		}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			context[_variable] = null;

			return BehaviorStatus.Success;
		}

		public override float GetGUIPropertyHeight()
		{
			return 1f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.TextField(_variable.ToString());
		}

	}

	public abstract class MathNode<T> : LeafNode
	{
		protected static readonly System.Type ContextIndexType = typeof(ContextIndex);

		protected static readonly System.Type FloatType = typeof(ContextIndex);
		protected static readonly System.Type DoubleType = typeof(ContextIndex);

		protected readonly ContextIndex _targetVar;
		protected readonly T _paramVar;

		public MathNode(ContextIndex targetVar, T paramVar)
		{
			this._targetVar = targetVar;
			this._paramVar = paramVar;
		}
	}

	public class SetVarTo<T> : MathNode<T>
	{
		public SetVarTo(ContextIndex targetVar, T paramVar) : base(targetVar, paramVar)
		{}

		public override BehaviorStatus UpdateTick(BehaviorContext context)
		{
			var type = typeof(T);

			if (type == ContextIndexType)
			{
				context[_targetVar] = context[_paramVar as ContextIndex];
			}
			else
			{
				context[_targetVar] = _paramVar;
			}

			return BehaviorStatus.Success;
		}

		public override float GetGUIPropertyHeight()
		{
			return 4f * DefaultPropertyHeight;
		}

		public override void DrawGUI(int windowID)
		{
			GUILayout.Label("Target:");
			GUILayout.TextField(_targetVar.ToString());

			GUILayout.Label("Param:");
			GUILayout.TextField(_paramVar.ToString());
		}

	}
}

