using System;
using UnityEngine;
using System.Collections;
//using UnityEditor;


[Serializable]
public class SceneViewSettings
{
	public bool trackGameCamera = false;
	public bool allowMouseInput = true;
}


[ExecuteInEditMode]
public class SceneCameraScript : MonoBehaviour
{
	private Camera _mainGameCamera;

	public SceneViewSettings[] sceneViewSettings;

	// Use this for early referencing
	private void Awake()
	{
		if (!Application.isEditor)
		{
			enabled = false;
			return;
		}

#if UNITY_EDITOR
		if (sceneViewSettings == null || sceneViewSettings.Length != UnityEditor.SceneView.sceneViews.Count)
			sceneViewSettings = new SceneViewSettings[UnityEditor.SceneView.sceneViews.Count];
#endif
	}

	private void Start()
	{
		if (!Application.isEditor)
		{
			enabled = false;
			return;
		}
	}

#if UNITY_EDITOR
	// Update is called once per frame
	private void LateUpdate ()
	{

		_mainGameCamera = Camera.main;

		var sceneViews = UnityEditor.SceneView.sceneViews;

		if (sceneViewSettings == null || sceneViewSettings.Length != sceneViews.Count)
			sceneViewSettings = new SceneViewSettings[sceneViews.Count];

		for (int i = 0; i < sceneViewSettings.Length; i++)
		{
			var sceneView = (UnityEditor.SceneView)sceneViews[i];
			bool needsRepaint = false;

			if (sceneViewSettings[i].trackGameCamera)
			{
				sceneView.pivot = _mainGameCamera.transform.position;
				sceneView.rotation = _mainGameCamera.transform.rotation;

				sceneView.AlignViewToObject(Camera.main.transform);
				needsRepaint = true;
			}

			if (!sceneViewSettings[i].allowMouseInput)
			{
				sceneView.wantsMouseMove = false;
			}
			
			if (needsRepaint)
				sceneView.Repaint();
			
		}
	}
#endif

}
