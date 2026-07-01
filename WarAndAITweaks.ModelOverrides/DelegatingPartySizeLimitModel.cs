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

internal sealed class DelegatingPartySizeLimitModel : PartySizeLimitModel
{
	private PartySizeLimitModel _cachedInner;

	private PartySizeLimitModel Inner
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
				return (PartySizeLimitModel)new DefaultPartySizeLimitModel();
			}
			PartySizeLimitModel val2 = null;
			foreach (PartySizeLimitModel item in ((IEnumerable)val).OfType<PartySizeLimitModel>())
			{
				if (item == this)
				{
					break;
				}
				val2 = item;
			}
			_cachedInner = (PartySizeLimitModel)(((object)val2) ?? ((object)new DefaultPartySizeLimitModel()));
			return _cachedInner;
		}
	}

	public override int GetTierPartySizeEffect(int tier)
	{
		return Inner.GetTierPartySizeEffect(tier);
	}

	public override int GetAssumedPartySizeForLordParty(Hero hero, IFaction targetFaction, Clan forClan)
	{
		return Inner.GetAssumedPartySizeForLordParty(hero, targetFaction, forClan);
	}

	public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool True)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Inner.GetPartyPrisonerSizeLimit(party, True);
	}

	public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		ExplainedNumber partyMemberSizeLimit = Inner.GetPartyMemberSizeLimit(party, true);
		if (!GlobalSettings<WarAndAiTweaksSettings>.Instance.EnablePartySizeBoost)
		{
			return partyMemberSizeLimit;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (party != null && party.MapFaction != null)
		{
			Hero leaderHero = party.LeaderHero;
			if (((leaderHero != null) ? leaderHero.Clan : null) != null)
			{
				if (!((IEnumerable<Settlement>)party.LeaderHero.Clan.Settlements).Any())
				{
					return partyMemberSizeLimit;
				}
				Hero leaderHero2 = party.LeaderHero;
				foreach (Settlement item in (List<Settlement>)(object)((leaderHero2 != null) ? leaderHero2.Clan.Settlements : null))
				{
					if (item.IsVillage)
					{
						num3 += (float)GlobalSettings<WarAndAiTweaksSettings>.Instance.PartySizeBonusVillage;
					}
					else if (item.IsCastle)
					{
						num2 += (float)GlobalSettings<WarAndAiTweaksSettings>.Instance.PartySizeBonusCastle;
					}
					else if (item.IsTown)
					{
						num += (float)GlobalSettings<WarAndAiTweaksSettings>.Instance.PartySizeBonusTown;
					}
				}
				if (num3 > 0f)
				{
					((ExplainedNumber)(ref partyMemberSizeLimit)).Add(num3, LanguageTranslater.L.T("[WAI] Villages bonus", "[WAI] Villages bonus"), (TextObject)null);
				}
				if (num2 > 0f)
				{
					((ExplainedNumber)(ref partyMemberSizeLimit)).Add(num2, LanguageTranslater.L.T("[WAI] Castles bonus", "[WAI] Castles bonus"), (TextObject)null);
				}
				if (num > 0f)
				{
					((ExplainedNumber)(ref partyMemberSizeLimit)).Add(num, LanguageTranslater.L.T("[WAI] Towns bonus", "[WAI] Towns bonus"), (TextObject)null);
				}
				return partyMemberSizeLimit;
			}
		}
		return partyMemberSizeLimit;
	}
}
