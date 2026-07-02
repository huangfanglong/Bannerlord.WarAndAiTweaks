using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace WarAndAITweaks.UI;

 	public class WarAndAiTweaksManagementVM : ViewModel
 	{
 	public static Action OnCloseRequested;

 	private WarAndAiTweaksManagementState _state;

 	private WarAndAIManagementScreen _screen;

 	private string _welcomeText;

	private bool _isFeastsTabSelected;

	private bool _isMarshalTabSelected;

	private bool _isMilitaryTabSelected;

	private bool _isDiplomacyTabSelected;

	private bool _isFiefTabSelected;

	private FeastManagementVM _feastManagement;

	private MilitaryManagementVM _militaryManagement;

	private FiefManagementVM _fiefManagement;

	private MarshalManagementVM _marshalManagement;

	[DataSourceProperty]
	public string ManagementWindowTitle => LanguageTranslater.L.S("management_window_title", "WarAndAI Management");

	[DataSourceProperty]
	public string FeastsTabLabel => LanguageTranslater.L.S("feasts_tab_label", "Feasts");

	[DataSourceProperty]
	public string MarshalTabLabel => LanguageTranslater.L.S("marshal_tab_label", "Marshal");

	[DataSourceProperty]
	public string MilitaryTabLabel => LanguageTranslater.L.S("military_tab_label", "Military");

	[DataSourceProperty]
	public string FiefsTabLabel => LanguageTranslater.L.S("fiefs_tab_label", "Fiefs");

	[DataSourceProperty]
	public string CancelButtonLabel => LanguageTranslater.L.S("cancel", "Cancel");

	[DataSourceProperty]
	public string DoneButtonLabel => LanguageTranslater.L.S("done", "Done");

	[DataSourceProperty]
	public string OngoingFeastsHeader => _feastManagement?.OngoingFeastsHeader ?? "";

	[DataSourceProperty]
	public string HostFeastButton => _feastManagement?.HostFeastButton ?? "";

	[DataSourceProperty]
	public string EndFeastButton => _feastManagement?.EndFeastButton ?? "";

	[DataSourceProperty]
	public string FactionHeader => _feastManagement?.FactionHeader ?? "";

	[DataSourceProperty]
	public string HostHeader => _feastManagement?.HostHeader ?? "";

	[DataSourceProperty]
	public string LocationHeader => _feastManagement?.LocationHeader ?? "";

	[DataSourceProperty]
	public string DurationHeader => _feastManagement?.DurationHeader ?? "";

	[DataSourceProperty]
	public string AttendeesHeader => _feastManagement?.AttendeesHeader ?? "";

	[DataSourceProperty]
	public MBBindingList<FeastItemVM> ActiveFeasts => _feastManagement?.ActiveFeasts ?? new MBBindingList<FeastItemVM>();

	[DataSourceProperty]
	public string MilitaryManagementHeader => _militaryManagement?.MilitaryManagementHeader ?? "";

	[DataSourceProperty]
	public string SelectAttackTargetButton => _militaryManagement?.SelectAttackTargetButton ?? "";

	[DataSourceProperty]
	public string CurrentTargetLabel => _militaryManagement?.CurrentTargetLabel ?? "";

	[DataSourceProperty]
	public string LordHeader => _militaryManagement?.LordHeader ?? "";

	[DataSourceProperty]
	public string MilitaryClanHeader => _militaryManagement?.MilitaryClanHeader ?? "";

	[DataSourceProperty]
	public string MilitaryStatusHeader => _militaryManagement?.MilitaryStatusHeader ?? "";

	[DataSourceProperty]
	public string PartySizeHeader => _militaryManagement?.PartySizeHeader ?? "";

	[DataSourceProperty]
	public string InfluenceHeader => _militaryManagement?.InfluenceHeader ?? "";

	[DataSourceProperty]
	public string ArmyStatusHeader => _militaryManagement?.ArmyStatusHeader ?? "";

	[DataSourceProperty]
	public bool CanSelectGlobalTarget => _militaryManagement?.CanSelectGlobalTarget ?? false;

	[DataSourceProperty]
	public MBBindingList<AiLordItemVM> AiLords => _militaryManagement?.AiLords ?? new MBBindingList<AiLordItemVM>();

	[DataSourceProperty]
	public string SelectedAttackTarget => _militaryManagement?.SelectedAttackTarget ?? "";

	[DataSourceProperty]
	public string FiefsManagementHeader => _fiefManagement?.FiefsManagementHeader ?? "";

	[DataSourceProperty]
	public string GrantFiefButton => _fiefManagement?.GrantFiefButton ?? "";

	[DataSourceProperty]
	public string RevokeFiefButton => _fiefManagement?.RevokeFiefButton ?? "";

	[DataSourceProperty]
	public string ClanHeader => _fiefManagement?.ClanHeader ?? "";

	[DataSourceProperty]
	public string RelationHeader => _fiefManagement?.RelationHeader ?? "";

	[DataSourceProperty]
	public string TownsHeader => _fiefManagement?.TownsHeader ?? "";

	[DataSourceProperty]
	public string CastlesHeader => _fiefManagement?.CastlesHeader ?? "";

	[DataSourceProperty]
	public bool CanManageFiefs => _fiefManagement?.CanManageFiefs ?? false;

	[DataSourceProperty]
	public MBBindingList<FiefItemVM> FiefClans => _fiefManagement?.FiefClans ?? new MBBindingList<FiefItemVM>();

	[DataSourceProperty]
	public string MarshalManagementHeader => _marshalManagement?.MarshalManagementHeader ?? "";

	[DataSourceProperty]
	public string AppointMarshalButton => _marshalManagement?.AppointMarshalButton ?? "";

	[DataSourceProperty]
	public string CurrentMarshalHeader => _marshalManagement?.CurrentMarshalHeader ?? "";

	[DataSourceProperty]
	public string NameHeader => _marshalManagement?.NameHeader ?? "";

	[DataSourceProperty]
	public string MarshalClanHeader => _marshalManagement?.MarshalClanHeader ?? "";

	[DataSourceProperty]
	public string SkillHeader => _marshalManagement?.SkillHeader ?? "";

	[DataSourceProperty]
	public string MarshalStatusHeader => _marshalManagement?.MarshalStatusHeader ?? "";

	[DataSourceProperty]
	public string CurrentMarshalName => _marshalManagement?.CurrentMarshalName ?? "";

	[DataSourceProperty]
	public string CurrentMarshalClan => _marshalManagement?.CurrentMarshalClan ?? "";

	[DataSourceProperty]
	public string CurrentMarshalSkill => _marshalManagement?.CurrentMarshalSkill ?? "";

	[DataSourceProperty]
	public string CurrentMarshalStatus => _marshalManagement?.CurrentMarshalStatus ?? "";

	[DataSourceProperty]
	public string MarshalStatus => _marshalManagement?.MarshalStatus ?? "";

	[DataSourceProperty]
	public bool CanSelectMarshal => _marshalManagement?.CanSelectMarshal ?? false;

	[DataSourceProperty]
	public ImageIdentifierVM CurrentMarshalBanner => _marshalManagement?.CurrentMarshalBanner;

	[DataSourceProperty]
	public string WelcomeText
	{
		get
		{
			return _welcomeText;
		}
		set
		{
			if (value != _welcomeText)
			{
				_welcomeText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "WelcomeText");
			}
		}
	}

	[DataSourceProperty]
	public bool IsFeastsTabSelected
	{
		get
		{
			return _isFeastsTabSelected;
		}
		set
		{
			if (value != _isFeastsTabSelected)
			{
				_isFeastsTabSelected = value;
				base.OnPropertyChangedWithValue(value, "IsFeastsTabSelected");
			}
		}
	}

	[DataSourceProperty]
	public bool IsMarshalTabSelected
	{
		get
		{
			return _isMarshalTabSelected;
		}
		set
		{
			if (value != _isMarshalTabSelected)
			{
				_isMarshalTabSelected = value;
				base.OnPropertyChangedWithValue(value, "IsMarshalTabSelected");
			}
		}
	}

	[DataSourceProperty]
	public bool IsMilitaryTabSelected
	{
		get
		{
			return _isMilitaryTabSelected;
		}
		set
		{
			if (value != _isMilitaryTabSelected)
			{
				_isMilitaryTabSelected = value;
				base.OnPropertyChangedWithValue(value, "IsMilitaryTabSelected");
			}
		}
	}

	[DataSourceProperty]
	public bool IsDiplomacyTabSelected
	{
		get
		{
			return _isDiplomacyTabSelected;
		}
		set
		{
			if (value != _isDiplomacyTabSelected)
			{
				_isDiplomacyTabSelected = value;
				base.OnPropertyChangedWithValue(value, "IsDiplomacyTabSelected");
			}
		}
	}

	[DataSourceProperty]
	public bool IsFiefTabSelected
	{
		get
		{
			return _isFiefTabSelected;
		}
		set
		{
			if (value != _isFiefTabSelected)
			{
				_isFiefTabSelected = value;
				base.OnPropertyChangedWithValue(value, "IsFiefTabSelected");
			}
		}
	}

	[DataSourceProperty]
	public FeastManagementVM FeastManagement
	{
		get
		{
			return _feastManagement;
		}
		set
		{
			if (value != _feastManagement)
			{
				_feastManagement = value;
				((ViewModel)this).OnPropertyChangedWithValue<FeastManagementVM>(value, "FeastManagement");
			}
		}
	}

	[DataSourceProperty]
	public MilitaryManagementVM MilitaryManagement
	{
		get
		{
			return _militaryManagement;
		}
		set
		{
			if (value != _militaryManagement)
			{
				_militaryManagement = value;
				((ViewModel)this).OnPropertyChangedWithValue<MilitaryManagementVM>(value, "MilitaryManagement");
			}
		}
	}

	[DataSourceProperty]
	public FiefManagementVM FiefManagement
	{
		get
		{
			return _fiefManagement;
		}
		set
		{
			if (value != _fiefManagement)
			{
				_fiefManagement = value;
				((ViewModel)this).OnPropertyChangedWithValue<FiefManagementVM>(value, "FiefManagement");
			}
		}
	}

	[DataSourceProperty]
	public MarshalManagementVM MarshalManagement
	{
		get
		{
			return _marshalManagement;
		}
		set
		{
			if (value != _marshalManagement)
			{
				_marshalManagement = value;
				((ViewModel)this).OnPropertyChangedWithValue<MarshalManagementVM>(value, "MarshalManagement");
			}
		}
	}

	public void ExecuteHostFeast()
	{
		_feastManagement?.ExecuteHostFeast();
	}

	public void ExecuteEndFeast()
	{
		_feastManagement?.ExecuteEndFeast();
	}

	public void ExecuteSelectGlobalTarget()
	{
		_militaryManagement?.ExecuteSelectGlobalTarget();
	}

	public void ExecuteGrantFiefTitle()
	{
		_fiefManagement?.ExecuteGrantFiefTitle();
	}

	public void ExecuteRevokeFiefTitle()
	{
		_fiefManagement?.ExecuteRevokeFiefTitle();
	}

	public void ExecuteAppointMarshal()
	{
		_marshalManagement?.ExecuteAppointMarshal();
	}

	public void ExecuteSelectFeastsTab()
	{
		IsFeastsTabSelected = true;
		IsMarshalTabSelected = false;
		IsMilitaryTabSelected = false;
		IsDiplomacyTabSelected = false;
		IsFiefTabSelected = false;
		FeastManagement?.RefreshData();
	}

	public void ExecuteSelectMarshalTab()
	{
		IsFeastsTabSelected = false;
		IsMarshalTabSelected = true;
		IsMilitaryTabSelected = false;
		IsDiplomacyTabSelected = false;
		IsFiefTabSelected = false;
		MarshalManagement?.RefreshData();
	}

	public void ExecuteSelectMilitaryTab()
	{
		IsFeastsTabSelected = false;
		IsMarshalTabSelected = false;
		IsMilitaryTabSelected = true;
		IsDiplomacyTabSelected = false;
		IsFiefTabSelected = false;
		MilitaryManagement?.RefreshData();
	}

	public void ExecuteSelectFief()
	{
		IsFeastsTabSelected = false;
		IsMarshalTabSelected = false;
		IsMilitaryTabSelected = false;
		IsDiplomacyTabSelected = false;
		IsFiefTabSelected = true;
		FiefManagement?.RefreshData();
	}

	public void ExecuteSelectDiplomacyTab()
	{
		IsFeastsTabSelected = false;
		IsMarshalTabSelected = false;
		IsMilitaryTabSelected = false;
		IsDiplomacyTabSelected = true;
		IsFiefTabSelected = false;
	}

  public WarAndAiTweaksManagementVM() : this(null, null)
   {
   }

  public WarAndAiTweaksManagementVM(WarAndAiTweaksManagementState state, WarAndAIManagementScreen screen)
   {
    _state = state;
    _screen = screen;
    RefreshWelcomeText();
    _feastManagement = new FeastManagementVM();
    _militaryManagement = new MilitaryManagementVM();
    _fiefManagement = new FiefManagementVM();
    _marshalManagement = new MarshalManagementVM();
    IsFeastsTabSelected = false;
    IsMarshalTabSelected = false;
    IsMilitaryTabSelected = true;
    IsFiefTabSelected = false;
   }

	public override void OnFinalize()
	{
		FeastManagementVM feastManagement = _feastManagement;
		if (feastManagement != null)
		{
			((ViewModel)feastManagement).OnFinalize();
		}
		MilitaryManagementVM militaryManagement = _militaryManagement;
		if (militaryManagement != null)
		{
			((ViewModel)militaryManagement).OnFinalize();
		}
		FiefManagementVM fiefManagement = _fiefManagement;
		if (fiefManagement != null)
		{
			((ViewModel)fiefManagement).OnFinalize();
		}
		MarshalManagementVM marshalManagement = _marshalManagement;
		if (marshalManagement != null)
		{
			((ViewModel)marshalManagement).OnFinalize();
		}
		base.OnFinalize();
	}

	private void RefreshWelcomeText()
	{
		Hero mainHero = Hero.MainHero;
		string text = ((mainHero == null) ? null : ((object)mainHero.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		WelcomeText = "War & AI Tweaks - " + text;
	}

	public void ExecuteCancel()
	{
		OnCloseRequested?.Invoke();
	}

	public void ExecuteDone()
	{
		OnCloseRequested?.Invoke();
	}
}