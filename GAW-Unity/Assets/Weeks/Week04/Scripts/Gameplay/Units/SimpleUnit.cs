using UnityEngine;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Week04.BehaviourTree;

using ContextIndex = System.String;


namespace Week04
{

	[SelectionBase]
	public class SimpleUnit : BaseUnit
	{
		public static EntryNode _DEBUGSTATIC_NODE;
		public static BehaviorContext _DEBUGSTATIC_BEHAVIORCONTEXT;


		protected override void InitBehaviourTree()
		{
			const string SUBJECT = "S_SUBJECT";
			const string TARGET = "UNIT_TARGET";
			const string TARGETS_STACK = "UNIT_TARGETS_STACK";

			const string POTENTIAL_TARGET = "UNIT_POTENTIAL_TARGET";

			const string RANGE_VAR = "F_RANGE";

			_behaviorContext = new BehaviorContext();
			_behaviorContext[SUBJECT] = this;

			behaviourTree = new EntryNode(
				new SelectorCompositeNode(
					new SequenceCompositeNode(
						// Get a target
						new SelectorCompositeNode(
							// Do we already have a target
							new SequenceCompositeNode(
								new ContainsUnit(TARGET),
								new CanTargetUnit(SUBJECT, TARGET),
								new CanHitUnit(SUBJECT, TARGET)
								//new PrintNode("Has preexisting target")

								),
							// If not, find a new target, this returns true if it found a target
							new SequenceCompositeNode(
								new ClearStack(TARGETS_STACK),
								new FindTargets(SUBJECT, TARGETS_STACK, RANGE_VAR),

								//new PrintNode("Starting Search"),


								new RepeatUntilFailDecoratorNode(
									new SequenceCompositeNode(

										new PopFromStack(POTENTIAL_TARGET, TARGETS_STACK),

										new InverterDecoratorNode(
											new SequenceCompositeNode(
												new ShouldTargetUnit(SUBJECT, POTENTIAL_TARGET),

												new CanTargetUnit(SUBJECT, POTENTIAL_TARGET),

												new CanHitUnit(SUBJECT, POTENTIAL_TARGET),

												// Passed all the checks, it's promoted to target now
												new SetVarTo<ContextIndex>(TARGET, POTENTIAL_TARGET)
											)
										)


									)
								),

								//new PrintNode("At the end of the road:"),
								//new PrintVarNode(TARGET),


								new InverterDecoratorNode(
									new IsNullNode(TARGET)
									)
								)
							// Couldn't find any target

							),
						// We could find a target
						// Now move in for the kill
						new SequenceCompositeNode(
							
							new MoveToUnit(SUBJECT, TARGET)
						)

					// 


					),
					new SequenceCompositeNode(
						new MoveNode(SUBJECT)
					)
				)
				);

			_behaviorContext[RANGE_VAR] = 5f;

			_DEBUGSTATIC_NODE = behaviourTree;
			_DEBUGSTATIC_BEHAVIORCONTEXT = _behaviorContext;

			var test = behaviourTree;
		}

		protected override void UpdateMovement(float dt)
		{
			if (faction.controlType == ControlType.Player)
			{
				var pFaction = (PlayerFaction) faction;

				if (pFaction.globalCommandState == PlayerFaction.CommandState.Advance)
					MoveUnitPosition(movementSpeed*dt);
				else
					MoveUnitPosition(-movementSpeed*dt);

			}
			else
			{
				MoveUnitPosition(movementSpeed*dt);
			}
		}

		protected override void UpdateTargetting(float dt)
		{
			var targetPosition = Globals.playfield.GetUnitPosition(_currentTarget);

			var ownPosition = GetUnitPositionX();

			var distanceToTarget = targetPosition - ownPosition;

			var movement = Mathf.Min(Mathf.Abs(distanceToTarget), movementSpeed*dt);

			MoveUnitPosition(movement * Mathf.Sign(distanceToTarget));
		}

		protected override void UpdateAttack(float dt)
		{
			
			//throw new System.NotImplementedException();
		}
	}
}
