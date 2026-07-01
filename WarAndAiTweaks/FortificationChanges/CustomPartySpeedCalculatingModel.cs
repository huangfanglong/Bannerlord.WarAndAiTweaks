using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace WarAndAiTweaks.FortificationChanges {
    public class CustomPartySpeedCalculatingModel : DefaultPartySpeedCalculatingModel {

        public override ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber explanation) {
            ExplainedNumber result = base.CalculateFinalSpeed(mobileParty, explanation);
            if (!WarAndAiTweaks.Settings.SlowDownPenalty)
                return result;

            if (mobileParty == null || mobileParty.LeaderHero == null || mobileParty.LeaderHero.Clan == null || mobileParty.LeaderHero.Clan.Kingdom == null || Helpers.FactionHelper.GetEnemyKingdoms(mobileParty.LeaderHero.Clan.Kingdom.MapFaction).Count() <= 0)
                return result;

            if (GetsSpeedPenalty(mobileParty))
                result.AddFactor(-.30f, new TaleWorlds.Localization.TextObject("Nearby Enemy Fortifications"));

            return result;
        }

        public static bool GetsSpeedPenalty(MobileParty party) {
            foreach (Kingdom kingdom in Helpers.FactionHelper.GetEnemyKingdoms(party.LeaderHero.Clan.Kingdom.MapFaction)) {
                if (kingdom.Settlements.Where(x => !x.IsVillage && ((party.GetPosition2D.Distance(x.GetPosition2D) <= 20f && x.IsCastle) || (party.GetPosition2D.Distance(x.GetPosition2D) <= 30f && x.IsTown))).Count() > 0) { return true; }
            }
            return false;
        }
    }
}