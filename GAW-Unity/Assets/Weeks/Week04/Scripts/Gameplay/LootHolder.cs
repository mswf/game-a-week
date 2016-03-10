using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Week04
{
	public enum ResourceType
	{
		Gold,
		Food
	}


	[System.Serializable]
	public class ResourceContainer
	{
		[System.Serializable]
		protected struct Resource
		{
			public ResourceType type;
			public double amount;

			public Resource(ResourceType type, double amount)
			{
				this.type = type;
				this.amount = amount;
			}
		}

		[SerializeField]
		protected List<Resource> resourceList;
		
		public ResourceContainer()
		{
			resourceList = new List<Resource>();
		}
		
		public double Get(ResourceType type)
		{
			for (var i = 0; i < resourceList.Count; i++)
			{
				if (resourceList[i].type == type)
					return resourceList[i].amount;
			}

			return 0d;
		}

		public double this[ResourceType type]
		{
			get
			{
				return Get(type);
			}
			set
			{
				Add(type, value);
			}
		}

		public bool ContainsLoot()
		{
			for (var i = 0; i < resourceList.Count; i++)
			{
				if (resourceList[i].amount > 0d)
				{
					return true;
				}
			}

			return false;
		}

		private double _Change(ResourceType type, double amount)
		{
			for (var i = 0; i < resourceList.Count; i++)
			{
				var lootElement = resourceList[i];
				if (lootElement.type == type)
				{
					double removedAmount;

					if ( (lootElement.amount + amount) < 0d)
					{
						removedAmount = lootElement.amount * -1d;

						lootElement.amount = 0d;
					}
					else
					{
						removedAmount = amount;

						lootElement.amount += amount;
					}

					resourceList[i] = lootElement;
					
					return removedAmount;
				}
			}

			if (amount > 0d)
			{
				resourceList.Add(new Resource(type, amount));
				return amount;
			}

			return 0d;
		}

		public double Add(ResourceType type, double amount)
		{
			return _Change(type, amount);
		}

		public double Subtract(ResourceType type, double amount)
		{
			return _Change(type, amount*-1d) *-1d;
		}

		public static ResourceContainer operator +(ResourceContainer a, ResourceContainer b)
		{
			var newContainer = new ResourceContainer();

			for (var i = 0; i < a.resourceList.Count; i++)
			{
				newContainer.Add(a.resourceList[i].type, a.resourceList[i].amount);
			}

			for (var i = 0; i < b.resourceList.Count; i++)
			{
				newContainer.Add(b.resourceList[i].type, b.resourceList[i].amount);
			}

			return newContainer;
		}

		public static ResourceContainer operator -(ResourceContainer a, ResourceContainer b)
		{
			var newContainer = new ResourceContainer();

			for (var i = 0; i < a.resourceList.Count; i++)
			{
				newContainer.Add(a.resourceList[i].type, a.resourceList[i].amount);
			}

			for (var i = 0; i < b.resourceList.Count; i++)
			{
				newContainer.Subtract(b.resourceList[i].type, b.resourceList[i].amount);
			}

			return newContainer;
		}

		public static ResourceContainer operator -(ResourceContainer a)
		{
			var newContainer = new ResourceContainer();

			for (var i = 0; i < a.resourceList.Count; i++)
			{
				newContainer.Subtract(a.resourceList[i].type, a.resourceList[i].amount);
			}

			return newContainer;
		}
	}

	[RequireComponent(typeof(BaseUnit))]
	public class LootHolder : MonoBehaviour
	{
		[SerializeField]
		private ResourceContainer _initialResourceContainer;

		private void Awake()
		{
			GetComponent<BaseUnit>().resourceContainer += _initialResourceContainer;


			//GetComponent<BaseUnit>().attackSpeed = 5000f;
		}
	}
}
