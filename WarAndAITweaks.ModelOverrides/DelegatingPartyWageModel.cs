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
using WarAndAiTweaks;

namespace WarAndAITweaks.ModelOverrides;

internal sealed class DelegatingPartyWageModel : PartyWageModel
{
	private PartyWageModel _cachedInner;

	private PartyWageModel Inner
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
				return (PartyWageModel)new DefaultPartyWageModel();
			}
			PartyWageModel val2 = null;
			foreach (PartyWageModel item in ((IEnumerable)val).OfType<PartyWageModel>())
			{
				if (item == this)
				{
					break;
				}
				val2 = item;
			}
			_cachedInner = (PartyWageModel)(((object)val2) ?? ((object)new DefaultPartyWageModel()));
			return _cachedInner;
		}
	}

	public override int MaxWage => Inner.MaxWage;

	public override int GetCharacterWage(CharacterObject character)
	{
		return Inner.GetCharacterWage(character);
	}

	public override int GetTroopRecruitmentCost(CharacterObject troop, Hero buyerHero, bool withoutItemCost = false)
	{
		int num = Inner.GetTroopRecruitmentCost(troop, buyerHero, withoutItemCost);
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableBandTogether && GlobalSettings<WarAndAiTweaksSettings>.Instance.BandTogetherRecruitmentCost)
		{
			num = ApplyBandTogetherDiscount(num, buyerHero);
		}
		return num;
	}

	public override ExplainedNumber GetTotalWage(MobileParty mobileParty, bool desc)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		ExplainedNumber totalWage = Inner.GetTotalWage(mobileParty, desc);
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableGarrisonWageModifier && mobileParty != null && mobileParty.IsGarrison)
		{
			float num = GlobalSettings<WarAndAiTweaksSettings>.Instance.GarrisonWageReductionMultiplier;
			if (num < 0f)
			{
				num = 0f;
			}
			if (num > 10f)
			{
				num = 10f;
			}
			if (Math.Abs(num - 1f) > 1E-06f)
			{
				float resultNumber = ((ExplainedNumber)(ref totalWage)).ResultNumber;
				float num2 = resultNumber * num;
				float num3 = num2 - resultNumber;
				if (Math.Abs(num3) > 0.0001f)
				{
					((ExplainedNumber)(ref totalWage)).Add(num3, LanguageTranslater.L.T("[WarAI] Garrison wage multiplier", "[WarAI] Garrison wage multiplier"), (TextObject)null);
				}
			}
		}
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableBandTogether && GlobalSettings<WarAndAiTweaksSettings>.Instance.BandTogetherPartyWages && mobileParty != null && mobileParty.IsLordParty && mobileParty.LeaderHero != null)
		{
			float num4 = CalculateBandTogetherDiscount(mobileParty.LeaderHero);
			if (num4 > 0f && num4 < 1f)
			{
				float resultNumber2 = ((ExplainedNumber)(ref totalWage)).ResultNumber;
				float num5 = resultNumber2 * (1f - num4);
				if (Math.Abs(num5) > 0.0001f)
				{
					((ExplainedNumber)(ref totalWage)).Add(0f - num5, LanguageTranslater.L.T("[WarAI] Band Together discount", "[WarAI] Band Together discount"), (TextObject)null);
				}
			}
		}
		return totalWage;
	}

	private int ApplyBandTogetherDiscount(int baseCost, Hero buyerHero)
	{
		if (buyerHero == null || buyerHero.Clan == null || buyerHero.Clan.Kingdom == null)
		{
			return baseCost;
		}
		Kingdom kingdom = buyerHero.Clan.Kingdom;
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

	private float CalculateBandTogetherDiscount(Hero hero)
	{
		object obj;
		if (hero == null)
		{
			obj = null;
		}
		else
		{
			Clan clan = hero.Clan;
			obj = ((clan != null) ? clan.Kingdom : null);
		}
		if (obj == null)
		{
			return 1f;
		}
		Kingdom kingdom = hero.Clan.Kingdom;
		int num = ((IEnumerable<Settlement>)kingdom.Settlements).Where((Settlement x) => x.IsTown || x.IsCastle).Count();
		int bandTogetherMaxFiefs = GlobalSettings<WarAndAiTweaksSettings>.Instance.BandTogetherMaxFiefs;
		if (num <= 0 || num > bandTogetherMaxFiefs)
		{
			return 1f;
		}
		if (kingdom.RulingClan == Clan.PlayerClan && !GlobalSettings<WarAndAiTweaksSettings>.Instance.BandTogetherIncludePlayerKingdom)
		{
			return 1f;
		}
		return (float)num / (float)bandTogetherMaxFiefs;
	}
}
