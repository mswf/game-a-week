using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using Week04.BehaviourTree;


namespace Week04
{
	public enum UnitStates
	{
		Moving,
		Targetting,
		Attacking,
		Dead
	}

	public enum LootableStatus
	{
		Fortified,
		OpenForLooting,
		Emptied,
	}

	[SelectionBase]
	public abstract class BaseUnit : MonoBehaviour
	{
		[SerializeField]
		public Faction faction;

		[SerializeField]
		private Vector3 _currentPosition;
		public Transform _transform;

		#region Stats
		[Header("Stats")]
		[SerializeField, ReadOnlyAttribute]
		private int _currentHealth;

		[SerializeField, ReadOnlyAttribute]
		private UnitBuildInstructions _buildInstructions;

		public float movementSpeed = 1f;

		[Header("Loot")]
		[SerializeField, ReadOnlyAttribute]
		public ResourceContainer resourceContainer;
		private LootableStatus _lootableStatus = LootableStatus.Emptied;

		public LootableStatus GetLootableStatus()
		{
			return _lootableStatus;
		}

		public bool ContainsLoot()
		{
			return resourceContainer.ContainsLoot();
		}

		#endregion

		public UnitStates state = UnitStates.Moving;

		protected BehaviorContext _behaviorContext;

		public void InitializeUnit(Faction unitFaction, UnitBuildInstructions template)
		{
			faction = unitFaction;

			_currentHealth = template.health;

			_buildInstructions = template;


			if (_currentHealth == 0)
				_currentHealth = 200;
			Globals.playfield.AddUnit(this);

			InitBehaviourTree();
		}

		[SerializeField]
		protected EntryNode behaviourTree;

		protected virtual void InitBehaviourTree()
		{

			const string SUBJECT = "S_SUBJECT";

			_behaviorContext = new BehaviorContext();
			_behaviorContext[SUBJECT] = this;

			behaviourTree = new EntryNode(
					new IsNullNode("Ghello")
				);

		}

		// Set up all internal references to this 
		private void Awake()
		{
			_transform = transform;
			_currentPosition = _transform.localPosition;
		}

		private void Start()
		{
			if ( Globals.gameController.factions.Contains(faction) == false)
			{
				Debug.LogWarning("Unit without a faction!!!!");
				InitializeUnit(faction, new UnitBuildInstructions());
				_currentHealth = 200;
			}
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

			_transform.localPosition = _currentPosition;
			Globals.playfield.unitPositions[this] = _currentPosition.x;


			behaviourTree.UpdateTree(dt, _behaviorContext);


			if (true)
				return;

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
					if (CanTarget(_currentTarget) == false || IsInRange(_currentTarget) == false)
					{
						_currentTarget = null;
						state = UnitStates.Moving;
						goto default;
					}

					if (IsReadyForAttack())
					{
						DebugExtension.DebugArrow(_currentPosition, new Vector3(_currentTarget._currentPosition.x - _currentPosition.x, 0f), Color.red, 1f, false);

						AttackUnit(_currentTarget);
					}

					UpdateAttack(dt);

					break;
				case UnitStates.Dead:

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
			return potentialTarget.gameObject != null && (potentialTarget.IsAlive() || potentialTarget.ContainsLoot());
		}

		public bool IsInRange(BaseUnit targetUnit)
		{
			if (Mathf.Abs(targetUnit.GetUnitPositionX() - GetUnitPositionX()) < 3f)
				return true;

			return false;
		}

		public virtual bool IsReadyForAttack()
		{
			return _timeSincePreviousAttack >= attackSpeed;
		}

		public bool IsAlive()
		{
			if (_currentHealth > 0)
				return true;


			return false;
		}

		public Vector3 GetUnitPosition()
		{
			var xPosition = Globals.playfield.unitPositions[this];
			return new Vector3(xPosition, 0);
		}

		public float GetUnitPositionX()
		{
			return Globals.playfield.unitPositions[this];
		}

		public void SetUnitPosition(float newPosition)
		{
			_currentPosition.x = newPosition;

			Globals.playfield.unitPositions[this] = newPosition;
		}

		public void MoveUnitPosition(float positionDelta)
		{
			_currentPosition.x += positionDelta;

			Globals.playfield.unitPositions[this] += positionDelta;
		}


		public void AttackUnit(BaseUnit target)
		{
			if (target.IsAlive())
				target.ReceiveAttack(this);
			else
				resourceContainer += target.ReceiveLootStrike(this);


			_timeSincePreviousAttack = 0f;
		}

		public void ReceiveAttack(BaseUnit attacker)
		{
			_currentHealth -= 50;

			if (IsAlive() == false)
			{
				Log.Steb("Blergh unit died");
				OnDeath();
			}
		}

		public ResourceContainer ReceiveLootStrike(BaseUnit attacker)
		{
			var takenLoot = new ResourceContainer();

			Log.Steb("Getting a lootstrike " + _buildInstructions.unitName);

			foreach (ResourceType lootType in Enum.GetValues(typeof(ResourceType)))
			{
				var takenAmount = resourceContainer.Subtract(lootType, 30d);

				Log.Steb("Took " + takenAmount + " of type " + lootType.ToString());

				takenLoot.Add(lootType, takenAmount);
			}

			return takenLoot;
		}

		public void OnDeath()
		{
			//Globals.playfield.RemoveUnit(this);
			state = UnitStates.Dead;


			//GameObject.Destroy(this.gameObject);
		}

		public bool OnReadyToDestroy()
		{
			Globals.playfield.RemoveUnit(this);

			return true;
		}

		public BaseUnit GetNearTarget()
		{
			var potentialTargets = GetUnitsWithinCircularRange(5f, GetUnitPosition());

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

		public BaseUnit[] GetUnitsWithinCircularRange(float radius, Vector3 position)
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

		public BaseUnit[] GetUnitsWithinCircularRange(float radius)
		{
			return GetUnitsWithinCircularRange(radius, GetUnitPosition());
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