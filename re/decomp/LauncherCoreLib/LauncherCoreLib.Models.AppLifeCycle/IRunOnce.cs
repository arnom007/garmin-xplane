using System;
using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IRunOnce
{
	string OperationName { get; }

	DateTimeOffset? CompletedAt { get; }

	System.Threading.Tasks.Task<IRunOnce> MarkCompleted();
}
