using System;
using System.Collections.Generic;
using System.Linq;
using FeastSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace WarAndAITweaks.TodayWeFeast;

[HarmonyPatch(typeof(HeroAgentSpawnCampaignBehavior), "AddLordsHallCharacters")]
internal static class Feast_AddLordsHallCharacters_Patch
{
	public static bool Prefix(Settlement settlement, ref List<MobileParty> partiesToBeSpawn)
	{
		Settlement settlement2 = settlement;
		try
		{
			if (settlement2 == null || partiesToBeSpawn == null)
			{
				return true;
			}
			FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f != null && f.Settlement == settlement2);
			if (feastByAttribute == null)
			{
				return true;
			}
			List<Hero> list = new List<Hero>();
			if (Hero.MainHero != null)
			{
				list.Add(Hero.MainHero);
			}
			if (feastByAttribute.Host != null && feastByAttribute.Host != Hero.MainHero)
			{
				list.Add(feastByAttribute.Host);
			}
			if (feastByAttribute.Attendees != null)
			{
				foreach (Hero item in (List<Hero>)(object)feastByAttribute.Attendees)
				{
					if (item != null && item != Hero.MainHero && !list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			Hero mainHero = Hero.MainHero;
			Hero val = ((mainHero != null) ? mainHero.Spouse : null);
			if (val != null && val != Hero.MainHero && !list.Contains(val))
			{
				list.Add(val);
			}
			HashSet<MobileParty> allowedParties = new HashSet<MobileParty>();
			foreach (Hero item2 in list)
			{
				if (item2 != null)
				{
					MobileParty partyBelongedTo = item2.PartyBelongedTo;
					if (partyBelongedTo != null && partyBelongedTo != MobileParty.MainParty)
					{
						allowedParties.Add(partyBelongedTo);
					}
				}
			}
			List<MobileParty> list2 = partiesToBeSpawn.Where((MobileParty p) => p != null && allowedParties.Contains(p)).ToList();
			foreach (MobileParty item3 in allowedParties)
			{
				if (!list2.Contains(item3) && item3.IsActive)
				{
					list2.Add(item3);
				}
			}
			partiesToBeSpawn = list2;
			return false;
		}
		catch (Exception)
		{
			return true;
		}
	}
}
