using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Week04
{
	public enum LootType
	{
		Gold,
		Food
	}


	[System.Serializable]
	public class LootContainer
	{
		[System.Serializable]
		protected struct LootElement
		{
			public LootType type;
			public double amount;

			public LootElement(LootType type, double amount)
			{
				this.type = type;
				this.amount = amount;
			}
		}

		[SerializeField]
		protected List<LootElement> lootDictionary;
		
		public LootContainer()
		{
			lootDictionary = new List<LootElement>();
		}
		
		public double Get(LootType type)
		{
			for (var i = 0; i < lootDictionary.Count; i++)
			{
				if (lootDictionary[i].type == type)
					return lootDictionary[i].amount;
			}

			return 0d;
		}

		public double this[LootType type]
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
			for (var i = 0; i < lootDictionary.Count; i++)
			{
				if (lootDictionary[i].amount > 0d)
				{
					return true;
				}
			}

			return false;
		}

		private double _Change(LootType type, double amount)
		{
			for (var i = 0; i < lootDictionary.Count; i++)
			{
				var lootElement = lootDictionary[i];
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

					lootDictionary[i] = lootElement;
					
					return removedAmount;
				}
			}

			if (amount > 0d)
			{
				lootDictionary.Add(new LootElement(type, amount));
				return amount;
			}

			return 0d;
		}

		public double Add(LootType type, double amount)
		{
			return _Change(type, amount);
		}

		public double Subtract(LootType type, double amount)
		{
			return _Change(type, amount*-1d) *-1d;
		}

		public static LootContainer operator +(LootContainer a, LootContainer b)
		{
			var newContainer = new LootContainer();

			for (var i = 0; i < a.lootDictionary.Count; i++)
			{
				newContainer.Add(a.lootDictionary[i].type, a.lootDictionary[i].amount);
			}

			for (var i = 0; i < b.lootDictionary.Count; i++)
			{
				newContainer.Add(b.lootDictionary[i].type, b.lootDictionary[i].amount);
			}

			return newContainer;
		}

		public static LootContainer operator -(LootContainer a, LootContainer b)
		{
			var newContainer = new LootContainer();

			for (var i = 0; i < a.lootDictionary.Count; i++)
			{
				newContainer.Add(a.lootDictionary[i].type, a.lootDictionary[i].amount);
			}

			for (var i = 0; i < b.lootDictionary.Count; i++)
			{
				newContainer.Subtract(b.lootDictionary[i].type, b.lootDictionary[i].amount);
			}

			return newContainer;
		}

		public static LootContainer operator -(LootContainer a)
		{
			var newContainer = new LootContainer();

			for (var i = 0; i < a.lootDictionary.Count; i++)
			{
				newContainer.Subtract(a.lootDictionary[i].type, a.lootDictionary[i].amount);
			}

			return newContainer;
		}
	}

	[RequireComponent(typeof(BaseUnit))]
	public class LootHolder : MonoBehaviour
	{
		[SerializeField]
		private LootContainer _initialLootContainer;

		private void Awake()
		{
			GetComponent<BaseUnit>().lootContainer += _initialLootContainer;


			//GetComponent<BaseUnit>().attackSpeed = 5000f;
		}
	}
}
