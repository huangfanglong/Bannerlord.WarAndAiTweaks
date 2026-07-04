using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace WarAndAITweaks.UI;

public class AiLordItemVM : ViewModel
{
	private Hero _hero;

	private string _lordName;

	private string _clanName;

	private string _status;

	private int _partySize;

	private int _influence;

	private bool _allowArmyCreation;

	private bool _canToggleArmyCreation;

	private string _armyStatusText;

	private ImageIdentifierVM _clanBanner;

	[DataSourceProperty]
	public string ToggleArmyCreationButton => LanguageTranslater.L.S("military_toggle_army_creation_button", "Toggle Army Creation");

	[DataSourceProperty]
	public string LordName
	{
		get
		{
			return _lordName;
		}
		set
		{
			if (value != _lordName)
			{
				_lordName = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "LordName");
			}
		}
	}

	[DataSourceProperty]
	public string ClanName
	{
		get
		{
			return _clanName;
		}
		set
		{
			if (value != _clanName)
			{
				_clanName = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "ClanName");
			}
		}
	}

	[DataSourceProperty]
	public string Status
	{
		get
		{
			return _status;
		}
		set
		{
			if (value != _status)
			{
				_status = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "Status");
			}
		}
	}

	[DataSourceProperty]
	public int PartySize
	{
		get
		{
			return _partySize;
		}
		set
		{
			if (value != _partySize)
			{
				_partySize = value;
				base.OnPropertyChangedWithValue(value, "PartySize");
			}
		}
	}

	[DataSourceProperty]
	public int Influence
	{
		get
		{
			return _influence;
		}
		set
		{
			if (value != _influence)
			{
				_influence = value;
				base.OnPropertyChangedWithValue(value, "Influence");
			}
		}
	}

	[DataSourceProperty]
	public bool AllowArmyCreation
	{
		get
		{
			return _allowArmyCreation;
		}
		set
		{
			if (value != _allowArmyCreation)
			{
				_allowArmyCreation = value;
				base.OnPropertyChangedWithValue(value, "AllowArmyCreation");
				ArmyStatusText = (value ? LanguageTranslater.L.S("army_status_enabled", "Enabled") : LanguageTranslater.L.S("army_status_disabled", "Disabled"));
			}
		}
	}

	[DataSourceProperty]
	public string ArmyStatusText
	{
		get
		{
			return _armyStatusText;
		}
		set
		{
			if (value != _armyStatusText)
			{
				_armyStatusText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "ArmyStatusText");
			}
		}
	}

	[DataSourceProperty]
	public bool CanToggleArmyCreation
	{
		get
		{
			return _canToggleArmyCreation;
		}
		set
		{
			if (value != _canToggleArmyCreation)
			{
				_canToggleArmyCreation = value;
				base.OnPropertyChangedWithValue(value, "CanToggleArmyCreation");
			}
		}
	}

	[DataSourceProperty]
	public ImageIdentifierVM ClanBanner
	{
		get
		{
			return _clanBanner;
		}
		set
		{
			if (value != _clanBanner)
			{
				_clanBanner = value;
				((ViewModel)this).OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
			}
		}
	}

	public AiLordItemVM(Hero hero)
	{
		_hero = hero;
		RefreshValues();
	}

	public void RefreshValues()
	{
		if (_hero == null)
		{
			return;
		}
		LordName = ((object)_hero.Name)?.ToString() ?? LanguageTranslater.L.S("Unknown", "Unknown");
		Clan clan = _hero.Clan;
		ClanName = ((clan == null) ? null : ((object)clan.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		try
		{
			Clan clan2 = _hero.Clan;
			if (((clan2 != null) ? clan2.Banner : null) != null)
			{
				ClanBanner = new BannerImageIdentifierVM(_hero.Clan.Banner);
			}
			else
			{
				ClanBanner = null;
			}
		}
		catch
		{
			ClanBanner = null;
		}
		MobileParty partyBelongedTo = _hero.PartyBelongedTo;
		int? obj2;
		if (partyBelongedTo == null)
		{
			obj2 = null;
		}
		else
		{
			PartyBase party = partyBelongedTo.Party;
			obj2 = ((party != null) ? new int?(party.NumberOfAllMembers) : null);
		}
		int? num = obj2;
		PartySize = num.GetValueOrDefault();
		Clan clan3 = _hero.Clan;
		Influence = (int)((clan3 != null) ? clan3.Influence : 0f);
		Status = GetLordStatus(_hero);
		CanToggleArmyCreation = IsPlayerKingdomRuler();
		ArmyStatusText = (AllowArmyCreation ? LanguageTranslater.L.S("army_status_enabled", "Enabled") : LanguageTranslater.L.S("army_status_disabled", "Disabled"));
	}

	private bool IsPlayerKingdomRuler()
	{
		return Hero.MainHero.Clan.Kingdom != null && Hero.MainHero == Hero.MainHero.Clan.Kingdom.Leader;
	}

	public void ExecuteToggleArmyCreation()
	{
		if (CanToggleArmyCreation)
		{
			AllowArmyCreation = !AllowArmyCreation;
			base.OnPropertyChangedWithValue(AllowArmyCreation, "AllowArmyCreation");
			string id = (AllowArmyCreation ? "ailord_army_enabled" : "ailord_army_disabled");
			string fallback = (AllowArmyCreation ? "{HERO} can now form armies" : "{HERO} is no longer allowed to form armies");
			string text = ((object)LanguageTranslater.L.T(id, fallback).SetTextVariable("HERO", ((object)_hero.Name).ToString())).ToString();
			InformationManager.DisplayMessage(new InformationMessage(text, AllowArmyCreation ? Colors.Green : Colors.Red));
		}
	}

	private string GetLordStatus(Hero hero)
	{
		if (hero.IsPrisoner)
		{
			return LanguageTranslater.L.S("prisoner", "Prisoner");
		}
		MobileParty partyBelongedTo = hero.PartyBelongedTo;
		if (partyBelongedTo != null && partyBelongedTo.IsActive)
		{
			if (partyBelongedTo.CurrentSettlement != null)
			{
				return ((object)LanguageTranslater.L.T("at_settlement", "At {SETTLEMENT}").SetTextVariable("SETTLEMENT", partyBelongedTo.CurrentSettlement.Name)).ToString();
			}
			return LanguageTranslater.L.S("active", "Active");
		}
		if (hero.CurrentSettlement != null)
		{
			return ((object)LanguageTranslater.L.T("at_settlement", "At {SETTLEMENT}").SetTextVariable("SETTLEMENT", hero.CurrentSettlement.Name)).ToString();
		}
		return LanguageTranslater.L.S("inactive", "Inactive");
	}

	public Hero GetHero()
	{
		return _hero;
	}
}


