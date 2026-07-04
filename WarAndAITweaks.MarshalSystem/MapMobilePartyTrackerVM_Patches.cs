using HarmonyLib;
using SandBox.ViewModelCollection.Map.Tracker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace WarAndAITweaks.MarshalSystem;

[HarmonyPatch(typeof(MapTrackerProvider))]
internal static class MapMobilePartyTrackerVM_Patches
{
	[HarmonyPostfix]
	[HarmonyPatch(MethodType.Constructor)]
	private static void Ctor_Postfix(MapTrackerProvider __instance)
	{
		MarshalMapTracker.Initialize(__instance);
		MarshalMapTracker.UpdateMarshalTracking();
	}

	[HarmonyPostfix]
	[HarmonyPatch("ResetTrackers")]
	private static void ResetTrackers_Postfix()
	{
		MarshalMapTracker.UpdateMarshalTracking();
	}

	[HarmonyPostfix]
	[HarmonyPatch("GetTrackers")]
	private static void GetTrackers_Postfix()
	{
		MarshalMapTracker.UpdateMarshalTracking();
	}

	[HarmonyPostfix]
	[HarmonyPatch("OnClanChangedKingdom")]
	private static void OnClanChangedKingdom_Postfix(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomActionDetail detail, bool showNotification)
	{
		if (clan == Clan.PlayerClan)
		{
			MarshalMapTracker.Clear();
			MarshalMapTracker.UpdateMarshalTracking();
		}
	}
}
