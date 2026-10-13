using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

public class LauncherSimMasterCommander : ILauncherSimMasterCommander
{
	private readonly ReadOnlyDictionary<SimInstallKey, ILauncherSimCommander> _launcherSimCommanders;

	private readonly IObservable<LscpPacketReceipt> _sharedAllMessagesStream;

	public LauncherSimMasterCommander(ITrainingSession trainingSession, ITrainingSessionLauncherSimCommanderProvider commanderProvider)
	{
		_sharedAllMessagesStream = Observable.Empty<LscpPacketReceipt>();
		Dictionary<SimInstallKey, ILauncherSimCommander> val = new Dictionary<SimInstallKey, ILauncherSimCommander>();
		System.Collections.Generic.IEnumerator<SimProcessSession> enumerator = ((System.Collections.Generic.IEnumerable<SimProcessSession>)trainingSession.Simulators).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				SimInstallKey installationKey = enumerator.Current.Simulator.InstallData.InstallationKey;
				if (commanderProvider.DoesSimSupportLscp(installationKey))
				{
					ILauncherSimCommander orCreateSimCommander = commanderProvider.GetOrCreateSimCommander(installationKey);
					val.Add(installationKey, orCreateSimCommander);
					_sharedAllMessagesStream = _sharedAllMessagesStream.Merge<LscpPacketReceipt>(orCreateSimCommander.Messages());
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		_launcherSimCommanders = new ReadOnlyDictionary<SimInstallKey, ILauncherSimCommander>((IDictionary<SimInstallKey, ILauncherSimCommander>)(object)val);
		_sharedAllMessagesStream = _sharedAllMessagesStream.Publish<LscpPacketReceipt>().RefCount();
	}

	public bool DoesSimSupportCommands(SimInstallKey installKey)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if ((object)installKey == null)
		{
			throw new ArgumentNullException("installKey");
		}
		ILauncherSimCommander launcherSimCommander = default(ILauncherSimCommander);
		return _launcherSimCommanders.TryGetValue(installKey, ref launcherSimCommander);
	}

	public System.Threading.Tasks.Task SendToClient(SimInstallKey installKey, LauncherSimCommandPacket packet, CancellationToken token)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (installKey == null)
		{
			throw new ArgumentNullException("installKey");
		}
		if (packet == null)
		{
			throw new ArgumentNullException("packet");
		}
		ILauncherSimCommander launcherSimCommander = default(ILauncherSimCommander);
		if (!_launcherSimCommanders.TryGetValue(installKey, ref launcherSimCommander))
		{
			throw new KeyNotFoundException("Invalid install key");
		}
		byte[] bytes = packet.ToBinary();
		return launcherSimCommander.SendToClient(bytes, token);
	}

	public System.Threading.Tasks.Task Broadcast(LauncherSimCommandPacket packet, CancellationToken token)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (packet == null)
		{
			throw new ArgumentNullException("packet");
		}
		byte[] bytes = packet.ToBinary();
		List<System.Threading.Tasks.Task> val = new List<System.Threading.Tasks.Task>();
		System.Collections.Generic.IEnumerator<KeyValuePair<SimInstallKey, ILauncherSimCommander>> enumerator = _launcherSimCommanders.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				val.Add(enumerator.Current.Value.SendToClient(bytes, token));
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		return System.Threading.Tasks.Task.WhenAll((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>)val);
	}

	public IObservable<LscpPacketReceipt> Messages(SimInstallKey installKey)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (installKey == null)
		{
			throw new ArgumentNullException("installKey");
		}
		ILauncherSimCommander launcherSimCommander = default(ILauncherSimCommander);
		if (!_launcherSimCommanders.TryGetValue(installKey, ref launcherSimCommander))
		{
			throw new KeyNotFoundException("Invalid install key");
		}
		return launcherSimCommander.Messages();
	}

	public IObservable<LscpPacketReceipt> AllMessages()
	{
		return _sharedAllMessagesStream;
	}
}
