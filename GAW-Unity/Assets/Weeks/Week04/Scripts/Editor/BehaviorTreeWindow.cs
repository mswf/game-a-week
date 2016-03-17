using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Week04.BehaviorTree;


namespace Week04
{
	public class BehaviorTreeWindow : EditorWindow
	{
		[MenuItem("Window/Behavior Tree Editor")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow<BehaviorTreeWindow>();
		}

		private BehaviorNodeDrawer _rootNode;
		private Rect _scrollViewRect;

		private Rect _toolsWindowRect;

		private Vector2 _zoomLevel;

		private BehaviorContextDrawer _contextDrawer;

		public BehaviorTreeWindow()
		{
			var title = new GUIContent("Behavior Tree");
			titleContent = title;
			_scrollViewRect = new Rect(0,0,12000f, 5000f);

			_scrollPosition = Vector2.zero;

			_zoomLevel = Vector2.one;

			_toolsWindowRect = new Rect(0, 0, 130f, 240f);

			_contextDrawer = new BehaviorContextDrawer();

		}

		protected void Update()
		{
			if (EditorWindow.focusedWindow == this)
			{


			}

			Repaint();
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
			const float moveSpeed = 2000f;

			var previousScrollPosition = _scrollPosition;

			float dt = Time.deltaTime;

			Event e = Event.current;
			if (e != null)
			{
				switch (e.type)
				{
					case EventType.keyDown:
					{
						var keyCode = e.keyCode;

						switch (keyCode)
						{
							case KeyCode.A:
							case KeyCode.LeftArrow:
								_scrollPosition.x -= moveSpeed * dt;
								break;
							case KeyCode.D:
							case KeyCode.RightArrow:
								_scrollPosition.x += moveSpeed * dt;
								break;
							case KeyCode.W:
							case KeyCode.UpArrow:
								_scrollPosition.y -= moveSpeed * dt;
								break;
							case KeyCode.S:
							case KeyCode.DownArrow:
								_scrollPosition.y += moveSpeed * dt;
								break;

							default:
								break;
						}

						if (keyCode == KeyCode.Q)
						{
							_zoomLevel *= 0.66f;
						}

						if (keyCode == KeyCode.E)
						{
							_zoomLevel *= 1.5f;
						}

						if (keyCode == KeyCode.R)
						{
							_zoomLevel = Vector2.one;
						}
						break;
					}
				}
			}


			if (_rootNode == null)
			{
				if (SimpleUnit._DEBUGSTATIC_NODE != null)
				{
					_rootNode = new BehaviorNodeDrawer(SimpleUnit._DEBUGSTATIC_NODE, 60f, 10f);

					_scrollViewRect.height = _rootNode.GetCombinedHeight();

					_contextDrawer.behaviorContext = SimpleUnit._DEBUGSTATIC_BEHAVIORCONTEXT;
				}
			}

			var windowRect = new Rect(0, 0, position.width, position.height);


			_scrollPosition = GUI.BeginScrollView(windowRect, _scrollPosition, _scrollViewRect);

			BeginWindows();

			//GUIUtility.ScaleAroundPivot(_zoomLevel, Vector2.zero);

			int idToUse = 1;

			if (_rootNode != null)
				_rootNode.OnDrawWindow(ref idToUse);

			if (previousScrollPosition.x != _scrollPosition.x || previousScrollPosition.y != _scrollPosition.y)
			{
				_contextDrawer.MovePosition(_scrollPosition - previousScrollPosition);
				_toolsWindowRect.position += _scrollPosition - previousScrollPosition;
			}

			_contextDrawer.OnDrawWindow(ref idToUse);

			_toolsWindowRect = GUI.Window(idToUse, _toolsWindowRect, DrawToolsWindow, "Tools");
			idToUse++;

			EndWindows();

			GUI.EndScrollView();


			var textEditor = EditorGUIUtility.GetStateObject(typeof (TextEditor), EditorGUIUtility.keyboardControl) as TextEditor;

			if (textEditor != null)
			{
				if (focusedWindow == this)
				{
					if (Event.current.Equals(Event.KeyboardEvent("#x")))
						textEditor.Cut();

					if (Event.current.Equals(Event.KeyboardEvent("#c")))
						textEditor.Copy();

					if (Event.current.Equals(Event.KeyboardEvent("#v")))
						textEditor.Paste();
				}
			}
		}

		private void DrawToolsWindow(int id)
		{
			GUILayout.Label("Base Settings", EditorStyles.boldLabel);

			if (GUILayout.Button("Reset Tree"))
			{
				_rootNode = null;
				SimpleUnit._DEBUGSTATIC_NODE = null;
				SimpleUnit._DEBUGSTATIC_BEHAVIORCONTEXT = null;

				if (_contextDrawer != null)
				{
					_contextDrawer.behaviorContext = null;
				}
			}

			if (SimpleUnit._DEBUGSTATIC_BEHAVIORCONTEXT != null)
			{
				if (GUILayout.Button("Reset uses"))
				{
					var state = SimpleUnit._DEBUGSTATIC_BEHAVIORCONTEXT.state;

					foreach (var baseNodeState in state.Values)
					{
						baseNodeState.timesFailure = 0;
						baseNodeState.timesRunning = 0;
						baseNodeState.timesSuccess = 0;
					}
				}
			}

			GUI.DragWindow();
		}

		private Vector2 _scrollPosition;
	}

	public class BehaviorNodeDrawer
	{
		public enum VisualNodeType
		{
			LeafNode = 0,
			DecoratorNode = 1,
			CompositeNode = 2
		}

		protected Rect _windowRect;

		private INode _nodeToDraw;

		private BehaviorNodeDrawer[] _childrenNodes;

		public const float MIN_WIDTH = 100f;
		public const float MIN_HEIGHT = 42f;

		public const float INITIAL_HORIZONTAL_SPACING = 20f;
		public const float SIBLING_VERTICAL_SPACING = 1f;
		public const float VERTICAL_SPACING = 8f;

		public VisualNodeType type;
		private string _windowTitle;

		private BehaviorNodeDrawer()
		{}

		public BehaviorNodeDrawer(INode nodeToDraw, float xPos, float yPos)
		{
			_windowRect = new Rect(xPos, yPos, MIN_WIDTH, MIN_HEIGHT + nodeToDraw.GetGUIPropertyHeight());
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
					_childrenNodes[i] = new BehaviorNodeDrawer(compositeChilds[i], _windowRect.x + INITIAL_HORIZONTAL_SPACING + MIN_WIDTH, _windowRect.y);
				}
			}


			var aggregatedHeight = 0f;

			for (int i = 0; i < _childrenNodes.Length; i++)
			{
				_childrenNodes[i].MoveVertical(aggregatedHeight);

				aggregatedHeight += _childrenNodes[i].GetCombinedHeight();
			}

			_windowTitle = _nodeToDraw.GetType().Name.Replace("Node", "").Replace("Decorator", "").Replace("Composite", "");
		}

		public float GetCombinedHeight()
		{
			var combinedHeight = 0f;

			for (int i = 0; i < _childrenNodes.Length; i++)
			{
				if (combinedHeight != 0f)
					combinedHeight += SIBLING_VERTICAL_SPACING;

				combinedHeight += _childrenNodes[i].GetCombinedHeight();
			}

			if (combinedHeight == 0f)
				combinedHeight = _windowRect.height;

			combinedHeight += VERTICAL_SPACING;

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

			GUI.Window(id, _windowRect, DrawNode, _windowTitle);

			id++;
		}

		private static readonly Color LeafNodeColor_Title = new Color(97f/255f, 151f/255f, 247f/255f, 0.5f);

		private static readonly Color DecoratorNodeColor_Title = new Color(247f/255f, 162f/255f, 151f/255f, 0.5f);
		private static readonly Color CompositeNodeColor_Title = new Color(188f/255f, 247f/255f, 151f/255f, 0.5f);

		private static readonly Color WhiteTransparentColor = new Color(1, 1, 1, 0);

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
			GUI.DrawTexture(new Rect(0, 0, _windowRect.width, 16f), EditorGUIUtility.whiteTexture);


			var state = SimpleUnit._DEBUGSTATIC_BEHAVIORCONTEXT.TryGetState<BaseNodeState>(_nodeToDraw);

			if (state != null)
			{
				var previousState = state.previousStatus;
				var timeSinceChange = Time.time - state.timeSinceStatusChange;


				switch (previousState)
				{
					case BehaviorStatus.Success:
						GUI.color = Color.Lerp(Color.green, WhiteTransparentColor, timeSinceChange);
						break;
					case BehaviorStatus.Failure:
						GUI.color = Color.Lerp(Color.red, WhiteTransparentColor, timeSinceChange);
						break;
					case BehaviorStatus.Running:
						GUI.color = Color.Lerp(Color.yellow, WhiteTransparentColor, timeSinceChange);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				GUI.DrawTexture(new Rect(0, 16f, _windowRect.width, _windowRect.height - 16f), EditorGUIUtility.whiteTexture);

				GUI.color = Color.black;
				
				var smallStyle = EditorStyles.miniLabel;
				smallStyle.fontSize = 8;

				GUILayout.Label("S: " + state.timesSuccess + " F: " + state.timesFailure + " R: " + state.timesRunning, smallStyle);
			}
			else
			{
				var smallStyle = EditorStyles.miniLabel;
				smallStyle.fontSize = 8;

				GUILayout.Label("S: 0 F: 0 R: 0", smallStyle);
			}


			GUI.color = Color.white;
			_nodeToDraw.DrawGUI(id);

			GUI.DragWindow();
		}

		private static void DrawNodeCurve(Rect start, Rect end)
		{
			const float offset = 20f;
			Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height/2f, 0);
			Vector3 endPos = new Vector3(end.x, end.y + end.height/2f, 0);

			Vector3 startTan = startPos + Vector3.right*offset;
			Vector3 endTan = endPos + Vector3.left*offset;
			Color shadowCol = new Color(0, 0, 0, 0.06f);
			for (int i = 0; i < 3; i++) // Draw a shadow
				Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1)*5f);

			Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, EditorGUIUtility.whiteTexture, 1f);
		}

	}

	public class BehaviorContextDrawer
	{
		private Rect _windowRect;

		public BehaviorContext behaviorContext;

		public BehaviorContextDrawer()
		{
			_windowRect = new Rect(200f, 200f, 400f, 600f);
		}

		public void OnDrawWindow(ref int id)
		{
			_windowRect = GUI.Window(id, _windowRect, DrawContext, "Behavior Context");

			id++;
		}

		private bool _isMemoryExpanded = true;
		private bool _isStateExpanded = false;

		private void DrawContext(int id)
		{
			if (behaviorContext == null)
			{
				GUILayout.Label("No active Behavior Context");
				GUI.DragWindow();

				return;
			}

			_isMemoryExpanded = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), _isMemoryExpanded, "Memory");
			if (_isMemoryExpanded)
			{
				EditorGUI.indentLevel = 1;

				var memory = behaviorContext.memory;

				foreach (KeyValuePair<string, object> keyValuePair in memory)
				{
					if (keyValuePair.Value != null)
						EditorGUILayout.LabelField(keyValuePair.Key.ToString(), keyValuePair.Value.ToString());
					else
						EditorGUILayout.LabelField(keyValuePair.Key.ToString(), "null");
				}
			}
			EditorGUI.indentLevel = 0;

			GUI.enabled = false;
			_isStateExpanded = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), _isStateExpanded, "State");

			if (_isStateExpanded)
			{
				EditorGUI.indentLevel = 1;

				var state = behaviorContext.state;

				foreach (KeyValuePair<object, BaseNodeState> keyValuePair in state)
				{
					if (keyValuePair.Value != null)
						EditorGUILayout.LabelField(keyValuePair.Key.ToString(), keyValuePair.Value.ToString());
					else
						EditorGUILayout.LabelField(keyValuePair.Key.ToString(), "null");
				}
			}

			GUI.enabled = true;

			GUI.DragWindow();
		}

		public void MovePosition(Vector2 positionDelta)
		{
			_windowRect.position += positionDelta;
		}

	}
}
