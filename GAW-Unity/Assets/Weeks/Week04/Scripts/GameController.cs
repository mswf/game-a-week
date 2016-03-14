using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Week04
{
	public class GameController : MonoBehaviour
	{

		public List<Faction> factions;

		public PlayerFaction localPlayerFaction;

		public List<UnitBuildInstructions> availablePlayerUnits; 

		private void Awake()
		{
			Globals.gameController = this;

			#region SetupAndroidScreen
			if (Screen.orientation == ScreenOrientation.Portrait ||
				Screen.orientation == ScreenOrientation.PortraitUpsideDown)
				Screen.orientation = ScreenOrientation.Landscape;
			
			Screen.orientation = ScreenOrientation.AutoRotation;

			Screen.autorotateToLandscapeLeft = true;
			Screen.autorotateToLandscapeRight = true;

			Screen.autorotateToPortrait = false;
			Screen.autorotateToPortraitUpsideDown = false;

			Screen.orientation = ScreenOrientation.AutoRotation;
			#endregion

			{
				var playerFaction = new PlayerFaction("Player Army")
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

#if UNITY_EDITOR
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

				localPlayerFaction = playerFaction;
				localPlayerFaction.availableUnits = availablePlayerUnits;
				Globals.playerFaction = playerFaction;
			}
		}

		private void OnEnable()
		{
			Globals.gameController = this;

			foreach (var faction in factions)
			{
				if (faction.controlType == ControlType.Player)
					Globals.playerFaction = (PlayerFaction) faction;
			}

		}

		// Use this for initialization
		private void Start()
		{
			Globals.UI.buildUnitButtonManager.OnOptionsChanged();
			Globals.UI.resourceDisplayManager.Initialize();

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