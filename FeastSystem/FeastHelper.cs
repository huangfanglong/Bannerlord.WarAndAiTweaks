using System;
using System.Collections.Generic;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;
using WarAndAITweaks.TodayWeFeast;
using WarAndAiTweaks;

namespace FeastSystem;

public static class FeastHelper
{
	public static void AddMenuOptions(CampaignGameStarter starter)
	{
		starter.AddGameMenuOption("town_keep", "manage_feast", LanguageTranslater.L.S("manage_feast_inventory", "Manage feast inventory"), (MenuCallbackArgs args) => CanManageFeast(args), (MenuCallbackArgs args) => ManageFeast(args), false, 5, false, (object)null);
		starter.AddGameMenuOption("castle", "manage_feast", LanguageTranslater.L.S("manage_feast_inventory", "Manage feast inventory"), (MenuCallbackArgs args) => CanManageFeast(args), (MenuCallbackArgs args) => ManageFeast(args), false, 5, false, (object)null);
	}

	public static bool CanHostFeast(Hero hero)
	{
		Clan val = ((hero != null) ? hero.Clan : null);
		MobileParty val2 = ((hero != null) ? hero.PartyBelongedTo : null);
		if (val == null || hero != val.Leader || val2 == null)
		{
			return false;
		}
		if (val == Hero.MainHero.Clan && hero != Hero.MainHero)
		{
			return false;
		}
		Hero spouse = hero.Spouse;
		int gold = hero.Gold;
		bool flag = ((IEnumerable<Settlement>)val.Settlements).Any((Settlement s) => s.IsTown || s.IsCastle);
		if (spouse == null || gold < 5000 || !flag)
		{
			return false;
		}
		return true;
	}

	public static void RemoveInvalidFeast(FeastData feast)
	{
		if (feast == null)
		{
			InformationManager.DisplayMessage(new InformationMessage("Tried to remove a null feast.", Colors.Red));
			return;
		}
		FeastBehavior._lastFeastRewardTimes.Remove(feast);
		FeastBehavior.RemoveFeast(feast);
		((List<Hero>)(object)feast.Attendees)?.Clear();
		feast.FeastRoster = null;
		feast.Host = null;
		feast.Settlement = null;
		feast.StartTime = default(CampaignTime);
		feast.Kingdom = null;
		feast.PlayerInvitationAcceptedTime = default(CampaignTime);
		feast.PlayerHasJoinedForFirstTime = false;
		feast.HostHasJoinedForFirstTime = false;
	}

	public static bool FactionEligbleForFeast(Kingdom k)
	{
		if (FeastBehavior.CurrentWars(k).Any())
		{
			return false;
		}
		if (FeastBehavior._lastFeastByFaction.TryGetValue(k, out var value))
		{
			CampaignTime now = CampaignTime.Now;
			if (now.ToDays - value.ToDays < (double)GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastCooldown)
			{
				return false;
			}
		}
		return true;
	}

	public static Settlement GetHostSettlement(Hero hero)
	{
		Hero hero2 = hero;
		return ((IEnumerable<Settlement>)hero2.Clan.Settlements).OrderBy((Settlement x) => x.GetValue(hero2, true)).FirstOrDefault((Settlement s) => s.IsTown || s.IsCastle);
	}

	public static void initializeFeastFood(FeastData feast)
	{
		List<ItemObject> list = ((IEnumerable<ItemObject>)MBObjectManager.Instance.GetObjectTypeList<ItemObject>()).Where((ItemObject item) => item.IsFood && item.IsTradeGood && item.Value > 0).ToList();
		for (int i = 0; i < 50; i++)
		{
			if (list.Count == 0)
			{
				break;
			}
			ItemObject val = list[MBRandom.RandomInt(list.Count)];
			feast.FeastRoster.AddToCounts(val, 1);
		}
	}

	public static void SendOutInvitationToFeast(FeastData feast)
	{
		Kingdom kingdom = feast.Host.Clan.Kingdom;
		Clan clan = feast.Host.Clan;
		if (kingdom == null)
		{
			return;
		}
		foreach (Clan item in (List<Clan>)(object)kingdom.Clans)
		{
			if (clan != item && item != feast.Host.Clan && !item.IsUnderMercenaryService && item.Leader != null && item.Leader.PartyBelongedTo != null)
			{
				if (item == Hero.MainHero.Clan)
				{
					ShowPlayerInvitation(feast);
				}
				else
				{
					HandleAIAcceptFeastInvite(feast, item);
				}
			}
		}
	}

	public static void ShowPlayerInvitation(FeastData feast)
	{
		FeastData feast2 = feast;
		if (feast2 == null || feast2.Host == null || feast2.Settlement == null)
		{
			return;
		}
		string hostName = ((object)feast2.Host.Name).ToString();
		string text = ((object)feast2.Host.Clan.Name).ToString();
		string settlementName = ((object)feast2.Settlement.Name).ToString();
		InquiryData val = new InquiryData(LanguageTranslater.L.S("feast_invitation_title", "Feast Invitation"), ((object)LanguageTranslater.L.T("feast_invitation_body", "You have been invited to a feast hosted by {HOST} from clan {CLAN} at {SETTLEMENT}. Do you wish to attend?").SetTextVariable("HOST", hostName).SetTextVariable("CLAN", text)
			.SetTextVariable("SETTLEMENT", settlementName)).ToString(), true, true, LanguageTranslater.L.S("feast_invitation_accept", "Accept"), LanguageTranslater.L.S("feast_invitation_decline", "Decline"), (Action)delegate
		{
			if (!((List<Hero>)(object)feast2.Attendees).Contains(Hero.MainHero))
			{
				((List<Hero>)(object)feast2.Attendees).Add(Hero.MainHero);
			}
			feast2.PlayerInvitationAcceptedTime = CampaignTime.Now;
			string stringId = ((MBObjectBase)feast2.Host).StringId;
			CampaignTime startTime = feast2.StartTime;
			string questId = "feast_quest_" + stringId + "_" + startTime.ToDays;
			new FeastQuest(questId, feast2.Host, feast2.Host, feast2.Settlement, feast2.StartTime);
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_invitation_accepted", "You have accepted the invitation to {HOST}'s feast at {SETTLEMENT}.").SetTextVariable("HOST", hostName).SetTextVariable("SETTLEMENT", settlementName)).ToString(), Colors.Green));
		}, (Action)delegate
		{
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_invitation_declined", "You have declined the invitation to {HOST}'s feast at {SETTLEMENT}.").SetTextVariable("HOST", hostName).SetTextVariable("SETTLEMENT", settlementName)).ToString(), Colors.Red));
			if (MBRandom.RandomFloatRanged(0f, 1f) <= GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastAIUpsetChance)
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_invitation_declined_disappointed", "{HOST} is disappointed that you will not be attending their feast.").SetTextVariable("HOST", hostName)).ToString(), Colors.Yellow));
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(feast2.Host, Hero.MainHero, -1, true);
			}
		}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null);
		InformationManager.ShowInquiry(val, true, false);
	}

	public static void HandleAIAcceptFeastInvite(FeastData feast, Clan clan)
	{
		Hero leader = clan.Leader;
		if (leader != null && feast != null && feast.Host != null && feast.Settlement != null && !((List<Hero>)(object)feast.Attendees).Contains(leader))
		{
			((List<Hero>)(object)feast.Attendees).Add(leader);
		}
	}

	private static bool CanManageFeast(MenuCallbackArgs args)
	{
		return true;
	}

	public static void ManageFeast(MenuCallbackArgs args)
	{
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f.Host == Hero.MainHero && f.Settlement == Settlement.CurrentSettlement);
		if (feastByAttribute == null || feastByAttribute.FeastRoster == null)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("feast_not_hosting", "You are not hosting a feast at this location."), Colors.Red));
		}
		else
		{
			try
			{
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("feast_manage_info", "Feast inventory management is not available in this version."), Colors.Yellow));
			}
			catch
			{
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("feast_manage_error", "Could not open feast inventory. Please try again."), Colors.Red));
			}
		}
	}

	public static void HandlePlayerAttendanceFailure(FeastData feast)
	{
		if (((List<Hero>)(object)feast.Attendees).Contains(Hero.MainHero))
		{
			_ = feast.PlayerInvitationAcceptedTime;
			CampaignTime val = CampaignTime.Now;
			double toDays = val.ToDays;
			val = feast.PlayerInvitationAcceptedTime;
			if (toDays - val.ToDays >= 5.0)
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_invitation_no_show", "{HOST} is upset that you accepted their feast invitation but did not show up.").SetTextVariable("HOST", ((object)feast.Host.Name).ToString())).ToString(), Colors.Yellow));
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(feast.Host, Hero.MainHero, -2, true);
				((List<Hero>)(object)feast.Attendees).Remove(Hero.MainHero);
				val = (feast.PlayerInvitationAcceptedTime = default(CampaignTime));
				feast.PlayerHasJoinedForFirstTime = false;
			}
		}
	}

	public static void HandleConsumeFoodAtFeast(FeastData feast)
	{
		FeastData feast2 = feast;
		if (!((IEnumerable<Hero>)feast2.Attendees).Any())
		{
			return;
		}
		int num = ((IEnumerable<Hero>)feast2.Attendees).Count(delegate(Hero x)
		{
			MobileParty partyBelongedTo = x.PartyBelongedTo;
			return ((partyBelongedTo != null) ? partyBelongedTo.CurrentSettlement : null) == feast2.Settlement;
		});
		if (num == 0)
		{
			return;
		}
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			ItemRosterElement val = ((IEnumerable<ItemRosterElement>)feast2.FeastRoster).FirstOrDefault(delegate(ItemRosterElement e)
			{
				EquipmentElement equipmentElement4 = e.EquipmentElement;
				ItemObject item3 = equipmentElement4.Item;
				return item3 != null && item3.IsFood && e.Amount > 0;
			});
			EquipmentElement equipmentElement = val.EquipmentElement;
			if (equipmentElement.Item == null)
			{
				break;
			}
			feast2.FeastRoster.AddToCounts(val.EquipmentElement, -1);
			num2++;
		}
		int num3 = ((IEnumerable<ItemRosterElement>)feast2.FeastRoster).Where(delegate(ItemRosterElement e)
		{
			EquipmentElement equipmentElement3 = e.EquipmentElement;
			ItemObject item2 = equipmentElement3.Item;
			return item2 != null && item2.IsFood;
		}).Sum((ItemRosterElement e) => e.Amount);
		int num4 = ((num > 0) ? (num3 / num) : 0);
		if (feast2.Host == Hero.MainHero)
		{
			MBInformationManager.AddQuickInformation(LanguageTranslater.L.T("feast_food_consumed", "Feast: Consumed {AMOUNT} food today. {DAYS} days of food remain.").SetTextVariable("AMOUNT", num2).SetTextVariable("DAYS", num4), 0, (BasicCharacterObject)null, null);
		}
		if (!((IEnumerable<ItemRosterElement>)feast2.FeastRoster).Any(delegate(ItemRosterElement e)
		{
			EquipmentElement equipmentElement2 = e.EquipmentElement;
			ItemObject item = equipmentElement2.Item;
			return item != null && item.IsFood && e.Amount > 0;
		}))
		{
			FeastBehavior.EndFeast(feast2, ((object)LanguageTranslater.L.T("feast_ended_no_food", "The feast at {SETTLEMENT} has ended due to lack of food.").SetTextVariable("SETTLEMENT", ((object)feast2.Settlement.Name).ToString())).ToString());
		}
	}

	public static void MaybeRemoveAILordFromFeast(FeastData feast, Hero hero)
	{
		if (feast == null || hero == null || hero == Hero.MainHero || !((List<Hero>)(object)feast.Attendees).Contains(hero) || feast.Host == hero)
		{
			return;
		}
		float num = GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastAIMinDays;
		float num2 = GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastAIMaxDays;
		CampaignTime val = CampaignTime.Now;
		double toDays = val.ToDays;
		val = feast.StartTime;
		float num3 = (float)(toDays - val.ToDays);
		if (num3 < num)
		{
			return;
		}
		float num4 = Math.Min(1f, (num3 - num) / (num2 - num));
		float num5 = MBRandom.RandomFloatRanged(0f, 1f);
		if (num5 < num4)
		{
			((List<Hero>)(object)feast.Attendees).Remove(hero);
			Clan clan = Hero.MainHero.Clan;
			Kingdom obj = ((clan != null) ? clan.Kingdom : null);
			Clan clan2 = feast.Host.Clan;
			if (obj == ((clan2 != null) ? clan2.Kingdom : null))
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_guest_left", "{GUEST} has left the feast at {SETTLEMENT}.").SetTextVariable("GUEST", ((object)hero.Name).ToString()).SetTextVariable("SETTLEMENT", ((object)feast.Settlement.Name).ToString())).ToString(), Colors.Yellow));
			}
		}
	}

	public static void MaybeEndAIFeast(FeastData feast)
	{
		if (feast == null || feast.Host == null || feast.Host == Hero.MainHero)
		{
			return;
		}
		float num = GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastAIMinDays;
		CampaignTime val = CampaignTime.Now;
		double toDays = val.ToDays;
		val = feast.StartTime;
		float num2 = (float)(toDays - val.ToDays);
		if (num2 < num)
		{
			return;
		}
		int count = ((List<Hero>)(object)feast.Attendees).Count;
		if (count > 5)
		{
			return;
		}
		if (count <= 1)
		{
			FeastBehavior.EndFeast(feast, ((object)LanguageTranslater.L.T("feast_ended_all_left", "The feast at {SETTLEMENT} has ended as all the guests have left.").SetTextVariable("SETTLEMENT", ((object)feast.Settlement.Name).ToString())).ToString());
			return;
		}
		float num3 = 1f - (float)(count - 1) / 4f;
		float num4 = MBRandom.RandomFloatRanged(0f, 1f);
		if (num4 < num3)
		{
			FeastBehavior.EndFeast(feast, ((object)LanguageTranslater.L.T("feast_ended_most_left", "The feast at {SETTLEMENT} has ended early as most guests have left.").SetTextVariable("SETTLEMENT", ((object)feast.Settlement.Name).ToString())).ToString());
		}
	}
}



