using System;
using System.Collections.Generic;
using System.Linq;
using FeastSystem;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;
using WarAndAiTweaks;

namespace WarAndAITweaks.MilitaryAI;

public class EnhancedAiMilitaryBehavior : CampaignBehaviorBase
{
	public class raidType
	{
		public Settlement target;

		public bool defending;

		public bool attacking;
	}

	[SaveableField(1001)]
	private static Dictionary<MobileParty, CampaignTime> ArmyCreationDate = new Dictionary<MobileParty, CampaignTime>();

	private static Dictionary<MobileParty, raidType> RaidTargets = new Dictionary<MobileParty, raidType>();

	private static Dictionary<MobileParty, AIBehaviorData> SavedBehaviors = new Dictionary<MobileParty, AIBehaviorData>();

	public const float behaviorScore = 10000f;

	public bool f = false;

	public static float ReasonableDistanceForVillageDefense { get; set; }

	public static float ReasonableDistanceForFortDefense { get; set; }

	public static float ReasonableDistanceForTownAttack { get; set; }

	public static float ReasonableDistanceForCastleAttack { get; set; }

	public static float ReasonableDistanceForRaid { get; set; }

	public static HashSet<IFaction> ValidAtWarFactions(IFaction sourceFaction)
	{
		HashSet<IFaction> hashSet = new HashSet<IFaction>();
		foreach (IFaction faction in Campaign.Current.Factions)
		{
			if (faction is Clan)
			{
				Clan val = (Clan)(object)((faction is Clan) ? faction : null);
				if (val != null && val != sourceFaction && val.Kingdom == null && ((IEnumerable<Settlement>)val.Settlements).Any() && val.IsAtWarWith(sourceFaction))
				{
					hashSet.Add(faction);
				}
			}
			if (faction is Kingdom)
			{
				Kingdom val2 = (Kingdom)(object)((faction is Kingdom) ? faction : null);
				if (val2 != null && val2 != sourceFaction && !val2.IsEliminated && ((IEnumerable<Settlement>)val2.Settlements).Any() && sourceFaction.IsAtWarWith((IFaction)(object)val2))
				{
					hashSet.Add(faction);
				}
			}
		}
		return hashSet;
	}

	public bool IsValidMilitaryKingdom(IFaction sourceFaction)
	{
		return sourceFaction != null && !sourceFaction.IsEliminated && ((IEnumerable<Settlement>)sourceFaction.Settlements).Any() && ValidAtWarFactions(sourceFaction).Any();
	}

	public static bool FactionIsDefensiveAgainstFaction(Kingdom SourceKingdom, Kingdom TargetKingdom)
	{
		return SourceKingdom.GetStanceWith((IFaction)(object)TargetKingdom).BehaviorPriority == 1;
	}

	public static bool FactionIsOffensiveAgainstFaction(Kingdom SourceKingdom, Kingdom TargetKingdom)
	{
		return SourceKingdom.GetStanceWith((IFaction)(object)TargetKingdom).BehaviorPriority == 2;
	}

	public static bool IsPlayerLeadKingdom(Kingdom SourceKingdom)
	{
		return SourceKingdom.Leader != null && SourceKingdom.Leader == Hero.MainHero;
	}

	public static bool IsPlayerClanParty(MobileParty party)
	{
		object obj;
		if (party == null)
		{
			obj = null;
		}
		else
		{
			Hero leaderHero = party.LeaderHero;
			obj = ((leaderHero != null) ? leaderHero.Clan : null);
		}
		return obj != null && party.LeaderHero.Clan == Hero.MainHero.Clan;
	}

	private static bool HasSettlementsUnderSiege(Kingdom k)
	{
		return k != null && ((IEnumerable<Settlement>)k.Settlements).Any((Settlement s) => s.IsUnderSiege && (s.IsTown || s.IsCastle));
	}

	public static bool PartyHasDefensiveObjective(MobileParty party)
	{
		return party != null && party.Objective == MobileParty.PartyObjective.Defensive;
	}

	public static bool PartyHasAggressiveObjective(MobileParty party)
	{
		return party != null && party.Objective == MobileParty.PartyObjective.Aggressive;
	}

	public static bool PartyHasBalancedObjective(MobileParty party)
	{
		return party != null && party.Objective == MobileParty.PartyObjective.Neutral;
	}

	public static bool PartyIsArmyLeader(MobileParty party)
	{
		return party != null && party.Army != null && party.Army.LeaderParty == party;
	}

	public static bool PartyIsReadyForMilitaryAction(MobileParty party)
	{
		return party != null && !party.IsDisbanding && (float)party.Party.NumberOfAllMembers / (float)party.Party.PartySizeLimit > 0.6f && party.Party.NumberOfAllMembers > 0 && (float)party.Party.NumberOfRegularMembers / (float)party.Party.NumberOfAllMembers > 0.6f && party.GetNumDaysForFoodToLast() >= 3;
	}

	public static bool IsReadyForPersonalDefense(MobileParty party)
	{
		return party != null && (float)party.GetNumDaysForFoodToLast() >= 3f;
	}

	private static bool HasArmyButNotLeader(MobileParty party)
	{
		return party != null && party.Army != null && party.Army.LeaderParty != party;
	}

	private static bool isValidParty(MobileParty party)
	{
		return party != null && party.Party != null && party.LeaderHero != null && party.IsLordParty && !party.IsDisbanding;
	}

	private static bool IsTooGoodToRaid(Hero hero)
	{
		return hero != null && (hero.GetTraitLevel(DefaultTraits.Honor) > 0 || hero.GetTraitLevel(DefaultTraits.Mercy) > 0);
	}

	private static bool IsValidRaidSettlement(MobileParty party, Settlement village, Kingdom sourceKingdom, bool isPlayerLeadKingdom, float partyStrength)
	{
		MobileParty party2 = party;
		Settlement village2 = village;
		if (party2 == null || village2 == null || sourceKingdom == null)
		{
			return false;
		}
		Hero owner = village2.Owner;
		object obj;
		if (owner == null)
		{
			obj = null;
		}
		else
		{
			Clan clan = owner.Clan;
			obj = ((clan != null) ? clan.Kingdom : null);
		}
		Kingdom val = (Kingdom)obj;
		if (val == null || val == sourceKingdom)
		{
			return false;
		}
		if (party2.GetPosition2D.Distance(village2.GetPosition2D) > ReasonableDistanceForRaid)
		{
			return false;
		}
		bool flag = RaidTargets.Any<KeyValuePair<MobileParty, raidType>>((KeyValuePair<MobileParty, raidType> x) => x.Key != party2 && x.Value != null && x.Value.target == village2 && x.Value.attacking);
		raidType value;
		bool flag2 = RaidTargets.TryGetValue(party2, out value) && value != null && value.target == village2 && value.attacking;
		if (flag && !flag2)
		{
			return false;
		}
		float totalStrengthOfDefenseForSettlement = MilitaryAIHelpers.GetTotalStrengthOfDefenseForSettlement(village2);
		if (partyStrength <= 2f * totalStrengthOfDefenseForSettlement)
		{
			return false;
		}
		return true;
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener((object)this, (Action)OnGameLoadFinishedEvent);
		CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener((object)this, (Action<MobileParty, PartyThinkParams>)AiHourlyTickEvent);
		CampaignEvents.KingdomCreatedEvent.AddNonSerializedListener((object)this, (Action<Kingdom>)OnKingdomCreated);
		CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener((object)this, (Action<Kingdom>)OnKingdomDestroyed);
		CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener((object)this, (Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementDetail>)OnSettlementOwnerChanged);
		CampaignEvents.WarDeclared.AddNonSerializedListener((object)this, (Action<IFaction, IFaction, DeclareWarDetail>)OnWarDeclared);
		CampaignEvents.MakePeace.AddNonSerializedListener((object)this, (Action<IFaction, IFaction, MakePeaceDetail>)OnMakePeace);
		CampaignEvents.ArmyDispersed.AddNonSerializedListener((object)this, (Action<Army, ArmyDispersionReason, bool>)OnArmyDispersed);
	}

	public override void SyncData(IDataStore dataStore)
	{
		try
		{
			dataStore.SyncData<Dictionary<MobileParty, CampaignTime>>("ArmyCreationDate", ref ArmyCreationDate);
		}
		catch
		{
			ArmyCreationDate = new Dictionary<MobileParty, CampaignTime>();
		}
	}

	private void OnGameLoadFinishedEvent()
	{
		float avgFortDistance = 56f;
		ReasonableDistanceForVillageDefense = (80f + 1.42f * avgFortDistance) / 2f;
		ReasonableDistanceForFortDefense = (160f + 2.84f * avgFortDistance) / 2f;
		ReasonableDistanceForTownAttack = (127f + 2.27f * avgFortDistance) / 2f;
		ReasonableDistanceForCastleAttack = (106f + 1.89f * avgFortDistance) / 2f;
		ReasonableDistanceForRaid = (106f + 1.89f * avgFortDistance) / 2f;
		MilitaryAIHelpers.ResetTargetCaches();
	}

	private void OnArmyDispersed(Army army, ArmyDispersionReason reason, bool isPlayersArmy)
	{
		if (army != null && army.LeaderParty != null && ArmyCreationDate.ContainsKey(army.LeaderParty))
		{
			ArmyCreationDate.Remove(army.LeaderParty);
		}
	}

	private void OnSettlementOwnerChanged(Settlement settlement, bool openToPlayer, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementDetail detail)
	{
		MilitaryAIHelpers.ResetTargetCaches();
	}

	private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarDetail detail)
	{
		MilitaryAIHelpers.ResetTargetCaches();
	}

	private void OnMakePeace(IFaction faction1, IFaction faction2, MakePeaceDetail detail)
	{
		MilitaryAIHelpers.ResetTargetCaches();
	}

	private void OnKingdomCreated(Kingdom k)
	{
		MilitaryAIHelpers.ResetTargetCaches();
	}

	private void OnKingdomDestroyed(Kingdom k)
	{
		MilitaryAIHelpers.ResetTargetCaches();
	}

	public static void SendToDefendWorkAround(MobileParty party, PartyThinkParams p, Settlement settlement)
	{
		if (party == null || p == null || settlement == null)
		{
			return;
		}
		AiBehavior val = (AiBehavior)8;
		MobileParty val2 = null;
		if (settlement.IsVillage)
		{
			val2 = MilitaryAIHelpers.GetRaidParty(settlement);
		}
		else
		{
			SiegeEvent siegeEvent = settlement.SiegeEvent;
			object obj;
			if (siegeEvent == null)
			{
				obj = null;
			}
			else
			{
				BesiegerCamp besiegerCamp = siegeEvent.BesiegerCamp;
				if (besiegerCamp == null)
				{
					obj = null;
				}
				else
				{
					MobileParty leaderParty = besiegerCamp.LeaderParty;
					if (leaderParty == null)
					{
						obj = null;
					}
					else
					{
						Army army = leaderParty.Army;
						obj = ((army != null) ? army.LeaderParty : null);
					}
				}
			}
			object obj2;
			if (obj == null)
			{
				SiegeEvent siegeEvent2 = settlement.SiegeEvent;
				if (siegeEvent2 == null)
				{
					obj2 = null;
				}
				else
				{
					BesiegerCamp besiegerCamp2 = siegeEvent2.BesiegerCamp;
					obj2 = ((besiegerCamp2 != null) ? besiegerCamp2.LeaderParty : null);
				}
			}
			else
			{
				SiegeEvent siegeEvent3 = settlement.SiegeEvent;
				if (siegeEvent3 == null)
				{
					obj2 = null;
				}
				else
				{
					BesiegerCamp besiegerCamp3 = siegeEvent3.BesiegerCamp;
					if (besiegerCamp3 == null)
					{
						obj2 = null;
					}
					else
					{
						MobileParty leaderParty2 = besiegerCamp3.LeaderParty;
						if (leaderParty2 == null)
						{
							obj2 = null;
						}
						else
						{
							Army army2 = leaderParty2.Army;
							obj2 = ((army2 != null) ? army2.LeaderParty : null);
						}
					}
				}
			}
			val2 = (MobileParty)obj2;
		}
		if (val2 != null)
		{
			SavedBehaviors[party] = new AIBehaviorData((IMapPoint)(object)val2, val, MobileParty.NavigationType.Default, false, false, false);
			(AIBehaviorData, float) tuple = (new AIBehaviorData((IMapPoint)(object)val2, val, MobileParty.NavigationType.Default, false, false, false), 10000f);
			p.AddBehaviorScore(in tuple);
		}
	}

	private void AiHourlyTickEvent(MobileParty mobileParty, PartyThinkParams p)
	{
		if (mobileParty.IsDisbanding)
		{
			return;
		}
		if (mobileParty.Army != null && mobileParty.Army.LeaderParty != mobileParty)
		{
			Hero leaderHero = mobileParty.LeaderHero;
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
			if (val != null && val.Leader != null && val.Leader != Hero.MainHero && mobileParty.LeaderHero == val.Leader)
			{
				mobileParty.Army = null;
			}
		}
		if (SavedBehaviors.ContainsKey(mobileParty) && !HasArmyButNotLeader(mobileParty))
		{
			AIBehaviorData item = SavedBehaviors[mobileParty];
			(AIBehaviorData, float) tuple = (item, float.MaxValue);
			p.AddBehaviorScore(in tuple);
			SavedBehaviors.Remove(mobileParty);
			return;
		}
		p.Initialization();
		object obj2;
		if (mobileParty == null)
		{
			obj2 = null;
		}
		else
		{
			Hero leaderHero2 = mobileParty.LeaderHero;
			if (leaderHero2 == null)
			{
				obj2 = null;
			}
			else
			{
				Clan clan2 = leaderHero2.Clan;
				obj2 = ((clan2 != null) ? clan2.Kingdom : null);
			}
		}
		Kingdom partyKingdom = (Kingdom)obj2;
		if (partyKingdom == null || !isValidParty(mobileParty) || HasArmyButNotLeader(mobileParty))
		{
			return;
		}
		HashSet<IFaction> hashSet = ValidAtWarFactions(partyKingdom.MapFaction);
		Campaign current = Campaign.Current;
		bool flag = ((current != null) ? current.GetCampaignBehavior<FeastBehavior>() : null) != null && FeastBehavior.GetFeastByAttribute((FeastData f) => f.Kingdom == partyKingdom) != null;
		if (!hashSet.Any() || flag)
		{
			HandleArmyDisband(mobileParty, isAtWar: false);
		}
		else
		{
			if (PartyIsArmyLeader(mobileParty) && !MilitaryAIHelpers.HasGatheredEnough(mobileParty, 0.4f))
			{
				return;
			}
			bool flag2 = PartyIsArmyLeader(mobileParty) || PartyIsReadyForMilitaryAction(mobileParty);
			bool kingdomUnderSiege = HasSettlementsUnderSiege(partyKingdom);
			if (!flag2 || (!FindMakeSiegeDefense(mobileParty, p, kingdomUnderSiege) && !FindMakeArmyAttackPlans(mobileParty, p, kingdomUnderSiege, hashSet)))
			{
				HandleArmyDisband(mobileParty, isAtWar: true);
				if (flag2)
				{
					FindPersonalRaid(mobileParty, p, hashSet);
				}
			}
		}
	}

	public bool FindMakeSiegeDefense(MobileParty party, PartyThinkParams p, bool kingdomUnderSiege)
	{
		MobileParty party2 = party;
		if (!kingdomUnderSiege)
		{
			return false;
		}
		Hero leaderHero = party2.LeaderHero;
		Clan partyClan = ((leaderHero != null) ? leaderHero.Clan : null);
		Clan obj = partyClan;
		Kingdom val = ((obj != null) ? obj.Kingdom : null);
		if (val == null)
		{
			return false;
		}
		bool flag = party2.Army != null && party2.Army.LeaderParty == party2;
		MBList<Settlement> val2 = Extensions.ToMBList<Settlement>((IEnumerable<Settlement>)(from s in (IEnumerable<Settlement>)partyClan.Settlements
			where s.IsUnderSiege && (s.IsTown || s.IsCastle)
			orderby s.GetValue(partyClan.Leader, true) descending
			select s));
		if (((List<Settlement>)(object)val2).Count == 0)
		{
			val2 = MilitaryAIHelpers.GetValidSiegeDefenseTargetsForKingdom(val);
		}
		if (val2 == null || ((List<Settlement>)(object)val2).Count == 0)
		{
			return false;
		}
		IFaction mapFaction = val.MapFaction;
		Settlement val3 = ((mapFaction != null) ? mapFaction.FactionMidSettlement : null);
		Settlement val4 = null;
		MobileParty val5 = null;
		float num = 10000f;
		foreach (Settlement item in (List<Settlement>)(object)val2)
		{
			if (val3 == null)
			{
				continue;
			}
			float distance = item.GetPosition2D.Distance(val3.GetPosition2D);
			if (distance < num)
			{
				num = distance;
				val4 = item;
				SiegeEvent siegeEvent = item.SiegeEvent;
				object obj2;
				if (siegeEvent == null)
				{
					obj2 = null;
				}
				else
				{
					BesiegerCamp besiegerCamp = siegeEvent.BesiegerCamp;
					obj2 = ((besiegerCamp != null) ? besiegerCamp.LeaderParty : null);
				}
				val5 = (MobileParty)obj2;
			}
		}
		List<Settlement> list = ((IEnumerable<Settlement>)val2).OrderBy((Settlement s) => party2.GetPosition2D.Distance(s.GetPosition2D)).ToList();
		List<MobileParty> armyMembers = MilitaryAIHelpers.GetCallMobilePartiesForArmy(party2);
		float num2 = (flag ? party2.GetTotalLandStrengthWithFollowers(true) : (party2.Party.EstimatedStrength + armyMembers.Sum((MobileParty x) => x.Party.EstimatedStrength)));
		foreach (Settlement item2 in list)
		{
			Hero owner = item2.Owner;
			object obj3;
			if (owner == null)
			{
				obj3 = null;
			}
			else
			{
				Clan clan = owner.Clan;
				obj3 = ((clan != null) ? clan.Leader : null);
			}
			Hero val6 = (Hero)obj3;
			if (val6 != null && GlobalSettings<WarAndAiTweaksSettings>.Instance.IgnoreLowRelationshipDefense)
			{
				int relation = party2.LeaderHero.GetRelation(val6);
				if (relation < -10)
				{
					continue;
				}
			}
			SiegeEvent siegeEvent2 = item2.SiegeEvent;
			object obj4;
			if (siegeEvent2 == null)
			{
				obj4 = null;
			}
			else
			{
				BesiegerCamp besiegerCamp2 = siegeEvent2.BesiegerCamp;
				obj4 = ((besiegerCamp2 != null) ? besiegerCamp2.LeaderParty : null);
			}
			MobileParty val7 = (MobileParty)obj4;
			if (val7 == null)
			{
				continue;
			}
			float totalSiegerStrengthForSettlement = MilitaryAIHelpers.GetTotalSiegerStrengthForSettlement(item2);
			if (item2 == val4)
			{
				if (num2 >= 2f * totalSiegerStrengthForSettlement)
				{
					if (!flag && !partyClan.IsUnderMercenaryService)
					{
						val.CreateArmy(party2.LeaderHero, item2, Army.ArmyTypes.Defender, new MBReadOnlyList<MobileParty>(armyMembers));
						HandleArmyCreation(party2);
					}
					SendToDefendWorkAround(party2, p, item2);
					return true;
				}
				SendToDefendWorkAround(party2, p, item2);
				return true;
			}
			if (!(num2 >= 2f * totalSiegerStrengthForSettlement))
			{
				continue;
			}
			if (!flag && !partyClan.IsUnderMercenaryService)
			{
				val.CreateArmy(party2.LeaderHero, item2, Army.ArmyTypes.Defender, new MBReadOnlyList<MobileParty>(armyMembers));
				HandleArmyCreation(party2);
			}
			SendToDefendWorkAround(party2, p, item2);
			return true;
		}
		return false;
	}

	public bool FindMakeArmyAttackPlans(MobileParty party, PartyThinkParams p, bool kingdomUnderSiege, HashSet<IFaction> AtWarKingdoms)
	{
		if (kingdomUnderSiege)
		{
			return false;
		}
		Hero leaderHero = party.LeaderHero;
		Clan val = ((leaderHero != null) ? leaderHero.Clan : null);
		Kingdom val2 = ((val != null) ? val.Kingdom : null);
		if (val2 == null)
		{
			return false;
		}
		MBList<Settlement> validAttackTargetsForKingdom = MilitaryAIHelpers.GetValidAttackTargetsForKingdom(val2, AtWarKingdoms);
		if (validAttackTargetsForKingdom == null || ((List<Settlement>)(object)validAttackTargetsForKingdom).Count == 0)
		{
			return false;
		}
		foreach (Settlement item in (List<Settlement>)(object)validAttackTargetsForKingdom)
		{
			if (item.IsUnderSiege && !MilitaryAIHelpers.IsSiegedByOwnKingdom(item, val2))
			{
				continue;
			}
			float totalStrengthOfDefenseForSettlement = MilitaryAIHelpers.GetTotalStrengthOfDefenseForSettlement(item);
			float num = (float)GlobalSettings<WarAndAiTweaksSettings>.Instance.MilitaryAdvantage * totalStrengthOfDefenseForSettlement;
			if (PartyIsArmyLeader(party))
			{
				float totalStrengthOfArmiesTargetSameSettlement = MilitaryAIHelpers.GetTotalStrengthOfArmiesTargetSameSettlement(item, party);
				totalStrengthOfArmiesTargetSameSettlement += party.GetTotalLandStrengthWithFollowers(true);
				if ((double)totalStrengthOfArmiesTargetSameSettlement >= (double)num * 0.7)
				{
					SavedBehaviors[party] = new AIBehaviorData((IMapPoint)(object)item, (AiBehavior)5, MobileParty.NavigationType.Default, false, false, false);
					(AIBehaviorData, float) tuple = (new AIBehaviorData((IMapPoint)(object)item, (AiBehavior)5, MobileParty.NavigationType.Default, false, false, false), 10000f);
					p.AddBehaviorScore(in tuple);
					return true;
				}
				continue;
			}
			float totalSpendableInfluenceFromClan = MilitaryAIHelpers.GetTotalSpendableInfluenceFromClan(val);
			if (!(totalSpendableInfluenceFromClan < 50f) && (val != Hero.MainHero.Clan || GlobalSettings<WarAndAiTweaksSettings>.Instance.EnablePlayerClanArmyCreation))
			{
				List<MobileParty> armyMembers = MilitaryAIHelpers.GetCallMobilePartiesForArmy(party);
				float num2 = armyMembers.Sum((MobileParty x) => x.Party.EstimatedStrength);
				if ((double)num2 >= (double)num * 1.5 && num2 > 600f && !val.IsUnderMercenaryService)
				{
					val2.CreateArmy(party.LeaderHero, item, Army.ArmyTypes.Besieger, new MBReadOnlyList<MobileParty>(armyMembers));
					HandleArmyCreation(party);
					return true;
				}
			}
		}
		return false;
	}

	private void FindPersonalRaid(MobileParty party, PartyThinkParams p, HashSet<IFaction> AtWarFactions)
	{
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
		if (val == null)
		{
			return;
		}
		raidType raidType = (RaidTargets.ContainsKey(party) ? RaidTargets[party] : null);
		bool flag = IsPlayerLeadKingdom(val);
		float totalStrength = party.Party.EstimatedStrength;
		if (raidType != null && raidType.target != null && raidType.target.IsUnderRaid && raidType.defending)
		{
			SendToDefendWorkAround(party, p, raidType.target);
			return;
		}
		if (raidType != null && raidType.target != null && raidType.attacking && IsValidRaidSettlement(party, raidType.target, val, flag, totalStrength))
		{
			SavedBehaviors[party] = new AIBehaviorData((IMapPoint)(object)raidType.target, (AiBehavior)4, MobileParty.NavigationType.Default, false, false, false);
			(AIBehaviorData, float) tuple = (new AIBehaviorData((IMapPoint)(object)raidType.target, (AiBehavior)4, MobileParty.NavigationType.Default, false, false, false), 10000f);
			p.AddBehaviorScore(in tuple);
			return;
		}
		RaidTargets.Remove(party);
		List<Settlement> list = ((IEnumerable<Settlement>)val.Settlements).Where((Settlement s) => s.IsVillage).ToList();
		foreach (Settlement item in list)
		{
			Hero owner = item.Owner;
			object obj2;
			if (owner == null)
			{
				obj2 = null;
			}
			else
			{
				Clan clan2 = owner.Clan;
				obj2 = ((clan2 != null) ? clan2.Leader : null);
			}
			Hero val2 = (Hero)obj2;
			if (val2 != null && GlobalSettings<WarAndAiTweaksSettings>.Instance.IgnoreLowRelationshipDefense)
			{
				int relation = party.LeaderHero.GetRelation(val2);
				if (relation < -10)
				{
					continue;
				}
			}
			Hero owner2 = item.Owner;
			object obj3;
			if (owner2 == null)
			{
				obj3 = null;
			}
			else
			{
				Clan clan3 = owner2.Clan;
				obj3 = ((clan3 != null) ? clan3.Kingdom : null);
			}
			Kingdom val3 = (Kingdom)obj3;
			if (val3 != null && item.IsUnderRaid && party.GetPosition2D.Distance(item.GetPosition2D) > ReasonableDistanceForRaid)
			{
				MobileParty lastAttackerParty = item.LastAttackerParty;
				if (lastAttackerParty != null)
				{
					raidType = new raidType
					{
						target = item,
						defending = true,
						attacking = false
					};
					RaidTargets[party] = raidType;
					SendToDefendWorkAround(party, p, item);
					return;
				}
			}
		}
		MBList<Settlement> militaryTargetVillages = MilitaryAIHelpers.GetMilitaryTargetVillages(party, AtWarFactions);
		if (militaryTargetVillages == null || ((List<Settlement>)(object)militaryTargetVillages).Count == 0)
		{
			return;
		}
		foreach (Settlement item2 in (List<Settlement>)(object)militaryTargetVillages)
		{
			Hero owner3 = item2.Owner;
			object obj4;
			if (owner3 == null)
			{
				obj4 = null;
			}
			else
			{
				Clan clan4 = owner3.Clan;
				obj4 = ((clan4 != null) ? clan4.Kingdom : null);
			}
			Kingdom val4 = (Kingdom)obj4;
			if (val4 != null && val4 != val && (!IsTooGoodToRaid(party.LeaderHero) || GlobalSettings<WarAndAiTweaksSettings>.Instance.AllowRaidingTooGoodLords))
			{
				bool flag2 = false;
				if (flag && IsPlayerClanParty(party) && PartyHasAggressiveObjective(party))
				{
					flag2 = true;
				}
				else if (flag && !IsPlayerClanParty(party) && FactionIsOffensiveAgainstFaction(val, val4))
				{
					flag2 = true;
				}
				else if (!flag)
				{
					flag2 = true;
				}
				if (flag2 && IsValidRaidSettlement(party, item2, val, flag, totalStrength))
				{
					raidType = new raidType
					{
						target = item2,
						defending = false,
						attacking = true
					};
					RaidTargets[party] = raidType;
					SavedBehaviors[party] = new AIBehaviorData((IMapPoint)(object)item2, (AiBehavior)4, MobileParty.NavigationType.Default, false, false, false);
					(AIBehaviorData, float) tuple = (new AIBehaviorData((IMapPoint)(object)item2, (AiBehavior)4, MobileParty.NavigationType.Default, false, false, false), 10000f);
					p.AddBehaviorScore(in tuple);
					break;
				}
			}
		}
	}

	private void HandleArmyDisband(MobileParty mobileParty, bool isAtWar)
	{
		if (mobileParty.Army != null && mobileParty.Army.LeaderParty == MobileParty.MainParty)
		{
			return;
		}
		Army army = mobileParty.Army;
		if (army == null)
		{
			return;
		}
		if (ArmyCreationDate.TryGetValue(mobileParty, out var value) && isAtWar)
		{
			CampaignTime now = CampaignTime.Now;
			if (!(now.ToDays - value.ToDays >= 3.0))
			{
				return;
			}
		}
		DisbandArmyAction.ApplyByObjectiveFinished(army);
		ArmyCreationDate.Remove(mobileParty);
	}

	private void HandleArmyCreation(MobileParty party)
	{
		if (ArmyCreationDate.ContainsKey(party))
		{
			ArmyCreationDate[party] = CampaignTime.Now;
		}
		else
		{
			ArmyCreationDate.Add(party, CampaignTime.Now);
		}
	}
}





