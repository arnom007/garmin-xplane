using System;
using System.Threading;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

public interface ILauncherSimMasterCommander
{
	bool DoesSimSupportCommands(SimInstallKey installKey);

	System.Threading.Tasks.Task SendToClient(SimInstallKey installKey, LauncherSimCommandPacket packet, CancellationToken token);

	System.Threading.Tasks.Task Broadcast(LauncherSimCommandPacket packet, CancellationToken token);

	IObservable<LscpPacketReceipt> Messages(SimInstallKey installKey);

	IObservable<LscpPacketReceipt> AllMessages();
}
