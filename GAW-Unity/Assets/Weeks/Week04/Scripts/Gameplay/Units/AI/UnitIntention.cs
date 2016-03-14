using System;
using UnityEngine;
using System.Collections.Generic;

using Object = System.Object;

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

		public class BehaviourContext
		{
			public float timeLeft;

			public BaseUnit target;

			public Dictionary<string, Object> memory;
		}

		public abstract class Node
		{
			public abstract BehaviourStatus UpdateTick(BehaviourContext context);

			public const string S_SUBJECT = "S_SUBJECT";

			public virtual void Initialize()
			{

			}

			public virtual void Cleanup()
			{

			}
		}

		public class EntryNode : Node
		{
			public Node firstNode;

			private BehaviourContext _context;

			private BaseUnit subject;

			public EntryNode(BaseUnit subject, Node firstNode)
			{
				this.subject = subject;
				this.firstNode = firstNode;

				_context = new BehaviourContext();

				_context.memory.Add(S_SUBJECT, subject);
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


/*
PushToStack(item, stackVar)
PopFromStack(stack, itemVar)
IsEmpty(stack)
*/
		public class PushToStack : LeafNode
		{
			private readonly string _itemVar;
			private readonly string _stackVar;


			public PushToStack(string itemVar, string stackVar)
			{
				this._itemVar = itemVar;
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var stack = context.memory[_stackVar] as Stack<Object>;

				if (stack == null)
				{
					stack = new Stack<Object>();
					context.memory.Add(_stackVar, stack);
				}

				var objectToPush = context.memory[_itemVar] as Object;
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
			private readonly string _itemVar;
			private readonly string _stackVar;


			public PopFromStack (string itemVar, string stackVar)
			{
				this._itemVar = itemVar;
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var stack = context.memory[_stackVar] as Stack<Object>;

				if (stack == null)
				{
					return BehaviourStatus.Failure;
				}

				var objectToPop = stack.Pop() as Object;
				if (objectToPop == null)
				{
					Debug.Log("Tried to pop nonexistent var '" + _itemVar + "' to stack '" + _stackVar + "'");
					return BehaviourStatus.Failure;
				}

				context.memory[_itemVar] = objectToPop;
				return BehaviourStatus.Success;
			}
		}

		public class IsStackEmpty : LeafNode
		{
			private readonly string _stackVar;

			public IsStackEmpty(string stackVar)
			{
				this._stackVar = stackVar;
			}

			public override BehaviourStatus UpdateTick(BehaviourContext context)
			{
				var stack = context.memory[_stackVar] as Stack<Object>;

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
	}
}
