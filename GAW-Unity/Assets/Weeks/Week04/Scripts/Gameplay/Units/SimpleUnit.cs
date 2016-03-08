using UnityEngine;
using System.Collections;

namespace Week04
{
	public class SimpleUnit : BaseUnit
	{

		public float movementSpeed = 5f;

		// Update is called once per frame
		private void Update ()
		{
			var dt = Time.deltaTime;

			if (faction.controlType == ControlType.Player)
			{
				var pFaction = (PlayerFaction) faction;

				if (pFaction.globalCommandState == PlayerFaction.CommandState.Advance)
					_currentPosition.x += movementSpeed * dt;
				else
					_currentPosition.x -= movementSpeed * dt;

			}
			else
			{
				_currentPosition.x += movementSpeed*dt;
			}

			Globals.playfield.ChangeUnitPosition(this, _currentPosition.x);

			_transform.localPosition = _currentPosition;

		}
	}
}
