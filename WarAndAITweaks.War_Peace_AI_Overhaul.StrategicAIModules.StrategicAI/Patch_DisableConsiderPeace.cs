using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Election;
using WarAndAiTweaks;

namespace WarAndAITweaks.War_Peace_AI_Overhaul.StrategicAIModules.StrategicAI;

[HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "ConsiderPeace")]
[HarmonyPriority(800)]
public class Patch_DisableConsiderPeace
{
	public static bool Prefix(Clan clan, Clan otherClan, Kingdom kingdom, IFaction otherFaction, out MakePeaceKingdomDecision decision)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableWarPeaceAIOverhaul)
		{
			decision = null;
			return true;
		}
		decision = null;
		return false;
	}
}
