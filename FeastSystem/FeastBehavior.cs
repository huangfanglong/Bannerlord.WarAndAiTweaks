using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using WarAndAITweaks.TodayWeFeast;
using WarAndAiTweaks;

namespace FeastSystem;

public class FeastBehavior : CampaignBehaviorBase
{
	[HarmonyPatch(typeof(AiPartyThinkBehavior), "PartyHourlyAiTick")]
	internal static class Patch_PartyHourlyAiTick_Feast
	{
		public static bool Prefix(MobileParty mobileParty)
		{
			if (mobileParty == null || !mobileParty.IsLordParty || mobileParty.LeaderHero == null || mobileParty.LeaderHero == Hero.MainHero)
			{
				return true;
			}
			Hero partyHero = mobileParty.LeaderHero;
			FeastData feastByAttribute = GetFeastByAttribute((FeastData f) => f != null && f.Attendees != null && (((List<Hero>)(object)f.Attendees).Contains(partyHero) || f.Host == partyHero));
			if (feastByAttribute == null || feastByAttribute.Settlement == null)
			{
				return true;
			}
			if (mobileParty.GetNumDaysForFoodToLast() < 2)
			{
				return true;
			}
			if (feastByAttribute.Settlement == null)
			{
				return true;
			}
			try
			{
				SetPartyAiAction.GetActionForVisitingSettlement(mobileParty, feastByAttribute.Settlement);
			}
			catch (Exception)
			{
				return true;
			}
			return false;
		}
	}

	[SaveableField(1)]
	public static MBList<FeastData> _activeFeasts = new MBList<FeastData>();

	[SaveableField(2)]
	public static Dictionary<Kingdom, CampaignTime> _lastFeastByFaction = new Dictionary<Kingdom, CampaignTime>();

	[SaveableField(3)]
	public static Dictionary<FeastData, RewardData> _lastFeastRewardTimes = new Dictionary<FeastData, RewardData>();

	[SaveableField(5)]
	public static Dictionary<Kingdom, CampaignTime> _lastFeastProcessByKingdom = new Dictionary<Kingdom, CampaignTime>();

	public static FeastData GetFeastByAttribute(Func<FeastData, bool> predicate)
	{
		return ((IEnumerable<FeastData>)_activeFeasts).FirstOrDefault<FeastData>(predicate);
	}

	public static void RemoveFeast(FeastData feast)
	{
		((List<FeastData>)(object)_activeFeasts).Remove(feast);
	}

	public static void AddFeast(FeastData feast)
	{
		((List<FeastData>)(object)_activeFeasts).Add(feast);
	}

	public static HashSet<Kingdom> CurrentWars(Kingdom sourceKingdom)
	{
		Kingdom sourceKingdom2 = sourceKingdom;
		return ((IEnumerable<Kingdom>)Kingdom.All).Where((Kingdom x) => isValidMilitaryTargetForKingdom(sourceKingdom2, x)).ToHashSet();
	}

	public static bool isValidMilitaryTargetForKingdom(Kingdom sourceKingdom, Kingdom targetKingdom)
	{
		return !targetKingdom.IsEliminated && ((IEnumerable<Settlement>)targetKingdom.Settlements).Any() && sourceKingdom.IsAtWarWith((IFaction)(object)targetKingdom);
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
		CampaignEvents.WarDeclared.AddNonSerializedListener((object)this, (Action<IFaction, IFaction, DeclareWarDetail>)OnWarDeclared);
		CampaignEvents.DailyTickClanEvent.AddNonSerializedListener((object)this, (Action<Clan>)OnDailyTickClan);
		CampaignEvents.SettlementEntered.AddNonSerializedListener((object)this, (Action<MobileParty, Settlement, Hero>)OnSettlementEntered);
	}

	public override void SyncData(IDataStore dataStore)
	{
		try
		{
			dataStore.SyncData<MBList<FeastData>>("_activeFeasts", ref _activeFeasts);
		}
		catch (Exception)
		{
			_activeFeasts = new MBList<FeastData>();
		}
		try
		{
			dataStore.SyncData<Dictionary<Kingdom, CampaignTime>>("_lastFeastByFaction", ref _lastFeastByFaction);
		}
		catch (Exception)
		{
			_lastFeastByFaction = new Dictionary<Kingdom, CampaignTime>();
		}
		try
		{
			dataStore.SyncData<Dictionary<FeastData, RewardData>>("_lastFeastRewardTimes", ref _lastFeastRewardTimes);
		}
		catch (Exception)
		{
			_lastFeastRewardTimes = new Dictionary<FeastData, RewardData>();
		}
		try
		{
			dataStore.SyncData<Dictionary<Kingdom, CampaignTime>>("_lastFeastProcessByKingdom", ref _lastFeastProcessByKingdom);
		}
		catch (Exception)
		{
			_lastFeastProcessByKingdom = new Dictionary<Kingdom, CampaignTime>();
		}
	}

	private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
	{
		FeastHelper.AddMenuOptions(campaignGameStarter);
		FeastQuest.RegisterFeastQuestDialogs(campaignGameStarter);
	}

	private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarDetail detail)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		foreach (FeastData item in (List<FeastData>)(object)Extensions.ToMBList<FeastData>((List<FeastData>)(object)_activeFeasts))
		{
			Hero host = item.Host;
			object obj;
			if (host == null)
			{
				obj = null;
			}
			else
			{
				Clan clan = host.Clan;
				if (clan == null)
				{
					obj = null;
				}
				else
				{
					Kingdom kingdom = clan.Kingdom;
					obj = ((kingdom != null) ? kingdom.MapFaction : null);
				}
			}
			IFaction val = (IFaction)obj;
			if (val != null && (val == faction1 || val == faction2))
			{
				TextObject val2 = new TextObject("{=feast_ended_by_war}The feast hosted by {HOST} has ended due to war between {FACTION1} and {FACTION2}.", (Dictionary<string, object>)null);
				Hero host2 = item.Host;
				val2.SetTextVariable("HOST", ((host2 == null) ? null : ((object)host2.Name)?.ToString()) ?? "Unknown");
				val2.SetTextVariable("FACTION1", ((faction1 == null) ? null : ((object)faction1.Name)?.ToString()) ?? "Unknown");
				val2.SetTextVariable("FACTION2", ((faction2 == null) ? null : ((object)faction2.Name)?.ToString()) ?? "Unknown");
				EndFeast(item, ((object)val2).ToString());
			}
		}
	}

	private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
	{
		Settlement settlement2 = settlement;
		FeastData feastByAttribute = GetFeastByAttribute((FeastData f) => f.Settlement == settlement2);
		if (feastByAttribute != null && !feastByAttribute.HostHasJoinedForFirstTime && feastByAttribute.Host == hero)
		{
			feastByAttribute.HostHasJoinedForFirstTime = true;
		}
	}

	private void OnDailyTickClan(Clan clan)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Kingdom clanKingdom = ((clan != null) ? clan.Kingdom : null);
		if (clan == null || clanKingdom == null || clan.IsUnderMercenaryService)
		{
			return;
		}
		if (_lastFeastProcessByKingdom.TryGetValue(clanKingdom, out var value))
		{
			CampaignTime now = CampaignTime.Now;
			if (((CampaignTime)(ref now)).ToDays - ((CampaignTime)(ref value)).ToDays < 1.0)
			{
				return;
			}
		}
		_lastFeastProcessByKingdom[clanKingdom] = CampaignTime.Now;
		FeastData feastByAttribute = GetFeastByAttribute((FeastData f) => f.Kingdom == clanKingdom);
		if (feastByAttribute != null)
		{
			Hero host = feastByAttribute.Host;
			object obj;
			if (host == null)
			{
				obj = null;
			}
			else
			{
				Clan clan2 = host.Clan;
				obj = ((clan2 != null) ? clan2.Kingdom : null);
			}
			Kingdom val = (Kingdom)obj;
			Kingdom kingdom = feastByAttribute.Kingdom;
			if (feastByAttribute.Settlement == null || val == null || kingdom == null || val != kingdom || kingdom.IsEliminated || CurrentWars(clanKingdom).Any())
			{
				FeastHelper.RemoveInvalidFeast(feastByAttribute);
				return;
			}
			TickFeast(feastByAttribute);
		}
		if (!FeastHelper.FactionEligbleForFeast(clanKingdom))
		{
			return;
		}
		Hero leader = clan.Leader;
		if (leader != null && leader != Hero.MainHero && FeastHelper.CanHostFeast(clan.Leader) && MBRandom.RandomFloatRanged(0f, 1f) <= GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastAIChance)
		{
			Settlement hostSettlement = FeastHelper.GetHostSettlement(leader);
			if (hostSettlement != null)
			{
				StartFeast(leader, hostSettlement);
			}
		}
	}

	private void TickFeast(FeastData feast)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (feast == null)
		{
			return;
		}
		if (feast.FeastRoster != null && feast.Attendees != null)
		{
			_ = feast.StartTime;
			if (feast.Settlement != null && feast.Host != null)
			{
				FeastHelper.HandlePlayerAttendanceFailure(feast);
				FeastHelper.HandleConsumeFoodAtFeast(feast);
				HandleAIFeastDecsisionsLogic(feast);
				HandleAIContributeFoodToFeast(feast);
				HandleBackgroundAIInteractions(feast);
				return;
			}
		}
		FeastHelper.RemoveInvalidFeast(feast);
	}

	public static void StartFeast(Hero host, Settlement settlement)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		FeastData feastData = new FeastData
		{
			Host = host,
			StartTime = CampaignTime.Now,
			Settlement = settlement,
			Attendees = new MBList<Hero>(),
			FeastRoster = new ItemRoster(),
			HostHasJoinedForFirstTime = false,
			Kingdom = host.Clan.Kingdom
		};
		MobileParty partyBelongedTo = host.PartyBelongedTo;
		if (partyBelongedTo != null && partyBelongedTo.CurrentSettlement != null && partyBelongedTo.CurrentSettlement == feastData.Settlement)
		{
			feastData.HostHasJoinedForFirstTime = true;
		}
		FeastHelper.initializeFeastFood(feastData);
		AddFeast(feastData);
		_lastFeastByFaction[host.Clan.Kingdom] = CampaignTime.Now;
		if (host == Hero.MainHero)
		{
			string[] obj = new string[6]
			{
				"feast_host_",
				((MBObjectBase)host).StringId,
				"_",
				((MBObjectBase)settlement).StringId,
				"_",
				null
			};
			CampaignTime now = CampaignTime.Now;
			obj[5] = ((object)(CampaignTime)(ref now)).ToString();
			string questId = string.Concat(obj);
			new FeastQuest(questId, host, host, settlement, CampaignTime.Now, isPlayerHost: true);
		}
		FeastHelper.SendOutInvitationToFeast(feastData);
	}

	public static void EndFeast(FeastData feast, string reason)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		Clan clan = Hero.MainHero.Clan;
		Kingdom obj = ((clan != null) ? clan.Kingdom : null);
		Hero host = feast.Host;
		object obj2;
		if (host == null)
		{
			obj2 = null;
		}
		else
		{
			Clan clan2 = host.Clan;
			obj2 = ((clan2 != null) ? clan2.Kingdom : null);
		}
		if (obj == obj2)
		{
			MBInformationManager.AddQuickInformation(new TextObject(reason, (Dictionary<string, object>)null), 0, (BasicCharacterObject)null, "");
		}
		Hero host2 = feast.Host;
		if (((host2 != null) ? host2.MapFaction : null) != null && feast.Kingdom != null)
		{
			_lastFeastByFaction[feast.Kingdom] = CampaignTime.Now;
		}
		_lastFeastRewardTimes.Remove(feast);
		RemoveFeast(feast);
		((List<Hero>)(object)feast.Attendees)?.Clear();
		feast.FeastRoster = null;
		feast.Host = null;
		feast.Settlement = null;
		feast.StartTime = default(CampaignTime);
		feast.Kingdom = null;
		feast.PlayerInvitationAcceptedTime = default(CampaignTime);
		feast.PlayerHasJoinedForFirstTime = false;
		feast.HostHasJoinedForFirstTime = false;
		InformationManager.DisplayMessage(new InformationMessage(reason, Colors.Cyan));
	}

	public void HandleBackgroundAIInteractions(FeastData feast)
	{
		if (feast == null || feast.Attendees == null || ((List<Hero>)(object)feast.Attendees).Count == 0)
		{
			return;
		}
		Hero host = feast.Host;
		if (host == null || host == Hero.MainHero)
		{
			return;
		}
		foreach (Hero item in (List<Hero>)(object)feast.Attendees)
		{
			if (item != null && item != host && item != Hero.MainHero && item.CurrentSettlement == feast.Settlement && !FeastQuest.HasRewardCooldown(host, item))
			{
				FeastQuest.GiveRewardWithCooldownRespect(host, item);
			}
		}
	}

	public static void HandleAIContributeFoodToFeast(FeastData feast)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		Hero val = feast?.Host;
		if (val == null || val == Hero.MainHero || feast == null || feast.Settlement == null)
		{
			return;
		}
		MobileParty partyBelongedTo = val.PartyBelongedTo;
		Settlement val2 = ((partyBelongedTo != null) ? partyBelongedTo.CurrentSettlement : null);
		if (val2 == null || val2 != feast.Settlement)
		{
			return;
		}
		MobileParty partyBelongedTo2 = val.PartyBelongedTo;
		ItemRoster val3 = ((partyBelongedTo2 != null) ? partyBelongedTo2.ItemRoster : null);
		if (val3 == null)
		{
			return;
		}
		List<ItemRosterElement> list = ((IEnumerable<ItemRosterElement>)val3).Where(delegate(ItemRosterElement e)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			EquipmentElement equipmentElement = ((ItemRosterElement)(ref e)).EquipmentElement;
			ItemObject item = ((EquipmentElement)(ref equipmentElement)).Item;
			return item != null && item.IsFood && ((ItemRosterElement)(ref e)).Amount > 0;
		}).ToList();
		foreach (ItemRosterElement item2 in list)
		{
			ItemRosterElement current = item2;
			int num = (int)Math.Floor((double)((ItemRosterElement)(ref current)).Amount * 0.8);
			if (num >= 1)
			{
				val3.AddToCounts(((ItemRosterElement)(ref current)).EquipmentElement, -num);
				feast.FeastRoster.AddToCounts(((ItemRosterElement)(ref current)).EquipmentElement, num);
			}
		}
	}

	public static void HandleAIFeastDecsisionsLogic(FeastData feast)
	{
		FeastData feast2 = feast;
		List<Hero> list = ((IEnumerable<Hero>)feast2.Attendees).Where((Hero x) => x != feast2.Host && x != Hero.MainHero).ToList();
		if (!list.Any())
		{
			return;
		}
		foreach (Hero item in list)
		{
			FeastHelper.MaybeRemoveAILordFromFeast(feast2, item);
			FeastHelper.MaybeEndAIFeast(feast2);
		}
	}
}
