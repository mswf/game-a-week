using UnityEngine;
using System.Collections;

namespace Week04
{
	public abstract class BaseBuilding : BaseUnit 
	{
		protected override void UpdateMovement(float dt)
		{
			
		}

		protected override void UpdateTargetting(float dt)
		{
			
		}

		protected override void UpdateAttack(float dt)
		{

		}

		public override bool IsReadyForAttack()
		{
			return false;
		}
	}
}
