using UnityEngine;
using System.Collections;

namespace Week04
{
	public enum RelationType
	{
		Friendly,
		Neutral,
		Hostile,
		Prey,
		Unknown
	}

	[System.Serializable]
	public class Relationship // : UnityEngine.Object
	{

		[Header("Debug Variables")]
		[ReadOnlyAttribute]
		public string fromFactionName;
		[ReadOnlyAttribute]
		public string toFactionName;

		[SerializeField]
		public RelationType currentRelation;

		[Header("The Rest")]
		[ReadOnlyAttribute]
		public Faction fromFaction;
		[ReadOnlyAttribute]
		public Faction toFaction;

		public Relationship(Faction fromFaction, Faction toFaction, RelationType currentRelation)
		{
			this.fromFaction = fromFaction;
			this.toFaction = toFaction;

			fromFactionName = fromFaction.name;
			toFactionName = toFaction.name;

			this.currentRelation = currentRelation;
		}

		public void ChangeCurrentRelation(RelationType newRelation)
		{
			currentRelation = newRelation;
		}
	}
}
