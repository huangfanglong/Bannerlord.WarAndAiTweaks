using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
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
				return new DefaultPartySizeLimitModel();
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
			_cachedInner = val2 ?? new DefaultPartySizeLimitModel();
			return _cachedInner;
		}
	}

	public override int MinimumNumberOfVillagersAtVillagerParty => Inner.MinimumNumberOfVillagersAtVillagerParty;

	public override int GetAssumedPartySizeForLordParty(Hero hero, IFaction targetFaction, Clan forClan)
	{
		return Inner.GetAssumedPartySizeForLordParty(hero, targetFaction, forClan);
	}

	public override int GetClanTierPartySizeEffectForHero(Hero hero)
	{
		return Inner.GetClanTierPartySizeEffectForHero(hero);
	}

	public override int GetNextClanTierPartySizeEffectChangeForHero(Hero hero)
	{
		return Inner.GetNextClanTierPartySizeEffectChangeForHero(hero);
	}

	public override int GetIdealVillagerPartySize(Village village)
	{
		return Inner.GetIdealVillagerPartySize(village);
	}

	public override TroopRoster FindAppropriateInitialRosterForMobileParty(MobileParty party, PartyTemplateObject partyTemplate)
	{
		return Inner.FindAppropriateInitialRosterForMobileParty(party, partyTemplate);
	}

	public override List<Ship> FindAppropriateInitialShipsForMobileParty(MobileParty party, PartyTemplateObject partyTemplate)
	{
		return Inner.FindAppropriateInitialShipsForMobileParty(party, partyTemplate);
	}

	public override ExplainedNumber CalculateGarrisonPartySizeLimit(Settlement settlement, bool includeDescriptions = false)
	{
		return Inner.CalculateGarrisonPartySizeLimit(settlement, includeDescriptions);
	}

	public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false)
	{
		return Inner.GetPartyPrisonerSizeLimit(party, includeDescriptions);
	}

	public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = true)
	{
		ExplainedNumber partyMemberSizeLimit = Inner.GetPartyMemberSizeLimit(party, includeDescriptions);
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
					partyMemberSizeLimit.Add(num3, LanguageTranslater.L.T("[WAI] Villages bonus", "[WAI] Villages bonus"), (TextObject)null);
				}
				if (num2 > 0f)
				{
					partyMemberSizeLimit.Add(num2, LanguageTranslater.L.T("[WAI] Castles bonus", "[WAI] Castles bonus"), (TextObject)null);
				}
				if (num > 0f)
				{
					partyMemberSizeLimit.Add(num, LanguageTranslater.L.T("[WAI] Towns bonus", "[WAI] Towns bonus"), (TextObject)null);
				}
				return partyMemberSizeLimit;
			}
		}
		return partyMemberSizeLimit;
	}
}
