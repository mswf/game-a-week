using UnityEngine;
using UnityEditor;

namespace Week01
{
	[CustomEditor(typeof(PoissonDiskCreator))]
	public class PoissonDiskCreatorInspector : Editor
	{
		private PoissonDiskCreator creator;

		private void OnEnable()
		{
			creator = target as PoissonDiskCreator;
			Undo.undoRedoPerformed += RefreshCreator;
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= RefreshCreator;
		}

		private void RefreshCreator()
		{
			if (Application.isPlaying)
			{
				creator.Refresh();
			}
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			DrawDefaultInspector();
			if (EditorGUI.EndChangeCheck() && Application.isPlaying)
			{
				RefreshCreator();
			}
		}
	}


}
