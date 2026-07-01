using System.Collections.Generic;
using FeastSystem;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;

public class FeastDuelSystem
{
	private static Settlement _pendingDuelSettlement;

	private static Hero _pendingDuelOpponentHero;

	private static Hero _pendingDuelChallengerHero;

	private Settlement _settlement;

	private Hero _opponentHero;

	private Hero _challengerHero;

	private bool _duelInitialized;

	internal static bool PendingFeastDuel;

	internal static Hero PendingDuelHost;

	internal static Hero PendingDuelPlayer;

	internal static Settlement PendingDuelSettlement;

	public static bool PendingDuelComment;

	public static bool LastDuelWasPlayerWin;

	public static string LastDuelHostId;

	public static void PrepareDuel(Hero challenger, Hero opponent, Settlement settlement)
	{
		_pendingDuelSettlement = settlement;
		_pendingDuelChallengerHero = challenger;
		_pendingDuelOpponentHero = opponent;
	}

	public static bool CanDuel(Hero challenger, Hero opponent)
	{
		if (challenger == null || opponent == null)
		{
			return false;
		}
		if (!challenger.IsAlive || !opponent.IsAlive || challenger.IsPrisoner || opponent.IsPrisoner)
		{
			return false;
		}
		if (challenger.CurrentSettlement != opponent.CurrentSettlement)
		{
			return false;
		}
		if (challenger.IsWounded || opponent.IsWounded)
		{
			return false;
		}
		Settlement currentSettlement = challenger.CurrentSettlement;
		object obj;
		if (currentSettlement == null)
		{
			obj = null;
		}
		else
		{
			LocationComplex locationComplex = currentSettlement.LocationComplex;
			obj = ((locationComplex != null) ? locationComplex.GetLocationWithId("arena") : null);
		}
		if (obj == null)
		{
			return false;
		}
		if (IsOnDuelCooldown(challenger, opponent, currentSettlement))
		{
			return false;
		}
		return true;
	}

	private static bool IsOnDuelCooldown(Hero hero1, Hero hero2, Settlement settlement)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Settlement settlement2 = settlement;
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f.Settlement == settlement2);
		if (feastByAttribute == null)
		{
			return false;
		}
		if (FeastBehavior._lastFeastRewardTimes.TryGetValue(feastByAttribute, out RewardData value) && value != null)
		{
			foreach (KeyValuePair<string, RewardData.RewardInfo> reward in value.Rewards)
			{
				RewardData.RewardInfo value2 = reward.Value;
				double nextDuelRewardTime = value2.nextDuelRewardTime;
				CampaignTime now = CampaignTime.Now;
				if (nextDuelRewardTime - ((CampaignTime)(ref now)).ToDays > 0.0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void RecordDuel(Hero hero1, Hero hero2, Settlement settlement)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Settlement settlement2 = settlement;
		FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f.Settlement == settlement2);
		if (feastByAttribute != null)
		{
			if (!FeastBehavior._lastFeastRewardTimes.TryGetValue(feastByAttribute, out RewardData value) || value == null)
			{
				value = new RewardData();
				FeastBehavior._lastFeastRewardTimes[feastByAttribute] = value;
			}
			RewardData.RewardInfo orCreate = value.GetOrCreate(hero1, hero2);
			CampaignTime now = CampaignTime.Now;
			orCreate.nextDuelRewardTime = ((CampaignTime)(ref now)).ToDays + 5.0;
		}
	}
}
