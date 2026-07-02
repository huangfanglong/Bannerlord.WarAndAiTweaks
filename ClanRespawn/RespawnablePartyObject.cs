using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.SaveSystem;

namespace ClanRespawn;

public class RespawnablePartyObject
{
	[SaveableField(52)]
	public int currentWageLimit;

	[SaveableField(53)]
	public PartyObjective partyobjective;

	[SaveableField(54)]
	public Hero partyHero;

	[SaveableField(55)]
	public bool isLord;

	[SaveableField(56)]
	public bool isCaravan;

	public RespawnablePartyObject(MobileParty party)
	{
		currentWageLimit = party.PaymentLimit;
		partyobjective = party.Objective;
		partyHero = party.LeaderHero;
		isLord = party.IsLordParty;
		isCaravan = party.IsCaravan;
	}
}
