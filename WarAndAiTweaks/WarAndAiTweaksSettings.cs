using System.Collections.Generic;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace WarAndAiTweaks;

internal sealed class WarAndAiTweaksSettings : AttributeGlobalSettings<WarAndAiTweaksSettings>
{
	public override string Id => "WarAndAiTweaks";

	public override string DisplayName => "War & AI Tweaks";

	public override string FolderName => "WarAndAiTweaks";

	public override string FormatType => "xml";

	[SettingPropertyDropdown("Management UI Hotkey. ALT+Selection", Order = 50, RequireRestart = false, HintText = "Select the key to open the War & AI Tweaks Management UI.")]
	[SettingPropertyGroup("UI Settings", GroupOrder = 50)]
	public Dropdown<string> ManagementUIHotkey { get; set; } = new Dropdown<string>((IEnumerable<string>)new string[10] { "Q", "X", "J", "K", "V", "B", "N", "M", "L", "O" }, 0);


	[SettingPropertyBool("Toggle Strategic AI", Order = 100, RequireRestart = true, IsToggle = true, HintText = "Toggle the Strategic AI system and associated features.")]
	[SettingPropertyGroup("Strategic AI", GroupOrder = 100)]
	public bool EnableWarPeaceAIOverhaul { get; set; } = true;


	[SettingPropertyBool("Toggle Player Kingdom AI Clan Voting", Order = 103, RequireRestart = false, HintText = "Toggle the ability for player kingdom AI clans to vote on Peace/War Decisions. Turning this off essentially gives you full control of War/Peace as there is no resistance from your AI clans.")]
	[SettingPropertyGroup("Strategic AI", GroupOrder = 100)]
	public bool EnablePlayerKingdomClanVoting { get; set; } = false;


	[SettingPropertyBool("Toggle Base Game Tribute Calculations", Order = 102, RequireRestart = false, HintText = "Toggle the usage of base game Tribute Calculations. If this is enabled, the base game tribute system will be used when making peace. When this is disabled, tributes are disabled and no payments are made on either side.")]
	[SettingPropertyGroup("Strategic AI", GroupOrder = 100)]
	public bool EnableBaseGameTributes { get; set; } = false;


	[SettingPropertyBool("Enable Player Peace Fix", Order = 101, RequireRestart = true, HintText = "Toggle the fix for the player being able to force the AI into peace.")]
	[SettingPropertyGroup("Strategic AI", GroupOrder = 100)]
	public bool EnablePlayerPeaceFix { get; set; } = true;


	[SettingPropertyInteger("Days of Peace for Minimum War Fatigue", 1, 300, "0", Order = 104, RequireRestart = false, HintText = "Number of consecutive days at peace required for a kingdom's war fatigue to fully recover (reach minimum).")]
	[SettingPropertyGroup("Strategic AI", GroupOrder = 100)]
	public int PeaceFatigueDays { get; set; } = 40;


	[SettingPropertyInteger("Days of War for Maximum War Fatigue", 1, 300, "0", Order = 105, RequireRestart = false, HintText = "Number of consecutive days at war required for a kingdom's war fatigue to reach its maximum value.")]
	[SettingPropertyGroup("Strategic AI", GroupOrder = 100)]
	public int WarFatigueDays { get; set; } = 150;


	[SettingPropertyBool("Show War Fatigue Debug Output", Order = 106, RequireRestart = false, HintText = "If enabled, detailed debug messages for war fatigue calculations will be shown in-game. Intended for troubleshooting and development.")]
	[SettingPropertyGroup("Strategic AI", GroupOrder = 100)]
	public bool ShowWarFatigueDebug { get; set; } = false;


	[SettingPropertyBool("Enable Enhanced Military", Order = 150, RequireRestart = true, IsToggle = true, HintText = "Toggle the Enhanced Military AI")]
	[SettingPropertyGroup("Enhanced Military System", GroupOrder = 150)]
	public bool EnableEnhancedMilitary { get; set; } = true;


	[SettingPropertyBool("Enable Player AI Party Army Creation", Order = 151, RequireRestart = false, HintText = "Allows player clan AI parties to create armies")]
	[SettingPropertyGroup("Enhanced Military System", GroupOrder = 150)]
	public bool EnablePlayerClanArmyCreation { get; set; } = false;


	[SettingPropertyBool("Prevent Clan Parties in AI Armies", Order = 152, RequireRestart = false, HintText = "Stops AI kingdoms from calling player-clan parties to their armies.")]
	[SettingPropertyGroup("Enhanced Military System")]
	public bool PreventClanPartiesFromBeingCalled { get; set; } = true;


	[SettingPropertyInteger("Required Military Advantage for Siege", 1, 10, "0", Order = 153, RequireRestart = false, HintText = "Determines how much strength the AI wants compared to the defenses of a siege target. For example, the default 2x says they want 2 times the strength of the defenders.")]
	[SettingPropertyGroup("Enhanced Military System")]
	public int MilitaryAdvantage { get; set; } = 2;


	[SettingPropertyBool("Allow Raiding by Honorable/Merciful Lords", Order = 154, RequireRestart = false, HintText = "If enabled, lords with positive Honor or Mercy traits are allowed to raid settlements. If disabled (default), these lords will not initiate raids.")]
	[SettingPropertyGroup("Enhanced Military System")]
	public bool AllowRaidingTooGoodLords { get; set; } = false;


	[SettingPropertyBool("Ignore Low Relationship Settlements for Defense", Order = 155, RequireRestart = false, HintText = "If enabled, AI parties will ignore defending settlements where their relationship to the owner is below -10.")]
	[SettingPropertyGroup("Enhanced Military System")]
	public bool IgnoreLowRelationshipDefense { get; set; } = true;


	[SettingPropertyBool("Enable BattleChatter", Order = 200, RequireRestart = true, IsToggle = true, HintText = "Toggle the battle chatter system.")]
	[SettingPropertyGroup("BattleChatter", GroupOrder = 200)]
	public bool EnableBattleChatter { get; set; } = true;


	[SettingPropertyFloatingInteger("Formation Ambient Chatter Chance", 0f, 1f, "#0.00", Order = 201, RequireRestart = false, HintText = "Chance (0-1) for formation ambient chatter to trigger each check. Lower = less frequent.")]
	[SettingPropertyGroup("BattleChatter", GroupOrder = 200)]
	public float FormationAmbientChance { get; set; } = 0.25f;


	[SettingPropertyFloatingInteger("Kill Cry Chatter Chance", 0f, 1f, "#0.00", Order = 202, RequireRestart = false, HintText = "Chance (0-1) for a kill cry to trigger when a hero kills an enemy.")]
	[SettingPropertyGroup("BattleChatter")]
	public float CryChance { get; set; } = 0.7f;


	[SettingPropertyFloatingInteger("Hit Reaction Chatter Chance", 0f, 1f, "#0.00", Order = 204, RequireRestart = false, HintText = "Chance (0-1) for a hit reaction chatter to trigger when a hero is hit.")]
	[SettingPropertyGroup("BattleChatter")]
	public float HitChance { get; set; } = 0.25f;


	[SettingPropertyFloatingInteger("Individual Chatter Time Cooldown", 1f, 60f, "#0.0", Order = 208, RequireRestart = false, HintText = "Cooldown in seconds between individual chatter events.")]
	[SettingPropertyGroup("BattleChatter")]
	public float IndividualChatterTimeCooldown { get; set; } = 10f;


	[SettingPropertyFloatingInteger("Formation Chatter Time Cooldown", 1f, 60f, "#0.0", Order = 209, RequireRestart = false, HintText = "Cooldown in seconds between formation chatter events.")]
	[SettingPropertyGroup("BattleChatter")]
	public float FormationChatterTimeCooldown { get; set; } = 10f;


	[SettingPropertyBool("Allow Player Battle Chatter", Order = 210, RequireRestart = false, HintText = "If enabled, the player character can speak in battle chatter events (kill cries, hit reactions, etc).")]
	[SettingPropertyGroup("BattleChatter")]
	public bool IncludePlayerInDialog { get; set; } = false;


	[SettingPropertyBool("Allow Profanity Lines", Order = 211, RequireRestart = false, HintText = "If enabled, will allow dialog lines with profanity in it")]
	[SettingPropertyGroup("BattleChatter")]
	public bool AllowProfanity { get; set; } = false;


	[SettingPropertyBool("Enable Marshal System", Order = 250, RequireRestart = true, IsToggle = true, HintText = "Toggle the Marshal System for kingdom leadership.")]
	[SettingPropertyGroup("Marshal System", GroupOrder = 250)]
	public bool EnableMarshalSystem { get; set; } = true;


	[SettingPropertyInteger("Marshal Term Length (Days)", 1, 365, "0", Order = 251, RequireRestart = false, HintText = "Number of in-game days a marshal serves before a new election is called.")]
	[SettingPropertyGroup("Marshal System", GroupOrder = 250)]
	public int MarshalTermDays { get; set; } = 100;


	[SettingPropertyFloatingInteger("Army Calling Influence Multiplier", 0.1f, 10f, "#0.0", Order = 252, RequireRestart = false, HintText = "Adjusts the influence cost for calling parties to armies. Lower values make it easier to form armies, while higher values make it harder. Increasing this value makes the marshal role much more important, as only the marshal can efficiently call armies. Default is 1.0 (base game cost).")]
	[SettingPropertyGroup("Marshal System", GroupOrder = 250)]
	public float ArmyCallingInfluenceMultiplier { get; set; } = 1f;


	[SettingPropertyBool("Enable Companion UI Changes", Order = 300, RequireRestart = true, IsToggle = true, HintText = "Toggle enhanced Companion UI features.")]
	[SettingPropertyGroup("Companion UI Changes", GroupOrder = 300)]
	public bool EnableCompanionUIChanges { get; set; } = true;


	[SettingPropertyDropdown("Companion Icon", Order = 303, RequireRestart = false, HintText = "Select the icon shown above clan members' heads in battle.")]
	[SettingPropertyGroup("Companion UI Changes")]
	public Dropdown<string> CompanionIcon { get; set; } = new Dropdown<string>((IEnumerable<string>)new string[4] { "Hero", "Noble", "NPC", "Spear" }, 0);


	[SettingPropertyInteger("Max Companions Displayed", 1, 1000, "0", Order = 302, RequireRestart = false, HintText = "Limits the number of companions shown with icons in battle. 1,000 is the default.")]
	[SettingPropertyGroup("Companion UI Changes")]
	public int MaxCompanionsDisplayed { get; set; } = 1000;


	[SettingPropertyDropdown("Companion Name Color", Order = 304, RequireRestart = false, HintText = "Select the color for companion name text above their head.")]
	[SettingPropertyGroup("Companion UI Changes")]
	public Dropdown<string> CompanionNameColor { get; set; } = new Dropdown<string>((IEnumerable<string>)new string[10] { "White", "Yellow", "Red", "Green", "Blue", "Cyan", "Magenta", "Orange", "Purple", "Black" }, 0);


	[SettingPropertyBool("Show Friendly Lord Icons", Order = 304, RequireRestart = false, HintText = "If enabled, shows icons above friendly lords (same kingdom as player) on the battlefield.")]
	[SettingPropertyGroup("Companion UI Changes")]
	public bool ShowFriendlyLordIcons { get; set; } = true;


	[SettingPropertyDropdown("Friendly Lord Icon", Order = 305, RequireRestart = false, HintText = "Select the icon shown above friendly lords' heads in battle (same kingdom as player).")]
	[SettingPropertyGroup("Companion UI Changes")]
	public Dropdown<string> FriendlyLordIcon { get; set; } = new Dropdown<string>((IEnumerable<string>)new string[4] { "Hero", "Noble", "NPC", "Spear" }, 1);


	[SettingPropertyDropdown("Friendly Lord Name Color", Order = 306, RequireRestart = false, HintText = "Select the color for friendly lord name text above their head (same kingdom as player).")]
	[SettingPropertyGroup("Companion UI Changes")]
	public Dropdown<string> FriendlyLordNameColor { get; set; } = new Dropdown<string>((IEnumerable<string>)new string[10] { "White", "Yellow", "Red", "Green", "Blue", "Cyan", "Magenta", "Orange", "Purple", "Black" }, 1);


	[SettingPropertyBool("Show Enemy Lord Icons", Order = 307, RequireRestart = false, HintText = "If enabled, shows icons above enemy lords (not on the player team) on the battlefield.")]
	[SettingPropertyGroup("Companion UI Changes")]
	public bool ShowEnemyLordIcons { get; set; } = false;


	[SettingPropertyDropdown("Enemy Lord Icon", Order = 308, RequireRestart = false, HintText = "Select the icon shown above enemy lords' heads in battle.")]
	[SettingPropertyGroup("Companion UI Changes")]
	public Dropdown<string> EnemyLordIcon { get; set; } = new Dropdown<string>((IEnumerable<string>)new string[4] { "Hero", "Noble", "NPC", "Spear" }, 0);


	[SettingPropertyDropdown("Enemy Lord Name Color", Order = 309, RequireRestart = false, HintText = "Select the color for enemy lord name text above their head.")]
	[SettingPropertyGroup("Companion UI Changes")]
	public Dropdown<string> EnemyLordNameColor { get; set; } = new Dropdown<string>((IEnumerable<string>)new string[10] { "White", "Yellow", "Red", "Green", "Blue", "Cyan", "Magenta", "Orange", "Purple", "Black" }, 2);


	[SettingPropertyInteger("Enemy Lord Icon Distance Threshold", 1, 10000, "0", Order = 310, RequireRestart = false, HintText = "Enemy lord icons will only be shown if the lord is within this distance from the player. Default is 75.")]
	[SettingPropertyGroup("Companion UI Changes")]
	public int EnemyLordIconDistanceThreshold { get; set; } = 75;


	[SettingPropertyBool("Enable Today We Feast System", Order = 350, RequireRestart = true, IsToggle = true, HintText = "Toggle the TodayWeFeast system (AI and player feasts).")]
	[SettingPropertyGroup("Today We Feast System", GroupOrder = 350)]
	public bool EnableTodayWeFeast { get; set; } = false;


	[SettingPropertyInteger("Kingdom Feast Cooldown", 1, 100, "0", Order = 351, RequireRestart = false, HintText = "Numbers of days that must pass before the end of the last feast before a kingdom may create another feast.")]
	[SettingPropertyGroup("Today We Feast System")]
	public int TodayWeFeastCooldown { get; set; } = 10;


	[SettingPropertyFloatingInteger("AI Chance to Host", 0f, 1f, "0.00", Order = 352, RequireRestart = false, HintText = "Sets the chance the AI will host a feast (If conditions are met like not at war, has spouse, gold, and settlements)")]
	[SettingPropertyGroup("Today We Feast System")]
	public float TodayWeFeastAIChance { get; set; } = 0.3f;


	[SettingPropertyFloatingInteger("AI Chance to get upset", 0f, 1f, "0.00", Order = 353, RequireRestart = false, HintText = "AI Chance to get upset if you decline their invitation to a feast")]
	[SettingPropertyGroup("Today We Feast System")]
	public float TodayWeFeastAIUpsetChance { get; set; } = 0.1f;


	[SettingPropertyInteger("Feast Dialog Reward Cooldown", 1, 100, "0", Order = 354, RequireRestart = false, HintText = "Numbers of days that must pass you may speak with a guest of a feast/host of a host to gain relationship/renown bonuses.")]
	[SettingPropertyGroup("Today We Feast System")]
	public int TodayWeFeastDialogCooldown { get; set; } = 2;


	[SettingPropertyInteger("AI Feast Minimum Days", 1, 100, "0", Order = 355, RequireRestart = false, HintText = "Minimum number of days before AI guests may start leaving a feast.")]
	[SettingPropertyGroup("Today We Feast System")]
	public int TodayWeFeastAIMinDays { get; set; } = 10;


	[SettingPropertyInteger("AI Feast Maximum Days", 1, 100, "0", Order = 356, RequireRestart = false, HintText = "Maximum number of days before AI feasts are likely to end.")]
	[SettingPropertyGroup("Today We Feast System")]
	public int TodayWeFeastAIMaxDays { get; set; } = 20;


	[SettingPropertyBool("Enable Clan Respawn", Order = 400, RequireRestart = true, IsToggle = true, HintText = "Automatically respawn all player-clan parties after they are defeated and restore their previous wage settings.")]
	[SettingPropertyGroup("Clan Respawn", GroupOrder = 400)]
	public bool EnableClanRespawn { get; set; } = true;


	[SettingPropertyBool("Enable Fill-Stacks", Order = 450, RequireRestart = false, IsToggle = true, HintText = "Controls the number of free troops AI lords receive when they respawn.")]
	[SettingPropertyGroup("Fill-Stacks", GroupOrder = 450)]
	public bool EnableFillStacks { get; set; } = true;


	[SettingPropertyInteger("Fill-Stacks Troop Count", 1, 100, "0", Order = 451, RequireRestart = false, HintText = "Number of free troops AI lords get when they respawn.")]
	[SettingPropertyGroup("Fill-Stacks")]
	public int FillStackTroopCount { get; set; } = 1;


	[SettingPropertyBool("Apply Fill-Stacks to Mercenaries", Order = 452, RequireRestart = false, HintText = "If enabled, mercenary clans are also affected by Fill-Stacks.")]
	[SettingPropertyGroup("Fill-Stacks")]
	public bool FillStackMercenaries { get; set; } = false;


	[SettingPropertyBool("Enable Auto Build Feature", Order = 500, RequireRestart = true, IsToggle = true, HintText = "Toggle allowing players to request their own governors to auto upgrade buildings in their governed fief.")]
	[SettingPropertyGroup("Auto Build Feature", GroupOrder = 500)]
	public bool EnableAutoBuilder { get; set; } = true;


	[SettingPropertyBool("Enable Fiefless Clan Relationship loss", Order = 550, RequireRestart = false, IsToggle = true, HintText = "Toggle the Relationship loss for fiefless clans.")]
	[SettingPropertyGroup("Fiefless Clans", GroupOrder = 550)]
	public bool EnableFieflessClansLoss { get; set; } = true;


	[SettingPropertyInteger("Fiefless Clan Relationship loss (Per Day)", -10, 0, "0", Order = 551, RequireRestart = false, HintText = "Daily relation change for clans with no fief.")]
	[SettingPropertyGroup("Fiefless Clans")]
	public int FieflessClansLoss { get; set; } = -1;


	[SettingPropertyBool("Enable Militia Boost", Order = 600, RequireRestart = true, IsToggle = true, HintText = "Increases militia in castles and towns, making them harder to capture. (Player and AI)")]
	[SettingPropertyGroup("Militia Boost", GroupOrder = 600)]
	public bool EnableMilitiaBoost { get; set; } = true;


	[SettingPropertyInteger("Militia Boost (Castle)", 0, 100, "0", Order = 601, RequireRestart = false, HintText = "Additional militia per day from Militia Grounds in castles. (Player and AI)")]
	[SettingPropertyGroup("Militia Boost")]
	public int MilitiaBoostCastle { get; set; } = 5;


	[SettingPropertyInteger("Militia Boost (Town)", 0, 100, "0", Order = 602, RequireRestart = false, HintText = "Additional militia per day from Militia Grounds in towns. (Player and AI)")]
	[SettingPropertyGroup("Militia Boost")]
	public int MilitiaBoostTown { get; set; } = 10;


	[SettingPropertyBool("Enable Party-Size Boost", Order = 650, RequireRestart = true, IsToggle = true, HintText = "Adds party size for every village, castle and town a clan owns. (Player and AI)")]
	[SettingPropertyGroup("Party Size", GroupOrder = 650)]
	public bool EnablePartySizeBoost { get; set; } = true;


	[SettingPropertyInteger("Party Size Bonus (Village)", 0, 100, "0", Order = 651, RequireRestart = false, HintText = "Party size bonus per village owned by the clan.")]
	[SettingPropertyGroup("Party Size")]
	public int PartySizeBonusVillage { get; set; } = 1;


	[SettingPropertyInteger("Party Size Bonus (Castle)", 0, 100, "0", Order = 652, RequireRestart = false, HintText = "Party size bonus per castle owned by the clan.")]
	[SettingPropertyGroup("Party Size")]
	public int PartySizeBonusCastle { get; set; } = 5;


	[SettingPropertyInteger("Party Size Bonus (Town)", 0, 100, "0", Order = 653, RequireRestart = false, HintText = "Party size bonus per town owned by the clan.")]
	[SettingPropertyGroup("Party Size")]
	public int PartySizeBonusTown { get; set; } = 10;


	[SettingPropertyBool("Enable Garrison Wage Modifer", Order = 700, RequireRestart = true, IsToggle = true, HintText = "Enables modifying the Garrison Wage. (Player and AI)")]
	[SettingPropertyGroup("Garrison Wage", GroupOrder = 700)]
	public bool EnableGarrisonWageModifier { get; set; } = false;


	[SettingPropertyFloatingInteger("Garrison Wage Multiplier", 0f, 10f, "0.00", Order = 701, RequireRestart = false, HintText = "Modifies the wages of garrison by this multiplier (1.0 = Base game wage) (0.5 = 50% base game wage). (Player and AI)")]
	[SettingPropertyGroup("Garrison Wage")]
	public float GarrisonWageReductionMultiplier { get; set; } = 1f;


	[SettingPropertyBool("Enable Garrison Food Reduction", Order = 750, RequireRestart = true, IsToggle = true, HintText = "Enables the modification of garrison food consumption. (Player and AI)")]
	[SettingPropertyGroup("Garrison Food", GroupOrder = 750)]
	public bool EnableGarrisonFoodReduction { get; set; } = false;


	[SettingPropertyFloatingInteger("Garrison Food Multiplier", 0f, 10f, "0.00", Order = 751, RequireRestart = false, HintText = "Modifies the food consumed by a garrison. Base game is 20 men per 1 food. So settings this to 2 would be 40 men per 1 food. Allowing for larger garrison (Player and AI)")]
	[SettingPropertyGroup("Garrison Food")]
	public int GarrisonFoodMuliplier { get; set; } = 2;


	[SettingPropertyBool("Enable Settlement Tax Modifier", Order = 800, RequireRestart = true, IsToggle = true, HintText = "Enables the modification of settlement tax returns (Player and AI)")]
	[SettingPropertyGroup("Settlement Tax", GroupOrder = 800)]
	public bool EnableSettlementTax { get; set; } = false;


	[SettingPropertyFloatingInteger("Settlment Tax Multiplier", 0f, 10f, "0.00", Order = 801, RequireRestart = false, HintText = "Modifies the tax returns for Castle/Towns. (1.0 = Base game taxes) (2 = 2x base game taxes). (Player and AI)")]
	[SettingPropertyGroup("Settlement Tax")]
	public float SettlementTaxMultiplier { get; set; } = 1f;


	[SettingPropertyBool("Enable Speed Buff/Debuff Feature", Order = 850, RequireRestart = true, IsToggle = true, HintText = "Enables the modification of party speed based on friendly/hostile Fortifications (Player and AI)")]
	[SettingPropertyGroup("Party Speed Feature", GroupOrder = 850)]
	public bool EnableSpeed { get; set; } = true;


	[SettingPropertyFloatingInteger("Friendly Fortification Speed Buff", 0f, 10f, "0.00", Order = 851, RequireRestart = true, HintText = "If enabled, parties near a friendly town or castle in a kingdom that are in, will recieve a speed buff")]
	[SettingPropertyGroup("Party Speed Feature")]
	public float SpeedBuffModifier { get; set; } = 0.2f;


	[SettingPropertyFloatingInteger("Hostile Fortification Speed Reduction", -10f, 0f, "0.00", Order = 852, RequireRestart = true, HintText = "If enabled, parties near a hostile town or castle in a kingdom that are in, will recieve a speed reduction")]
	[SettingPropertyGroup("Party Speed Feature")]
	public float SpeedDebuffModifier { get; set; } = -0.2f;


	[SettingPropertyBool("Enable Band Together System", Order = 950, RequireRestart = true, IsToggle = true, HintText = "Reduces troop recruitment costs, party wages, and army influence costs for kingdoms with fewer fiefs. Helps smaller kingdoms compete. (Player and AI)")]
	[SettingPropertyGroup("Band Together System", GroupOrder = 950)]
	public bool EnableBandTogether { get; set; } = false;


	[SettingPropertyBool("Include Player Kingdom", Order = 951, RequireRestart = false, HintText = "If enabled, the Band Together bonuses will also apply to the player's kingdom when they are the ruler.")]
	[SettingPropertyGroup("Band Together System")]
	public bool BandTogetherIncludePlayerKingdom { get; set; } = false;


	[SettingPropertyInteger("Maximum Fiefs for Band Together", 1, 50, "0", Order = 952, RequireRestart = false, HintText = "Maximum number of Towns/Castles a kingdom can have to still receive Band Together bonuses. Default is 5 fiefs.")]
	[SettingPropertyGroup("Band Together System")]
	public int BandTogetherMaxFiefs { get; set; } = 5;


	[SettingPropertyBool("Apply to Recruitment Costs", Order = 953, RequireRestart = false, HintText = "If enabled, reduces troop recruitment costs based on the number of fiefs the kingdom has.")]
	[SettingPropertyGroup("Band Together System")]
	public bool BandTogetherRecruitmentCost { get; set; } = true;


	[SettingPropertyBool("Apply to Party Wages", Order = 954, RequireRestart = false, HintText = "If enabled, reduces party wages based on the number of fiefs the kingdom has.")]
	[SettingPropertyGroup("Band Together System")]
	public bool BandTogetherPartyWages { get; set; } = true;


	[SettingPropertyBool("Apply to Army Influence Cost", Order = 955, RequireRestart = false, HintText = "If enabled, reduces the influence cost to call parties to armies based on the number of fiefs the kingdom has.")]
	[SettingPropertyGroup("Band Together System")]
	public bool BandTogetherArmyInfluence { get; set; } = true;


	[SettingPropertyBool("Enable Settlement Culture Change", Order = 980, RequireRestart = true, HintText = "When enabled, all towns and castles will automatically change their culture to match their owner's kingdom culture. This also updates the culture of notables and bound villages, affecting recruitment and local culture. Runs weekly and applies immediately on ownership change.")]
	[SettingPropertyGroup("Miscellaneous Settings", GroupOrder = 980)]
	public bool EnableCultureChange { get; set; } = false;

}
