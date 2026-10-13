using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.EventLogging;

public class HttpRequestErrorSendingTrackedEvent : WebExceptionSendingTrackedEvent
{
	[field: CompilerGenerated]
	public override System.Exception Inner
	{
		[CompilerGenerated]
		get;
	}

	public HttpRequestErrorSendingTrackedEvent(System.Exception inner)
	{
		Inner = inner;
	}

	public virtual string ToString()
	{
		return $"Error: Exception {Inner} occurred when attempting to send tracked event to app center";
	}
}
