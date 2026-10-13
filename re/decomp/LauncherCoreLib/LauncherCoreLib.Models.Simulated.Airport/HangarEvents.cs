using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Simulated.Airport;

public sealed class HangarEvents : System.IDisposable
{
	private readonly Subject<DataChangeNotification<AircraftIdentifier>> _aircraftChanges = new Subject<DataChangeNotification<AircraftIdentifier>>();

	private readonly Subject<DataChangeNotification<SimAircraftKey>> _simulatorChanges = new Subject<DataChangeNotification<SimAircraftKey>>();

	private readonly Subject<DataChangeNotification<AircraftPackageFamily>> _aircraftFamilyChanges = new Subject<DataChangeNotification<AircraftPackageFamily>>();

	private readonly Subject<AircraftIdentifier> _selectedAircraftChanges = new Subject<AircraftIdentifier>();

	private readonly Subject<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>> _snapshotChange = new Subject<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>>();

	private bool _disposed;

	public IObservable<DataChangeNotification<AircraftIdentifier>> WhenAircraftChanges => ((IObservable<DataChangeNotification<AircraftIdentifier>>)(object)_aircraftChanges).AsObservable<DataChangeNotification<AircraftIdentifier>>();

	public IObservable<DataChangeNotification<SimAircraftKey>> WhenSimulatorChanges => ((IObservable<DataChangeNotification<SimAircraftKey>>)(object)_simulatorChanges).AsObservable<DataChangeNotification<SimAircraftKey>>();

	public IObservable<DataChangeNotification<AircraftPackageFamily>> WhenAircraftFamilyDataChanges => ((IObservable<DataChangeNotification<AircraftPackageFamily>>)(object)_aircraftFamilyChanges).AsObservable<DataChangeNotification<AircraftPackageFamily>>();

	public IObservable<AircraftIdentifier> WhenSelectedAircraftChanges => ((IObservable<AircraftIdentifier>)(object)_selectedAircraftChanges).AsObservable<AircraftIdentifier>();

	public IObservable<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>> WhenSnapshotChanges => ((IObservable<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>>)(object)_snapshotChange).AsObservable<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>>();

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			_aircraftChanges.Dispose();
			_simulatorChanges.Dispose();
			_aircraftFamilyChanges.Dispose();
			_selectedAircraftChanges.Dispose();
			_snapshotChange.Dispose();
		}
	}

	public void PostAircraftChange(DataChangeNotification<AircraftIdentifier> aircraftChange)
	{
		lock (_aircraftChanges)
		{
			_aircraftChanges.OnNext(aircraftChange);
		}
	}

	public void PostSelectedAircraftChange(AircraftIdentifier result)
	{
		lock (_selectedAircraftChanges)
		{
			_selectedAircraftChanges.OnNext(result);
		}
	}

	public void PostSimulatorChange(DataChangeNotification<SimAircraftKey> simulatorChange)
	{
		lock (_simulatorChanges)
		{
			_simulatorChanges.OnNext(simulatorChange);
		}
	}

	public void PostSimulatorChanges(System.Collections.Generic.IEnumerable<DataChangeNotification<SimAircraftKey>> simChanges)
	{
		lock (_simulatorChanges)
		{
			System.Collections.Generic.IEnumerator<DataChangeNotification<SimAircraftKey>> enumerator = simChanges.GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					DataChangeNotification<SimAircraftKey> current = enumerator.Current;
					_simulatorChanges.OnNext(current);
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
	}

	public void PostFamilyDataChange(DataChangeNotification<AircraftPackageFamily> familyChange)
	{
		lock (_aircraftFamilyChanges)
		{
			_aircraftFamilyChanges.OnNext(familyChange);
		}
	}

	public void PostFamilyDataChanges(System.Collections.Generic.IEnumerable<DataChangeNotification<AircraftPackageFamily>> familyChanges)
	{
		lock (_aircraftFamilyChanges)
		{
			System.Collections.Generic.IEnumerator<DataChangeNotification<AircraftPackageFamily>> enumerator = familyChanges.GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					DataChangeNotification<AircraftPackageFamily> current = enumerator.Current;
					_aircraftFamilyChanges.OnNext(current);
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
	}

	public void PostSnapshotChange(DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>> snapshotChange)
	{
		lock (_snapshotChange)
		{
			_snapshotChange.OnNext(snapshotChange);
		}
	}
}
