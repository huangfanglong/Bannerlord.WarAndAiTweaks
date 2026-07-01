using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FeastSystem;
using Helpers;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using WarAndAiTweaks;

namespace WarAndAITweaks.TodayWeFeast;

public sealed class FeastQuest : QuestBase
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action<CharacterObject> _003C_003E9__47_0;

		public static Func<Hero, string> _003C_003E9__52_0;

		public static OnConditionDelegate _003C_003E9__54_0;

		public static OnConditionDelegate _003C_003E9__54_1;

		public static OnConditionDelegate _003C_003E9__54_2;

		public static OnConsequenceDelegate _003C_003E9__54_3;

		public static OnConditionDelegate _003C_003E9__54_4;

		public static OnConditionDelegate _003C_003E9__54_5;

		public static OnConsequenceDelegate _003C_003E9__54_6;

		public static OnConditionDelegate _003C_003E9__54_7;

		public static OnConsequenceDelegate _003C_003E9__54_8;

		public static OnConditionDelegate _003C_003E9__54_9;

		public static OnConsequenceDelegate _003C_003E9__54_10;

		public static OnConditionDelegate _003C_003E9__54_11;

		public static OnConditionDelegate _003C_003E9__54_12;

		public static OnConsequenceDelegate _003C_003E9__54_13;

		public static OnConditionDelegate _003C_003E9__54_14;

		public static OnConditionDelegate _003C_003E9__54_15;

		public static OnConsequenceDelegate _003C_003E9__54_16;

		public static OnConditionDelegate _003C_003E9__54_17;

		internal void _003COnGameMenuOpened_003Eb__47_0(CharacterObject winner)
		{
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			if (winner == CharacterObject.PlayerCharacter)
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("duel_won", "[if:convo_excited]{PLAYER.NAME}, well fought! You bested me in the arena. The guests are still talking about it.").SetTextVariable("OPPONENT", ((object)FeastDuelSystem.PendingDuelHost.Name).ToString())).ToString(), Colors.Green));
				Hero.MainHero.AddSkillXp(DefaultSkills.OneHanded, 100f);
				GiveGoldAction.ApplyBetweenCharacters(FeastDuelSystem.PendingDuelHost, Hero.MainHero, 200, false);
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(FeastDuelSystem.PendingDuelHost, Hero.MainHero, 2, true);
				MBInformationManager.AddQuickInformation(LanguageTranslater.L.T("feast_q_duel_reward", "You gained 200 gold, 2 relation, and 100 One Handed XP!"), 0, (BasicCharacterObject)null, "");
				FeastDuelSystem.LastDuelWasPlayerWin = true;
			}
			else
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("duel_lost", "[if:convo_proud]A good effort, {PLAYER.NAME}, but victory was mine this time. Perhaps a rematch later?").SetTextVariable("OPPONENT", ((object)FeastDuelSystem.PendingDuelHost.Name).ToString())).ToString(), Colors.Red));
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, FeastDuelSystem.PendingDuelHost, 200, false);
				FeastDuelSystem.LastDuelWasPlayerWin = false;
			}
			FeastDuelSystem.LastDuelHostId = ((MBObjectBase)FeastDuelSystem.PendingDuelHost).StringId;
			FeastDuelSystem.PendingDuelComment = true;
		}

		internal string _003CUpdateMingleTaskLog_003Eb__52_0(Hero h)
		{
			return ((object)h.Name).ToString();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_0()
		{
			return IsPlayerAttendingAIHostFeast() && IsFirstTimePlayerTalksToAIHost();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_1()
		{
			return IsPlayerAttendingAIHostFeast() && !IsFirstTimePlayerTalksToAIHost();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_2()
		{
			return ShouldCompleteInteractionForCurrentConversation();
		}

		internal void _003CRegisterFeastQuestDialogs_003Eb__54_3()
		{
			CompleteInteractionForCurrentConversation();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_4()
		{
			return IsPlayerHostTalkingToGuest();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_5()
		{
			return ShouldCompleteInteractionForCurrentConversation() && !HasRewardCooldown(Hero.MainHero, Hero.OneToOneConversationHero);
		}

		internal void _003CRegisterFeastQuestDialogs_003Eb__54_6()
		{
			CompleteInteractionForCurrentConversation();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_7()
		{
			return ShouldShowDuelCommentDialog(win: true);
		}

		internal void _003CRegisterFeastQuestDialogs_003Eb__54_8()
		{
			ClearDuelCommentFlag();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_9()
		{
			return ShouldShowDuelCommentDialog(win: false);
		}

		internal void _003CRegisterFeastQuestDialogs_003Eb__54_10()
		{
			ClearDuelCommentFlag();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_11()
		{
			return ShouldOfferDuelForCurrentConversation();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_12()
		{
			return true;
		}

		internal void _003CRegisterFeastQuestDialogs_003Eb__54_13()
		{
			AcceptHostDuelChallenge();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_14()
		{
			return true;
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_15()
		{
			return true;
		}

		internal void _003CRegisterFeastQuestDialogs_003Eb__54_16()
		{
			DeclineHostDuelChallenge();
		}

		internal bool _003CRegisterFeastQuestDialogs_003Eb__54_17()
		{
			return !ShouldOfferDuelForCurrentConversation();
		}
	}

	private const int RelationRewardPerTalk = 1;

	private const int RenownRewardPerTalk = 3;

	private const int PlayerHostLeadershipXpOnSuccess = 50;

	private const int PlayerHostGoldOnSuccess = 1000;

	internal static readonly Dictionary<Hero, FeastQuest> ActiveByHost = new Dictionary<Hero, FeastQuest>();

	[SaveableField(10)]
	private Hero _host;

	[SaveableField(20)]
	private Settlement _settlement;

	[SaveableField(30)]
	private CampaignTime _feastStart;

	[SaveableField(40)]
	private bool _travelCompleted;

	[SaveableField(50)]
	private bool _firstInteractionCompleted;

	[SaveableField(60)]
	private bool _activeInteractionTask;

	[SaveableField(70)]
	private JournalLog _travelLog;

	[SaveableField(80)]
	private JournalLog _interactionLog;

	[SaveableField(90)]
	private int _timesSpokenToHost;

	[SaveableField(100)]
	private bool _isPlayerHost;

	[SaveableField(110)]
	private List<Hero> _spokenGuests = new List<Hero>();

	[SaveableField(130)]
	private int _mingleInitialTargetCount;

	private string _lastMingleLogText;

	private int CooldownDays => GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastDialogCooldown;

	public override TextObject Title
	{
		get
		{
			TextObject val = LanguageTranslater.L.T("feast_q_title", "Feast at {SETTLEMENT}");
			Settlement settlement = _settlement;
			val.SetTextVariable("SETTLEMENT", ((settlement != null) ? settlement.Name : null) ?? TextObject.Empty);
			return val;
		}
	}

	public override bool IsRemainingTimeHidden => true;

	public override bool IsSpecialQuest => true;

	private FeastData FindActiveFeast()
	{
		return FeastBehavior.GetFeastByAttribute((FeastData f) => f != null && f.Host == _host && f.Settlement == _settlement);
	}

	private bool IsFeastStillValid()
	{
		Hero host = _host;
		object obj;
		if (host == null)
		{
			obj = null;
		}
		else
		{
			Clan clan = host.Clan;
			obj = ((clan != null) ? clan.Kingdom : null);
		}
		if (obj == null || _settlement == null)
		{
			return false;
		}
		return FindActiveFeast() != null && !_host.Clan.Kingdom.IsEliminated;
	}

	public static bool HasRewardCooldown(Hero host, Hero guest)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		Hero host2 = host;
		Hero guest2 = guest;
		if (host2 == null || guest2 == null)
		{
			return false;
		}
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f != null && f.Host == host2 && f.Attendees != null && ((List<Hero>)(object)f.Attendees).Contains(guest2));
		if (feastByAttribute == null)
		{
			return false;
		}
		if (FeastBehavior._lastFeastRewardTimes.TryGetValue(feastByAttribute, out RewardData value) && value != null)
		{
			RewardData.RewardInfo orCreate = value.GetOrCreate(host2, guest2);
			double nextFeastRewardTime = orCreate.nextFeastRewardTime;
			CampaignTime now = CampaignTime.Now;
			double num = nextFeastRewardTime - ((CampaignTime)(ref now)).ToDays;
			return num > 0.0;
		}
		return false;
	}

	public static void GiveRewardWithCooldownRespect(Hero host, Hero guest)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		Hero host2 = host;
		Hero guest2 = guest;
		if (host2 == null || guest2 == null)
		{
			return;
		}
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f != null && f.Host == host2 && f.Attendees != null && ((List<Hero>)(object)f.Attendees).Contains(guest2));
		if (feastByAttribute != null)
		{
			if (!FeastBehavior._lastFeastRewardTimes.TryGetValue(feastByAttribute, out RewardData value) || value == null)
			{
				value = new RewardData();
				FeastBehavior._lastFeastRewardTimes[feastByAttribute] = value;
			}
			RewardData.RewardInfo orCreate = value.GetOrCreate(host2, guest2);
			CampaignTime now = CampaignTime.Now;
			orCreate.nextFeastRewardTime = ((CampaignTime)(ref now)).ToDays + (double)GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastDialogCooldown;
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(host2, guest2, 1, true);
			GainRenownAction.Apply(host2, 3f, false);
			GainRenownAction.Apply(guest2, 3f, false);
		}
	}

	private TextObject GetQuestStartedLogText()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		if (_isPlayerHost)
		{
			TextObject val = LanguageTranslater.L.T("feast_q_host_started", "You are hosting a feast at {SETTLEMENT}.");
			if (_settlement != null)
			{
				val.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
			}
			if (string.IsNullOrEmpty(((object)val).ToString()))
			{
				Debug.Print("[FeastQuest] Warning: feast_q_host_started returned empty text", 0, (DebugColor)12, 17592186044416uL);
				return new TextObject("You are hosting a feast at {SETTLEMENT}.", (Dictionary<string, object>)null);
			}
			return val;
		}
		TextObject val2 = LanguageTranslater.L.T("feast_q_start", "{QUEST_GIVER.NAME} has invited you to a feast at {SETTLEMENT}. {?QUEST_GIVER.GENDER}She{?}He{\\?} asks that you travel there and speak with {?QUEST_GIVER.GENDER}her{?}him{\\?} to enjoy the festivities and build relations with the realm's nobility.");
		Hero questGiver = ((QuestBase)this).QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", ((QuestBase)this).QuestGiver.CharacterObject, val2, false);
		}
		if (_settlement != null)
		{
			val2.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
		}
		if (string.IsNullOrEmpty(((object)val2).ToString()))
		{
			Debug.Print("[FeastQuest] Warning: feast_q_start returned empty text", 0, (DebugColor)12, 17592186044416uL);
			return new TextObject("{QUEST_GIVER.NAME} has invited you to a feast at {SETTLEMENT}.", (Dictionary<string, object>)null);
		}
		return val2;
	}

	private TextObject GetQuestConcludedLogText()
	{
		TextObject val = LanguageTranslater.L.T("feast_q_success", "You attended the feast hosted by {QUEST_GIVER.NAME} at {SETTLEMENT}. Your participation has strengthened your bonds with the nobles of the realm.");
		Hero questGiver = ((QuestBase)this).QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", ((QuestBase)this).QuestGiver.CharacterObject, val, false);
		}
		if (_settlement != null)
		{
			val.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
		}
		return val;
	}

	private TextObject GetQuestCanceledDueToWarLogText()
	{
		TextObject val = LanguageTranslater.L.T("feast_q_cancel_war", "Your clan is now at war with {QUEST_GIVER.NAME}'s faction. The feast invitation has been rescinded, and your agreement was canceled.");
		Hero questGiver = ((QuestBase)this).QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", ((QuestBase)this).QuestGiver.CharacterObject, val, false);
		}
		return val;
	}

	private TextObject GetPlayerArrivedAtFeastLogText()
	{
		if (_isPlayerHost)
		{
			TextObject val = LanguageTranslater.L.T("feast_q_host_arrived", "You have arrived at {SETTLEMENT} to start your feast.");
			if (_settlement != null)
			{
				val.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
			}
			return val;
		}
		TextObject val2 = LanguageTranslater.L.T("feast_q_arrived", "You have arrived at the feast hosted by {QUEST_GIVER.NAME} in {SETTLEMENT}. Speak with {?QUEST_GIVER.GENDER}her{?}him{\\?} to participate in the festivities.");
		Hero questGiver = ((QuestBase)this).QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", ((QuestBase)this).QuestGiver.CharacterObject, val2, false);
		}
		if (_settlement != null)
		{
			val2.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
		}
		return val2;
	}

	private TextObject GetPlayerSpokeWithHostLogText()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		TextObject val = LanguageTranslater.L.T("feast_q_spoke", "You spoke with {QUEST_GIVER.NAME} at the feast. {?QUEST_GIVER.GENDER}She{?}He{\\?} appreciates your attendance and conversation. Stay and enjoy the festivities while time passes.");
		Hero questGiver = ((QuestBase)this).QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", ((QuestBase)this).QuestGiver.CharacterObject, val, false);
		}
		if (string.IsNullOrEmpty(((object)val).ToString()))
		{
			Debug.Print("[FeastQuest] Warning: feast_q_spoke returned empty text", 0, (DebugColor)12, 17592186044416uL);
			return new TextObject("You spoke with {QUEST_GIVER.NAME} at the feast.", (Dictionary<string, object>)null);
		}
		return val;
	}

	private TextObject GetCooldownExpiredLogText()
	{
		TextObject val = LanguageTranslater.L.T("feast_q_cooldown", "Enough time has passed. You may speak with {QUEST_GIVER.NAME} again to continue building your relationship.");
		Hero questGiver = ((QuestBase)this).QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", ((QuestBase)this).QuestGiver.CharacterObject, val, false);
		}
		return val;
	}

	public FeastQuest(string questId, Hero questGiver, Hero host, Settlement feastSettlement, CampaignTime feastStart, bool isPlayerHost = false)
		: base(questId, questGiver, CampaignTime.DaysFromNow(365f), 0)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		_host = host;
		_settlement = feastSettlement;
		_feastStart = feastStart;
		_isPlayerHost = isPlayerHost;
		_timesSpokenToHost = 0;
		if (_host != null && ActiveByHost.TryGetValue(_host, out FeastQuest value) && ((QuestBase)value).IsOngoing)
		{
			((QuestBase)value).CompleteQuestWithCancel(LanguageTranslater.L.T("feast_q_replaced", "A new feast invitation has replaced the previous one."));
		}
		ActiveByHost[_host] = this;
		((QuestBase)this).SetDialogs();
		((QuestBase)this).InitializeQuestOnCreation();
		((QuestBase)this).StartQuest();
		TextObject val = LanguageTranslater.L.T("feast_q_start", "{QUEST_GIVER.NAME} has invited you to a feast at {SETTLEMENT}. {?QUEST_GIVER.GENDER}She{?}He{\\?} asks that you travel there and speak with {?QUEST_GIVER.GENDER}her{?}him{\\?} to enjoy the festivities and build relations with the realm's nobility.");
		Debug.Print("[FeastQuest] LanguageTranslater test - feast_q_start: '" + ((object)val).ToString() + "'", 0, (DebugColor)12, 17592186044416uL);
		((QuestBase)this).AddLog(GetQuestStartedLogText(), false);
		TextObject val2 = (_isPlayerHost ? LanguageTranslater.L.T("feast_q_host_travel", "Travel to {SETTLEMENT} to start your feast.") : LanguageTranslater.L.T("feast_q_travel", "Travel to {SETTLEMENT} to attend the feast."));
		if (_settlement != null)
		{
			val2.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
			_travelLog = ((QuestBase)this).AddLog(val2, false);
			((QuestBase)this).AddTrackedObject((ITrackableCampaignObject)(object)_settlement);
		}
	}

	protected override void SetDialogs()
	{
	}

	protected override void RegisterEvents()
	{
		CampaignEvents.SettlementEntered.AddNonSerializedListener((object)this, (Action<MobileParty, Settlement, Hero>)OnSettlementEntered);
		CampaignEvents.WarDeclared.AddNonSerializedListener((object)this, (Action<IFaction, IFaction, DeclareWarDetail>)OnWarDeclared);
		CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener((object)this, (Action<Clan, Kingdom, Kingdom, ChangeKingdomActionDetail, bool>)OnClanChangedKingdom);
		CampaignEvents.GameMenuOpened.AddNonSerializedListener((object)this, (Action<MenuCallbackArgs>)OnGameMenuOpened);
	}

	protected override void HourlyTick()
	{
		if (!((QuestBase)this).IsOngoing)
		{
			return;
		}
		if (!IsFeastStillValid())
		{
			if (_isPlayerHost)
			{
				Hero.MainHero.AddSkillXp(DefaultSkills.Leadership, 50f);
				GiveGoldAction.ApplyBetweenCharacters((Hero)null, Hero.MainHero, 1000, false);
				((QuestBase)this).AddLog(LanguageTranslater.L.T("feast_q_host_success", "Your feast was a success! You gained leadership XP and gold."), false);
			}
			((QuestBase)this).AddLog(GetQuestConcludedLogText(), false);
			((QuestBase)this).CompleteQuestWithSuccess();
		}
		else if (_isPlayerHost)
		{
			if (_travelCompleted && !_activeInteractionTask)
			{
				CreateMingleTask();
			}
			else if (_activeInteractionTask && _interactionLog != null)
			{
				UpdateMingleTaskLog();
			}
		}
		else if (_travelCompleted && _firstInteractionCompleted && !_activeInteractionTask && !HasRewardCooldown(_host, Hero.MainHero))
		{
			CreateSpeakTask(firstTime: false);
		}
	}

	protected override void OnTimedOut()
	{
	}

	protected override void OnFinalize()
	{
		if (_settlement != null && ((QuestBase)this).IsTracked((ITrackableCampaignObject)(object)_settlement))
		{
			((QuestBase)this).RemoveTrackedObject((ITrackableCampaignObject)(object)_settlement);
		}
		if (_host != null && ActiveByHost.TryGetValue(_host, out FeastQuest value) && value == this)
		{
			ActiveByHost.Remove(_host);
		}
		if (_interactionLog != null)
		{
			((QuestBase)this).RemoveLog(_interactionLog);
			_interactionLog = null;
		}
	}

	protected override void InitializeQuestOnGameLoad()
	{
		if (_host != null)
		{
			ActiveByHost[_host] = this;
		}
	}

	public override void OnFailed()
	{
	}

	public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
	{
		if (hero == _host)
		{
			result = false;
		}
	}

	private void OnWarDeclared(IFaction f1, IFaction f2, DeclareWarDetail detail)
	{
		Hero host = _host;
		object obj;
		if (host == null)
		{
			obj = null;
		}
		else
		{
			Clan clan = host.Clan;
			obj = ((clan != null) ? clan.Kingdom : null);
		}
		Kingdom val = (Kingdom)obj;
		if (val != null && (val == f1 || val == f2))
		{
			((QuestBase)this).AddLog(GetQuestCanceledDueToWarLogText(), false);
			((QuestBase)this).CompleteQuestWithCancel((TextObject)null);
		}
	}

	private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomActionDetail detail, bool showNotification = true)
	{
		if (clan == Clan.PlayerClan)
		{
			Hero host = _host;
			object obj;
			if (host == null)
			{
				obj = null;
			}
			else
			{
				Clan clan2 = host.Clan;
				obj = ((clan2 != null) ? clan2.Kingdom : null);
			}
			if (obj != null && _host.Clan.Kingdom.IsAtWarWith(Hero.MainHero.MapFaction))
			{
				((QuestBase)this).AddLog(GetQuestCanceledDueToWarLogText(), false);
				((QuestBase)this).CompleteQuestWithCancel((TextObject)null);
			}
		}
	}

	private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
	{
		if (((QuestBase)this).IsOngoing && party == MobileParty.MainParty && settlement == _settlement && !_travelCompleted)
		{
			_travelCompleted = true;
			if (_travelLog != null && ((QuestBase)this).IsTracked((ITrackableCampaignObject)(object)_settlement))
			{
				((QuestBase)this).RemoveTrackedObject((ITrackableCampaignObject)(object)_settlement);
			}
			((QuestBase)this).AddLog(GetPlayerArrivedAtFeastLogText(), false);
			if (_isPlayerHost)
			{
				CreateMingleTask();
			}
			else
			{
				CreateSpeakTask(firstTime: true);
			}
		}
	}

	private void OnGameMenuOpened(MenuCallbackArgs args)
	{
		if (!FeastDuelSystem.PendingFeastDuel || Hero.MainHero.CurrentSettlement != FeastDuelSystem.PendingDuelSettlement)
		{
			return;
		}
		FeastDuelSystem.PendingFeastDuel = false;
		Location locationWithId = FeastDuelSystem.PendingDuelSettlement.LocationComplex.GetLocationWithId("arena");
		int num = ((!FeastDuelSystem.PendingDuelSettlement.IsTown) ? 1 : FeastDuelSystem.PendingDuelSettlement.Town.GetWallLevel());
		string sceneName = locationWithId.GetSceneName(num);
		CampaignMission.OpenArenaDuelMission(sceneName, locationWithId, FeastDuelSystem.PendingDuelHost.CharacterObject, false, false, (Action<CharacterObject>)delegate(CharacterObject winner)
		{
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			if (winner == CharacterObject.PlayerCharacter)
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("duel_won", "[if:convo_excited]{PLAYER.NAME}, well fought! You bested me in the arena. The guests are still talking about it.").SetTextVariable("OPPONENT", ((object)FeastDuelSystem.PendingDuelHost.Name).ToString())).ToString(), Colors.Green));
				Hero.MainHero.AddSkillXp(DefaultSkills.OneHanded, 100f);
				GiveGoldAction.ApplyBetweenCharacters(FeastDuelSystem.PendingDuelHost, Hero.MainHero, 200, false);
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(FeastDuelSystem.PendingDuelHost, Hero.MainHero, 2, true);
				MBInformationManager.AddQuickInformation(LanguageTranslater.L.T("feast_q_duel_reward", "You gained 200 gold, 2 relation, and 100 One Handed XP!"), 0, (BasicCharacterObject)null, "");
				FeastDuelSystem.LastDuelWasPlayerWin = true;
			}
			else
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("duel_lost", "[if:convo_proud]A good effort, {PLAYER.NAME}, but victory was mine this time. Perhaps a rematch later?").SetTextVariable("OPPONENT", ((object)FeastDuelSystem.PendingDuelHost.Name).ToString())).ToString(), Colors.Red));
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, FeastDuelSystem.PendingDuelHost, 200, false);
				FeastDuelSystem.LastDuelWasPlayerWin = false;
			}
			FeastDuelSystem.LastDuelHostId = ((MBObjectBase)FeastDuelSystem.PendingDuelHost).StringId;
			FeastDuelSystem.PendingDuelComment = true;
		}, 100f);
	}

	private void CreateSpeakTask(bool firstTime)
	{
		_activeInteractionTask = true;
		TextObject val = (firstTime ? LanguageTranslater.L.T("feast_q_speak_first", "Speak to {QUEST_GIVER.NAME} at the feast.") : LanguageTranslater.L.T("feast_q_speak_again", "Speak to {QUEST_GIVER.NAME} again now that enough time has passed."));
		StringHelpers.SetCharacterProperties("QUEST_GIVER", ((QuestBase)this).QuestGiver.CharacterObject, val, false);
		if (!firstTime)
		{
			((QuestBase)this).AddLog(GetCooldownExpiredLogText(), false);
		}
		_interactionLog = ((QuestBase)this).AddLog(val, false);
		if (_settlement != null && !((QuestBase)this).IsTracked((ITrackableCampaignObject)(object)_settlement))
		{
			((QuestBase)this).AddTrackedObject((ITrackableCampaignObject)(object)_settlement);
		}
	}

	private List<Hero> GetCurrentlyAvailableGuests()
	{
		FeastData feastData = FindActiveFeast();
		if (feastData == null || feastData.Attendees == null)
		{
			return new List<Hero>();
		}
		return ((IEnumerable<Hero>)feastData.Attendees).Where((Hero h) => h != null && h != _host && h != Hero.MainHero && h.CurrentSettlement == _settlement && !_spokenGuests.Contains(h) && !HasRewardCooldown(_host, h)).ToList();
	}

	private void CreateMingleTask()
	{
		_activeInteractionTask = true;
		_spokenGuests = new List<Hero>();
		_mingleInitialTargetCount = GetCurrentlyAvailableGuests().Count;
		UpdateMingleTaskLog();
	}

	private void UpdateMingleTaskLog()
	{
		List<Hero> currentlyAvailableGuests = GetCurrentlyAvailableGuests();
		TextObject val = LanguageTranslater.L.T("feast_q_mingle_task", "Mingle with guests");
		TextObject val2;
		if (currentlyAvailableGuests.Count == 0)
		{
			val2 = LanguageTranslater.L.T("feast_q_mingle_complete", "You have mingled with all available guests.");
		}
		else
		{
			string text = string.Join(", ", currentlyAvailableGuests.Select((Hero h) => ((object)h.Name).ToString()));
			val2 = LanguageTranslater.L.T("feast_q_mingle_next", "You should speak to {LIST}.");
			val2.SetTextVariable("LIST", text);
		}
		string text2 = ((object)val2).ToString();
		int count = _spokenGuests.Count;
		if (_lastMingleLogText != text2)
		{
			if (_interactionLog != null)
			{
				((QuestBase)this).RemoveLog(_interactionLog);
			}
			_interactionLog = ((QuestBase)this).AddDiscreteLog(val2, val, _spokenGuests.Count, _mingleInitialTargetCount, (TextObject)null, false);
			_lastMingleLogText = text2;
		}
	}

	private void TryCompleteInteractionForCurrentConversation()
	{
		if (!_activeInteractionTask)
		{
			return;
		}
		if (_isPlayerHost)
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			if (oneToOneConversationHero != null && oneToOneConversationHero != Hero.MainHero && !_spokenGuests.Contains(oneToOneConversationHero))
			{
				if (!HasRewardCooldown(Hero.MainHero, oneToOneConversationHero))
				{
					GiveRewardWithCooldownRespect(Hero.MainHero, oneToOneConversationHero);
				}
				_spokenGuests.Add(oneToOneConversationHero);
				UpdateMingleTaskLog();
				if (GetCurrentlyAvailableGuests().Count == 0)
				{
					_activeInteractionTask = false;
				}
			}
			return;
		}
		if (_host != null && !HasRewardCooldown(_host, Hero.MainHero))
		{
			GiveRewardWithCooldownRespect(_host, Hero.MainHero);
		}
		_activeInteractionTask = false;
		_timesSpokenToHost++;
		if (!_firstInteractionCompleted)
		{
			_firstInteractionCompleted = true;
		}
		if (_interactionLog != null)
		{
			((QuestBase)this).RemoveLog(_interactionLog);
			_interactionLog = null;
		}
		((QuestBase)this).AddLog(GetPlayerSpokeWithHostLogText(), false);
		if (_timesSpokenToHost > 1)
		{
			TextObject val = LanguageTranslater.L.T("feast_q_cooldown_info", "You must wait {DAYS} days before speaking with {QUEST_GIVER.NAME} again.");
			val.SetTextVariable("DAYS", CooldownDays);
			StringHelpers.SetCharacterProperties("QUEST_GIVER", ((QuestBase)this).QuestGiver.CharacterObject, val, false);
			MBInformationManager.AddQuickInformation(val, 0, (BasicCharacterObject)null, "");
		}
	}

	public static void RegisterFeastQuestDialogs(CampaignGameStarter starter)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Expected O, but got Unknown
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Expected O, but got Unknown
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Expected O, but got Unknown
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Expected O, but got Unknown
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Expected O, but got Unknown
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Expected O, but got Unknown
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Expected O, but got Unknown
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Expected O, but got Unknown
		object obj = _003C_003Ec._003C_003E9__54_0;
		if (obj == null)
		{
			OnConditionDelegate val = () => IsPlayerAttendingAIHostFeast() && IsFirstTimePlayerTalksToAIHost();
			_003C_003Ec._003C_003E9__54_0 = val;
			obj = (object)val;
		}
		starter.AddDialogLine("feastquest_aihost_greet_player_first", "start", "feastquest_aihost_menu", "{=feast_q_host_greet}Ah, {PLAYER.NAME}! Welcome to my feast. [ib:hip][if:convo_excited]The festivities are in full swing!", (OnConditionDelegate)obj, (OnConsequenceDelegate)null, 125, (OnClickableConditionDelegate)null);
		object obj2 = _003C_003Ec._003C_003E9__54_1;
		if (obj2 == null)
		{
			OnConditionDelegate val2 = () => IsPlayerAttendingAIHostFeast() && !IsFirstTimePlayerTalksToAIHost();
			_003C_003Ec._003C_003E9__54_1 = val2;
			obj2 = (object)val2;
		}
		starter.AddDialogLine("feastquest_aihost_greet_player_return", "start", "feastquest_aihost_menu", "{=feast_q_host_greet_return}Glad to see you're still here, {PLAYER.NAME}! [ib:normal][if:convo_approving]I hope you're enjoying yourself.", (OnConditionDelegate)obj2, (OnConsequenceDelegate)null, 125, (OnClickableConditionDelegate)null);
		object obj3 = _003C_003Ec._003C_003E9__54_2;
		if (obj3 == null)
		{
			OnConditionDelegate val3 = () => ShouldCompleteInteractionForCurrentConversation();
			_003C_003Ec._003C_003E9__54_2 = val3;
			obj3 = (object)val3;
		}
		starter.AddPlayerLine("feastquest_aihost_player_respond", "feastquest_aihost_menu", "feastquest_aihost_response", "{=feast_q_player_speak}Thank you for the invitation. I am honored to be here.", (OnConditionDelegate)obj3, (OnConsequenceDelegate)null, 100, (OnClickableConditionDelegate)null, (OnPersuasionOptionDelegate)null);
		object obj4 = _003C_003Ec._003C_003E9__54_3;
		if (obj4 == null)
		{
			OnConsequenceDelegate val4 = delegate
			{
				CompleteInteractionForCurrentConversation();
			};
			_003C_003Ec._003C_003E9__54_3 = val4;
			obj4 = (object)val4;
		}
		starter.AddDialogLine("feastquest_aihost_response_line", "feastquest_aihost_response", "feastquest_duel_offer", "{=feast_q_host_response}The honor is mine. [ib:normal][if:convo_approving]Enjoy yourself, and feel free to mingle with the other guests.", (OnConditionDelegate)null, (OnConsequenceDelegate)obj4, 100, (OnClickableConditionDelegate)null);
		object obj5 = _003C_003Ec._003C_003E9__54_4;
		if (obj5 == null)
		{
			OnConditionDelegate val5 = () => IsPlayerHostTalkingToGuest();
			_003C_003Ec._003C_003E9__54_4 = val5;
			obj5 = (object)val5;
		}
		starter.AddDialogLine("feastquest_guest_greet_playerhost", "start", "feastquest_guest_menu", "{=feast_q_guest_greet}It is an honour to be at your feast! [ib:hip][if:convo_approving]The hall is splendid.", (OnConditionDelegate)obj5, (OnConsequenceDelegate)null, 120, (OnClickableConditionDelegate)null);
		object obj6 = _003C_003Ec._003C_003E9__54_5;
		if (obj6 == null)
		{
			OnConditionDelegate val6 = () => ShouldCompleteInteractionForCurrentConversation() && !HasRewardCooldown(Hero.MainHero, Hero.OneToOneConversationHero);
			_003C_003Ec._003C_003E9__54_5 = val6;
			obj6 = (object)val6;
		}
		starter.AddPlayerLine("feastquest_playerhost_respond_guest", "feastquest_guest_menu", "feastquest_guest_response", "{=feast_q_player_host_ack}Welcome, enjoy the feast. Make yourself at home.", (OnConditionDelegate)obj6, (OnConsequenceDelegate)null, 110, (OnClickableConditionDelegate)null, (OnPersuasionOptionDelegate)null);
		object obj7 = _003C_003Ec._003C_003E9__54_6;
		if (obj7 == null)
		{
			OnConsequenceDelegate val7 = delegate
			{
				CompleteInteractionForCurrentConversation();
			};
			_003C_003Ec._003C_003E9__54_6 = val7;
			obj7 = (object)val7;
		}
		starter.AddDialogLine("feastquest_guest_response_line", "feastquest_guest_response", "feastquest_duel_offer", "{=feast_q_guest_response}Thank you. I shall enjoy the company.", (OnConditionDelegate)null, (OnConsequenceDelegate)obj7, 100, (OnClickableConditionDelegate)null);
		object obj8 = _003C_003Ec._003C_003E9__54_7;
		if (obj8 == null)
		{
			OnConditionDelegate val8 = () => ShouldShowDuelCommentDialog(win: true);
			_003C_003Ec._003C_003E9__54_7 = val8;
			obj8 = (object)val8;
		}
		object obj9 = _003C_003Ec._003C_003E9__54_8;
		if (obj9 == null)
		{
			OnConsequenceDelegate val9 = delegate
			{
				ClearDuelCommentFlag();
			};
			_003C_003Ec._003C_003E9__54_8 = val9;
			obj9 = (object)val9;
		}
		starter.AddDialogLine("feastquest_duel_comment_win", "start", "feastquest_host_menu", "{=feast_q_duel_comment_win}[if:convo_excited]{PLAYER.NAME}, well fought! You bested me in the arena. The guests are still talking about it.", (OnConditionDelegate)obj8, (OnConsequenceDelegate)obj9, 150, (OnClickableConditionDelegate)null);
		object obj10 = _003C_003Ec._003C_003E9__54_9;
		if (obj10 == null)
		{
			OnConditionDelegate val10 = () => ShouldShowDuelCommentDialog(win: false);
			_003C_003Ec._003C_003E9__54_9 = val10;
			obj10 = (object)val10;
		}
		object obj11 = _003C_003Ec._003C_003E9__54_10;
		if (obj11 == null)
		{
			OnConsequenceDelegate val11 = delegate
			{
				ClearDuelCommentFlag();
			};
			_003C_003Ec._003C_003E9__54_10 = val11;
			obj11 = (object)val11;
		}
		starter.AddDialogLine("feastquest_duel_comment_lose", "start", "feastquest_host_menu", "{=feast_q_duel_comment_lose}[if:convo_proud]A good effort, {PLAYER.NAME}, but victory was mine this time. Perhaps a rematch later?", (OnConditionDelegate)obj10, (OnConsequenceDelegate)obj11, 150, (OnClickableConditionDelegate)null);
		object obj12 = _003C_003Ec._003C_003E9__54_11;
		if (obj12 == null)
		{
			OnConditionDelegate val12 = () => ShouldOfferDuelForCurrentConversation();
			_003C_003Ec._003C_003E9__54_11 = val12;
			obj12 = (object)val12;
		}
		starter.AddDialogLine("feastquest_duel_offer_line", "feastquest_duel_offer", "feastquest_duel_player_response", "{=feast_q_duel_offer}Say, {PLAYER.NAME}, care for a friendly bout? [ib:confident][if:convo_excited]Nothing like a bit of steel to liven up a feast!", (OnConditionDelegate)obj12, (OnConsequenceDelegate)null, 100, (OnClickableConditionDelegate)null);
		object obj13 = _003C_003Ec._003C_003E9__54_12;
		if (obj13 == null)
		{
			OnConditionDelegate val13 = () => true;
			_003C_003Ec._003C_003E9__54_12 = val13;
			obj13 = (object)val13;
		}
		object obj14 = _003C_003Ec._003C_003E9__54_13;
		if (obj14 == null)
		{
			OnConsequenceDelegate val14 = delegate
			{
				AcceptHostDuelChallenge();
			};
			_003C_003Ec._003C_003E9__54_13 = val14;
			obj14 = (object)val14;
		}
		starter.AddPlayerLine("feastquest_player_accept_duel", "feastquest_duel_player_response", "feastquest_duel_meet_arena", "{=feast_q_player_accept_duel}I accept your challenge!", (OnConditionDelegate)obj13, (OnConsequenceDelegate)obj14, 100, (OnClickableConditionDelegate)null, (OnPersuasionOptionDelegate)null);
		object obj15 = _003C_003Ec._003C_003E9__54_14;
		if (obj15 == null)
		{
			OnConditionDelegate val15 = () => true;
			_003C_003Ec._003C_003E9__54_14 = val15;
			obj15 = (object)val15;
		}
		starter.AddDialogLine("feastquest_duel_meet_arena", "feastquest_duel_meet_arena", "close_window", "{=feast_q_duel_meet_arena}Meet me in the arena to begin your duel.", (OnConditionDelegate)obj15, (OnConsequenceDelegate)null, 100, (OnClickableConditionDelegate)null);
		object obj16 = _003C_003Ec._003C_003E9__54_15;
		if (obj16 == null)
		{
			OnConditionDelegate val16 = () => true;
			_003C_003Ec._003C_003E9__54_15 = val16;
			obj16 = (object)val16;
		}
		object obj17 = _003C_003Ec._003C_003E9__54_16;
		if (obj17 == null)
		{
			OnConsequenceDelegate val17 = delegate
			{
				DeclineHostDuelChallenge();
			};
			_003C_003Ec._003C_003E9__54_16 = val17;
			obj17 = (object)val17;
		}
		starter.AddPlayerLine("feastquest_player_decline_duel", "feastquest_duel_player_response", "close_window", "{=feast_q_player_decline_duel}Perhaps another time.", (OnConditionDelegate)obj16, (OnConsequenceDelegate)obj17, 100, (OnClickableConditionDelegate)null, (OnPersuasionOptionDelegate)null);
		object obj18 = _003C_003Ec._003C_003E9__54_17;
		if (obj18 == null)
		{
			OnConditionDelegate val18 = () => !ShouldOfferDuelForCurrentConversation();
			_003C_003Ec._003C_003E9__54_17 = val18;
			obj18 = (object)val18;
		}
		starter.AddDialogLine("feastquest_no_duel", "feastquest_duel_offer", "close_window", (string)null, (OnConditionDelegate)obj18, (OnConsequenceDelegate)null, 100, (OnClickableConditionDelegate)null);
	}

	private static bool IsPlayerAttendingAIHostFeast()
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		if (oneToOneConversationHero == null)
		{
			return false;
		}
		FeastQuest feastQuest = FindForConversationHost();
		if (feastQuest == null || !((QuestBase)feastQuest).IsOngoing || feastQuest._isPlayerHost)
		{
			return false;
		}
		if (!feastQuest._travelCompleted)
		{
			return false;
		}
		return feastQuest._host != null && oneToOneConversationHero == feastQuest._host;
	}

	private static bool IsPlayerHostTalkingToGuest()
	{
		Hero talkTo = Hero.OneToOneConversationHero;
		if (talkTo == null)
		{
			return false;
		}
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f != null && f.Host == Hero.MainHero && f.Attendees != null && ((List<Hero>)(object)f.Attendees).Contains(talkTo));
		if (feastByAttribute == null)
		{
			return false;
		}
		FeastQuest value;
		return ActiveByHost.TryGetValue(feastByAttribute.Host, out value) && value != null && ((QuestBase)value).IsOngoing && value._isPlayerHost;
	}

	private static bool IsFirstTimePlayerTalksToAIHost()
	{
		FeastQuest feastQuest = FindForConversationHost();
		if (feastQuest == null)
		{
			return false;
		}
		return feastQuest._travelCompleted && feastQuest._activeInteractionTask && !feastQuest._firstInteractionCompleted;
	}

	private static bool ShouldCompleteInteractionForCurrentConversation()
	{
		FeastQuest feastQuest = FindForConversationHost();
		return feastQuest != null && ((QuestBase)feastQuest).IsOngoing && feastQuest._activeInteractionTask;
	}

	private static void CompleteInteractionForCurrentConversation()
	{
		FindForConversationHost()?.TryCompleteInteractionForCurrentConversation();
	}

	private static FeastQuest FindForConversationHost()
	{
		Hero talkTo = Hero.OneToOneConversationHero;
		if (talkTo == null)
		{
			return null;
		}
		if (ActiveByHost.TryGetValue(talkTo, out FeastQuest value))
		{
			return value;
		}
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f != null && f.Attendees != null && ((List<Hero>)(object)f.Attendees).Contains(talkTo) && f.Host == Hero.MainHero);
		if (feastByAttribute != null && ActiveByHost.TryGetValue(feastByAttribute.Host, out FeastQuest value2))
		{
			return value2;
		}
		return null;
	}

	private static bool ShouldShowDuelCommentDialog(bool win)
	{
		return FeastDuelSystem.PendingDuelComment && Hero.OneToOneConversationHero != null && FeastDuelSystem.LastDuelHostId == ((MBObjectBase)Hero.OneToOneConversationHero).StringId && FeastDuelSystem.LastDuelWasPlayerWin == win;
	}

	private static void ClearDuelCommentFlag()
	{
		FeastDuelSystem.PendingDuelComment = false;
	}

	private static bool ShouldOfferDuelForCurrentConversation()
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		Hero mainHero = Hero.MainHero;
		if (oneToOneConversationHero == null || mainHero == null)
		{
			return false;
		}
		if (MBRandom.RandomFloatRanged(0f, 1f) >= 0.3f)
		{
			return false;
		}
		return FeastDuelSystem.CanDuel(oneToOneConversationHero, mainHero);
	}

	private static void AcceptHostDuelChallenge()
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		Hero mainHero = Hero.MainHero;
		Settlement currentSettlement = Hero.MainHero.CurrentSettlement;
		if (oneToOneConversationHero != null && mainHero != null && currentSettlement != null)
		{
			FeastDuelSystem.PendingFeastDuel = true;
			FeastDuelSystem.PendingDuelHost = oneToOneConversationHero;
			FeastDuelSystem.PendingDuelPlayer = mainHero;
			FeastDuelSystem.PendingDuelSettlement = currentSettlement;
		}
	}

	private static void DeclineHostDuelChallenge()
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		Hero mainHero = Hero.MainHero;
		Settlement settlement = ((mainHero != null) ? mainHero.CurrentSettlement : null);
		if (oneToOneConversationHero == null || mainHero == null || settlement == null)
		{
			return;
		}
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f.Settlement == settlement);
		if (feastByAttribute != null)
		{
			if (!FeastBehavior._lastFeastRewardTimes.TryGetValue(feastByAttribute, out RewardData value) || value == null)
			{
				value = new RewardData();
				FeastBehavior._lastFeastRewardTimes[feastByAttribute] = value;
			}
			RewardData.RewardInfo orCreate = value.GetOrCreate(oneToOneConversationHero, mainHero);
			CampaignTime now = CampaignTime.Now;
			orCreate.nextDuelRewardTime = ((CampaignTime)(ref now)).ToDays + (double)GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastDialogCooldown;
		}
		int relation = oneToOneConversationHero.GetRelation(mainHero);
		if (relation >= 20)
		{
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_duel_declined_friendly", "{CHALLENGER} nods understandingly. \"Another time, perhaps!\"").SetTextVariable("CHALLENGER", oneToOneConversationHero.Name)).ToString(), Colors.White));
		}
		else if (relation <= -20)
		{
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(oneToOneConversationHero, mainHero, -1, true);
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_duel_declined_hostile", "{CHALLENGER} scoffs. \"As I thought. All talk, no action.\"").SetTextVariable("CHALLENGER", oneToOneConversationHero.Name)).ToString(), Colors.Yellow));
		}
		else
		{
			InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("feast_duel_declined_neutral", "{CHALLENGER} looks slightly disappointed but shrugs it off.").SetTextVariable("CHALLENGER", oneToOneConversationHero.Name)).ToString(), Colors.White));
		}
	}
}
