using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace WarAndAITweaks.BattleChatter;

public static class BattleChatterLocalizationLoader
{
	private static readonly Dictionary<string, List<string>> _chatterLines = new Dictionary<string, List<string>>();

	private static bool _isLoaded = false;

	private static string _currentLanguage = "English";

	private static bool _hasLoggedError = false;

	private static readonly object _loadLock = new object();

	public static event Action OnLinesReloaded;

	public static List<string> GetLines(string category, bool includeProfanity = false)
	{
		EnsureLoaded();
		List<string> list = new List<string>();
		try
		{
			if (_chatterLines.TryGetValue(category, out List<string> value))
			{
				list.AddRange(value);
			}
			if (includeProfanity)
			{
				string key = category + "_profane";
				if (_chatterLines.TryGetValue(key, out List<string> value2))
				{
					list.AddRange(value2);
				}
			}
		}
		catch (Exception ex)
		{
			LogError("GetLines failed for category " + category + ": " + ex.Message);
		}
		return list;
	}

	public static void ReloadLines()
	{
		lock (_loadLock)
		{
			try
			{
				_chatterLines.Clear();
				_isLoaded = false;
				_hasLoggedError = false;
				LoadLocalizationFiles();
				BattleChatterLocalizationLoader.OnLinesReloaded?.Invoke();
			}
			catch (Exception ex)
			{
				LogError("ReloadLines failed: " + ex.Message);
			}
		}
	}

	private static void EnsureLoaded()
	{
		if (_isLoaded)
		{
			return;
		}
		lock (_loadLock)
		{
			if (!_isLoaded)
			{
				LoadLocalizationFiles();
				_isLoaded = true;
			}
		}
	}

	private static void LoadLocalizationFiles()
	{
		try
		{
			_currentLanguage = GetCurrentLanguage();
			string localizationPath = GetLocalizationPath(_currentLanguage);
			if (File.Exists(localizationPath))
			{
				LoadFromFile(localizationPath);
				return;
			}
			string localizationPath2 = GetLocalizationPath("English");
			if (File.Exists(localizationPath2))
			{
				LoadFromFile(localizationPath2);
				return;
			}
			string baseLocalizationPath = GetBaseLocalizationPath();
			if (File.Exists(baseLocalizationPath))
			{
				LoadFromFile(baseLocalizationPath);
			}
			else
			{
				Debug.Print("[BattleChatter] No localization files found! Using fallback text.", 0, (DebugColor)12, 17592186044416uL);
			}
		}
		catch (Exception ex)
		{
			LogError("LoadLocalizationFiles failed: " + ex.Message);
		}
	}

	private static string GetCurrentLanguage()
	{
		try
		{
			string languageTitle = LocalizedTextManager.GetLanguageTitle(BannerlordConfig.Language);
			return languageTitle ?? "English";
		}
		catch (Exception ex)
		{
			LogError("GetCurrentLanguage failed: " + ex.Message);
			return "English";
		}
	}

	private static string GetLocalizationPath(string language)
	{
		try
		{
			string path = BasePath.Name + "Modules/WarAndAiTweaks/ModuleData/Languages/";
			string languageCode = GetLanguageCode(language);
			return Path.Combine(path, languageCode, "battlechatter.xml");
		}
		catch (Exception ex)
		{
			LogError("GetLocalizationPath failed: " + ex.Message);
			return "";
		}
	}

	private static string GetBaseLocalizationPath()
	{
		try
		{
			string path = BasePath.Name + "Modules/WarAndAiTweaks/ModuleData/Languages/";
			return Path.Combine(path, "battlechatter.xml");
		}
		catch (Exception ex)
		{
			LogError("GetBaseLocalizationPath failed: " + ex.Message);
			return "";
		}
	}

	private static string GetLanguageCode(string language)
	{
		if (1 == 0)
		{
		}
		string result = language switch
		{
			"English" => "", 
			"Deutsch" => "DE", 
			"Español" => "SP", 
			"Français" => "FR", 
			"Italiano" => "IT", 
			"Polski" => "PL", 
			"Português (Brasil)" => "BR", 
			"Русский" => "RU", 
			"Türkçe" => "TR", 
			"繁體中文" => "CNt", 
			"简体中文" => "CNs", 
			"한국어" => "KO", 
			"日本語" => "JP", 
			_ => "", 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	private static void LoadFromFile(string filePath)
	{
		try
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
			{
				LogError("File not found: " + filePath);
				return;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filePath);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("//string");
			if (xmlNodeList == null || xmlNodeList.Count == 0)
			{
				Debug.Print("[BattleChatter] No string nodes found in " + filePath, 0, (DebugColor)12, 17592186044416uL);
				return;
			}
			int num = 0;
			foreach (XmlNode item in xmlNodeList)
			{
				if (++num > 10000)
				{
					LogError($"Exceeded max nodes to process ({10000}) in {filePath}");
					break;
				}
				XmlAttribute xmlAttribute = item.Attributes?["id"];
				XmlAttribute xmlAttribute2 = item.Attributes?["text"];
				if (xmlAttribute != null && xmlAttribute.Value != null && xmlAttribute2?.Value != null)
				{
					string value = xmlAttribute.Value;
					string value2 = xmlAttribute2.Value;
					string key = ExtractCategory(value);
					if (!_chatterLines.ContainsKey(key))
					{
						_chatterLines[key] = new List<string>();
					}
					_chatterLines[key].Add(value2);
				}
			}
			Debug.Print($"[BattleChatter] Loaded {num} lines from {filePath}", 0, (DebugColor)12, 17592186044416uL);
		}
		catch (Exception ex)
		{
			LogError("LoadFromFile failed for " + filePath + ": " + ex.Message);
			throw;
		}
	}

	private static string ExtractCategory(string id)
	{
		try
		{
			if (string.IsNullOrEmpty(id))
			{
				return "";
			}
			for (int num = id.Length - 1; num >= 0; num--)
			{
				if (id[num] == '_' && num < id.Length - 1)
				{
					string s = id.Substring(num + 1);
					if (int.TryParse(s, out var _))
					{
						return id.Substring(0, num);
					}
				}
			}
			return id;
		}
		catch (Exception ex)
		{
			LogError("ExtractCategory failed for " + id + ": " + ex.Message);
			return id;
		}
	}

	public static string GetRandomLine(string category, bool includeProfanity = false)
	{
		try
		{
			List<string> lines = GetLines(category, includeProfanity);
			if (lines.Count == 0)
			{
				return "";
			}
			return lines[MBRandom.RandomInt(lines.Count)];
		}
		catch (Exception ex)
		{
			LogError("GetRandomLine failed for category " + category + ": " + ex.Message);
			return "";
		}
	}

	public static bool HasLines(string category, bool includeProfanity = false)
	{
		try
		{
			return GetLines(category, includeProfanity).Count > 0;
		}
		catch (Exception ex)
		{
			LogError("HasLines failed for category " + category + ": " + ex.Message);
			return false;
		}
	}

	private static void LogError(string message)
	{
		if (!_hasLoggedError)
		{
			Debug.Print("[BattleChatter] " + message, 0, (DebugColor)12, 17592186044416uL);
			_hasLoggedError = true;
		}
	}
}
