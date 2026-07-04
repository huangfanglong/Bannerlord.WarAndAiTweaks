using TaleWorlds.CampaignSystem;

namespace WarAndAITweaks.War_Peace_AI_Overhaul;

public class DiplomaticAgreementInfo
{
	public object Agreement { get; set; }

	public string AgreementType { get; set; }

	public bool IsExpired { get; set; }

	public Kingdom Kingdom1 { get; set; }

	public Kingdom Kingdom2 { get; set; }

	public string Kingdom1Name { get; set; }

	public string Kingdom2Name { get; set; }

	public string Description { get; set; }
}
