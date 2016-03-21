using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;

namespace Week04
{
	namespace BehaviorTree
	{
		public class BehaviorTreeSelectorWindow
		{
			public Rect _windowRect;

			private BehaviorTreeWindow _parent;

			private BehaviorTreeSelectorWindow()
			{}

			public BehaviorTreeSelectorWindow(BehaviorTreeWindow parent)
			{
				this._parent = parent;

				_windowRect = new Rect(100f, 100f, 200f, 500f);

				_currentSelected = new KeyValuePair<string, WeakReferenceT<INode>>();
			}

			public void OnDrawWindow(ref int id)
			{
				_windowRect = GUI.Window(id, _windowRect, DrawNode, "Select a Tree");
				id++;
			}

			private KeyValuePair<string, WeakReferenceT<INode>> _currentSelected;

			protected void DrawNode(int id)
			{
				if (_currentSelected.Value != null && _currentSelected.Value.IsAlive)
				{
					GUI.contentColor = Color.black;
				}
				else
				{
					GUI.contentColor = Color.red;
				}
				if (_currentSelected.Key != null)
				{
					GUILayout.Label("Tree: " + _currentSelected.Key.ToString());
				}
				else
				{
					GUI.contentColor = Color.red;
					GUILayout.Label("No tree selected");
				}
				GUILayout.Space(5f);
				
				foreach (KeyValuePair<string, WeakReferenceT<INode>> treeRef in BehaviorTreeGlobals.behaviorTrees)
				{
					var weakRef = treeRef.Value;

					if (weakRef.IsAlive)
					{
						if (GUILayout.Button(treeRef.Key.ToString()))
						{
							_currentSelected = treeRef;
							this._parent.SetBehaviorTree(treeRef);

						}
					}
					else
					{
						GUI.enabled = false;
						if (GUILayout.Button(treeRef.Key.ToString()))
						{

						}
						GUI.enabled = true;

					}
				}


				GUI.DragWindow();

			}


		}


		public class BehaviorContextSelectorWindow
		{
			public Rect _windowRect;

			private BehaviorTreeWindow _parent;
			private Vector2 _scrollPosition;

			private BehaviorContextSelectorWindow()
			{ }

			public BehaviorContextSelectorWindow(BehaviorTreeWindow parent)
			{
				this._parent = parent;

				_windowRect = new Rect(200f, 100f, 200f, 500f);

				_currentSelected = new WeakReferenceT<BehaviorContext>(null);

				_scrollPosition = Vector2.zero;
			}

			public void OnDrawWindow(ref int id)
			{
				_windowRect = GUI.Window(id, _windowRect, DrawNode, "Select a Context");
				id++;
			}

			private WeakReferenceT<BehaviorContext> _currentSelected;

			protected void DrawNode(int id)
			{
				if (_currentSelected.IsAlive)
				{
					GUI.contentColor = Color.black;
					GUILayout.Label("You have selected a context");
					GUILayout.Label(_currentSelected.Target.ToString());


				}
				else
				{
					GUI.contentColor = Color.red;
					GUILayout.Label("No context selected");
					GUILayout.Label("");
				}

				GUILayout.Space(5f);

				_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

				foreach (WeakReferenceT<BehaviorContext> contextRef in BehaviorTreeGlobals.behaviorContexts)
				{

					if (contextRef.IsAlive)
					{
						if (GUILayout.Button(contextRef.Target.ToString()))
						{
							_currentSelected = contextRef;
							this._parent.SetBehaviorContext(contextRef);
						}
					}
					else
					{
						GUI.enabled = false;
						if (GUILayout.Button(contextRef.ToString()))
						{

						}
						GUI.enabled = true;

					}
				}

				EditorGUILayout.EndScrollView();

				GUI.DragWindow();

			}


		}
	}



}
