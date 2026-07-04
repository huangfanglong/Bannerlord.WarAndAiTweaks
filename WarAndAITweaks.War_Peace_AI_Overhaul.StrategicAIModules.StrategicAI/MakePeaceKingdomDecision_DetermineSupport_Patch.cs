using HarmonyLib;
using MCM.Abstractions.Base.Global;
using Strategic4XDiplomacy;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using WarAndAiTweaks;

namespace WarAndAITweaks.War_Peace_AI_Overhaul.StrategicAIModules.StrategicAI;

[HarmonyPatch(typeof(MakePeaceKingdomDecision), "DetermineSupport")]
internal static class MakePeaceKingdomDecision_DetermineSupport_Patch
{
	private static bool Prefix(MakePeaceKingdomDecision __instance, Clan clan, DecisionOutcome possibleOutcome, ref float __result)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableWarPeaceAIOverhaul)
		{
			return true;
		}
		if (clan != null && clan != Clan.PlayerClan)
		{
			MakePeaceKingdomDecision.MakePeaceDecisionOutcome val = (MakePeaceKingdomDecision.MakePeaceDecisionOutcome)(object)((possibleOutcome is MakePeaceKingdomDecision.MakePeaceDecisionOutcome) ? possibleOutcome : null);
			if (val != null)
			{
				Kingdom val2 = val.Kingdom ?? clan.Kingdom;
				IFaction factionToMakePeaceWith = val.FactionToMakePeaceWith;
				Kingdom val3 = (Kingdom)(object)((factionToMakePeaceWith is Kingdom) ? factionToMakePeaceWith : null);
				if (val2.Leader == Hero.MainHero && !GlobalSettings<WarAndAiTweaksSettings>.Instance.EnablePlayerKingdomClanVoting)
				{
					return false;
				}
				if (val2 != null && val3 != null)
				{
					float warFatigue = Strategic4XDiplomacyBehavior.GetWarFatigue(val2);
					float warFatigue2 = Strategic4XDiplomacyBehavior.GetWarFatigue(val3);
					float num = ((warFatigue == 1f && warFatigue2 == 1f) ? float.MaxValue : float.MinValue);
					__result = (val.ShouldPeaceBeDeclared ? num : (0f - num));
					return false;
				}
			}
		}
		return true;
	}
}
