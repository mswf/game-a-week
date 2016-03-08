using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Week04
{
	public class BuildUnitButton : Button
	{

		private Text _descriptionText;

		private bool _isEnabled = true;
		private bool _isVisible = true;

		// Use this for early referencing
		protected override void Awake()
		{
			base.Awake();

			_descriptionText = GetComponentInChildren<Text>();

			SetVisible(false);
		}
		
		// Update is called once per frame
		private void Update () 
		{
		
		}

		public void SetEnabled(bool isEnabled)
		{
			_isEnabled = isEnabled;

			interactable = isEnabled && _isVisible;


		}

		public void SetVisible(bool isVisible)
		{
			_isVisible = isVisible;

			interactable = _isEnabled && _isVisible;

			targetGraphic.enabled = isVisible;
			_descriptionText.enabled = isVisible;
		}


		private UnitBuildInstructions _currentBuildInstructions;
		public void SetToInstructions(UnitBuildInstructions unitBuildInstructions)
		{
			_currentBuildInstructions = unitBuildInstructions;
			_descriptionText.text = unitBuildInstructions.unitName;
		}


		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);

			Globals.playerFaction.BuildUnit(_currentBuildInstructions);
		}
	}
}
