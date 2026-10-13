using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt;

namespace LauncherCoreLib.Models.EventLogging;

public static class EventUserDataCensor
{
	[field: CompilerGenerated]
	public static ShouldCensorUserDataInValueForProperty All
	{
		[CompilerGenerated]
		get;
	} = (AppCenterPropertyName _) => true;


	[field: CompilerGenerated]
	public static ShouldCensorUserDataInValueForProperty None
	{
		[CompilerGenerated]
		get;
	} = (AppCenterPropertyName _) => false;


	public static ShouldCensorUserDataInValueForProperty CensorProperties(AppCenterPropertyName first, params AppCenterPropertyName[] extras)
	{
		return CensorProperties((System.Collections.Generic.IEnumerable<AppCenterPropertyName>)Enumerable.ToHashSet<AppCenterPropertyName>(new AppCenterPropertyName[1] { first }.ConcatFast<AppCenterPropertyName>(extras)));
	}

	public static ShouldCensorUserDataInValueForProperty CensorProperties(System.Collections.Generic.IEnumerable<AppCenterPropertyName> propertiesToCensor)
	{
		List<AppCenterPropertyName> allProps = Enumerable.ToList<AppCenterPropertyName>(propertiesToCensor);
		if (allProps.Count > 5)
		{
			HashSet<AppCenterPropertyName> hash = Enumerable.ToHashSet<AppCenterPropertyName>((System.Collections.Generic.IEnumerable<AppCenterPropertyName>)allProps);
			return (AppCenterPropertyName pn) => hash.Contains(pn);
		}
		return (AppCenterPropertyName pn) => allProps.Contains(pn);
	}

	public static ShouldCensorUserDataInValueForProperty Invert(this ShouldCensorUserDataInValueForProperty shouldCensor)
	{
		ShouldCensorUserDataInValueForProperty shouldCensor2 = shouldCensor;
		return (AppCenterPropertyName pn) => !shouldCensor2(pn);
	}
}
