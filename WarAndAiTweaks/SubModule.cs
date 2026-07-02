using System;
using System.Linq;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;
using Bannerlord.UIExtenderEx;
using ClanRespawn;
using FeastSystem;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using Strategic4XDiplomacy;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using WarAndAITweaks.Autobuild;
using WarAndAITweaks.BattleChatter;
using WarAndAITweaks.Culture;
using WarAndAITweaks.MarshalSystem;
using WarAndAITweaks.MilitaryAI;
using WarAndAITweaks.ModelOverrides;
using WarAndAITweaks.TodayWeFeast;
using WarAndAITweaks.UI;
using WarAndAiTweaks.Companion;

namespace WarAndAiTweaks;

public class SubModule : MBSubModuleBase
{
	public static readonly string Name = typeof(SubModule).Namespace;

	public static bool IsUIActive;

	private GauntletLayer _waiLayer;

	private WarAndAiTweaksManagementVM _waiVM;

	private bool _isUIOpen;

	protected override void OnSubModuleLoad()
	{
		UIExtender val = new UIExtender("WarAndAiTweaks.DiplomacyUI");
		val.Register(typeof(SubModule).Assembly);
		val.Enable();
		WarAndAiTweaksManagementVM.OnCloseRequested = CloseWaiOverlay;
	}

	public override void OnGameInitializationFinished(Game game)
	{
		try
		{
			string selectedValue = GlobalSettings<WarAndAiTweaksSettings>.Instance.ManagementUIHotkey.SelectedValue;
			InformationManager.DisplayMessage(new InformationMessage("[War & AI Tweaks] Press ALT + " + selectedValue.ToUpperInvariant() + " to open the Management UI.", Colors.Cyan));
		}
		catch (Exception)
		{
		}
	}

	private void SafePatchAll(Harmony harmony)
	{
		foreach (var type in typeof(SubModule).Assembly.GetTypes())
		{
			try
			{
				var attr = type.GetCustomAttributes(typeof(HarmonyPatch), false);
				if (attr.Length > 0)
				{
					var processor = harmony.CreateClassProcessor(type);
					processor.Patch();
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage($"[WAI] Failed to patch {type.FullName}: {ex.Message}", Colors.Red));
			}
		}
	}

	protected override void OnGameStart(Game game, IGameStarter gameStarter)
	{
		Harmony val = new Harmony("mod.octavius.bannerlord");
		SafePatchAll(val);
		if (game.GameType is Campaign)
		{
			CampaignGameStarter val2 = (CampaignGameStarter)gameStarter;
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableCultureChange)
			{
				val2.AddBehavior((CampaignBehaviorBase)(object)new SettlementCultureChangerBehavior());
			}
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableMarshalSystem)
			{
				val2.AddBehavior((CampaignBehaviorBase)(object)new MarshalSystemBehavior());
			}
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableWarPeaceAIOverhaul)
			{
				val2.AddBehavior((CampaignBehaviorBase)(object)new Strategic4XDiplomacyBehavior());
			}
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableTodayWeFeast)
			{
				val2.AddBehavior((CampaignBehaviorBase)(object)new FeastBehavior());
			}
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableClanRespawn)
			{
				val2.AddBehavior((CampaignBehaviorBase)(object)new ClanRespawnBehavior());
			}
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableAutoBuilder)
			{
				val2.AddBehavior((CampaignBehaviorBase)(object)new AutoBuildBehavior());
			}
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableFieflessClansLoss)
			{
				val2.AddBehavior((CampaignBehaviorBase)(object)new FiefRelationshipBehavior());
			}
			if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableEnhancedMilitary)
			{
				val2.AddBehavior((CampaignBehaviorBase)(object)new EnhancedAiMilitaryBehavior());
			}
			val2.AddModel((GameModel)(object)new DelegatingPartySpeedModel());
			val2.AddModel((GameModel)(object)new DelegatingPartySizeLimitModel());
			val2.AddModel((GameModel)(object)new DelegatingBuildingEffectModel());
			val2.AddModel((GameModel)(object)new DelegatingSettlementFoodModel());
			val2.AddModel((GameModel)(object)new DelegatingArmyManagementCalculationModel());
			val2.AddModel((GameModel)(object)new DelegatingPartyWageModel());
			val2.AddModel((GameModel)(object)new DelegatingSettlementTaxModel());
		}
	}

 	protected override void OnApplicationTick(float dt)
 	{
 		try
 		{
 			if (Campaign.Current == null || ScreenManager.TopScreen == null)
 			{
 				return;
 			}

 			if ((Input.IsKeyDown(InputKey.LeftAlt) || Input.IsKeyDown(InputKey.RightAlt)) && Input.IsKeyPressed(InputKey.Q))
 			{
 				if (_isUIOpen)
 				{
 					CloseWaiOverlay();
 				}
 				else
 				{
 					OpenWaiOverlay();
 				}
 			}

 			if (Input.IsKeyPressed(InputKey.Escape) && _isUIOpen)
 			{
 				CloseWaiOverlay();
 			}
 		}
 		catch (Exception ex)
 		{
 			InformationManager.DisplayMessage(new InformationMessage($"[WAI] {ex.Message}", Colors.Red));
 		}
 	}

 	private void OpenWaiOverlay()
 	{
 		try
 		{
 			_waiVM = new WarAndAiTweaksManagementVM();
 			_waiLayer = new GauntletLayer("WaiOverlay", 10000, false);
 			_waiLayer.LoadMovie("WarAndAITweaksManagement", (ViewModel)(object)_waiVM);
 			((ScreenLayer)_waiLayer).IsFocusLayer = true;
 			((ScreenLayer)_waiLayer).InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
 			ScreenManager.TrySetFocus((ScreenLayer)(object)_waiLayer);
 			ScreenManager.TopScreen.AddLayer((ScreenLayer)(object)_waiLayer);
 			IsUIActive = true;
 			_isUIOpen = true;
 		}
 		catch (Exception ex)
 		{
 			InformationManager.DisplayMessage(new InformationMessage($"[WAI] OpenWaiOverlay ERROR: {ex.Message}", Colors.Red));
 		}
 	}

 	private void CloseWaiOverlay()
 	{
 		try
 		{
 			IsUIActive = false;
 			if (_waiLayer != null)
 			{
 				ScreenManager.TryLoseFocus((ScreenLayer)(object)_waiLayer);
 				ScreenManager.TopScreen.RemoveLayer((ScreenLayer)(object)_waiLayer);
 				_waiLayer = null;
 			}
 			_waiVM = null;
 			_isUIOpen = false;
 		}
 		catch (Exception ex)
 		{
 			InformationManager.DisplayMessage(new InformationMessage($"[WAI] CloseWaiOverlay ERROR: {ex.Message}", Colors.Red));
 		}
 	}

	public override void OnMissionBehaviorInitialize(Mission mission)
	{
		((MBSubModuleBase)this).OnMissionBehaviorInitialize(mission);
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableCompanionUIChanges)
		{
			mission.AddMissionBehavior((MissionBehavior)(object)new CompanionHighlightingMissionBehavior());
		}
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableBattleChatter)
		{
			mission.AddMissionBehavior((MissionBehavior)(object)new BattleChatter());
		}
		ICampaignMission current = CampaignMission.Current;
		Location val = ((current != null) ? current.Location : null);
		LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
		Settlement settlement = ((locationEncounter != null) ? locationEncounter.Settlement : null);
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f.Settlement == settlement);
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.EnableTodayWeFeast && feastByAttribute != null)
		{
			mission.AddMissionBehavior((MissionBehavior)(object)new FeastMissionLogic());
		}
	}
}
