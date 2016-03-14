using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Week04
{
	public class ResourceDisplay : MonoBehaviour
	{
		private ResourceType _resourceType;

		public Text resourceNameDisplay;
		public Text resourceAmountDisplay;

		private double _previousResourceAmount;
		private bool _isVisible;

		// Use this for early referencing
		private void Awake()
		{

		}

		// Use this for initialization
		private void Start () 
		{
		
		}
		
		// Update is called once per frame
		private void Update () 
		{
		
		}

		public void SetVisible(bool isVisible)
		{
			_isVisible = isVisible;
			gameObject.SetActive(isVisible);
		}

		public void SetToResource(ResourceType lootType)
		{
			_resourceType = lootType;

			resourceNameDisplay.text = lootType.ToString();

			UpdateDisplay();
		}


		public void UpdateDisplay()
		{
			var newResourceAmount = Globals.playerFaction.resources.Get(_resourceType);

			if (newResourceAmount != _previousResourceAmount)
			{
				resourceAmountDisplay.text = newResourceAmount.ToString();
			}

			_previousResourceAmount = newResourceAmount;

		}
	}
}
