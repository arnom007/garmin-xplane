using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.AppLifeCycle.Migrations;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Simulated.Airport;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models;

public class CreateDefaultAircraftStartupAction : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	private enum StartingAircraftStatus
	{
		Missing,
		NeedsRepair,
		Exists
	}

	private readonly IUnitOfWork _unitOfWork;

	private readonly IStartingAircraftFactory _startingAircraft;

	private readonly IStartupRegistry _startupRegistry;

	private readonly IAircraftSnapshotDialogPresenter _snapshotDialogPresenter;

	public CreateDefaultAircraftStartupAction(IUnitOfWork unitOfWork, IStartingAircraftFactory startingAircraft, IStartupRegistry startupRegistry, IAircraftSnapshotDialogPresenter snapshotDialogPresenter)
	{
		_unitOfWork = unitOfWork;
		_startingAircraft = startingAircraft;
		_startupRegistry = startupRegistry;
		_snapshotDialogPresenter = snapshotDialogPresenter;
	}

	public async Task OnStartup(CancellationToken shutdown)
	{
		await _startupRegistry.WaitForStartupActionToComplete<BuiltInMigrations>().DetachContext();
		IList<AircraftListData> list = _startingAircraft.CreateStartingAircraftInfos();
		if (list.Count > 0)
		{
			await CreateStartingAircraft(list, shutdown).DetachContext();
		}
		else
		{
			Log(LogLevel.Warning, "There were no starting aircraft to create.");
		}
	}

	private async Task CreateStartingAircraft(IList<AircraftListData> startingAircraft, CancellationToken shutdown)
	{
		try
		{
			foreach (AircraftListData sa in startingAircraft)
			{
				if (!sa.AircraftIdentifier.IsValid)
				{
					throw new InvalidOperationException("Cannot check for an aircraft with an invalid identifier.");
				}
				shutdown.ThrowIfCancellationRequested();
				StartingAircraftStatus startingAircraftStatus = await AircraftExists(sa).DetachContext();
				switch (startingAircraftStatus)
				{
				case StartingAircraftStatus.Missing:
				{
					Log(LogLevel.Info, "The {0} aircraft was not found. Creating a default one.", sa.AircraftIdentifier);
					using (CreateAircraftHarness createAircraftHarness = await CreateAircraftHarness.Start(_unitOfWork, sa).DetachContext())
					{
						await createAircraftHarness.TemporaryAircraft.LoadDefaultSims().LastOrDefaultAsync().ToTask(shutdown)
							.DetachContext();
						ReactiveAircraft reactiveAircraft = await createAircraftHarness.Confirm().DetachContext();
						Log(LogLevel.Info, "Created new aircraft ({0}: {1}) with default sims.", reactiveAircraft.AircraftIdentifier, reactiveAircraft.ListData.CachedValue?.DisplayName ?? "<NULL>");
					}
					break;
				}
				case StartingAircraftStatus.NeedsRepair:
					Log(LogLevel.Warning, $"The {sa.AircraftIdentifier} aircraft was found in need of repair.");
					break;
				case StartingAircraftStatus.Exists:
					Log(LogLevel.Debug, "The {0} aircraft already exists. Skipping.", sa.AircraftIdentifier);
					break;
				default:
					throw new NotSupportedException(startingAircraftStatus.ToString());
				}
			}
		}
		catch (IOException ex)
		{
			if (!ex.IsDiskFull())
			{
				throw;
			}
			Log(LogLevel.Error, "Cannot create default aircraft because the disk is full.");
		}
		if (await _unitOfWork.Aircraft.GetLastSelectedAircraftOrDefault().DetachContext() == AircraftIdentifier.Default && startingAircraft.Count > 0)
		{
			AircraftListData aircraftListData = startingAircraft.First();
			Log(LogLevel.Info, "Setting the selected aircraft to {0}", aircraftListData.AircraftIdentifier);
			await _unitOfWork.Aircraft.StoreLastSelectedAircraft(aircraftListData.AircraftIdentifier).DetachContext();
		}
	}

	private async Task<StartingAircraftStatus> AircraftExists(AircraftListData sa)
	{
		try
		{
			await _unitOfWork.Aircraft.GetAircraftListDataById(sa.AircraftIdentifier).DetachContext();
			return StartingAircraftStatus.Exists;
		}
		catch (AircraftRollbackException)
		{
			return StartingAircraftStatus.NeedsRepair;
		}
		catch (FormatException)
		{
			return StartingAircraftStatus.NeedsRepair;
		}
		catch (AircraftNotFoundException)
		{
			return StartingAircraftStatus.Missing;
		}
	}
}
