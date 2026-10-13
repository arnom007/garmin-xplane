using System;
using GTI.Net;

namespace LauncherCoreLib.Models.TrainerPackages;

public interface ILauncherGtiRunner : IGtiRunner
{
	event EventHandler MutationCommandCompleted;
}
