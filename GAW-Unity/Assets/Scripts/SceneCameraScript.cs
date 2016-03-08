using System;
using UnityEngine;
using System.Collections;
using UnityEditor;


[Serializable]
public class SceneViewSettings
{
	public bool trackGameCamera = false;
	public bool allowMouseInput = true;
}


[ExecuteInEditMode]
public class SceneCameraScript : MonoBehaviour
{
	private Camera _sceneViewCamera;
	private Camera _mainGameCamera;

	public SceneViewSettings[] sceneViewSettings;

	public bool trackGame = false;


	// Use this for early referencing
	private void Awake()
	{
		if (sceneViewSettings.Length != SceneView.sceneViews.Count)
			sceneViewSettings = new SceneViewSettings[SceneView.sceneViews.Count];
	}

	// Update is called once per frame
	private void Update ()
	{
		_mainGameCamera = Camera.main;

		var sceneViews = SceneView.sceneViews;

		if (sceneViewSettings == null || sceneViewSettings.Length != SceneView.sceneViews.Count)
			sceneViewSettings = new SceneViewSettings[SceneView.sceneViews.Count];

		for (int i = 0; i < sceneViewSettings.Length; i++)
		{
			var sceneView = (SceneView)sceneViews[i];
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
}
