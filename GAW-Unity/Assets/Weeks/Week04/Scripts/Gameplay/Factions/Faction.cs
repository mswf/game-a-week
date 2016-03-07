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

		public ControlType controlType;
		public Color teamColor = Color.gray;

		[SerializeField]
		public Dictionary<Faction, Relationship> relationships;

		public Faction(string factionName)
		{
			this.name = factionName;
			relationships = new Dictionary<Faction, Relationship>();
		}

		public Relationship DefineRelationshipTo(Faction otherFaction, RelationType currentRelation)
		{
			var newRelationship = new Relationship(this, otherFaction, currentRelation);

			relationships.Add(otherFaction, newRelationship);
			
			return newRelationship;
		}
	}
}
