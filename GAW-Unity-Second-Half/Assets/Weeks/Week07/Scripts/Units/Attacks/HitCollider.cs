using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Week07
{
	[RequireComponent(typeof(Collider))]
	public class HitCollider : SteffBehaviour
	{
		public static int layerMask = LayerMask.NameToLayer("HitCollider");

		[HideInInspector]
		public Collider _collider;
		protected override void Awake()
		{
			base.Awake();
			_collider = GetComponent<Collider>();

			gameObject.layer = layerMask;
		}

		public virtual void RegisterHit(Weapon attackingWeapon)
		{
			Debug.Log(this + " Got Hit!");

			DebugExtension.DebugArrow(attackingWeapon._transform.position,
				_transform.position - attackingWeapon._transform.position, Color.red, Time.fixedDeltaTime, false);

			var offset = (_transform.position - attackingWeapon._transform.position);
			offset.y = 0;
			offset.Normalize();

			Camera.main.DOShakePosition(0.5f, 1f, 50);
			_transform.Translate(offset);
		}

		

	}
}
