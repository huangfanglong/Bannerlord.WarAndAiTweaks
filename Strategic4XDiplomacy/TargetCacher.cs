using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace Strategic4XDiplomacy;

public class TargetCacher
{
	public MBList<Kingdom> NearbyTargets = new MBList<Kingdom>();

	public MBList<Kingdom> Neighbors = new MBList<Kingdom>();

	public bool needsUpdate = true;
}
