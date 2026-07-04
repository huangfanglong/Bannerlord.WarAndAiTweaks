using ClanRespawn;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace WarAndAITweaks.ClanRespawn;

[HarmonyPatch(typeof(Clan), "CreateCompanionToLordClan")]
[HarmonyPriority(0)]
public class CreateClanPatch
{
	public static void Prefix(Hero hero, Settlement settlement, TextObject clanName, int newClanIconId)
	{
		Hero hero2 = hero;
		ClanRespawnBehavior.Parties.RemoveAll((RespawnablePartyObject x) => x.partyHero == hero2);
	}
}
