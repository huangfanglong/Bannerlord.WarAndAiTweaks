using System;
using System.Collections.Generic;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using WarAndAiTweaks;

namespace Strategic4XDiplomacy;

public class FiefRelationshipBehavior : CampaignBehaviorBase
{
	public override void RegisterEvents()
	{
		CampaignEvents.DailyTickClanEvent.AddNonSerializedListener((object)this, (Action<Clan>)DailyTickClanEvent);
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	private void DailyTickClanEvent(Clan clan)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		if (clan != null && clan.Kingdom != null && ((List<Settlement>)(object)clan.Settlements).Count <= 0 && !clan.IsUnderMercenaryService && clan.Leader != Hero.MainHero)
		{
			int fieflessClansLoss = GlobalSettings<WarAndAiTweaksSettings>.Instance.FieflessClansLoss;
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(clan.Leader, clan.Kingdom.Leader, fieflessClansLoss, true);
			if (clan.Kingdom.Leader == Hero.MainHero)
			{
				TextObject val = LanguageTranslater.L.T("relationship_fiefless_clan_loss", "Your relation with {CLAN} has decreased by {AMOUNT} because they have no fiefs.");
				val.SetTextVariable("CLAN", ((object)clan.Name).ToString());
				val.SetTextVariable("AMOUNT", fieflessClansLoss);
				InformationManager.DisplayMessage(new InformationMessage(((object)val).ToString(), Colors.Red));
			}
		}
	}
}
