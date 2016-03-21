using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;


namespace BehaviorTree
{
	public abstract class SubWindow
	{
		public Rect _windowRect;

		protected BehaviorTreeWindow _parent;

		protected string WindowTitle;

		private SubWindow() { }

		protected SubWindow(BehaviorTreeWindow behaviorTreeWindow)
		{
			_parent = behaviorTreeWindow;
			
		}

		public void OnGUI(ref int windowId)
		{
			_windowRect = GUI.Window(windowId, _windowRect, DrawWindow, WindowTitle);
			windowId++;
		}

		public abstract void DrawWindow(int id);

		public virtual void SetPosition(Vector2 position)
		{
			_windowRect.position = position;
		}

		public virtual void MovePosition(Vector2 position)
		{
			_windowRect.position += position;
		}

		public virtual void MoveVertical(float yPos)
		{
			_windowRect.y += yPos;
		}
	}

	public abstract class SearchableSubWindow : SubWindow
	{
		protected Vector2 _scrollPosition;
		protected string _filterText;


		public SearchableSubWindow(BehaviorTreeWindow behaviorTreeWindow) : base(behaviorTreeWindow)
		{
			_scrollPosition = Vector2.zero;
			_filterText = "";
		}
	}

	public class BehaviorTreeSelectorWindow : SearchableSubWindow
	{
		public BehaviorTreeSelectorWindow(BehaviorTreeWindow parent) : base(parent)
		{
			WindowTitle = "Select Tree";

			_windowRect = new Rect(100f, 100f, 200f, 500f);

			_currentSelected = new KeyValuePair<string, WeakReferenceT<INode>>();
		}

		private KeyValuePair<string, WeakReferenceT<INode>> _currentSelected;

		public override void DrawWindow(int id)
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
				GUILayout.Label(_currentSelected.Key.ToString());
			}
			else
			{
				GUI.contentColor = Color.red;
				GUILayout.Label("No tree selected");
			}

			GUILayout.Space(10f);

			_filterText = GUILayout.TextField(_filterText);

			bool filterText = _filterText != "";

			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			foreach (KeyValuePair<string, WeakReferenceT<INode>> treeRef in BehaviorTreeGlobals.behaviorTrees)
			{
				var weakRef = treeRef.Value;

				var name = treeRef.Key.ToString();

				if (!filterText || name.Contains(_filterText))
				{
					if (weakRef.IsAlive)
					{
						if (GUILayout.Button(name))
						{
							_currentSelected = treeRef;
							this._parent.SetBehaviorTree(treeRef);

						}
					}
					else
					{
						GUI.enabled = false;
						if (GUILayout.Button(name))
						{

						}
						GUI.enabled = true;

					}
				}


			}

			EditorGUILayout.EndScrollView();

			if (id != -1)
				GUI.DragWindow();
		}
	}


	public class BehaviorContextSelectorWindow : SearchableSubWindow
	{

		public BehaviorContextSelectorWindow(BehaviorTreeWindow parent) : base(parent)
		{
			WindowTitle = "Select Context";

			_windowRect = new Rect(200f, 100f, 200f, 500f);

			_currentSelected = new WeakReferenceT<BehaviorContext>(null);

		}

		private WeakReferenceT<BehaviorContext> _currentSelected;

		public override void DrawWindow(int id)
		{
			if (_currentSelected.IsAlive)
			{
				GUI.contentColor = Color.black;
				GUILayout.Label(_currentSelected.Target.ToString());


			}
			else
			{
				GUI.contentColor = Color.red;
				GUILayout.Label("No context selected");
			}

			GUILayout.Space(10f);

			_filterText = GUILayout.TextField(_filterText);

			bool filterText = _filterText != "";


			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			foreach (WeakReferenceT<BehaviorContext> contextRef in BehaviorTreeGlobals.behaviorContexts)
			{
				if (contextRef.IsAlive == false)
					break;

				var name = contextRef.Target.ToString();

				if (!filterText || name.Contains(_filterText))
				{
					if (contextRef.IsAlive)
					{
						if (GUILayout.Button(name))
						{
							_currentSelected = contextRef;
							this._parent.SetBehaviorContext(contextRef);
						}
					}
					else
					{
						GUI.enabled = false;
						if (GUILayout.Button(name))
						{

						}
						GUI.enabled = true;

					}
				}


			}

			EditorGUILayout.EndScrollView();
			if (id != -1)
				GUI.DragWindow();

		}


	}
}

