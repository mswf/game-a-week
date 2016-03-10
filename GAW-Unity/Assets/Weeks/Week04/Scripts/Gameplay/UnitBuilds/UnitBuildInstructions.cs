using UnityEngine;
using System.Collections;

namespace Week04
{

	public enum UnitType
	{
		Simple
	}

	[System.Serializable]
	public class UnitBuildInstructions
	{
		[Header("Definition")]
		public string unitName;

		[Multiline]
		public string description;

		public UnitType type;

		[Header("Stats")]
		public int health = 200;
		public double buildCost = 100;

		public float attackSpeed = 2f;
		public float attackPower = 20f;


		[Header("Visual")]
		public GameObject prefab;
	}
}
