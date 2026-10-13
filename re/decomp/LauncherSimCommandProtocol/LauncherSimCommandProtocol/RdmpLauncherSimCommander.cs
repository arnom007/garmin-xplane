using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Panel;
using ReliableDirectMessageProtocol;

namespace LauncherSimCommandProtocol;

public class RdmpLauncherSimCommander : ILauncherSimCommander
{
	private readonly IObservable<LscpPacketReceipt> _sharedMessageStream;

	private readonly SimInstallKey _installKey;

	private readonly System.Threading.Tasks.Task _exitTask;

	private readonly IReliableDirectMessageCom _rdmpClient;

	public RdmpLauncherSimCommander(SimInstallKey installKey, System.Threading.Tasks.Task exitTask, IReliableDirectMessageCom rdmpClient)
	{
		_installKey = installKey;
		_exitTask = exitTask;
		_rdmpClient = rdmpClient;
		_sharedMessageStream = _rdmpClient.Messages().SelectMany<RdmpPayload, LauncherSimCommandPacket>((Func<RdmpPayload, System.Collections.Generic.IEnumerable<LauncherSimCommandPacket>>)((RdmpPayload payload) => LscpBufferParser.GetPackets(payload))).Where<LauncherSimCommandPacket>((Func<LauncherSimCommandPacket, bool>)((LauncherSimCommandPacket packet) => packet != null))
			.Select<LauncherSimCommandPacket, LscpPacketReceipt>((Func<LauncherSimCommandPacket, LscpPacketReceipt>)([CompilerGenerated] (LauncherSimCommandPacket pkt) => new LscpPacketReceipt(pkt, _installKey)))
			.Publish<LscpPacketReceipt>()
			.RefCount();
	}

	public System.Threading.Tasks.Task SendToClient(byte[] bytes, CancellationToken token)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (_exitTask.IsCompleted)
		{
			throw new SimOfflineException(_installKey, "Command could not be sent because the sim has been closed");
		}
		return _rdmpClient.Send(bytes, token);
	}

	public IObservable<LscpPacketReceipt> Messages()
	{
		return _sharedMessageStream;
	}
}
