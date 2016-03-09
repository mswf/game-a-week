using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Week04
{
	public enum UnitStates
	{
		Moving,
		Targetting,
		Attacking
	}

	[SelectionBase]
	public abstract class BaseUnit : MonoBehaviour
	{
		[SerializeField]
		public Faction faction;
		
		public Vector3 _currentPosition;
		public Transform _transform;

		#region Stats
		[Header("Stats")]
		[SerializeField, ReadOnlyAttribute]
		private int _currentHealth;

		[SerializeField, ReadOnlyAttribute]
		private UnitBuildInstructions _buildInstructions;

		#endregion

		public UnitStates state = UnitStates.Moving;

		public void InitializeUnit(Faction unitFaction, UnitBuildInstructions template)
		{
			faction = unitFaction;

			_currentHealth = template.health;

			_buildInstructions = template;
		}

		// Set up all internal references to this 
		private void Awake()
		{
			_transform = transform;
			_currentPosition = _transform.localPosition;
		}

		// Use this for initialization
		private void Start()
		{

			Globals.playfield.AddUnit(this);
		}


		#region UnitInteractions

		public float attackSpeed = 2f;


		private float _timeSincePreviousAttack = 0f;

		private BaseUnit _currentTarget;

		// Update is called once per frame
		private void Update()
		{
			var dt = Time.deltaTime;

			_timeSincePreviousAttack += dt;

			switch (state)
			{
				case UnitStates.Moving:
					_currentTarget = GetNearTarget();

					if (_currentTarget != null)
					{
						state = UnitStates.Targetting;
						goto case UnitStates.Targetting;
					}

					UpdateMovement(dt);

					break;
				case UnitStates.Targetting:

					if (CanTarget(_currentTarget) == false)
					{
						Log.Steb("Cant target anymore");
						_currentTarget = null;
						state = UnitStates.Moving;
						goto default;
					}

					if (IsReadyForAttack())
					{
						AttackUnit(_currentTarget);
					}

					break;
				case UnitStates.Attacking:
					break;
				default:
					break;
			}
		}

		protected abstract void UpdateMovement(float dt);
		protected abstract void UpdateTargetting(float dt);
		protected abstract void UpdateAttack(float dt);
		

		public bool CanTarget(BaseUnit target)
		{
			return target.IsAlive();
		}

		public bool IsReadyForAttack()
		{
			return _timeSincePreviousAttack >= attackSpeed;
		}

		public bool IsAlive()
		{
			if (_currentHealth > 0)
				return true;
			

			return false;
		}

		public void AttackUnit(BaseUnit target)
		{
			target.ReceiveAttack(this);

			_timeSincePreviousAttack = 0f;
		}

		public void ReceiveAttack(BaseUnit attacker)
		{
			_currentHealth -= 5;

			if (_currentHealth <= 0)
			{
				Log.Steb("Blergh unit died");
			}
		}

		public BaseUnit GetNearTarget()
		{
			var potentialTargets = GetUnitsWithinCircularRange(5f, _currentPosition);

			if (potentialTargets.Length == 0)
				return null;

			if (potentialTargets[0] != this)
				return potentialTargets[0];
			else if (potentialTargets.Length > 1)
				return potentialTargets[1];
			else
				return null;

		}

		#endregion

		public float GetInitialPosition()
		{
			if (_currentPosition.x != 0f)
			{
				_currentPosition.x = transform.position.x;
			}
			return _currentPosition.x;
		}
		
		protected BaseUnit[] GetUnitsWithinCircularRange(float radius, Vector3 position)
		{
			BaseUnit[] units;
			
			Collider[] collidersForPhysicsTest = new Collider[10];

			int numCollidersInRange = Physics.OverlapSphereNonAlloc(position, radius, collidersForPhysicsTest);

			var testVar = collidersForPhysicsTest;

			DebugExtension.DebugWireSphere(position, Color.red, radius, Time.deltaTime, true);


			if (numCollidersInRange > 0)
				FilterAllUnits(ref collidersForPhysicsTest, out units, numCollidersInRange);
			else
				units = new BaseUnit[0];

			return units;
		}

		protected BaseUnit[] GetUnitsWithinCircularRange(float radius)
		{
			return GetUnitsWithinCircularRange(radius, _currentPosition);
		}

		public const string UNIT_COLLIDER_TAG = "Unit";
		private static readonly List<BaseUnit> UnitBuffer = new List<BaseUnit>(10);

		public static void FilterAllUnits(ref Collider[] colliders, out BaseUnit[] units, int colliderCount = -1)
		{
			UnitBuffer.Clear();

			if (colliderCount == -1)
			{
				colliderCount = colliders.Length;
			}

			for (int i = 0; i < colliderCount; i++)
			{
				//Log.Steb(colliders[i].tag);
				if (colliders[i].CompareTag(UNIT_COLLIDER_TAG))
				{
					//Log.Steb("Found unit with tag");
					UnitBuffer.Add(colliders[i].GetComponent<UnitCollider>().unit);
				}
			}

			units = UnitBuffer.ToArray();
		}
	}
}