using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;

namespace Week02
{

	public class SimpleVariableStorage : VariableStorageBehaviour
	{
		// Where we actually keeping our variables
		private Dictionary<string, float> _variables = new Dictionary<string, float>();

		// A default value to apply when the object wakes up, or 
		// when ResetToDefaults is called
		[System.Serializable]   
		public class DefaultVariable
		{
			public string name;
			public float value;
		}

		// Our list of default variables, for debugging.
		public DefaultVariable[] defaultVariables;

		[Header("Optional debugging tools")]
		// A UI.Text that can show the current list of all variables. Optional.
		public UnityEngine.UI.Text debugTextView;

		// Reset to our default values when the game starts
		void Awake()
		{
			ResetToDefaults();
		}

		// Erase all variables and reset to default values
		public override void ResetToDefaults()
		{
			Clear();

			foreach (var variable in defaultVariables)
			{
				SetNumber("$" + variable.name, variable.value);
			}
		}

		// Set a variable's value
		public override void SetNumber(string variableName, float number)
		{
			_variables[variableName] = number;
		}

		// Get a variable's value, or 0.0 if it doesn't exist
		public override float GetNumber(string variableName)
		{
			float value = 0.0f;
			if (_variables.ContainsKey(variableName))
			{

				value = _variables[variableName];

			}
			return value;
		}

		// Erase all variables
		public override void Clear()
		{
			_variables.Clear();
		}

		// If we have a debug view, show the list of all variables in it
		void Update()
		{
			if (debugTextView != null)
			{
				var stringBuilder = new System.Text.StringBuilder();
				foreach (KeyValuePair<string, float> item in _variables)
				{
					stringBuilder.AppendLine(string.Format("{0} = {1}",
															 item.Key,
															 item.Value));
				}
				debugTextView.text = stringBuilder.ToString();
			}
		}
	}


}
