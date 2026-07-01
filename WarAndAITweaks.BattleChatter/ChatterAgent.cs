using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace WarAndAITweaks.BattleChatter;

public class ChatterAgent
{
	public CharacterObject CharacterObject { get; set; }

	public Agent Agent { get; set; }

	public bool IsMale
	{
		get
		{
			CharacterObject characterObject = CharacterObject;
			return characterObject != null && !((BasicCharacterObject)characterObject).IsFemale;
		}
	}

	public bool IsFemale
	{
		get
		{
			CharacterObject characterObject = CharacterObject;
			return characterObject != null && ((BasicCharacterObject)characterObject).IsFemale;
		}
	}
}
