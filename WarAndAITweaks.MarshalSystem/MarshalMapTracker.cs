using System;
using System.Reflection;
using SandBox.ViewModelCollection.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace WarAndAITweaks.MarshalSystem;

internal static class MarshalMapTracker
{
	private static MapMobilePartyTrackerVM _trackerVM;

	private static MethodInfo _miAddIfNotAddedParty;

	private static MethodInfo _miRemoveIfExistsParty;

	private static MobileParty _lastTrackedMarshalParty;

	private static bool _initialized;

	public static void Initialize(MapMobilePartyTrackerVM trackerVM)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		if (trackerVM == null)
		{
			return;
		}
		_trackerVM = trackerVM;
		try
		{
			Type typeFromHandle = typeof(MapMobilePartyTrackerVM);
			_miAddIfNotAddedParty = typeFromHandle.GetMethod("AddIfNotAdded", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[1] { typeof(MobileParty) }, null);
			_miRemoveIfExistsParty = typeFromHandle.GetMethod("RemoveIfExists", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[1] { typeof(MobileParty) }, null);
			_initialized = _miAddIfNotAddedParty != null && _miRemoveIfExistsParty != null;
			if (!_initialized)
			{
				InformationManager.DisplayMessage(new InformationMessage("[Marshal] Failed to bind AddIfNotAdded/RemoveIfExists.", Colors.Yellow));
			}
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage("[Marshal] Reflection init error: " + ex.Message, Colors.Red));
			_initialized = false;
		}
	}

	public static void UpdateMarshalTracking()
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		if (!_initialized || _trackerVM == null)
		{
			return;
		}
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
				RemoveCurrentMarshalTracker();
				return;
			}
			Hero marshal = MarshalSystemBehavior.GetMarshal(val);
			MobileParty val2 = ((marshal != null) ? marshal.PartyBelongedTo : null);
			if (ShouldRemoveCurrent(_lastTrackedMarshalParty, val2, val))
			{
				RemoveCurrentMarshalTracker();
			}
			if (ShouldTrack(val2, val))
			{
				_miAddIfNotAddedParty?.Invoke(_trackerVM, new object[1] { val2 });
				_lastTrackedMarshalParty = val2;
			}
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage("[Marshal] Update tracking error: " + ex.Message, Colors.Red));
		}
	}

	public static void Clear()
	{
		RemoveCurrentMarshalTracker();
	}

	private static bool ShouldTrack(MobileParty party, Kingdom playerKingdom)
	{
		return party != null && !party.IsMainParty && party != MobileParty.MainParty && party.Army == null && party.LeaderHero != null && party.MapFaction == playerKingdom;
	}

	private static bool ShouldRemoveCurrent(MobileParty lastTracked, MobileParty currentMarshalParty, Kingdom playerKingdom)
	{
		if (lastTracked == null)
		{
			return false;
		}
		return currentMarshalParty == null || currentMarshalParty != lastTracked || currentMarshalParty.Army != null || currentMarshalParty.IsMainParty || currentMarshalParty == MobileParty.MainParty || currentMarshalParty.MapFaction != playerKingdom;
	}

	private static void RemoveCurrentMarshalTracker()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		if (_lastTrackedMarshalParty == null || _trackerVM == null)
		{
			return;
		}
		try
		{
			_miRemoveIfExistsParty?.Invoke(_trackerVM, new object[1] { _lastTrackedMarshalParty });
		}
		catch (Exception ex)
		{
			InformationManager.DisplayMessage(new InformationMessage("[Marshal] Remove tracker error: " + ex.Message, Colors.Yellow));
		}
		finally
		{
			_lastTrackedMarshalParty = null;
		}
	}
}
