using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace WarAndAITweaks.UI;

public class MilitaryManagementVM : ViewModel
{
	private MBBindingList<AiLordItemVM> _aiLords;

	private string _selectedAttackTarget;

	private bool _isAnyFactionSelected;

	public static Settlement _globalAttackTarget;

	[DataSourceProperty]
	public string MilitaryManagementHeader => LanguageTranslater.L.S("military_management_header", "AI Lord Military Management");

	[DataSourceProperty]
	public string SelectAttackTargetButton => LanguageTranslater.L.S("military_select_attack_target_button", "Select Attack Target");

	[DataSourceProperty]
	public string CurrentTargetLabel => LanguageTranslater.L.S("military_current_target_label", "Current Target:");

	[DataSourceProperty]
	public string LordHeader => LanguageTranslater.L.S("military_lord_header", "Lord");

	[DataSourceProperty]
	public string MilitaryClanHeader => LanguageTranslater.L.S("military_clan_header", "Clan");

	[DataSourceProperty]
	public string MilitaryStatusHeader => LanguageTranslater.L.S("military_status_header", "Status");

	[DataSourceProperty]
	public string PartySizeHeader => LanguageTranslater.L.S("military_party_size_header", "Party Size");

	[DataSourceProperty]
	public string InfluenceHeader => LanguageTranslater.L.S("military_influence_header", "Influence");

	[DataSourceProperty]
	public string ArmyStatusHeader => LanguageTranslater.L.S("military_army_status_header", "Army Status");

	[DataSourceProperty]
	public string ArmyCreationHeader => LanguageTranslater.L.S("military_army_creation_header", "Army Creation");

	[DataSourceProperty]
	public bool IsAnyFactionSelected
	{
		get
		{
			return _isAnyFactionSelected;
		}
		set
		{
			if (value != _isAnyFactionSelected)
			{
				_isAnyFactionSelected = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "IsAnyFactionSelected");
			}
		}
	}

	[DataSourceProperty]
	public bool CanSelectGlobalTarget
	{
		get
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
			return obj != null && Hero.MainHero == Hero.MainHero.Clan.Kingdom.Leader;
		}
	}

	[DataSourceProperty]
	public MBBindingList<AiLordItemVM> AiLords
	{
		get
		{
			return _aiLords;
		}
		set
		{
			if (value != _aiLords)
			{
				_aiLords = value;
				((ViewModel)this).OnPropertyChangedWithValue<MBBindingList<AiLordItemVM>>(value, "AiLords");
			}
		}
	}

	[DataSourceProperty]
	public string SelectedAttackTarget
	{
		get
		{
			return _selectedAttackTarget;
		}
		set
		{
			if (value != _selectedAttackTarget)
			{
				_selectedAttackTarget = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "SelectedAttackTarget");
			}
		}
	}

	public MilitaryManagementVM()
	{
		SelectedAttackTarget = LanguageTranslater.L.S("None", "None");
	}

	public void RefreshData()
	{
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		AiLords = new MBBindingList<AiLordItemVM>();
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
			List<Hero> list = (from c in (IEnumerable<Clan>)val.Clans
				where !c.IsEliminated && !c.IsUnderMercenaryService && c.Leader != null && c.Leader != Hero.MainHero
				select c.Leader into h
				orderby ((object)h.Name).ToString()
				select h).ToList();
			foreach (Hero item in list)
			{
				((Collection<AiLordItemVM>)(object)AiLords).Add(new AiLordItemVM(item));
			}
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("error_loading_military_data", "Error loading military data: {EX}").SetTextVariable("EX", ex.Message)).ToString(), Colors.Red));
		}
	}

	public void ExecuteSelectGlobalTarget()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
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
		if (val == null || val.Leader != Hero.MainHero)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("only_kingdom_rulers_set_global_targets", "Only kingdom rulers can set global targets."), Colors.Red));
			return;
		}
		List<Settlement> enemySettlements = GetEnemySettlements(val);
		if (enemySettlements.Count == 0)
		{
			InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("no_enemy_settlements_for_targeting", "No enemy settlements available for targeting."), Colors.Yellow));
		}
		else
		{
			ShowGlobalTargetSelection(enemySettlements);
		}
	}

	private List<Settlement> GetEnemySettlements(Kingdom kingdom)
	{
		Kingdom kingdom2 = kingdom;
		return (from s in (IEnumerable<Settlement>)Settlement.All
			where (s.IsTown || s.IsCastle) && s.MapFaction != kingdom2 && kingdom2.IsAtWarWith(s.MapFaction)
			orderby ((object)s.Name).ToString()
			select s).ToList();
	}

	private void ShowGlobalTargetSelection(List<Settlement> settlements)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		List<InquiryElement> list = new List<InquiryElement>();
		list.Add(new InquiryElement((object)null, LanguageTranslater.L.S("none_clear_target_label", "None - Clear Target"), (ImageIdentifier)null, true, LanguageTranslater.L.S("none_clear_target_hint", "Clear the current attack target")));
		foreach (Settlement settlement in settlements)
		{
			IFaction mapFaction = settlement.MapFaction;
			string arg = ((mapFaction == null) ? null : ((object)mapFaction.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
			string text = $"{settlement.Name} ({arg})";
			string text2 = (settlement.IsTown ? LanguageTranslater.L.S("town", "Town") : LanguageTranslater.L.S("castle", "Castle"));
			list.Add(new InquiryElement((object)settlement, text, (ImageIdentifier)null, true, text2));
		}
		MBInformationManager.ShowMultiSelectionInquiry(new MultiSelectionInquiryData(LanguageTranslater.L.S("select_global_attack_target_title", "Select Global Attack Target"), LanguageTranslater.L.S("select_global_attack_target_body", "Choose a target for your AI lords to attack:"), list, true, 1, 1, LanguageTranslater.L.S("select_button", "Select"), LanguageTranslater.L.S("cancel_button", "Cancel"), (Action<List<InquiryElement>>)delegate(List<InquiryElement> elements)
		{
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Expected O, but got Unknown
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Expected O, but got Unknown
			if (elements != null && elements.Any())
			{
				object obj = elements.FirstOrDefault()?.Identifier;
				Settlement val = (_globalAttackTarget = (Settlement)((obj is Settlement) ? obj : null));
				SelectedAttackTarget = ((val == null) ? null : ((object)val.Name)?.ToString()) ?? LanguageTranslater.L.S("None", "None");
				if (val != null)
				{
					string text3 = ((object)LanguageTranslater.L.T("global_attack_target_set", "Global attack target set to {SETTLEMENT}").SetTextVariable("SETTLEMENT", val.Name)).ToString();
					InformationManager.DisplayMessage(new InformationMessage(text3, Colors.Cyan));
				}
				else
				{
					InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("global_attack_target_cleared", "Global attack target has been cleared"), Colors.Cyan));
				}
			}
		}, (Action<List<InquiryElement>>)delegate
		{
		}, "", false), false, false);
	}

	private void ValidateGlobalAttackTarget()
	{
		if (_globalAttackTarget != null)
		{
			IFaction mapFaction = _globalAttackTarget.MapFaction;
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
			if (mapFaction == obj || !Hero.MainHero.Clan.Kingdom.IsAtWarWith(_globalAttackTarget.MapFaction))
			{
				_globalAttackTarget = null;
				SelectedAttackTarget = LanguageTranslater.L.S("None", "None");
			}
		}
	}

	public static Settlement GetGlobalAttackTarget()
	{
		return _globalAttackTarget;
	}
}
