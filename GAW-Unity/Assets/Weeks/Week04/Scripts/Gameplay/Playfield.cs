using System;
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

		private void OnEnable()
		{
			Globals.playfield = this;
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

		public float GetUnitPosition(BaseUnit unit)
		{
			return unitPositions[unit];
		}

		// Use this for initialization
		private void Start () 
		{
		
		}
		
		private readonly Vector3 debugLineEndPosition = new Vector3(100f,0,0);

		private const float tickmarkSpacing = 10f;


		private static readonly Color MovingColor = Color.green;
		private static readonly Color TargettingColor = Color.yellow;
		private static readonly Color AttackingColor = Color.red;
		private static readonly Color DeadColor = Color.black;


		private void LateUpdate () 
		{
			if ( drawDebug)
			{
				var dt = Time.deltaTime;

				Debug.DrawLine(Vector3.zero, debugLineEndPosition, Color.black, dt, true);

				for (float i = 0; i < debugLineEndPosition.x; i += tickmarkSpacing)
				{
					Debug.DrawRay(new Vector3(i, 0), Vector3.forward*3f, Color.black, dt, false);
				}

				foreach (KeyValuePair<BaseUnit, float> unitPosition in unitPositions)
				{
					Debug.DrawLine(new Vector3(unitPosition.Value, 0f), unitPosition.Key.GetUnitPosition(), Color.gray, dt, true);
					const float inner_0 = 1f;
					const float inner_1 = 0.98f;
					const float inner_2 = 0.96f;
					
					switch (unitPosition.Key.state)
					{
						case UnitStates.Moving:
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), MovingColor, inner_0, dt, false);
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), MovingColor, inner_1, dt, false);
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), MovingColor, inner_2, dt, false);

							break;
						case UnitStates.Targetting:
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), TargettingColor, inner_0, dt, false);
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), TargettingColor, inner_1, dt, false);
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), TargettingColor, inner_2, dt, false);
							break;
						case UnitStates.Attacking:
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), AttackingColor, inner_0, dt, false);
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), AttackingColor, inner_1, dt, false);
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), AttackingColor, inner_2, dt, false);
							break;
						default:
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), DeadColor, inner_0, dt, false);
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), DeadColor, inner_1, dt, false);
							DebugExtension.DebugCircle(unitPosition.Key.GetUnitPosition(), DeadColor, inner_2, dt, false);
							break;
							throw new ArgumentOutOfRangeException();
					}

				}
			}
		}

		public void RemoveUnit(BaseUnit baseUnit)
		{
			unitPositions.Remove(baseUnit);
		}
	}
}
