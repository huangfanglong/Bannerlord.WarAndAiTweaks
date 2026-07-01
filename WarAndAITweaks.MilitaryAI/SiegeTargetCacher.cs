using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace WarAndAITweaks.MilitaryAI;

public class SiegeTargetCacher
{
	public MBList<Settlement> Targets = new MBList<Settlement>();

	public bool needsUpdate = true;
}
