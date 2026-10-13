using System;

namespace LauncherCoreLib.Utility;

public interface INetworkDetector
{
	bool IsNetworkAvailable { get; }

	IObservable<bool> WhenNetworkIsAvailableChanges { get; }
}
