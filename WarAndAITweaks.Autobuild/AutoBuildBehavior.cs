using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace WarAndAITweaks.Autobuild;

public sealed class AutoBuildBehavior : CampaignBehaviorBase
{
	[SaveableField(1)]
	private List<Town> autoBuildTowns = new List<Town>();

	private static Town GovSettlement()
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		return (oneToOneConversationHero != null) ? oneToOneConversationHero.GovernorOf : null;
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
		CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener((object)this, (Action<Settlement>)DailyTickSettlementEvent);
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData<List<Town>>("autoBuildTowns", ref autoBuildTowns);
		if (autoBuildTowns == null)
		{
			autoBuildTowns = new List<Town>();
		}
	}

	private void OnSessionLaunched(CampaignGameStarter s)
	{
		s.AddPlayerLine("autobuild_enable", "hero_main_options", "autobuild_menu_enable", LanguageTranslater.L.S("autobuild_enable", "I would like you to start auto-building in your settlement."), () => HasGov() && !autoBuildTowns.Contains(GovSettlement()), delegate
		{
			Town town2 = GovSettlement();
			Enable(town2);
		}, 100, null, null);
		s.AddDialogLine("autobuild_menu_enable", "autobuild_menu_enable", "hero_main_options", LanguageTranslater.L.S("autobuild_enable_response", "Understood. I will begin auto-building projects."), null, null, 100, null);
		s.AddPlayerLine("autobuild_disable", "hero_main_options", "autobuild_menu_disable", LanguageTranslater.L.S("autobuild_disable", "Stop auto-building in your settlement."), () => HasGov() && autoBuildTowns.Contains(GovSettlement()), delegate
		{
			Town town = GovSettlement();
			Disable(town);
		}, 100, null, null);
		s.AddDialogLine("autobuild_menu_disable", "autobuild_menu_disable", "hero_main_options", LanguageTranslater.L.S("autobuild_disable_response", "Understood. I will stop auto-building projects."), null, null, 100, null);
	}

	private void DailyTickSettlementEvent(Settlement settlement)
	{
		if (settlement != null)
		{
			Town town = settlement.Town;
			if (town != null && autoBuildTowns.Contains(town))
			{
				TryQueueOneBuilding(town);
			}
		}
	}

	private void TryQueueOneBuilding(Town town)
	{
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Invalid comparison between Unknown and I4
		if (town.BuildingsInProgress.Count >= 1)
		{
			return;
		}
		if (town.Governor == null)
		{
			Disable(town);
			return;
		}
		Building val = Campaign.Current.Models.BuildingScoreCalculationModel.GetNextBuilding(town);
		if (val == null)
		{
			foreach (Building item in (List<Building>)(object)town.Buildings)
			{
				if (item == null || item.CurrentLevel >= 3 || town.BuildingsInProgress.Contains(item))
				{
					continue;
				}
				val = item;
				break;
			}
		}
		if (val != null)
		{
			List<Building> list = new List<Building>(town.BuildingsInProgress) { val };
			BuildingHelper.ChangeCurrentBuildingQueue(list, town);
			Hero governor = town.Governor;
			string text = ((governor != null) ? ((object)governor.Name).ToString() : LanguageTranslater.L.S("Unknown", "Unknown"));
			string text2 = ((object)val.BuildingType.Name).ToString();
			string text3 = ((object)((SettlementComponent)town).Settlement.Name).ToString();
			int num = val.CurrentLevel + 1;
			TextObject val2 = LanguageTranslater.L.T("autobuild_started", "{GOVERNOR} has started building {BUILDING} (Level {LEVEL}) at {SETTLEMENT}.");
			val2.SetTextVariable("GOVERNOR", text);
			val2.SetTextVariable("BUILDING", text2);
			val2.SetTextVariable("LEVEL", num);
			val2.SetTextVariable("SETTLEMENT", text3);
			InformationManager.DisplayMessage(new InformationMessage(((object)val2).ToString(), Colors.Green));
		}
	}

	private static bool HasGov()
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		return oneToOneConversationHero != null && oneToOneConversationHero != Hero.MainHero && oneToOneConversationHero.Clan == Clan.PlayerClan && oneToOneConversationHero.GovernorOf != null;
	}

	private void Enable(Town town)
	{
		Hero governor = town.Governor;
		string text = ((governor != null) ? ((object)governor.Name).ToString() : LanguageTranslater.L.S("Unknown", "Unknown"));
		string text2 = ((object)((SettlementComponent)town).Settlement.Name).ToString();
		if (town != null && !autoBuildTowns.Contains(town))
		{
			autoBuildTowns.Add(town);
			TextObject val = LanguageTranslater.L.T("autobuild_enable_log", "{GOVERNOR} has been instructed to auto construct buildings at {SETTLEMENT}");
			val.SetTextVariable("GOVERNOR", text);
			val.SetTextVariable("SETTLEMENT", text2);
			Log(((object)val).ToString());
		}
	}

	private void Disable(Town town)
	{
		Hero governor = town.Governor;
		string text = ((governor != null) ? ((object)governor.Name).ToString() : LanguageTranslater.L.S("Unknown", "Unknown"));
		string text2 = ((object)((SettlementComponent)town).Settlement.Name).ToString();
		if (town != null && autoBuildTowns.Contains(town))
		{
			autoBuildTowns.Remove(town);
			TextObject val = LanguageTranslater.L.T("autobuild_disable_log", "{GOVERNOR} has been instructed to no longer auto construct buildings at {SETTLEMENT}");
			val.SetTextVariable("GOVERNOR", text);
			val.SetTextVariable("SETTLEMENT", text2);
			Log(((object)val).ToString());
		}
	}

	private static void Log(string msg)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		InformationManager.DisplayMessage(new InformationMessage(msg ?? ""));
	}
}
