using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LauncherCoreLib.Models.EventLogging;

public static class PropertiesBuilderExtensions
{
	public static IImmutableDictionary<AppCenterPropertyName, AppCenterPropertyValue> FillEmptySlots(this IImmutableDictionary<AppCenterPropertyName, AppCenterPropertyValue> properties, string simPrefix, string simFamilyId, int maxSimCount)
	{
		if (maxSimCount == 0)
		{
			return properties.Add(AppCenterPropertyName.ParseTruncateAndHandleErrors($"{simPrefix}{1}"), AppCenterPropertyValue.ParseTruncateAndHandleErrors("<Empty_" + simFamilyId + ">"));
		}
		for (int i = 1; i <= maxSimCount; i++)
		{
			AppCenterPropertyName appCenterPropertyName = AppCenterPropertyName.ParseTruncateAndHandleErrors($"{simPrefix}{i}");
			if (!((IReadOnlyDictionary<AppCenterPropertyName, AppCenterPropertyValue>)(object)properties).ContainsKey(appCenterPropertyName))
			{
				properties = properties.Add(appCenterPropertyName, AppCenterPropertyValue.ParseTruncateAndHandleErrors("<Empty_" + simFamilyId + ">"));
			}
		}
		return properties;
	}

	public static string ToUnderscoreCase(this string str)
	{
		return Enumerable.Aggregate<string>(Enumerable.Select<char, string>((System.Collections.Generic.IEnumerable<char>)str, (Func<char, int, string>)((char c, int i) => (i > 0 && char.IsUpper(c)) ? $"_{c}" : c.ToString())), (Func<string, string, string>)((string s1, string s2) => s1 + s2));
	}

	public static ImmutableList<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>> AddNewKvp(this ImmutableList<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>> list, AppCenterPropertyName propName, AppCenterPropertyValue propVal)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return list.Add(new KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>(propName, propVal));
	}
}
