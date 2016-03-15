
#define SAFE_MODE

#define DEBUG_MEMORY


using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Object = System.Object;

using ContextIndex = System.String;
using Stack_Object = System.Collections.Generic.Stack<System.Object>;

namespace Week04
{

	namespace BehaviourTree
	{
		//http://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php

		public enum BehaviourStatus
		{
			Success,
			Failure,
			Running
		}

#if DEBUG_MEMORY
		[System.Serializable]
		public class DebugMemoryEntry
		{
			[ReadOnlyAttribute]
			public ContextIndex Key;

			//[ShowOnlyIfNotNull, SerializeField]
			private Object _value;
			public Object Value
			{
				get
				{
					return _value;
				}
				set
				{
					_value = value;


					var type = value.GetType();

					valueUnit = null;
					valueFloat = float.PositiveInfinity;
					valueStack = null;
					valueString = "";

					if (type == typeof(BaseUnit) || type.IsSubclassOf(typeof(BaseUnit)))
						valueUnit = value as BaseUnit;
					else if (type == typeof(float))
						valueFloat = (value as float?).Value;
					else if (type == typeof (Stack_Object))
					{
						valueStack = value as Stack_Object;
						valueString = "<STACK_OBJECT>";
					}
					else if (type == typeof (string))
						valueString = value as string;
				}
			}
			
			[ShowOnlyIfNotNull]
			public BaseUnit valueUnit;
			[ShowOnlyIfNotNull]
			public float valueFloat = float.PositiveInfinity;
			[ShowOnlyIfNotNull]
			public string valueString = "";

			// TODO: draw the stack
			[ShowOnlyIfNotNull]
			public Stack_Object valueStack = null;

			public DebugMemoryEntry(ContextIndex index, Object entry)
			{
				Key = index;
				Value = entry;
			}
		}
#endif

		[System.Serializable]
		public class BehaviourContext
		{
			public float timeLeft;

#if DEBUG_MEMORY
			[SerializeField]
			public List<DebugMemoryEntry> memory;
#else
			public Dictionary<ContextIndex, Object> memory;
#endif
			public BehaviourContext()
			{
#if DEBUG_MEMORY
				memory = new List <DebugMemoryEntry>();
#else
				memory = new Dictionary<ContextIndex, object>();
#endif
			}

		public Object this[ContextIndex memoryKey] 
			{
				get
				{
#if DEBUG_MEMORY
					for (int i = 0; i < memory.Count; i++)
					{
						if (memory[i].Key == memoryKey)
							return memory[i].Value;
					}
					return null;
#else
					if (memory.ContainsKey(memoryKey))
						return memory[memoryKey];
					else
						return null;
#endif
				}
				set
				{
#if DEBUG_MEMORY
					for (int i = 0; i < memory.Count; i++)
					{
						if (memory[i].Key == memoryKey)
						{
							memory[i] = new DebugMemoryEntry(memoryKey, value);
							return;
						}
					}
					memory.Add(new DebugMemoryEntry(memoryKey, value));
#else
					memory[memoryKey] = value;
#endif
				}
			}
		}

		public abstract class Node
		{
			public abstract BehaviourStatus UpdateTick(BehaviourContext context);

			public const ContextIndex S_SUBJECT = "S_SUBJECT";

			public virtual void Initialize()
			{

			}

			public virtual void Cleanup()
			{

			}
		}
		
		[System.Serializable]
		public class EntryNode : Node
		{
			public Node firstNode;

			public BehaviourContext _context;

			private BaseUnit subject;

			public EntryNode(BaseUnit subject, Node firstNode)
			{
				this.subject = subject;
				this.firstNode = firstNode;

				_context = new BehaviourContext();

				_context[S_SUBJECT] = subject;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				throw new NotImplementedException();
			}

			private BehaviourStatus _prevStatus;

			public void UpdateTree(float dt)
			{
				_context.timeLeft = dt;

				if (_prevStatus != BehaviourStatus.Running)
					firstNode.Initialize();

				var result = firstNode.UpdateTick(_context);

				if (result != BehaviourStatus.Running)
					firstNode.Cleanup();

				_prevStatus = result;
			}
		}

		public abstract class CompositeNode : Node
		{
			protected Node[] childNodes;

			protected CompositeNode(params Node[] childNodes)
			{
				this.childNodes = childNodes;
			}

			
		}

		public class SequenceCompositeNode : CompositeNode
		{
			public SequenceCompositeNode(params Node[] childNodes) : base(childNodes)
			{
			}


			private int _currentNodeIndex;

			public override void Initialize()
			{
				_currentNodeIndex = -1;

			}

			public override void Cleanup()
			{
				_currentNodeIndex = -1;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				if (_currentNodeIndex >= 0)
				{
					var result = childNodes[_currentNodeIndex].UpdateTick(context);

					switch (result)
					{
						case BehaviourStatus.Success:
							childNodes[_currentNodeIndex].Cleanup();
							break;
						case BehaviourStatus.Failure:
							childNodes[_currentNodeIndex].Cleanup();
							return BehaviourStatus.Failure;
						case BehaviourStatus.Running:
							return BehaviourStatus.Running;
						default:
							throw new ArgumentOutOfRangeException();
					}

				}
				else
				{
					_currentNodeIndex = 0;
				}


				for (int index = _currentNodeIndex; index < childNodes.Length; index++)
				{
					childNodes[index].Initialize();
					var result = childNodes[index].UpdateTick(context);

					switch (result)
					{
						case BehaviourStatus.Success:
							childNodes[index].Cleanup();
							break;
						case BehaviourStatus.Failure:
							childNodes[index].Cleanup();
							
							return BehaviourStatus.Failure;
						case BehaviourStatus.Running:
							_currentNodeIndex = index;

							return BehaviourStatus.Running;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				return BehaviourStatus.Success;
			}
		}

		public class SelectorCompositeNode : CompositeNode
		{
			public SelectorCompositeNode(params Node[] childNodes) : base(childNodes)
			{
			}

			private int _currentNodeIndex;

			public override void Initialize()
			{
				_currentNodeIndex = -1;

			}

			public override void Cleanup()
			{
				_currentNodeIndex = -1;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{

				if (_currentNodeIndex >= 0)
				{
					var result = childNodes[_currentNodeIndex].UpdateTick(context);

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
					_currentNodeIndex = 0;
				}

				for (int index = _currentNodeIndex; index < childNodes.Length; index++)
				{
					var childNode = childNodes[index];
					var result = childNode.UpdateTick(context);

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

		public abstract class DecoratorNode : Node
		{
			protected Node childNode;

			protected DecoratorNode(Node childNode)
			{
				this.childNode = childNode;
			}

			public override void Initialize()
			{
				childNode.Initialize();
			}

			public override void Cleanup()
			{
				childNode.Cleanup();
			}
		}

		public class InverterDecoratorNode : DecoratorNode
		{
			public InverterDecoratorNode(Node childNode) : base(childNode)
			{
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var result = childNode.UpdateTick(context);

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
			public SucceederDecoratorNode(Node childNode) : base(childNode)
			{
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				childNode.UpdateTick(context);
				return BehaviourStatus.Success;
			}
		}

		public class RepeatUntilFailDecoratorNode : DecoratorNode
		{
			public RepeatUntilFailDecoratorNode(Node childNode) : base(childNode)
			{
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var result = childNode.UpdateTick(context);

				if (result == BehaviourStatus.Failure)
					return BehaviourStatus.Success;

				return BehaviourStatus.Running;
				
			}
		}

		public abstract class LeafNode : Node
		{
			protected LeafNode()
			{
				
			}
		}

		public class PrintNode : LeafNode
		{
			private string _messageToPrint;

			public PrintNode(string messageToPrint)
			{
				this._messageToPrint = messageToPrint;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				Debug.Log(_messageToPrint);

				return BehaviourStatus.Success;
			}
		}
		
		public class PrintVarNode : LeafNode
		{
			private string _varToPrint;

			public PrintVarNode(string varToPrint)
			{
				this._varToPrint = varToPrint;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var toPrint = context[_varToPrint];
				if (toPrint == null)
					Debug.Log("null");
				else
					Debug.Log(toPrint.ToString());

				return BehaviourStatus.Success;
			}
		}

		/*
		PushToStack(item, stackVar)
		PopFromStack(stack, itemVar)
		IsEmpty(stack)
		*/
		public class PushToStack : LeafNode
		{
			private readonly ContextIndex _itemVar;
			private readonly ContextIndex _stackVar;


			public PushToStack(ContextIndex itemVar, ContextIndex stackVar)
			{
				this._itemVar = itemVar;
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var stack = context[_stackVar] as Stack_Object;

				if (stack == null)
				{
					stack = new Stack_Object();
					context[_stackVar] = stack;
				}

				var objectToPush = context[_itemVar] as Object;
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


			public PopFromStack (ContextIndex itemVar, ContextIndex stackVar)
			{
				this._itemVar = itemVar;
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var stack = context[_stackVar] as Stack_Object;

				if (stack == null || stack.Count == 0)
				{
				//	Debug.LogError("Not a stack! Variable: " + _stackVar.ToString());
					return BehaviourStatus.Failure;
				}

				var objectToPop = stack.Pop() as Object;
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var subjectUnit = context[_subjectVar] as BaseUnit;
				var targetUnit = context[_targetVar] as BaseUnit;

				if (subjectUnit == null || targetUnit == null)
					return BehaviourStatus.Failure;

				var subjectPosition = subjectUnit.GetUnitPositionX();
				var targetPosition = targetUnit.GetUnitPositionX();

				var distanceToTarget = targetPosition - subjectPosition;

				var maxTravelDistance = subjectUnit.movementSpeed*context.timeLeft;

				var signedDistance = Mathf.Sign(distanceToTarget);

				if (distanceToTarget*signedDistance > maxTravelDistance)
				{
					targetUnit.MoveUnitPosition(maxTravelDistance*signedDistance);
					context.timeLeft = 0f;
					return BehaviourStatus.Running;
				}
				else
				{
					var fraction = (distanceToTarget*signedDistance)/maxTravelDistance;

					context.timeLeft -= context.timeLeft*fraction;
					return BehaviourStatus.Success;
				}


			}
		}

		public class CanHitUnit : UnitToUnitInteraction
		{
			public CanHitUnit(string subjectVar, string targetVar) : base(subjectVar, targetVar) { }

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override void Cleanup()
			{
				base.Cleanup();
			}

			public override void Initialize()
			{
				base.Initialize();
			}
		}

		public class IsNullNode : LeafNode
		{
			private string _variable;

			public IsNullNode(ContextIndex variable)
			{
				this._variable = variable;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
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

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var type = typeof (T);

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
