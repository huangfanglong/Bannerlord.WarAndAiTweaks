using System.Collections.Generic;
using ClanRespawn;
using FeastSystem;
using Strategic4XDiplomacy;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Resolvers;
using WarAndAITweaks.MarshalSystem;
using WarAndAITweaks.TodayWeFeast;

public class WarAndAiTweaksSaveDefiner : SaveableTypeDefiner
{
	public WarAndAiTweaksSaveDefiner()
		: base(1852400000)
	{
	}

	protected override void DefineClassTypes()
	{
		base.AddClassDefinition(typeof(FeastData), 145324326, (IObjectResolver)null);
		base.AddClassDefinition(typeof(FeastQuest), 145324327, (IObjectResolver)null);
		base.AddClassDefinition(typeof(RewardData), 145324329, (IObjectResolver)null);
		base.AddClassDefinition(typeof(RewardData.RewardInfo), 145324330, (IObjectResolver)null);
		base.AddClassDefinition(typeof(Strategic4XDiplomacyBehavior), 145324328, (IObjectResolver)null);
		base.AddClassDefinition(typeof(MarshalSystemBehavior), 145324331, (IObjectResolver)null);
		base.AddClassDefinition(typeof(MarshalData), 145324332, (IObjectResolver)null);
		base.AddClassDefinition(typeof(MarshalDecision), 145324333, (IObjectResolver)null);
		base.AddClassDefinition(typeof(MarshalDecision.MarshalOutcome), 145324334, (IObjectResolver)null);
		base.AddClassDefinition(typeof(RespawnablePartyObject), 1137982638, (IObjectResolver)null);
		base.AddClassDefinition(typeof(ClanRespawnBehavior), 1337782639, (IObjectResolver)null);
	}

	protected override void DefineContainerDefinitions()
	{
		base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, int>));
		base.ConstructContainerDefinition(typeof(List<Hero>));
		base.ConstructContainerDefinition(typeof(List<Kingdom>));
		base.ConstructContainerDefinition(typeof(List<Settlement>));
		base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, double>));
		base.ConstructContainerDefinition(typeof(Dictionary<Hero, CampaignTime>));
		base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, float>));
		base.ConstructContainerDefinition(typeof(Dictionary<Clan, bool>));
		base.ConstructContainerDefinition(typeof(Dictionary<MobileParty, CampaignTime>));
		base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, CampaignTime>));
		base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, Hero>));
		base.ConstructContainerDefinition(typeof(Dictionary<string, float>));
		base.ConstructContainerDefinition(typeof(List<int>));
		base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, List<int>>));
		base.ConstructContainerDefinition(typeof(List<Army>));
		base.ConstructContainerDefinition(typeof(Dictionary<IFaction, CampaignTime>));
		base.ConstructContainerDefinition(typeof(List<FeastData>));
		base.ConstructContainerDefinition(typeof(Dictionary<string, CampaignTime>));
		base.ConstructContainerDefinition(typeof(Dictionary<Settlement, Hero>));
		base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, MarshalData>));
		base.ConstructContainerDefinition(typeof(List<RespawnablePartyObject>));
		base.ConstructContainerDefinition(typeof(Dictionary<Kingdom, Kingdom>));
		base.ConstructContainerDefinition(typeof(MBList<FeastData>));
		base.ConstructContainerDefinition(typeof(MBList<Hero>));
		base.ConstructContainerDefinition(typeof(HashSet<Hero>));
		base.ConstructContainerDefinition(typeof(Dictionary<string, bool>));
		base.ConstructContainerDefinition(typeof(List<string>));
		base.ConstructContainerDefinition(typeof(Dictionary<FeastData, RewardData>));
		base.ConstructContainerDefinition(typeof(Dictionary<string, FeastQuest>));
		base.ConstructContainerDefinition(typeof(Dictionary<string, RewardData.RewardInfo>));
	}
}
