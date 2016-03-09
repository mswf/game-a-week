﻿using UnityEngine;
using System.Collections;

namespace Week04
{
	[SelectionBase]
	public class SimpleUnit : BaseUnit
	{

		public float movementSpeed = 1f;

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
