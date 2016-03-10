using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Week04
{
	public class CommandButton : Button
	{
		private Text _text;

		// Use this for initialization
		protected override void Awake()
		{
			base.Awake();
			if (!Application.isPlaying)
				return;

			_text = GetComponentInChildren<Text>();
		}

		protected override void Start()
		{
			base.Start();
			if (!Application.isPlaying)
				return;
			SetToState(Globals.playerFaction.globalCommandState);
		}

		// Update is called once per frame
		private void Update()
		{
			if (!Application.isPlaying)
				return;

			if (_currentState != Globals.playerFaction.globalCommandState)
				SetToState(Globals.playerFaction.globalCommandState);
		}


		private PlayerFaction.CommandState _currentState;

		private void SetToState(PlayerFaction.CommandState newState)
		{
			_currentState = newState;

			if (newState == PlayerFaction.CommandState.Advance)
				SetToRetreat();
			else if (newState == PlayerFaction.CommandState.Retreat)
				SetToAdvance();

		}


		private void SetToAdvance()
		{
			_text.text = "Advance";
		}

		private void SetToRetreat()
		{
			_text.text = "Retreat";

		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);

			Globals.playerFaction.ToggleGlobalCommandState();
			SetToState(Globals.playerFaction.globalCommandState);
		}

		

	}
}
