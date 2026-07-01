using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace WarAndAITweaks.UI;

public class FiefItemVM : ViewModel
{
	private readonly Clan _clan;

	private string _clanName;

	private ImageIdentifierVM _clanBanner;

	private string _relationText;

	private int _townCount;

	private int _castleCount;

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
	public string RelationText
	{
		get
		{
			return _relationText;
		}
		set
		{
			if (value != _relationText)
			{
				_relationText = value;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "RelationText");
			}
		}
	}

	[DataSourceProperty]
	public int TownCount
	{
		get
		{
			return _townCount;
		}
		set
		{
			if (value != _townCount)
			{
				_townCount = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "TownCount");
			}
		}
	}

	[DataSourceProperty]
	public int CastleCount
	{
		get
		{
			return _castleCount;
		}
		set
		{
			if (value != _castleCount)
			{
				_castleCount = value;
				((ViewModel)this).OnPropertyChangedWithValue(value, "CastleCount");
			}
		}
	}

	public FiefItemVM(Clan clan)
	{
		_clan = clan;
		((ViewModel)this).RefreshValues();
	}

	public override void RefreshValues()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		((ViewModel)this).RefreshValues();
		if (_clan == null)
		{
			return;
		}
		ClanName = ((object)_clan.Name)?.ToString() ?? LanguageTranslater.L.S("Unknown", "Unknown");
		try
		{
			if (_clan.Banner != null)
			{
				ClanBanner = new ImageIdentifierVM(new ImageIdentifier(_clan.Banner));
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
		float num = 0f;
		try
		{
			Hero leader = _clan.Leader;
			if (leader != null && Hero.MainHero != null)
			{
				num = Hero.MainHero.Clan.GetRelationWithClan(leader.Clan);
			}
		}
		catch
		{
			num = 0f;
		}
		RelationText = ((num >= 0f) ? $"+{num:0}" : $"{num:0}");
		try
		{
			TownCount = ((IEnumerable<Settlement>)_clan.Settlements)?.Count((Settlement s) => s.IsTown) ?? 0;
			CastleCount = ((IEnumerable<Settlement>)_clan.Settlements)?.Count((Settlement s) => s.IsCastle) ?? 0;
		}
		catch
		{
			TownCount = 0;
			CastleCount = 0;
		}
	}

	public Clan GetClan()
	{
		return _clan;
	}
}
