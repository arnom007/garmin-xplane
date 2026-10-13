using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LruSoftwareUpdateModule.Models;
using TrainerPackageLib;

namespace PilotTrainers.Models;

public class PilotAircraftPackageUpdater : IAircraftPackageUpdater
{
	private readonly IAircraftPackageReferenceLocker _aircraftPackageReferenceLocker;

	private readonly PbtLocalAircraftRecognizer _pbtRecognizer;

	public PilotAircraftPackageUpdater(IAircraftPackageReferenceLocker aircraftPackageReferenceLocker, PbtLocalAircraftRecognizer pbtRecognizer)
	{
		_aircraftPackageReferenceLocker = aircraftPackageReferenceLocker;
		_pbtRecognizer = pbtRecognizer;
	}

	public async Task<IAircraftPackageUpdateStrategy?> GetStrategyForAircraft(ReactiveAircraft aircraft, ISoftwareUpdateSeed seed, ISoftwareUpdateDeclarationList declarations, ISoftwareUpdateOptionList options, CancellationToken token = default(CancellationToken))
	{
		IAircraftFilterOption aircraftFilterOption = options.OfType<IAircraftFilterOption>().FirstOrDefault();
		if (aircraftFilterOption != null && !aircraftFilterOption.IsAircraftIncluded(aircraft.AircraftIdentifier))
		{
			return null;
		}
		if (seed is IPackageBasedUpdateSeed packageSeed)
		{
			ITrainerPackage package = packageSeed.TargetPackage;
			AircraftSnapshotData aircraftSnapshotData = await aircraft.CompositeSnapshots.GetUpdatedValue().ConfigureAwait(continueOnCapturedContext: false);
			SimPanel panel = aircraftSnapshotData.Panel;
			if (packageSeed.UpdateType == PackageUpdateType.EmbeddedReleaseMigration)
			{
				if (_pbtRecognizer(aircraftSnapshotData).IsSome)
				{
					return null;
				}
				if (EmbeddedReleaseAircraftPackageUpdateStrategy.CanUpdate(panel, package, out EmbeddedReleaseAircraftPackageUpdateStrategy strategy))
				{
					if (BugFixAircraftPackageUpdateStrategy.CanUpdate(panel, package, out BugFixAircraftPackageUpdateStrategy strategy2))
					{
						return new CombinedPackageUpdateStrategy(new IAircraftPackageUpdateStrategy[2]
						{
							WrapBugfixStrategy(strategy2),
							strategy
						});
					}
					return strategy;
				}
			}
			else
			{
				if (packageSeed.UpdateType != 0)
				{
					throw new NotSupportedException(packageSeed.UpdateType.ToString());
				}
				if (BugFixAircraftPackageUpdateStrategy.CanUpdate(panel, package, out BugFixAircraftPackageUpdateStrategy strategy3))
				{
					return WrapBugfixStrategy(strategy3);
				}
			}
		}
		return null;
		IAircraftPackageUpdateStrategy WrapBugfixStrategy(BugFixAircraftPackageUpdateStrategy bugfixStategy)
		{
			return new AircraftRefLockingPackageUpdateStrategy(bugfixStategy, _aircraftPackageReferenceLocker);
		}
	}
}
