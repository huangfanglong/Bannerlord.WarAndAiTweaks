using System.Collections;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using WarAndAiTweaks;

namespace WarAndAITweaks.ModelOverrides;

internal class DelegatingBuildingEffectModel : BuildingEffectModel
{
	private BuildingEffectModel _cachedInner;

	private BuildingEffectModel Inner
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
				return new DefaultBuildingEffectModel();
			}
			BuildingEffectModel val2 = null;
			foreach (BuildingEffectModel item in ((IEnumerable)val).OfType<BuildingEffectModel>())
			{
				if (item == this)
				{
					break;
				}
				val2 = item;
			}
			_cachedInner = val2 ?? new DefaultBuildingEffectModel();
			return _cachedInner;
		}
	}

	public override ExplainedNumber GetBuildingEffect(Building building, BuildingEffectEnum effect)
	{
		ExplainedNumber result = Inner.GetBuildingEffect(building, effect);
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableMilitiaBoost || building == null)
		{
			return result;
		}
		if (((object)building.Name).ToString() == "Militia Grounds" && effect == BuildingEffectEnum.Militia)
		{
			TextObject bonusText = LanguageTranslater.L.T("[WarAI] Militia boost", "[WarAI] Militia boost");
			if (((SettlementComponent)building.Town).IsCastle)
			{
				result.Add((float)GlobalSettings<WarAndAiTweaksSettings>.Instance.MilitiaBoostCastle, bonusText, (TextObject)null);
			}
			if (((SettlementComponent)building.Town).IsTown)
			{
				result.Add((float)GlobalSettings<WarAndAiTweaksSettings>.Instance.MilitiaBoostTown, bonusText, (TextObject)null);
			}
		}
		return result;
	}
}
