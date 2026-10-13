using System.Threading;
using System.Threading.Tasks;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using LauncherCoreLib.Models.Simulated.Airport;
using LruSoftwareUpdateModule.Models;

namespace PilotTrainers.Models;

public class AircraftRefLockingPackageUpdateStrategy : IAircraftPackageUpdateStrategy
{
	private readonly IAircraftPackageUpdateStrategy _inner;

	private readonly IAircraftPackageReferenceLocker _aircraftPackageReferenceLocker;

	public AircraftRefLockingPackageUpdateStrategy(IAircraftPackageUpdateStrategy inner, IAircraftPackageReferenceLocker aircraftPackageReferenceLocker)
	{
		_inner = inner;
		_aircraftPackageReferenceLocker = aircraftPackageReferenceLocker;
	}

	public async Task<LruSoftwareUpdateResult> UpdateAircraft(ReactiveAircraft aircraft, CancellationToken token = default(CancellationToken))
	{
		using (await _aircraftPackageReferenceLocker.GetAircraftRefLock(aircraft.AircraftIdentifier, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false))
		{
			return await _inner.UpdateAircraft(aircraft, token).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
