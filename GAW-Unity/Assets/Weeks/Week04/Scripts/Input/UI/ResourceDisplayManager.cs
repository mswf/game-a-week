using System;
using UnityEngine;
using System.Collections;

namespace Week04
{
	public class ResourceDisplayManager : MonoBehaviour 
	{
		public ResourceDisplay[] resourceDisplays;

		private bool _isInitted = false;

		protected void Awake()
		{
			Globals.UI.resourceDisplayManager = this;
		}

		protected void OnEnable()
		{
			Globals.UI.resourceDisplayManager = this;

		}

		public void UpdateDisplays()
		{
			if (!_isInitted)
				return;

			foreach (var buildUnitButton in resourceDisplays)
			{
				buildUnitButton.UpdateDisplay();
			}
		}

		public void Initialize()
		{
			foreach (var buildUnitButton in resourceDisplays)
			{
				buildUnitButton.SetVisible(false);
			}


			var instructionsCount = 0;

			foreach (ResourceType lootType in Enum.GetValues(typeof(ResourceType)))
			{
				resourceDisplays[instructionsCount].SetVisible(true);
				resourceDisplays[instructionsCount].SetToResource(lootType);

				instructionsCount++;
			}
		}
	}
}
