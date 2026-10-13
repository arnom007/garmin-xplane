using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aviation_Trainer_Launcher.LauncherUILib.ViewModels;
using Domain.AircraftDescriptor;
using Domain.AvionicsPanel;
using Domain.Hashing;
using Domain.JsonParsing;
using Domain.PurposeBuiltTraining;
using Domain.TrainerPackages;
using Domain.UtilTypes;
using IO.AircraftDescriptor;
using LOLA.Extensions;
using LOLA.Reactive;
using LOLA.Tasks;
using LanguageExt;
using LanguageExt.Common;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;
using LauncherCoreLib.Models.Winfs;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;
using Nito.AsyncEx;
using RecordShims;
using TrainerManifestParsing;
using TrainerPackageLib;

namespace Aviation_Trainer_Launcher.PilotTrainers.Models.PurposeBuiltTraining;

public static class PbtAircraft
{
	private const string PBT_DESCRIPTOR_ID_JSON_META_PROPERTY_NAME = "pbt_descriptor_id";

	private const string PBT_LOCAL_ID_JSON_META_PROPERTY_NAME = "pbt_local_id";

	private const string PBT_DESCRIPTOR_SHA512_PROPERTY_NAME = "pbt_descriptor_sha512";

	public static IEnumerable<(PbtAircraftDescriptorKey DescriptorKey, AircraftIdentifier LocalAircraftKey)> KnownPurposeBuiltAircraft => new(string, string)[4]
	{
		("CJ1", "PBTCJ1"),
		("CJ2", "PBTCJ2"),
		("PC12", "PBTPC12"),
		("SR22TN", "PBTSR22TN")
	}.Select(((string, string) t) => (new PbtAircraftDescriptorKey(StringBasedId.Parse(t.Item1).AssertValid()), AircraftIdentifier.FromString(t.Item2)));

	public static IReadOnlyDictionary<PbtAircraftDescriptorKey, PbtLocalAircraftIdentifier> PbtToLocal { get; } = KnownPurposeBuiltAircraft.ToDictionary<(PbtAircraftDescriptorKey, AircraftIdentifier), PbtAircraftDescriptorKey, PbtLocalAircraftIdentifier>(((PbtAircraftDescriptorKey DescriptorKey, AircraftIdentifier LocalAircraftKey) t) => t.DescriptorKey, ((PbtAircraftDescriptorKey DescriptorKey, AircraftIdentifier LocalAircraftKey) t) => new PbtLocalAircraftIdentifier(StringBasedId.Parse(t.LocalAircraftKey.IdentStringChecked).AssertValid()));


	public static IReadOnlyDictionary<PbtLocalAircraftIdentifier, PbtAircraftDescriptorKey> LocalToPbt { get; } = KnownPurposeBuiltAircraft.ToDictionary<(PbtAircraftDescriptorKey, AircraftIdentifier), PbtLocalAircraftIdentifier, PbtAircraftDescriptorKey>(((PbtAircraftDescriptorKey DescriptorKey, AircraftIdentifier LocalAircraftKey) t) => new PbtLocalAircraftIdentifier(StringBasedId.Parse(t.LocalAircraftKey.IdentStringChecked).AssertValid()), ((PbtAircraftDescriptorKey DescriptorKey, AircraftIdentifier LocalAircraftKey) t) => t.DescriptorKey);


	public static LocalAircraftIdentifierLookup CreateLocalAircraftLookup()
	{
		return delegate(PbtAircraftDescriptor d)
		{
			PbtAircraftDescriptor d2 = d;
			return PbtToLocal.TryGetValueRo(d2.Id).IfNone(delegate
			{
				throw new NotSupportedException("Unknown purpose built aircraft: " + d2.Id.Id.RawString + ".");
			});
		};
	}

	public static IObservable<Either<JsonError, PbtAircraftDescriptor>> AsPbtDescriptor(this IObservable<Either<JsonError, DeployedAircraftDescriptor>> ad)
	{
		return ad.Select<Either<JsonError, DeployedAircraftDescriptor>, Either<JsonError, PbtAircraftDescriptor>>((Either<JsonError, DeployedAircraftDescriptor> e) => e.Map((DeployedAircraftDescriptor dd) => new PbtAircraftDescriptor(dd.Descriptor, new PbtAircraftDescriptorFileHash(dd.FileHash))));
	}

	public static PbtLocalAircraftIdentifier ToLocalAircraftId(this AircraftIdentifier local)
	{
		return new PbtLocalAircraftIdentifier(StringBasedId.Parse(local.IdentStringChecked).AssertValid());
	}

	public static AircraftIdentifier ToStorageAircraftId(this PbtLocalAircraftIdentifier local)
	{
		return AircraftIdentifier.FromTrainerIdentifier(local.Id.RawString);
	}

	public static PurposeBuiltAircraftCreator BuildPurposeBuiltAircraftCreator(IUnitOfWork unit, LocalAircraftIdentifierLookup localLookup)
	{
		LocalAircraftIdentifierLookup localLookup2 = localLookup;
		IUnitOfWork unit2 = unit;
		AsyncLock asyncLock = new AsyncLock();
		return async delegate(PbtAircraftDescriptor descriptor)
		{
			PbtAircraftDescriptor descriptor2 = descriptor;
			PbtLocalAircraftIdentifier local = localLookup2(descriptor2);
			AircraftIdentifier ai = local.ToStorageAircraftId();
			using (await asyncLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				try
				{
					await unit2.Aircraft.GetAircraftListDataById(ai).ConfigureAwait(continueOnCapturedContext: false);
					return PbtLocalRepresentation.Valid(local);
				}
				catch (AircraftNotFoundException)
				{
				}
				await unit2.Packages.LoadingComplete.ConfigureAwait(continueOnCapturedContext: false);
				IDictionary<Domain.TrainerPackages.TrainerEmbeddedReleaseId, ITrainerPackage> relevantPackages = await (from ii in descriptor2.RawDescriptorData.EnumeratePackageElementReferenceDeclarations().ToUniquePackageRefs().ToObservable()
						.SelectMany((PbtAircraftPackageReference pr) => unit2.Packages.GetPackageByReleaseId(new TrainerPackageLib.TrainerEmbeddedReleaseId(pr.ReleaseId.RawString)))
					select ii?.PrimaryPackage).WhereClassNotNull().ToDictionary((ITrainerPackage p) => Domain.TrainerPackages.TrainerEmbeddedReleaseId.ParseOrThrow(p.TrainerPackageId.EmbeddedReleaseCompositeId));
				Dictionary<Domain.TrainerPackages.TrainerEmbeddedReleaseId, IRootManifestJsonProvider> relevantManifests = relevantPackages.ToDictionary((KeyValuePair<Domain.TrainerPackages.TrainerEmbeddedReleaseId, ITrainerPackage> kv) => kv.Key, (KeyValuePair<Domain.TrainerPackages.TrainerEmbeddedReleaseId, ITrainerPackage> kv) => ((JObject)kv.Value.BaseInfo.Manifest.CopyRoot()).ParseAsManifest().AssertValid());
				return await PbtPackageInterface.CreateManifestBasedAircraftValidator((PbtAircraftPackageReference pr) => relevantManifests.TryGetValueD(pr.ReleaseId))(descriptor2.RawDescriptorData).ToEither().ToAsync().Bind((Seq<PbtResolvedPackageElementReference<IRootManifestJsonProvider>> packageElements) => Prelude.TryAsync(async delegate
				{
					ReadOnlyJsonDataSource listModuleData = new ReadOnlyJsonDataSource(new JObject
					{
						["pbt_descriptor_id"] = JToken.FromObject(descriptor2.Id.Id.RawString),
						["pbt_local_id"] = JToken.FromObject(local.Id.RawString),
						["pbt_descriptor_sha512"] = descriptor2.Hash.Sha512.ToHashLowerHexString()
					});
					AircraftListData startingListData = new AircraftListData(ai, descriptor2.DisplayName.RawString, descriptor2.RawDescriptorData.Description.Value, ToLegacyAirframe(descriptor2.RawDescriptorData.AirframeType), listModuleData, isHidden: true, canBeDeleted: true);
					CreateAircraftHarness harness = await CreateAircraftHarness.Start(unit2, startingListData).ConfigureAwait(continueOnCapturedContext: false);
					harness.CanGenerateNewId = false;
					ReadOnlyJsonDataSource aircraftModuleData = new ReadOnlyJsonDataSource(new JObject { ["winfs_transforms"] = CreateSlotWinfsObj(descriptor2) });
					AircraftHeavyData updatedHeavy = new AircraftHeavyData(ai, aircraftModuleData);
					await harness.TemporaryAircraft.UpdateHeavy(updatedHeavy).ConfigureAwait(continueOnCapturedContext: false);
					Seq<PbtAircraftError> value = (await Task.WhenAll(descriptor2.RawDescriptorData.SimData.Select(async delegate(AircraftSimData sim)
					{
						ITrainerPackage simPackage = relevantPackages[sim.PackageReleaseId];
						ReactiveSimulator reactiveSim = await harness.TemporaryAircraft.Install(new TrainerModelKey(simPackage.TrainerPackageId, sim.Model.RawString), ToLegacySlot(sim.InstallSlot.Slot)).ConfigureAwait(continueOnCapturedContext: false);
						using (ISettingsBinder<ReactiveSimulator> settingBinder = await reactiveSim.BindSettings().ConfigureAwait(continueOnCapturedContext: false))
						{
							foreach (var settingReference in sim.Settings)
							{
								IActiveSetting activeSetting = settingBinder.Table.GetByScopedIdentifier(settingReference.localId.Id).AssertValid("package setting references have been resolved already.");
								activeSetting.SelectByLocalValueId(settingReference.settingValue.Id);
								if (!activeSetting.CurrentValue.EndsWith(settingReference.settingValue.Id, StringComparison.Ordinal))
								{
									TrainerCompleteReleaseId completePackageId = new TrainerCompleteReleaseId(sim.PackageReleaseId, simPackage.BaseInfo.BinaryVersion);
									PbtResolvedPackageElementReference<IRootManifestJsonProvider> settingValueReference = packageElements.Find<PbtResolvedPackageElementReference<IRootManifestJsonProvider>>((PbtResolvedPackageElementReference<IRootManifestJsonProvider> r) => r.PackageRef == completePackageId && r.Declaration.ElementReference.LocalSettingId() == settingReference.localId).AssertValid("These were validated above.");
									return Prelude.Some(PbtAircraftError.ParseConstraintViolation(sim.SimName, settingValueReference).AssertValid());
								}
							}
							await reactiveSim.UpdateSettings(settingBinder.Table.ToPersistenceDictionaryV2()).ConfigureAwait(continueOnCapturedContext: false);
						}
						return Prelude.None;
					})).ConfigureAwait(continueOnCapturedContext: false)).Somes().ToSeq();
					if (value.Count != 0)
					{
						return Prelude.Left(value).Bind<PbtLocalRepresentation>().ToAsync();
					}
					try
					{
						await harness.Confirm().ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (DuplicateNameException)
					{
					}
					return PbtLocalRepresentation.Valid(local);
				}).ToEither().MapLeft((Error ex) => PbtAircraftError.BrokenAircraft(ex.ToException().Message).Cons())
					.Bind((EitherAsync<Seq<PbtAircraftError>, PbtLocalRepresentation> e) => e))
					.IfLeft((Seq<PbtAircraftError> errors) => PbtLocalRepresentation.Broken(local, errors.Head))
					.ConfigureAwait(continueOnCapturedContext: false);
			}
		};
		static JObject CreateSlotWinfsObj(PbtAircraftDescriptor acDescriptor)
		{
			JObject jObject = new JObject();
			foreach (AircraftSimData simDatum in acDescriptor.RawDescriptorData.SimData)
			{
				jObject.Add(ToLegacySlot(simDatum.InstallSlot.Slot).SerializedSlot, new JObject { ["Winfs"] = simDatum.FsMountInfo.RawJson });
			}
			return jObject;
		}
		static TrainerPackageLib.AirframeCategoryClassType ToLegacyAirframe(Domain.TrainerPackages.AirframeCategoryClassType domainAirframe)
		{
			return (TrainerPackageLib.AirframeCategoryClassType)domainAirframe;
		}
		static InstallationSlot ToLegacySlot(PanelSlot slot)
		{
			return slot.ToLegacySlot();
		}
	}

	public static PurposeBuiltAircraftResetter BuildPurposeBuiltAircraftResetter(IUnitOfWork unit, LocalAircraftIdentifierLookup localLookup, PurposeBuiltAircraftCreator creator)
	{
		LocalAircraftIdentifierLookup localLookup2 = localLookup;
		IUnitOfWork unit2 = unit;
		PurposeBuiltAircraftCreator creator2 = creator;
		return async delegate(PbtAircraftDescriptor d)
		{
			PbtLocalAircraftIdentifier local = localLookup2(d);
			AircraftIdentifier ai = local.ToStorageAircraftId();
			try
			{
				await unit2.Aircraft.DeleteAircraft(ai).ConfigureAwait(continueOnCapturedContext: false);
				await unit2.PowerStats.ClearAllStatsForAircraft(ai).ConfigureAwait(continueOnCapturedContext: false);
				await unit2.FlushEverything().ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (RepoItemNotFoundException)
			{
			}
			return await creator2(d).ConfigureAwait(continueOnCapturedContext: false);
		};
	}

	public static PbtAircraftBugFixUpdateChecker BugFixUpdateChecker(IAircraftPackageReferenceLocker aircraftPackageReferenceLocker, IPackageReferenceFilter packageReferenceFilter)
	{
		IPackageReferenceFilter packageReferenceFilter2 = packageReferenceFilter;
		IAircraftPackageReferenceLocker aircraftPackageReferenceLocker2 = aircraftPackageReferenceLocker;
		return (PbtAircraftDescriptor descriptor, ReactiveAircraft localAircraft, CancellationToken cancel) => Body(localAircraft, cancel).ToAsync();
		async Task<Option<PbtAircraftError>> Body(ReactiveAircraft localAircraft, CancellationToken cancel)
		{
			ReactiveAircraft aircraftChange = localAircraft;
			List<PackageReferenceUpgradeInfo> source = (await Task.WhenAll((await Task.WhenAll(from pRef in (await localAircraft.ReferencedPackagesOverTime().FirstAsync()).Keys.ToList()
				select AircraftPackageUpgrade.GetPackageUpgradeInfo(aircraftChange, pRef)).ConfigureAwait(continueOnCapturedContext: false)).Select((PackageReferenceUpgradeInfo up) => packageReferenceFilter2.RemoveUnsupportedPackages(up))).ConfigureAwait(continueOnCapturedContext: false)).WhereClassNotNull().ToList();
			List<PackageReferenceUpgradeInfo> automaticBugfixUpdates = source.Where((PackageReferenceUpgradeInfo up) => up.IsUpgrade && !up.IsCurrentIdInstalled && !up.HasBreakingChanges).ToList();
			if (automaticBugfixUpdates.Count > 0)
			{
				using (await aircraftPackageReferenceLocker2.GetAircraftRefLock(aircraftChange.AircraftIdentifier, cancel).ConfigureAwait(continueOnCapturedContext: false))
				{
					await UpdateAutomaticBugFixRefsFromAircraft().ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			return Prelude.None;
			async Task UpdateAutomaticBugFixRefsFromAircraft()
			{
				await aircraftChange.TakeSnapshot(new AircraftSnapshotMetaData("PRE-Auto Update bug fix trainers.", new ReadOnlyJsonDataSource())).DetachContext();
				SimPanel panel = await aircraftChange.Panel.GetUpdatedValue().DetachContext();
				SimPanel updatedPanel = panel;
				List<SimInstallKey> withBreakingChanges = new List<SimInstallKey>();
				foreach (PackageReferenceUpgradeInfo bugfixUpdate3 in automaticBugfixUpdates)
				{
					IEnumerable<SimInstallData> simUpdates = from sim in EnumerateSimsThatNeedBugfix(bugfixUpdate3, panel)
						select sim.WithNewPackage(bugfixUpdate3.NewestPackageChecked.TrainerPackageId);
					updatedPanel = updatedPanel.WithUpdatedSimulators(simUpdates);
					withBreakingChanges.AddRange(from simData in EnumerateSimsThatNeedBugfix(bugfixUpdate3, panel)
						where bugfixUpdate3.NewestPackageChecked.ContainsBreakingChangesSince(simData.FullPackageIdOrThrow().BinaryVersion)
						select simData.InstallationKey);
					await UpdateFamilyData(aircraftChange, bugfixUpdate3.NewestPackageChecked, bugfixUpdate3.CurrentId.EmbeddedReleaseCompositeId).DetachContext();
				}
				await aircraftChange.UnitOfWork.Simulators.DeleteAircraftWinfsArchives(aircraftChange.AircraftIdentifier, withBreakingChanges).DetachContext();
				await aircraftChange.UpdatePanel(updatedPanel).DetachContext();
				await aircraftChange.TakeSnapshot(new AircraftSnapshotMetaData("POST-Auto Update bug fix trainers.", new ReadOnlyJsonDataSource())).DetachContext();
			}
		}
		static IEnumerable<SimInstallData> EnumerateSimsThatNeedBugfix(PackageReferenceUpgradeInfo bugfixUpdate, SimPanel simPanel)
		{
			PackageReferenceUpgradeInfo bugfixUpdate2 = bugfixUpdate;
			return simPanel.AllSimulators.Values.Where((SimInstallData simData) => simData.InstallationKey.ReleaseId == bugfixUpdate2.CurrentId.EmbeddedReleaseCompositeId && (simData.FullPackageId == null || bugfixUpdate2.NewestPackageChecked.TrainerPackageId.IsSuccessorOf(simData.FullPackageId)));
		}
		static async Task UpdateFamilyData(ReactiveAircraft aircraft, ITrainerPackage targetPackage, TrainerPackageLib.TrainerEmbeddedReleaseId releaseIdToMigrate)
		{
			ITrainerPackage targetPackage2 = targetPackage;
			FamilyData familyDataOrEmpty = (await aircraft.FamilyLookup.GetUpdatedValue().DetachContext()).GetFamilyDataOrEmpty(releaseIdToMigrate);
			ImmutableDictionary<SettingIdentifier, string> updatedSettings = ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>(((IEnumerable<KeyValuePair<SettingIdentifier, string>>)familyDataOrEmpty.Settings).Select((KeyValuePair<SettingIdentifier, string> s) => s.ToUpdatedPackageReference(targetPackage2.TrainerPackageId)));
			FamilyData data = familyDataOrEmpty.With<FamilyData>(delegate(IPropertyChangeSet<FamilyData> copy)
			{
				copy.Mutate((FamilyData c) => c.Settings, updatedSettings);
			});
			AircraftPackageFamily aircraftPackageFamily = new AircraftPackageFamily(aircraft.AircraftIdentifier, targetPackage2.TrainerPackageId.EmbeddedReleaseCompositeId);
			await aircraft.UnitOfWork.FamilyData.UpdateFamilyDataForRelease(aircraftPackageFamily, data).DetachContext();
			using ISettingsBinder<AircraftPackageFamily> settingsBinder = await aircraft.UnitOfWork.Settings.BindFamilySettings(aircraftPackageFamily).DetachContext();
			settingsBinder.Table.LoadPersistedOptionsAndGetInvalids((IEnumerable<KeyValuePair<SettingIdentifier, string>>)updatedSettings);
		}
	}

	public static PbtLocalAircraftRecognizer CreateRecognizer()
	{
		return delegate(AircraftSnapshotData snap)
		{
			AircraftSnapshotData snap2 = snap;
			return (first: ParseDescriptorKey(), second: ParseLocalId(), third: ParseHash()).Map3((PbtAircraftDescriptorKey k, PbtLocalAircraftIdentifier l, PbtAircraftDescriptorFileHash h) => new PbtDataStoredInLocalAircraft(k, l, h));
			Option<PbtAircraftDescriptorKey> ParseDescriptorKey()
			{
				JToken jToken3 = snap2.ListData.ListModuleData.CopyData("pbt_descriptor_id");
				return UtilTypeParsers.StringBasedIdParser(jToken3 ?? JValue.CreateNull()).Map((StringBasedId s) => new PbtAircraftDescriptorKey(s)).ToOption();
			}
			Option<PbtAircraftDescriptorFileHash> ParseHash()
			{
				JToken jToken = snap2.ListData.ListModuleData.CopyData("pbt_descriptor_sha512");
				return UtilTypeParsers.NonEmptyStringParser(jToken ?? JValue.CreateNull()).ToOption().Bind((NonEmptyString s) => DataHashExtensions.ParseHexStringOrNull(string.op_Implicit(s.RawValue)))
					.Bind((byte[] b) => Sha512Hash.FromHashBytes(b))
					.Map((Sha512Hash h) => new PbtAircraftDescriptorFileHash(h));
			}
			Option<PbtLocalAircraftIdentifier> ParseLocalId()
			{
				JToken jToken2 = snap2.ListData.ListModuleData.CopyData("pbt_local_id");
				return UtilTypeParsers.StringBasedIdParser(jToken2 ?? JValue.CreateNull()).Map((StringBasedId s) => new PbtLocalAircraftIdentifier(s)).ToOption();
			}
		};
	}

	public static GetAircraftWinFsSessionWorkflowMode CreatePbtWinfsModeSelector(PbtLocalAircraftRecognizer pbtRecognizer)
	{
		PbtLocalAircraftRecognizer pbtRecognizer2 = pbtRecognizer;
		return (AircraftSnapshotData snap) => pbtRecognizer2(snap).Map((PbtDataStoredInLocalAircraft _) => WinfsSessionWorkflowBehaviorMode.PurposeBuiltAircraft).IfNone(WinfsSessionWorkflowBehaviorMode.Normal);
	}

	public static GetAircraftSessionBehaviorMode CreatePbtSessionProp(PbtLocalAircraftRecognizer pbtRecognizer)
	{
		PbtLocalAircraftRecognizer pbtRecognizer2 = pbtRecognizer;
		return (AircraftSnapshotData snap) => pbtRecognizer2(snap).Map((PbtDataStoredInLocalAircraft _) => SessionBehaviorMode.PurposeBuiltAircraft).IfNone(SessionBehaviorMode.ALaCarteTrainers);
	}

	public static PbtLocalAircraftLocator CreateLocalAircraftLocator(IUnitOfWork unitOfWork, LocalAircraftIdentifierLookup localLookup, PbtLocalAircraftRecognizer recognizer, PbtAircraftBugFixUpdateChecker bugFixChecker)
	{
		LocalAircraftIdentifierLookup localLookup2 = localLookup;
		PbtLocalAircraftRecognizer recognizer2 = recognizer;
		PbtAircraftBugFixUpdateChecker bugFixChecker2 = bugFixChecker;
		IUnitOfWork unitOfWork2 = unitOfWork;
		return async delegate(PbtAircraftDescriptor d, CancellationToken cancel)
		{
			PbtAircraftDescriptor d2 = d;
			return await Prelude.TryAsync(async () => await GetLocalAircraft(localLookup2(d2)).ToAsync().MapAsync(async delegate(ReactiveAircraft la)
			{
				PbtLocalAircraftIdentifier localId2 = localLookup2(d2);
				AircraftSnapshotData snapshot = await la.CompositeSnapshots.GetUpdatedValue().ConfigureAwait(continueOnCapturedContext: false);
				return await recognizer2(snapshot).ToAsync().Filter((PbtDataStoredInLocalAircraft pbtDataInAircraft) => pbtDataInAircraft.DescriptorHash == d2.Hash).MapAsync(async (PbtDataStoredInLocalAircraft _) => await bugFixChecker2(d2, la, cancel).Map((PbtAircraftError error) => PbtLocalRepresentation.Broken(localId2, error)).IfNone(() => PbtLocalRepresentation.Valid(localId2)).ConfigureAwait(continueOnCapturedContext: false))
					.IfNone(() => PbtLocalRepresentation.Broken(localId2, PbtAircraftError.BrokenAircraft("This aircraft is out of sync with the training description and must be reset.")))
					.ConfigureAwait(continueOnCapturedContext: false);
			}).IfNoneAsync(() => Task.FromResult(PbtLocalRepresentation.Missing))
				.ConfigureAwait(continueOnCapturedContext: false)).ToEither().IfLeft((Error e) => PbtLocalRepresentation.Broken(localLookup2(d2), PbtAircraftError.BrokenAircraft(e.Message)))
				.ConfigureAwait(continueOnCapturedContext: false);
		};
		async Task<Option<ReactiveAircraft>> GetLocalAircraft(PbtLocalAircraftIdentifier localId)
		{
			ReactiveAircraft a = new ReactiveAircraft(unitOfWork2, localId.ToStorageAircraftId());
			try
			{
				await a.ListData.GetUpdatedValue().ConfigureAwait(continueOnCapturedContext: false);
				return a;
			}
			catch (AircraftNotFoundException)
			{
				return Prelude.None;
			}
		}
	}

	public static InstallationSlot ToLegacySlot(this PanelSlot slot)
	{
		SlotAvionicsType avType = slot.AvType;
		TrainerPackageLib.AvionicsType avionicsType = default(TrainerPackageLib.AvionicsType);
		switch (avType)
		{
		case SlotAvionicsType.Navigator:
			avionicsType = TrainerPackageLib.AvionicsType.Navigator;
			break;
		case SlotAvionicsType.Display:
		case SlotAvionicsType.BackupDisplay:
			avionicsType = TrainerPackageLib.AvionicsType.Display;
			break;
		case SlotAvionicsType.Other:
			throw new NotSupportedException();
		default:
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(avType);
			break;
		}
		TrainerPackageLib.AvionicsType category = avionicsType;
		return new InstallationSlot((int)new SimInstanceIndex(slot.Index).RawValue, category);
	}
}
