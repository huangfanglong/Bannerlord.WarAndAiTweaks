using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace ClanRespawn;

public class ClanRespawnBehavior : CampaignBehaviorBase
{
	[SaveableField(50)]
	public static List<RespawnablePartyObject> Parties = new List<RespawnablePartyObject>();

	public override void RegisterEvents()
	{
		CampaignEvents.MobilePartyCreated.AddNonSerializedListener((object)this, (Action<MobileParty>)onMobilePartyCreated);
		CampaignEvents.OnPartyDisbandStartedEvent.AddNonSerializedListener((object)this, (Action<MobileParty>)onPartyDisbanded);
		CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnGameLoaded);
		CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnGameLoaded);
		CampaignEvents.BeforeHeroKilledEvent.AddNonSerializedListener((object)this, (Action<Hero, Hero, KillCharacterActionDetail, bool>)onHerokilled);
		CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener((object)this, (Action<Hero, PartyBase, IFaction, EndCaptivityDetail, bool>)onFugitive);
	}

	private void onFugitive(Hero prisonerBeingReleased, PartyBase partyBase, IFaction faction, EndCaptivityDetail detail, bool isReleasedFromParty)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Expected O, but got Unknown
		Hero prisonerBeingReleased2 = prisonerBeingReleased;
		if (prisonerBeingReleased2.Clan != Hero.MainHero.Clan || (int)detail == 5)
		{
			return;
		}
		RespawnablePartyObject respawnablePartyObject = Parties.FirstOrDefault((RespawnablePartyObject x) => x.partyHero == prisonerBeingReleased2);
		if (respawnablePartyObject != null && !prisonerBeingReleased2.IsActive && prisonerBeingReleased2.IsAlive)
		{
			bool isLord = respawnablePartyObject.isLord;
			bool isCaravan = respawnablePartyObject.isCaravan;
			if (!isLord && !isCaravan)
			{
				RemoveParty(prisonerBeingReleased2);
				return;
			}
			if (respawnablePartyObject.isLord && prisonerBeingReleased2.Clan != null && ((List<WarPartyComponent>)(object)prisonerBeingReleased2.Clan.WarPartyComponents).Count < 4)
			{
				SpawnLordPartyAtPosition(prisonerBeingReleased2);
				return;
			}
			if (respawnablePartyObject.isCaravan)
			{
				string text = LanguageTranslater.L.S("Caravan Respawn", "Caravan Respawn");
				TextObject val = LanguageTranslater.L.T("caravan_respawn_body", "{HERO} is attempting to reform their caravan. This will cost you 15,000 denars. Do you wish to proceed?");
				val.SetTextVariable("HERO", ((object)prisonerBeingReleased2.Name).ToString());
				string text2 = LanguageTranslater.L.S("Yes", "Yes");
				string text3 = LanguageTranslater.L.S("No", "No");
				string notEnoughDenars = LanguageTranslater.L.S("not_enough_denars", "You do not have enough denars to reform this caravan.");
				InformationManager.ShowInquiry(new InquiryData(text, ((object)val).ToString(), true, true, text2, text3, (Action)delegate
				{
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					//IL_003a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0044: Expected O, but got Unknown
					if (Hero.MainHero.Gold >= 15000)
					{
						SpawnCaravanAtPosition(prisonerBeingReleased2);
					}
					else
					{
						InformationManager.DisplayMessage(new InformationMessage(notEnoughDenars, Colors.Red));
						RemoveParty(prisonerBeingReleased2);
					}
				}, (Action)null, "", 0f, (Action)null, (Func<ValueTuple<bool, string>>)null, (Func<ValueTuple<bool, string>>)null), true, false);
				return;
			}
		}
		RemoveParty(prisonerBeingReleased2);
	}

	private void OnGameLoaded(CampaignGameStarter gameStarterObject)
	{
		List<RespawnablePartyObject> parties = Parties;
		if (parties == null || !parties.Any())
		{
			Parties = new List<RespawnablePartyObject>();
		}
	}

	private void onPartyDisbanded(MobileParty mobileparty)
	{
		RemoveParty(mobileparty.LeaderHero);
	}

	private void onHerokilled(Hero victim, Hero killer, KillCharacterActionDetail actiondefailt, bool shownotifcation)
	{
		RemoveParty(victim);
	}

	private void onMobilePartyCreated(MobileParty mobileparty)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		MobileParty mobileparty2 = mobileparty;
		if (mobileparty2.LeaderHero != null && mobileparty2.LeaderHero.Clan == Hero.MainHero.Clan && (mobileparty2.IsLordParty || mobileparty2.IsCaravan))
		{
			RespawnablePartyObject respawnablePartyObject = Parties.Where((RespawnablePartyObject x) => x.partyHero == mobileparty2.LeaderHero).FirstOrDefault();
			if (respawnablePartyObject != null)
			{
				mobileparty2.SetWagePaymentLimit(respawnablePartyObject.currentWageLimit);
				mobileparty2.SetPartyObjective(respawnablePartyObject.partyobjective);
			}
			else
			{
				Parties.Add(new RespawnablePartyObject(mobileparty2));
			}
		}
	}

	private static void SpawnLordPartyAtPosition(Hero hero)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		if (!hero.IsActive && hero.IsAlive)
		{
			GiveGoldAction.ApplyBetweenCharacters((Hero)null, hero, 500, true);
			MobilePartyHelper.SpawnLordParty(hero, SettlementHelper.GetBestSettlementToSpawnAround(hero));
			TextObject val = LanguageTranslater.L.T("party_rallied", "{HERO} has rallied once more, forming a new party after their defeat and recovery.");
			val.SetTextVariable("HERO", ((object)hero.Name).ToString());
			InformationManager.DisplayMessage(new InformationMessage(((object)val).ToString(), Colors.Cyan));
		}
	}

	private static void SpawnCaravanAtPosition(Hero hero)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		if (!hero.IsActive && hero.IsAlive)
		{
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, hero, 15000, true);
			CaravanPartyComponent.CreateCaravanParty(Hero.MainHero, SettlementHelper.GetBestSettlementToSpawnAround(hero), null, false, hero, null, false);
			TextObject val = LanguageTranslater.L.T("caravan_rallied", "{HERO} has rallied once more, forming a new caravan after their defeat and recovery.");
			val.SetTextVariable("HERO", ((object)hero.Name).ToString());
			InformationManager.DisplayMessage(new InformationMessage(((object)val).ToString(), Colors.Cyan));
		}
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData<List<RespawnablePartyObject>>("Parties", ref Parties);
	}

	public static void RemoveParty(Hero hero)
	{
		Hero hero2 = hero;
		Parties.RemoveAll((RespawnablePartyObject x) => x.partyHero == hero2);
	}
}

