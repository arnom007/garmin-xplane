using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging.DataDescriptors;

public sealed class NavigationFailedDataDescriptor : GenericTrackedEventDataDescriptor
{
	private readonly Uri? _navFrom;

	private readonly Uri _navTo;

	private readonly AppCenterPropertyName _navErrorPropName = AppCenterPropertyName.ParseAndThrowOnErrors("Nav_Error_Message");

	[field: CompilerGenerated]
	public System.Exception Error
	{
		[CompilerGenerated]
		get;
	}

	protected override TrackedEventPrototype EventPrototype => TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("Navigation"), EventTargetApps.Launcher);

	protected override PropertiesBuilder BuildProperties
	{
		get
		{
			return [CompilerGenerated] () =>
			{
				string text = PageNameOrUnit(_navFrom);
				string text2 = PageNameOrUnit(_navTo);
				return (System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>)ImmutableList.Create<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>>().AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Navigated"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(text + " -> " + text2 + " Failed")).AddNewKvp(AppCenterPropertyName.ParseAndThrowOnErrors("Nav_Error_Type"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(Error.GetType().FullName ?? ""))
					.AddNewKvp(_navErrorPropName, AppCenterPropertyValue.ParseTruncateAndHandleErrors(Error.Message ?? ""));
			};
			[CompilerGenerated]
			static string PageNameOrUnit(Uri? pageUri)
			{
				return ((object)pageUri)?.ToString() ?? "()";
			}
		}
	}

	public NavigationFailedDataDescriptor(Uri? navFrom, Uri navTo, System.Exception error)
	{
		_navFrom = navFrom;
		_navTo = navTo;
		Error = error;
	}

	public override TrackedEventContainer Instance()
	{
		return EventPrototype.ToInstance().With(BuildProperties, null, EventUserDataCensor.CensorProperties(_navErrorPropName));
	}
}
