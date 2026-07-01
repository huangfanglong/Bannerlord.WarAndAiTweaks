using System.Linq;
using ClanRespawn;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;

namespace WarAndAITweaks.ClanRespawn;

[HarmonyPatch(typeof(MobileParty), "SetPartyObjective")]
[HarmonyPriority(0)]
public class UpdatesPartiesAggressivenessPatch
{
	public static void Prefix(ref MobileParty __instance, PartyObjective objective)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		MobileParty VMParty = __instance;
		RespawnablePartyObject respawnablePartyObject = ClanRespawnBehavior.Parties.Where((RespawnablePartyObject x) => x.partyHero == VMParty.LeaderHero).FirstOrDefault();
		if (respawnablePartyObject != null)
		{
			respawnablePartyObject.partyobjective = objective;
		}
	}
}
