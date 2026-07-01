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
using WarAndAiTweaks;

namespace WarAndAITweaks.ModelOverrides;

internal class DelegatingBuildingEffectModel : BuildingEffectModel
{
	private BuildingEffectModel _cachedInner;

	private BuildingEffectModel Inner
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
				return (BuildingEffectModel)new DefaultBuildingEffectModel();
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
			_cachedInner = (BuildingEffectModel)(((object)val2) ?? ((object)new DefaultBuildingEffectModel()));
			return _cachedInner;
		}
	}

	public override float GetBuildingEffectAmount(Building building, BuildingEffectEnum effect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		float num = Inner.GetBuildingEffectAmount(building, effect);
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableMilitiaBoost)
		{
			return num;
		}
		if (((object)building.Name).ToString() == "Militia Grounds" && (int)effect == 7)
		{
			if (((SettlementComponent)building.Town).IsCastle)
			{
				num += (float)GlobalSettings<WarAndAiTweaksSettings>.Instance.MilitiaBoostCastle;
			}
			if (((SettlementComponent)building.Town).IsTown)
			{
				num += (float)GlobalSettings<WarAndAiTweaksSettings>.Instance.MilitiaBoostTown;
			}
		}
		return num;
	}
}
