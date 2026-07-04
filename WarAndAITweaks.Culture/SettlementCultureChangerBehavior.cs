using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace WarAndAITweaks.Culture;

public class SettlementCultureChangerBehavior : CampaignBehaviorBase
{
	public override void RegisterEvents()
	{
		CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener((object)this, (Action<Settlement>)SettlementTickEvent);
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	private void SettlementTickEvent(Settlement settlement)
	{
		if (settlement == null)
		{
			return;
		}
		object obj;
		if (settlement == null)
		{
			obj = null;
		}
		else
		{
			Clan ownerClan = settlement.OwnerClan;
			if (ownerClan == null)
			{
				obj = null;
			}
			else
			{
				Kingdom kingdom = ownerClan.Kingdom;
				obj = ((kingdom != null) ? kingdom.Culture : null);
			}
		}
		CultureObject val = (CultureObject)obj;
		if (val == null)
		{
			return;
		}
		if (settlement.IsTown || settlement.IsCastle)
		{
			if (settlement.Culture == val)
			{
				return;
			}
			settlement.Culture = val;
			{
				foreach (Hero item in (List<Hero>)(object)settlement.Notables)
				{
					if (item.Culture != val)
					{
						item.Culture = val;
					}
				}
				return;
			}
		}
		if (settlement.IsVillage && settlement.Culture != val)
		{
			settlement.Culture = val;
		}
	}
}
