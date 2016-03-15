﻿using UnityEngine;
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
		protected override void InitBehaviourTree()
		{
			const string SUBJECT = Node.S_SUBJECT;
			const string TARGET = "UNIT_TARGET";
			const string TARGETS_STACK = "UNIT_TARGETS_STACK";

			const string POTENTIAL_TARGET = "UNIT_POTENTIAL_TARGET";

			const string RANGE_VAR = "F_RANGE";

			behaviourTree = new EntryNode(this,
				new SelectorCompositeNode(
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

									//new PrintNode("Testing"),
									//new PrintVarNode(POTENTIAL_TARGET),


									new ShouldTargetUnit(SUBJECT, POTENTIAL_TARGET),

									new PrintNode("1"),


									new CanTargetUnit(SUBJECT, POTENTIAL_TARGET),
									new PrintNode("2"),

									new CanHitUnit(SUBJECT, POTENTIAL_TARGET),
									new PrintNode("3"),

									// Passed all the checks, it's promoted to target now
									new SetVarTo<ContextIndex>(TARGET, POTENTIAL_TARGET)
									)
								),

							//new PrintNode("At the end of the road:"),
							//new PrintVarNode(TARGET),


							new InverterDecoratorNode(
								new IsNullNode(POTENTIAL_TARGET)
								)
							)
							// Couldn't find any target

						),
					// We could find a target
					// Now move in for the kill
					new SequenceCompositeNode(
						new PrintNode("Has a target"),

						new MoveToUnit(SUBJECT, TARGET)
					)

				// 


				));

			behaviourTree._context[RANGE_VAR] = 5f;

			var test = behaviourTree;
		}

		protected override void UpdateMovement(float dt)
		{
			if (faction.controlType == ControlType.Player)
			{
				var pFaction = (PlayerFaction)faction;

				if (pFaction.globalCommandState == PlayerFaction.CommandState.Advance)
					_currentPosition.x += movementSpeed * dt;
				else
					_currentPosition.x -= movementSpeed * dt;

			}
			else
			{
				_currentPosition.x += movementSpeed * dt;
			}

			Globals.playfield.ChangeUnitPosition(this, _currentPosition.x);

			_transform.localPosition = _currentPosition;
		}

		protected override void UpdateTargetting(float dt)
		{
			var targetPosition = Globals.playfield.GetUnitPosition(_currentTarget);

			var ownPosition = _currentPosition.x;

			var distanceToTarget = targetPosition - ownPosition;

			var movement = Mathf.Min(Mathf.Abs(distanceToTarget), movementSpeed*dt);

			_currentPosition.x += movement * Mathf.Sign(distanceToTarget);
			Globals.playfield.ChangeUnitPosition(this, _currentPosition.x);

			_transform.localPosition = _currentPosition;
		}

		protected override void UpdateAttack(float dt)
		{
			
			//throw new System.NotImplementedException();
		}
	}
}
