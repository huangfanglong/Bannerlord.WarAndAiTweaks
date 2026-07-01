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

internal sealed class DelegatingSettlementFoodModel : SettlementFoodModel
{
	private SettlementFoodModel _cachedInner;

	private SettlementFoodModel Inner
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
				return (SettlementFoodModel)new DefaultSettlementFoodModel();
			}
			SettlementFoodModel val2 = null;
			foreach (SettlementFoodModel item in ((IEnumerable)val).OfType<SettlementFoodModel>())
			{
				if (item == this)
				{
					break;
				}
				val2 = item;
			}
			_cachedInner = (SettlementFoodModel)(((object)val2) ?? ((object)new DefaultSettlementFoodModel()));
			return _cachedInner;
		}
	}

	public override int NumberOfMenOnGarrisonToEatOneFood
	{
		get
		{
			int numberOfMenOnGarrisonToEatOneFood = Inner.NumberOfMenOnGarrisonToEatOneFood;
			if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableGarrisonFoodReduction)
			{
				return numberOfMenOnGarrisonToEatOneFood;
			}
			return numberOfMenOnGarrisonToEatOneFood * GlobalSettings<WarAndAiTweaksSettings>.Instance.GarrisonFoodMuliplier;
		}
	}

	public override int NumberOfProsperityToEatOneFood => Inner.NumberOfProsperityToEatOneFood;

	public override int FoodStocksUpperLimit => Inner.FoodStocksUpperLimit;

	public override int CastleFoodStockUpperLimitBonus => Inner.CastleFoodStockUpperLimitBonus;

	public override ExplainedNumber CalculateTownFoodStocksChange(Town town, bool includeDescriptions = false, bool includeDescriptionsForMarket = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return Inner.CalculateTownFoodStocksChange(town, includeDescriptions, includeDescriptionsForMarket);
	}
}
