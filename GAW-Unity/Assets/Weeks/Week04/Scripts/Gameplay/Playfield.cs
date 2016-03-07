using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace Week04
{
	public class Playfield : MonoBehaviour
	{
		public bool drawDebug = false;

		public Dictionary<BaseUnit, float> unitPositions = new Dictionary<BaseUnit, float>();

		// Use this for early referencing
		private void Awake()
		{
			Globals.playfield = this;

			if (!Application.isEditor)
			{
				drawDebug = false;
			}
		}

		public void AddUnit(BaseUnit unit)
		{
			var initialPosition = unit.GetInitialPosition();

			AddUnit(unit, initialPosition);
		}

		public void AddUnit(BaseUnit unit, float initialPosition)
		{
			unitPositions.Add(unit, initialPosition);
		}

		public void ChangeUnitPosition(BaseUnit unit, float newPosition)
		{
			unitPositions[unit] = newPosition;
		}

		// Use this for initialization
		private void Start () 
		{
		
		}
		
		private readonly Vector3 debugLineEndPosition = new Vector3(100f,0,0);

		private const float tickmarkSpacing = 10f;

		private void LateUpdate () 
		{
			if ( drawDebug)
			{
				var dt = Time.deltaTime;

				Debug.DrawLine(Vector3.zero, debugLineEndPosition, Color.black, dt, true);

				for (float i = 0; i < debugLineEndPosition.x; i += tickmarkSpacing)
				{
					Debug.DrawRay(new Vector3(i, 0), Vector3.forward, Color.black, dt, false);
				}

				foreach (KeyValuePair<BaseUnit, float> unitPosition in unitPositions)
				{
					Debug.DrawLine(new Vector3(unitPosition.Value, 0f), unitPosition.Key._currentPosition, Color.gray, dt, true);
					DebugExtension.DebugCircle(unitPosition.Key._currentPosition, Color.yellow, 1f, dt, false);
				}
			}
		}
	}
}
