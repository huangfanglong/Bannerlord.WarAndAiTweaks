using HarmonyLib;
using System.Reflection;

namespace WarAndAiTweaks.Patches;

[HarmonyPatch]
public class Patch_MapScreen_OnFocusChangeOnGameWindow
{
    static MethodBase TargetMethod()
    {
        var type = AccessTools.TypeByName("SandBox.View.Map.MapScreen");
        if (type == null) return null;
        return AccessTools.Method(type, "OnFocusChangeOnGameWindow", new[] { typeof(bool) });
    }

    static bool Prefix(bool focusGained)
    {
        if (focusGained) return true;
        if (SubModule.IsUIActive) return false;
        return true;
    }
}
