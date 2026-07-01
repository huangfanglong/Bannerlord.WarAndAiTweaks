using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using WarAndAiTweaks;

namespace WarAndAITweaks.War_Peace_AI_Overhaul.StrategicAIModules.StrategicAI;

[HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "ConsiderWar")]
[HarmonyPriority(800)]
public class Patch_DisableConsiderWar
{
	public static bool Prefix(Clan clan, Kingdom kingdom, IFaction otherFaction)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableWarPeaceAIOverhaul)
		{
			return true;
		}
		return false;
	}
}
