using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Election;
using WarAndAiTweaks;

namespace WarAndAITweaks.War_Peace_AI_Overhaul.StrategicAIModules.StrategicAI;

[HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "GetRandomWarDecision")]
[HarmonyPriority(800)]
public class Patch_DisableRandomWar
{
	public static bool Prefix(ref KingdomDecision __result)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableWarPeaceAIOverhaul)
		{
			return true;
		}
		__result = null;
		return false;
	}
}
