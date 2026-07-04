using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using WarAndAITweaks.Utils;

namespace WarAndAITweaks.War_Peace_AI_Overhaul;

internal class DiplomacyPlugin
{
	[CompilerGenerated]
	private sealed class _003CAllRelatedHeroes_003Ed__24 : IEnumerable<Hero>, IEnumerable, IEnumerator<Hero>, IDisposable, IEnumerator
	{
		private int _003C_003E1__state;

		private Hero _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private Hero inHero;

		public Hero _003C_003E3__inHero;

		private bool includeExSpouses;

		public bool _003C_003E3__includeExSpouses;

		private IEnumerator<Hero> _003C_003Es__1;

		private Hero _003Chero_003E5__2;

		private IEnumerator<Hero> _003C_003Es__3;

		private Hero _003Chero_003E5__4;

		private List<Hero>.Enumerator _003C_003Es__5;

		private Hero _003Chero_003E5__6;

		private IEnumerator<Hero> _003C_003Es__7;

		private Hero _003Chero_003E5__8;

		private List<Hero>.Enumerator _003C_003Es__9;

		private Hero _003Chero_003E5__10;

		Hero IEnumerator<Hero>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CAllRelatedHeroes_003Ed__24(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			switch (_003C_003E1__state)
			{
			case -3:
			case 2:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
				break;
			case -4:
			case 4:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally2();
				}
				break;
			case -5:
			case 6:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally3();
				}
				break;
			case -6:
			case 7:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally4();
				}
				break;
			case -7:
			case 8:
				try
				{
				}
				finally
				{
					_003C_003Em__Finally5();
				}
				break;
			}
			_003C_003Es__1 = null;
			_003Chero_003E5__2 = null;
			_003C_003Es__3 = null;
			_003Chero_003E5__4 = null;
			_003C_003Es__5 = default(List<Hero>.Enumerator);
			_003Chero_003E5__6 = null;
			_003C_003Es__7 = null;
			_003Chero_003E5__8 = null;
			_003C_003Es__9 = default(List<Hero>.Enumerator);
			_003Chero_003E5__10 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					if (inHero == null)
					{
						return false;
					}
					if (inHero.Father != null)
					{
						_003C_003E2__current = inHero.Father;
						_003C_003E1__state = 1;
						return true;
					}
					goto IL_0135;
				case 1:
					_003C_003E1__state = -1;
					_003C_003Es__1 = inHero.Father.Siblings.GetEnumerator();
					_003C_003E1__state = -3;
					goto IL_0119;
				case 2:
					_003C_003E1__state = -3;
					_003Chero_003E5__2 = null;
					goto IL_0119;
				case 3:
					_003C_003E1__state = -1;
					_003C_003Es__3 = inHero.Mother.Siblings.GetEnumerator();
					_003C_003E1__state = -4;
					goto IL_01d3;
				case 4:
					_003C_003E1__state = -4;
					_003Chero_003E5__4 = null;
					goto IL_01d3;
				case 5:
					_003C_003E1__state = -1;
					goto IL_022b;
				case 6:
					_003C_003E1__state = -5;
					_003Chero_003E5__6 = null;
					goto IL_0288;
				case 7:
					_003C_003E1__state = -6;
					_003Chero_003E5__8 = null;
					goto IL_0305;
				case 8:
					{
						_003C_003E1__state = -7;
						_003Chero_003E5__10 = null;
						goto IL_0387;
					}
					IL_01d3:
					if (_003C_003Es__3.MoveNext())
					{
						_003Chero_003E5__4 = _003C_003Es__3.Current;
						_003C_003E2__current = _003Chero_003E5__4;
						_003C_003E1__state = 4;
						return true;
					}
					_003C_003Em__Finally2();
					_003C_003Es__3 = null;
					goto IL_01ef;
					IL_0135:
					if (inHero.Mother != null)
					{
						_003C_003E2__current = inHero.Mother;
						_003C_003E1__state = 3;
						return true;
					}
					goto IL_01ef;
					IL_0305:
					if (_003C_003Es__7.MoveNext())
					{
						_003Chero_003E5__8 = _003C_003Es__7.Current;
						_003C_003E2__current = _003Chero_003E5__8;
						_003C_003E1__state = 7;
						return true;
					}
					_003C_003Em__Finally4();
					_003C_003Es__7 = null;
					if (!includeExSpouses)
					{
						break;
					}
					_003C_003Es__9 = ((List<Hero>)(object)inHero.ExSpouses).GetEnumerator();
					_003C_003E1__state = -7;
					goto IL_0387;
					IL_0119:
					if (_003C_003Es__1.MoveNext())
					{
						_003Chero_003E5__2 = _003C_003Es__1.Current;
						_003C_003E2__current = _003Chero_003E5__2;
						_003C_003E1__state = 2;
						return true;
					}
					_003C_003Em__Finally1();
					_003C_003Es__1 = null;
					goto IL_0135;
					IL_01ef:
					if (inHero.Spouse != null)
					{
						_003C_003E2__current = inHero.Spouse;
						_003C_003E1__state = 5;
						return true;
					}
					goto IL_022b;
					IL_0387:
					if (_003C_003Es__9.MoveNext())
					{
						_003Chero_003E5__10 = _003C_003Es__9.Current;
						_003C_003E2__current = _003Chero_003E5__10;
						_003C_003E1__state = 8;
						return true;
					}
					_003C_003Em__Finally5();
					_003C_003Es__9 = default(List<Hero>.Enumerator);
					break;
					IL_022b:
					_003C_003Es__5 = ((List<Hero>)(object)inHero.Children).GetEnumerator();
					_003C_003E1__state = -5;
					goto IL_0288;
					IL_0288:
					if (_003C_003Es__5.MoveNext())
					{
						_003Chero_003E5__6 = _003C_003Es__5.Current;
						_003C_003E2__current = _003Chero_003E5__6;
						_003C_003E1__state = 6;
						return true;
					}
					_003C_003Em__Finally3();
					_003C_003Es__5 = default(List<Hero>.Enumerator);
					_003C_003Es__7 = inHero.Siblings.GetEnumerator();
					_003C_003E1__state = -6;
					goto IL_0305;
				}
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003Es__1 != null)
			{
				_003C_003Es__1.Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -1;
			if (_003C_003Es__3 != null)
			{
				_003C_003Es__3.Dispose();
			}
		}

		private void _003C_003Em__Finally3()
		{
			_003C_003E1__state = -1;
			((IDisposable)_003C_003Es__5).Dispose();
		}

		private void _003C_003Em__Finally4()
		{
			_003C_003E1__state = -1;
			if (_003C_003Es__7 != null)
			{
				_003C_003Es__7.Dispose();
			}
		}

		private void _003C_003Em__Finally5()
		{
			_003C_003E1__state = -1;
			((IDisposable)_003C_003Es__9).Dispose();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<Hero> IEnumerable<Hero>.GetEnumerator()
		{
			_003CAllRelatedHeroes_003Ed__24 _003CAllRelatedHeroes_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CAllRelatedHeroes_003Ed__ = this;
			}
			else
			{
				_003CAllRelatedHeroes_003Ed__ = new _003CAllRelatedHeroes_003Ed__24(0);
			}
			_003CAllRelatedHeroes_003Ed__.inHero = _003C_003E3__inHero;
			_003CAllRelatedHeroes_003Ed__.includeExSpouses = _003C_003E3__includeExSpouses;
			return _003CAllRelatedHeroes_003Ed__;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Hero>)this).GetEnumerator();
		}
	}

	private static Type _cachedManagerType = null;

	private static Type _cachedSettingsType = null;

	private static Type _cachedGlobalSettingsGenericType = null;

	private static object _cachedSettingsInstance = null;

	private static List<DiplomaticAgreementInfo> _cachedAgreements = new List<DiplomaticAgreementInfo>();

	private static CampaignTime _lastAgreementsCacheUpdate = CampaignTime.Zero;

	private static readonly float AGREEMENTS_CACHE_DAYS = 1f;

	public static bool IsDiplomacyModLoaded()
	{
		try
		{
			if (_cachedSettingsType != null)
			{
				return true;
			}
			Type type = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				type = assembly.GetType("Diplomacy.Settings");
				if (type != null)
				{
					_cachedSettingsType = type;
					InformationManager.DisplayMessage(new InformationMessage("W&AI has detected the Diplomacy mod.", Colors.Yellow));
					return true;
				}
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public static List<DiplomaticAgreementInfo> GetAllDiplomaticAgreements()
	{
		_ = CampaignTime.Now;
		_ = _lastAgreementsCacheUpdate;
		CampaignTime now = CampaignTime.Now;
		if (now.ToDays - _lastAgreementsCacheUpdate.ToDays < (double)AGREEMENTS_CACHE_DAYS)
		{
			return _cachedAgreements;
		}
		List<DiplomaticAgreementInfo> list = new List<DiplomaticAgreementInfo>();
		try
		{
			Type diplomaticAgreementManagerType = GetDiplomaticAgreementManagerType();
			if (diplomaticAgreementManagerType == null)
			{
				return list;
			}
			object managerInstance = GetManagerInstance(diplomaticAgreementManagerType);
			if (managerInstance == null)
			{
				return list;
			}
			object propertyValue = Reflector.GetPropertyValue<object>(managerInstance, "Agreements");
			if (propertyValue == null)
			{
				return list;
			}
			ProcessAgreementsDictionary(propertyValue, list);
		}
		catch (Exception)
		{
			list.Clear();
		}
		_cachedAgreements = list;
		_ = CampaignTime.Now;
		if (true)
		{
			_lastAgreementsCacheUpdate = CampaignTime.Now;
		}
		return list;
	}

	private static Type GetDiplomaticAgreementManagerType()
	{
		if (_cachedManagerType != null)
		{
			return _cachedManagerType;
		}
		Type type = Type.GetType("Diplomacy.DiplomaticAction.DiplomaticAgreementManager, Diplomacy");
		if (type == null)
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				type = assembly.GetType("Diplomacy.DiplomaticAction.DiplomaticAgreementManager");
				if (type != null)
				{
					break;
				}
			}
		}
		_cachedManagerType = type;
		return type;
	}

	private static object GetManagerInstance(Type managerType)
	{
		try
		{
			object instance = Activator.CreateInstance(managerType, nonPublic: true);
			return Reflector.GetPropertyValue<object>(instance, "Instance");
		}
		catch
		{
			return managerType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
		}
	}

	private static void ProcessAgreementsDictionary(object agreementsDict, List<DiplomaticAgreementInfo> result)
	{
		Type type = agreementsDict.GetType();
		try
		{
			if (!(Reflector.InvokeMethod(agreementsDict, "get_Keys", null) is IEnumerable enumerable))
			{
				return;
			}
			foreach (object item in enumerable)
			{
				try
				{
					if (Reflector.InvokeMethod(agreementsDict, "get_Item", null, item) is IEnumerable agreementsList)
					{
						var (kingdom, kingdom2) = ExtractKingdomsFromFactionPair(item);
						ProcessAgreementsList(agreementsList, kingdom, kingdom2, result);
					}
				}
				catch (Exception)
				{
				}
			}
		}
		catch (Exception)
		{
			if (!(type.GetMethod("get_Keys") != null) || !(type.GetMethod("get_Item") != null))
			{
				return;
			}
			IEnumerable enumerable2 = type.GetMethod("get_Keys").Invoke(agreementsDict, null) as IEnumerable;
			MethodInfo method = type.GetMethod("get_Item");
			if (enumerable2 == null || !(method != null))
			{
				return;
			}
			foreach (object item2 in enumerable2)
			{
				try
				{
					if (method.Invoke(agreementsDict, new object[1] { item2 }) is IEnumerable agreementsList2)
					{
						var (kingdom3, kingdom4) = ExtractKingdomsFromFactionPair(item2);
						ProcessAgreementsList(agreementsList2, kingdom3, kingdom4, result);
					}
				}
				catch (Exception)
				{
				}
			}
		}
	}

	private static (Kingdom kingdom1, Kingdom kingdom2) ExtractKingdomsFromFactionPair(object factionPair)
	{
		Kingdom item = null;
		Kingdom item2 = null;
		try
		{
			object propertyValue = Reflector.GetPropertyValue<object>(factionPair, "Faction1");
			object propertyValue2 = Reflector.GetPropertyValue<object>(factionPair, "Faction2");
			item = (Kingdom)((propertyValue is Kingdom) ? propertyValue : null);
			item2 = (Kingdom)((propertyValue2 is Kingdom) ? propertyValue2 : null);
		}
		catch (Exception)
		{
		}
		return (kingdom1: item, kingdom2: item2);
	}

	private static void ProcessAgreementsList(IEnumerable agreementsList, Kingdom kingdom1, Kingdom kingdom2, List<DiplomaticAgreementInfo> result)
	{
		foreach (object agreements in agreementsList)
		{
			if (agreements != null)
			{
				try
				{
					DiplomaticAgreementInfo item = new DiplomaticAgreementInfo
					{
						Agreement = agreements,
						Kingdom1 = kingdom1,
						Kingdom2 = kingdom2,
						Kingdom1Name = (((kingdom1 == null) ? null : ((object)kingdom1.Name)?.ToString()) ?? "Unknown"),
						Kingdom2Name = (((kingdom2 == null) ? null : ((object)kingdom2.Name)?.ToString()) ?? "Unknown"),
						AgreementType = GetAgreementType(agreements),
						IsExpired = GetAgreementExpiredStatus(agreements),
						Description = BuildAgreementDescription(agreements, kingdom1, kingdom2)
					};
					result.Add(item);
				}
				catch (Exception)
				{
				}
			}
		}
	}

	private static string GetAgreementType(object agreement)
	{
		try
		{
			return Reflector.InvokeMethod(agreement, "GetAgreementType", null)?.ToString() ?? "Unknown";
		}
		catch (Exception)
		{
			return agreement.GetType().Name;
		}
	}

	private static bool GetAgreementExpiredStatus(object agreement)
	{
		try
		{
			if (Reflector.InvokeMethod(agreement, "IsExpired", null) is bool result)
			{
				return result;
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	private static string BuildAgreementDescription(object agreement, Kingdom kingdom1, Kingdom kingdom2)
	{
		string agreementType = GetAgreementType(agreement);
		string text = ((kingdom1 == null) ? null : ((object)kingdom1.Name)?.ToString()) ?? "Unknown";
		string text2 = ((kingdom2 == null) ? null : ((object)kingdom2.Name)?.ToString()) ?? "Unknown";
		string text3 = (GetAgreementExpiredStatus(agreement) ? " (Expired)" : "");
		return agreementType + " between " + text + " and " + text2 + text3;
	}

	public static bool HasAgreement(Kingdom kingdom1, Kingdom kingdom2)
	{
		List<DiplomaticAgreementInfo> allDiplomaticAgreements = GetAllDiplomaticAgreements();
		foreach (DiplomaticAgreementInfo item in allDiplomaticAgreements)
		{
			if ((item.Kingdom1 == kingdom1 && item.Kingdom2 == kingdom2) || (item.Kingdom1 == kingdom2 && item.Kingdom2 == kingdom1))
			{
				return true;
			}
		}
		return false;
	}

	private static object GetCachedSettingsInstance()
	{
		if (_cachedSettingsInstance != null)
		{
			return _cachedSettingsInstance;
		}
		try
		{
			if (_cachedSettingsType == null)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly in assemblies)
				{
					_cachedSettingsType = assembly.GetType("Diplomacy.Settings");
					if (_cachedSettingsType != null)
					{
						break;
					}
				}
			}
			if (_cachedSettingsType == null)
			{
				return null;
			}
			if (_cachedGlobalSettingsGenericType == null)
			{
				Assembly[] assemblies2 = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly2 in assemblies2)
				{
					_cachedGlobalSettingsGenericType = assembly2.GetType("MCM.Abstractions.Base.Global.GlobalSettings`1");
					if (_cachedGlobalSettingsGenericType != null)
					{
						break;
					}
				}
			}
			if (_cachedGlobalSettingsGenericType == null)
			{
				return null;
			}
			Type type = _cachedGlobalSettingsGenericType.MakeGenericType(_cachedSettingsType);
			try
			{
				_cachedSettingsInstance = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
			}
			catch
			{
				_cachedSettingsInstance = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
			}
			return _cachedSettingsInstance;
		}
		catch
		{
			return null;
		}
	}

	public static bool? GetNoWarOnGoodRelations()
	{
		try
		{
			object cachedSettingsInstance = GetCachedSettingsInstance();
			if (cachedSettingsInstance == null)
			{
				return null;
			}
			return Reflector.GetPropertyValue<bool?>(cachedSettingsInstance, "NoWarOnGoodRelations");
		}
		catch
		{
			return null;
		}
	}

	public static int? GetNoWarOnGoodRelationsThreshold()
	{
		try
		{
			object cachedSettingsInstance = GetCachedSettingsInstance();
			if (cachedSettingsInstance == null)
			{
				return null;
			}
			return Reflector.GetPropertyValue<int?>(cachedSettingsInstance, "NoWarOnGoodRelationsThreshold");
		}
		catch
		{
			return null;
		}
	}

	public static bool? GetNoWarBetweenFriends()
	{
		try
		{
			object cachedSettingsInstance = GetCachedSettingsInstance();
			if (cachedSettingsInstance == null)
			{
				return null;
			}
			return Reflector.GetPropertyValue<bool?>(cachedSettingsInstance, "NoWarBetweenFriends");
		}
		catch
		{
			return null;
		}
	}

	public static bool? GetNoWarWhenMarriedLeaderClans()
	{
		try
		{
			object cachedSettingsInstance = GetCachedSettingsInstance();
			if (cachedSettingsInstance == null)
			{
				return null;
			}
			return Reflector.GetPropertyValue<bool?>(cachedSettingsInstance, "NoWarWhenMarriedLeaderClans");
		}
		catch
		{
			return null;
		}
	}

	public static bool HasMarriedClanLeaderRelation(Clan clan, Clan other)
	{
		Clan other2 = other;
		if (clan == null || other2 == null || clan.Leader == null)
		{
			return false;
		}
		Hero spouse = clan.Leader.Spouse;
		if (spouse == null)
		{
			foreach (Hero item in AllRelatedHeroes(clan.Leader))
			{
				if (item.IsAlive)
				{
					Hero spouse2 = item.Spouse;
					if (spouse2 != null && AllRelatedHeroes(spouse2).Any((Hero spouseFamMember) => spouseFamMember.Clan == other2))
					{
						return true;
					}
				}
			}
			return false;
		}
		return AllRelatedHeroes(spouse).Any((Hero ownSpouseFamMember) => ownSpouseFamMember.Clan == other2);
	}

	[IteratorStateMachine(typeof(_003CAllRelatedHeroes_003Ed__24))]
	public static IEnumerable<Hero> AllRelatedHeroes(Hero inHero, bool includeExSpouses = false)
	{
		//yield-return decompiler failed: Unexpected instruction in Iterator.Dispose()
		return new _003CAllRelatedHeroes_003Ed__24(-2)
		{
			_003C_003E3__inHero = inHero,
			_003C_003E3__includeExSpouses = includeExSpouses
		};
	}
}


