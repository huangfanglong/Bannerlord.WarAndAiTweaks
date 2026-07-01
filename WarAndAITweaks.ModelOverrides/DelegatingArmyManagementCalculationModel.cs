using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using WarAndAITweaks.MarshalSystem;
using WarAndAITweaks.MilitaryAI;
using WarAndAiTweaks;

namespace WarAndAITweaks.ModelOverrides;

internal sealed class DelegatingArmyManagementCalculationModel : ArmyManagementCalculationModel
{
	private ArmyManagementCalculationModel _cachedInner;

	private ArmyManagementCalculationModel Inner
	{
		get
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			if (_cachedInner != null)
			{
				return _cachedInner;
			}
			Campaign current = Campaign.Current;
			object obj;
			if (current == null)
			{
				obj = null;
			}
			else
			{
				GameModels models = current.Models;
				obj = ((models != null) ? ((GameModelsManager)models).GetGameModels() : null);
			}
			MBReadOnlyList<GameModel> val = (MBReadOnlyList<GameModel>)obj;
			if (val == null)
			{
				return (ArmyManagementCalculationModel)new DefaultArmyManagementCalculationModel();
			}
			ArmyManagementCalculationModel val2 = null;
			foreach (ArmyManagementCalculationModel item in ((IEnumerable)val).OfType<ArmyManagementCalculationModel>())
			{
				if (item == this)
				{
					break;
				}
				val2 = item;
			}
			_cachedInner = (ArmyManagementCalculationModel)(((object)val2) ?? ((object)new DefaultArmyManagementCalculationModel()));
			return _cachedInner;
		}
	}

	public override int InfluenceValuePerGold => Inner.InfluenceValuePerGold;

	public override int AverageCallToArmyCost => Inner.AverageCallToArmyCost;

	public override int CohesionThresholdForDispersion => Inner.CohesionThresholdForDispersion;

	public override int CalculateTotalInfluenceCost(Army army, float percentage)
	{
		return Inner.CalculateTotalInfluenceCost(army, percentage);
	}

	public override int GetCohesionBoostInfluenceCost(Army army, int percentageToBoost = 100)
	{
		return Inner.GetCohesionBoostInfluenceCost(army, percentageToBoost);
	}

	public override int GetCohesionBoostGoldCost(Army army, float percentageToBoost = 100f)
	{
		return Inner.GetCohesionBoostGoldCost(army, percentageToBoost);
	}

	public override float GetPartySizeScore(MobileParty party)
	{
		return Inner.GetPartySizeScore(party);
	}

	public override int GetPartyRelation(Hero hero)
	{
		return Inner.GetPartyRelation(hero);
	}

	public override int GetPartyStrength(PartyBase party)
	{
		return Inner.GetPartyStrength(party);
	}

	public override bool CheckPartyEligibility(MobileParty party)
	{
		return Inner.CheckPartyEligibility(party);
	}

	public override int CalculateNewCohesion(Army army, PartyBase newParty, int calculatedCohesion, int sign)
	{
		return Inner.CalculateNewCohesion(army, newParty, calculatedCohesion, sign);
	}

	public override float DailyBeingAtArmyInfluenceAward(MobileParty party)
	{
		return Inner.DailyBeingAtArmyInfluenceAward(party);
	}

	public override int CalculatePartyInfluenceCost(MobileParty leaderParty, MobileParty targetParty)
	{
		int num = Inner.CalculatePartyInfluenceCost(leaderParty, targetParty);
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableMarshalSystem)
		{
			object obj;
			if (leaderParty == null)
			{
				obj = null;
			}
			else
			{
				Clan actualClan = leaderParty.ActualClan;
				obj = ((actualClan != null) ? actualClan.Kingdom : null);
			}
			Kingdom val = (Kingdom)obj;
			if (val != null)
			{
				Hero marshal = MarshalSystemBehavior.GetMarshal(val);
				if (marshal != null && marshal == leaderParty.LeaderHero)
				{
					float marshalBonus = MarshalHelper.GetMarshalBonus(marshal);
					float val2 = (float)num * (1f - marshalBonus);
					num = (int)Math.Floor(Math.Max(val2, 0f));
				}
			}
			float armyCallingInfluenceMultiplier = GlobalSettings<WarAndAiTweaksSettings>.Instance.ArmyCallingInfluenceMultiplier;
			if (Math.Abs(armyCallingInfluenceMultiplier - 1f) > 1E-06f)
			{
				num = (int)Math.Round((float)num * armyCallingInfluenceMultiplier);
			}
		}
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableBandTogether && GlobalSettings<WarAndAiTweaksSettings>.Instance.BandTogetherArmyInfluence)
		{
			num = ApplyBandTogetherInfluenceDiscount(num, leaderParty);
		}
		return num;
	}

	public override ExplainedNumber CalculateDailyCohesionChange(Army army, bool includeDescriptions = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		ExplainedNumber result = Inner.CalculateDailyCohesionChange(army, includeDescriptions);
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableMarshalSystem)
		{
			object kingdom;
			if (army == null)
			{
				kingdom = null;
			}
			else
			{
				MobileParty leaderParty = army.LeaderParty;
				if (leaderParty == null)
				{
					kingdom = null;
				}
				else
				{
					Clan actualClan = leaderParty.ActualClan;
					kingdom = ((actualClan != null) ? actualClan.Kingdom : null);
				}
			}
			Hero marshal = MarshalSystemBehavior.GetMarshal((Kingdom)kingdom);
			if (marshal != null)
			{
				object obj;
				if (army == null)
				{
					obj = null;
				}
				else
				{
					MobileParty leaderParty2 = army.LeaderParty;
					obj = ((leaderParty2 != null) ? leaderParty2.LeaderHero : null);
				}
				if (marshal == obj)
				{
					float marshalBonus = MarshalHelper.GetMarshalBonus(marshal);
					float resultNumber = ((ExplainedNumber)(ref result)).ResultNumber;
					if (resultNumber < 0f)
					{
						float num = 0.5f + marshalBonus;
						float num2 = resultNumber * num;
						float num3 = num2 - resultNumber;
						((ExplainedNumber)(ref result)).Add(num3, LanguageTranslater.L.T("Marshal_penalty_reduction_leadership_bonus", "Marshal penalty reduction + leadership bonus"), (TextObject)null);
					}
					else
					{
						float num4 = 1f + marshalBonus;
						float num5 = resultNumber * num4;
						float num6 = num5 - resultNumber;
						((ExplainedNumber)(ref result)).Add(num6, LanguageTranslater.L.T("Marshal_leadership_bonus", "Marshal leadership bonus"), (TextObject)null);
					}
				}
			}
		}
		return result;
	}

	public override List<MobileParty> GetMobilePartiesToCallToArmy(MobileParty leaderParty)
	{
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableEnhancedMilitary && !GlobalSettings<WarAndAiTweaksSettings>.Instance.PreventClanPartiesFromBeingCalled)
		{
			return Inner.GetMobilePartiesToCallToArmy(leaderParty);
		}
		List<MobileParty> callMobilePartiesForArmy = MilitaryAIHelpers.GetCallMobilePartiesForArmy(leaderParty);
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.PreventClanPartiesFromBeingCalled)
		{
			foreach (MobileParty item in (List<MobileParty>)(object)Extensions.ToMBList<MobileParty>(callMobilePartiesForArmy))
			{
				if (item.LeaderHero != null && item.LeaderHero != Hero.MainHero && item.LeaderHero.Clan == Hero.MainHero.Clan)
				{
					callMobilePartiesForArmy.Remove(item);
				}
			}
		}
		return callMobilePartiesForArmy;
	}

	private int ApplyBandTogetherInfluenceDiscount(int baseCost, MobileParty leaderParty)
	{
		object obj;
		if (leaderParty == null)
		{
			obj = null;
		}
		else
		{
			Hero leaderHero = leaderParty.LeaderHero;
			if (leaderHero == null)
			{
				obj = null;
			}
			else
			{
				Clan clan = leaderHero.Clan;
				obj = ((clan != null) ? clan.Kingdom : null);
			}
		}
		if (obj == null)
		{
			return baseCost;
		}
		Kingdom kingdom = leaderParty.LeaderHero.Clan.Kingdom;
		int count = ((List<Town>)(object)kingdom.Fiefs).Count;
		int bandTogetherMaxFiefs = GlobalSettings<WarAndAiTweaksSettings>.Instance.BandTogetherMaxFiefs;
		if (count <= 0 || count > bandTogetherMaxFiefs)
		{
			return baseCost;
		}
		if (kingdom.RulingClan == Clan.PlayerClan && !GlobalSettings<WarAndAiTweaksSettings>.Instance.BandTogetherIncludePlayerKingdom)
		{
			return baseCost;
		}
		int num = count * 100 / bandTogetherMaxFiefs;
		return baseCost * num / 100;
	}
}
