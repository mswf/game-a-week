using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;

namespace BehaviorTree
{
public class BehaviorTreeWindow : EditorWindow
{
	[MenuItem("Window/Behavior Tree Editor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<BehaviorTreeWindow>();
	}

	[NonSerialized]
	private BehaviorNodeDrawer _rootNode;
	private Rect _scrollViewRect;


	private Rect _toolsWindowRect;

	private BehaviorTreeSelectorWindow _treeSelectorWindow;

	private BehaviorContextSelectorWindow _contextSelectorWindow;


	private Vector2 _zoomLevel;

	private BehaviorContextDrawer _contextDrawer;


	private KeyValuePair<string, WeakReferenceT<INode>> _currentNode;
	private WeakReferenceT<BehaviorContext> _currentContext;

	private bool _showSelector = true;

	public INode GetCurrentNode()
	{
		return _currentNode.Value.Target;
	}

	public BehaviorContext GetCurrentContext()
	{
		return _currentContext.Target;
	}

	public void SetBehaviorTree(KeyValuePair<string, WeakReferenceT<INode>> newNode)
	{
		if (!_currentNode.Value.IsAlive)
		{
			_currentNode = newNode;
			OnBehaviorTreeChanged();
		}

		if (newNode.Value.Target == _currentNode.Value.Target)
			return;

		_currentNode = newNode;
		OnBehaviorTreeChanged();
	}

	public void SetBehaviorContext(WeakReferenceT<BehaviorContext> newContext)
	{
		if (!_currentContext.IsAlive)
		{
			_currentContext = newContext;
			OnBehaviorTreeChanged();
		}

		if (newContext.Target == _currentContext.Target)
			return;


		_currentContext = newContext;

		_contextDrawer.behaviorContext = newContext;

		OnBehaviorTreeChanged();
	}

	public void OnBehaviorTreeChanged()
	{
		if (_currentNode.Value.IsAlive)
		{
			_rootNode = new BehaviorNodeDrawer(this, null, _currentNode.Value.Target, 60f, 10f);
			RecalculateScrollHeight();
		}
		else
		{
			_rootNode = null;
		}

		
	}

	public void RecalculateScrollHeight()
	{
		_scrollViewRect.height = Mathf.Max(_rootNode.GetCombinedHeight(), position.height);

	}

		public void OnContextChanged()
	{
		_contextDrawer.behaviorContext = _currentContext;

	}

	public BehaviorTreeWindow()
	{
		_currentNode = new KeyValuePair<string, WeakReferenceT<INode>>("", new WeakReferenceT<INode>(null));
		_currentContext = new WeakReferenceT<BehaviorContext>(null);

		var title = new GUIContent("Behavior Tree");
		titleContent = title;
		_scrollViewRect = new Rect(0,0,12000f, 5000f);

		_scrollPosition = Vector2.zero;

		_zoomLevel = Vector2.one;

		_toolsWindowRect = new Rect(0, 0, 130f, 240f);

		_contextDrawer = new BehaviorContextDrawer();

		_treeSelectorWindow = new BehaviorTreeSelectorWindow(this);
		_contextSelectorWindow = new BehaviorContextSelectorWindow(this);
	}

	protected void Update()
	{
		if (EditorWindow.focusedWindow == this)
		{


		}

		Repaint();

		// remember this
		TestConditional("Ghello");
	}

	[Conditional("STEB_TEST")]
	public static void TestConditional(System.Object message)
	{
		UnityEngine.Debug.Log(message);
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

		Rect windowPos = position;

		var windowRect = new Rect(0, 0, windowPos.width, windowPos.height);

		_prevWindowRect = windowPos;

		_scrollPosition = GUI.BeginScrollView(windowRect, _scrollPosition, _scrollViewRect);

		BeginWindows();

		//GUIUtility.ScaleAroundPivot(_zoomLevel, Vector2.zero);

		int idToUse = 1;

		if (_rootNode != null)
			_rootNode.OnDrawWindow(ref idToUse);



		_contextDrawer.OnDrawWindow(ref idToUse);

		_toolsWindowRect = GUI.Window(idToUse, _toolsWindowRect, DrawToolsWindow, "Tools");
		idToUse++;

		//_treeSelectorWindow.OnGUI(ref idToUse);
		//_contextSelectorWindow.OnGUI(ref idToUse);
		
		EndWindows();

		GUI.EndScrollView();

		GUI.color = Color.white;


		if (_showSelector)
		{
			const float selectorWidth = 220f;
			const float selectorHeight = 160f;

			var selectorRect = new Rect(0, windowPos.height - selectorHeight - 16f, selectorWidth, selectorHeight);

			GUILayout.BeginArea(selectorRect);

			EditorGUI.DrawRect(new Rect(0, 0, selectorWidth, selectorHeight), new Color(187f / 255f, 188f / 255f, 191f / 255f, 0.5f));

			_contextSelectorWindow.DrawWindow(-1);

			GUILayout.EndArea();

			var selectorRect2 = new Rect(selectorWidth, windowPos.height - selectorHeight - 16f, selectorWidth, selectorHeight);

			GUILayout.BeginArea(selectorRect2);

			EditorGUI.DrawRect(new Rect(0, 0, selectorWidth, selectorHeight), new Color(187f / 255f, 188f / 255f, 191f / 255f, 0.5f));

			_treeSelectorWindow.DrawWindow(-1);

			GUILayout.EndArea();
		}

			/*
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
			*/

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
								case KeyCode.Tab:
									_showSelector = !_showSelector;
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

			
			if (previousScrollPosition.x != _scrollPosition.x || previousScrollPosition.y != _scrollPosition.y)
			{
				_contextDrawer.MovePosition(_scrollPosition - previousScrollPosition);
				_toolsWindowRect.position += _scrollPosition - previousScrollPosition;

				_treeSelectorWindow.MovePosition(_scrollPosition - previousScrollPosition);
				_contextSelectorWindow.MovePosition(_scrollPosition - previousScrollPosition);
			}
			
		}

	private void DrawToolsWindow(int id)
	{
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);

		if (GUILayout.Button("Reset Tree"))
		{
			_rootNode = null;
			
			_currentNode = new KeyValuePair<string, WeakReferenceT<INode>>("", new WeakReferenceT<INode>(null));
			_currentContext = new WeakReferenceT<BehaviorContext>(null);

			if (_contextDrawer != null)
			{
				_contextDrawer.behaviorContext = _currentContext;
			}
		}

		if (_currentContext.Target != null)
		{
			if (GUILayout.Button("Reset uses"))
			{
				var state = _currentContext.Target.state;

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
	private Rect _prevWindowRect;
}

public class BehaviorNodeDrawer
{
	public enum VisualNodeType
	{
		LeafNode = 0,
		DecoratorNode = 1,
		CompositeNode = 2
	}

	public Rect _windowRect;

	protected INode _nodeToDraw;

	protected BehaviorNodeDrawer _parentNode;

	protected BehaviorNodeDrawer[] _childrenNodes;

	public const float MIN_WIDTH = 100f;
	public const float MIN_HEIGHT = 42f;

	public const float INITIAL_HORIZONTAL_SPACING = 20f;
	public const float SIBLING_VERTICAL_SPACING = 1f;
	public const float VERTICAL_SPACING = 8f;

	public VisualNodeType type;
	protected string _windowTitle;

	protected Vector2 _originalPos;

	private BehaviorGroupDrawer _groupDrawer;

	private BehaviorTreeWindow _behaviorTreeWindow;

	private BehaviorNodeDrawer()
	{}


	public BehaviorNodeDrawer(BehaviorTreeWindow behaviorTreeWindow, BehaviorNodeDrawer parentNode, INode nodeToDraw, float xPos, float yPos)
	{
		this._originalPos = new Vector2(xPos, yPos);
		this._behaviorTreeWindow = behaviorTreeWindow;
		this._parentNode = parentNode;
		
		var regionDecoratorNode = nodeToDraw as EditorRegionDecoratorNode;
		if (regionDecoratorNode != null)
		{
			_groupDrawer = new BehaviorGroupDrawer(behaviorTreeWindow, this, regionDecoratorNode, this);
			nodeToDraw = regionDecoratorNode.getChildNode();
		}

		_windowRect = new Rect(xPos, yPos, MIN_WIDTH, MIN_HEIGHT + nodeToDraw.GetGUIPropertyHeight());					
		_nodeToDraw = nodeToDraw;

		var leafNode = nodeToDraw as ILeafNode;
		if (leafNode != null)
		{
			type = VisualNodeType.LeafNode;
			_childrenNodes = new BehaviorNodeDrawer[0];
		}

		var decoratorNode = nodeToDraw as IDecoratorNode;
		if (decoratorNode != null)
		{
			type = VisualNodeType.DecoratorNode;

			_childrenNodes = new BehaviorNodeDrawer[1]
			{
				new BehaviorNodeDrawer(behaviorTreeWindow, this, decoratorNode.getChildNode(), _windowRect.x + INITIAL_HORIZONTAL_SPACING + MIN_WIDTH, _windowRect.y)
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
				_childrenNodes[i] = new BehaviorNodeDrawer(behaviorTreeWindow, this, compositeChilds[i], _windowRect.x + INITIAL_HORIZONTAL_SPACING + MIN_WIDTH, _windowRect.y);
			}
		}

		if (_groupDrawer != null)
			_groupDrawer.Init();

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

		if (_groupDrawer != null && _groupDrawer.isExpanded == false)
			return 40f + VERTICAL_SPACING*2f;

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

	public float GetCombinedWidth()
	{
		var combinedWidth = 0f;

		for (int i = 0; i < _childrenNodes.Length; i++)
		{
			//if (combinedWidth != 0f)
				//combinedWidth += INITIAL_HORIZONTAL_SPACING;

			combinedWidth = Mathf.Max(combinedWidth, _childrenNodes[i].GetCombinedWidth());
		}

		combinedWidth += INITIAL_HORIZONTAL_SPACING;
		combinedWidth += _windowRect.width;

		return combinedWidth;
	}

	public void SetPosition(Vector2 position)
	{
		_windowRect.position = position;

		if (_groupDrawer != null)
			_groupDrawer.Init();
	}

	public void MovePositionRecursively(Vector2 position)
	{
		SetPosition(_windowRect.position + position);


		for (int i = 0; i < _childrenNodes.Length; i++)
		{
			_childrenNodes[i].MovePositionRecursively(position);
		}
	}

		public void MoveVertical(float yPos)
	{
		_windowRect.y += yPos;

		for (int i = 0; i < _childrenNodes.Length; i++)
		{
			_childrenNodes[i].MoveVertical(yPos);
		}

		if (_groupDrawer != null)
			_groupDrawer.Init();
	}

	public void ResetPosition()
	{
		_windowRect.position = _originalPos;
	}

	public void RecalculatePosition()
	{
		if (_parentNode != null)
			_parentNode.RecalculatePosition();
		else
			OrderChildren();
	}

	public void OrderChildren()
	{
		ResetPosition();

		for (int i = 0; i < _childrenNodes.Length; i++)
		{
			_childrenNodes[i].ResetPosition();
			_childrenNodes[i].OrderChildren();
		}

		var aggregatedHeight = 0f;

		for (int i = 0; i < _childrenNodes.Length; i++)
		{
			_childrenNodes[i].MoveVertical(aggregatedHeight);

			aggregatedHeight += _childrenNodes[i].GetCombinedHeight();
		}
	}

	public void OnDrawWindow(ref int id)
	{
		if (_groupDrawer != null)
		{
			if (!_groupDrawer.OnDraw(ref id))
			{
				return;
			}
		}

		for (int i = 0; i < _childrenNodes.Length; i++)
		{
			DrawNodeCurve(_windowRect, _childrenNodes[i]._windowRect);
			_childrenNodes[i].OnDrawWindow(ref id);
		}
		var previousPos = _windowRect.position;
		_windowRect = GUI.Window(id, _windowRect, DrawNode, _windowTitle);

		for (int i = 0; i < _childrenNodes.Length; i++)
		{
			_childrenNodes[i].MovePositionRecursively(_windowRect.position - previousPos);
		}

		id++;
	}

	protected static readonly Color LeafNodeColor_Title = new Color(97f/255f, 151f/255f, 247f/255f, 0.5f);

	protected static readonly Color DecoratorNodeColor_Title = new Color(247f/255f, 162f/255f, 151f/255f, 0.5f);
	protected static readonly Color CompositeNodeColor_Title = new Color(188f/255f, 247f/255f, 151f/255f, 0.5f);

	protected static readonly Color WhiteTransparentColor = new Color(1, 1, 1, 0);

	protected void DrawNode(int id)
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


		var curContext = _behaviorTreeWindow.GetCurrentContext();
		BaseNodeState state;

		if (curContext != null)
		{
			state = curContext.TryGetState<BaseNodeState>(_nodeToDraw);
		}
		else
			state = null;

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

	protected static void DrawNodeCurve(Rect start, Rect end)
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

public class BehaviorGroupDrawer
{
	private BehaviorNodeDrawer _parentNode;
	private EditorRegionDecoratorNode _childNode;
	private BehaviorNodeDrawer _childNodeDrawer;

	private Rect _rectangle;

	private string _label;

	private bool _isExpanded;
	private BehaviorTreeWindow _behaviorTreeWindow;


	public bool isExpanded
	{
		get { return _isExpanded; }
	}

	public BehaviorGroupDrawer(BehaviorTreeWindow behaviorTreeWindow, BehaviorNodeDrawer parentNode, EditorRegionDecoratorNode childNode, BehaviorNodeDrawer childNodeDrawer)
	{
		this._behaviorTreeWindow = behaviorTreeWindow;

		_parentNode = parentNode;
		_childNode = childNode;
		_childNodeDrawer = childNodeDrawer;

		_label = childNode.label;

		_isExpanded = childNode.startExpanded;
	}

	public void Init()
	{
		_rectangle = new Rect(
			_childNodeDrawer._windowRect.position,
			new Vector2(
				_childNodeDrawer.GetCombinedWidth() - BehaviorNodeDrawer.INITIAL_HORIZONTAL_SPACING / 2f,
				_childNodeDrawer.GetCombinedHeight() - BehaviorNodeDrawer.VERTICAL_SPACING
			)
		);

		_rectangle.y -= BehaviorNodeDrawer.VERTICAL_SPACING / 2f;
		_rectangle.x -= BehaviorNodeDrawer.INITIAL_HORIZONTAL_SPACING / 4f;


	}

	public bool OnDraw(ref int id)
	{
		GUI.color = _childNode.regionColor;

		if (_isExpanded)
		{
			GUI.DrawTexture(_rectangle, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill, true);

			var labelPosition = new Rect(_rectangle);
			GUI.Label(labelPosition, _label);

			labelPosition.x += labelPosition.width - 100f;

			GUI.Label(labelPosition, _label);

			labelPosition.y += labelPosition.height - 20f;

			GUI.Label(labelPosition, _label);

			labelPosition.x -= labelPosition.width - 100f;

			labelPosition.width = 100f;
			labelPosition.height = 40f;
			labelPosition.y -= 20f;


			if (GUI.Button(labelPosition, _label))
			{
				_isExpanded = !_isExpanded;
				_parentNode.RecalculatePosition();
					_behaviorTreeWindow.RecalculateScrollHeight();
			}
		}
		else
		{
			var labelPosition = new Rect(_rectangle);

			labelPosition.width = 100f;
			labelPosition.height = 40f;
			labelPosition.y += labelPosition.height - 30f;
			
			if (GUI.Button(labelPosition, _label))
			{
				_isExpanded = !_isExpanded;
				_parentNode.RecalculatePosition();
					_behaviorTreeWindow.RecalculateScrollHeight();


				}
			}
		
		return _isExpanded;

	}
}

public class BehaviorContextDrawer
{
	private Rect _windowRect;

	public WeakReferenceT<BehaviorContext> behaviorContext;

	public BehaviorContextDrawer()
	{
		_windowRect = new Rect(200f, 200f, 400f, 600f);

		behaviorContext = new WeakReferenceT<BehaviorContext>(null);
	}

	public void OnDrawWindow(ref int id)
	{
		_windowRect.height = GetPreferredWindowHeight();

		_windowRect = GUI.Window(id, _windowRect, DrawContext, "Behavior Context");

		id++;
	}

	private bool _isMemoryExpanded = true;
	private bool _isStateExpanded = false;

	private void DrawContext(int id)
	{
		if (! behaviorContext.IsAlive)
		{
			GUILayout.Label("No active Behavior Context");
			GUI.DragWindow();

			return;
		}

		_isMemoryExpanded = EditorGUI.Foldout(EditorGUILayout.GetControlRect(), _isMemoryExpanded, "Memory");
		if (_isMemoryExpanded)
		{
			EditorGUI.indentLevel = 1;

			var memory = behaviorContext.Target.memory;

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

			var state = behaviorContext.Target.state;

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

	public float GetPreferredWindowHeight()
	{
		const float propertyHeight = 18f;
		var totalHeight = 0f;

		// title height
		totalHeight += 16f;
		// padding
		totalHeight += 8f;
		totalHeight += propertyHeight*2f;

		if (behaviorContext == null)
			return totalHeight;

		if (_isMemoryExpanded && behaviorContext.IsAlive)
		{
			totalHeight += propertyHeight * behaviorContext.Target.memory.Count;
		}

		if (_isStateExpanded && behaviorContext.IsAlive)
		{
			totalHeight += propertyHeight * behaviorContext.Target.state.Count;
		}


		return totalHeight;
	}

	public void MovePosition(Vector2 positionDelta)
	{
		_windowRect.position += positionDelta;
	}

}


}

