using System.Collections.Generic;
using TaleWorlds.Localization;

public static class LanguageTranslater
{
	public static class L
	{
		public static TextObject T(string id, string fallback)
		{
			return new TextObject("{=" + id + "}" + fallback, (Dictionary<string, object>)null);
		}

		public static string S(string id, string fallback)
		{
			return ((object)T(id, fallback)).ToString();
		}
	}
}
