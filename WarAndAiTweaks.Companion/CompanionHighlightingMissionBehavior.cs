using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace WarAndAiTweaks.Companion;

[DefaultView]
public class CompanionHighlightingMissionBehavior : MissionView
{
	private enum InitializationState
	{
		NotStarted,
		WaitingForDeployment,
		Initializing,
		Ready,
		Failed
	}

	private InitializationState _state = InitializationState.NotStarted;

	private GauntletLayer _gauntletLayer;

	private CompanionHighlighterVM _dataSource;

	private Camera _camera;

	private readonly Dictionary<Agent, CompanionIconVM> _agentIconMap = new Dictionary<Agent, CompanionIconVM>();

	private readonly Dictionary<Hero, Agent> createdStoredAgents = new Dictionary<Hero, Agent>();

	private readonly HashSet<Hero> _removedCompanions = new HashSet<Hero>();

	private readonly Dictionary<Hero, Agent> _friendlyLords = new Dictionary<Hero, Agent>();

	private readonly Dictionary<Hero, Agent> _enemyLords = new Dictionary<Hero, Agent>();

	private int _maxCompanionsToDisplay = GlobalSettings<WarAndAiTweaksSettings>.Instance.MaxCompanionsDisplayed;

	private bool IsCombatMission
	{
		get
		{
			Mission mission = ((MissionBehavior)this).Mission;
			return mission != null && mission.CombatType == Mission.MissionCombatType.Combat;
		}
	}

	public override void OnBehaviorInitialize()
	{
		base.OnBehaviorInitialize();
		_state = InitializationState.Failed;
	}

	public override void OnMissionScreenFinalize()
	{
		base.OnMissionScreenFinalize();
		CleanupUI();
	}

	public override void AfterStart()
	{
		base.AfterStart();
	}

	public override void OnAgentCreated(Agent agent)
	{
		base.OnAgentCreated(agent);
		if (agent == null)
		{
			return;
		}
		BasicCharacterObject character = agent.Character;
		CharacterObject val = (CharacterObject)(object)((character is CharacterObject) ? character : null);
		if (val == null || !((BasicCharacterObject)val).IsHero)
		{
			return;
		}
		Hero heroObject = val.HeroObject;
		if (heroObject == null || heroObject == Hero.MainHero)
		{
			return;
		}
		Hero mainHero = Hero.MainHero;
		if (((mainHero != null) ? mainHero.Clan : null) != null && heroObject.Clan == Hero.MainHero.Clan)
		{
			createdStoredAgents[heroObject] = agent;
		}
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.ShowFriendlyLordIcons)
		{
			Clan clan = heroObject.Clan;
			Kingdom val2 = ((clan != null) ? clan.Kingdom : null);
			Hero mainHero2 = Hero.MainHero;
			object obj;
			if (mainHero2 == null)
			{
				obj = null;
			}
			else
			{
				Clan clan2 = mainHero2.Clan;
				obj = ((clan2 != null) ? clan2.Kingdom : null);
			}
			Kingdom val3 = (Kingdom)obj;
			if (val2 != null && val3 != null && val2 == val3 && heroObject.Clan != Hero.MainHero.Clan)
			{
				_friendlyLords[heroObject] = agent;
			}
		}
		if (GlobalSettings<WarAndAiTweaksSettings>.Instance.ShowEnemyLordIcons)
		{
			Clan clan3 = heroObject.Clan;
			Kingdom val4 = ((clan3 != null) ? clan3.Kingdom : null);
			Hero mainHero3 = Hero.MainHero;
			object obj2;
			if (mainHero3 == null)
			{
				obj2 = null;
			}
			else
			{
				Clan clan4 = mainHero3.Clan;
				obj2 = ((clan4 != null) ? clan4.Kingdom : null);
			}
			Kingdom val5 = (Kingdom)obj2;
			if (val4 != null && val5 != null && val4 != val5 && val4.IsAtWarWith((IFaction)(object)val5))
			{
				_enemyLords[heroObject] = agent;
			}
		}
	}

	public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
	{
		Agent affectedAgent2 = affectedAgent;
		base.OnAgentRemoved(affectedAgent2, affectorAgent, agentState, blow);
		Hero key = createdStoredAgents.FirstOrDefault<KeyValuePair<Hero, Agent>>((KeyValuePair<Hero, Agent> kvp) => kvp.Value == affectedAgent2).Key;
		if (key != null)
		{
			createdStoredAgents.Remove(key);
			_removedCompanions.Add(key);
		}
		Hero key2 = _friendlyLords.FirstOrDefault<KeyValuePair<Hero, Agent>>((KeyValuePair<Hero, Agent> kvp) => kvp.Value == affectedAgent2).Key;
		if (key2 != null)
		{
			_friendlyLords.Remove(key2);
		}
		Hero key3 = _enemyLords.FirstOrDefault<KeyValuePair<Hero, Agent>>((KeyValuePair<Hero, Agent> kvp) => kvp.Value == affectedAgent2).Key;
		if (key3 != null)
		{
			_enemyLords.Remove(key3);
		}
		if (_agentIconMap.TryGetValue(affectedAgent2, out CompanionIconVM value))
		{
			value.IsVisible = false;
			((Collection<CompanionIconVM>)(object)_dataSource?.Companions).Remove(value);
			_agentIconMap.Remove(affectedAgent2);
		}
	}

	public override void OnMissionScreenTick(float dt)
	{
		base.OnMissionScreenTick(dt);
	}

	private bool TryInitializeUI()
	{
		if (((MissionBehavior)this).Mission == null || ((MissionBehavior)this).Mission.CurrentState != Mission.State.Continuing)
		{
			return false;
		}
		if (((MissionBehavior)this).Mission.PlayerTeam == null || !((MissionBehavior)this).Mission.PlayerTeam.IsValid)
		{
			return false;
		}
		if (Agent.Main == null || !Agent.Main.IsActive())
		{
			return false;
		}
		MissionScreen missionScreen = ((MissionView)this).MissionScreen;
		_camera = ((missionScreen != null) ? missionScreen.CombatCamera : null);
		if ((NativeObject)(object)_camera == (NativeObject)null)
		{
			return false;
		}
		if (_gauntletLayer != null)
		{
			CleanupUI();
		}
		_gauntletLayer = new GauntletLayer("GauntletLayer", 110);
		_dataSource = new CompanionHighlighterVM();
		_gauntletLayer.LoadMovie("CompanionMissionHUD", (ViewModel)(object)_dataSource);
		((ScreenBase)((MissionView)this).MissionScreen).AddLayer((ScreenLayer)(object)_gauntletLayer);
		_agentIconMap.Clear();
		return true;
	}

	private void addCompanionsToDatasource()
	{
		if (_dataSource == null)
		{
			return;
		}
		int num = _agentIconMap.Count;
		List<Hero> list = new List<Hero>();
		foreach (KeyValuePair<Hero, Agent> createdStoredAgent in createdStoredAgents)
		{
			Hero key = createdStoredAgent.Key;
			Agent value = createdStoredAgent.Value;
			if (key == null || key == Hero.MainHero || _removedCompanions.Contains(key))
			{
				continue;
			}
			if (value == null || !value.IsActive())
			{
				list.Add(key);
			}
			else if (!_agentIconMap.ContainsKey(value))
			{
				if (num >= _maxCompanionsToDisplay)
				{
					break;
				}
				BasicCharacterObject character = value.Character;
				CharacterObject val = (CharacterObject)(object)((character is CharacterObject) ? character : null);
				if (val != null && ((BasicCharacterObject)val).Name != null)
				{
					CompanionIconVM companionIconVM = new CompanionIconVM(((object)((BasicCharacterObject)val).Name).ToString())
					{
						IsVisible = false
					};
					((Collection<CompanionIconVM>)(object)_dataSource.Companions).Add(companionIconVM);
					_agentIconMap[value] = companionIconVM;
					num++;
				}
			}
		}
		foreach (Hero item in list)
		{
			createdStoredAgents.Remove(item);
			_removedCompanions.Add(item);
		}
	}

	private void addFriendlyLordsToDatasource()
	{
		if (_dataSource == null || !GlobalSettings<WarAndAiTweaksSettings>.Instance.ShowFriendlyLordIcons)
		{
			return;
		}
		int num = _agentIconMap.Count;
		List<Hero> list = new List<Hero>();
		foreach (KeyValuePair<Hero, Agent> friendlyLord in _friendlyLords)
		{
			Hero key = friendlyLord.Key;
			Agent value = friendlyLord.Value;
			if (key == null || key == Hero.MainHero || _removedCompanions.Contains(key))
			{
				continue;
			}
			if (value == null || !value.IsActive())
			{
				list.Add(key);
			}
			else if (!_agentIconMap.ContainsKey(value))
			{
				if (num >= _maxCompanionsToDisplay)
				{
					break;
				}
				BasicCharacterObject character = value.Character;
				CharacterObject val = (CharacterObject)(object)((character is CharacterObject) ? character : null);
				if (val != null && ((BasicCharacterObject)val).Name != null)
				{
					CompanionIconVM companionIconVM = new CompanionIconVM(((object)((BasicCharacterObject)val).Name).ToString(), CompanionIconVM.IconType.FriendlyLord)
					{
						IsVisible = false
					};
					((Collection<CompanionIconVM>)(object)_dataSource.Companions).Add(companionIconVM);
					_agentIconMap[value] = companionIconVM;
					num++;
				}
			}
		}
		foreach (Hero item in list)
		{
			_friendlyLords.Remove(item);
		}
	}

	private void addEnemyLordsToDatasource()
	{
		if (_dataSource == null || !GlobalSettings<WarAndAiTweaksSettings>.Instance.ShowEnemyLordIcons)
		{
			return;
		}
		int num = _agentIconMap.Count;
		List<Hero> list = new List<Hero>();
		foreach (KeyValuePair<Hero, Agent> enemyLord in _enemyLords)
		{
			Hero key = enemyLord.Key;
			Agent value = enemyLord.Value;
			if (key == null || key == Hero.MainHero || _removedCompanions.Contains(key))
			{
				continue;
			}
			if (value == null || !value.IsActive())
			{
				list.Add(key);
			}
			else if (!_agentIconMap.ContainsKey(value))
			{
				if (num >= _maxCompanionsToDisplay)
				{
					break;
				}
				BasicCharacterObject character = value.Character;
				CharacterObject val = (CharacterObject)(object)((character is CharacterObject) ? character : null);
				if (val != null && ((BasicCharacterObject)val).Name != null)
				{
					CompanionIconVM companionIconVM = new CompanionIconVM(((object)((BasicCharacterObject)val).Name).ToString(), CompanionIconVM.IconType.EnemyLord)
					{
						IsVisible = false
					};
					((Collection<CompanionIconVM>)(object)_dataSource.Companions).Add(companionIconVM);
					_agentIconMap[value] = companionIconVM;
					num++;
				}
			}
		}
		foreach (Hero item in list)
		{
			_enemyLords.Remove(item);
		}
	}

	private void UpdateIconPosition(Agent agent, CompanionIconVM iconVM)
	{
		Vec3 eyeGlobalPosition = agent.GetEyeGlobalPosition();
		eyeGlobalPosition.z += 0.8f;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		MBWindowManager.WorldToScreen(_camera, eyeGlobalPosition, ref num, ref num2, ref num3);
		Vec3 position = agent.Position;
		float num4 = position.Distance(Agent.Main.Position);
		if (iconVM.Type == CompanionIconVM.IconType.EnemyLord)
		{
			int enemyLordIconDistanceThreshold = GlobalSettings<WarAndAiTweaksSettings>.Instance.EnemyLordIconDistanceThreshold;
			if (num4 > (float)enemyLordIconDistanceThreshold)
			{
				iconVM.IsVisible = false;
				return;
			}
		}
		if (num3 > 0f && num > 0f && num2 > 0f && num < Screen.RealScreenResolutionWidth && num2 < Screen.RealScreenResolutionHeight)
		{
			iconVM.IsVisible = true;
			iconVM.PositionX = num - 40f;
			iconVM.PositionY = num2 - 25f;
			float num5 = MBMath.Lerp(1f, 0.5f, MBMath.ClampFloat((num4 - 5f) / 50f, 0f, 1f), 1E-05f);
			num5 = MBMath.ClampFloat(num5, 0.5f, 1f);
			iconVM.Width = 30f * num5;
			iconVM.Height = 30f * num5;
			iconVM.FontSize = (int)(16f * num5);
		}
		else
		{
			iconVM.IsVisible = false;
		}
	}

	private void UpdateCompanionIcons()
	{
		if ((NativeObject)(object)_camera == (NativeObject)null || Agent.Main == null || !Agent.Main.IsActive())
		{
			HideAllIcons();
			return;
		}
		if (IsPausedOrInMenu())
		{
			HideAllIcons();
			return;
		}
		List<Agent> list = new List<Agent>();
		foreach (KeyValuePair<Hero, Agent> createdStoredAgent in createdStoredAgents)
		{
			Agent value = createdStoredAgent.Value;
			if (_agentIconMap.TryGetValue(value, out CompanionIconVM value2))
			{
				if (value == null || !value.IsActive())
				{
					value2.IsVisible = false;
					list.Add(value);
				}
				else
				{
					UpdateIconPosition(value, value2);
				}
			}
		}
		foreach (KeyValuePair<Hero, Agent> friendlyLord in _friendlyLords)
		{
			Agent value3 = friendlyLord.Value;
			if (_agentIconMap.TryGetValue(value3, out CompanionIconVM value4))
			{
				if (value3 == null || !value3.IsActive())
				{
					value4.IsVisible = false;
					list.Add(value3);
				}
				else
				{
					UpdateIconPosition(value3, value4);
				}
			}
		}
		foreach (KeyValuePair<Hero, Agent> enemyLord in _enemyLords)
		{
			Agent value5 = enemyLord.Value;
			if (_agentIconMap.TryGetValue(value5, out CompanionIconVM value6))
			{
				if (value5 == null || !value5.IsActive())
				{
					value6.IsVisible = false;
					list.Add(value5);
				}
				else
				{
					UpdateIconPosition(value5, value6);
				}
			}
		}
		foreach (Agent item in list)
		{
			if (_agentIconMap.TryGetValue(item, out CompanionIconVM value7))
			{
				((Collection<CompanionIconVM>)(object)_dataSource.Companions).Remove(value7);
				_agentIconMap.Remove(item);
			}
		}
	}

	private void HideAllIcons()
	{
		foreach (CompanionIconVM value in _agentIconMap.Values)
		{
			value.IsVisible = false;
		}
	}

	private bool IsPausedOrInMenu()
	{
		MissionState current = MissionState.Current;
		if ((current != null && current.Paused) || MBCommon.IsPaused)
		{
			return true;
		}
		Game current2 = Game.Current;
		object obj;
		if (current2 == null)
		{
			obj = null;
		}
		else
		{
			GameStateManager gameStateManager = current2.GameStateManager;
			obj = ((gameStateManager != null) ? gameStateManager.ActiveState : null);
		}
		if (obj != null && !(Game.Current.GameStateManager.ActiveState is MissionState))
		{
			return true;
		}
		return false;
	}

	private void CleanupUI()
	{
		if (_gauntletLayer != null && ((MissionView)this).MissionScreen != null)
		{
			((ScreenBase)((MissionView)this).MissionScreen).RemoveLayer((ScreenLayer)(object)_gauntletLayer);
		}
		_gauntletLayer = null;
		if (_dataSource != null)
		{
			((ViewModel)_dataSource).OnFinalize();
			_dataSource = null;
		}
		_agentIconMap.Clear();
		_camera = null;
		_removedCompanions.Clear();
	}
}

