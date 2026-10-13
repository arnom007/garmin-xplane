using System;

namespace LauncherCoreLib.Models.EventLogging;

public abstract class WebExceptionSendingTrackedEvent : TrackedEventError
{
	public abstract System.Exception Inner { get; }
}
