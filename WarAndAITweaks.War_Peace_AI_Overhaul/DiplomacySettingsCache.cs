using TaleWorlds.CampaignSystem;

namespace WarAndAITweaks.War_Peace_AI_Overhaul;

public static class DiplomacySettingsCache
{
	private static bool _isInitialized;

	private static bool _isDiplomacyModLoaded;

	private static bool? _noWarBetweenFriends;

	private static bool? _noWarOnGoodRelations;

	private static int? _noWarOnGoodRelationsThreshold;

	private static bool? _noWarWhenMarriedLeaderClans;

	private static CampaignTime _lastUpdate;

	public static bool IsDiplomacyModLoaded => _isDiplomacyModLoaded;

	public static bool? NoWarBetweenFriends => _noWarBetweenFriends;

	public static bool? NoWarOnGoodRelations => _noWarOnGoodRelations;

	public static int? NoWarOnGoodRelationsThreshold => _noWarOnGoodRelationsThreshold;

	public static bool? NoWarWhenMarriedLeaderClans => _noWarWhenMarriedLeaderClans;

	public static void Initialize()
	{
		_isDiplomacyModLoaded = DiplomacyPlugin.IsDiplomacyModLoaded();
		_isInitialized = true;
		if (_isDiplomacyModLoaded)
		{
			EnsureInitialized();
		}
	}

	public static void EnsureInitialized()
	{
		if (!_isDiplomacyModLoaded)
		{
			return;
		}
		if (_isInitialized)
		{
			_ = _lastUpdate;
			CampaignTime now = CampaignTime.Now;
			if (!(now.ToDays - _lastUpdate.ToDays > 5.0))
			{
				return;
			}
		}
		_noWarBetweenFriends = DiplomacyPlugin.GetNoWarBetweenFriends();
		_noWarOnGoodRelations = DiplomacyPlugin.GetNoWarOnGoodRelations();
		_noWarOnGoodRelationsThreshold = DiplomacyPlugin.GetNoWarOnGoodRelationsThreshold();
		_noWarWhenMarriedLeaderClans = DiplomacyPlugin.GetNoWarWhenMarriedLeaderClans();
		_lastUpdate = CampaignTime.Now;
		_isInitialized = true;
	}
}


