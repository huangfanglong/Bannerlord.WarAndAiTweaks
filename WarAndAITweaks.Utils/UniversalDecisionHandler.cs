using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Election;
using WarAndAITweaks.MarshalSystem;

namespace WarAndAITweaks.Utils;

internal class UniversalDecisionHandler
{
	public static bool HandleAddingDecision(KingdomDecision decision, Kingdom targetKingdom, Kingdom proposerKingdom)
	{
		if (decision == null || proposerKingdom == null)
		{
			return false;
		}
		Campaign current = Campaign.Current;
		KingdomDecisionProposalBehavior val = ((current != null) ? current.GetCampaignBehavior<KingdomDecisionProposalBehavior>() : null);
		if (val == null)
		{
			return false;
		}
		FieldInfo field = typeof(KingdomDecisionProposalBehavior).GetField("_kingdomDecisionsList", BindingFlags.Instance | BindingFlags.NonPublic);
		List<KingdomDecision> list = field?.GetValue(val) as List<KingdomDecision>;
		if (list == null)
		{
			list = new List<KingdomDecision>();
			field?.SetValue(val, list);
		}
		bool flag = false;
		MakePeaceKingdomDecision val2 = (MakePeaceKingdomDecision)(object)((decision is MakePeaceKingdomDecision) ? decision : null);
		CampaignTime triggerTime;
		if (val2 != null && val2.FactionToMakePeaceWith == Hero.MainHero.MapFaction)
		{
			foreach (KingdomDecision item in list)
			{
				MakePeaceKingdomDecision val3 = (MakePeaceKingdomDecision)(object)((item is MakePeaceKingdomDecision) ? item : null);
				if (val3 == null)
				{
					continue;
				}
				if (((KingdomDecision)val3).Kingdom == Hero.MainHero.MapFaction && val3.FactionToMakePeaceWith == proposerKingdom)
				{
					triggerTime = item.TriggerTime;
					if (triggerTime.IsFuture)
					{
						goto IL_014a;
					}
				}
				if (((KingdomDecision)val3).Kingdom != proposerKingdom || val3.FactionToMakePeaceWith != Hero.MainHero.MapFaction)
				{
					continue;
				}
				triggerTime = item.TriggerTime;
				if (!triggerTime.IsFuture)
				{
					continue;
				}
				goto IL_014a;
				IL_014a:
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return false;
		}
		bool flag2 = false;
		foreach (KingdomDecision item2 in list)
		{
			DeclareWarDecision val4 = (DeclareWarDecision)(object)((item2 is DeclareWarDecision) ? item2 : null);
			if (val4 != null)
			{
				DeclareWarDecision val5 = (DeclareWarDecision)(object)((decision is DeclareWarDecision) ? decision : null);
				if (val5 != null)
				{
					if (val4.FactionToDeclareWarOn == val5.FactionToDeclareWarOn)
					{
						Clan proposerClan = ((KingdomDecision)val4).ProposerClan;
						IFaction obj = ((proposerClan != null) ? proposerClan.MapFaction : null);
						Clan proposerClan2 = ((KingdomDecision)val5).ProposerClan;
						if (obj == ((proposerClan2 != null) ? proposerClan2.MapFaction : null))
						{
							flag2 = true;
							break;
						}
					}
					continue;
				}
			}
			MakePeaceKingdomDecision val6 = (MakePeaceKingdomDecision)(object)((item2 is MakePeaceKingdomDecision) ? item2 : null);
			if (val6 != null)
			{
				MakePeaceKingdomDecision val7 = (MakePeaceKingdomDecision)(object)((decision is MakePeaceKingdomDecision) ? decision : null);
				if (val7 != null)
				{
					if (val6.FactionToMakePeaceWith == val7.FactionToMakePeaceWith)
					{
						Clan proposerClan3 = ((KingdomDecision)val6).ProposerClan;
						IFaction obj2 = ((proposerClan3 != null) ? proposerClan3.MapFaction : null);
						Clan proposerClan4 = ((KingdomDecision)val7).ProposerClan;
						if (obj2 == ((proposerClan4 != null) ? proposerClan4.MapFaction : null))
						{
							flag2 = true;
							break;
						}
					}
					continue;
				}
			}
			if (item2 is MarshalDecision marshalDecision && decision is MarshalDecision marshalDecision2 && marshalDecision.Kingdom == marshalDecision2.Kingdom)
			{
				triggerTime = item2.TriggerTime;
				if (triggerTime.IsFuture)
				{
					flag2 = true;
					break;
				}
			}
		}
		if (flag2)
		{
			return false;
		}
		list.Add(decision);
		new KingdomElection(decision);
		proposerKingdom.AddDecision(decision, false);
		return true;
	}
}


