using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace WarAndAITweaks.UI;

[GameStateScreen(typeof(WarAndAiTweaksManagementState))]
public class WarAndAIManagementScreen : ScreenBase, IGameStateListener
{
	private WarAndAiTweaksManagementState _warAndAiTweaksState;

	private GauntletLayer _mainLayer;

	private GauntletLayer _contentLayer;

	private WarAndAiTweaksManagementVM _datasource;

	private string _currentTabMovie = "";

	private IGauntletMovie _currentMovie;

	public WarAndAIManagementScreen(WarAndAiTweaksManagementState warAndAiTweaks)
	{
		_warAndAiTweaksState = warAndAiTweaks;
		((GameState)_warAndAiTweaksState).RegisterListener((IGameStateListener)(object)this);
	}

	void IGameStateListener.OnActivate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (_mainLayer == null)
		{
			_mainLayer = new GauntletLayer(1, "GauntletLayer", true);
			_datasource = new WarAndAiTweaksManagementVM(_warAndAiTweaksState, this);
			_mainLayer.LoadMovie("WarAndAITweaksManagement", (ViewModel)(object)_datasource);
			((ScreenLayer)_mainLayer).InputRestrictions.SetInputRestrictions(true, (InputUsageMask)7);
			((ScreenLayer)_mainLayer).IsFocusLayer = true;
			ScreenManager.TrySetFocus((ScreenLayer)(object)_mainLayer);
			((ScreenBase)this).AddLayer((ScreenLayer)(object)_mainLayer);
		}
	}

	void IGameStateListener.OnDeactivate()
	{
		GauntletLayer mainLayer = _mainLayer;
		if (mainLayer != null)
		{
			((ScreenLayer)mainLayer).InputRestrictions.ResetInputRestrictions();
		}
		if (_mainLayer != null)
		{
			((ScreenBase)this).RemoveLayer((ScreenLayer)(object)_mainLayer);
			_mainLayer = null;
		}
		WarAndAiTweaksManagementVM datasource = _datasource;
		if (datasource != null)
		{
			((ViewModel)datasource).OnFinalize();
		}
		_datasource = null;
		_currentTabMovie = "";
	}

	protected override void OnFrameTick(float dt)
	{
		((ScreenBase)this).OnFrameTick(dt);
		GauntletLayer mainLayer = _mainLayer;
		if (mainLayer != null && ((ScreenLayer)mainLayer).Input.IsKeyPressed((InputKey)16))
		{
			_datasource?.ExecuteCancel();
		}
		GauntletLayer mainLayer2 = _mainLayer;
		if (mainLayer2 != null && ((ScreenLayer)mainLayer2).Input.IsKeyPressed((InputKey)45))
		{
			_datasource?.ExecuteDone();
		}
	}

	void IGameStateListener.OnInitialize()
	{
	}

	void IGameStateListener.OnFinalize()
	{
	}
}
