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

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_0;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_1;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_2;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__54_3;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_4;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_5;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__54_6;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_7;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__54_8;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_9;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__54_10;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_11;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_12;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__54_13;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_14;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_15;

		public static ConversationSentence.OnConsequenceDelegate _003C_003E9__54_16;

		public static ConversationSentence.OnConditionDelegate _003C_003E9__54_17;

		internal void _003COnGameMenuOpened_003Eb__47_0(CharacterObject winner)
		{
			if (winner == CharacterObject.PlayerCharacter)
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("duel_won", "[if:convo_excited]{PLAYER.NAME}, well fought! You bested me in the arena. The guests are still talking about it.").SetTextVariable("OPPONENT", ((object)FeastDuelSystem.PendingDuelHost.Name).ToString())).ToString(), Colors.Green));
				Hero.MainHero.AddSkillXp(DefaultSkills.OneHanded, 100f);
				GiveGoldAction.ApplyBetweenCharacters(FeastDuelSystem.PendingDuelHost, Hero.MainHero, 200, false);
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(FeastDuelSystem.PendingDuelHost, Hero.MainHero, 2, true);
				MBInformationManager.AddQuickInformation(LanguageTranslater.L.T("feast_q_duel_reward", "You gained 200 gold, 2 relation, and 100 One Handed XP!"), 0, null, null);
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
			val.SetTextVariable("SETTLEMENT", ((settlement != null) ? settlement.Name : null) ?? new TextObject("", null));
			return val;
		}
	}

	public override bool IsRemainingTimeHidden => true;

	public override string SpecialQuestType => "SaintsWarAndAiTweaks.Feast";

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
			double num = nextFeastRewardTime - now.ToDays;
			return num > 0.0;
		}
		return false;
	}

	public static void GiveRewardWithCooldownRespect(Hero host, Hero guest)
	{
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
			orCreate.nextFeastRewardTime = now.ToDays + (double)GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastDialogCooldown;
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(host2, guest2, 1, true);
			GainRenownAction.Apply(host2, 3f, false);
			GainRenownAction.Apply(guest2, 3f, false);
		}
	}

	private TextObject GetQuestStartedLogText()
	{
		if (_isPlayerHost)
		{
			TextObject val = LanguageTranslater.L.T("feast_q_host_started", "You are hosting a feast at {SETTLEMENT}.");
			if (_settlement != null)
			{
				val.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
			}
			if (string.IsNullOrEmpty(((object)val).ToString()))
			{
				Debug.Print("[FeastQuest] Warning: feast_q_host_started returned empty text", 0, (Debug.DebugColor)12, 17592186044416uL);
				return new TextObject("You are hosting a feast at {SETTLEMENT}.", (Dictionary<string, object>)null);
			}
			return val;
		}
		TextObject val2 = LanguageTranslater.L.T("feast_q_start", "{QUEST_GIVER.NAME} has invited you to a feast at {SETTLEMENT}. {?QUEST_GIVER.GENDER}She{?}He{\\?} asks that you travel there and speak with {?QUEST_GIVER.GENDER}her{?}him{\\?} to enjoy the festivities and build relations with the realm's nobility.");
		Hero questGiver = base.QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, val2, false);
		}
		if (_settlement != null)
		{
			val2.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
		}
		if (string.IsNullOrEmpty(((object)val2).ToString()))
		{
			Debug.Print("[FeastQuest] Warning: feast_q_start returned empty text", 0, (Debug.DebugColor)12, 17592186044416uL);
			return new TextObject("{QUEST_GIVER.NAME} has invited you to a feast at {SETTLEMENT}.", (Dictionary<string, object>)null);
		}
		return val2;
	}

	private TextObject GetQuestConcludedLogText()
	{
		TextObject val = LanguageTranslater.L.T("feast_q_success", "You attended the feast hosted by {QUEST_GIVER.NAME} at {SETTLEMENT}. Your participation has strengthened your bonds with the nobles of the realm.");
		Hero questGiver = base.QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, val, false);
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
		Hero questGiver = base.QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, val, false);
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
		Hero questGiver = base.QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, val2, false);
		}
		if (_settlement != null)
		{
			val2.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
		}
		return val2;
	}

	private TextObject GetPlayerSpokeWithHostLogText()
	{
		TextObject val = LanguageTranslater.L.T("feast_q_spoke", "You spoke with {QUEST_GIVER.NAME} at the feast. {?QUEST_GIVER.GENDER}She{?}He{\\?} appreciates your attendance and conversation. Stay and enjoy the festivities while time passes.");
		Hero questGiver = base.QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, val, false);
		}
		if (string.IsNullOrEmpty(((object)val).ToString()))
		{
			Debug.Print("[FeastQuest] Warning: feast_q_spoke returned empty text", 0, (Debug.DebugColor)12, 17592186044416uL);
			return new TextObject("You spoke with {QUEST_GIVER.NAME} at the feast.", (Dictionary<string, object>)null);
		}
		return val;
	}

	private TextObject GetCooldownExpiredLogText()
	{
		TextObject val = LanguageTranslater.L.T("feast_q_cooldown", "Enough time has passed. You may speak with {QUEST_GIVER.NAME} again to continue building your relationship.");
		Hero questGiver = base.QuestGiver;
		if (((questGiver != null) ? questGiver.CharacterObject : null) != null)
		{
			StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, val, false);
		}
		return val;
	}

	public FeastQuest(string questId, Hero questGiver, Hero host, Settlement feastSettlement, CampaignTime feastStart, bool isPlayerHost = false)
		: base(questId, questGiver, CampaignTime.DaysFromNow(365f), 0)
	{
		_host = host;
		_settlement = feastSettlement;
		_feastStart = feastStart;
		_isPlayerHost = isPlayerHost;
		_timesSpokenToHost = 0;
		if (_host != null && ActiveByHost.TryGetValue(_host, out FeastQuest value) && value.IsOngoing)
		{
			value.CompleteQuestWithCancel(LanguageTranslater.L.T("feast_q_replaced", "A new feast invitation has replaced the previous one."));
		}
		ActiveByHost[_host] = this;
		base.StartQuest();
		TextObject val = LanguageTranslater.L.T("feast_q_start", "{QUEST_GIVER.NAME} has invited you to a feast at {SETTLEMENT}. {?QUEST_GIVER.GENDER}She{?}He{\\?} asks that you travel there and speak with {?QUEST_GIVER.GENDER}her{?}him{\\?} to enjoy the festivities and build relations with the realm's nobility.");
		Debug.Print("[FeastQuest] LanguageTranslater test - feast_q_start: '" + ((object)val).ToString() + "'", 0, (Debug.DebugColor)12, 17592186044416uL);
		base.AddLog(GetQuestStartedLogText(), false);
		TextObject val2 = (_isPlayerHost ? LanguageTranslater.L.T("feast_q_host_travel", "Travel to {SETTLEMENT} to start your feast.") : LanguageTranslater.L.T("feast_q_travel", "Travel to {SETTLEMENT} to attend the feast."));
		if (_settlement != null)
		{
			val2.SetTextVariable("SETTLEMENT", _settlement.EncyclopediaLinkWithName);
			_travelLog = base.AddLog(val2, false);
			base.AddTrackedObject((ITrackableCampaignObject)(object)_settlement);
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
		if (!base.IsOngoing)
		{
			return;
		}
		if (!IsFeastStillValid())
		{
			if (_isPlayerHost)
			{
				Hero.MainHero.AddSkillXp(DefaultSkills.Leadership, 50f);
				GiveGoldAction.ApplyBetweenCharacters((Hero)null, Hero.MainHero, 1000, false);
				base.AddLog(LanguageTranslater.L.T("feast_q_host_success", "Your feast was a success! You gained leadership XP and gold."), false);
			}
			base.AddLog(GetQuestConcludedLogText(), false);
			base.CompleteQuestWithSuccess();
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
		if (_settlement != null && base.IsTracked((ITrackableCampaignObject)(object)_settlement))
		{
			base.RemoveTrackedObject((ITrackableCampaignObject)(object)_settlement);
		}
		if (_host != null && ActiveByHost.TryGetValue(_host, out FeastQuest value) && value == this)
		{
			ActiveByHost.Remove(_host);
		}
		if (_interactionLog != null)
		{
			base.RemoveLog(_interactionLog);
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

	public override void OnHeroCanHaveCampaignIssuesInfoIsRequested(Hero hero, ref bool result)
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
			base.AddLog(GetQuestCanceledDueToWarLogText(), false);
			base.CompleteQuestWithCancel((TextObject)null);
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
				base.AddLog(GetQuestCanceledDueToWarLogText(), false);
				base.CompleteQuestWithCancel((TextObject)null);
			}
		}
	}

	private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
	{
		if (base.IsOngoing && party == MobileParty.MainParty && settlement == _settlement && !_travelCompleted)
		{
			_travelCompleted = true;
			if (_travelLog != null && base.IsTracked((ITrackableCampaignObject)(object)_settlement))
			{
				base.RemoveTrackedObject((ITrackableCampaignObject)(object)_settlement);
			}
			base.AddLog(GetPlayerArrivedAtFeastLogText(), false);
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
			if (winner == CharacterObject.PlayerCharacter)
			{
				InformationManager.DisplayMessage(new InformationMessage(((object)LanguageTranslater.L.T("duel_won", "[if:convo_excited]{PLAYER.NAME}, well fought! You bested me in the arena. The guests are still talking about it.").SetTextVariable("OPPONENT", ((object)FeastDuelSystem.PendingDuelHost.Name).ToString())).ToString(), Colors.Green));
				Hero.MainHero.AddSkillXp(DefaultSkills.OneHanded, 100f);
				GiveGoldAction.ApplyBetweenCharacters(FeastDuelSystem.PendingDuelHost, Hero.MainHero, 200, false);
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(FeastDuelSystem.PendingDuelHost, Hero.MainHero, 2, true);
				MBInformationManager.AddQuickInformation(LanguageTranslater.L.T("feast_q_duel_reward", "You gained 200 gold, 2 relation, and 100 One Handed XP!"), 0, null, null);
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
		StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, val, false);
		if (!firstTime)
		{
			base.AddLog(GetCooldownExpiredLogText(), false);
		}
		_interactionLog = base.AddLog(val, false);
		if (_settlement != null && !base.IsTracked((ITrackableCampaignObject)(object)_settlement))
		{
			base.AddTrackedObject((ITrackableCampaignObject)(object)_settlement);
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
				base.RemoveLog(_interactionLog);
			}
			_interactionLog = base.AddDiscreteLog(val2, val, _spokenGuests.Count, _mingleInitialTargetCount, (TextObject)null, false);
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
			base.RemoveLog(_interactionLog);
			_interactionLog = null;
		}
		base.AddLog(GetPlayerSpokeWithHostLogText(), false);
		if (_timesSpokenToHost > 1)
		{
			TextObject val = LanguageTranslater.L.T("feast_q_cooldown_info", "You must wait {DAYS} days before speaking with {QUEST_GIVER.NAME} again.");
			val.SetTextVariable("DAYS", CooldownDays);
			StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, val, false);
			MBInformationManager.AddQuickInformation(val, 0, null, null);
		}
	}

	public static void RegisterFeastQuestDialogs(CampaignGameStarter starter)
	{
		object obj = _003C_003Ec._003C_003E9__54_0;
		if (obj == null)
		{
			ConversationSentence.OnConditionDelegate val = () => IsPlayerAttendingAIHostFeast() && IsFirstTimePlayerTalksToAIHost();
			_003C_003Ec._003C_003E9__54_0 = val;
			obj = (object)val;
		}
		starter.AddDialogLine("feastquest_aihost_greet_player_first", "start", "feastquest_aihost_menu", "{=feast_q_host_greet}Ah, {PLAYER.NAME}! Welcome to my feast. [ib:hip][if:convo_excited]The festivities are in full swing!", (ConversationSentence.OnConditionDelegate)obj, (ConversationSentence.OnConsequenceDelegate)null, 125, null);
		object obj2 = _003C_003Ec._003C_003E9__54_1;
		if (obj2 == null)
		{
			ConversationSentence.OnConditionDelegate val2 = () => IsPlayerAttendingAIHostFeast() && !IsFirstTimePlayerTalksToAIHost();
			_003C_003Ec._003C_003E9__54_1 = val2;
			obj2 = (object)val2;
		}
		starter.AddDialogLine("feastquest_aihost_greet_player_return", "start", "feastquest_aihost_menu", "{=feast_q_host_greet_return}Glad to see you're still here, {PLAYER.NAME}! [ib:normal][if:convo_approving]I hope you're enjoying yourself.", (ConversationSentence.OnConditionDelegate)obj2, (ConversationSentence.OnConsequenceDelegate)null, 125, null);
		object obj3 = _003C_003Ec._003C_003E9__54_2;
		if (obj3 == null)
		{
			ConversationSentence.OnConditionDelegate val3 = () => ShouldCompleteInteractionForCurrentConversation();
			_003C_003Ec._003C_003E9__54_2 = val3;
			obj3 = (object)val3;
		}
		starter.AddPlayerLine("feastquest_aihost_player_respond", "feastquest_aihost_menu", "feastquest_aihost_response", "{=feast_q_player_speak}Thank you for the invitation. I am honored to be here.", (ConversationSentence.OnConditionDelegate)obj3, (ConversationSentence.OnConsequenceDelegate)null, 100, null, null);
		object obj4 = _003C_003Ec._003C_003E9__54_3;
		if (obj4 == null)
		{
			ConversationSentence.OnConsequenceDelegate val4 = delegate
			{
				CompleteInteractionForCurrentConversation();
			};
			_003C_003Ec._003C_003E9__54_3 = val4;
			obj4 = (object)val4;
		}
		starter.AddDialogLine("feastquest_aihost_response_line", "feastquest_aihost_response", "feastquest_duel_offer", "{=feast_q_host_response}The honor is mine. [ib:normal][if:convo_approving]Enjoy yourself, and feel free to mingle with the other guests.", (ConversationSentence.OnConditionDelegate)null, (ConversationSentence.OnConsequenceDelegate)obj4, 100, null);
		object obj5 = _003C_003Ec._003C_003E9__54_4;
		if (obj5 == null)
		{
			ConversationSentence.OnConditionDelegate val5 = () => IsPlayerHostTalkingToGuest();
			_003C_003Ec._003C_003E9__54_4 = val5;
			obj5 = (object)val5;
		}
		starter.AddDialogLine("feastquest_guest_greet_playerhost", "start", "feastquest_guest_menu", "{=feast_q_guest_greet}It is an honour to be at your feast! [ib:hip][if:convo_approving]The hall is splendid.", (ConversationSentence.OnConditionDelegate)obj5, (ConversationSentence.OnConsequenceDelegate)null, 120, null);
		object obj6 = _003C_003Ec._003C_003E9__54_5;
		if (obj6 == null)
		{
			ConversationSentence.OnConditionDelegate val6 = () => ShouldCompleteInteractionForCurrentConversation() && !HasRewardCooldown(Hero.MainHero, Hero.OneToOneConversationHero);
			_003C_003Ec._003C_003E9__54_5 = val6;
			obj6 = (object)val6;
		}
		starter.AddPlayerLine("feastquest_playerhost_respond_guest", "feastquest_guest_menu", "feastquest_guest_response", "{=feast_q_player_host_ack}Welcome, enjoy the feast. Make yourself at home.", (ConversationSentence.OnConditionDelegate)obj6, (ConversationSentence.OnConsequenceDelegate)null, 110, null, null);
		object obj7 = _003C_003Ec._003C_003E9__54_6;
		if (obj7 == null)
		{
			ConversationSentence.OnConsequenceDelegate val7 = delegate
			{
				CompleteInteractionForCurrentConversation();
			};
			_003C_003Ec._003C_003E9__54_6 = val7;
			obj7 = (object)val7;
		}
		starter.AddDialogLine("feastquest_guest_response_line", "feastquest_guest_response", "feastquest_duel_offer", "{=feast_q_guest_response}Thank you. I shall enjoy the company.", (ConversationSentence.OnConditionDelegate)null, (ConversationSentence.OnConsequenceDelegate)obj7, 100, null);
		object obj8 = _003C_003Ec._003C_003E9__54_7;
		if (obj8 == null)
		{
			ConversationSentence.OnConditionDelegate val8 = () => ShouldShowDuelCommentDialog(win: true);
			_003C_003Ec._003C_003E9__54_7 = val8;
			obj8 = (object)val8;
		}
		object obj9 = _003C_003Ec._003C_003E9__54_8;
		if (obj9 == null)
		{
			ConversationSentence.OnConsequenceDelegate val9 = delegate
			{
				ClearDuelCommentFlag();
			};
			_003C_003Ec._003C_003E9__54_8 = val9;
			obj9 = (object)val9;
		}
		starter.AddDialogLine("feastquest_duel_comment_win", "start", "feastquest_host_menu", "{=feast_q_duel_comment_win}[if:convo_excited]{PLAYER.NAME}, well fought! You bested me in the arena. The guests are still talking about it.", (ConversationSentence.OnConditionDelegate)obj8, (ConversationSentence.OnConsequenceDelegate)obj9, 150, null);
		object obj10 = _003C_003Ec._003C_003E9__54_9;
		if (obj10 == null)
		{
			ConversationSentence.OnConditionDelegate val10 = () => ShouldShowDuelCommentDialog(win: false);
			_003C_003Ec._003C_003E9__54_9 = val10;
			obj10 = (object)val10;
		}
		object obj11 = _003C_003Ec._003C_003E9__54_10;
		if (obj11 == null)
		{
			ConversationSentence.OnConsequenceDelegate val11 = delegate
			{
				ClearDuelCommentFlag();
			};
			_003C_003Ec._003C_003E9__54_10 = val11;
			obj11 = (object)val11;
		}
		starter.AddDialogLine("feastquest_duel_comment_lose", "start", "feastquest_host_menu", "{=feast_q_duel_comment_lose}[if:convo_proud]A good effort, {PLAYER.NAME}, but victory was mine this time. Perhaps a rematch later?", (ConversationSentence.OnConditionDelegate)obj10, (ConversationSentence.OnConsequenceDelegate)obj11, 150, null);
		object obj12 = _003C_003Ec._003C_003E9__54_11;
		if (obj12 == null)
		{
			ConversationSentence.OnConditionDelegate val12 = () => ShouldOfferDuelForCurrentConversation();
			_003C_003Ec._003C_003E9__54_11 = val12;
			obj12 = (object)val12;
		}
		starter.AddDialogLine("feastquest_duel_offer_line", "feastquest_duel_offer", "feastquest_duel_player_response", "{=feast_q_duel_offer}Say, {PLAYER.NAME}, care for a friendly bout? [ib:confident][if:convo_excited]Nothing like a bit of steel to liven up a feast!", (ConversationSentence.OnConditionDelegate)obj12, (ConversationSentence.OnConsequenceDelegate)null, 100, null);
		object obj13 = _003C_003Ec._003C_003E9__54_12;
		if (obj13 == null)
		{
			ConversationSentence.OnConditionDelegate val13 = () => true;
			_003C_003Ec._003C_003E9__54_12 = val13;
			obj13 = (object)val13;
		}
		object obj14 = _003C_003Ec._003C_003E9__54_13;
		if (obj14 == null)
		{
			ConversationSentence.OnConsequenceDelegate val14 = delegate
			{
				AcceptHostDuelChallenge();
			};
			_003C_003Ec._003C_003E9__54_13 = val14;
			obj14 = (object)val14;
		}
		starter.AddPlayerLine("feastquest_player_accept_duel", "feastquest_duel_player_response", "feastquest_duel_meet_arena", "{=feast_q_player_accept_duel}I accept your challenge!", (ConversationSentence.OnConditionDelegate)obj13, (ConversationSentence.OnConsequenceDelegate)obj14, 100, null, null);
		object obj15 = _003C_003Ec._003C_003E9__54_14;
		if (obj15 == null)
		{
			ConversationSentence.OnConditionDelegate val15 = () => true;
			_003C_003Ec._003C_003E9__54_14 = val15;
			obj15 = (object)val15;
		}
		starter.AddDialogLine("feastquest_duel_meet_arena", "feastquest_duel_meet_arena", "close_window", "{=feast_q_duel_meet_arena}Meet me in the arena to begin your duel.", (ConversationSentence.OnConditionDelegate)obj15, (ConversationSentence.OnConsequenceDelegate)null, 100, null);
		object obj16 = _003C_003Ec._003C_003E9__54_15;
		if (obj16 == null)
		{
			ConversationSentence.OnConditionDelegate val16 = () => true;
			_003C_003Ec._003C_003E9__54_15 = val16;
			obj16 = (object)val16;
		}
		object obj17 = _003C_003Ec._003C_003E9__54_16;
		if (obj17 == null)
		{
			ConversationSentence.OnConsequenceDelegate val17 = delegate
			{
				DeclineHostDuelChallenge();
			};
			_003C_003Ec._003C_003E9__54_16 = val17;
			obj17 = (object)val17;
		}
		starter.AddPlayerLine("feastquest_player_decline_duel", "feastquest_duel_player_response", "close_window", "{=feast_q_player_decline_duel}Perhaps another time.", (ConversationSentence.OnConditionDelegate)obj16, (ConversationSentence.OnConsequenceDelegate)obj17, 100, null, null);
		object obj18 = _003C_003Ec._003C_003E9__54_17;
		if (obj18 == null)
		{
			ConversationSentence.OnConditionDelegate val18 = () => !ShouldOfferDuelForCurrentConversation();
			_003C_003Ec._003C_003E9__54_17 = val18;
			obj18 = (object)val18;
		}
		starter.AddDialogLine("feastquest_no_duel", "feastquest_duel_offer", "close_window", (string)null, (ConversationSentence.OnConditionDelegate)obj18, (ConversationSentence.OnConsequenceDelegate)null, 100, null);
	}

	private static bool IsPlayerAttendingAIHostFeast()
	{
		Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
		if (oneToOneConversationHero == null)
		{
			return false;
		}
		FeastQuest feastQuest = FindForConversationHost();
		if (feastQuest == null || !feastQuest.IsOngoing || feastQuest._isPlayerHost)
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
		return ActiveByHost.TryGetValue(feastByAttribute.Host, out value) && value != null && value.IsOngoing && value._isPlayerHost;
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
		return feastQuest != null && feastQuest.IsOngoing && feastQuest._activeInteractionTask;
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
			orCreate.nextDuelRewardTime = now.ToDays + (double)GlobalSettings<WarAndAiTweaksSettings>.Instance.TodayWeFeastDialogCooldown;
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






