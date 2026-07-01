using HarmonyLib;
using SandBox.ViewModelCollection.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace WarAndAITweaks.MarshalSystem;

[HarmonyPatch(typeof(MapMobilePartyTrackerVM))]
internal static class MapMobilePartyTrackerVM_Patches
{
	[HarmonyPostfix]
	[HarmonyPatch(/*Could not decode attribute arguments.*/)]
	private static void Ctor_Postfix(MapMobilePartyTrackerVM __instance)
	{
		MarshalMapTracker.Initialize(__instance);
	}

	[HarmonyPostfix]
	[HarmonyPatch("InitList")]
	private static void InitList_Postfix()
	{
		MarshalMapTracker.UpdateMarshalTracking();
	}

	[HarmonyPostfix]
	[HarmonyPatch("Update")]
	private static void Update_Postfix()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		CampaignTime now = CampaignTime.Now;
		if ((int)((CampaignTime)(ref now)).ToHours % 1 == 0)
		{
			MarshalMapTracker.UpdateMarshalTracking();
		}
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
