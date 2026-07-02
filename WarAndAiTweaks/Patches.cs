using HarmonyLib;
using System;
using System.IO;
using System.Reflection;

namespace WarAndAiTweaks.Patches;

[HarmonyPatch]
public class Patch_MapScreen_OnFocusChangeOnGameWindow
{
    static void Log(string msg)
    {
        try { File.AppendAllText(@"C:\ProgramData\Mount and Blade II Bannerlord\logs\wai_debug.log", $"{DateTime.Now:HH:mm:ss.fff} [Patch] {msg}\n"); } catch { }
    }

    static MethodBase TargetMethod()
    {
        var type = AccessTools.TypeByName("SandBox.View.Map.MapScreen");
        Log("TargetMethod: type=" + (type?.FullName ?? "null"));
        if (type == null) return null;
        return AccessTools.Method(type, "OnFocusChangeOnGameWindow", new[] { typeof(bool) });
    }

    static bool Prefix(bool focusGained)
    {
        Log($"Prefix focusGained={focusGained} IsUIActive={SubModule.IsUIActive}");
        if (focusGained) return true;
        if (SubModule.IsUIActive) return false;
        return true;
    }
}
