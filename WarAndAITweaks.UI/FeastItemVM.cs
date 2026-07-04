using System.Collections.Generic;
using System.Linq;
using FeastSystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace WarAndAITweaks.UI;

public class FeastItemVM : ViewModel
{
	private string _hostName;

	private string _settlementName;

	private string _factionName;

	private string _duration;

	private int _attendeeCount;

	private string _attendeeList;

	private ImageIdentifierVM _factionBanner;

	private ImageIdentifierVM _clanBanner;

	private string _clanName;

	[DataSourceProperty]
	public string HostName
	{
		get
		{
			return _hostName;
		}
		set
		{
			if (value != _hostName)
			{
				_hostName = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "HostName");
			}
		}
	}

	[DataSourceProperty]
	public string SettlementName
	{
		get
		{
			return _settlementName;
		}
		set
		{
			if (value != _settlementName)
			{
				_settlementName = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "SettlementName");
			}
		}
	}

	[DataSourceProperty]
	public string FactionName
	{
		get
		{
			return _factionName;
		}
		set
		{
			if (value != _factionName)
			{
				_factionName = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "FactionName");
			}
		}
	}

	[DataSourceProperty]
	public string Duration
	{
		get
		{
			return _duration;
		}
		set
		{
			if (value != _duration)
			{
				_duration = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "Duration");
			}
		}
	}

	[DataSourceProperty]
	public int AttendeeCount
	{
		get
		{
			return _attendeeCount;
		}
		set
		{
			if (value != _attendeeCount)
			{
				_attendeeCount = value;
				base.OnPropertyChangedWithValue(value, "AttendeeCount");
			}
		}
	}

	[DataSourceProperty]
	public string AttendeeList
	{
		get
		{
			return _attendeeList;
		}
		set
		{
			if (value != _attendeeList)
			{
				_attendeeList = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "AttendeeList");
			}
		}
	}

	[DataSourceProperty]
	public ImageIdentifierVM FactionBanner
	{
		get
		{
			return _factionBanner;
		}
		set
		{
			if (value != _factionBanner)
			{
				_factionBanner = value;
				((ViewModel)this).OnPropertyChangedWithValue<ImageIdentifierVM>(value, "FactionBanner");
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

	public FeastItemVM(FeastData feast)
	{
		Hero host = feast.Host;
		HostName = ((host == null) ? null : ((object)host.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		Settlement settlement = feast.Settlement;
		SettlementName = ((settlement == null) ? null : ((object)settlement.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		Hero host2 = feast.Host;
		Clan val = ((host2 != null) ? host2.Clan : null);
		ClanName = ((val == null) ? null : ((object)val.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		Kingdom val2 = ((val != null) ? val.Kingdom : null);
		FactionName = ((val2 == null) ? null : ((object)val2.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		try
		{
			if (((val2 != null) ? val2.Banner : null) != null)
			{
				FactionBanner = new BannerImageIdentifierVM(val2.Banner);
			}
		}
		catch
		{
			FactionBanner = null;
		}
		try
		{
			if (((val != null) ? val.Banner : null) != null)
			{
				ClanBanner = new BannerImageIdentifierVM(val.Banner);
			}
		}
		catch
		{
			ClanBanner = null;
		}
		AttendeeCount = ((List<Hero>)(object)feast.Attendees)?.Count ?? 0;
		CampaignTime val3 = CampaignTime.Now - feast.StartTime;
		int num = (int)val3.ToDays;
		Duration = ((num == 0) ? LanguageTranslater.L.S("feast_duration_just_started", "Just started") : ((object)LanguageTranslater.L.T("feast_duration_days", "{DAYS} day(s)").SetTextVariable("DAYS", num.ToString())).ToString());
		if (feast.Attendees != null && ((List<Hero>)(object)feast.Attendees).Count > 0)
		{
			List<string> list = (from attendee in (IEnumerable<Hero>)feast.Attendees
				where attendee != null && !string.IsNullOrEmpty(((object)attendee.Name)?.ToString())
				select ((object)attendee.Name).ToString()).ToList();
			AttendeeList = ((list.Count > 0) ? string.Join(", ", list) : LanguageTranslater.L.S("feast_no_attendees", "No attendees yet"));
		}
		else
		{
			AttendeeList = LanguageTranslater.L.S("feast_no_attendees", "No attendees yet");
		}
	}
}



