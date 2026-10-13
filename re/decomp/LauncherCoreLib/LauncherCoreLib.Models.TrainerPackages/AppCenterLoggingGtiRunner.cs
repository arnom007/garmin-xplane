using System;
using GTI.Net;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;

namespace LauncherCoreLib.Models.TrainerPackages;

public class AppCenterLoggingGtiRunner : GtiRunnerWrapper
{
	public AppCenterLoggingGtiRunner(IGtiRunner runner)
		: base(runner)
	{
	}

	public override IGtiProcessExecution Execute(params string[] commandLineArgs)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		IGtiProcessExecution gtiProcessExecution = base.Execute(commandLineArgs);
		if (gtiProcessExecution != null)
		{
			gtiProcessExecution = new GtiProcessWrapper(gtiProcessExecution, delegate(GtiProcessResult pr)
			{
				pr.LogInterestingResultToAppCenter();
				return pr;
			});
		}
		return gtiProcessExecution ?? throw new InvalidOperationException($"Inner Runner ({base.Runner}) returned null GTI execution.");
	}
}
