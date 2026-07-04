using System.Linq;
using FeastSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;

namespace WarAndAITweaks.TodayWeFeast;

[HarmonyPatch(typeof(HeroAgentSpawnCampaignBehavior), "RefreshLocationOfHeroForSettlement")]
internal static class Feast_AddLordsHallCharacters_Patch
{
	public static bool Prefix(Hero hero, Settlement settlement)
	{
		if (settlement == null || hero == null)
		{
			return true;
		}
		FeastData feast = FeastBehavior.GetFeastByAttribute((FeastData f) => f != null && f.Settlement == settlement);
		if (feast == null)
		{
			return true;
		}
		if (hero == Hero.MainHero)
		{
			return true;
		}
		if (feast.Host != null && hero == feast.Host)
		{
			return true;
		}
		if (feast.Attendees != null && feast.Attendees.Contains(hero))
		{
			return true;
		}
		Hero mainHero = Hero.MainHero;
		if (mainHero?.Spouse != null && hero == mainHero.Spouse)
		{
			return true;
		}
		if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero == hero)
		{
			return false;
		}
		return true;
	}
}
