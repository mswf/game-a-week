using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Week04
{
	public class UnitBuilder : MonoBehaviour
	{
		public Dictionary<UnitType, GameObject> unitBuilds = new Dictionary<UnitType, GameObject>(); 

		public GameObject buildUnit(Transform spawn, UnitBuildInstructions buildInstructions, Faction faction)
		{
			GameObject unit;

			if (buildInstructions.prefab != null)
				unit = GameObject.Instantiate(buildInstructions.prefab);
			else
				unit = GameObject.Instantiate(unitBuilds[buildInstructions.type]);

			var unitComponent = unit.GetComponent<BaseUnit>();
			unitComponent.InitializeUnit(faction);

			return unit;
		}


		// Use this for early referencing
		private void Awake()
		{
			Globals.unitBuilder = this;
		}

		private void OnEnable()
		{
			Globals.unitBuilder = this;
		}

		// Use this for initialization
		private void Start () 
		{
		
		}
		
		// Update is called once per frame
		private void Update () 
		{
		
		}
	}
}
