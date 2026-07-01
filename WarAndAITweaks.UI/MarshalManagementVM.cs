using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using WarAndAITweaks.MarshalSystem;
using WarAndAITweaks.Utils;

namespace WarAndAITweaks.UI;

public class MarshalManagementVM : ViewModel
{
	private string _currentMarshalName;

	private string _currentMarshalClan;

	private string _currentMarshalSkill;

	private string _currentMarshalStatus;

	private string _marshalStatus;

	private bool _canSelectMarshal;

	private ImageIdentifierVM _currentMarshalBanner;

	[DataSourceProperty]
	public string MarshalManagementHeader => LanguageTranslater.L.S("marshal_management_header", "Marshal Management");

	[DataSourceProperty]
	public string AppointMarshalButton => LanguageTranslater.L.S("appoint_marshal_button", "Appoint Marshal");

	[DataSourceProperty]
	public string CurrentMarshalHeader => LanguageTranslater.L.S("current_marshal_header", "Current Marshal");

	[DataSourceProperty]
	public string NameHeader => LanguageTranslater.L.S("marshal_name_header", "Name");

	[DataSourceProperty]
	public string MarshalClanHeader => LanguageTranslater.L.S("marshal_clan_header", "Clan");

	[DataSourceProperty]
	public string SkillHeader => LanguageTranslater.L.S("marshal_skill_header", "Skill");

	[DataSourceProperty]
	public string MarshalStatusHeader => LanguageTranslater.L.S("marshal_status_header", "Status");

	[DataSourceProperty]
	public string CurrentMarshalName
	{
		get
		{
			return _currentMarshalName;
		}
		set
		{
			if (value != _currentMarshalName)
			{
				_currentMarshalName = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "CurrentMarshalName");
			}
		}
	}

	[DataSourceProperty]
	public string CurrentMarshalClan
	{
		get
		{
			return _currentMarshalClan;
		}
		set
		{
			if (value != _currentMarshalClan)
			{
				_currentMarshalClan = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "CurrentMarshalClan");
			}
		}
	}

	[DataSourceProperty]
	public string CurrentMarshalSkill
	{
		get
		{
			return _currentMarshalSkill;
		}
		set
		{
			if (value != _currentMarshalSkill)
			{
				_currentMarshalSkill = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "CurrentMarshalSkill");
			}
		}
	}

	[DataSourceProperty]
	public string CurrentMarshalStatus
	{
		get
		{
			return _currentMarshalStatus;
		}
		set
		{
			if (value != _currentMarshalStatus)
			{
				_currentMarshalStatus = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "CurrentMarshalStatus");
			}
		}
	}

	[DataSourceProperty]
	public string MarshalStatus
	{
		get
		{
			return _marshalStatus;
		}
		set
		{
			if (value != _marshalStatus)
			{
				_marshalStatus = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "MarshalStatus");
			}
		}
	}

	[DataSourceProperty]
	public bool CanSelectMarshal
	{
		get
		{
			return _canSelectMarshal;
		}
		set
		{
			if (value != _canSelectMarshal)
			{
				_canSelectMarshal = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "CanSelectMarshal");
			}
		}
	}

	[DataSourceProperty]
	public ImageIdentifierVM CurrentMarshalBanner
	{
		get
		{
			return _currentMarshalBanner;
		}
		set
		{
			if (value != _currentMarshalBanner)
			{
				_currentMarshalBanner = value;
				((ViewModel)this).OnPropertyChangedWithValue<ImageIdentifierVM>(value, "CurrentMarshalBanner");
			}
		}
	}

	public MarshalManagementVM()
	{
		MarshalSystemBehavior.MarshalAppointed += OnMarshalAppointed;
		RefreshData();
	}

	public void RefreshData()
	{
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
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
			CanSelectMarshal = val != null && val.Leader == Hero.MainHero;
			if (val == null)
			{
				CurrentMarshalName = LanguageTranslater.L.S("not_in_kingdom_marshal", "Not in a kingdom");
				CurrentMarshalClan = "";
				CurrentMarshalSkill = "";
				CurrentMarshalStatus = "";
				MarshalStatus = LanguageTranslater.L.S("marshal_status_must_be_in_kingdom", "You must be part of a kingdom to view marshal information.");
				CurrentMarshalBanner = null;
				return;
			}
			Hero marshal = MarshalSystemBehavior.GetMarshal(val);
			if (marshal != null)
			{
				CurrentMarshalName = ((object)marshal.Name)?.ToString() ?? LanguageTranslater.L.S("Unknown", "Unknown");
				Clan clan2 = marshal.Clan;
				CurrentMarshalClan = ((clan2 == null) ? null : ((object)clan2.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
				try
				{
					CurrentMarshalSkill = MarshalHelper.CalculateTrueSkill(marshal).ToString("F0");
				}
				catch
				{
					CurrentMarshalSkill = LanguageTranslater.L.S("Unknown", "Unknown");
				}
				CurrentMarshalStatus = GetMarshalStatus(marshal);
				try
				{
					Clan clan3 = marshal.Clan;
					if (((clan3 != null) ? clan3.Banner : null) != null)
					{
						CurrentMarshalBanner = new ImageIdentifierVM(new ImageIdentifier(marshal.Clan.Banner));
					}
					else
					{
						CurrentMarshalBanner = null;
					}
				}
				catch
				{
					CurrentMarshalBanner = null;
				}
				MarshalStatus = (CanSelectMarshal ? LanguageTranslater.L.S("marshal_status_ruler", "As ruler, you can appoint a new marshal.") : ((object)LanguageTranslater.L.T("marshal_status_current_of", "Current Marshal of {KINGDOM}").SetTextVariable("KINGDOM", val.Name)).ToString());
			}
			else
			{
				CurrentMarshalName = LanguageTranslater.L.S("no_marshal_appointed", "No marshal appointed");
				CurrentMarshalClan = "";
				CurrentMarshalSkill = "";
				CurrentMarshalStatus = "";
				CurrentMarshalBanner = null;
				MarshalStatus = (CanSelectMarshal ? LanguageTranslater.L.S("no_marshal_currently_appointed", "No marshal is currently appointed.") : ((object)LanguageTranslater.L.T("marshal_status_none_for_kingdom", "{KINGDOM} currently has no marshal.").SetTextVariable("KINGDOM", val.Name)).ToString());
			}
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("error_loading_marshal_data", "Error loading marshal data: {EX}").SetTextVariable("EX", ex.Message)).ToString(), Colors.Red));
			CurrentMarshalName = LanguageTranslater.L.S("error_loading_data", "Error loading data");
			CurrentMarshalClan = "";
			CurrentMarshalSkill = "";
			CurrentMarshalStatus = "";
			CurrentMarshalBanner = null;
			MarshalStatus = LanguageTranslater.L.S("failed_to_load_marshal_information", "Failed to load marshal information.");
		}
	}

	public void ExecuteAppointMarshal()
	{
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
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
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("not_in_a_kingdom", "You are not part of a kingdom."), Colors.Red));
				return;
			}
			if (val.Leader != Hero.MainHero)
			{
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("only_ruler_call_marshal", "Only the ruler can call a marshal election."), Colors.Red));
				return;
			}
			MBList<Hero> val2 = new MBList<Hero>();
			foreach (Hero item in (List<Hero>)(object)val.Heroes)
			{
				if (MarshalHelper.IsEligibleForMarshalship(item))
				{
					((List<Hero>)(object)val2).Add(item);
				}
			}
			if (((List<Hero>)(object)val2).Count == 0)
			{
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("no_eligible_vassals", "No eligible vassals are available for marshal election."), Colors.Yellow));
				return;
			}
			Clan val3 = val.RulingClan ?? Hero.MainHero.Clan;
			if (val3 == null)
			{
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("cannot_determine_proposer", "Cannot determine proposer for the election."), Colors.Red));
				return;
			}
			MarshalDecision marshalDecision = new MarshalDecision(val3, val, (List<Hero>)(object)val2);
			if (!((KingdomDecision)marshalDecision).IsAllowed())
			{
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("marshal_not_allowed", "Marshal election is not allowed at this time."), Colors.Red));
			}
			else if (UniversalDecisionHandler.HandleAddingDecision((KingdomDecision)(object)marshalDecision, val, val))
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("marshal_election_proposed", "A marshal election has been proposed for {KINGDOM}.").SetTextVariable("KINGDOM", val.Name)).ToString(), Colors.Cyan));
			}
			else
			{
				InformationManager.DisplayMessage(new InformationMessage(LanguageTranslater.L.S("marshal_election_already_pending", "A marshal election is already pending."), Colors.Yellow));
			}
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("error_proposing_marshal", "Error proposing marshal election: {EX}").SetTextVariable("EX", ex.Message)).ToString(), Colors.Red));
		}
	}

	private void OnMarshalAppointed(Kingdom kingdom, Hero marshal)
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
		if (obj == kingdom)
		{
			RefreshData();
		}
	}

	private string GetMarshalStatus(Hero marshal)
	{
		if (marshal == null)
		{
			return LanguageTranslater.L.S("Unknown", "Unknown");
		}
		try
		{
			if (marshal.IsPrisoner)
			{
				return LanguageTranslater.L.S("prisoner", "Prisoner");
			}
			MobileParty partyBelongedTo = marshal.PartyBelongedTo;
			if (partyBelongedTo != null && partyBelongedTo.IsActive)
			{
				if (partyBelongedTo.CurrentSettlement != null)
				{
					return ((object)LanguageTranslater.L.T("at_settlement", "At {SETTLEMENT}").SetTextVariable("SETTLEMENT", partyBelongedTo.CurrentSettlement.Name)).ToString();
				}
				return LanguageTranslater.L.S("active", "Active");
			}
			if (marshal.CurrentSettlement != null)
			{
				return ((object)LanguageTranslater.L.T("at_settlement", "At {SETTLEMENT}").SetTextVariable("SETTLEMENT", marshal.CurrentSettlement.Name)).ToString();
			}
			return LanguageTranslater.L.S("inactive", "Inactive");
		}
		catch (Exception)
		{
			return LanguageTranslater.L.S("Unknown", "Unknown");
		}
	}

	public override void OnFinalize()
	{
		MarshalSystemBehavior.MarshalAppointed -= OnMarshalAppointed;
		((ViewModel)this).OnFinalize();
	}
}
