using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Strategic4XDiplomacy;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace WarAndAITweaks.War_Peace_AI_Overhaul;

internal class StrategicX4AIHelpers
{
	public static float GetKingdomStrength(Kingdom k)
	{
		Kingdom i = k;
		float num = 0f;
		num += i.CurrentTotalStrength;
		return num + ((IEnumerable<Kingdom>)Strategic4XDiplomacyBehavior.MajorKingdoms).Where((Kingdom x) => x != i && !FactionManager.IsAtWarAgainstFaction((IFaction)(object)x, (IFaction)(object)i)).Sum((Kingdom p) => p.CurrentTotalStrength);
	}

	public static HashSet<Kingdom> GetNeighbors(Kingdom kingdom)
	{
		HashSet<Kingdom> hashSet = new HashSet<Kingdom>();
		if (kingdom == null || ((List<Settlement>)(object)kingdom.Settlements).Count == 0)
		{
			return hashSet;
		}
		List<Settlement> list = ((IEnumerable<Settlement>)kingdom.Settlements).Where((Settlement s) => s.IsTown || s.IsCastle).ToList();
		if (list.Count == 0)
		{
			return hashSet;
		}
		foreach (Settlement item in list)
		{
			Kingdom val = null;
			float num = float.MaxValue;
			foreach (Kingdom item2 in (List<Kingdom>)(object)Strategic4XDiplomacyBehavior.MajorKingdoms)
			{
				if (item2 == kingdom)
				{
					continue;
				}
				List<Settlement> list2 = ((IEnumerable<Settlement>)item2.Settlements).Where((Settlement s) => s.IsTown || s.IsCastle).ToList();
				if (list2.Count == 0)
				{
					continue;
				}
				foreach (Settlement item3 in list2)
				{
					float distance = item.GetPosition2D.Distance(item3.GetPosition2D);
					if (distance < num)
					{
						num = distance;
						val = item2;
					}
				}
			}
			if (val != null)
			{
				hashSet.Add(val);
			}
		}
		return hashSet;
	}

	public static void UpdateMajorKingdoms()
	{
		((List<Kingdom>)(object)Strategic4XDiplomacyBehavior.MajorKingdoms).Clear();
		foreach (Kingdom item in (List<Kingdom>)(object)Kingdom.All)
		{
			if (((IEnumerable<Settlement>)item.Settlements).Any())
			{
				((List<Kingdom>)(object)Strategic4XDiplomacyBehavior.MajorKingdoms).Add(item);
			}
		}
	}

	public static bool AreKingdomsWithinReasonableAttackDistance(Kingdom kingdomA, Kingdom kingdomB)
	{
		if (kingdomA == null || kingdomB == null || ((List<Settlement>)(object)kingdomA.Settlements).Count == 0 || ((List<Settlement>)(object)kingdomB.Settlements).Count == 0)
		{
			return false;
		}
		List<Settlement> list = ((IEnumerable<Settlement>)kingdomA.Settlements).Where((Settlement x) => x.IsCastle || x.IsTown).ToList();
		List<Settlement> list2 = ((IEnumerable<Settlement>)kingdomB.Settlements).Where((Settlement x) => x.IsCastle || x.IsTown).ToList();
		if (list.Count == 0 || list2.Count == 0)
		{
			return false;
		}
		float reasonableDistanceForBesiegingTown = Strategic4XDiplomacyBehavior.ReasonableDistanceForBesiegingTown;
		foreach (Settlement item in list)
		{
			foreach (Settlement item2 in list2)
			{
				float distance = item.GetPosition2D.Distance(item2.GetPosition2D);
				if (distance <= reasonableDistanceForBesiegingTown)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static string BuildNarrative(Kingdom us, Kingdom them, string tag)
	{
		string text = ((us == null) ? null : ((object)us.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		string text2 = ((them == null) ? null : ((object)them.Name)?.ToString()) ?? LanguageTranslater.L.S("Unknown", "Unknown");
		string text3 = tag switch
		{
			"prevent_snowball_war" => "narrative_prevent_snowball_war", 
			"expansion_war" => "narrative_expansion_war", 
			"multi_war_peace" => "narrative_multi_war_peace", 
			"overwhelmed_peace" => "narrative_overwhelmed_peace", 
			"snowball_peace" => "narrative_snowball_peace", 
			"war_fatigue_peace" => "narrative_war_fatigue_peace", 
			_ => string.Empty, 
		};
		string text4 = text3;
		if (!string.IsNullOrEmpty(text4))
		{
			TextObject val = LanguageTranslater.L.T(text4, GetXmlTextForKey(text4)).SetTextVariable("US", text).SetTextVariable("THEM", text2);
			string text5 = ((object)val).ToString();
			if (text5.Contains("{=" + text4 + "}") || text5.Contains("FALLBACK_TEXT_NOT_FOUND"))
			{
				return GetFallbackText(tag, text, text2);
			}
			return text5;
		}
		return LanguageTranslater.L.S("narrative_default", "Councillors discuss shifting allegiances across the realm.");
	}

	private static string GetXmlTextForKey(string key)
	{
		string result = key switch
		{
			"narrative_prevent_snowball_war" => "Our council reports that {US} moves to contain {THEM}'s unchecked expansion.", 
			"narrative_expansion_war" => "Strategists believe {US} covets the rich provinces of {THEM}.", 
			"narrative_multi_war_peace" => "Facing foes on several fronts, {US} requests peace with {THEM}.", 
			"narrative_overwhelmed_peace" => "Outmatched by {THEM}, {US} requests honourable terms.", 
			"narrative_snowball_peace" => "Recognising {THEM}'s rising power, {US} requests a truce.", 
			"narrative_war_fatigue_peace" => "After a gruelling campaign, {US} requests {THEM} to end hostilities.", 
			"narrative_default" => "Councillors discuss shifting allegiances across the realm.", 
			"Unknown" => "Unknown", 
			_ => string.Empty, 
		};
		return result;
	}

	private static string GetFallbackText(string tag, string usName, string themName)
	{
		string result = tag switch
		{
			"prevent_snowball_war" => "Our council reports that " + usName + " moves to contain " + themName + "'s unchecked expansion.", 
			"expansion_war" => "Strategists believe " + usName + " covets the rich provinces of " + themName + ".", 
			"multi_war_peace" => "Facing foes on several fronts, " + usName + " requests peace with " + themName + ".", 
			"overwhelmed_peace" => "Outmatched by " + themName + ", " + usName + " requests honourable terms.", 
			"snowball_peace" => "Recognising " + themName + "'s rising power, " + usName + " requests a truce.", 
			"war_fatigue_peace" => "After a gruelling campaign, " + usName + " requests " + themName + " to end hostilities.", 
			_ => "Councillors discuss shifting allegiances across the realm.", 
		};
		return result;
	}

	public static bool IsSnowballingFaction(Kingdom kingdom)
	{
		if (((kingdom != null) ? kingdom.Leader : null) == null)
		{
			return false;
		}
		MBList<Kingdom> majorKingdoms = Strategic4XDiplomacyBehavior.MajorKingdoms;
		if (((List<Kingdom>)(object)majorKingdoms).Count < 2)
		{
			return false;
		}
		float avgTerritory = ((IEnumerable<Kingdom>)majorKingdoms).Select((Func<Kingdom, float>)((Kingdom k) => ((List<Town>)(object)k.Fiefs).Count)).Average();
		List<Kingdom> list = ((IEnumerable<Kingdom>)majorKingdoms).Where((Kingdom k) => (float)((List<Town>)(object)k.Fiefs).Count >= 0.3f * avgTerritory).ToList();
		if (list.Count < 2)
		{
			return false;
		}
		float num = list.Select((Kingdom k) => GetKingdomStrength(k)).Average();
		float num2 = ((IEnumerable<Kingdom>)list).Select((Func<Kingdom, float>)((Kingdom k) => ((List<Town>)(object)k.Fiefs).Count)).Average();
		float num3 = ((num > 0f) ? (GetKingdomStrength(kingdom) / num) : 1f);
		float num4 = ((num2 > 0f) ? ((float)((List<Town>)(object)kingdom.Fiefs).Count / num2) : 1f);
		return num3 >= 1.7f || num4 >= 1.7f;
	}

	public static List<KingdomDecision> GetBaseGameKingdomDecisionsList()
	{
		Campaign current = Campaign.Current;
		KingdomDecisionProposalBehavior val = ((current != null) ? current.GetCampaignBehavior<KingdomDecisionProposalBehavior>() : null);
		if (val == null)
		{
			return null;
		}
		return typeof(KingdomDecisionProposalBehavior).GetField("_kingdomDecisionsList", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(val) as List<KingdomDecision>;
	}
}

