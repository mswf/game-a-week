using UnityEngine;
using System.Collections;


namespace Week04
{
	public class BaseUnit : MonoBehaviour
	{
		[SerializeField]
		private Faction faction;

		public void InitializeUnit(Faction unitFaction)
		{
			faction = unitFaction;
		}

		// Set up all internal references to this 
		private void Awake()
		{
			
		}

		// Use this for initialization
		private void Start()
		{

		}

		// Update is called once per frame
		private void Update()
		{

		}
	}


	
}