using System.Collections.Generic;
using TaleWorlds.Localization;

public static class LanguageTranslater
{
	public static class L
	{
		public static TextObject T(string id, string fallback)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			return new TextObject("{=" + id + "}" + fallback, (Dictionary<string, object>)null);
		}

		public static string S(string id, string fallback)
		{
			return ((object)T(id, fallback)).ToString();
		}
	}
}
