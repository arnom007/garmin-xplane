using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class ConnectionErrorSendingTrackedEvent : WebExceptionSendingTrackedEvent
{
	[field: CompilerGenerated]
	public override System.Exception Inner
	{
		[CompilerGenerated]
		get;
	}

	public ConnectionErrorSendingTrackedEvent(System.Exception inner)
	{
		Inner = inner;
	}

	public virtual string ToString()
	{
		return $"Error: Exception {Inner} occured while trying to connect to the App Center API endpoint";
	}
}
