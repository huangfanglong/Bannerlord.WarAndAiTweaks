using System.Linq;
using ClanRespawn;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;

namespace WarAndAITweaks.ClanRespawn;

[HarmonyPatch(typeof(ClanFinanceExpenseItemVM), "OnCurrentWageLimitUpdated")]
[HarmonyPriority(0)]
public class OnCurrentWageLimitUpdatedPatch
{
	public static void Postfix(ref ClanFinanceExpenseItemVM __instance, int newValue)
	{
		MobileParty VMParty = default(MobileParty);
		ref MobileParty reference = ref VMParty;
		object value = Traverse.Create((object)__instance).Field("_mobileParty").GetValue();
		reference = (MobileParty)((value is MobileParty) ? value : null);
		RespawnablePartyObject respawnablePartyObject = ClanRespawnBehavior.Parties.Where(delegate(RespawnablePartyObject x)
		{
			Hero partyHero = x.partyHero;
			MobileParty obj = VMParty;
			return partyHero == ((obj != null) ? obj.LeaderHero : null);
		}).FirstOrDefault();
		if (respawnablePartyObject != null)
		{
			respawnablePartyObject.currentWageLimit = __instance.CurrentWageLimit;
		}
	}
}
