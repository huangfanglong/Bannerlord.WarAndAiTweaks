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
using WarAndAiTweaks;

namespace WarAndAITweaks.ModelOverrides;

internal sealed class DelegatingSettlementTaxModel : SettlementTaxModel
{
	private SettlementTaxModel _cachedInner;

	private SettlementTaxModel Inner
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
				return (SettlementTaxModel)new DefaultSettlementTaxModel();
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
			_cachedInner = (SettlementTaxModel)(((object)val2) ?? ((object)new DefaultSettlementTaxModel()));
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

	public override float GetVillageTaxRatio()
	{
		return Inner.GetVillageTaxRatio();
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
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		ExplainedNumber result = Inner.CalculateTownTax(town, includeDescriptions);
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableSettlementTax)
		{
			return result;
		}
		float settlementTaxMultiplier = GlobalSettings<WarAndAiTweaksSettings>.Instance.SettlementTaxMultiplier;
		if (Math.Abs(settlementTaxMultiplier - 1f) > 1E-06f)
		{
			((ExplainedNumber)(ref result)).AddFactor(settlementTaxMultiplier - 1f, LanguageTranslater.L.T("[WarAI] Tax multiplier", "[WarAI] Tax multiplier"));
		}
		return result;
	}
}
