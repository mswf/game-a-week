using UnityEngine;
using System.Collections;

namespace Week04
{
	public class CombatResolver 
	{

		public static void ResolveCombat(CombatData combatData)
		{
			if (combatData.isResolved == true)
			{
				Log.Steb("Trying to resolve already resolved attack");
			}


			combatData.isResolved = true;
		}

	}

	public class CombatData
	{
		public BaseUnit attacker;
		public BaseUnit[] targets;

		public bool isResolved = false;

		public CombatData(BaseUnit attacker, BaseUnit target)
		{
			this.attacker = attacker;
			this.targets = new BaseUnit[] { target };
		}

		public CombatData(BaseUnit attacker, BaseUnit[] targets)
		{
			this.attacker = attacker;
			this.targets = targets;
		}
	}
}
