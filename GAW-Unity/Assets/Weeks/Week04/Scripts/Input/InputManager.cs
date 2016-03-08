using UnityEngine;
using System.Collections;

namespace Week04
{
	public enum InputState
	{
		GameMenu,
		Ingame
	}

	public class InputManager : MonoBehaviour 
	{
		public InputState currentState = InputState.Ingame;


		// Use this for early referencing
		private void Awake()
		{
			Globals.inputManager = this;

		}

		private void OnEnable()
		{
			Globals.inputManager = this;
		}

		// Use this for initialization
		private void Start () 
		{
		
		}
		
		// Update is called once per frame
		private void Update () 
		{
			if (currentState == InputState.Ingame)
			{
				Globals.cameraController.UpdateInput();
			}
		}

		public bool IsCameraMovable()
		{
			if (currentState == InputState.Ingame)
			{
				return true;
			}
			return false;
		}
	}
}
