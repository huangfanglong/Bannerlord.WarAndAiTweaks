using System;
using System.Reflection;
using SandBox.ViewModelCollection.Map.Tracker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.Tracker;
using TaleWorlds.Library;

namespace WarAndAITweaks.MarshalSystem;

internal static class MarshalMapTracker
{
	private static MapTrackerProvider _trackerProvider;

	private static object _trackerContainer;

	private static MethodInfo _miHasTrackerFor;

	private static MethodInfo _miGetTrackerFor;

	private static MethodInfo _miAddTracker;

	private static MethodInfo _miRemoveTracker;

	private static MobileParty _lastTrackedMarshalParty;

	private static bool _initialized;

	public static void Initialize(MapTrackerProvider trackerProvider)
	{
		if (trackerProvider == null)
		{
			return;
		}
		_trackerProvider = trackerProvider;
		try
		{
			FieldInfo containerField = typeof(MapTrackerProvider).GetField("_trackerContainer", BindingFlags.Instance | BindingFlags.NonPublic);
			_trackerContainer = containerField?.GetValue(trackerProvider);
			Type containerType = _trackerContainer?.GetType();
			_miHasTrackerFor = containerType?.GetMethod("HasTrackerFor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(ITrackableCampaignObject) }, null);
			_miGetTrackerFor = containerType?.GetMethod("GetTrackerFor", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(ITrackableCampaignObject) }, null);
			_miAddTracker = containerType?.GetMethod("AddTracker", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(MapTrackerItemVM) }, null);
			_miRemoveTracker = containerType?.GetMethod("RemoveTracker", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { typeof(MapTrackerItemVM) }, null);
			_initialized = _trackerContainer != null && _miHasTrackerFor != null && _miGetTrackerFor != null && _miAddTracker != null && _miRemoveTracker != null;
			if (!_initialized)
			{
				InformationManager.DisplayMessage(new InformationMessage("[Marshal] Failed to bind MapTrackerProvider container.", Colors.Yellow));
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
		if (!_initialized || _trackerProvider == null)
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
				AddMarshalTracker(val2);
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

	private static void AddMarshalTracker(MobileParty party)
	{
		if (party == null || HasTrackerFor(party))
		{
			return;
		}
		_miAddTracker.Invoke(_trackerContainer, new object[1] { new MapMobilePartyTrackItemVM(party) });
	}

	private static bool HasTrackerFor(MobileParty party)
	{
		object result = _miHasTrackerFor.Invoke(_trackerContainer, new object[1] { (ITrackableCampaignObject)(object)party });
		return result is bool value && value;
	}

	private static void RemoveCurrentMarshalTracker()
	{
		if (_lastTrackedMarshalParty == null || _trackerContainer == null)
		{
			return;
		}
		try
		{
			object tracker = _miGetTrackerFor.Invoke(_trackerContainer, new object[1] { (ITrackableCampaignObject)(object)_lastTrackedMarshalParty });
			if (tracker is MapTrackerItemVM trackerVM)
			{
				_miRemoveTracker.Invoke(_trackerContainer, new object[1] { trackerVM });
			}
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
