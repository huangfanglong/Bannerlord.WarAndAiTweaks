using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace WarAndAITweaks.TodayWeFeast;

internal class FeastMissionLogic : MissionBehavior
{
	public static SoundEvent _ambienceLoop;

	public static SoundEvent _tavernTrack;

	public static SoundEvent _musicianTrack;

	public override MissionBehaviorType BehaviorType => (MissionBehaviorType)1;

	public override void AfterStart()
	{
		base.AfterStart();
		PlayFeastAmbience();
	}

	public static void PlayFeastAmbience()
	{
		StopFeastAmbience();
		Mission current = Mission.Current;
		if (!((NativeObject)(object)((current != null) ? current.Scene : null) != (NativeObject)null))
		{
			return;
		}
		Scene scene = Mission.Current.Scene;
		int eventIdFromString = SoundEvent.GetEventIdFromString("event:/mission/ambient/area/interior/tavern");
		if (eventIdFromString != -1)
		{
			_ambienceLoop = SoundEvent.CreateEvent(eventIdFromString, scene);
			SoundEvent ambienceLoop = _ambienceLoop;
			if (ambienceLoop != null)
			{
				ambienceLoop.Play();
			}
		}
		int eventIdFromString2 = SoundEvent.GetEventIdFromString("event:/mission/ambient/detail/tavern_track_01");
		if (eventIdFromString2 != -1)
		{
			_tavernTrack = SoundEvent.CreateEvent(eventIdFromString2, scene);
			SoundEvent tavernTrack = _tavernTrack;
			if (tavernTrack != null)
			{
				tavernTrack.Play();
			}
		}
		int eventIdFromString3 = SoundEvent.GetEventIdFromString("event:/music/musicians/vlandia/01");
		if (eventIdFromString3 != -1)
		{
			_musicianTrack = SoundEvent.CreateEvent(eventIdFromString3, scene);
			if (_musicianTrack != null)
			{
				_musicianTrack.Play();
			}
		}
	}

	public static void StopFeastAmbience()
	{
		SoundEvent ambienceLoop = _ambienceLoop;
		if (ambienceLoop != null)
		{
			ambienceLoop.Stop();
		}
		SoundEvent tavernTrack = _tavernTrack;
		if (tavernTrack != null)
		{
			tavernTrack.Stop();
		}
		SoundEvent musicianTrack = _musicianTrack;
		if (musicianTrack != null)
		{
			musicianTrack.Stop();
		}
	}
}
