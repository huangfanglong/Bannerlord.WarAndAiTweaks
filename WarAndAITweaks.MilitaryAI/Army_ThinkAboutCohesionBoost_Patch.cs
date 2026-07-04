using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using WarAndAiTweaks;

namespace WarAndAITweaks.MilitaryAI;

[HarmonyPatch(typeof(Army), "ThinkAboutCohesionBoost")]
public class Army_ThinkAboutCohesionBoost_Patch
{
	public static bool Prefix(Army __instance)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableEnhancedMilitary)
		{
			return true;
		}
		MobileParty leaderParty = __instance.LeaderParty;
		object obj;
		if (leaderParty == null)
		{
			obj = null;
		}
		else
		{
			Hero leaderHero = leaderParty.LeaderHero;
			obj = ((leaderHero != null) ? leaderHero.Clan : null);
		}
		Clan val = (Clan)obj;
		MobileParty leaderParty2 = __instance.LeaderParty;
		object obj2;
		if (leaderParty2 == null)
		{
			obj2 = null;
		}
		else
		{
			Hero leaderHero2 = leaderParty2.LeaderHero;
			if (leaderHero2 == null)
			{
				obj2 = null;
			}
			else
			{
				Clan clan = leaderHero2.Clan;
				obj2 = ((clan != null) ? clan.Kingdom : null);
			}
		}
		Kingdom val2 = (Kingdom)obj2;
		if (val2 == null || val == null)
		{
			return true;
		}
		if (val.Influence > 1000f)
		{
			float num = MathF.Min(100f, 100f - __instance.Cohesion);
			ArmyManagementCalculationModel armyManagementCalculationModel = Campaign.Current.Models.ArmyManagementCalculationModel;
			int num2 = armyManagementCalculationModel.CalculateTotalInfluenceCost(__instance, num);
			if (val.Influence > (float)num2)
			{
				__instance.BoostCohesionWithInfluence(num, num2);
			}
			return false;
		}
		return true;
	}
}
