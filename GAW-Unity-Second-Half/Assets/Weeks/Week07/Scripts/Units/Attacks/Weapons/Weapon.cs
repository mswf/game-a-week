using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Week07
{
	public abstract class Weapon : SteffBehaviour 
	{
		// Use this for early referencing
		protected override void Awake()
		{
			base.Awake();


		}

		protected bool _isAttacking = false;

		public bool isAttacking
		{
			get { return _isAttacking; }
		}

		public abstract void DoAttack();

		public abstract bool IsReadyForAttack();


		protected List<HitCollider> _collidersHit  = new List<HitCollider>();

		protected void AttackCollider(HitCollider collider)
		{
			if (_collidersHit.Contains(collider) == false)
			{
				_collidersHit.Add(collider);

				collider.RegisterHit(this);
			}
		}
		 
	}
}
