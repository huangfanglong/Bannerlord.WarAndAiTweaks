using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace FeastSystem;

public class FeastData
{
	[SaveableField(10)]
	public ItemRoster FeastRoster;

	[SaveableProperty(2)]
	public Hero Host { get; set; }

	[SaveableProperty(3)]
	public CampaignTime StartTime { get; set; }

	[SaveableProperty(4)]
	public Settlement Settlement { get; set; }

	[SaveableProperty(5)]
	public MBList<Hero> Attendees { get; set; } = new MBList<Hero>();


	[SaveableProperty(6)]
	public bool HostHasJoinedForFirstTime { get; set; } = false;


	[SaveableProperty(7)]
	public Kingdom Kingdom { get; set; }

	[SaveableProperty(8)]
	public CampaignTime PlayerInvitationAcceptedTime { get; set; }

	[SaveableProperty(9)]
	public bool PlayerHasJoinedForFirstTime { get; set; }
}
