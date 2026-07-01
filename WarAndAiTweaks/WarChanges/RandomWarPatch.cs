using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace WarAndAiTweaks.WarChanges {

    [HarmonyPatch(typeof(KingdomDecisionProposalBehavior), "GetRandomWarDecision")]
    [HarmonyPriority(999)]
    public class RandomWarPatch {

        public static void Postfix(Clan clan, ref KingdomDecision __result) {
            if (!WarAndAiTweaks.Settings.EnableDeclareWarChanges)
                return;
            __result = null;
            if (clan.Kingdom.RulingClan == Clan.PlayerClan || clan != clan.Kingdom.RulingClan)
                return;
            if (clan.Kingdom.UnresolvedDecisions.FirstOrDefault((KingdomDecision x) => x is DeclareWarDecision) != null) 
                return;
            IFaction clanMapFaction = clan.Kingdom.MapFaction;
            IFaction BestFaction = null;
            BestFaction = Campaign.Current.Factions.Where(x => (x.IsRebelClan || x.IsKingdomFaction) && x.Settlements.Count > 0 && x != clanMapFaction).Aggregate((x, y) => getScoreOfDeclaringWar(clanMapFaction, x, clan) > getScoreOfDeclaringWar(clanMapFaction, y, clan) ? x : y);
            if (BestFaction == null) 
                return;
            Object[] parametersArray = new object[] { clan, clan.Kingdom, BestFaction };
            bool ConsiderWarResult = (bool)typeof(KingdomDecisionProposalBehavior).GetMethod("ConsiderWar", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(new KingdomDecisionProposalBehavior(), parametersArray);
            if (ConsiderWarResult)
                __result = new DeclareWarDecision(clan, BestFaction);
            else
                __result = null;
        }

        public static float getScoreOfDeclaringWar(IFaction factionConsidering, IFaction targetFaction, Clan evaluatingClan) {
            TextObject textObject = null;
            float distance = factionConsidering.FactionMidSettlement.GetPosition2D.Distance(targetFaction.FactionMidSettlement.GetPosition2D);
            return Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringWar(factionConsidering, targetFaction, evaluatingClan, out textObject) - distance * 1000f;
        }
    }
}