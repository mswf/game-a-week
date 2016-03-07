using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Week04
{
	public class GameController : MonoBehaviour
	{

		public List<Faction> factions;

		private void Awake()
		{
			#region SetupAndroidScreen
			Screen.orientation = ScreenOrientation.LandscapeRight;
			Screen.orientation = ScreenOrientation.AutoRotation;

			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = true;

			Screen.autorotateToPortrait = false;
			Screen.autorotateToPortraitUpsideDown = false;

			Screen.orientation = ScreenOrientation.AutoRotation;
			#endregion

			Globals.gameController = this;


			{
				var playerFaction = new Faction("Player Army")
				{
					controlType = ControlType.Player,
					teamColor = Color.cyan
				};

				var civilianFaction = new Faction("The Farmers")
				{
					controlType = ControlType.Civilian,
					teamColor = Color.green
				};

				var defenderFaction = new Faction("The Royal Army")
				{
					controlType = ControlType.King,
					teamColor = Color.yellow
				};

				playerFaction.DefineRelationshipTo(civilianFaction, RelationType.Prey);
				playerFaction.DefineRelationshipTo(defenderFaction, RelationType.Hostile);

				civilianFaction.DefineRelationshipTo(playerFaction, RelationType.Unknown);
				civilianFaction.DefineRelationshipTo(defenderFaction, RelationType.Friendly);

				defenderFaction.DefineRelationshipTo(playerFaction, RelationType.Hostile);
				defenderFaction.DefineRelationshipTo(civilianFaction, RelationType.Neutral);

				factions.Add(playerFaction);
				factions.Add(civilianFaction);
				factions.Add(defenderFaction);

				#if DEBUG
				foreach (var faction in factions)
				{
					if (faction != playerFaction)
						Assert.IsTrue(playerFaction.relationships.ContainsKey(faction));
					if (faction != civilianFaction)
						Assert.IsTrue(civilianFaction.relationships.ContainsKey(faction));
					if (faction != defenderFaction)
						Assert.IsTrue(defenderFaction.relationships.ContainsKey(faction));
				}
				#endif
			}
		}

		// Use this for initialization
		private void Start()
		{
	
		}

		// Update is called once per frame
		private void Update()
		{

		}

		public void OnDestroy()
		{
			Globals.Reset();
		}
	}
}