using System;
using System.Collections.Generic;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;
using WarAndAiTweaks;

namespace WarAndAITweaks.MarshalSystem;

public class MarshalSystemBehavior : CampaignBehaviorBase
{
	[SaveableField(432)]
	private Dictionary<Kingdom, MarshalData> _kingdomMarshals = new Dictionary<Kingdom, MarshalData>();

	public static Dictionary<Kingdom, CampaignTime> _lastMarshalProcess = new Dictionary<Kingdom, CampaignTime>();

	public static event Action<Kingdom, Hero> MarshalAppointed;

	public override void RegisterEvents()
	{
		CampaignEvents.DailyTickClanEvent.AddNonSerializedListener((object)this, (Action<Clan>)DailyTickClanEvent);
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
		CampaignEvents.HeroKilledEvent.AddNonSerializedListener((object)this, (Action<Hero, Hero, KillCharacterActionDetail, bool>)OnHeroKilled);
		CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener((object)this, (Action<Clan, Kingdom, Kingdom, ChangeKingdomActionDetail, bool>)OnClanChangedKingdom);
		CampaignEvents.KingdomDestroyedEvent.AddNonSerializedListener((object)this, (Action<Kingdom>)OnKingdomDestroyed);
		CampaignEvents.KingdomCreatedEvent.AddNonSerializedListener((object)this, (Action<Kingdom>)OnKingdomCreated);
	}

	public override void SyncData(IDataStore dataStore)
	{
		try
		{
			dataStore.SyncData<Dictionary<Kingdom, MarshalData>>("_kingdomMarshals", ref _kingdomMarshals);
		}
		catch
		{
			_kingdomMarshals = new Dictionary<Kingdom, MarshalData>();
		}
	}

	private void OnKingdomCreated(Kingdom kingdom)
	{
		if (!_kingdomMarshals.ContainsKey(kingdom))
		{
			_kingdomMarshals[kingdom] = new MarshalData();
		}
	}

	private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
	{
		foreach (Kingdom item in ((IEnumerable<Kingdom>)Kingdom.All).Where((Kingdom k) => !k.IsEliminated && !k.IsMinorFaction))
		{
			MarshalHelper.RemoveAllInvalidMarshalDecisions(item);
			if (!_kingdomMarshals.ContainsKey(item))
			{
				_kingdomMarshals[item] = new MarshalData();
			}
		}
	}

	private void DailyTickClanEvent(Clan clan)
	{
		if (clan.Kingdom == null || ((List<Settlement>)(object)clan.Kingdom.Settlements).Count < 1)
		{
			return;
		}
		Kingdom kingdom = clan.Kingdom;
		if (kingdom.RulingClan == null || clan != kingdom.RulingClan)
		{
			return;
		}
		if (_lastMarshalProcess.TryGetValue(kingdom, out var value))
		{
			CampaignTime now = CampaignTime.Now;
			if (now.ToDays - value.ToDays < 5.0)
			{
				return;
			}
		}
		_lastMarshalProcess[kingdom] = CampaignTime.Now;
		ProcessMarshalStateAndElections(kingdom);
	}

	private void ProcessMarshalStateAndElections(Kingdom kingdom)
	{
		if (!_kingdomMarshals.ContainsKey(kingdom))
		{
			_kingdomMarshals[kingdom] = new MarshalData();
		}
		MarshalData marshalData = _kingdomMarshals[kingdom];
		Hero currentMarshal = marshalData.CurrentMarshal;
		if (currentMarshal != null && (currentMarshal.IsDead || currentMarshal.Clan == null || currentMarshal.Clan.Kingdom != kingdom))
		{
			RemoveMarshal(kingdom, currentMarshal.IsDead ? "died" : "left the kingdom");
			StartMarshalElection(kingdom);
			return;
		}
		if (currentMarshal != null)
		{
			CampaignTime val = CampaignTime.Now;
			double toDays = val.ToDays;
			val = marshalData.AppointmentEndDate;
			if (toDays >= val.ToDays)
			{
				StartMarshalElection(kingdom);
				return;
			}
		}
		if (currentMarshal == null)
		{
			StartMarshalElection(kingdom);
		}
	}

	private void StartMarshalElection(Kingdom kingdom)
	{
		Kingdom kingdom2 = kingdom;
		Clan clan = Hero.MainHero.Clan;
		Kingdom val = ((clan != null) ? clan.Kingdom : null);
		if ((val != null && val == kingdom2) || kingdom2 == null || kingdom2.IsEliminated || kingdom2.RulingClan == null || kingdom2.RulingClan.Leader == null || kingdom2.Clans == null || ((List<Clan>)(object)kingdom2.Clans).Count == 0)
		{
			return;
		}
		List<Hero> list = (from h in GetMarshalCandidates(kingdom2)
			where h != null && h.Clan != null && !h.IsDead && h.Clan.Kingdom == kingdom2
			select h).ToList();
		if (list.Any() && !((IEnumerable<KingdomDecision>)kingdom2.UnresolvedDecisions).Any((KingdomDecision d) => d is MarshalDecision))
		{
			MarshalDecision marshalDecision = new MarshalDecision(kingdom2.RulingClan, kingdom2, list);
			if (((KingdomDecision)marshalDecision).IsAllowed())
			{
				kingdom2.AddDecision((KingdomDecision)(object)marshalDecision, false);
			}
		}
	}

	public void AppointMarshal(Kingdom kingdom, Hero marshal)
	{
		MarshalData marshalData = _kingdomMarshals[kingdom];
		marshalData.CurrentMarshal = marshal;
		marshalData.AppointmentStartDate = CampaignTime.Now;
		int num = Math.Max(1, GlobalSettings<WarAndAiTweaksSettings>.Instance.MarshalTermDays);
		marshalData.AppointmentEndDate = CampaignTime.DaysFromNow((float)num);
		if (Hero.MainHero.Clan.Kingdom != null && Hero.MainHero.Clan.Kingdom == kingdom)
		{
			TextObject val = LanguageTranslater.L.T("marshal_appointed", "{MARSHAL} has been appointed as the new Marshal of {KINGDOM}.");
			val.SetTextVariable("MARSHAL", marshal.Name);
			val.SetTextVariable("KINGDOM", kingdom.Name);
			InformationManager.DisplayMessage(new InformationMessage(((object)val).ToString(), Colors.Green));
		}
		MarshalSystemBehavior.MarshalAppointed?.Invoke(kingdom, marshal);
	}

	private void RemoveMarshal(Kingdom kingdom, string reason)
	{
		MarshalData marshalData = _kingdomMarshals[kingdom];
		if (marshalData.CurrentMarshal != null && Hero.MainHero.Clan != null && Hero.MainHero.Clan.Kingdom == kingdom)
		{
			TextObject val = LanguageTranslater.L.T("marshal_removed", "{MARSHAL} is no longer Marshal of {KINGDOM} ({REASON})");
			val.SetTextVariable("MARSHAL", marshalData.CurrentMarshal.Name);
			val.SetTextVariable("KINGDOM", kingdom.Name);
			val.SetTextVariable("REASON", reason);
			InformationManager.DisplayMessage(new InformationMessage(((object)val).ToString(), Colors.Red));
		}
		marshalData.CurrentMarshal = null;
	}

	private List<Hero> GetMarshalCandidates(Kingdom kingdom)
	{
		MBList<Hero> val = new MBList<Hero>();
		foreach (Hero item in (List<Hero>)(object)kingdom.Heroes)
		{
			if (MarshalHelper.IsEligibleForMarshalship(item))
			{
				((List<Hero>)(object)val).Add(item);
			}
		}
		return (List<Hero>)(object)val;
	}

	private void OnHeroKilled(Hero victim, Hero killer, KillCharacterActionDetail detail, bool showNotification)
	{
		foreach (KeyValuePair<Kingdom, MarshalData> item in _kingdomMarshals.ToList())
		{
			if (item.Value.CurrentMarshal == victim)
			{
				RemoveMarshal(item.Key, "died");
				StartMarshalElection(item.Key);
			}
		}
	}

	private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomActionDetail detail, bool showNotification)
	{
		if ((oldKingdom == null || ((List<Settlement>)(object)oldKingdom.Settlements).Count != 0) && oldKingdom != null && _kingdomMarshals.ContainsKey(oldKingdom) && _kingdomMarshals[oldKingdom].CurrentMarshal == clan.Leader)
		{
			RemoveMarshal(oldKingdom, "left the kingdom");
			StartMarshalElection(oldKingdom);
		}
	}

	private void OnKingdomDestroyed(Kingdom destroyedKingdom)
	{
		_kingdomMarshals.Remove(destroyedKingdom);
		_lastMarshalProcess.Remove(destroyedKingdom);
	}

	public static Hero GetMarshal(Kingdom kingdom)
	{
		if (kingdom == null)
		{
			return null;
		}
		MarshalSystemBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<MarshalSystemBehavior>();
		return (campaignBehavior != null && campaignBehavior._kingdomMarshals.ContainsKey(kingdom)) ? campaignBehavior._kingdomMarshals[kingdom].CurrentMarshal : null;
	}

	public static bool IsMarshal(Hero hero)
	{
		Hero hero2 = hero;
		return Campaign.Current.GetCampaignBehavior<MarshalSystemBehavior>()?._kingdomMarshals.Values.Any((MarshalData m) => m.CurrentMarshal == hero2) ?? false;
	}
}


