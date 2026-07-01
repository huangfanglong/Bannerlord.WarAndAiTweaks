using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FeastSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;

namespace WarAndAITweaks.TodayWeFeast;

[HarmonyPatch(typeof(Location), "AddCharacter", new Type[] { typeof(LocationCharacter) })]
internal static class Feast_Location_AddCharacter_Patch
{
	private static bool Prefix(Location __instance, LocationCharacter locationCharacter)
	{
		try
		{
			if (__instance == null || locationCharacter == null)
			{
				return true;
			}
			Type type = Type.GetType("TaleWorlds.CampaignSystem.CampaignBehaviors.HeroAgentSpawnCampaignBehavior, TaleWorlds.CampaignSystem");
			if (type == null)
			{
				return true;
			}
			object obj = type.GetProperty("LordsHall", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
			Location val = (Location)((obj is Location) ? obj : null);
			if (val == null)
			{
				return true;
			}
			if (__instance != val)
			{
				return true;
			}
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			Settlement settlement = ((locationEncounter != null) ? locationEncounter.Settlement : null);
			if (settlement == null)
			{
				return true;
			}
			FeastData feastByAttribute = FeastBehavior.GetFeastByAttribute((FeastData f) => f != null && f.Settlement == settlement);
			if (feastByAttribute == null)
			{
				return true;
			}
			Hero val2 = TryExtractHeroFromLocationCharacter(locationCharacter);
			if (val2 == null)
			{
				return true;
			}
			Hero mainHero = Hero.MainHero;
			Hero val3 = ((mainHero != null) ? mainHero.Spouse : null);
			return val2 == mainHero || (feastByAttribute.Host != null && val2 == feastByAttribute.Host) || (feastByAttribute.Attendees != null && ((List<Hero>)(object)feastByAttribute.Attendees).Contains(val2)) || (val3 != null && val2 == val3);
		}
		catch
		{
			return true;
		}
	}

	private static Hero TryExtractHeroFromLocationCharacter(LocationCharacter locChar)
	{
		if (locChar == null)
		{
			return null;
		}
		Type type = ((object)locChar).GetType();
		string[] array = new string[7] { "Character", "AgentData", "AgentOrigin", "Agent", "CharacterObject", "CharacterObj", "locationCharacter" };
		string[] array2 = array;
		foreach (string name in array2)
		{
			PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property != null && property.GetIndexParameters().Length == 0)
			{
				try
				{
					object value = property.GetValue(locChar);
					Hero val = TryExtractHeroFromObject(value);
					if (val != null)
					{
						return val;
					}
				}
				catch
				{
				}
			}
			FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (!(field != null))
			{
				continue;
			}
			try
			{
				object value2 = field.GetValue(locChar);
				Hero val2 = TryExtractHeroFromObject(value2);
				if (val2 != null)
				{
					return val2;
				}
			}
			catch
			{
			}
		}
		MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MemberInfo memberInfo in members)
		{
			try
			{
				object obj3 = null;
				if (memberInfo is FieldInfo fieldInfo)
				{
					obj3 = fieldInfo.GetValue(locChar);
				}
				else if (memberInfo is PropertyInfo propertyInfo && propertyInfo.GetIndexParameters().Length == 0)
				{
					obj3 = propertyInfo.GetValue(locChar);
				}
				Hero val3 = TryExtractHeroFromObject(obj3);
				if (val3 != null)
				{
					return val3;
				}
			}
			catch
			{
			}
		}
		return null;
	}

	private static Hero TryExtractHeroFromObject(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		Hero val = (Hero)((obj is Hero) ? obj : null);
		if (val != null)
		{
			return val;
		}
		CharacterObject val2 = (CharacterObject)((obj is CharacterObject) ? obj : null);
		if (val2 != null)
		{
			return val2.HeroObject;
		}
		Type type = obj.GetType();
		MemberInfo memberInfo = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(delegate(MemberInfo m)
		{
			Type type2 = null;
			if (m is FieldInfo fieldInfo3)
			{
				type2 = fieldInfo3.FieldType;
			}
			else if (m is PropertyInfo propertyInfo3 && propertyInfo3.GetIndexParameters().Length == 0)
			{
				type2 = propertyInfo3.PropertyType;
			}
			return !(type2 == null) && (type2 == typeof(CharacterObject) || type2 == typeof(Hero));
		});
		if (memberInfo != null)
		{
			try
			{
				object obj2 = null;
				if (memberInfo is FieldInfo fieldInfo)
				{
					obj2 = fieldInfo.GetValue(obj);
				}
				else if (memberInfo is PropertyInfo propertyInfo)
				{
					obj2 = propertyInfo.GetValue(obj);
				}
				Hero val3 = (Hero)((obj2 is Hero) ? obj2 : null);
				if (val3 != null)
				{
					return val3;
				}
				CharacterObject val4 = (CharacterObject)((obj2 is CharacterObject) ? obj2 : null);
				if (val4 != null)
				{
					return val4.HeroObject;
				}
			}
			catch
			{
			}
		}
		MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MemberInfo memberInfo2 in members)
		{
			try
			{
				object obj4 = null;
				if (memberInfo2 is FieldInfo fieldInfo2)
				{
					obj4 = fieldInfo2.GetValue(obj);
				}
				else if (memberInfo2 is PropertyInfo propertyInfo2 && propertyInfo2.GetIndexParameters().Length == 0)
				{
					obj4 = propertyInfo2.GetValue(obj);
				}
				Hero val5 = (Hero)((obj4 is Hero) ? obj4 : null);
				if (val5 != null)
				{
					return val5;
				}
				CharacterObject val6 = (CharacterObject)((obj4 is CharacterObject) ? obj4 : null);
				if (val6 != null)
				{
					return val6.HeroObject;
				}
			}
			catch
			{
			}
		}
		return null;
	}
}
