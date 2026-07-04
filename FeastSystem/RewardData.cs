using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace FeastSystem;

public class RewardData
{
	public class RewardInfo
	{
		[SaveableField(1)]
		public double nextFeastRewardTime;

		[SaveableField(2)]
		public double nextDuelRewardTime;
	}

	[SaveableField(20)]
	public Dictionary<string, RewardInfo> Rewards = new Dictionary<string, RewardInfo>();

	public RewardInfo GetOrCreate(Hero host, Hero guest)
	{
		string key = ((MBObjectBase)host).StringId + "|" + ((MBObjectBase)guest).StringId;
		if (!Rewards.TryGetValue(key, out RewardInfo value))
		{
			value = new RewardInfo();
			Rewards[key] = value;
		}
		return value;
	}
}
