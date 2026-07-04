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

	[DataSourceProperty]
	public string ClanName { get; private set; }

	[DataSourceProperty]
	public ImageIdentifierVM ClanBanner { get; private set; }

	[DataSourceProperty]
	public string RelationText { get; private set; }

	[DataSourceProperty]
	public int TownCount { get; private set; }

	[DataSourceProperty]
	public int CastleCount { get; private set; }

	public FiefItemVM(Clan clan)
	{
		_clan = clan;
		if (clan == null)
			return;
		ClanName = ((object)clan.Name)?.ToString() ?? LanguageTranslater.L.S("Unknown", "Unknown");
		try
		{
			ClanBanner = new BannerImageIdentifierVM(clan.Banner);
		}
		catch
		{
			ClanBanner = null;
		}
		float num = 0f;
		try
		{
			Hero leader = clan.Leader;
			if (leader != null && Hero.MainHero != null)
				num = Hero.MainHero.Clan.GetRelationWithClan(leader.Clan);
		}
		catch
		{
			num = 0f;
		}
		RelationText = ((num >= 0f) ? $"+{num:0}" : $"{num:0}");
		try
		{
			TownCount = ((IEnumerable<Settlement>)clan.Settlements)?.Count((Settlement s) => s.IsTown) ?? 0;
			CastleCount = ((IEnumerable<Settlement>)clan.Settlements)?.Count((Settlement s) => s.IsCastle) ?? 0;
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
