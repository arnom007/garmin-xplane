using DataRouterMatrix;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class LauncherRoarClientFactory : ILauncherRoarClientFactory
{
	public IRoarClient Create(DataRouterAssemblyInfo clientAssemblyInfo)
	{
		return RoarClientFactory.Create(clientAssemblyInfo.AssemblyFullPath, clientAssemblyInfo.NativeVersion);
	}
}
