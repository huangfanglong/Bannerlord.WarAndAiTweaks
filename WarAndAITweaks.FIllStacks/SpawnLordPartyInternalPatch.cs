using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using WarAndAiTweaks;

namespace WarAndAITweaks.FIllStacks;

[HarmonyPatch(typeof(DefaultPartySizeLimitModel), "FindAppropriateInitialRosterForMobileParty")]
[HarmonyPriority(800)]
public class SpawnLordPartyInternalPatch
{
	public static bool Prefix(MobileParty party, PartyTemplateObject partyTemplate, ref TroopRoster __result)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableFillStacks || !party.IsLordParty)
		{
			return true;
		}
		Hero leaderHero = party.LeaderHero;
		if (leaderHero != null)
		{
			Clan clan = leaderHero.Clan;
			if (clan != null && clan.IsUnderMercenaryService)
			{
				return true;
			}
		}
		TroopRoster roster = TroopRoster.CreateDummyTroopRoster();
		int fillCount = GlobalSettings<WarAndAiTweaksSettings>.Instance.FillStackTroopCount;
		for (int i = 0; i < partyTemplate.Stacks.Count; i++)
		{
			roster.AddToCounts(partyTemplate.Stacks[i].Character, fillCount);
		}
		__result = roster;
		return false;
	}
}
