using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace WarAndAITweaks.MarshalSystem;

[Serializable]
public class MarshalData
{
	[SaveableProperty(1)]
	public Hero CurrentMarshal { get; set; }

	[SaveableProperty(2)]
	public CampaignTime AppointmentStartDate { get; set; }

	[SaveableProperty(3)]
	public CampaignTime AppointmentEndDate { get; set; }
}
