using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GTI.Net;
using LOLA.Reactive;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class GtiStatusProvider : IInstallerListingInfoProvider, ICachedGtiDataProvider
{
	[Flags]
	private enum GtiListingSearchPriority
	{
		None = 0,
		HasVersion = 1,
		HasFamily = 2
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		public System.Collections.Generic.IEnumerable<ITrainerPackage> packageHints;

		public TrainerEmbeddedReleaseId releaseId;

		public GtiStatusProvider _003C_003E4__this;

		public Func<IRescuedCatalogPackage, ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string>> _003C_003E9__12;

		internal bool _003CGetInstallerListingForRelease_003Eb__2(ITrainerPackage p)
		{
			return p.TrainerPackageId.EmbeddedReleaseCompositeId != releaseId;
		}

		internal System.Collections.Generic.IEnumerable<IRescuedCatalogPackage> _003CGetInstallerListingForRelease_003Eb__3(System.Collections.Generic.IList<IRescuedCatalogPackage> rescues)
		{
			return OrderListingTuples(Enumerable.Select<IRescuedCatalogPackage, ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string>>((System.Collections.Generic.IEnumerable<IRescuedCatalogPackage>)rescues, (Func<IRescuedCatalogPackage, ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string>>)((IRescuedCatalogPackage r) => new ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string>(r, DeterminePriority(releaseId, r.GtiName), r.GtiName))));
		}

		internal ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string> _003CGetInstallerListingForRelease_003Eb__12(IRescuedCatalogPackage r)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			return new ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string>(r, DeterminePriority(releaseId, r.GtiName), r.GtiName);
		}

		internal bool _003CGetInstallerListingForRelease_003Eb__6(ValueTuple<IRescuedCatalogPackage, ITrainerPackage> t)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return t.Item2?.TrainerPackageId.EmbeddedReleaseCompositeId == releaseId;
		}

		internal ValueTuple<GtiListingEntry, GtiListingSearchPriority, string> _003CGetInstallerListingForRelease_003Eb__10(GtiListingEntry e)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			return new ValueTuple<GtiListingEntry, GtiListingSearchPriority, string>(e, DeterminePriority(releaseId, e.EntryName), e.EntryName);
		}

		internal bool _003CGetInstallerListingForRelease_003Eb__16(ValueTuple<GtiListingEntry, ITrainerPackage> t)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return t.Item2?.TrainerPackageId.EmbeddedReleaseCompositeId == releaseId;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetInstallerListingForRelease_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ITrainerInstallerListingInfo> _003C_003Et__builder;

		public System.Collections.Generic.IEnumerable<ITrainerPackage> packageHints;

		public TrainerEmbeddedReleaseId releaseId;

		public GtiStatusProvider _003C_003E4__this;

		private _003C_003Ec__DisplayClass5_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<GtiListingEntry> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			GtiStatusProvider gtiStatusProvider = _003C_003E4__this;
			ITrainerInstallerListingInfo result2;
			try
			{
				ITrainerInstallerListingInfo orAdd = default(ITrainerInstallerListingInfo);
				ConfiguredTaskAwaiter<GtiListingEntry> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass5_0();
					_003C_003E8__1.packageHints = packageHints;
					_003C_003E8__1.releaseId = releaseId;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					if (gtiStatusProvider._statusCache.TryGetValue(_003C_003E8__1.releaseId, ref orAdd))
					{
						goto IL_0112;
					}
					awaiter = _003C_003E8__1._003CGetInstallerListingForRelease_003Eg__SearchForMatchingEntryOrNull_007C0().DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<GtiListingEntry>, _003CGetInstallerListingForRelease_003Ed__5>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<GtiListingEntry>);
					num = (_003C_003E1__state = -1);
				}
				GtiListingEntry result = awaiter.GetResult();
				ITrainerInstallerListingInfo trainerInstallerListingInfo2;
				if (result != null)
				{
					ITrainerInstallerListingInfo trainerInstallerListingInfo = new GtiListingStatus(result);
					trainerInstallerListingInfo2 = trainerInstallerListingInfo;
				}
				else
				{
					ITrainerInstallerListingInfo trainerInstallerListingInfo = new NotListedInfo();
					trainerInstallerListingInfo2 = trainerInstallerListingInfo;
				}
				ITrainerInstallerListingInfo trainerInstallerListingInfo3 = trainerInstallerListingInfo2;
				orAdd = gtiStatusProvider._statusCache.GetOrAdd(_003C_003E8__1.releaseId, trainerInstallerListingInfo3);
				goto IL_0112;
				IL_0112:
				result2 = orAdd;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly IGtiListingProvider _gtiProvider;

	private readonly ITrainerPackageParser _packageParser;

	private readonly ICatalogPackageRefuge _packageRefuge;

	private readonly ConcurrentDictionary<TrainerEmbeddedReleaseId, ITrainerInstallerListingInfo> _statusCache = new ConcurrentDictionary<TrainerEmbeddedReleaseId, ITrainerInstallerListingInfo>();

	public GtiStatusProvider(IGtiListingProvider gtiProvider, ITrainerPackageParser parser, ICatalogPackageRefuge catalogPackageRefuge)
	{
		_gtiProvider = gtiProvider;
		_packageParser = parser;
		_packageRefuge = catalogPackageRefuge;
	}

	[AsyncStateMachine(typeof(_003CGetInstallerListingForRelease_003Ed__5))]
	public async System.Threading.Tasks.Task<ITrainerInstallerListingInfo> GetInstallerListingForRelease(TrainerEmbeddedReleaseId releaseId, System.Collections.Generic.IEnumerable<ITrainerPackage> packageHints)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		System.Collections.Generic.IEnumerable<ITrainerPackage> packageHints2 = packageHints;
		TrainerEmbeddedReleaseId releaseId2 = releaseId;
		ITrainerInstallerListingInfo orAdd = default(ITrainerInstallerListingInfo);
		if (!_statusCache.TryGetValue(releaseId2, ref orAdd))
		{
			GtiListingEntry gtiListingEntry = await SearchForMatchingEntryOrNull().DetachContext();
			ITrainerInstallerListingInfo trainerInstallerListingInfo2;
			if (gtiListingEntry != null)
			{
				ITrainerInstallerListingInfo trainerInstallerListingInfo = new GtiListingStatus(gtiListingEntry);
				trainerInstallerListingInfo2 = trainerInstallerListingInfo;
			}
			else
			{
				ITrainerInstallerListingInfo trainerInstallerListingInfo = new NotListedInfo();
				trainerInstallerListingInfo2 = trainerInstallerListingInfo;
			}
			ITrainerInstallerListingInfo trainerInstallerListingInfo3 = trainerInstallerListingInfo2;
			orAdd = _statusCache.GetOrAdd(releaseId2, trainerInstallerListingInfo3);
		}
		return orAdd;
		[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass5_0._003C_003CGetInstallerListingForRelease_003Eg__SearchForMatchingEntryOrNull_007C0_003Ed))]
		async System.Threading.Tasks.Task<GtiListingEntry> SearchForMatchingEntryOrNull()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<string, ITrainerPackage> packagesByPath = Enumerable.ToDictionary<ITrainerPackage, string>(packageHints2, (Func<ITrainerPackage, string>)((ITrainerPackage p) => p.BaseInfo.PackageRootPath), (IEqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash);
			if (Enumerable.Any<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)packagesByPath.Values, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.TrainerPackageId.EmbeddedReleaseCompositeId != releaseId2)))
			{
				throw new ArgumentException("All packages must be for the specified release ID.", "packageHints");
			}
			System.Threading.Tasks.Task<string> t2 = _packageRefuge.ListRescuedPackages().ToList<IRescuedCatalogPackage>().Select<System.Collections.Generic.IList<IRescuedCatalogPackage>, System.Collections.Generic.IEnumerable<IRescuedCatalogPackage>>((Func<System.Collections.Generic.IList<IRescuedCatalogPackage>, System.Collections.Generic.IEnumerable<IRescuedCatalogPackage>>)((System.Collections.Generic.IList<IRescuedCatalogPackage> rescues) => OrderListingTuples(Enumerable.Select<IRescuedCatalogPackage, ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string>>((System.Collections.Generic.IEnumerable<IRescuedCatalogPackage>)rescues, (Func<IRescuedCatalogPackage, ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string>>)((IRescuedCatalogPackage r) => new ValueTuple<IRescuedCatalogPackage, GtiListingSearchPriority, string>(r, DeterminePriority(releaseId2, r.GtiName), r.GtiName))))))
				.Flatten<IRescuedCatalogPackage>()
				.Select<IRescuedCatalogPackage, IObservable<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>>((Func<IRescuedCatalogPackage, IObservable<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>>)((IRescuedCatalogPackage rescued) => Observable.FromAsync<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>((Func<System.Threading.Tasks.Task<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass5_2._003C_003CGetInstallerListingForRelease_003Eb__13_003Ed))] async () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					IRescuedCatalogPackage rescuedCatalogPackage = rescued;
					return new ValueTuple<IRescuedCatalogPackage, ITrainerPackage>(rescuedCatalogPackage, await GetPackageOrPartialPackage(rescued.RescuedDirectory).DetachContext());
				}))))
				.Window<IObservable<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>>(2)
				.Select<IObservable<IObservable<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>>, IObservable<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>>((Func<IObservable<IObservable<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>>, IObservable<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>>)((IObservable<IObservable<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>> window) => window.Merge<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>()))
				.Concat<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>()
				.FirstOrDefaultAsync<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>>((ValueTuple<IRescuedCatalogPackage, ITrainerPackage> t) => t.Item2?.TrainerPackageId.EmbeddedReleaseCompositeId == releaseId2)
				.Select<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>, string>((Func<ValueTuple<IRescuedCatalogPackage, ITrainerPackage>, string>)((ValueTuple<IRescuedCatalogPackage, ITrainerPackage> t) => t.Item1?.GtiName))
				.ToTask<string>();
			System.Threading.Tasks.Task<IDictionary<string, GtiListingEntry>> listEntriesTask = _gtiProvider.GtiListEntries().Where<GtiListingEntry>((Func<GtiListingEntry, bool>)((GtiListingEntry e) => !e.IsTheCatalog)).ToDictionary<GtiListingEntry, string>((GtiListingEntry e) => e.EntryName)
				.ToTask<IDictionary<string, GtiListingEntry>>();
			string recordedGtiName = await t2.DetachContext();
			string expectedGtiName = GetExpectedGtiName(releaseId2);
			IDictionary<string, GtiListingEntry> val = await listEntriesTask.DetachContext();
			GtiListingEntry result = default(GtiListingEntry);
			if (val.TryGetValue(recordedGtiName ?? expectedGtiName, ref result) || val.TryGetValue(expectedGtiName, ref result))
			{
				return result;
			}
			return await OrderListingTuples(Enumerable.Select<GtiListingEntry, ValueTuple<GtiListingEntry, GtiListingSearchPriority, string>>((System.Collections.Generic.IEnumerable<GtiListingEntry>)val.Values, (Func<GtiListingEntry, ValueTuple<GtiListingEntry, GtiListingSearchPriority, string>>)((GtiListingEntry e) => new ValueTuple<GtiListingEntry, GtiListingSearchPriority, string>(e, DeterminePriority(releaseId2, e.EntryName), e.EntryName)))).ToObservable().Select<GtiListingEntry, IObservable<ValueTuple<GtiListingEntry, ITrainerPackage>>>((Func<GtiListingEntry, IObservable<ValueTuple<GtiListingEntry, ITrainerPackage>>>)((GtiListingEntry listEntry) => Observable.FromAsync<ValueTuple<GtiListingEntry, ITrainerPackage>>((Func<System.Threading.Tasks.Task<ValueTuple<GtiListingEntry, ITrainerPackage>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass5_3._003C_003CGetInstallerListingForRelease_003Eb__18_003Ed))] async () =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ITrainerPackage trainerPackage = null;
				if (listEntry.InstalledPath != null)
				{
					trainerPackage = await GetPackageOrPartialPackage(PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.InstalledByGti, listEntry.InstalledPath)).DetachContext();
				}
				if (trainerPackage == null && !listEntry.IsDiscontinued && listEntry.CatalogedPath != null)
				{
					trainerPackage = await GetPackageOrPartialPackage(PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, listEntry.CatalogedPath)).DetachContext();
				}
				return new ValueTuple<GtiListingEntry, ITrainerPackage>(listEntry, trainerPackage);
			})))).Window<IObservable<ValueTuple<GtiListingEntry, ITrainerPackage>>>(2)
				.Select<IObservable<IObservable<ValueTuple<GtiListingEntry, ITrainerPackage>>>, IObservable<ValueTuple<GtiListingEntry, ITrainerPackage>>>((Func<IObservable<IObservable<ValueTuple<GtiListingEntry, ITrainerPackage>>>, IObservable<ValueTuple<GtiListingEntry, ITrainerPackage>>>)((IObservable<IObservable<ValueTuple<GtiListingEntry, ITrainerPackage>>> window) => window.Merge<ValueTuple<GtiListingEntry, ITrainerPackage>>()))
				.Concat<ValueTuple<GtiListingEntry, ITrainerPackage>>()
				.FirstOrDefaultAsync<ValueTuple<GtiListingEntry, ITrainerPackage>>((ValueTuple<GtiListingEntry, ITrainerPackage> t) => t.Item2?.TrainerPackageId.EmbeddedReleaseCompositeId == releaseId2)
				.Select<ValueTuple<GtiListingEntry, ITrainerPackage>, GtiListingEntry>((Func<ValueTuple<GtiListingEntry, ITrainerPackage>, GtiListingEntry>)((ValueTuple<GtiListingEntry, ITrainerPackage> t) => t.Item1));
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass5_1._003C_003CGetInstallerListingForRelease_003Eg__GetPackageOrPartialPackage_007C11_003Ed))]
			async System.Threading.Tasks.Task<ITrainerPackage?> GetPackageOrPartialPackage(PotentialPackageDirectory potentialPackageDirectory)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ITrainerPackage result2 = default(ITrainerPackage);
				if (packagesByPath.TryGetValue(potentialPackageDirectory.PackageDirectory.FullPath, ref result2))
				{
					return result2;
				}
				try
				{
					return await _packageParser.ParsePackage(potentialPackageDirectory).DetachContext();
				}
				catch (TrainerPackageLoadException ex)
				{
					return ex.PartialPackage;
				}
			}
		}
	}

	public void NotifyOfCatalogUpdate(Version catalogVersion)
	{
		_statusCache.Clear();
	}

	private static GtiListingSearchPriority DeterminePriority(TrainerEmbeddedReleaseId releaseId, string gtiName)
	{
		string gtiName2 = gtiName;
		TrainerEmbeddedReleaseId releaseId2 = releaseId;
		return FamilyFlag() | VersionFlag();
		[CompilerGenerated]
		GtiListingSearchPriority FamilyFlag()
		{
			if (!gtiName2.Contains(releaseId2.FamilyId.IdentStringChecked, (StringComparison)4))
			{
				return GtiListingSearchPriority.None;
			}
			return GtiListingSearchPriority.HasFamily;
		}
		[CompilerGenerated]
		GtiListingSearchPriority VersionFlag()
		{
			if (!gtiName2.Contains(releaseId2.VersionId.IdentStringChecked, (StringComparison)4))
			{
				return GtiListingSearchPriority.None;
			}
			return GtiListingSearchPriority.HasVersion;
		}
	}

	private static System.Collections.Generic.IEnumerable<T> OrderListingTuples<T>(System.Collections.Generic.IEnumerable<ValueTuple<T, GtiListingSearchPriority, string>> source)
	{
		return Enumerable.Select<ValueTuple<T, GtiListingSearchPriority, string>, T>((System.Collections.Generic.IEnumerable<ValueTuple<T, GtiListingSearchPriority, string>>)Enumerable.ThenBy<ValueTuple<T, GtiListingSearchPriority, string>, string>(Enumerable.OrderByDescending<ValueTuple<T, GtiListingSearchPriority, string>, GtiListingSearchPriority>(source, (Func<ValueTuple<T, GtiListingSearchPriority, string>, GtiListingSearchPriority>)((ValueTuple<T, GtiListingSearchPriority, string> t) => t.Item2)), (Func<ValueTuple<T, GtiListingSearchPriority, string>, string>)((ValueTuple<T, GtiListingSearchPriority, string> t) => t.Item3)), (Func<ValueTuple<T, GtiListingSearchPriority, string>, T>)((ValueTuple<T, GtiListingSearchPriority, string> t) => t.Item1));
	}

	private static string GetExpectedGtiName(TrainerEmbeddedReleaseId releaseId)
	{
		return $"GTI__{releaseId.FamilyId}__{releaseId.VersionId}";
	}
}
