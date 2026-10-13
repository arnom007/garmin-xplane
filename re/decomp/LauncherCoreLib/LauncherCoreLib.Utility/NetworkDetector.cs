using System;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Linq;

namespace LauncherCoreLib.Utility;

public class NetworkDetector : INetworkDetector
{
	public bool IsNetworkAvailable => NetworkInterface.GetIsNetworkAvailable();

	public IObservable<bool> WhenNetworkIsAvailableChanges => Observable.Return(IsNetworkAvailable).Concat<bool>(Observable.FromEventPattern<NetworkAvailabilityChangedEventHandler, NetworkAvailabilityEventArgs>(delegate(NetworkAvailabilityChangedEventHandler h)
	{
		NetworkChange.NetworkAvailabilityChanged += h;
	}, delegate(NetworkAvailabilityChangedEventHandler h)
	{
		NetworkChange.NetworkAvailabilityChanged -= h;
	}).Select<EventPattern<NetworkAvailabilityEventArgs>, bool>((Func<EventPattern<NetworkAvailabilityEventArgs>, bool>)((EventPattern<NetworkAvailabilityEventArgs> e) => e.EventArgs.IsAvailable)));
}
