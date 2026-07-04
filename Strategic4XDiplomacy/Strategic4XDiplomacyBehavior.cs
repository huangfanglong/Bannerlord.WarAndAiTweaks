using System;
using System.Collections.Generic;
using System.Linq;
using FeastSystem;
using Helpers;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using WarAndAITweaks.Utils;
using WarAndAITweaks.War_Peace_AI_Overhaul;
using WarAndAiTweaks;

namespace Strategic4XDiplomacy;

public class Strategic4XDiplomacyBehavior : CampaignBehaviorBase
{
	[SaveableField(0)]
	public static Dictionary<Kingdom, CampaignTime> _lastDiplomaticAction = new Dictionary<Kingdom, CampaignTime>();

	[SaveableField(1)]
	public static Dictionary<Kingdom, float> _warFatigue = new Dictionary<Kingdom, float>();

	[SaveableField(2)]
	public static Dictionary<Kingdom, CampaignTime> _lastPlayerPeaceOffer = new Dictionary<Kingdom, CampaignTime>();

	[SaveableField(3)]
	public static Dictionary<Kingdom, CampaignTime> _lastRequestByAIToPlayer = new Dictionary<Kingdom, CampaignTime>();

	public static Dictionary<Kingdom, TargetCacher> _targetCachers = new Dictionary<Kingdom, TargetCacher>();

	public static MBList<Kingdom> MajorKingdoms = new MBList<Kingdom>();

	public static MBList<Kingdom> snowballingKingdoms = new MBList<Kingdom>();

	private static Dictionary<Kingdom, Queue<bool>> _warFatigueUpdateQueue = new Dictionary<Kingdom, Queue<bool>>();

	private static FeastBehavior _cachedFeastBehavior;

	public static float ReasonableDistanceForBesiegingTown { get; set; }

	public static float GetWarFatigue(Kingdom k)
	{
		float value;
		return (k != null && _warFatigue.TryGetValue(k, out value)) ? value : 0f;
	}

	public HashSet<Kingdom> CurrentWars(Kingdom sourceKingdom)
	{
		Kingdom sourceKingdom2 = sourceKingdom;
		return ((IEnumerable<Kingdom>)Kingdom.All).Where((Kingdom x) => isValidMilitaryTargetForKingdom(sourceKingdom2, x)).ToHashSet();
	}

	public bool IsValidMilitaryKingdom(Kingdom sourceKingdom)
	{
		return sourceKingdom != null && ((IEnumerable<Settlement>)sourceKingdom.Settlements).Any() && CurrentWars(sourceKingdom).Any();
	}

	public static bool isValidMilitaryTargetForKingdom(Kingdom sourceKingdom, Kingdom targetKingdom)
	{
		return !targetKingdom.IsEliminated && ((IEnumerable<Settlement>)targetKingdom.Settlements).Any() && sourceKingdom.IsAtWarWith((IFaction)(object)targetKingdom);
	}

	public override void RegisterEvents()
	{
		CampaignEvents.DailyTickEvent.AddNonSerializedListener((object)this, (Action)DailyTick);
		CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener((object)this, (Action)OnGameLoadFinishedEvent);
		CampaignEvents.KingdomCreatedEvent.AddNonSerializedListener((object)this, (Action<Kingdom>)OnKingdomCreated);
		CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener((object)this, (Action<Kingdom>)OnKingdomDestroyed);
		CampaignEvents.DailyTickClanEvent.AddNonSerializedListener((object)this, (Action<Clan>)DailyTickClanEvent);
		CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener((object)this, (Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementDetail>)OnSettlementOwnerChanged);
	}

	public override void SyncData(IDataStore dataStore)
	{
		try
		{
			dataStore.SyncData<Dictionary<Kingdom, CampaignTime>>("_lastDiplomaticAction", ref _lastDiplomaticAction);
		}
		catch (Exception)
		{
			_lastDiplomaticAction = new Dictionary<Kingdom, CampaignTime>();
		}
		try
		{
			dataStore.SyncData<Dictionary<Kingdom, float>>("_warFatigue", ref _warFatigue);
		}
		catch (Exception)
		{
			_warFatigue = new Dictionary<Kingdom, float>();
		}
		try
		{
			dataStore.SyncData<Dictionary<Kingdom, CampaignTime>>("_lastPlayerPeaceOffer", ref _lastPlayerPeaceOffer);
		}
		catch (Exception)
		{
			_lastPlayerPeaceOffer = new Dictionary<Kingdom, CampaignTime>();
		}
		try
		{
			dataStore.SyncData<Dictionary<Kingdom, CampaignTime>>("_lastRequestByAIToPlayer", ref _lastRequestByAIToPlayer);
		}
		catch (Exception)
		{
			_lastRequestByAIToPlayer = new Dictionary<Kingdom, CampaignTime>();
		}
	}

	private void OnGameLoadFinishedEvent()
	{
		DiplomacySettingsCache.Initialize();
		StrategicX4AIHelpers.UpdateMajorKingdoms();
		List<Kingdom> list = _targetCachers.Keys.Where((Kingdom k) => !((List<Kingdom>)(object)MajorKingdoms).Contains(k)).ToList();
		foreach (Kingdom item in list)
		{
			_targetCachers.Remove(item);
		}
		foreach (Kingdom item2 in (List<Kingdom>)(object)MajorKingdoms)
		{
			updateKingdomTargetsCache(item2);
		}
		ReasonableDistanceForBesiegingTown = (127f + 2.27f * 56f) / 2f;
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableBaseGameTributes)
		{
			return;
		}
		foreach (Kingdom item3 in (List<Kingdom>)(object)Kingdom.All)
		{
			foreach (Kingdom otherKingdom in (List<Kingdom>)(object)Kingdom.All)
			{
				if (otherKingdom == item3)
				{
					continue;
				}
				StanceLink stanceWith = item3.GetStanceWith((IFaction)(object)otherKingdom);
				if (stanceWith != null)
				{
					stanceWith.SetDailyTributePaid(item3.MapFaction, 0, 0);
				}
			}
		}
	}

	private void OnSettlementOwnerChanged(Settlement settlement, bool openToPlayer, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementDetail detail)
	{
		StrategicX4AIHelpers.UpdateMajorKingdoms();
		foreach (TargetCacher value in _targetCachers.Values)
		{
			value.needsUpdate = true;
		}
	}

	private void OnKingdomCreated(Kingdom kingdom)
	{
		StrategicX4AIHelpers.UpdateMajorKingdoms();
		updateKingdomTargetsCache(kingdom);
	}

	private void OnKingdomDestroyed(Kingdom kingdom)
	{
		StrategicX4AIHelpers.UpdateMajorKingdoms();
		_targetCachers.Remove(kingdom);
		_lastDiplomaticAction.Remove(kingdom);
		_lastPlayerPeaceOffer.Remove(kingdom);
		_lastRequestByAIToPlayer.Remove(kingdom);
		_warFatigue.Remove(kingdom);
		((List<Kingdom>)(object)snowballingKingdoms).Remove(kingdom);
		_warFatigueUpdateQueue.Remove(kingdom);
	}

	private void DailyTick()
	{
		StrategicX4AIHelpers.UpdateMajorKingdoms();
		foreach (Kingdom item in (List<Kingdom>)(object)MajorKingdoms)
		{
			if (!_warFatigueUpdateQueue.ContainsKey(item))
			{
				_warFatigueUpdateQueue[item] = new Queue<bool>();
			}
			_warFatigueUpdateQueue[item].Enqueue(item: true);
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.ShowWarFatigueDebug)
			{
				InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] Queued war fatigue update for {item.Name}. Queue size: {_warFatigueUpdateQueue[item].Count}", Colors.Cyan));
			}
		}
	}

	private void DailyTickClanEvent(Clan clan)
	{
		if (((clan != null) ? clan.Kingdom : null) == null || !((List<Kingdom>)(object)MajorKingdoms).Contains(clan.Kingdom))
		{
			return;
		}
		Kingdom kingdom = clan.Kingdom;
		if (_warFatigueUpdateQueue.ContainsKey(kingdom) && _warFatigueUpdateQueue[kingdom].Count > 0)
		{
			_warFatigueUpdateQueue[kingdom].Dequeue();
			UpdateWarFatigue(kingdom);
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.ShowWarFatigueDebug)
			{
				InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] Processed war fatigue update for {kingdom.Name}. Remaining in queue: {_warFatigueUpdateQueue[kingdom].Count}", Colors.Cyan));
			}
		}
		if (kingdom.Leader == Hero.MainHero)
		{
			return;
		}
		if (_lastDiplomaticAction.TryGetValue(kingdom, out var value))
		{
			CampaignTime now = CampaignTime.Now;
			if (now.ToDays - value.ToDays < 7.0)
			{
				return;
			}
		}
		bool flag = StrategicX4AIHelpers.IsSnowballingFaction(kingdom);
		bool flag2 = ((List<Kingdom>)(object)snowballingKingdoms).Contains(kingdom);
		if (flag && !flag2)
		{
			((List<Kingdom>)(object)snowballingKingdoms).Add(kingdom);
		}
		else if (!flag && flag2)
		{
			((List<Kingdom>)(object)snowballingKingdoms).Remove(kingdom);
		}
		_lastDiplomaticAction[kingdom] = CampaignTime.Now;
		if (!MakePeaceWithKingdoms(kingdom) && !CurrentWars(kingdom).Any() && !(GetWarFatigue(kingdom) > 0f))
		{
			MakeWarWithKingdoms(kingdom);
		}
	}

	public static void updateKingdomTargetsCache(Kingdom k)
	{
		if (!_targetCachers.ContainsKey(k))
		{
			_targetCachers[k] = new TargetCacher();
		}
		TargetCacher targetCacher = _targetCachers[k];
		((List<Kingdom>)(object)targetCacher.NearbyTargets).Clear();
		((List<Kingdom>)(object)targetCacher.Neighbors).Clear();
		HashSet<Kingdom> neighbors = StrategicX4AIHelpers.GetNeighbors(k);
		foreach (Kingdom item in neighbors)
		{
			((List<Kingdom>)(object)targetCacher.Neighbors).Add(item);
		}
		foreach (Kingdom item2 in (List<Kingdom>)(object)MajorKingdoms)
		{
			if (item2 != k && !((List<Kingdom>)(object)targetCacher.Neighbors).Contains(item2) && StrategicX4AIHelpers.AreKingdomsWithinReasonableAttackDistance(k, item2))
			{
				((List<Kingdom>)(object)targetCacher.NearbyTargets).Add(item2);
			}
		}
		targetCacher.needsUpdate = false;
	}

	private bool MakePeaceWithKingdoms(Kingdom k)
	{
		foreach (Kingdom item in CurrentWars(k))
		{
			float warFatigue = GetWarFatigue(k);
			float warFatigue2 = GetWarFatigue(item);
			string tag = "multi_war_peace";
			bool flag = ((List<Kingdom>)(object)snowballingKingdoms).Contains(k);
			bool flag2 = StrategicX4AIHelpers.GetKingdomStrength(item) > 1.5f * StrategicX4AIHelpers.GetKingdomStrength(k);
			bool flag3 = warFatigue > 0.7f;
			if (flag)
			{
				tag = "snowball_peace";
			}
			else if (flag2)
			{
				tag = "overwhelmed_peace";
			}
			else if (flag3)
			{
				tag = "war_fatigue_peace";
			}
			else if (CurrentWars(k).Count > 1)
			{
				tag = "multi_war_peace";
			}
			string story = StrategicX4AIHelpers.BuildNarrative(k, item, tag);
			if (AttemptPeace(k, item, story, warFatigue, warFatigue2))
			{
				return true;
			}
		}
		return false;
	}

	public void MakeWarWithKingdoms(Kingdom k)
	{
		Kingdom i = k;
		if (_cachedFeastBehavior == null)
		{
			Campaign current = Campaign.Current;
			_cachedFeastBehavior = ((current != null) ? current.GetCampaignBehavior<FeastBehavior>() : null);
		}
		bool flag = _cachedFeastBehavior != null;
		bool flag2 = flag && FeastBehavior.GetFeastByAttribute((FeastData f) => f.Kingdom == i) != null;
		if (!(flag && flag2))
		{
			(Kingdom, string) tuple = FindBestTargetForWar(i);
			if (tuple.Item1 != null)
			{
				DeclareWar(i, tuple.Item1, tuple.Item2);
			}
		}
	}

	public static (Kingdom target, string story) FindBestTargetForWar(Kingdom k)
	{
		MBList<Kingdom> validWarTargets = GetValidWarTargets(k);
		if (((List<Kingdom>)(object)validWarTargets).Count == 0)
		{
			return (target: null, story: null);
		}
		float kingdomStrength = StrategicX4AIHelpers.GetKingdomStrength(k);
		Dictionary<Kingdom, float> dictionary = ((IEnumerable<Kingdom>)validWarTargets).ToDictionary((Kingdom t) => t, (Kingdom t) => StrategicX4AIHelpers.GetKingdomStrength(t));
		MBList<Kingdom> val = new MBList<Kingdom>();
		foreach (Kingdom target in (List<Kingdom>)(object)validWarTargets)
		{
			bool flag = dictionary[target] <= 1.25f * kingdomStrength;
			bool flag2 = ((IEnumerable<Kingdom>)MajorKingdoms).Any((Kingdom other) => other != target && target.IsAtWarWith((IFaction)(object)other));
			if (flag || flag2)
			{
				((List<Kingdom>)(object)val).Add(target);
			}
		}
		if (((List<Kingdom>)(object)val).Count > 0)
		{
			Kingdom val2 = SelectBestExpansionTarget(k, val);
			if (val2 != null)
			{
				string item = (((List<Kingdom>)(object)snowballingKingdoms).Contains(val2) ? StrategicX4AIHelpers.BuildNarrative(k, val2, "prevent_snowball_war") : StrategicX4AIHelpers.BuildNarrative(k, val2, "expansion_war"));
				return (target: val2, story: item);
			}
		}
		return (target: null, story: null);
	}

	public static MBList<Kingdom> GetValidWarTargets(Kingdom kingdom)
	{
		Kingdom kingdom2 = kingdom;
		Kingdom obj = kingdom2;
		if (((obj != null) ? obj.Leader : null) == null)
		{
			return new MBList<Kingdom>();
		}
		if (!_targetCachers.TryGetValue(kingdom2, out TargetCacher value) || value.needsUpdate)
		{
			updateKingdomTargetsCache(kingdom2);
			value = _targetCachers[kingdom2];
		}
		IEnumerable<Kingdom> source = (IEnumerable<Kingdom>)((((List<Kingdom>)(object)value.Neighbors).Count > 0) ? value.Neighbors : value.NearbyTargets);
		float inputStrength = StrategicX4AIHelpers.GetKingdomStrength(kingdom2);
		Dictionary<Kingdom, float> targetStrengths = source.ToDictionary((Kingdom x) => x, (Kingdom x) => StrategicX4AIHelpers.GetKingdomStrength(x));
		List<DiplomaticAgreementInfo> list = null;
		if (DiplomacySettingsCache.IsDiplomacyModLoaded)
		{
			DiplomacySettingsCache.EnsureInitialized();
			list = DiplomacyPlugin.GetAllDiplomaticAgreements();
		}
		HashSet<Kingdom> kingdomAgreements = new HashSet<Kingdom>();
		if (list != null)
		{
			foreach (DiplomaticAgreementInfo item in list)
			{
				if (item.Kingdom1 == kingdom2)
				{
					kingdomAgreements.Add(item.Kingdom2);
				}
				else if (item.Kingdom2 == kingdom2)
				{
					kingdomAgreements.Add(item.Kingdom1);
				}
			}
		}
		if (DiplomacySettingsCache.IsDiplomacyModLoaded)
		{
			return Extensions.ToMBList<Kingdom>(source.Where((Kingdom x) => (!((double)targetStrengths[x] * 0.8 >= (double)inputStrength) || ((IEnumerable<Kingdom>)MajorKingdoms).Any((Kingdom other) => other != x && x.IsAtWarWith((IFaction)(object)other))) && (!kingdomAgreements.Contains(x) || FactionManager.IsAtWarAgainstFaction((IFaction)(object)x, (IFaction)(object)kingdom2)) && (!DiplomacySettingsCache.NoWarBetweenFriends.GetValueOrDefault() || kingdom2.Leader == null || x.Leader == null || !kingdom2.Leader.IsFriend(x.Leader)) && (!DiplomacySettingsCache.NoWarOnGoodRelations.GetValueOrDefault() || !DiplomacySettingsCache.NoWarOnGoodRelationsThreshold.HasValue || kingdom2.Leader == null || x.Leader == null || kingdom2.Leader.GetRelation(x.Leader) < DiplomacySettingsCache.NoWarOnGoodRelationsThreshold.Value) && (!DiplomacySettingsCache.NoWarWhenMarriedLeaderClans.GetValueOrDefault() || kingdom2.RulingClan == null || x.RulingClan == null || !DiplomacyPlugin.HasMarriedClanLeaderRelation(kingdom2.RulingClan, x.RulingClan))));
		}
		return Extensions.ToMBList<Kingdom>(source.Where((Kingdom x) => !((double)targetStrengths[x] * 0.8 >= (double)inputStrength) || ((IEnumerable<Kingdom>)MajorKingdoms).Any((Kingdom other) => other != x && x.IsAtWarWith((IFaction)(object)other))));
	}

	public static Kingdom SelectBestExpansionTarget(Kingdom kingdom, MBList<Kingdom> potentialTargets)
	{
		Kingdom kingdom2 = kingdom;
		if (potentialTargets == null || ((List<Kingdom>)(object)potentialTargets).Count == 0 || kingdom2 == null)
		{
			return null;
		}
		float kingdomStrength = StrategicX4AIHelpers.GetKingdomStrength(kingdom2);
		var list = ((IEnumerable<Kingdom>)potentialTargets).Where((Kingdom t) => t != null && t != kingdom2).Select(delegate(Kingdom t)
		{
			Kingdom target = t;
			float kingdomStrength2 = StrategicX4AIHelpers.GetKingdomStrength(t);
			int richness = ((IEnumerable<Settlement>)t.Settlements).Count((Settlement s) => s.IsTown || s.IsCastle);
			int warsCount = ((IEnumerable<Kingdom>)MajorKingdoms).Count((Kingdom k) => k != t && t.IsAtWarWith((IFaction)(object)k));
			Hero leader = kingdom2.Leader;
			int relation;
			if (((leader != null) ? leader.Clan : null) != null)
			{
				Hero leader2 = t.Leader;
				if (((leader2 != null) ? leader2.Clan : null) != null)
				{
					relation = kingdom2.Leader.Clan.GetRelationWithClan(t.Leader.Clan);
					goto IL_00bd;
				}
			}
			relation = 0;
			goto IL_00bd;
			IL_00bd:
			return new
			{
				Target = target,
				Strength = kingdomStrength2,
				Richness = richness,
				WarsCount = warsCount,
				Relation = relation,
				IsSnowballing = ((List<Kingdom>)(object)snowballingKingdoms).Contains(t)
			};
		}).ToList();
		if (list.Count == 0)
		{
			return null;
		}
		float maxRichness = list.Max(x => x.Richness);
		float num = list.Max(x => x.WarsCount);
		var source = Extensions.ToMBList(list.Select(t =>
		{
			int num2 = 0;
			if (kingdomStrength > t.Strength * 1.2f)
			{
				num2++;
			}
			if (maxRichness > 0f && (float)t.Richness >= maxRichness * 0.7f)
			{
				num2++;
			}
			if (t.WarsCount > 0)
			{
				num2++;
			}
			if (t.Relation < 0)
			{
				num2++;
			}
			if (t.IsSnowballing)
			{
				num2 += 2;
			}
			return new
			{
				Target = t.Target,
				Score = num2,
				Strength = t.Strength,
				Richness = t.Richness,
				WarsCount = t.WarsCount,
				Relation = t.Relation,
				IsSnowballing = t.IsSnowballing
			};
		}));
		int bestScore = source.Max(x => x.Score);
		var list2 = source.Where(x => x.Score == bestScore).ToList();
		if (list2.Count == 1)
		{
			return list2[0].Target;
		}
		return (from x in list2
			orderby x.IsSnowballing descending, x.WarsCount descending, x.Strength, x.Richness descending, x.Relation, ((MBObjectBase)x.Target).StringId
			select x).FirstOrDefault()?.Target;
	}

	private bool AttemptPeace(Kingdom kingdom, Kingdom target, string story, float ourDesire, float theirDesire)
	{
		if (kingdom == null || target == null || kingdom.RulingClan == null || kingdom.RulingClan.Leader == null || target.RulingClan == null || target.RulingClan.Leader == null)
		{
			return false;
		}
		Clan clan = Hero.MainHero.Clan;
		Kingdom val = ((clan != null) ? clan.Kingdom : null);
		bool flag = val != null && val == target;
		if (flag && target.Leader == Hero.MainHero && ourDesire == 1f)
		{
			HandlePlayerKingdomPeace(kingdom, target, story);
			return false;
		}
		Kingdom val2;
		Kingdom val3;
		if (flag)
		{
			val2 = target;
			val3 = kingdom;
		}
		else
		{
			val2 = kingdom;
			val3 = target;
		}
		if (ourDesire != 1f || theirDesire != 1f)
		{
			return false;
		}
		MakePeaceKingdomDecision val4;
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableBaseGameTributes)
		{
			int valueForFaction = ((Barterable)new PeaceBarterable(val2.RulingClan.Leader, (IFaction)(object)val2, (IFaction)(object)val3, CampaignTime.Years(1f))).GetValueForFaction((IFaction)(object)val3);
			int dailyTributePaid = 0;
			val4 = new MakePeaceKingdomDecision(val2.RulingClan, (IFaction)(object)val3, dailyTributePaid, 0, true);
		}
		else
		{
			val4 = new MakePeaceKingdomDecision(val2.RulingClan, (IFaction)(object)val3, 0, 0, true);
		}
		if (val4 == null)
		{
			return false;
		}
		bool flag2 = UniversalDecisionHandler.HandleAddingDecision((KingdomDecision)(object)val4, val3, val2);
		if (flag2)
		{
			InformationManager.DisplayMessage(new InformationMessage(story, Colors.Green));
		}
		return flag2;
	}

	private void DeclareWar(Kingdom kingdom, Kingdom target, string story)
	{
		if (!kingdom.IsAtWarWith((IFaction)(object)target))
		{
			DeclareWarDecision val = new DeclareWarDecision(kingdom.RulingClan, (IFaction)(object)target);
			if (val != null && UniversalDecisionHandler.HandleAddingDecision((KingdomDecision)(object)val, target, kingdom))
			{
				InformationManager.DisplayMessage(new InformationMessage(story, Colors.Red));
			}
		}
	}

	private void UpdateWarFatigue(Kingdom k)
	{
		bool showWarFatigueDebug = GlobalSettings<WarAndAiTweaksSettings>.Instance.ShowWarFatigueDebug;
		if (showWarFatigueDebug)
		{
			InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] UpdateWarFatigue called for kingdom: {((k != null) ? k.Name : null)}", Colors.Magenta));
		}
		if (!_warFatigue.ContainsKey(k))
		{
			if (showWarFatigueDebug)
			{
				InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] No war fatigue entry for {k.Name}, initializing to 0.", Colors.Magenta));
			}
			_warFatigue[k] = 0f;
		}
		float num = 1f / (float)GlobalSettings<WarAndAiTweaksSettings>.Instance.WarFatigueDays;
		float num2 = 1f / (float)GlobalSettings<WarAndAiTweaksSettings>.Instance.PeaceFatigueDays;
		if (showWarFatigueDebug)
		{
			InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] BUILD_STEP: {num}, DECAY_STEP: {num2}", Colors.Magenta));
		}
		float num3 = (_warFatigue.TryGetValue(k, out var value) ? value : 0f);
		if (showWarFatigueDebug)
		{
			InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] Current war fatigue for {k.Name}: {num3}", Colors.Magenta));
		}
		bool flag = CurrentWars(k).Any();
		if (showWarFatigueDebug)
		{
			InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] {k.Name} is at war: {flag}", Colors.Magenta));
		}
		if (!flag)
		{
			if (num3 <= 0f)
			{
				if (showWarFatigueDebug)
				{
					InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] {k.Name} is at peace and fatigue is already at minimum ({num3}). Early exit.", Colors.Magenta));
				}
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("war_fatigue_update", "{KINGDOM} War Fatigue updated to {FATIGUE:P1}").SetTextVariable("KINGDOM", k.Name).SetTextVariable("FATIGUE", num3)).ToString(), Colors.Yellow));
				return;
			}
			float num4 = MathF.Max(0f, num3 - num2);
			if (showWarFatigueDebug)
			{
				InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] {k.Name} is at peace. Decaying fatigue from {num3} to {num4}.", Colors.Magenta));
			}
			num3 = num4;
			_warFatigue[k] = num3;
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("war_fatigue_update", "{KINGDOM} War Fatigue updated to {FATIGUE:P1}").SetTextVariable("KINGDOM", k.Name).SetTextVariable("FATIGUE", num3)).ToString(), Colors.Yellow));
			return;
		}
		if (num3 >= 1f)
		{
			if (showWarFatigueDebug)
			{
				InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] {k.Name} is at war and fatigue is already at maximum ({num3}). Early exit.", Colors.Magenta));
			}
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("war_fatigue_update", "{KINGDOM} War Fatigue updated to {FATIGUE:P1}").SetTextVariable("KINGDOM", k.Name).SetTextVariable("FATIGUE", num3)).ToString(), Colors.Yellow));
			return;
		}
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		List<StanceLink> list = new List<StanceLink>();
		foreach (Kingdom otherK in (List<Kingdom>)(object)Kingdom.All)
		{
			if (otherK != k)
			{
				StanceLink stanceWith = k.GetStanceWith((IFaction)(object)otherK);
				if (stanceWith != null && stanceWith.IsAtWar)
				{
					list.Add(stanceWith);
				}
			}
		}
		foreach (StanceLink item in list)
		{
			IFaction faction = item.Faction2;
			Kingdom val = (Kingdom)(object)((faction is Kingdom) ? faction : null);
			if (val == null || val == k)
			{
				IFaction faction2 = item.Faction1;
				val = (Kingdom)(object)((faction2 is Kingdom) ? faction2 : null);
			}
			if (val != null && val != k)
			{
				int count = 0;
				int count2 = 0;
				float num9 = item.GetCasualties((IFaction)(object)k);
				int count3 = DiplomacyHelper.GetPrisonersOfWarTakenByFaction((IFaction)(object)val, (IFaction)(object)k).Count;
				if (showWarFatigueDebug)
				{
					InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] Stance with {val.Name}: Raids={count}, Sieges={count2}, Casualties={num9}, Prisoners={count3}", Colors.Magenta));
				}
				num5 += (float)count;
				num8 += (float)count2;
				num6 += num9;
				num7 += (float)count3;
			}
		}
		if (showWarFatigueDebug)
		{
			InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] Total events for {k.Name}: Raids={num5}, Sieges={num8}, Casualties={num6}, Prisoners={num7}", Colors.Magenta));
		}
		float num10 = num5 * 0.0005f + num6 * 2E-06f + num7 * 0.0005f + num8 * 0.005f;
		if (showWarFatigueDebug)
		{
			InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] Event-driven increase for {k.Name}: {num10}", Colors.Magenta));
		}
		float num11 = MathF.Min(1f, num3 + num + num10);
		if (showWarFatigueDebug)
		{
			InformationManager.DisplayMessage(new InformationMessage($"[DEBUG] {k.Name} war fatigue updated from {num3} to {num11} (BUILD_STEP={num}, eventIncrease={num10})", Colors.Magenta));
		}
		_warFatigue[k] = num11;
		InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("war_fatigue_update", "{KINGDOM} War Fatigue updated to {FATIGUE:P1}").SetTextVariable("KINGDOM", k.Name).SetTextVariable("FATIGUE", num11)).ToString(), Colors.Yellow));
	}

	private static void HandlePlayerKingdomPeace(Kingdom aiKingdom, Kingdom playerKingdom, string reason)
	{
		Kingdom aiKingdom2 = aiKingdom;
		Kingdom playerKingdom2 = playerKingdom;
		if (_lastPlayerPeaceOffer.TryGetValue(aiKingdom2, out var value))
		{
			CampaignTime now = CampaignTime.Now;
			if (now.ToDays - value.ToDays < 20.0)
			{
				return;
			}
		}
		_lastPlayerPeaceOffer[aiKingdom2] = CampaignTime.Now;
		InformationManager.ShowInquiry(new InquiryData(((object)LanguageTranslater.L.T("diplomacy_peace_offer_title", "{AI} Seeks Peace").SetTextVariable("AI", aiKingdom2.Name)).ToString(), ((object)LanguageTranslater.L.T("diplomacy_peace_offer_body", "{AI} wishes to negotiate peace with {PLAYER}. {REASON} Do you accept?").SetTextVariable("AI", aiKingdom2.Name).SetTextVariable("PLAYER", playerKingdom2.Name)
			.SetTextVariable("REASON", reason)).ToString(), true, true, LanguageTranslater.L.S("diplomacy_accept", "Accept"), LanguageTranslater.L.S("diplomacy_decline", "Decline"), (Action)delegate
		{
			MakePeaceKingdomDecision decision;
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableBaseGameTributes)
			{
				int valueForFaction = ((Barterable)new PeaceBarterable(aiKingdom2.RulingClan.Leader, (IFaction)(object)aiKingdom2, (IFaction)(object)playerKingdom2, CampaignTime.Years(1f))).GetValueForFaction((IFaction)(object)playerKingdom2);
				int dailyTributePaid = 0;
				decision = new MakePeaceKingdomDecision(playerKingdom2.RulingClan, (IFaction)(object)aiKingdom2, dailyTributePaid, 0, true);
			}
			else
			{
				decision = new MakePeaceKingdomDecision(playerKingdom2.RulingClan, (IFaction)(object)aiKingdom2, 0, 0, true);
			}
			UniversalDecisionHandler.HandleAddingDecision((KingdomDecision)(object)decision, aiKingdom2, playerKingdom2);
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("diplomacy_peace_proposal_sent", "Peace proposal between {PLAYER} and {AI} has been sent to your council.").SetTextVariable("PLAYER", playerKingdom2.Name).SetTextVariable("AI", aiKingdom2.Name)).ToString(), Colors.Green));
		}, (Action)delegate
		{
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("diplomacy_peace_declined", "{PLAYER} declined peace with {AI}.").SetTextVariable("PLAYER", playerKingdom2.Name).SetTextVariable("AI", aiKingdom2.Name)).ToString(), Colors.Red));
		}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
	}
}


