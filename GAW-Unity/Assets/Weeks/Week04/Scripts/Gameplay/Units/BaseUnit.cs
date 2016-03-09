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
			if (_currentHealth == 0)
				_currentHealth = 20;
			Globals.playfield.AddUnit(this);
		}


		#region UnitInteractions

		public float attackSpeed = 2f;


		private float _timeSincePreviousAttack = 0f;

		protected BaseUnit _currentTarget;

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

					if (CanTarget(_currentTarget) == false && ShouldTarget(_currentTarget))
					{
						_currentTarget = null;
						state = UnitStates.Moving;
						goto default;
					}

					if (IsInRange(_currentTarget))
					{
						state = UnitStates.Attacking;
						goto case UnitStates.Attacking;
					}

					UpdateTargetting(dt);

					break;
				case UnitStates.Attacking:
					if (CanTarget(_currentTarget) == false && ShouldTarget(_currentTarget))
					{
						_currentTarget = null;
						state = UnitStates.Moving;
						goto default;
					}

					if (IsReadyForAttack())
					{
						AttackUnit(_currentTarget);
					}

					UpdateAttack(dt);

					break;
				default:
					break;
			}
		}

		protected abstract void UpdateMovement(float dt);
		protected abstract void UpdateTargetting(float dt);
		protected abstract void UpdateAttack(float dt);

		public bool ShouldTarget(BaseUnit potentialTarget)
		{
			if (faction == potentialTarget.faction)
				return false;


			// TODO calculate faction logic here
			return true;
		}


		public bool CanTarget(BaseUnit potentialTarget)
		{
			return potentialTarget.IsAlive();
		}

		public bool IsInRange(BaseUnit targetUnit)
		{
			if (Mathf.Abs(Globals.playfield.GetUnitPosition(targetUnit) - _currentPosition.x) < 3f)
				return true;
			
			return false;
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

			if (IsAlive() == false)
			{
				Log.Steb("Blergh unit died");
				OnUnitDeath();

			}
		}

		public void OnUnitDeath()
		{
			GameObject.Destroy(this.gameObject);
		}

		public BaseUnit GetNearTarget()
		{
			var potentialTargets = GetUnitsWithinCircularRange(5f, _currentPosition);

			for (int i = 0; i < potentialTargets.Length; i++)
			{
				if (ShouldTarget(potentialTargets[i]) && CanTarget(potentialTargets[i]))
					return potentialTargets[i];
			}
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

		private static Collider[] collidersForPhysicsTest = new Collider[40];
		
		protected BaseUnit[] GetUnitsWithinCircularRange(float radius, Vector3 position)
		{
			BaseUnit[] units;	

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
				if (colliders[i].CompareTag(UNIT_COLLIDER_TAG))
				{
					UnitBuffer.Add(colliders[i].GetComponent<UnitCollider>().unit);
				}
			}

			units = UnitBuffer.ToArray();
		}
	}
}