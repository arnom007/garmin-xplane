using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class NavigationDataDescriptor : TimeRangeTrackedEventDataDescriptor
{
	private readonly Uri? _navFrom;

	private readonly Uri? _navTo;

	private readonly TimeSpan? _duration;

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Navigation"), EventTargetApps.Launcher);

	private NavigationDataDescriptor(Uri? navFrom, Uri? navTo, TimeSpan? duration)
	{
		_navFrom = navFrom;
		_navTo = navTo;
		_duration = duration;
	}

	public static NavigationDataDescriptor Initial(Uri initialUri)
	{
		return new NavigationDataDescriptor(null, initialUri, null);
	}

	public static NavigationDataDescriptor PageTransition(Uri from, Uri to, TimeSpan timeSpentOnFrom)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return new NavigationDataDescriptor(from, to, timeSpentOnFrom);
	}

	public static NavigationDataDescriptor Final(Uri from, TimeSpan timeSpentOnFrom)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return new NavigationDataDescriptor(from, null, timeSpentOnFrom);
	}

	public override TrackedEventContainer Instance(ITimeRangeProvider timeRangeProvider)
	{
		return EventPrototype.ToInstance().With(BuildProperties(timeRangeProvider), null, EventUserDataCensor.None);
	}

	protected override PropertiesBuilder BuildProperties(ITimeRangeProvider timeRangeProvider)
	{
		ITimeRangeProvider timeRangeProvider2 = timeRangeProvider;
		return delegate
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			string propertyValue = (_duration.HasValue ? timeRangeProvider2.GetTimeRangeString(_duration.Value) : "Initial Page");
			string text = PageNameOrUnit(_navFrom);
			string text2 = PageNameOrUnit(_navTo);
			return (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Time_Spent_On_Page"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(propertyValue)).AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Navigated"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(text + " -> " + text2));
		};
		[CompilerGenerated]
		static string PageNameOrUnit(Uri? pageUri)
		{
			return ((object)pageUri)?.ToString() ?? "()";
		}
	}
}
