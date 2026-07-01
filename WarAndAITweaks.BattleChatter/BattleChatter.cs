using System;
using System.Collections.Generic;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using WarAndAiTweaks;

namespace WarAndAITweaks.BattleChatter;

internal class BattleChatter : MissionBehavior
{
	private float _lastGlobalIndividualChatterTime = -100f;

	private float _nextGlobalFormationChatterTime = 0f;

	private const int FormationTickerCooldown = 10;

	private float _nextFormationTickerTime = 0f;

	private static bool _initialized = false;

	private Dictionary<CharacterObject, ChatterAgent> HeroList = new Dictionary<CharacterObject, ChatterAgent>();

	private static readonly HashSet<string> DialogCategories = new HashSet<string>
	{
		"bc_kill_male", "bc_kill_female", "bc_hit_lowhp", "bc_hit_heavy", "bc_hit_light", "bc_form_same_inf", "bc_form_same_cav", "bc_form_same_rng", "bc_form_inf_to_rng", "bc_form_inf_to_cav",
		"bc_form_rng_to_inf", "bc_form_rng_to_cav", "bc_form_cav_to_inf", "bc_form_cav_to_rng"
	};

	private static float FormationAmbientChance => GlobalSettings<WarAndAiTweaksSettings>.Instance.FormationAmbientChance;

	private static float CryChance => GlobalSettings<WarAndAiTweaksSettings>.Instance.CryChance;

	private static float HitChance => GlobalSettings<WarAndAiTweaksSettings>.Instance.HitChance;

	private static float FormationChatterTimeCooldown => GlobalSettings<WarAndAiTweaksSettings>.Instance.FormationChatterTimeCooldown;

	private static float IndividualChatterTimeCooldown => GlobalSettings<WarAndAiTweaksSettings>.Instance.IndividualChatterTimeCooldown;

	private static bool IncludePlayerInDialog => GlobalSettings<WarAndAiTweaksSettings>.Instance.IncludePlayerInDialog;

	private static bool AllowProfanity => GlobalSettings<WarAndAiTweaksSettings>.Instance.AllowProfanity;

	public override MissionBehaviorType BehaviorType => (MissionBehaviorType)1;

	private bool IsFormationActive(Formation formation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		return (int)formation.GetUnderAttackTypeOfUnits(1f) > 0;
	}

	public BattleChatter()
	{
		EnsureInitialized();
	}

	private static void EnsureInitialized()
	{
		if (_initialized)
		{
			return;
		}
		try
		{
			BattleChatterLocalizationLoader.ReloadLines();
			_initialized = true;
		}
		catch (Exception ex)
		{
			Debug.Print("[BattleChatter] Failed to initialize: " + ex.Message, 0, (Debug.DebugColor)12, 17592186044416uL);
		}
	}

	public override void AfterStart()
	{
		try
		{
			if (!_initialized)
			{
				BattleChatterLocalizationLoader.ReloadLines();
				_initialized = true;
			}
		}
		catch (Exception ex)
		{
			LogError("AfterStart failed: " + ex.Message);
		}
	}

	public override void OnAgentCreated(Agent agent)
	{
		try
		{
			BasicCharacterObject obj = ((agent != null) ? agent.Character : null);
			CharacterObject val = (CharacterObject)(object)((obj is CharacterObject) ? obj : null);
			if (val != null && val.HeroObject != null && IsEligibleHero(val, agent))
			{
				ChatterAgent value = new ChatterAgent
				{
					CharacterObject = val,
					Agent = agent
				};
				HeroList[val] = value;
			}
		}
		catch (Exception ex)
		{
			LogError("OnAgentCreated failed: " + ex.Message);
		}
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		try
		{
			if (!HeroList.Any())
			{
				return;
			}
			BasicCharacterObject obj = ((affectedAgent != null) ? affectedAgent.Character : null);
			CharacterObject val = (CharacterObject)(object)((obj is CharacterObject) ? obj : null);
			if (val != null && HeroList.ContainsKey(val))
			{
				HeroList.Remove(val);
				return;
			}
			Mission mission = ((MissionBehavior)this).Mission;
			float num = ((mission != null) ? mission.CurrentTime : 0f);
			if (num - _lastGlobalIndividualChatterTime < IndividualChatterTimeCooldown || affectorAgent == null || affectedAgent == null || (int)agentState == 3)
			{
				return;
			}
			BasicCharacterObject character = affectorAgent.Character;
			CharacterObject val2 = (CharacterObject)(object)((character is CharacterObject) ? character : null);
			if (val2 == null || !HeroList.TryGetValue(val2, out ChatterAgent value) || MBRandom.RandomFloatRanged(0f, 1f) >= CryChance)
			{
				return;
			}
			string text = (value.IsFemale ? "bc_kill_female" : "bc_kill_male");
			if (DialogCategories.Contains(text))
			{
				string randomLine = BattleChatterLocalizationLoader.GetRandomLine(text, AllowProfanity);
				if (!string.IsNullOrEmpty(randomLine))
				{
					SpeakLine(affectorAgent, randomLine, ChatterType.InvididualChatter);
				}
			}
		}
		catch (Exception ex)
		{
			LogError("OnAgentRemoved failed: " + ex.Message);
		}
	}

	public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!HeroList.Any())
			{
				return;
			}
			BasicCharacterObject obj = ((affectedAgent != null) ? affectedAgent.Character : null);
			CharacterObject val = (CharacterObject)(object)((obj is CharacterObject) ? obj : null);
			BasicCharacterObject obj2 = ((affectorAgent != null) ? affectorAgent.Character : null);
			CharacterObject val2 = (CharacterObject)(object)((obj2 is CharacterObject) ? obj2 : null);
			if (val == null || !HeroList.ContainsKey(val))
			{
				return;
			}
			Mission mission = ((MissionBehavior)this).Mission;
			float num = ((mission != null) ? mission.CurrentTime : 0f);
			if (num - _lastGlobalIndividualChatterTime < IndividualChatterTimeCooldown || MBRandom.RandomFloatRanged(0f, 1f) >= HitChance)
			{
				return;
			}
			Blow val3 = blow;
			int num2;
			if (!val3.IsBlowCrit((int)affectorAgent.HealthLimit))
			{
				val3 = blow;
				num2 = val3.IsHeadShot() ? 1 : 0;
			}
			else
			{
				num2 = 1;
			}
			bool flag = (byte)num2 != 0;
			string text = ((affectedAgent.Health < affectedAgent.HealthLimit * 0.3f) ? "bc_hit_lowhp" : ((!flag) ? "bc_hit_light" : "bc_hit_heavy"));
			if (DialogCategories.Contains(text))
			{
				string randomLine = BattleChatterLocalizationLoader.GetRandomLine(text, AllowProfanity);
				if (!string.IsNullOrEmpty(randomLine))
				{
					SpeakLine(affectedAgent, randomLine, ChatterType.InvididualChatter);
				}
			}
		}
		catch (Exception ex)
		{
			LogError("OnAgentHit failed: " + ex.Message);
		}
	}

	public override void OnMissionTick(float dt)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		((MissionBehavior)this).OnMissionTick(dt);
		if (((MissionBehavior)this).Mission == null || (int)((MissionBehavior)this).Mission.CombatType > 0)
		{
			return;
		}
		try
		{
			if (HeroList.Any())
			{
				Mission mission = ((MissionBehavior)this).Mission;
				float num = ((mission != null) ? mission.CurrentTime : 0f);
				if (!(num < _nextFormationTickerTime))
				{
					_nextFormationTickerTime = num + 10f;
					HandleInterFormationChatter();
				}
			}
		}
		catch (Exception ex)
		{
			LogError("OnMissionTick failed: " + ex.Message);
		}
	}

	private void HandleInterFormationChatter()
	{
		Mission mission = ((MissionBehavior)this).Mission;
		float num = ((mission != null) ? mission.CurrentTime : 0f);
		if (num < _nextGlobalFormationChatterTime || MBRandom.RandomFloatRanged(0f, 1f) > FormationAmbientChance)
		{
			return;
		}
		List<ChatterAgent> list = HeroList.Values.Where(delegate(ChatterAgent ca)
		{
			Agent agent3 = ca.Agent;
			int result;
			if (agent3 != null && agent3.IsActive())
			{
				Agent agent4 = ca.Agent;
				if (((agent4 != null) ? agent4.Formation : null) != null)
				{
					result = (IsFormationActive(ca.Agent.Formation) ? 1 : 0);
					goto IL_003c;
				}
			}
			result = 0;
			goto IL_003c;
			IL_003c:
			return (byte)result != 0;
		}).ToList();
		if (list.Count < 2)
		{
			return;
		}
		int num2 = MBRandom.RandomInt(list.Count);
		ChatterAgent chatterAgent = list[num2];
		ChatterAgent chatterAgent2 = list[(num2 + 1) % list.Count];
		if (!IsFormationActive(chatterAgent.Agent.Formation) || !IsFormationActive(chatterAgent2.Agent.Formation))
		{
			return;
		}
		UnitType agentUnitType = GetAgentUnitType(chatterAgent.Agent);
		UnitType agentUnitType2 = GetAgentUnitType(chatterAgent2.Agent);
		Agent agent = chatterAgent.Agent;
		int? num3 = ((agent == null) ? null : agent.Formation?.Index);
		Agent agent2 = chatterAgent2.Agent;
		if (num3 == ((agent2 == null) ? null : agent2.Formation?.Index))
		{
			if (agentUnitType != agentUnitType2 || agentUnitType == UnitType.Unknown)
			{
				return;
			}
			if (1 == 0)
			{
			}
			string text = agentUnitType switch
			{
				UnitType.Infantry => "bc_form_same_inf", 
				UnitType.Cavalry => "bc_form_same_cav", 
				UnitType.Archer => "bc_form_same_rng", 
				_ => null, 
			};
			if (1 == 0)
			{
			}
			string text2 = text;
			if (text2 == null || !DialogCategories.Contains(text2))
			{
				return;
			}
			string randomLine = BattleChatterLocalizationLoader.GetRandomLine(text2, AllowProfanity);
			if (!string.IsNullOrEmpty(randomLine))
			{
				string text3 = randomLine;
				CharacterObject characterObject = chatterAgent2.CharacterObject;
				randomLine = text3.Replace("{TARGET}", ((characterObject == null) ? null : ((object)((BasicCharacterObject)characterObject).Name)?.ToString()) ?? "");
				if (chatterAgent.Agent != null)
				{
					SpeakLine(chatterAgent.Agent, randomLine, ChatterType.FormationChatter);
				}
			}
		}
		else
		{
			if (agentUnitType == UnitType.Unknown || agentUnitType2 == UnitType.Unknown || agentUnitType == agentUnitType2)
			{
				return;
			}
			string crossFormationCategory = GetCrossFormationCategory(agentUnitType, agentUnitType2);
			if (crossFormationCategory == null || !DialogCategories.Contains(crossFormationCategory))
			{
				return;
			}
			string randomLine2 = BattleChatterLocalizationLoader.GetRandomLine(crossFormationCategory, AllowProfanity);
			if (!string.IsNullOrEmpty(randomLine2))
			{
				string text4 = randomLine2;
				CharacterObject characterObject2 = chatterAgent2.CharacterObject;
				randomLine2 = text4.Replace("{TARGET}", ((characterObject2 == null) ? null : ((object)((BasicCharacterObject)characterObject2).Name)?.ToString()) ?? "");
				if (chatterAgent.Agent != null)
				{
					SpeakLine(chatterAgent.Agent, randomLine2, ChatterType.FormationChatter);
				}
			}
		}
	}

	private static string? GetCrossFormationCategory(UnitType speakerType, UnitType targetType)
	{
		if (1 == 0)
		{
		}
		string result;
		switch (speakerType)
		{
		case UnitType.Infantry:
			if (targetType == UnitType.Archer)
			{
				result = "bc_form_inf_to_rng";
				break;
			}
			if (targetType != UnitType.Cavalry)
			{
				goto default;
			}
			result = "bc_form_inf_to_cav";
			break;
		case UnitType.Archer:
			if (targetType == UnitType.Infantry)
			{
				result = "bc_form_rng_to_inf";
				break;
			}
			if (targetType != UnitType.Cavalry)
			{
				goto default;
			}
			result = "bc_form_rng_to_cav";
			break;
		case UnitType.Cavalry:
			if (targetType == UnitType.Infantry)
			{
				result = "bc_form_cav_to_inf";
				break;
			}
			if (targetType != UnitType.Archer)
			{
				goto default;
			}
			result = "bc_form_cav_to_rng";
			break;
		default:
			result = null;
			break;
		}
		if (1 == 0)
		{
		}
		return result;
	}

	private static UnitType GetAgentUnitType(Agent agent)
	{
		if (((agent != null) ? agent.Formation : null) == null)
		{
			return UnitType.Unknown;
		}
		return agent.Formation.Index switch
		{
			0 => UnitType.Infantry, 
			1 => UnitType.Archer, 
			2 => UnitType.Cavalry, 
			3 => UnitType.Archer, 
			_ => (!agent.HasMount) ? UnitType.Infantry : UnitType.Cavalry, 
		};
	}

	private void SpeakLine(Agent agent, string lineToSpeak, ChatterType chatterType)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		try
		{
			BasicCharacterObject obj = ((agent != null) ? agent.Character : null);
			CharacterObject val = (CharacterObject)(object)((obj is CharacterObject) ? obj : null);
			if (val != null && val.HeroObject != null)
			{
				Mission mission = ((MissionBehavior)this).Mission;
				float num = ((mission != null) ? mission.CurrentTime : 0f);
				if (chatterType == ChatterType.InvididualChatter)
				{
					_lastGlobalIndividualChatterTime = num;
				}
				if (chatterType == ChatterType.FormationChatter)
				{
					_nextGlobalFormationChatterTime = num + FormationChatterTimeCooldown;
				}
				MBInformationManager.AddQuickInformation(new TextObject(lineToSpeak, (Dictionary<string, object>)null), 0, agent.Character, null);
			}
		}
		catch (Exception ex)
		{
			LogError("SpeakLine failed: " + ex.Message);
		}
	}

	private bool IsEligibleHero(CharacterObject character, Agent agent)
	{
		try
		{
			if (!agent.IsActive() || !agent.IsHero)
			{
				return false;
			}
			Hero mainHero = Hero.MainHero;
			if (character == ((mainHero != null) ? mainHero.CharacterObject : null))
			{
				return IncludePlayerInDialog;
			}
			Hero heroObject = character.HeroObject;
			int result;
			if (((heroObject != null) ? heroObject.Clan : null) != null)
			{
				Hero mainHero2 = Hero.MainHero;
				if (((mainHero2 != null) ? mainHero2.Clan : null) != null)
				{
					result = ((heroObject.Clan == Hero.MainHero.Clan) ? 1 : 0);
					goto IL_007b;
				}
			}
			result = 0;
			goto IL_007b;
			IL_007b:
			return (byte)result != 0;
		}
		catch
		{
			return false;
		}
	}

	private void LogError(string message)
	{
		Debug.Print("[BattleChatter] " + message, 0, (Debug.DebugColor)12, 17592186044416uL);
	}
}



