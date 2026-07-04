using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace WarAndAiTweaks
{
    [HarmonyPatch(typeof(TownMarketData), "AddNumberInStore")]
    internal static class NullCategoryGuard
    {
        [HarmonyPrefix]
        static bool Prefix(ItemCategory itemCategory, int number, int value)
        {
            return itemCategory != null;
        }
    }
}