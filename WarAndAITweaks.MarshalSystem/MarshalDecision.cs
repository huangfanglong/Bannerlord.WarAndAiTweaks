using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace WarAndAITweaks.MarshalSystem;

internal class MarshalDecision : KingdomDecision
{
	public class MarshalOutcome : DecisionOutcome
	{
		[SaveableField(400)]
		public readonly Hero Candidate;

		public MarshalOutcome(Hero candidate)
		{
			Candidate = candidate;
		}

		public override TextObject GetDecisionTitle()
		{
			string text = ((Candidate != null) ? MarshalHelper.CalculateTrueSkill(Candidate).ToString("F0") : "0");
			TextObject val = LanguageTranslater.L.T("marshal_decision_title", "Appoint {MARSHAL} (TrueSkill: {TRUESKILL})");
			Hero candidate = Candidate;
			val.SetTextVariable("MARSHAL", ((candidate != null) ? candidate.Name : null) ?? LanguageTranslater.L.T("Unknown", "Unknown"));
			val.SetTextVariable("TRUESKILL", text);
			return val;
		}

		public override TextObject GetDecisionDescription()
		{
			string text = ((Candidate != null) ? MarshalHelper.CalculateTrueSkill(Candidate).ToString("F0") : "0");
			TextObject val = LanguageTranslater.L.T("marshal_decision_desc", "{MARSHAL} of {CLAN} is a candidate for Marshal. \n True Skill: {TRUESKILL}");
			Hero candidate = Candidate;
			val.SetTextVariable("MARSHAL", ((candidate != null) ? candidate.Name : null) ?? LanguageTranslater.L.T("Unknown", "Unknown"));
			Hero candidate2 = Candidate;
			object obj;
			if (candidate2 == null)
			{
				obj = null;
			}
			else
			{
				Clan clan = candidate2.Clan;
				obj = ((clan != null) ? clan.Name : null);
			}
			if (obj == null)
			{
				obj = LanguageTranslater.L.T("Unknown", "Unknown");
			}
			val.SetTextVariable("CLAN", (TextObject)obj);
			val.SetTextVariable("TRUESKILL", text);
			return val;
		}

		public override string GetDecisionLink()
		{
			Hero candidate = Candidate;
			return (candidate == null) ? null : candidate.EncyclopediaLink?.ToString();
		}

		public override ImageIdentifier GetDecisionImageIdentifier()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return (Candidate == null) ? ((ImageIdentifier)null) : new ImageIdentifier(CharacterCode.CreateFrom((BasicCharacterObject)(object)Candidate.CharacterObject));
		}
	}

	[CompilerGenerated]
	private sealed class _003CDetermineInitialCandidates_003Ed__12 : IEnumerable<DecisionOutcome>, IEnumerable, IEnumerator<DecisionOutcome>, IDisposable, IEnumerator
	{
		private int _003C_003E1__state;

		private DecisionOutcome _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		public MarshalDecision _003C_003E4__this;

		private List<Hero>.Enumerator _003C_003Es__1;

		private Hero _003Ccandidate_003E5__2;

		DecisionOutcome IEnumerator<DecisionOutcome>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CDetermineInitialCandidates_003Ed__12(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
			_003C_003Es__1 = default(List<Hero>.Enumerator);
			_003Ccandidate_003E5__2 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003Es__1 = _003C_003E4__this.Candidates.GetEnumerator();
					_003C_003E1__state = -3;
					break;
				case 1:
					_003C_003E1__state = -3;
					_003Ccandidate_003E5__2 = null;
					break;
				}
				if (_003C_003Es__1.MoveNext())
				{
					_003Ccandidate_003E5__2 = _003C_003Es__1.Current;
					_003C_003E2__current = (DecisionOutcome)(object)new MarshalOutcome(_003Ccandidate_003E5__2);
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003Em__Finally1();
				_003C_003Es__1 = default(List<Hero>.Enumerator);
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			((IDisposable)_003C_003Es__1).Dispose();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<DecisionOutcome> IEnumerable<DecisionOutcome>.GetEnumerator()
		{
			_003CDetermineInitialCandidates_003Ed__12 result;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				result = this;
			}
			else
			{
				result = new _003CDetermineInitialCandidates_003Ed__12(0)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			return result;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<DecisionOutcome>)this).GetEnumerator();
		}
	}

	[SaveableField(400)]
	public readonly Kingdom Kingdom;

	[SaveableField(401)]
	public readonly Clan ProposerClan;

	[SaveableField(402)]
	public readonly List<Hero> Candidates;

	public MarshalDecision(Clan proposerClan, Kingdom kingdom, List<Hero> candidates)
		: base(proposerClan)
	{
		Kingdom = kingdom;
		ProposerClan = proposerClan;
		Candidates = (candidates ?? new List<Hero>()).OrderByDescending((Hero x) => MarshalHelper.CalculateTrueSkill(x)).Take(3).ToList();
	}

	public override bool IsAllowed()
	{
		return Kingdom != null && Kingdom.RulingClan != null && Candidates != null && Candidates.Count > 0 && Candidates.All((Hero c) => c != null && c.Clan != null && !c.IsDead);
	}

	public override int GetProposalInfluenceCost()
	{
		return 100;
	}

	public override TextObject GetGeneralTitle()
	{
		return LanguageTranslater.L.T("marshal_decision_general_title", "Marshal Appointment");
	}

	public override TextObject GetSupportTitle()
	{
		TextObject val = LanguageTranslater.L.T("marshal_decision_support_title", "Vote for Marshal of {KINGDOM_NAME}");
		Kingdom kingdom = Kingdom;
		val.SetTextVariable("KINGDOM_NAME", ((kingdom == null) ? null : ((object)kingdom.InformalName)?.ToString()) ?? ((object)LanguageTranslater.L.T("Unknown", "Unknown")).ToString());
		return val;
	}

	public override TextObject GetChooseTitle()
	{
		TextObject val = LanguageTranslater.L.T("marshal_decision_choose_title", "Appointing a Marshal for {KINGDOM_NAME}");
		Kingdom kingdom = Kingdom;
		val.SetTextVariable("KINGDOM_NAME", ((kingdom == null) ? null : ((object)kingdom.InformalName)?.ToString()) ?? ((object)LanguageTranslater.L.T("Unknown", "Unknown")).ToString());
		return val;
	}

	public override TextObject GetSupportDescription()
	{
		TextObject val = LanguageTranslater.L.T("marshal_decision_support_desc", "{FACTION_LEADER} will decide who will serve as Marshal of {KINGDOM_NAME}. You can pick your stance regarding this decision.");
		Clan obj = ((KingdomDecision)this).DetermineChooser();
		object obj2;
		if (obj == null)
		{
			obj2 = null;
		}
		else
		{
			Hero leader = obj.Leader;
			obj2 = ((leader == null) ? null : ((object)leader.Name)?.ToString());
		}
		if (obj2 == null)
		{
			obj2 = ((object)LanguageTranslater.L.T("Unknown", "Unknown")).ToString();
		}
		val.SetTextVariable("FACTION_LEADER", (string)obj2);
		Kingdom kingdom = Kingdom;
		val.SetTextVariable("KINGDOM_NAME", ((kingdom == null) ? null : ((object)kingdom.InformalName)?.ToString()) ?? ((object)LanguageTranslater.L.T("Unknown", "Unknown")).ToString());
		return val;
	}

	public override TextObject GetChooseDescription()
	{
		TextObject val = LanguageTranslater.L.T("marshal_decision_choose_desc", "As {?IS_FEMALE}queen{?}king{\\?}, you must appoint the Marshal of {KINGDOM_NAME}");
		Clan obj = ((KingdomDecision)this).DetermineChooser();
		int num;
		if (obj != null)
		{
			Hero leader = obj.Leader;
			if (((leader != null) ? new bool?(leader.IsFemale) : null).GetValueOrDefault())
			{
				num = 1;
				goto IL_0050;
			}
		}
		num = 0;
		goto IL_0050;
		IL_0050:
		val.SetTextVariable("IS_FEMALE", num);
		Kingdom kingdom = Kingdom;
		val.SetTextVariable("KINGDOM_NAME", ((kingdom == null) ? null : ((object)kingdom.InformalName)?.ToString()) ?? ((object)LanguageTranslater.L.T("Unknown", "Unknown")).ToString());
		return val;
	}

	[IteratorStateMachine(typeof(_003CDetermineInitialCandidates_003Ed__12))]
	public override IEnumerable<DecisionOutcome> DetermineInitialCandidates()
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CDetermineInitialCandidates_003Ed__12(-2)
		{
			_003C_003E4__this = this
		};
	}

	public override Clan DetermineChooser()
	{
		Kingdom kingdom = Kingdom;
		return (kingdom != null) ? kingdom.RulingClan : null;
	}

	public override float CalculateMeritOfOutcome(DecisionOutcome candidateOutcome)
	{
		float num = 0f;
		foreach (Clan item in (List<Clan>)(object)Kingdom.Clans)
		{
			if (item.Leader != Hero.MainHero)
			{
				num += CalculateMeritOfOutcomeForClan(item, candidateOutcome);
			}
		}
		return num;
	}

	private (float maxSkill, float maxStrength, float maxTier, float maxInfluence, float maxWealth) GetMaxCandidateStats()
	{
		float num = ((Candidates.Count > 0) ? Candidates.Max((Hero c) => MarshalHelper.CalculateTrueSkill(c)) : 1f);
		float num2 = ((Candidates.Count > 0) ? Candidates.Max(delegate(Hero c)
		{
			Clan clan3 = c.Clan;
			return (clan3 != null) ? clan3.TotalStrength : 0f;
		}) : 1f);
		float num3 = ((Candidates.Count > 0) ? Candidates.Max(delegate(Hero c)
		{
			Clan clan2 = c.Clan;
			return ((float?)((clan2 != null) ? new int?(clan2.Tier) : null)) ?? 0f;
		}) : 1f);
		float num4 = ((Candidates.Count > 0) ? Candidates.Max(delegate(Hero c)
		{
			Clan clan = c.Clan;
			return (clan != null) ? clan.Influence : 0f;
		}) : 1f);
		float num5 = ((Candidates.Count > 0) ? ((float)Candidates.Max((Hero c) => c.Gold)) : 1f);
		if (num <= 0f)
		{
			num = 1f;
		}
		if (num2 <= 0f)
		{
			num2 = 1f;
		}
		if (num3 <= 0f)
		{
			num3 = 1f;
		}
		if (num4 <= 0f)
		{
			num4 = 1f;
		}
		if (num5 <= 0f)
		{
			num5 = 1f;
		}
		return (maxSkill: num, maxStrength: num2, maxTier: num3, maxInfluence: num4, maxWealth: num5);
	}

	private float CalculateMeritOfOutcomeForClan(Clan clan, DecisionOutcome candidateOutcome)
	{
		if (!(candidateOutcome is MarshalOutcome { Candidate: not null } marshalOutcome) || clan == null || clan.Leader == null)
		{
			return 0f;
		}
		Hero candidate = marshalOutcome.Candidate;
		if (candidate.Clan == null)
		{
			return 0f;
		}
		(float maxSkill, float maxStrength, float maxTier, float maxInfluence, float maxWealth) maxCandidateStats = GetMaxCandidateStats();
		float item = maxCandidateStats.maxSkill;
		float item2 = maxCandidateStats.maxStrength;
		float item3 = maxCandidateStats.maxTier;
		float item4 = maxCandidateStats.maxInfluence;
		float item5 = maxCandidateStats.maxWealth;
		float num = N(MarshalHelper.CalculateTrueSkill(candidate), item);
		float num2 = N(candidate.Clan.TotalStrength, item2);
		float num3 = N(candidate.Clan.Tier, item3);
		float num4 = N(candidate.Clan.Influence, item4);
		float num5 = N(candidate.Gold, item5);
		float num6 = 0f;
		try
		{
			num6 = MBMath.ClampFloat((float)clan.Leader.GetRelation(candidate), -100f, 100f);
		}
		catch
		{
			num6 = 0f;
		}
		float num7 = (num6 + 100f) / 200f;
		float num8 = 0.35f * num + 0.15f * num2 + 0.1f * num3 + 0.1f * num4 + 0.1f * num5 + 0.2f * num7;
		float num9 = -3f + num8 * 11f;
		return MBMath.ClampFloat(num9, -3f, 8f);
		static float N(float v, float max)
		{
			return MBMath.ClampFloat((max > 0f) ? (v / max) : 0f, 0f, 1f);
		}
	}

	public override float DetermineSupport(Clan clan, DecisionOutcome outcome)
	{
		return CalculateMeritOfOutcomeForClan(clan, outcome) * 100f;
	}

	public override void DetermineSponsors(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
	{
		foreach (DecisionOutcome item in (List<DecisionOutcome>)(object)possibleOutcomes)
		{
			if (item is MarshalOutcome marshalOutcome)
			{
				Hero candidate = marshalOutcome.Candidate;
				item.SetSponsor((candidate != null) ? candidate.Clan : null);
			}
		}
	}

	public override void ApplyChosenOutcome(DecisionOutcome chosenOutcome)
	{
		if (chosenOutcome is MarshalOutcome { Candidate: not null } marshalOutcome)
		{
			Campaign.Current.GetCampaignBehavior<MarshalSystemBehavior>()?.AppointMarshal(Kingdom, marshalOutcome.Candidate);
		}
	}

	public override void ApplySecondaryEffects(MBReadOnlyList<DecisionOutcome> possibleOutcomes, DecisionOutcome chosenOutcome)
	{
	}

	public override TextObject GetSecondaryEffects()
	{
		return LanguageTranslater.L.T("marshal_decision_secondary_effects", "All supporters gain some relation with each other.");
	}

	public override TextObject GetChosenOutcomeText(DecisionOutcome chosenOutcome, SupportStatus supportStatus, bool isShortVersion)
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		if (chosenOutcome is MarshalOutcome marshalOutcome)
		{
			if (1 == 0)
			{
			}
			string text = (((int)supportStatus == 1) ? "marshal_decision_outcome_majority" : (((int)supportStatus != 2) ? "marshal_decision_outcome_single" : "marshal_decision_outcome_minority"));
			if (1 == 0)
			{
			}
			string text2 = text;
			TextObject val = LanguageTranslater.L.T(text2, (text2 == "marshal_decision_outcome_majority" || text2 == "marshal_decision_outcome_minority" || text2 == "marshal_decision_outcome_single") ? "{RULER.NAME} of the {KINGDOM} has appointed {MARSHAL.NAME} as Marshal." : "");
			Kingdom kingdom = Kingdom;
			object obj;
			if (kingdom == null)
			{
				obj = null;
			}
			else
			{
				Clan rulingClan = kingdom.RulingClan;
				if (rulingClan == null)
				{
					obj = null;
				}
				else
				{
					Hero leader = rulingClan.Leader;
					obj = ((leader != null) ? leader.CharacterObject : null);
				}
			}
			StringHelpers.SetCharacterProperties("RULER", (CharacterObject)obj, val, false);
			Hero candidate = marshalOutcome.Candidate;
			StringHelpers.SetCharacterProperties("MARSHAL", (candidate != null) ? candidate.CharacterObject : null, val, false);
			Kingdom kingdom2 = Kingdom;
			val.SetTextVariable("KINGDOM", ((kingdom2 != null) ? kingdom2.InformalName : null) ?? LanguageTranslater.L.T("Unknown", "Unknown"));
			return val;
		}
		return new TextObject("", (Dictionary<string, object>)null);
	}

	public override DecisionOutcome GetQueriedDecisionOutcome(MBReadOnlyList<DecisionOutcome> possibleOutcomes)
	{
		if (possibleOutcomes == null || ((List<DecisionOutcome>)(object)possibleOutcomes).Count == 0)
		{
			return null;
		}
		DecisionOutcome val = ((IEnumerable<DecisionOutcome>)possibleOutcomes).OrderByDescending((DecisionOutcome o) => o.Merit).FirstOrDefault();
		if (val == null || val.Merit <= 0f)
		{
			Kingdom kingdom = Kingdom;
			object obj;
			if (kingdom == null)
			{
				obj = null;
			}
			else
			{
				Clan rulingClan = kingdom.RulingClan;
				obj = ((rulingClan != null) ? rulingClan.Leader : null);
			}
			Hero ruler = (Hero)obj;
			MarshalOutcome marshalOutcome = ((IEnumerable)possibleOutcomes).OfType<MarshalOutcome>().FirstOrDefault((MarshalOutcome o) => o.Candidate == ruler);
			return (DecisionOutcome)(((object)marshalOutcome) ?? ((object)((IEnumerable<DecisionOutcome>)possibleOutcomes).FirstOrDefault()));
		}
		return val;
	}
}
