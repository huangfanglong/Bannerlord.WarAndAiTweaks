using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using WarAndAiTweaks.FortificationChanges;

namespace WarAndAiTweaks {

    public class WarAndAiTweaks : MBSubModuleBase {
        public static MCMSettings Settings { get; private set; }
        public static string ModName { get; private set; } = "WarAndAiTweaks";

        private bool isInitialized = false;
        private bool isLoaded = false;

        protected override void OnSubModuleLoad() {
            try {
                base.OnSubModuleLoad();
                if (isInitialized) return;
                new Harmony("Bannerlord.Windwhistle." + ModName).PatchAll();
                isInitialized = true;
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"[{ModName}] OnSubModuleLoad error: {e.Message}", Colors.Red));
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot() {
            try {
                base.OnBeforeInitialModuleScreenSetAsRoot();
                if (isLoaded) return;
                ModName = GetType().Assembly.GetName().Name;
                Settings = MCMSettings.Instance ?? throw new NullReferenceException("Settings are null");
                InformationManager.DisplayMessage(new InformationMessage($"{ModName} Loaded.", Colors.Green));
                isLoaded = true;
            } catch (Exception e) {
                InformationManager.DisplayMessage(new InformationMessage($"[{ModName}] OnBeforeInitialModuleScreenSetAsRoot error: {e.Message}", Colors.Red));
            }
        }

        protected override void OnGameStart(Game game, IGameStarter starterObject) {
            base.OnGameStart(game, starterObject);
            if (Settings != null && Settings.PartySpeedModifications)
                starterObject.AddModel(new CustomPartySpeedCalculatingModel());
        }
    }
}