using System;
using DataRouterMatrix;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ISimSessionRouterInstallationAdapter
{
	ValueTuple<DataRouterAssemblyInfo, DataRouterAssemblyInfo> ResolveRouterAssembliesForEnvironment(ISessionEnvironment environment);
}
