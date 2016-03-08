using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Week04
{
	public enum ControlType
	{
		Player,
		Civilian,
		King
	}

	[System.Serializable]
	public class Faction // : UnityEngine.Object
	{
		public string name;

		public ControlType controlType = ControlType.Civilian;
		public Color teamColor = Color.gray;

		[SerializeField]
		public Dictionary<Faction, Relationship> relationships;

		public Faction(string factionName)
		{
			this.name = factionName;
			relationships = new Dictionary<Faction, Relationship>();
		}

		protected Faction()
		{
			this.name = "Default Name";
			relationships = new Dictionary<Faction, Relationship>();
		}

		public Relationship DefineRelationshipTo(Faction otherFaction, RelationType currentRelation)
		{
			var newRelationship = new Relationship(this, otherFaction, currentRelation);

			relationships.Add(otherFaction, newRelationship);

			return newRelationship;
		}
	}

	[System.Serializable]
	public class PlayerFaction : Faction
	{
		public enum CommandState
		{
			Advance,
			Retreat
		}

		public PlayerFaction(string factionName) : base()
		{
			this.name = factionName;
			relationships = new Dictionary<Faction, Relationship>();
		}

		public CommandState globalCommandState = CommandState.Advance;

		public void ToggleGlobalCommandState()
		{
			if (globalCommandState == CommandState.Advance)
				globalCommandState = CommandState.Retreat;
			else if (globalCommandState == CommandState.Retreat)
				globalCommandState = CommandState.Advance;
			
		}

		public List<UnitBuildInstructions> availableUnits;

		public void BuildUnit(UnitBuildInstructions currentBuildInstructions)
		{
			var newUnit = Globals.unitBuilder.buildUnit(Globals.gameController.transform, currentBuildInstructions, this);

		}
	}
}
