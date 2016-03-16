using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Week04.BehaviourTree;


namespace Week04
{
	public class BehaviorTreeWindow : EditorWindow
	{
		[MenuItem("Window/Behavior Tree Editor")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow<BehaviorTreeWindow>();
		}
		/*
		private string myString = "Hello World";
		private bool groupEnabled;
		private bool myBool = true;
		private float myFloat = 1.23f;
		*/

		private BehaviorNodeDrawer _rootNode;
		private Rect _scrollViewRect;


		public BehaviorTreeWindow()
		{
			var title = new GUIContent("Behavior Tree");
			titleContent = title;
			_scrollViewRect = new Rect(0,0,1200f, 500f);

			_scrollPosition = Vector2.zero;

		}

		protected void OnDestroy()
		{
			
		}

		protected void OnSelectionChange()
		{
	//		var currentSelection = Selection.instanceIDs;
		}


		protected void OnGUI()
		{
	//		GUIUtility.ScaleAroundPivot(Vector2.one, Vector2.zero);

			if (_rootNode == null)
			{
				if (SimpleUnit._DEBUGSTATIC_NODE != null)
				{
					_rootNode = new BehaviorNodeDrawer(SimpleUnit._DEBUGSTATIC_NODE, 60f, 10f);

					_scrollViewRect.height = _rootNode.GetCombinedHeight();
				}
			}


			
			var windowRect = new Rect(0,0, position.width, position.height);

			_scrollPosition = GUI.BeginScrollView(windowRect, _scrollPosition, _scrollViewRect	);

			BeginWindows();

			int idToUse = 1;

			if (_rootNode != null)
				_rootNode.OnDrawWindow(ref idToUse);

			EndWindows();

			

			GUI.EndScrollView();

			GUILayout.Label("Base Settings", EditorStyles.boldLabel);
			/*
			myString = EditorGUILayout.TextField("Text Field", myString);

			groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
				myBool = EditorGUILayout.Toggle("Toggle", myBool);
				myFloat = EditorGUILayout.Slider("Slider", myFloat, -3f, 3f);
		
			EditorGUILayout.EndToggleGroup();
				
			*/
		}

		private Vector2 _scrollPosition;
	}

	public class BehaviorNodeDrawer
	{
		protected Rect _windowRect;

		private BehaviorNodeDrawer() { }
		private INode _nodeToDraw;

		private BehaviorNodeDrawer[] _childrenNodes;

		public const float MIN_WIDTH = 100f;
		public const float MIN_HEIGHT = 50f;

		public const float INITIAL_HORIZONTAL_SPACING = 20f;

		public const float SIBLING_VERTICAL_SPACING = 1f;
		
		public const float VERTICAL_SPACING = 8f;


		public enum VisualNodeType
		{
			LeafNode = 0,
			DecoratorNode = 1,
			CompositeNode = 2
		}

		public VisualNodeType type;
		private string _windowTitle;

		public BehaviorNodeDrawer(INode nodeToDraw, float xPos, float yPos)
		{
			_windowRect = new Rect(xPos, yPos, MIN_WIDTH, MIN_HEIGHT);
			_nodeToDraw = nodeToDraw;

			var leafNode = nodeToDraw as LeafNode;
			if (leafNode != null)
			{
				type = VisualNodeType.LeafNode;
				_childrenNodes = new BehaviorNodeDrawer[0];
			}

			var decoratorNode = nodeToDraw as DecoratorNode;
			if (decoratorNode != null)
			{

				type = VisualNodeType.DecoratorNode;

				_childrenNodes = new BehaviorNodeDrawer[1]
				{
					new BehaviorNodeDrawer(decoratorNode.childNode, _windowRect.x + INITIAL_HORIZONTAL_SPACING + MIN_WIDTH, _windowRect.y)
				};
			}

			var compositeNode = nodeToDraw as ICompositeNode;
			if (compositeNode != null)
			{
				type = VisualNodeType.CompositeNode;

				var compositeChilds = compositeNode.getChildNodes();

				_childrenNodes = new BehaviorNodeDrawer[compositeChilds.Length];

				for (int i = 0; i < compositeChilds.Length; i++)
				{
					_childrenNodes[i] = new BehaviorNodeDrawer(compositeChilds[i], 
															_windowRect.x + INITIAL_HORIZONTAL_SPACING + MIN_WIDTH,
															_windowRect.y);
				}
			}


			var aggregatedHeight = 0f;

			//_windowRect.y += GetCombinedHeight();

			for (int i = 0; i < _childrenNodes.Length; i++)
			{
				_childrenNodes[i].MoveVertical(aggregatedHeight);

				aggregatedHeight += _childrenNodes[i].GetCombinedHeight();
			}

			_windowTitle = _nodeToDraw.GetType().Name.Replace("Node", "").Replace("Decorator", "").Replace("Composite", "");



		}

		public float GetCombinedHeight()
		{
			float combinedHeight = 0f;

			for (int i = 0; i < _childrenNodes.Length; i++)
			{
				if (combinedHeight != 0f)
					combinedHeight += SIBLING_VERTICAL_SPACING;

				combinedHeight += _childrenNodes[i].GetCombinedHeight();
			}

			if (combinedHeight == 0f)
				combinedHeight = _windowRect.height;

			combinedHeight += VERTICAL_SPACING;
			//combinedHeight += VERTICAL_SPACING;


			return combinedHeight;
		}

		public void SetPosition(Vector2 position)
		{
			_windowRect.position = position;
		}

		public void MoveVertical(float yPos)
		{
			_windowRect.y += yPos;

			for (int i = 0; i < _childrenNodes.Length; i++)
			{
				_childrenNodes[i].MoveVertical(yPos);
			}

		}

		public void OnDrawWindow(ref int id)
		{
			for (int i = 0; i < _childrenNodes.Length; i++)
			{
				DrawNodeCurve(_windowRect, _childrenNodes[i]._windowRect);
				_childrenNodes[i].OnDrawWindow(ref id);
			}


			//_windowRect = GUI.Window(id, _windowRect, DrawNode, _nodeToDraw.ToString());

		//	var style =

	//		_nodeToDraw.GetType().Name
	
			
			 //_windowRect = 
			 GUI.Window(id, _windowRect, DrawNode, _windowTitle);


			id++;
		}

		private static readonly Color LeafNodeColor_Title = new Color(97f/255f, 151f/255f, 247f/255f, 0.5f);

		private static readonly Color DecoratorNodeColor_Title = new Color(247f / 255f, 162f / 255f, 151f / 255f, 0.5f);
		private static readonly Color CompositeNodeColor_Title = new Color(188f / 255f, 247f / 255f, 151f / 255f, 0.5f);


		private void DrawNode(int id)
		{
			switch (type)
			{
				case VisualNodeType.LeafNode:
					GUI.color = LeafNodeColor_Title;
					break;
				case VisualNodeType.DecoratorNode:
					GUI.color = DecoratorNodeColor_Title;

					break;
				case VisualNodeType.CompositeNode:
					GUI.color = CompositeNodeColor_Title;

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}


			GUI.DrawTexture(new Rect(0,0,_windowRect.width, 16f), EditorGUIUtility.whiteTexture);

			//GUI.color = Color.black;

			//GUI.DragWindow();

			GUI.DragWindow();

			GUILayout.Label("Base Settings", EditorStyles.boldLabel);
			_nodeToDraw.DrawGUI(id);


		}

		private static void DrawNodeCurve(Rect start, Rect end)
		{
			const float offset = 20f;
			Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height/2f, 0);
			Vector3 endPos = new Vector3(end.x, end.y + end.height/2f, 0);

			Vector3 startTan = startPos + Vector3.right* offset;
			Vector3 endTan = endPos + Vector3.left* offset;
			Color shadowCol = new Color(0, 0, 0, 0.06f);
			for (int i = 0; i < 3; i++) // Draw a shadow
				Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1)*5f);
				
			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, EditorGUIUtility.whiteTexture, 1f);
		}
	}
}

/*

using UnityEngine;
using UnityEditor;
 
public class NodeEditor : EditorWindow
{

	Rect window1;
	Rect window2;

	[MenuItem("Window/Node editor")]
	static void ShowEditor()
	{
		NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
		editor.Init();
	}

	public void Init()
	{
		window1 = new Rect(10, 10, 100, 100);
		window2 = new Rect(210, 210, 100, 100);
	}

	void OnGUI()
	{
		DrawNodeCurve(window1, window2); // Here the curve is drawn under the windows

		BeginWindows();
		window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1");   // Updates the Rect's when these are dragged
		window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
		EndWindows();
	}

	void DrawNodeWindow(int id)
	{
		GUI.DragWindow();
	}

	void DrawNodeCurve(Rect start, Rect end)
	{
		Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
		Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
		Vector3 startTan = startPos + Vector3.right * 50;
		Vector3 endTan = endPos + Vector3.left * 50;
		Color shadowCol = new Color(0, 0, 0, 0.06f);
		for (int i = 0; i < 3; i++) // Draw a shadow
			Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
		Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
	}
}


*/