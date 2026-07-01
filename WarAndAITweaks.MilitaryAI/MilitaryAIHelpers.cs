using System;
using System.Collections.Generic;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Library;
using WarAndAITweaks.MarshalSystem;
using WarAndAITweaks.UI;
using WarAndAiTweaks;

namespace WarAndAITweaks.MilitaryAI;

public class MilitaryAIHelpers
{
	private static Dictionary<Kingdom, MBList<Settlement>> SiegeTargetsCache = new Dictionary<Kingdom, MBList<Settlement>>();

	private static Dictionary<Kingdom, MBList<Settlement>> VillageTargetCache = new Dictionary<Kingdom, MBList<Settlement>>();

	public static float GetTotalSpendableInfluenceFromClan(Clan clan)
	{
		return MathF.Max(clan.Influence - 200f, 0f);
	}

	public static bool HasGatheredEnough(MobileParty party, float percentage)
	{
		float totalStrengthWithFollowers = party.GetTotalStrengthWithFollowers(true);
		float totalStrengthWithFollowers2 = party.GetTotalStrengthWithFollowers(false);
		if (totalStrengthWithFollowers2 >= totalStrengthWithFollowers * percentage)
		{
			return true;
		}
		return false;
	}

	public static bool IsAnyArmyTryingToDefend(Kingdom k, Settlement settlement)
	{
		Settlement settlement2 = settlement;
		return ((IEnumerable<Army>)k.Armies).Any((Army x) => x.LeaderParty.TargetSettlement == settlement2);
	}

	public static bool FoundStrongerHostilePartyNearSettlement(Settlement x, MobileParty party, float radius)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Hero leaderHero = party.LeaderHero;
		object obj;
		if (leaderHero == null)
		{
			obj = null;
		}
		else
		{
			Clan clan = leaderHero.Clan;
			obj = ((clan != null) ? clan.Kingdom : null);
		}
		Kingdom val = (Kingdom)obj;
		if (party == null || x == null || val == null)
		{
			return false;
		}
		float totalStrength = party.Party.TotalStrength;
		LocatableSearchData<MobileParty> val2 = MobileParty.StartFindingLocatablesAroundPosition(x.Position2D, radius);
		for (MobileParty val3 = MobileParty.FindNextLocatable(ref val2); val3 != null; val3 = MobileParty.FindNextLocatable(ref val2))
		{
			if (val3.IsLordParty)
			{
				Hero leaderHero2 = val3.LeaderHero;
				object obj2;
				if (leaderHero2 == null)
				{
					obj2 = null;
				}
				else
				{
					Clan clan2 = leaderHero2.Clan;
					obj2 = ((clan2 != null) ? clan2.Kingdom : null);
				}
				Kingdom val4 = (Kingdom)obj2;
				if (val4 != null && val.IsAtWarWith((IFaction)(object)val4))
				{
					PartyBase party2 = val3.Party;
					if (((party2 != null) ? party2.TotalStrength : 0f) > totalStrength * 1.2f)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static bool IsVillageTargetedForRaid(Settlement village, MobileParty self)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Invalid comparison between Unknown and I4
		Kingdom kingdom = self.LeaderHero.Clan.Kingdom;
		object obj;
		if (self == null)
		{
			obj = null;
		}
		else
		{
			Hero leaderHero = self.LeaderHero;
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
		if (obj == null || village == null || !village.IsVillage)
		{
			return false;
		}
		IEnumerable<MobileParty> allParties = kingdom.AllParties;
		if (allParties == null)
		{
			return false;
		}
		foreach (MobileParty item in allParties)
		{
			if (item == null || item == self || !item.IsLordParty || !item.IsActive || item.IsDisbanding || item.MapEvent != null || item.Ai == null || item.TargetSettlement != village || (int)item.DefaultBehavior != 4)
			{
				continue;
			}
			return true;
		}
		return false;
	}

	public static float GetStrengthOfVillageRaiders(Settlement village)
	{
		if (village == null || !village.IsVillage)
		{
			return 0f;
		}
		MBReadOnlyList<MobileParty> lordParties = Campaign.Current.LordParties;
		if (lordParties == null)
		{
			return 0f;
		}
		foreach (MobileParty item in (List<MobileParty>)(object)lordParties)
		{
			if (((item != null) ? item.MapEvent : null) != null && item.MapEvent.IsRaid && item.TargetSettlement == village)
			{
				return item.GetTotalStrengthWithFollowers(true);
			}
		}
		return 0f;
	}

	public static (Settlement settlement, bool isFriendly) GetFirstNearbyTownOrCastle(MobileParty party, float radius)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (party == null)
		{
			return (settlement: null, isFriendly: false);
		}
		LocatableSearchData<Settlement> val = Settlement.StartFindingLocatablesAroundPosition(party.Position2D, radius);
		for (Settlement val2 = Settlement.FindNextLocatable(ref val); val2 != null; val2 = Settlement.FindNextLocatable(ref val))
		{
			if (val2.IsTown || val2.IsCastle)
			{
				if (val2.MapFaction == party.MapFaction)
				{
					return (settlement: val2, isFriendly: true);
				}
				if (val2.MapFaction != null && party.MapFaction != null && val2.MapFaction.IsAtWarWith(party.MapFaction))
				{
					return (settlement: val2, isFriendly: false);
				}
			}
		}
		return (settlement: null, isFriendly: false);
	}

	public static float GetTotalStrengthOfArmiesTargetSameSettlement(Settlement settlement, MobileParty party)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		object obj;
		if (party == null)
		{
			obj = null;
		}
		else
		{
			Hero leaderHero = party.LeaderHero;
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
		Kingdom val = (Kingdom)obj;
		if (val == null || settlement == null)
		{
			return 0f;
		}
		float num = 0f;
		foreach (Army item in (List<Army>)(object)val.Armies)
		{
			MobileParty leaderParty = item.LeaderParty;
			if (leaderParty != null && leaderParty != party && leaderParty.TargetSettlement == settlement && (int)leaderParty.DefaultBehavior == 5)
			{
				num += leaderParty.GetTotalStrengthWithFollowers(true);
			}
		}
		return num;
	}

	public static MBList<Settlement> GetValidAttackTargetsForKingdom(Kingdom kingdom, HashSet<IFaction> AtWarKingdoms)
	{
		if (kingdom == null)
		{
			return new MBList<Settlement>();
		}
		if (!AtWarKingdoms.Any())
		{
			return new MBList<Settlement>();
		}
		if (EnhancedAiMilitaryBehavior.IsPlayerLeadKingdom(kingdom))
		{
			Settlement globalAttackTarget = MilitaryManagementVM.GetGlobalAttackTarget();
			if (globalAttackTarget != null)
			{
				MBList<Settlement> obj = new MBList<Settlement>();
				((List<Settlement>)(object)obj).Add(globalAttackTarget);
				return obj;
			}
		}
		if (SiegeTargetsCache.TryGetValue(kingdom, out MBList<Settlement> value))
		{
			return value;
		}
		MBList<Settlement> val = new MBList<Settlement>();
		MBList<(Settlement, float)> val2 = new MBList<(Settlement, float)>();
		IFaction mapFaction = kingdom.MapFaction;
		Settlement val3 = ((mapFaction != null) ? mapFaction.FactionMidSettlement : null);
		if (val3 == null)
		{
			return val;
		}
		foreach (IFaction AtWarKingdom in AtWarKingdoms)
		{
			if (AtWarKingdom.Settlements == null)
			{
				continue;
			}
			foreach (Settlement item in (List<Settlement>)(object)AtWarKingdom.Settlements)
			{
				if (item.IsTown || item.IsCastle)
				{
					float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(item, val3);
					((List<(Settlement, float)>)(object)val2).Add((item, distance));
				}
			}
		}
		((List<(Settlement, float)>)(object)val2).Sort((Comparison<(Settlement, float)>)(((Settlement settlement, float distance) a, (Settlement settlement, float distance) b) => a.distance.CompareTo(b.distance)));
		int num = Math.Min(2, ((List<(Settlement, float)>)(object)val2).Count);
		for (int i = 0; i < num; i++)
		{
			((List<Settlement>)(object)val).Add(((List<(Settlement, float)>)(object)val2)[i].Item1);
		}
		SiegeTargetsCache[kingdom] = val;
		return val;
	}

	public static MBList<Settlement> GetMilitaryTargetVillages(MobileParty party, HashSet<IFaction> atWarFactions)
	{
		MBList<Settlement> result = new MBList<Settlement>();
		Hero leaderHero = party.LeaderHero;
		object obj;
		if (leaderHero == null)
		{
			obj = null;
		}
		else
		{
			Clan clan = leaderHero.Clan;
			obj = ((clan != null) ? clan.Kingdom : null);
		}
		Kingdom val = (Kingdom)obj;
		if (party == null || val == null)
		{
			return result;
		}
		if (VillageTargetCache.TryGetValue(val, out MBList<Settlement> value))
		{
			return value;
		}
		MBList<Settlement> val2 = new MBList<Settlement>();
		foreach (IFaction atWarFaction in atWarFactions)
		{
			if (atWarFaction.Settlements == null)
			{
				continue;
			}
			foreach (Settlement item in (List<Settlement>)(object)atWarFaction.Settlements)
			{
				if (item.IsVillage && !item.IsRaided)
				{
					((List<Settlement>)(object)val2).Add(item);
				}
			}
		}
		VillageTargetCache[val] = val2;
		return val2;
	}

	public static void ResetTargetCaches()
	{
		SiegeTargetsCache.Clear();
		VillageTargetCache.Clear();
	}

	public static MBList<Settlement> GetValidSiegeDefenseTargetsForKingdom(Kingdom kingdom)
	{
		MBList<Settlement> val = new MBList<Settlement>();
		if (kingdom == null || kingdom.Settlements == null)
		{
			return val;
		}
		foreach (Settlement item in (List<Settlement>)(object)kingdom.Settlements)
		{
			if ((item.IsTown || item.IsCastle) && item.IsUnderSiege)
			{
				((List<Settlement>)(object)val).Add(item);
			}
		}
		return val;
	}

	public static bool IsSiegedByOwnKingdom(Settlement settlement, Kingdom kingdom)
	{
		if (settlement == null || kingdom == null || !settlement.IsUnderSiege)
		{
			return false;
		}
		SiegeEvent siegeEvent = settlement.SiegeEvent;
		object obj;
		if (siegeEvent == null)
		{
			obj = null;
		}
		else
		{
			BesiegerCamp besiegerCamp = siegeEvent.BesiegerCamp;
			obj = ((besiegerCamp != null) ? besiegerCamp.LeaderParty : null);
		}
		MobileParty val = (MobileParty)obj;
		if (val == null)
		{
			return false;
		}
		Hero leaderHero = val.LeaderHero;
		object obj2;
		if (leaderHero == null)
		{
			obj2 = null;
		}
		else
		{
			Clan clan = leaderHero.Clan;
			obj2 = ((clan != null) ? clan.Kingdom : null);
		}
		Kingdom val2 = (Kingdom)obj2;
		if (val2 == null)
		{
			return false;
		}
		if (val2 == kingdom)
		{
			return true;
		}
		return false;
	}

	public static float GetTotalSiegerStrengthForSettlement(Settlement settlement)
	{
		BesiegerCamp val = settlement.SiegeEvent?.BesiegerCamp;
		if (val == null)
		{
			return 0f;
		}
		float num = 0f;
		return ((settlement == null) ? null : settlement.SiegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType((BattleTypes)5)?.Sum((PartyBase x) => x.TotalStrength)).GetValueOrDefault();
	}

	public static float GetTotalStrengthOfDefenseForSettlement(Settlement settlement)
	{
		if (!((IEnumerable<MobileParty>)settlement.Parties).Any())
		{
			return 0f;
		}
		return ((IEnumerable<MobileParty>)settlement.Parties).Where((MobileParty x) => x.IsGarrison || x.IsMilitia).Sum((MobileParty x) => x.Party.TotalStrength);
	}

	public static List<MobileParty> GetCallMobilePartiesForArmy(MobileParty leaderParty)
	{
		List<MobileParty> list = new List<MobileParty>();
		if (((leaderParty != null) ? leaderParty.LeaderHero : null) == null)
		{
			return list;
		}
		float num = leaderParty.LeaderHero.Clan.Influence;
		if (num < 50f)
		{
			return list;
		}
		IDisbandPartyCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IDisbandPartyCampaignBehavior>();
		WarAndAiTweaksSettings instance = GlobalSettings<WarAndAiTweaksSettings>.Instance;
		IFaction mapFaction = leaderParty.MapFaction;
		MBReadOnlyList<WarPartyComponent> val = ((mapFaction != null) ? mapFaction.WarPartyComponents : null);
		if (val == null)
		{
			return list;
		}
		List<(MobileParty, float, int)> list2 = new List<(MobileParty, float, int)>(((List<WarPartyComponent>)(object)val).Count);
		foreach (WarPartyComponent item in (List<WarPartyComponent>)(object)val)
		{
			MobileParty mobileParty = ((PartyComponent)item).MobileParty;
			Hero val2 = ((mobileParty != null) ? mobileParty.LeaderHero : null);
			if (mobileParty == null || mobileParty == leaderParty || val2 == null || !mobileParty.IsLordParty || mobileParty.Army != null || mobileParty.IsMainParty)
			{
				continue;
			}
			Hero leaderHero = mobileParty.LeaderHero;
			object obj;
			if (leaderHero == null)
			{
				obj = null;
			}
			else
			{
				Clan clan = leaderHero.Clan;
				if (clan == null)
				{
					obj = null;
				}
				else
				{
					Kingdom kingdom = clan.Kingdom;
					obj = ((kingdom != null) ? kingdom.Leader : null);
				}
			}
			if (obj != null)
			{
				Hero leaderHero2 = mobileParty.LeaderHero;
				Hero leaderHero3 = mobileParty.LeaderHero;
				object obj2;
				if (leaderHero3 == null)
				{
					obj2 = null;
				}
				else
				{
					Clan clan2 = leaderHero3.Clan;
					if (clan2 == null)
					{
						obj2 = null;
					}
					else
					{
						Kingdom kingdom2 = clan2.Kingdom;
						obj2 = ((kingdom2 != null) ? kingdom2.Leader : null);
					}
				}
				if (leaderHero2 == obj2)
				{
					continue;
				}
			}
			if (mobileParty.Ai == null || mobileParty.Ai.DoNotMakeNewDecisions || MarshalSystemBehavior.IsMarshal(val2))
			{
				continue;
			}
			Settlement currentSettlement = mobileParty.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) != null || mobileParty.IsDisbanding || mobileParty.Food <= 0f - mobileParty.FoodChange * 5f || mobileParty.PartySizeRatio <= 0.6f || !val2.CanLeadParty() || mobileParty.MapEvent != null || mobileParty.BesiegedSettlement != null || (campaignBehavior != null && campaignBehavior.IsPartyWaitingForDisband(mobileParty)))
			{
				continue;
			}
			int num2 = Campaign.Current.Models.ArmyManagementCalculationModel.CalculatePartyInfluenceCost(leaderParty, mobileParty);
			if (!((float)num2 > num))
			{
				float totalStrength = mobileParty.Party.TotalStrength;
				float num3 = 1f - (float)mobileParty.Party.MemberRoster.TotalWounded / MathF.Max(1f, (float)mobileParty.Party.MemberRoster.TotalManCount);
				float num4 = totalStrength / ((float)num2 + 0.1f) * num3;
				if (num4 > 0f)
				{
					list2.Add((mobileParty, num4, num2));
				}
			}
		}
		if (list2.Count == 0)
		{
			return list;
		}
		list2.Sort(((MobileParty party, float score, int cost) a, (MobileParty party, float score, int cost) b) => b.score.CompareTo(a.score));
		foreach (var item2 in list2)
		{
			if (num > (float)item2.Item3)
			{
				num -= (float)item2.Item3;
				list.Add(item2.Item1);
			}
		}
		return list;
	}

	public static MobileParty GetRaidParty(Settlement village)
	{
		if (!village.IsUnderRaid)
		{
			return null;
		}
		if (village.LastAttackerParty != null && village.LastAttackerParty.CurrentSettlement == village)
		{
			return village.LastAttackerParty;
		}
		return null;
	}
}
