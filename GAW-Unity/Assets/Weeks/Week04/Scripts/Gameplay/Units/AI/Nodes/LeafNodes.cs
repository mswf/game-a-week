
using UnityEngine;

using ContextIndex = System.String;
using Stack_Object = System.Collections.Generic.Stack<System.Object>;


namespace Week04
{
	namespace BehaviourTree
	{
		public abstract class LeafNode<StateType> : Node<StateType>, ILeafNode where StateType : BaseNodeState, new()
		{
		}

		public abstract class LeafNode : LeafNode<BaseNodeState>
		{
		}


		public class PrintNode : LeafNode
		{
			private string _messageToPrint;

			public PrintNode(string messageToPrint)
			{
				this._messageToPrint = messageToPrint;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				Debug.Log(_messageToPrint);

				return BehaviourStatus.Success;
			}

			public override void DrawGUI(int windowID)
			{
				//			GUI.
			}
		}

		public class PrintVarNode : LeafNode
		{
			private string _varToPrint;

			public PrintVarNode(string varToPrint)
			{
				this._varToPrint = varToPrint;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var toPrint = context[_varToPrint];
				if (toPrint == null)
					Debug.Log("null");
				else
					Debug.Log(toPrint.ToString());

				return BehaviourStatus.Success;
			}
		}

		public class PushToStack : LeafNode
		{
			private readonly ContextIndex _itemVar;
			private readonly ContextIndex _stackVar;


			public PushToStack(ContextIndex itemVar, ContextIndex stackVar)
			{
				this._itemVar = itemVar;
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var stack = context[_stackVar] as Stack_Object;

				if (stack == null)
				{
					stack = new Stack_Object();
					context[_stackVar] = stack;
				}

				var objectToPush = context[_itemVar] as System.Object;
				if (objectToPush == null)
				{
					Debug.Log("Tried to push nonexistent var '" + _itemVar + "' to stack '" + _stackVar + "'");
					return BehaviourStatus.Success;
				}

				stack.Push(objectToPush);

				return BehaviourStatus.Success;
			}
		}

		public class PopFromStack : LeafNode
		{
			private readonly ContextIndex _itemVar;
			private readonly ContextIndex _stackVar;


			public PopFromStack(ContextIndex itemVar, ContextIndex stackVar)
			{
				this._itemVar = itemVar;
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var stack = context[_stackVar] as Stack_Object;

				if (stack == null || stack.Count == 0)
				{
					//	Debug.LogError("Not a stack! Variable: " + _stackVar.ToString());
					return BehaviourStatus.Failure;
				}

				var objectToPop = stack.Pop() as System.Object;
				if (objectToPop == null)
				{
					Debug.Log("Tried to pop nonexistent var '" + _itemVar + "' to stack '" + _stackVar + "'");
					return BehaviourStatus.Failure;
				}

				context[_itemVar] = objectToPop;
				return BehaviourStatus.Success;
			}
		}

		public class IsStackEmpty : LeafNode
		{
			private readonly ContextIndex _stackVar;

			public IsStackEmpty(ContextIndex stackVar)
			{
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var stack = context[_stackVar] as Stack_Object;

				if (stack == null)
				{
					return BehaviourStatus.Success;
				}

				if (stack.Count == 0)
					return BehaviourStatus.Success;
				else
					return BehaviourStatus.Failure;
			}
		}

		public class ClearStack : LeafNode
		{

			private readonly ContextIndex _stackVar;

			public ClearStack(ContextIndex stackVar)
			{
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var stack = context[_stackVar] as Stack_Object;

				if (stack == null)
				{
					return BehaviourStatus.Success;
				}

				stack.Clear();

				return BehaviourStatus.Success;
			}
		}

		public class ContainsUnit : LeafNode
		{
			private ContextIndex _unitVar;

			public ContainsUnit(ContextIndex unitVar)
			{
				this._unitVar = unitVar;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var unit = context[_unitVar] as BaseUnit;

				if (unit == null)
					return BehaviourStatus.Failure;
				else
					return BehaviourStatus.Success;
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
		}

		public class CanTargetUnit : UnitToUnitInteraction
		{
			public CanTargetUnit(string subjectVar, string targetVar) : base(subjectVar, targetVar) { }

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var subjectUnit = context[_subjectVar] as BaseUnit;
				var targetUnit = context[_targetVar] as BaseUnit;

				if (subjectUnit == null || targetUnit == null)
					return BehaviourStatus.Failure;

				if (subjectUnit.CanTarget(targetUnit))
					return BehaviourStatus.Success;
				else
					return BehaviourStatus.Failure;
			}
		}

		public class ShouldTargetUnit : UnitToUnitInteraction
		{
			public ShouldTargetUnit(string subjectVar, string targetVar) : base(subjectVar, targetVar) { }

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var subjectUnit = context[_subjectVar] as BaseUnit;
				var targetUnit = context[_targetVar] as BaseUnit;

				if (subjectUnit == null || targetUnit == null)
					return BehaviourStatus.Failure;

				if (subjectUnit.ShouldTarget(targetUnit))
					return BehaviourStatus.Success;
				else
					return BehaviourStatus.Failure;
			}
		}

		public class MoveToUnit : UnitToUnitInteraction
		{
			public MoveToUnit(string subjectVar, string targetVar) : base(subjectVar, targetVar)
			{
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var subjectUnit = context[_subjectVar] as BaseUnit;
				var targetUnit = context[_targetVar] as BaseUnit;

				if (subjectUnit == null || targetUnit == null)
					return BehaviourStatus.Failure;

				var subjectPosition = subjectUnit.GetUnitPositionX();
				var targetPosition = targetUnit.GetUnitPositionX();

				var distanceToTarget = targetPosition - subjectPosition;

				var maxTravelDistance = subjectUnit.movementSpeed * context.timeLeft;

				var signedDistance = Mathf.Sign(distanceToTarget);

				if (distanceToTarget * signedDistance > maxTravelDistance)
				{
					targetUnit.MoveUnitPosition(maxTravelDistance * signedDistance);
					context.timeLeft = 0f;
					return BehaviourStatus.Running;
				}
				else
				{
					var fraction = (distanceToTarget * signedDistance) / maxTravelDistance;

					context.timeLeft -= context.timeLeft * fraction;
					return BehaviourStatus.Success;
				}


			}
		}

		public class CanHitUnit : UnitToUnitInteraction
		{
			public CanHitUnit(string subjectVar, string targetVar) : base(subjectVar, targetVar) { }

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var subjectUnit = context[_subjectVar] as BaseUnit;
				var targetUnit = context[_targetVar] as BaseUnit;

				if (subjectUnit == null || targetUnit == null)
					return BehaviourStatus.Failure;


				if (subjectUnit.IsInRange(targetUnit))
					return BehaviourStatus.Success;
				else
					return BehaviourStatus.Failure;
			}
		}

		public class FindTargets : LeafNode
		{
			private ContextIndex _subjectVar;
			private ContextIndex _targetStack;

			private ContextIndex _rangeVar;
			private float? _range;

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

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				float range;
				if (_range.HasValue)
					range = _range.Value;
				else
				{
					range = (context[_rangeVar] as float?).GetValueOrDefault(5f);
				}

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
					return BehaviourStatus.Failure;

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

				return BehaviourStatus.Success;
			}
		}

		public class IsNullNode : LeafNode
		{
			private string _variable;

			public IsNullNode(ContextIndex variable)
			{
				this._variable = variable;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				var variableToCheck = context[_variable];

				if (variableToCheck == null)
					return BehaviourStatus.Success;

				return BehaviourStatus.Failure;
			}
		}

		public class SetToNullNode : LeafNode
		{
			private string _variable;

			public SetToNullNode(ContextIndex variable)
			{
				this._variable = variable;
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
			{
				context[_variable] = null;

				return BehaviourStatus.Success;
			}
		}

		public abstract class MathNode<T> : LeafNode
		{
			protected static readonly System.Type ContextIndexType = typeof(ContextIndex);

			protected static readonly System.Type FloatType = typeof(ContextIndex);
			protected static readonly System.Type DoubleType = typeof(ContextIndex);

			protected ContextIndex _targetVar;
			protected T _paramVar;

			public MathNode(ContextIndex targetVar, T paramVar)
			{
				this._targetVar = targetVar;
				this._paramVar = paramVar;
			}
		}

		public class SetVarTo<T> : MathNode<T>
		{
			public SetVarTo(string targetVar, T paramVar) : base(targetVar, paramVar)
			{
			}

			public override BehaviourStatus UpdateTick(BehaviorContext context)
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

				return BehaviourStatus.Success;
			}
		}
	}

}
