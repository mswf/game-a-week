
//#define DEBUG_W04

using UnityEngine;
using System.Collections;

namespace Week04
{

	public static class Globals
	{
		public static GameController gameController;

		public static InputManager inputManager;	
		public static WorldInput worldInput;
		public static CameraController cameraController;
		
		public static Playfield playfield;


		public static void Reset()
		{
			gameController = null;

			inputManager = null;
			worldInput = null;
			cameraController = null;

			playfield = null;
		}
	}

	public enum ScrollDirection
	{
		ToLeft,
		ToRight
	}
}
