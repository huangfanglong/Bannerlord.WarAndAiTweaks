using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using Strategic4XDiplomacy;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using WarAndAiTweaks;

namespace WarAndAITweaks.War_Peace_AI_Overhaul.StrategicAIModules.StrategicAI;

[HarmonyPatch(typeof(DeclareWarDecision), "DetermineSupport")]
internal static class DeclareWarDecision_DetermineSupport_Patch
{
	private static bool Prefix(DeclareWarDecision __instance, Clan clan, DecisionOutcome possibleOutcome, ref float __result)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableWarPeaceAIOverhaul)
		{
			return true;
		}
		if (clan != null && clan != Clan.PlayerClan)
		{
			DeclareWarDecision.DeclareWarDecisionOutcome val = (DeclareWarDecision.DeclareWarDecisionOutcome)(object)((possibleOutcome is DeclareWarDecision.DeclareWarDecisionOutcome) ? possibleOutcome : null);
			if (val != null)
			{
				Kingdom kingdom = val.Kingdom ?? clan.Kingdom;
				IFaction factionToDeclareWarOn = val.FactionToDeclareWarOn;
				Kingdom val2 = (Kingdom)(object)((factionToDeclareWarOn is Kingdom) ? factionToDeclareWarOn : null);
				if (kingdom.Leader == Hero.MainHero && !GlobalSettings<WarAndAiTweaksSettings>.Instance.EnablePlayerKingdomClanVoting)
				{
					return false;
				}
				if (kingdom != null && val2 != null)
				{
					Kingdom item = Strategic4XDiplomacyBehavior.FindBestTargetForWar(kingdom).target;
					float num = ((item == val2 && !((IEnumerable<Kingdom>)Strategic4XDiplomacyBehavior.MajorKingdoms).Any((Kingdom x) => x.IsAtWarWith((IFaction)(object)kingdom)) && Strategic4XDiplomacyBehavior.GetWarFatigue(kingdom) <= 0f) ? float.MaxValue : float.MinValue);
					__result = (val.ShouldWarBeDeclared ? num : (0f - num));
					return false;
				}
			}
		}
		return true;
	}
}
