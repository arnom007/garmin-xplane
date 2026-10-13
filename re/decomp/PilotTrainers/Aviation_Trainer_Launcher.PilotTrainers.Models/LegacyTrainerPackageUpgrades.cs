using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models;

public class LegacyTrainerPackageUpgrades : IAutomaticLegacyPackageUpgrades
{
	private sealed class RequiredCheckResult : IAutomaticLegacyPackageUpgradeCheckResult
	{
		private readonly HashSet<TrainerPackageIdentifier> _legacyPackageRefs;

		public bool IsUpgradeNeeded => true;

		public RequiredCheckResult(HashSet<TrainerPackageIdentifier> legacyPackageRefs)
		{
			_legacyPackageRefs = legacyPackageRefs;
		}

		public async Task ApplyUpgrade(ReactiveAircraft reactiveAircraft)
		{
			HashSet<TrainerIdentifier> affectedFamilies = _legacyPackageRefs.Select((TrainerPackageIdentifier tpi) => tpi.FamilyId).ToHashSet();
			IDictionary<TrainerIdentifier, ITrainerPackage> mostUpToDatePackageByFamily = await (from p in reactiveAircraft.UnitOfWork.Packages.GetAllPackages()
				where affectedFamilies.Contains(p.TrainerPackageId.FamilyId) && LEGACY_PACKAGE_BOUNDARIES[p.TrainerPackageId.FamilyId].LegacyMaxVersion < p.TrainerPackageId.BinaryVersion
				group p by p.TrainerPackageId.FamilyId).SelectMany((IGroupedObservable<TrainerIdentifier, ITrainerPackage> familyGroup) => from maxList in familyGroup.MaxBy((ITrainerPackage p) => p.TrainerPackageId.BinaryVersion)
				select maxList.OrderByDescending((ITrainerPackage p) => p.BaseInfo.PackageType.IsCompleteOnDisk()).First()).ToDictionary((ITrainerPackage p) => p.TrainerPackageId.FamilyId);
			await reactiveAircraft.TakeSnapshot(new AircraftSnapshotMetaData("PRE-Migrate 2X Trainers.", new ReadOnlyJsonDataSource())).DetachContext();
			SimPanel simPanel = await reactiveAircraft.Panel.GetUpdatedValue().DetachContext();
			List<SimInstallData> source = simPanel.AllSimulators.Values.Where((SimInstallData sim) => sim.FullPackageId != null && _legacyPackageRefs.Contains(sim.FullPackageId)).ToList();
			ITrainerPackage value3;
			List<(SimInstallData, ITrainerPackage, SimInstallData)> source2 = (from sim in (from sim in source
					where mostUpToDatePackageByFamily.TryGetValue(sim.InstallationKey.ReleaseId.FamilyId, out value3) && value3.GetModel(sim.InstallationKey.ModelId) != null
					group sim by sim.InstallationKey).SelectMany((IGrouping<SimInstallKey, SimInstallData> keyGroup) => keyGroup.OrderBy((SimInstallData sim) => sim.FullPackageId?.BinaryVersion).Take(1))
				select (previous: sim, package: mostUpToDatePackageByFamily[sim.InstallationKey.ReleaseId.FamilyId], updatedSim: sim.WithNewPackage(mostUpToDatePackageByFamily[sim.InstallationKey.ReleaseId.FamilyId].TrainerPackageId))).ToList();
			ITrainerPackage value2;
			IEnumerable<SimInstallKey> first = from sim in source
				where mostUpToDatePackageByFamily.TryGetValue(sim.InstallationKey.ReleaseId.FamilyId, out value2) && value2.TrainerPackageId.EmbeddedReleaseCompositeId != sim.InstallationKey.ReleaseId
				select sim.InstallationKey;
			ITrainerPackage value;
			IEnumerable<SimInstallKey> second = from sim in source
				where !mostUpToDatePackageByFamily.TryGetValue(sim.InstallationKey.ReleaseId.FamilyId, out value) || value.GetModel(sim.InstallationKey.ModelId) == null
				select sim.InstallationKey;
			IEnumerable<SimInstallKey> enumerable = first.Concat(second);
			SimPanel updatedPanel2 = simPanel;
			updatedPanel2 = updatedPanel2.WithDeletedSimulators(enumerable);
			updatedPanel2 = updatedPanel2.WithUpdatedSimulators(source2.Select<(SimInstallData, ITrainerPackage, SimInstallData), SimInstallData>(((SimInstallData previous, ITrainerPackage package, SimInstallData updatedSim) t) => t.updatedSim));
			IEnumerable<SimInstallKey> second2 = from t in source2
				where t.previous.FullPackageId != null && t.package.ContainsBreakingChangesSince(t.previous.FullPackageId.BinaryVersion)
				select t.updatedSim.InstallationKey;
			IEnumerable<SimInstallKey> simsToClear = enumerable.Concat(second2).Distinct();
			await reactiveAircraft.UnitOfWork.Simulators.DeleteAircraftWinfsArchives(reactiveAircraft.AircraftIdentifier, simsToClear).DetachContext();
			await reactiveAircraft.UpdatePanel(updatedPanel2).DetachContext();
			await reactiveAircraft.TakeSnapshot(new AircraftSnapshotMetaData("POST-Migrate 2X Trainers.", new ReadOnlyJsonDataSource())).DetachContext();
		}
	}

	private static readonly Dictionary<TrainerIdentifier, (TrainerIdentifier FamilyId, Version LegacyMaxVersion)> LEGACY_PACKAGE_BOUNDARIES = GenerateBoundaries();

	public IAutomaticLegacyPackageUpgradeCheckResult CheckPackageReferences(IReadOnlyDictionary<TrainerPackageIdentifier, ITrainerPackage?> packRefs)
	{
		(TrainerIdentifier, Version) value;
		HashSet<TrainerPackageIdentifier> hashSet = (from pRef in packRefs
			where LEGACY_PACKAGE_BOUNDARIES.TryGetValue(pRef.Key.FamilyId, out value) && pRef.Key.BinaryVersion <= value.Item2
			select pRef into kv
			select kv.Key).ToHashSet();
		if (hashSet.Count > 0)
		{
			return new RequiredCheckResult(hashSet);
		}
		return NoLegacyPackageUpgrades.Instance;
	}

	private static Dictionary<TrainerIdentifier, (TrainerIdentifier FamilyId, Version LegacyMaxVersion)> GenerateBoundaries()
	{
		return new(TrainerIdentifier, Version)[5]
		{
			CreateLegacyVersionBoundary("GPS175", "3.10.2.0"),
			CreateLegacyVersionBoundary("GTN", "6.62.4.0"),
			CreateLegacyVersionBoundary("GTN_Xi", "20.1.2.0"),
			CreateLegacyVersionBoundary("TXi", "3.1.4.0"),
			CreateLegacyVersionBoundary("GDU620", "7.37.4.0")
		}.ToDictionary(((TrainerIdentifier FamilyId, Version LegacyMaxVersion) t) => t.FamilyId);
		static (TrainerIdentifier FamilyId, Version LegacyMaxVersion) CreateLegacyVersionBoundary(TrainerIdentifier family, string versionStr)
		{
			return (FamilyId: family, LegacyMaxVersion: Version.Parse(versionStr));
		}
	}
}
