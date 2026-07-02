using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace WarAndAITweaks.UI;

public class FiefManagementVM : ViewModel
{
	private MBBindingList<FiefItemVM> _fiefClans;

	[DataSourceProperty]
	public string FiefsManagementHeader => LanguageTranslater.L.S("fiefs_management_header", "Fiefs Management");

	[DataSourceProperty]
	public string GrantFiefButton => LanguageTranslater.L.S("fiefs_grant_button", "Grant Fief Title");

	[DataSourceProperty]
	public string RevokeFiefButton => LanguageTranslater.L.S("fiefs_revoke_button", "Revoke Fief Title");

	[DataSourceProperty]
	public string ClanHeader => LanguageTranslater.L.S("fiefs_clan_header", "Clan");

	[DataSourceProperty]
	public string RelationHeader => LanguageTranslater.L.S("fiefs_relation_header", "Relation");

	[DataSourceProperty]
	public string TownsHeader => LanguageTranslater.L.S("fiefs_towns_header", "Towns");

	[DataSourceProperty]
	public string CastlesHeader => LanguageTranslater.L.S("fiefs_castles_header", "Castles");

	[DataSourceProperty]
	public bool CanManageFiefs
	{
		get
		{
			if (Hero.MainHero?.Clan?.Kingdom == null)
				return false;
			return Hero.MainHero == Hero.MainHero.Clan.Kingdom.Leader;
		}
	}

	[DataSourceProperty]
	public MBBindingList<FiefItemVM> FiefClans
	{
		get
		{
			return _fiefClans;
		}
		set
		{
			if (value != _fiefClans)
			{
				_fiefClans = value;
				((ViewModel)this).OnPropertyChangedWithValue<MBBindingList<FiefItemVM>>(value, "FiefClans");
			}
		}
	}

	public FiefManagementVM()
	{
		FiefClans = new MBBindingList<FiefItemVM>();
	}

	public void RefreshData()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		FiefClans = new MBBindingList<FiefItemVM>();
		try
		{
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
			Kingdom val = (Kingdom)obj;
			if (val == null)
			{
				return;
			}
			foreach (Clan item in from c in (IEnumerable<Clan>)val.Clans
				where !c.IsEliminated
				orderby ((object)c.Name).ToString()
				select c)
			{
				((Collection<FiefItemVM>)(object)FiefClans).Add(new FiefItemVM(item));
			}
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("error_loading_fief_data", "Error loading fief data: {EX}").SetTextVariable("EX", ex.Message)).ToString(), Colors.Red));
		}
	}

	public void ExecuteGrantFiefTitle()
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
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
		if (kingdom == null)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("not_part_of_kingdom", "You are not part of a kingdom."), Colors.Red));
			return;
		}
		if (kingdom.Leader != Hero.MainHero)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("only_ruler_can_grant_fiefs", "Only the ruler can grant fief titles."), Colors.Red));
			return;
		}
		List<Clan> list = (from c in (IEnumerable<Clan>)kingdom.Clans
			where !c.IsEliminated && c.Leader != null
			orderby ((object)c.Name).ToString()
			select c).ToList();
		if (list.Count == 0)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("no_valid_clans_in_kingdom", "No valid clans in your kingdom."), Colors.Yellow));
			return;
		}
		List<InquiryElement> list2 = new List<InquiryElement>();
		foreach (Clan item in list)
		{
			ImageIdentifier val = null;
			try
			{
				if (item.Banner != null)
				{
					val = new BannerImageIdentifier(item.Banner);
				}
			}
			catch
			{
			}
			list2.Add(new InquiryElement((object)item, FormatClanSelectionLabel(item), val, true, "Select clan"));
		}
		MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(LanguageTranslater.L.S("grant_fief_title", "Grant Fief Title"), LanguageTranslater.L.S("select_clan_to_grant_fief", "Select a clan to grant a fief to:"), list2, true, 1, 1, "Next", "Cancel", (Action<List<InquiryElement>>)delegate(List<InquiryElement> elements)
		{
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Expected O, but got Unknown
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Expected O, but got Unknown
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Expected O, but got Unknown
			Clan selectedClan = default(Clan);
			ref Clan reference = ref selectedClan;
			object obj3 = elements?.FirstOrDefault()?.Identifier;
			reference = (Clan)((obj3 is Clan) ? obj3 : null);
			if (selectedClan != null && selectedClan.Leader != null)
			{
				List<Settlement> list3 = (from s in (IEnumerable<Settlement>)Settlement.All
					where (s.IsTown || s.IsCastle) && s.MapFaction == kingdom && s.OwnerClan != selectedClan
					orderby ((object)s.Name).ToString()
					select s).ToList();
				if (list3.Count == 0)
				{
					InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("no_valid_fiefs_to_grant", "No valid fiefs to grant to this clan."), Colors.Yellow));
				}
				else
				{
					List<InquiryElement> list4 = new List<InquiryElement>();
					foreach (Settlement item2 in list3)
					{
						Clan ownerClan = item2.OwnerClan;
						string arg = ((ownerClan == null) ? null : ((object)ownerClan.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
						string text = $"{item2.Name} (Owner: {arg})";
						string text2 = (item2.IsTown ? "Town" : "Castle");
						list4.Add(new InquiryElement((object)item2, text, (ImageIdentifier)null, true, text2));
					}
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(LanguageTranslater.L.S("grant_fief_title", "Grant Fief Title"), ((object)LanguageTranslater.L.T("select_fief_to_grant", "Select a fief to grant to {CLAN}:").SetTextVariable("CLAN", ((object)selectedClan.Name).ToString())).ToString(), list4, true, 1, 1, "Grant", "Cancel", (Action<List<InquiryElement>>)delegate(List<InquiryElement> fes)
					{
						//IL_0184: Unknown result type (might be due to invalid IL or missing references)
						//IL_0189: Unknown result type (might be due to invalid IL or missing references)
						//IL_0193: Expected O, but got Unknown
						//IL_0089: Unknown result type (might be due to invalid IL or missing references)
						//IL_008e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0098: Expected O, but got Unknown
						//IL_0137: Unknown result type (might be due to invalid IL or missing references)
						//IL_013c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0146: Expected O, but got Unknown
						object obj4 = fes?.FirstOrDefault()?.Identifier;
						Settlement val2 = (Settlement)((obj4 is Settlement) ? obj4 : null);
						if (val2 == null)
						{
							return;
						}
						Clan ownerClan2 = val2.OwnerClan;
						try
						{
							ChangeOwnerOfSettlementAction.ApplyByDefault(selectedClan.Leader, val2);
							InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("fief_granted", "Granted {SETTLEMENT} to {CLAN}.").SetTextVariable("SETTLEMENT", ((object)val2.Name).ToString()).SetTextVariable("CLAN", ((object)selectedClan.Name).ToString())).ToString(), Colors.Green));
							if (ownerClan2 != null && ownerClan2 != selectedClan && ownerClan2 != Hero.MainHero.Clan)
							{
								ApplyFiefLossPenalty(ownerClan2, "fief granted to another clan");
							}
							if (selectedClan != Hero.MainHero.Clan)
							{
								ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, selectedClan.Leader, 5, true);
								InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("relation_increased_fief_granted", "Relation with {CLAN} increased by 5 (fief granted).").SetTextVariable("CLAN", ((object)selectedClan.Name).ToString())).ToString(), Colors.Green));
							}
							RefreshData();
						}
						catch (Exception ex)
						{
							InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("failed_to_grant_fief", "Failed to grant fief: {EX}").SetTextVariable("EX", ex.Message)).ToString(), Colors.Red));
						}
					}, (Action<List<InquiryElement>>)delegate
					{
					}, "", false), false, false);
				}
			}
		}, (Action<List<InquiryElement>>)delegate
		{
		}, "", false), false, false);
	}

	public void ExecuteRevokeFiefTitle()
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
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
		if (kingdom == null)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("not_part_of_kingdom", "You are not part of a kingdom."), Colors.Red));
			return;
		}
		if (kingdom.Leader != Hero.MainHero)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("only_ruler_can_grant_fiefs", "Only the ruler can grant fief titles."), Colors.Red));
			return;
		}
		List<Clan> list = (from c in (IEnumerable<Clan>)kingdom.Clans
			where !c.IsEliminated && c.Leader != null
			orderby ((object)c.Name).ToString()
			select c).ToList();
		if (list.Count == 0)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("no_valid_clans_in_kingdom", "No valid clans in your kingdom."), Colors.Yellow));
			return;
		}
		List<InquiryElement> list2 = new List<InquiryElement>();
		foreach (Clan item in list)
		{
			ImageIdentifier val = null;
			try
			{
				if (item.Banner != null)
				{
					val = new BannerImageIdentifier(item.Banner);
				}
			}
			catch
			{
			}
			list2.Add(new InquiryElement((object)item, FormatClanSelectionLabel(item), val, true, "Select clan"));
		}
		MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(LanguageTranslater.L.S("revoke_fief_title", "Revoke Fief Title"), LanguageTranslater.L.S("select_clan_to_revoke_fief", "Select a clan to revoke a fief from:"), list2, true, 1, 1, "Next", "Cancel", (Action<List<InquiryElement>>)delegate(List<InquiryElement> elements)
		{
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Expected O, but got Unknown
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Expected O, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			Clan selectedClan = default(Clan);
			ref Clan reference = ref selectedClan;
			object obj3 = elements?.FirstOrDefault()?.Identifier;
			reference = (Clan)((obj3 is Clan) ? obj3 : null);
			if (selectedClan != null)
			{
				List<Settlement> list3 = (from s in (IEnumerable<Settlement>)Settlement.All
					where (s.IsTown || s.IsCastle) && s.MapFaction == kingdom && s.OwnerClan == selectedClan
					orderby ((object)s.Name).ToString()
					select s).ToList();
				if (list3.Count == 0)
				{
					InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("clan_owns_no_fiefs", "This clan owns no fiefs to revoke."), Colors.Yellow));
				}
				else
				{
					List<InquiryElement> list4 = new List<InquiryElement>();
					foreach (Settlement item2 in list3)
					{
						string text = (item2.IsTown ? "Town" : "Castle");
						list4.Add(new InquiryElement((object)item2, ((object)item2.Name)?.ToString() ?? LanguageTranslater.L.S("Unknown", "Unknown"), (ImageIdentifier)null, true, text));
					}
					MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(LanguageTranslater.L.S("revoke_fief_title", "Revoke Fief Title"), ((object)LanguageTranslater.L.T("select_fief_to_revoke", "Select a fief to revoke from {CLAN}:").SetTextVariable("CLAN", ((object)selectedClan.Name).ToString())).ToString(), list4, true, 1, 1, "Revoke", "Cancel", (Action<List<InquiryElement>>)delegate(List<InquiryElement> fes)
					{
						//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
						//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
						//IL_00d8: Expected O, but got Unknown
						//IL_0063: Unknown result type (might be due to invalid IL or missing references)
						//IL_0068: Unknown result type (might be due to invalid IL or missing references)
						//IL_0072: Expected O, but got Unknown
						object obj4 = fes?.FirstOrDefault()?.Identifier;
						Settlement val2 = (Settlement)((obj4 is Settlement) ? obj4 : null);
						if (val2 == null)
						{
							return;
						}
						try
						{
							ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, val2);
							InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("fief_revoked", "Revoked {SETTLEMENT} and granted it to you.").SetTextVariable("SETTLEMENT", ((object)val2.Name).ToString())).ToString(), Colors.Green));
							ApplyFiefLossPenalty(selectedClan, "title revoked");
							RefreshData();
						}
						catch (Exception ex)
						{
							InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("failed_to_grant_fief", "Failed to grant fief: {EX}").SetTextVariable("EX", ex.Message)).ToString(), Colors.Red));
						}
					}, (Action<List<InquiryElement>>)delegate
					{
					}, "", false), false, false);
				}
			}
		}, (Action<List<InquiryElement>>)delegate
		{
		}, "", false), false, false);
	}

	private static string FormatClanSelectionLabel(Clan clan)
	{
		if (clan == null)
		{
			return LanguageTranslater.L.S("Unknown", "Unknown");
		}
		float num = 0f;
		try
		{
			Hero leader = clan.Leader;
			if (leader != null && Hero.MainHero != null)
			{
				num = Hero.MainHero.Clan.GetRelationWithClan(leader.Clan);
			}
		}
		catch
		{
			num = 0f;
		}
		int num2 = 0;
		int num3 = 0;
		try
		{
			num2 = ((IEnumerable<Settlement>)clan.Settlements)?.Count((Settlement s) => s.IsTown) ?? 0;
			num3 = ((IEnumerable<Settlement>)clan.Settlements)?.Count((Settlement s) => s.IsCastle) ?? 0;
		}
		catch
		{
		}
		string text = ((num >= 0f) ? $"+{num:0}" : $"{num:0}");
		return $"{clan.Name} | Rel: {text} | Towns: {num2} | Castles: {num3}";
	}

	private void ApplyFiefLossPenalty(Clan losingClan, string context)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		try
		{
			Hero val = ((losingClan != null) ? losingClan.Leader : null);
			if (val != null && val != Hero.MainHero)
			{
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, val, -10, true);
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("relation_decreased_fief_loss", "Relation with {CLAN} decreased by 10 ({CONTEXT}).").SetTextVariable("CLAN", ((object)losingClan.Name).ToString()).SetTextVariable("CONTEXT", context)).ToString(), Colors.Red));
			}
		}
		catch
		{
		}
	}
}

