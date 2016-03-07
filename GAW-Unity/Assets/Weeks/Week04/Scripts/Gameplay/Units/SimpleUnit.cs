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


			_currentPosition.x += movementSpeed*dt;

			Globals.playfield.ChangeUnitPosition(this, _currentPosition.x);

			_transform.localPosition = _currentPosition;

		}
	}
}
