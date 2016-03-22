
//#define SAFE_MODE

//#define DEBUG_MEMORY

#define ALLOW_EDITOR

using System;
using UnityEngine;
using System.Collections.Generic;

using Object = System.Object;
using ContextIndex = System.String;



namespace BehaviorTree
{
	//Introduction article:
	//http://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php

	//LibGDX AI documentation on Behavior Trees
	//https://github.com/libgdx/gdx-ai/wiki/Behavior-Trees
	// Example of a dog
	//https://github.com/jsjolund/GdxDemo3D/blob/master/android/assets/btrees/dog.btree


	//http://twvideo01.ubm-us.net/o1/vault/gdc10/slides/ChampandardDaweHernandezCerpa_BehaviorTrees.pdf

	//Understanding the Second Generation of Behavior Trees and Preparing for Challenges Beyond
	//https://youtu.be/n4aREFb3SsU


	/*
	NOTES:

	- more status: 
		invalid .. .. .. 
		fresh .. .. .. cancelled
		
	- structure
		It's possible to make a node initialize and cleanup inside of its own update function?
		I moved them outside so that I don't need to add it to simple leaf nodes
		But by adding tests, I can verify that these are called

	- add attributes to 
		- the classes, with "TaskConstraint(minChildren = 0, maxChildren = int.max)"
		- all properties of task with "TaskAttribute(name = fieldName, required = false) (required to be set yes or no)"
		- I can also do typechecking, by adding a type to the TaskAttribute, i can say that a var will be used to store a Stack<>, if other code tries to store a different type in the stack, throw a parsing error
		// https://msdn.microsoft.com/en-us/library/aa288454(v=vs.71).aspx
		- cancel the collection of all this info by adding [Conditional("EDITOR")] to the TaskConstraintAttribute definition
		//https://msdn.microsoft.com/en-us/library/aa288454(v=vs.71).aspx

		- Use attributes to determine what properties the editor should draw instead of the virtual function all nodes have
		- I can add attributes to the return types of functions, to add extra help info
		//https://msdn.microsoft.com/en-us/library/z0w1kczw.aspx

	- Include decorator
		- decorator node with a reference to another BT

	- Enticers
		- FE: if idle, look around for actors that try to promote themselves as things that satisfy this state. If found, get BT from actor and attach it to idle
	
	*/
	
	public enum BehaviorStatus
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
	public class BehaviorContext
	{
		public float timeLeft;

#if DEBUG_MEMORY
		[SerializeField]
		public List<DebugMemoryEntry> memory;
#else
		public Dictionary<ContextIndex, Object> memory;
#endif
		public Dictionary<Object, BaseNodeState> state; 

		public BehaviorContext()
		{
#if DEBUG_MEMORY
			memory = new List <DebugMemoryEntry>();
#else
			memory = new Dictionary<ContextIndex, Object>();
#endif

			state = new Dictionary<object, BaseNodeState>();
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

		public T GetState<T>(Object stateKey) where T : BaseNodeState, new()
		{
			if (state.ContainsKey(stateKey))
				return (T) state[stateKey];

			var newState = new T();
			state[stateKey] = newState;

			return newState;
		}

		public T TryGetState<T>(Object stateKey) where T : BaseNodeState
		{
			if (state.ContainsKey(stateKey))
				return (T)state[stateKey];
			return null;
		}

		public override string ToString()
		{
			var owner = this["U_SUBJECT"];
			if (owner != null)
				return "MEM: " + owner.ToString();

			return base.ToString();
		}
	}

	public interface INode
	{
		BehaviorStatus Update(BehaviorContext context);

		void Initialize(BehaviorContext context);
		void Cleanup(BehaviorContext context);

		void SetBehaviorState(BehaviorState behaviorState);

		void DrawGUI(int windowID);
		float GetGUIPropertyHeight();
	}

	public interface ICompositeNode
	{
		INode[] getChildNodes();
	}

	public interface IDecoratorNode
	{
		INode getChildNode();
	}

	public interface ILeafNode
	{
		
	}

	public abstract class Node<StateType> : INode where StateType : BaseNodeState, new()
	{
		protected BehaviorState behaviorState;
		
		public virtual BehaviorStatus Update(BehaviorContext context)
		{
			//return UpdateTick(context);

			///*
			var result = UpdateTick(context);

			var nodeState = context.GetState<StateType>(this);
			nodeState.previousStatus = result;
			nodeState.timeSinceStatusChange = Time.time;
			nodeState.timesCalled++;

			switch (result)
			{
				case BehaviorStatus.Success:
					nodeState.timesSuccess++;
					break;
				case BehaviorStatus.Failure:
					nodeState.timesFailure++;

					break;
				case BehaviorStatus.Running:
					nodeState.timesRunning++;

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
			//*/
		}

		public abstract BehaviorStatus UpdateTick(BehaviorContext context);

		public virtual void Initialize(BehaviorContext context)
		{
#if ALLOW_EDITOR
			var nodeState = context.GetState<StateType>(this);
			nodeState.timeSinceStatusChange = Time.time;
#endif
		}

		public virtual void Cleanup(BehaviorContext context)
		{
#if ALLOW_EDITOR
			var nodeState = context.GetState<StateType>(this);
			nodeState.timeSinceStatusChange = Time.time;
#endif
		}

		public abstract void SetBehaviorState(BehaviorState behaviorState);

		public virtual void DrawGUI(int windowID) {}

		protected const float DefaultPropertyHeight = 18f;
		
		public virtual float GetGUIPropertyHeight()
		{
			return 0;
		}
	}
}

