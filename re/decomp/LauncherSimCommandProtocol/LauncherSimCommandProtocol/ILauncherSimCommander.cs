using System;
using System.Threading;
using System.Threading.Tasks;

namespace LauncherSimCommandProtocol;

public interface ILauncherSimCommander
{
	System.Threading.Tasks.Task SendToClient(byte[] bytes, CancellationToken token);

	IObservable<LscpPacketReceipt> Messages();
}
