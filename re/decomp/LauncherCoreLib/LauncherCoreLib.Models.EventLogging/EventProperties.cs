using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public static class EventProperties
{
	[field: CompilerGenerated]
	public static PropertiesBuilder None
	{
		[CompilerGenerated]
		get;
	} = FromEnumerable(Enumerable.Empty<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>());


	public static PropertiesBuilder FromEnumerable(System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>> properties)
	{
		System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>> properties2 = properties;
		return () => properties2;
	}

	public static PropertiesBuilder FromEnumerable(System.Collections.Generic.IEnumerable<ValueTuple<AppCenterPropertyName, AppCenterPropertyValue>> properties)
	{
		return FromEnumerable(Enumerable.Select<ValueTuple<AppCenterPropertyName, AppCenterPropertyValue>, KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>(properties, (Func<ValueTuple<AppCenterPropertyName, AppCenterPropertyValue>, KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)((ValueTuple<AppCenterPropertyName, AppCenterPropertyValue> t) => KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(t.Item1, t.Item2))));
	}

	public static PropertiesBuilder StaticList(params ValueTuple<AppCenterPropertyName, AppCenterPropertyValue>[] properties)
	{
		return FromEnumerable(properties);
	}
}
