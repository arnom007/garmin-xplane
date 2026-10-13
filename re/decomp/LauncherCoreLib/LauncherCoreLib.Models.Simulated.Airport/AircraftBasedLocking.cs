using System;
using System.Collections.Concurrent;
using Nito.AsyncEx;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftBasedLocking
{
	private readonly ConcurrentDictionary<AircraftIdentifier, AsyncReaderWriterLock> _aircraftLocks = new ConcurrentDictionary<AircraftIdentifier, AsyncReaderWriterLock>();

	public AsyncReaderWriterLock GetLockForAircraftUnlocked(AircraftIdentifier aircraftIdentifier)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		aircraftIdentifier.ThrowIfDefault((System.Exception?)new ArgumentException("Cannot use invalid aircraft identifier."));
		return _aircraftLocks.GetOrAdd(aircraftIdentifier, (Func<AircraftIdentifier, AsyncReaderWriterLock>)((AircraftIdentifier _) => new AsyncReaderWriterLock()));
	}

	public AsyncReaderWriterLock? RemoveLock(AircraftIdentifier aircraftIdentifier)
	{
		AsyncReaderWriterLock result = default(AsyncReaderWriterLock);
		_aircraftLocks.TryRemove(aircraftIdentifier, ref result);
		return result;
	}
}
