using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FeastSystem;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using WarAndAiTweaks;

namespace WarAndAITweaks.UI;

public class FeastManagementVM : ViewModel
{
	private MBBindingList<FeastItemVM> _activeFeasts;

	[DataSourceProperty]
	public string OngoingFeastsHeader => LanguageTranslater.L.S("feasts_ongoing_header", "Ongoing Feasts");

	[DataSourceProperty]
	public string HostFeastButton => LanguageTranslater.L.S("feasts_host_button", "Host Feast");

	[DataSourceProperty]
	public string EndFeastButton => LanguageTranslater.L.S("feasts_end_button", "End Feast");

	[DataSourceProperty]
	public string FactionHeader => LanguageTranslater.L.S("feasts_faction_header", "Faction");

	[DataSourceProperty]
	public string HostHeader => LanguageTranslater.L.S("feasts_host_header", "Host");

	[DataSourceProperty]
	public string LocationHeader => LanguageTranslater.L.S("feasts_location_header", "Location");

	[DataSourceProperty]
	public string DurationHeader => LanguageTranslater.L.S("feasts_duration_header", "Duration");

	[DataSourceProperty]
	public string AttendeesHeader => LanguageTranslater.L.S("feasts_attendees_header", "Attendees");

	[DataSourceProperty]
	public MBBindingList<FeastItemVM> ActiveFeasts
	{
		get
		{
			return _activeFeasts;
		}
		set
		{
			if (value != _activeFeasts)
			{
				_activeFeasts = value;
				((ViewModel)this).OnPropertyChangedWithValue<MBBindingList<FeastItemVM>>(value, "ActiveFeasts");
			}
		}
	}

	public FeastManagementVM()
	{
		ActiveFeasts = new MBBindingList<FeastItemVM>();
	}

	public void RefreshData()
	{
		ActiveFeasts = new MBBindingList<FeastItemVM>();
		try
		{
			foreach (FeastData item in (List<FeastData>)(object)FeastBehavior._activeFeasts)
			{
				((Collection<FeastItemVM>)(object)ActiveFeasts).Add(new FeastItemVM(item));
			}
		}
		catch (Exception)
		{
		}
	}

	public void ExecuteHostFeast()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Expected O, but got Unknown
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Expected O, but got Unknown
		Hero player = Hero.MainHero;
		Hero obj = player;
		Clan val = ((obj != null) ? obj.Clan : null);
		Kingdom kingdom = ((val != null) ? val.Kingdom : null);
		if (player == null || val == null)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("you_not_part_of_clan", "You are not part of a clan."), Colors.Red));
			return;
		}
		if (!FeastHelper.CanHostFeast(player))
		{
			ShowFeastHostingError();
			return;
		}
		if (kingdom != null)
		{
			if (KingdomHasActiveFeast(kingdom))
			{
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("kingdom_already_hosting_feast", "Your kingdom is already hosting a feast."), Colors.Red));
				return;
			}
			if (!FeastHelper.FactionEligbleForFeast(kingdom))
			{
				if (((IEnumerable<Kingdom>)Kingdom.All).Any((Kingdom x) => x.IsAtWarWith((IFaction)(object)kingdom) && ((IEnumerable<Settlement>)x.Settlements).Any()))
				{
					InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("cannot_host_feast_during_war", "Cannot host a feast during war."), Colors.Red));
					return;
				}
				int feastCooldownDaysLeft = GetFeastCooldownDaysLeft((IFaction)(object)kingdom);
				if (feastCooldownDaysLeft > 0)
				{
					InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("must_wait_days_before_hosting", "Your kingdom must wait {DAYS} more day(s) before hosting another feast.").SetTextVariable("DAYS", feastCooldownDaysLeft.ToString())).ToString(), Colors.Red));
					return;
				}
			}
		}
		List<Settlement> list = (from s in (IEnumerable<Settlement>)val.Settlements
			where (s.IsTown || s.IsCastle) && !s.IsUnderSiege
			orderby ((object)s.Name).ToString()
			select s).ToList();
		if (list.Count == 0)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("no_valid_settlements_to_host_feast", "No valid settlements to host a feast."), Colors.Yellow));
			return;
		}
		List<InquiryElement> list2 = new List<InquiryElement>();
		foreach (Settlement item in list)
		{
			list2.Add(new InquiryElement((object)item, ((object)item.Name)?.ToString() ?? LanguageTranslater.L.S("Unknown", "Unknown"), (ImageIdentifier)null, true, item.IsTown ? "Town" : "Castle"));
		}
		MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(LanguageTranslater.L.S("host_feast_title", "Host Feast"), LanguageTranslater.L.S("host_feast_select_settlement", "Select a settlement to host a feast:"), list2, true, 1, 1, LanguageTranslater.L.S("host", "Host"), LanguageTranslater.L.S("cancel", "Cancel"), (Action<List<InquiryElement>>)delegate(List<InquiryElement> elements)
		{
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Expected O, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			object obj2 = elements?.FirstOrDefault()?.Identifier;
			Settlement val2 = (Settlement)((obj2 is Settlement) ? obj2 : null);
			if (val2 != null)
			{
				if (!FeastHelper.CanHostFeast(player))
				{
					ShowFeastHostingError();
				}
				else
				{
					if (kingdom != null)
					{
						if (KingdomHasActiveFeast(kingdom))
						{
							InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("kingdom_already_hosting_feast", "Your kingdom is already hosting a feast."), Colors.Red));
							return;
						}
						if (!FeastHelper.FactionEligbleForFeast(kingdom))
						{
							InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("kingdom_cannot_host_feast_now", "Your kingdom cannot host a feast right now."), Colors.Red));
							return;
						}
					}
					GiveGoldAction.ApplyBetweenCharacters(player, (Hero)null, 5000, false);
					FeastBehavior.StartFeast(player, val2);
					InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_started", "You have started a feast at {SETTLEMENT}!").SetTextVariable("SETTLEMENT", ((object)val2.Name).ToString())).ToString(), Colors.Green));
					RefreshData();
				}
			}
		}, (Action<List<InquiryElement>>)delegate
		{
		}, "", false), false, false);
	}

	public void ExecuteEndFeast()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		Hero mainHero = Hero.MainHero;
		object obj;
		if (mainHero == null)
		{
			obj = null;
		}
		else
		{
			Clan clan = mainHero.Clan;
			obj = ((clan != null) ? clan.Kingdom : null);
		}
		Kingdom kingdom = (Kingdom)obj;
		if (mainHero == null || kingdom == null)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("not_part_of_kingdom", "You are not part of a kingdom."), Colors.Yellow));
			return;
		}
		FeastData feast = FeastBehavior.GetFeastByAttribute((FeastData f) => f?.Kingdom == kingdom);
		if (feast == null)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("no_active_feast_for_kingdom", "Your kingdom is not hosting any active feasts."), Colors.Yellow));
			return;
		}
		Settlement settlement = feast.Settlement;
		string settlementName = ((settlement == null) ? null : ((object)settlement.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		InformationManager.ShowInquiry(new InquiryData(LanguageTranslater.L.S("end_feast_title", "End Feast"), ((object)LanguageTranslater.L.T("end_feast_confirm", "Are you sure you want to end the feast at {SETTLEMENT}?").SetTextVariable("SETTLEMENT", settlementName)).ToString(), true, true, LanguageTranslater.L.S("end_feast", "End Feast"), LanguageTranslater.L.S("cancel", "Cancel"), (Action)delegate
		{
			FeastBehavior.EndFeast(feast, ((object)LanguageTranslater.L.T("you_end_feast_message", "You end the feast at {SETTLEMENT}.").SetTextVariable("SETTLEMENT", settlementName)).ToString());
			RefreshData();
		}, (Action)delegate
		{
		}, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
	}

	private static bool KingdomHasActiveFeast(Kingdom k)
	{
		Kingdom i = k;
		if (i == null)
		{
			return false;
		}
		return FeastBehavior.GetFeastByAttribute(delegate(FeastData f)
		{
			object obj;
			if (f == null)
			{
				obj = null;
			}
			else
			{
				Hero host = f.Host;
				if (host == null)
				{
					obj = null;
				}
				else
				{
					Clan clan = host.Clan;
					obj = ((clan != null) ? clan.Kingdom : null);
				}
			}
			return obj == i;
		}) != null;
	}

	private static int GetFeastCooldownDaysLeft(IFaction faction)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (faction == null)
		{
			return 0;
		}
		if (FeastBehavior._lastFeastByFaction.TryGetValue((Kingdom)(object)((faction is Kingdom) ? faction : null), out var value))
		{
			int todayWeFeastCooldown = GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastCooldown;
			CampaignTime now = CampaignTime.Now;
			int num = (int)Math.Floor(now.ToDays - value.ToDays);
			int num2 = todayWeFeastCooldown - num;
			return (num2 > 0) ? num2 : 0;
		}
		return 0;
	}

	private void ShowFeastHostingError()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		Hero mainHero = Hero.MainHero;
		Clan val = ((mainHero != null) ? mainHero.Clan : null);
		if (mainHero == null || val == null)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("you_not_part_of_clan", "You are not part of a clan."), Colors.Red));
		}
		else if (mainHero != val.Leader)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("only_clan_leader_can_host_feast", "Only the clan leader can host a feast."), Colors.Red));
		}
		else if (mainHero.Spouse == null)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("must_be_married_to_host_feast", "You must be married to host a feast."), Colors.Red));
		}
		else if (mainHero.Gold < 5000)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("need_gold_to_host_feast", "You need 5000 gold to host a feast."), Colors.Red));
		}
		else if (!((IEnumerable<Settlement>)val.Settlements).Any((Settlement s) => s.IsTown || s.IsCastle))
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("must_own_settlement_to_host_feast", "You must own a castle or town to host a feast."), Colors.Red));
		}
		else
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("cannot_host_feast_right_now", "You cannot host a feast right now."), Colors.Red));
		}
	}
}


