using System.Collections;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using WarAndAITweaks.MilitaryAI;
using WarAndAiTweaks;

namespace WarAndAITweaks.ModelOverrides;

internal sealed class DelegatingPartySpeedModel : PartySpeedModel
{
	private PartySpeedModel _cachedInner;

	private PartySpeedModel Inner
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
				return (PartySpeedModel)new DefaultPartySpeedCalculatingModel();
			}
			PartySpeedModel val2 = null;
			foreach (PartySpeedModel item in ((IEnumerable)val).OfType<PartySpeedModel>())
			{
				if (item == this)
				{
					break;
				}
				val2 = item;
			}
			_cachedInner = (PartySpeedModel)(((object)val2) ?? ((object)new DefaultPartySpeedCalculatingModel()));
			return _cachedInner;
		}
	}

	public override float BaseSpeed => Inner.BaseSpeed;

	public override float MinimumSpeed => Inner.MinimumSpeed;

	public override ExplainedNumber CalculateBaseSpeed(MobileParty party, bool includeDescriptions = false, int additionalTroopOnFootCount = 0, int additionalTroopOnHorseCount = 0)
	{
		return Inner.CalculateBaseSpeed(party, includeDescriptions, additionalTroopOnFootCount, additionalTroopOnHorseCount);
	}

	public override ExplainedNumber CalculateFinalSpeed(MobileParty party, ExplainedNumber finalSpeed)
	{
		if (party == null)
		{
			return finalSpeed;
		}
		ExplainedNumber result = Inner.CalculateFinalSpeed(party, finalSpeed);
		if (!party.IsLordParty)
		{
			return result;
		}
		if (party.Army != null && party.Army.LeaderParty != party)
		{
			return result;
		}
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableSpeed)
		{
			return result;
		}
		var (val, flag) = MilitaryAIHelpers.GetFirstNearbyTownOrCastle(party, 30f);
		if (val != null)
		{
			if (flag)
			{
				result.Add(GlobalSettings<WarAndAiTweaksSettings>.Instance.SpeedBuffModifier, LanguageTranslater.L.T("near_friendly_infrastructure", "Near Friendly Infrastructure"), (TextObject)null);
			}
			else
			{
				result.Add(GlobalSettings<WarAndAiTweaksSettings>.Instance.SpeedDebuffModifier, LanguageTranslater.L.T("near_hostile_infrastructure", "Near Hostile Infrastructure"), (TextObject)null);
			}
		}
		return result;
	}
}

