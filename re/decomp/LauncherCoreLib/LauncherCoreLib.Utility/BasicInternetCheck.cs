using System;
using System.Reactive.Linq;
using LauncherCoreLib.Models.AbstractUi;

namespace LauncherCoreLib.Utility;

public class BasicInternetCheck
{
	private readonly INetworkDetector _networkDetector;

	public IObservable<FeatureAvailableIndicator> WhenInternetIsAvailable => _networkDetector.WhenNetworkIsAvailableChanges.Select<bool, FeatureAvailableIndicator>((Func<bool, FeatureAvailableIndicator>)((bool isAvailable) => isAvailable ? FeatureAvailableIndicator.Available : FeatureAvailableIndicator.Disabled("A network connection is required.")));

	public BasicInternetCheck(INetworkDetector detector)
	{
		_networkDetector = detector;
	}
}
