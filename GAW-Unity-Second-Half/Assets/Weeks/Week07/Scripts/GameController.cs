using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Week07
{
	public class GameController : MonoBehaviour 
	{
		// Use this for early referencing
		private void Awake()
		{

		}

		// Use this for initialization
		private void Start () 
		{
			GameGlobals.levelGenerator.CreateInitialLevel();
		}
		
		// Update is called once per frame
		private void Update () 
		{
		
		}
	}

	public static class GameGlobals
	{
		public static List<Unit> units = new List<Unit>();


		public static LevelGenerator levelGenerator;
	}
}
