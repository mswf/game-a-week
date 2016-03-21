using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property,
											GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	public override void OnGUI(Rect position,
							   SerializedProperty property,
							   GUIContent label)
	{
		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true;
	}
}

[CustomPropertyDrawer(typeof(ShowOnlyIfNotNull))]
public class ShowOnlyIfNotNullDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property,
											GUIContent label)
	{
		if (ShouldDraw(property, label))
			return EditorGUI.GetPropertyHeight(property, label, true);
		else
			return 0f;

		//Debug.Log(property.type);
	}

	public override void OnGUI(Rect position,
							   SerializedProperty property,
							   GUIContent label)
	{
		GUI.enabled = false;

		if (ShouldDraw(property, label))
			EditorGUI.PropertyField(position, property, label, true);

		GUI.enabled = true;
	}

	public bool ShouldDraw(SerializedProperty property, GUIContent label)
	{
		var type = property.type;

		if (type == "PPtr<$BaseUnit>")
		{
			if (property.objectReferenceInstanceIDValue != 0)
				return true;
			else
				return false;
		}
		else if (type == "float")
		{
			if (property.floatValue == float.PositiveInfinity)
				return false;
			else
				return true;
		}
		else if (type == "string")
		{
			if (property.stringValue == "")
				return false;
			else
				return true;
		}
		else
		{
			Debug.Log(type);
		}

		return true;
	}
}