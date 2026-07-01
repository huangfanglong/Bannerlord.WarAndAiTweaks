using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using WarAndAiTweaks;

namespace WarAndAITweaks.FIllStacks;

[HarmonyPatch(typeof(MobileParty), "FillPartyStacks")]
[HarmonyPriority(800)]
public class SpawnLordPartyInternalPatch
{
	public static void Prefix(PartyTemplateObject pt, ref int troopNumberLimit, ref MobileParty __instance)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableFillStacks || !__instance.IsLordParty)
		{
			return;
		}
		Hero leaderHero = __instance.LeaderHero;
		if (leaderHero != null)
		{
			Clan clan = leaderHero.Clan;
			if (((clan != null) ? new bool?(clan.IsUnderMercenaryService) : null).GetValueOrDefault())
			{
				return;
			}
		}
		troopNumberLimit = GlobalSettings<WarAndAiTweaksSettings>.Instance.FillStackTroopCount;
	}
}
