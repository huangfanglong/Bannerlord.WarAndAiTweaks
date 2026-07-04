using System;
using System.Collections;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using WarAndAiTweaks;

namespace WarAndAITweaks.ModelOverrides;

internal sealed class DelegatingSettlementTaxModel : SettlementTaxModel
{
	private SettlementTaxModel _cachedInner;

	private SettlementTaxModel Inner
	{
		get
		{
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
				return new DefaultSettlementTaxModel();
			}
			SettlementTaxModel val2 = null;
			foreach (SettlementTaxModel item in ((IEnumerable)val).OfType<SettlementTaxModel>())
			{
				if (item == this)
				{
					break;
				}
				val2 = item;
			}
			_cachedInner = val2 ?? new DefaultSettlementTaxModel();
			return _cachedInner;
		}
	}

	public override float SettlementCommissionRateTown => Inner.SettlementCommissionRateTown;

	public override float SettlementCommissionRateVillage => Inner.SettlementCommissionRateVillage;

	public override int SettlementCommissionDecreaseSecurityThreshold => Inner.SettlementCommissionDecreaseSecurityThreshold;

	public override int MaximumDecreaseBasedOnSecuritySecurity => Inner.MaximumDecreaseBasedOnSecuritySecurity;

	public override float GetTownTaxRatio(Town town)
	{
		return Inner.GetTownTaxRatio(town);
	}

	public override float GetVillageTaxRatio(Village village)
	{
		return Inner.GetVillageTaxRatio(village);
	}

	public override int CalculateVillageTaxFromIncome(Village village, int marketIncome)
	{
		return Inner.CalculateVillageTaxFromIncome(village, marketIncome);
	}

	public override float GetTownCommissionChangeBasedOnSecurity(Town town, float commission)
	{
		return Inner.GetTownCommissionChangeBasedOnSecurity(town, commission);
	}

	public override ExplainedNumber CalculateTownTax(Town town, bool includeDescriptions = false)
	{
		ExplainedNumber result = Inner.CalculateTownTax(town, includeDescriptions);
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableSettlementTax)
		{
			return result;
		}
		float settlementTaxMultiplier = GlobalSettings<WarAndAiTweaksSettings>.Instance.SettlementTaxMultiplier;
		if (Math.Abs(settlementTaxMultiplier - 1f) > 1E-06f)
		{
			result.AddFactor(settlementTaxMultiplier - 1f, LanguageTranslater.L.T("[WarAI] Tax multiplier", "[WarAI] Tax multiplier"));
		}
		return result;
	}
}
