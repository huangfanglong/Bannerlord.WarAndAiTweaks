using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Library;
using WarAndAITweaks.War_Peace_AI_Overhaul;

namespace WarAndAITweaks.MarshalSystem;

public static class MarshalHelper
{
	public static float CalculateMarshalScore(Hero hero, Hero ruler)
	{
		if (hero == null || ruler == null || hero.Clan == null || hero.PartyBelongedTo == null)
		{
			return 0f;
		}
		float num = MBMath.ClampFloat((float)ruler.GetRelation(hero), -100f, 100f);
		float num2 = (num + 100f) / 200f;
		float num3 = MBMath.ClampFloat(CalculateTrueSkill(hero), 0f, 300f);
		float num4 = num3 / 300f;
		float partySizeRatio = hero.PartyBelongedTo.PartySizeRatio;
		float num5 = MBMath.ClampFloat(partySizeRatio, 0f, 250f) / 250f;
		float totalStrength = hero.Clan.CurrentTotalStrength;
		float num6 = MBMath.ClampFloat(totalStrength, 0f, 2000f) / 2000f;
		float influence = hero.Clan.Influence;
		float num7 = MBMath.ClampFloat(influence, 0f, 1000f) / 1000f;
		return (num2 + num4 + num5 + num6 + num7) / 5f;
	}

	public static Hero SelectBestCandidate(List<Hero> candidates, Hero ruler)
	{
		Hero ruler2 = ruler;
		return (from h in candidates
			orderby CalculateMarshalScore(h, ruler2) descending, MBMath.ClampFloat((float)ruler2.GetRelation(h), -100f, 100f) descending, CalculateTrueSkill(h) descending, h.Age descending
			select h).FirstOrDefault();
	}

	public static bool IsEligibleForMarshalship(Hero hero)
	{
		if (hero == null)
		{
			return false;
		}
		if (hero.PartyBelongedTo == null)
		{
			return false;
		}
		if (hero.PartyBelongedTo.PartySizeRatio < 0.6f)
		{
			return false;
		}
		if (!hero.IsAlive)
		{
			return false;
		}
		if (hero.Clan == null)
		{
			return false;
		}
		if (hero.Clan.Kingdom == null)
		{
			return false;
		}
		if (hero.Clan.Leader != hero)
		{
			return false;
		}
		if (hero.Clan.IsUnderMercenaryService)
		{
			return false;
		}
		return true;
	}

	public static float CalculateTrueSkill(Hero hero)
	{
		return (float)hero.GetSkillValue(DefaultSkills.Tactics) * 0.6f + (float)hero.GetSkillValue(DefaultSkills.Leadership) * 0.4f;
	}

	public static float GetMarshalBonus(Hero marshal)
	{
		if (marshal == null)
		{
			return 0f;
		}
		float num = CalculateTrueSkill(marshal);
		float num2 = MBMath.ClampFloat(num / 330f, 0f, 1f);
		return num2 * 0.7f;
	}

	public static void RemoveAllInvalidMarshalDecisions(Kingdom kingdom)
	{
		Kingdom kingdom2 = kingdom;
		if (kingdom2 == null)
		{
			return;
		}
		Hero marshal = MarshalSystemBehavior.GetMarshal(kingdom2);
		if (marshal == null)
		{
			return;
		}
		StrategicX4AIHelpers.GetBaseGameKingdomDecisionsList()?.RemoveAll((KingdomDecision d) => d is MarshalDecision marshalDecision && marshalDecision.Kingdom == kingdom2);
		List<MarshalDecision> list = (from md in ((IEnumerable)kingdom2.UnresolvedDecisions).OfType<MarshalDecision>()
			where md.Kingdom == kingdom2
			select md).ToList();
		foreach (MarshalDecision item in list)
		{
			kingdom2.RemoveDecision((KingdomDecision)(object)item);
		}
	}

	public static void RemoveMarshalForTesting(Kingdom kingdom)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		if (kingdom == null)
		{
			return;
		}
		MarshalSystemBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<MarshalSystemBehavior>();
		if (campaignBehavior != null)
		{
			FieldInfo field = typeof(MarshalSystemBehavior).GetField("_kingdomMarshals", BindingFlags.Instance | BindingFlags.NonPublic);
			if (!(field == null) && field.GetValue(campaignBehavior) is Dictionary<Kingdom, MarshalData> dictionary && dictionary.TryGetValue(kingdom, out var value) && value.CurrentMarshal != null)
			{
				Hero currentMarshal = value.CurrentMarshal;
				value.CurrentMarshal = null;
				value.AppointmentStartDate = default(CampaignTime);
				value.AppointmentEndDate = default(CampaignTime);
				InformationManager.DisplayMessage(new InformationMessage($"[TEST] Marshal {currentMarshal.Name} removed from {kingdom.Name}.", Colors.Red));
			}
		}
	}

	public static string GetMarshalStatusText(Hero hero)
	{
		return "";
	}

	public static bool IsValidMarshalCandidate(Hero hero, Kingdom kingdom)
	{
		return true;
	}

	public static int GetMarshalDaysRemaining(Hero marshal)
	{
		return 0;
	}
}
