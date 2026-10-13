using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public sealed class AppLifeCycleActionRecord
{
	[field: CompilerGenerated]
	public IAppLifeCycleAction Action
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Threading.Tasks.Task Completion
	{
		[CompilerGenerated]
		get;
	}

	public AppLifeCycleActionRecord(IAppLifeCycleAction action, System.Threading.Tasks.Task completion)
	{
		Action = action;
		Completion = completion;
	}

	public string ToString()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0} - {1}", (object)(Action.ActionName ?? ((object)Action).ToString()), (object)Completion.Status);
	}
}
