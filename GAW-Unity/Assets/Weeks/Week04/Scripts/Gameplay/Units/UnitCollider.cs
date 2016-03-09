using UnityEngine;
using System.Collections;

namespace Week04
{
	[RequireComponent(typeof(Collider))]
	public class UnitCollider : MonoBehaviour
	{
		public BaseUnit unit;

		protected void Awake()
		{
#if UNITY_EDITOR

			if (unit == null)
			{
				Debug.LogError("No unit assigned to collider!");
			}
#endif
		}
	}
}
