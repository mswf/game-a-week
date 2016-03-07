using UnityEngine;
using System.Collections;


namespace Week04
{
	public class BaseUnit : MonoBehaviour
	{
		[SerializeField]
		public Faction faction;
		
		internal Vector3 _currentPosition;
		internal Transform _transform;

		public void InitializeUnit(Faction unitFaction)
		{
			faction = unitFaction;
		}

		// Set up all internal references to this 
		private void Awake()
		{
			_transform = transform;
			_currentPosition = _transform.localPosition;
		}

		// Use this for initialization
		private void Start()
		{

			Globals.playfield.AddUnit(this);
		}

		// Update is called once per frame
		private void Update()
		{

		}

		public float GetInitialPosition()
		{
			if (_currentPosition.x != 0f)
			{
				_currentPosition.x = transform.position.x;
			}
			return _currentPosition.x;
		}
	}


	
}