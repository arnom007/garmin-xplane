using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LOLA.Logging;

namespace LauncherCoreLib.Models.AbstractUi;

public static class BaseUiAliases
{
	public static System.Collections.Generic.IList<UiAlias> RegisterAllStaticUiActions(IRootUiAliasRegistry rootRegistry, System.Type staticClassType)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (rootRegistry == null)
		{
			throw new ArgumentNullException("rootRegistry");
		}
		if (staticClassType == (System.Type)null)
		{
			throw new ArgumentNullException("staticClassType");
		}
		System.Type uiAliasType = typeof(UiAlias);
		Logger.Log(LogLevel.Info, "Registering the {0}'s from {1}", ((MemberInfo)uiAliasType).Name, staticClassType.FullName);
		List<FieldInfo> val = Enumerable.ToList<FieldInfo>(Enumerable.Where<FieldInfo>((System.Collections.Generic.IEnumerable<FieldInfo>)staticClassType.GetFields((BindingFlags)24), (Func<FieldInfo, bool>)((FieldInfo f) => f.FieldType == uiAliasType && f.IsInitOnly)));
		if (val.Count == 0)
		{
			Logger.Log(LogLevel.Warning, "The {0} class did not define any public static {1} fields.", staticClassType, ((MemberInfo)uiAliasType).Name);
		}
		Dictionary<FieldInfo, UiAlias> val2 = Enumerable.ToDictionary<FieldInfo, FieldInfo, UiAlias>((System.Collections.Generic.IEnumerable<FieldInfo>)val, (Func<FieldInfo, FieldInfo>)((FieldInfo fi) => fi), (Func<FieldInfo, UiAlias>)((FieldInfo fi) => (UiAlias)fi.GetValue((object)null)));
		System.Collections.Generic.IEnumerator<KeyValuePair<FieldInfo, UiAlias>> enumerator = Enumerable.Where<KeyValuePair<FieldInfo, UiAlias>>((System.Collections.Generic.IEnumerable<KeyValuePair<FieldInfo, UiAlias>>)val2, (Func<KeyValuePair<FieldInfo, UiAlias>, bool>)((KeyValuePair<FieldInfo, UiAlias> fr) => UiAlias.IsValid(fr.Value))).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				rootRegistry.AddorUpdateAlias(enumerator.Current.Value);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		List<KeyValuePair<FieldInfo, UiAlias>> val3 = Enumerable.ToList<KeyValuePair<FieldInfo, UiAlias>>(Enumerable.Where<KeyValuePair<FieldInfo, UiAlias>>((System.Collections.Generic.IEnumerable<KeyValuePair<FieldInfo, UiAlias>>)val2, (Func<KeyValuePair<FieldInfo, UiAlias>, bool>)((KeyValuePair<FieldInfo, UiAlias> fr) => !UiAlias.IsValid(fr.Value))));
		if (val3.Count > 0)
		{
			Logger.Log(LogLevel.Error, "These {0} fields in {1} are invalid and should be given a value to be registered: [{2}]", ((MemberInfo)uiAliasType).Name, staticClassType, string.Join(", ", Enumerable.Select<KeyValuePair<FieldInfo, UiAlias>, string>((System.Collections.Generic.IEnumerable<KeyValuePair<FieldInfo, UiAlias>>)val3, (Func<KeyValuePair<FieldInfo, UiAlias>, string>)((KeyValuePair<FieldInfo, UiAlias> i) => ((MemberInfo)i.Key).Name))));
		}
		List<FieldInfo> val4 = Enumerable.ToList<FieldInfo>(Enumerable.Where<FieldInfo>((System.Collections.Generic.IEnumerable<FieldInfo>)Enumerable.ToList<FieldInfo>(Enumerable.Where<FieldInfo>((System.Collections.Generic.IEnumerable<FieldInfo>)staticClassType.GetFields((BindingFlags)60), (Func<FieldInfo, bool>)((FieldInfo fi) => fi.FieldType == uiAliasType))), (Func<FieldInfo, bool>)((FieldInfo fi) => fi.IsPrivate || !fi.IsStatic || !fi.IsInitOnly)));
		if (val4.Count > 0)
		{
			Logger.Log(LogLevel.Error, "These {0} fields in {1} are either not public, or not static, or not readonly: [{2}] Which defeats the strong typing advantage that this function enables.", ((MemberInfo)uiAliasType).Name, staticClassType, string.Join(", ", Enumerable.Select<FieldInfo, string>((System.Collections.Generic.IEnumerable<FieldInfo>)val4, (Func<FieldInfo, string>)((FieldInfo i) => ((MemberInfo)i).Name))));
		}
		return (System.Collections.Generic.IList<UiAlias>)Enumerable.ToList<UiAlias>(Enumerable.Where<UiAlias>((System.Collections.Generic.IEnumerable<UiAlias>)val2.Values, (Func<UiAlias, bool>)UiAlias.IsValid));
	}
}
