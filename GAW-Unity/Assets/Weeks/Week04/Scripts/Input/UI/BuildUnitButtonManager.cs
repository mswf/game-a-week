using UnityEngine;
using System.Collections;

namespace Week04
{
	public class BuildUnitButtonManager : MonoBehaviour
	{
		public BuildUnitButton[] buildUnitButtons;

		protected void Awake()
		{
			Globals.UI.buildUnitButtonManager = this;
		}

		protected void OnEnable()
		{
			Globals.UI.buildUnitButtonManager = this;

		}

		public void OnOptionsChanged()
		{
			foreach (var buildUnitButton in buildUnitButtons)
			{
				buildUnitButton.SetVisible(false);
			}

			var instructions = Globals.playerFaction.availableUnits;
			var instructionsCount = instructions.Count;
			for (int i = 0; i < instructionsCount; i++)
			{
				buildUnitButtons[i].SetVisible(true);
				buildUnitButtons[i].SetToInstructions(instructions[i]);
			}
		}
	}
}
