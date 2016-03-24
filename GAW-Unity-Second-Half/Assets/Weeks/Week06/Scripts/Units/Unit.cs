using UnityEngine;
using System.Collections.Generic;

using BehaviorTree;
using Week06.BehaviorTree;

namespace Week06
{
	public class Unit : BaseUnit 
	{
		public const string TREE_UNIT = "TREE_UNIT";

		private Squad _squad;

		public void Initialize(GameController gameController, Squad squad)
		{
			this._squad = squad;
		
			base.Initialize(gameController);
		}

		public const string U_SQUAD = "U_SQUAD";
		
		public const string STACK_POTENTIAL_CLUES = "U_SQUAD";


		protected override void InitializeBehaviorTree(BehaviorState bState)
		{
			if (bState.IsTreeDefined(TREE_UNIT) == false)
			{
				_behaviorTreeEntry = bState.AddTree(TREE_UNIT,

				new EntryNode(
					new SequenceCompositeNode(
						new SequenceCompositeNode(
							new ClearStack(STACK_POTENTIAL_CLUES),
							new FindVisualClues(U_SUBJECT, STACK_POTENTIAL_CLUES)
						),
						//new StubNode("GhellO")
						new FollowSquadNode(U_SUBJECT)

					)
				)


				);
			}
			else
			{
				_behaviorTreeEntry = bState.GetTree(TREE_UNIT);
			}
		}


		protected override void InitializeBehaviorContext(BehaviorContext behaviorContext)
		{
			base.InitializeBehaviorContext(behaviorContext);

			behaviorContext[U_SQUAD] = _squad;
			behaviorContext.DefineStack(STACK_POTENTIAL_CLUES);
		}

		public void MoveWithSquad()
		{
			navMeshAgent.destination = _squad._transform.position;
		}


		private Collider[] _colliderBuffer = new Collider[20];

		private RaycastHit[] _rayCastHitBuffer = new RaycastHit[3];

		private static readonly Color viewConeColor = new Color(1f,0,0,0.5f);

		public Clue[] GetPotentialClues()
		{

			var forward = _transform.forward;

			const float range = 10f;
			const string clueTag = "Clue";
			const float viewAngle = 30f;

			DebugExtension.DebugCone(_transform.position, forward*range, viewConeColor, viewAngle, Time.deltaTime);

			//DebugExtension.DebugWireSphere(_transform.position, Color.red, range, Time.deltaTime, false);

			var numColliders = Physics.OverlapSphereNonAlloc(_transform.position, range, _colliderBuffer);

			var ClueList = new List<Clue>();

			for (int i = 0; i < numColliders; i++)
			{
				if (_colliderBuffer[i].CompareTag(clueTag))
				{

					var directionToTarget = (_colliderBuffer[i].transform.position - _transform.position);

					if (Vector3.Angle(forward, directionToTarget) <= viewAngle)
					{
	

						var rayCastHits = Physics.RaycastNonAlloc(_transform.position, directionToTarget, _rayCastHitBuffer);

						for (int j = 0; j < 1; j++)
						{
							if (_rayCastHitBuffer[j].collider == _colliderBuffer[i])
							{
								Debug.DrawLine(_transform.position, _colliderBuffer[i].transform.position, Color.magenta, Time.deltaTime);
								ClueList.Add(_colliderBuffer[i].GetComponent<Clue>());
							}
						}
					}





				}
			}


			return ClueList.ToArray();
		}
	}
}
