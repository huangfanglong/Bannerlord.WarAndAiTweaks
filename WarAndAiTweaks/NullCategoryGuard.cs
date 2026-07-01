using HarmonyLib;
using System;
using System.IO;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace WarAndAiTweaks
{
    [HarmonyPatch(typeof(ItemRoster), "AddToCounts", new[] { typeof(EquipmentElement), typeof(int) })]
    internal static class ItemRosterGuard
    {
        [HarmonyPrefix]
        static bool Pre(EquipmentElement rosterElement, int number)
        {
            if (rosterElement.Item == null || rosterElement.Item.ItemCategory == null)
                return false;
            return true;
        }
    }
}