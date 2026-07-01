using System;
using System.Collections.Generic;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using Strategic4XDiplomacy;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Diplomacy;
using TaleWorlds.Library;
using WarAndAiTweaks;

namespace WarAndAITweaks.War_Peace_AI_Overhaul.StrategicAIModules.StrategicAI;

[HarmonyPatch(typeof(KingdomDiplomacyVM), "OnDeclarePeace")]
[HarmonyPriority(999)]
public class KingdomPlayerPeacePatch
{
	public static bool Prefix(KingdomWarItemVM item, ref KingdomDiplomacyVM __instance, KingdomDecision ____currentItemsUnresolvedDecision, Action<KingdomDecision> ____forceDecision)
	{
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnablePlayerPeaceFix || !GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableWarPeaceAIOverhaul)
		{
			return true;
		}
		if (____currentItemsUnresolvedDecision != null)
		{
			____forceDecision(____currentItemsUnresolvedDecision);
			return false;
		}
		IFaction faction = ((KingdomDiplomacyItemVM)item).Faction2;
		Kingdom val = (Kingdom)(object)((faction is Kingdom) ? faction : null);
		Kingdom kingdom = Hero.MainHero.Clan.Kingdom;
		if (val == null || kingdom == null)
		{
			return true;
		}
		if (Strategic4XDiplomacyBehavior.GetWarFatigue(val) >= 1f)
		{
			return true;
		}
		string text = "";
		text = ((!((List<Kingdom>)(object)Strategic4XDiplomacyBehavior.snowballingKingdoms).Contains(kingdom)) ? StrategicX4AIHelpers.BuildNarrative(val, kingdom, "expansion_war") : StrategicX4AIHelpers.BuildNarrative(val, kingdom, "prevent_snowball_war"));
		InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("diplomacy_ai_refuses_peace", "{AI} refuses peace: {REASON}").SetTextVariable("AI", val.Name).SetTextVariable("REASON", text)).ToString(), Colors.Red));
		return false;
	}
}
