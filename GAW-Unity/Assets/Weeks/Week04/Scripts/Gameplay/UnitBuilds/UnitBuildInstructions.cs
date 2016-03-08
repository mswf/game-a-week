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
		public double health = 20;
		public double buildCost = 100;

		[Header("Visual")]
		public GameObject prefab;
	}
}
