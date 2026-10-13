using System;
using System.Runtime.CompilerServices;
using GTI.Net;

namespace LauncherCoreLib.Models.TrainerPackages;

public class BaseGtiRunnerProvider : IBaseGtiRunnerProvider
{
	[field: CompilerGenerated]
	public IGtiRunner UnprotectedGtiRunner
	{
		[CompilerGenerated]
		get;
	}

	[Obsolete("Transition GTI to newer layered FileTrust API")]
	public BaseGtiRunnerProvider(GtiRunnerFactory gtiFactory)
	{
		UnprotectedGtiRunner = new AppCenterLoggingGtiRunner(gtiFactory.CreateDefaultRunner());
	}
}
