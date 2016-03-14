
using UnityEngine;
using System.Collections;

namespace Week04
{
	public static class Globals
	{
		public class UIGlobals
		{
			public BuildUnitButtonManager buildUnitButtonManager;
			public ResourceDisplayManager resourceDisplayManager;
		}

		public static UIGlobals UI = new UIGlobals();

		public static GameController gameController;

		public static PlayerFaction playerFaction;

		public static InputManager inputManager;
		public static WorldInput worldInput;
		public static CameraController cameraController;

		public static Playfield playfield;
		public static UnitBuilder unitBuilder;


		public static void Reset()
		{
			gameController = null;

			inputManager = null;
			worldInput = null;
			cameraController = null;

			playfield = null;

			UI = new UIGlobals();
		}
	}

	public enum ScrollDirection
	{
		ToLeft,
		ToRight
	}
}
