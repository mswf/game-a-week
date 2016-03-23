using UnityEngine;
using System.Collections.Generic;
using BehaviorTree;

namespace Week06
{
	public class GameController : BehaviorStateMonobehavior
	{
		// Use this for early referencing
		private void Awake()
		{
			GameObject.DontDestroyOnLoad(this);
			behaviorState = new BehaviorState();


			var parser = new DefinitionParser();

			parser.ParseTestFile();
		}

		public List<Squad> squadsInScene;

		public List<Unit> unitsInScene;

		[SerializeField]
		public BehaviorState behaviorState;

		public override BehaviorState GetBehaviorState()
		{
			return behaviorState;
		}

		// Use this for initialization
		private void Start ()
		{
			var squads = GameObject.FindObjectsOfType<Squad>();
			squadsInScene = new List<Squad>(squads.Length);

			for (int i = 0; i < squads.Length; i++)
			{
				squadsInScene.Add(squads[i]);

				squads[i].Initialize(this);
			}

			var units = GameObject.FindObjectsOfType<Unit>();
			
			unitsInScene = new List<Unit>(units.Length);
			
			for (int i = 0; i < units.Length; i++)
			{
				unitsInScene.Add(units[i]);

				units[i].Initialize(this);
			}
		}
		
		// Update is called once per frame
		private void Update () 
		{
		
		}


	}
}
