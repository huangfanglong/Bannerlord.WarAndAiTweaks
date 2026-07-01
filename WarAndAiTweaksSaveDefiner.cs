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
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(FeastData), 145324326, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(FeastQuest), 145324327, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(RewardData), 145324329, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(RewardData.RewardInfo), 145324330, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(Strategic4XDiplomacyBehavior), 145324328, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(MarshalSystemBehavior), 145324331, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(MarshalData), 145324332, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(MarshalDecision), 145324333, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(MarshalDecision.MarshalOutcome), 145324334, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(RespawnablePartyObject), 1137982638, (IObjectResolver)null);
		((SaveableTypeDefiner)this).AddClassDefinition(typeof(ClanRespawnBehavior), 1337782639, (IObjectResolver)null);
	}

	protected override void DefineContainerDefinitions()
	{
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Kingdom, int>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<Hero>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<Kingdom>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<Settlement>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Kingdom, double>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Hero, CampaignTime>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Kingdom, float>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Clan, bool>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<MobileParty, CampaignTime>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Kingdom, CampaignTime>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Kingdom, Hero>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<string, float>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<int>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Kingdom, List<int>>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<Army>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<IFaction, CampaignTime>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<FeastData>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<string, CampaignTime>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Settlement, Hero>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Kingdom, MarshalData>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<RespawnablePartyObject>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<Kingdom, Kingdom>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(MBList<FeastData>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(MBList<Hero>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(HashSet<Hero>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<string, bool>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(List<string>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<FeastData, RewardData>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<string, FeastQuest>));
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<string, RewardData.RewardInfo>));
	}
}
