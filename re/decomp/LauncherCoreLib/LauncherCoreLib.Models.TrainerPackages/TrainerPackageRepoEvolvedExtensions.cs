using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class TrainerPackageRepoEvolvedExtensions
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{
		public TrainerPackageIdentifier trainerPackageIdentifier;

		internal bool _003CGetPackageById_003Eb__0(ITrainerPackage p)
		{
			return p.TrainerPackageId == trainerPackageIdentifier;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CAfterLoading_003Ed__6<T> : IAsyncStateMachine where T : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

		public ITrainerPackageRepoEvolved repo;

		public Func<ITrainerPackageRepoEvolved, System.Threading.Tasks.Task<T>> accessor;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<T> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			T result;
			try
			{
				ConfiguredTaskAwaiter<T> awaiter;
				ConfiguredTaskAwaiter awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<T>);
						num = (_003C_003E1__state = -1);
						goto IL_00e1;
					}
					ConfiguredTaskAwaitable val = repo.LoadingComplete.DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAfterLoading_003Ed__6<T>>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				awaiter = ((Func<ITrainerPackageRepoEvolved, System.Threading.Tasks.Task<ITrainerPackageRepoEvolved>>)(object)accessor).Invoke(repo).DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<T>, _003CAfterLoading_003Ed__6<T>>(ref awaiter, ref this);
					return;
				}
				goto IL_00e1;
				IL_00e1:
				result = awaiter.GetResult();
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetPackageById_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ITrainerPackage> _003C_003Et__builder;

		public TrainerPackageIdentifier trainerPackageIdentifier;

		public ITrainerPackageRepoEvolved repo;

		private _003C_003Ec__DisplayClass8_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<PackageInstallationInfo> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			ITrainerPackage result;
			try
			{
				ConfiguredTaskAwaiter<PackageInstallationInfo> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass8_0();
					_003C_003E8__1.trainerPackageIdentifier = trainerPackageIdentifier;
					if ((object)_003C_003E8__1.trainerPackageIdentifier == null)
					{
						throw new ArgumentNullException("trainerPackageIdentifier");
					}
					awaiter = repo.GetPackageByReleaseId(_003C_003E8__1.trainerPackageIdentifier.EmbeddedReleaseCompositeId).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PackageInstallationInfo>, _003CGetPackageById_003Ed__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PackageInstallationInfo>);
					num = (_003C_003E1__state = -1);
				}
				result = Enumerable.FirstOrDefault<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)awaiter.GetResult().Packages, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.TrainerPackageId == _003C_003E8__1.trainerPackageIdentifier));
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
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	public static IObservable<ITrainerPackage> WhenAPackageIsRegistered(this ITrainerPackageRepoEvolved repo)
	{
		return repo.WhenPackageIsChanged.FilterByChangeType<ITrainerPackage>(new DataChangeType[2]
		{
			DataChangeType.Created,
			DataChangeType.Updated
		}).StripChangeType<ITrainerPackage>();
	}

	public static IObservable<ITrainerPackage> WhenAPackageIsUnRegistered(this ITrainerPackageRepoEvolved repo)
	{
		return repo.WhenPackageIsChanged.FilterByChangeType<ITrainerPackage>(new DataChangeType[1] { DataChangeType.Deleted }).StripChangeType<ITrainerPackage>();
	}

	public static IObservable<ITrainerPackage> FilterByExactPackageId(this IObservable<ITrainerPackage> packages, TrainerPackageIdentifier trainerPackageIdentifier)
	{
		TrainerPackageIdentifier trainerPackageIdentifier2 = trainerPackageIdentifier;
		return packages.Where<ITrainerPackage>((Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.TrainerPackageId == trainerPackageIdentifier2));
	}

	public static IObservable<ITrainerPackage> FilterByEmbeddedReleaseLine(this IObservable<ITrainerPackage?> packages, TrainerEmbeddedReleaseId embeddedReleaseId)
	{
		TrainerEmbeddedReleaseId embeddedReleaseId2 = embeddedReleaseId;
		return packages.Where<ITrainerPackage>((Func<ITrainerPackage, bool>)((ITrainerPackage p) => p != null && p.TrainerPackageId.EmbeddedReleaseCompositeId == embeddedReleaseId2));
	}

	public static IObservable<ITrainerPackage?> FilterByEmbeddedReleaseLineOrNull(this IObservable<ITrainerPackage?> packages, TrainerEmbeddedReleaseId embeddedReleaseId)
	{
		TrainerEmbeddedReleaseId embeddedReleaseId2 = embeddedReleaseId;
		return packages.Where<ITrainerPackage>((Func<ITrainerPackage, bool>)((ITrainerPackage p) => p == null || p.TrainerPackageId.EmbeddedReleaseCompositeId == embeddedReleaseId2));
	}

	public static IObservable<T> AfterLoading<T>(this ITrainerPackageRepoEvolved repo, Func<ITrainerPackageRepoEvolved, IObservable<T>> accessor)
	{
		return ((System.IDisposable)repo.LoadingComplete).ToObservable().IgnoreElements<Unit>().SelectDefault<Unit, T>(default(T))
			.Concat<T>(accessor.Invoke(repo));
	}

	[AsyncStateMachine(typeof(_003CAfterLoading_003Ed__6<>))]
	public static async System.Threading.Tasks.Task<T> AfterLoading<T>(this ITrainerPackageRepoEvolved repo, Func<ITrainerPackageRepoEvolved, System.Threading.Tasks.Task<T>> accessor)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		await repo.LoadingComplete.DetachContext();
		return await accessor.Invoke(repo).DetachContext();
	}

	public static IObservable<ITrainerPackage> Packages(this ITrainerPackageRepoEvolved repo)
	{
		ITrainerPackageRepoEvolved repo2 = repo;
		return (from p in repo2.GetAllPackages()
			group p by p.TrainerPackageId.EmbeddedReleaseCompositeId).SelectMany<IGroupedObservable<TrainerEmbeddedReleaseId, ITrainerPackage>, ITrainerPackage>((Func<IGroupedObservable<TrainerEmbeddedReleaseId, ITrainerPackage>, System.Threading.Tasks.Task<ITrainerPackage>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass7_0._003C_003CPackages_003Eb__1_003Ed))] async (IGroupedObservable<TrainerEmbeddedReleaseId, ITrainerPackage> groupedByEmbeddedRelease) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			PackageInstallationInfo packageInstallationInfo = await repo2.GetPackageByReleaseId(groupedByEmbeddedRelease.Key).DetachContext();
			return packageInstallationInfo.IsRelevantToUser ? packageInstallationInfo.PrimaryPackage : null;
		})).WhereClassNotNull<ITrainerPackage>();
	}

	[AsyncStateMachine(typeof(_003CGetPackageById_003Ed__8))]
	public static async System.Threading.Tasks.Task<ITrainerPackage?> GetPackageById(this ITrainerPackageRepoEvolved repo, TrainerPackageIdentifier trainerPackageIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TrainerPackageIdentifier trainerPackageIdentifier2 = trainerPackageIdentifier;
		if ((object)trainerPackageIdentifier2 == null)
		{
			throw new ArgumentNullException("trainerPackageIdentifier");
		}
		return Enumerable.FirstOrDefault<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)(await repo.GetPackageByReleaseId(trainerPackageIdentifier2.EmbeddedReleaseCompositeId).DetachContext()).Packages, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.TrainerPackageId == trainerPackageIdentifier2));
	}

	public static IObservable<ISet<TrainerEmbeddedReleaseId>> GetEmbeddedReleasesSnapShots(this ITrainerPackageRepoEvolved repo)
	{
		ITrainerPackageRepoEvolved repo2 = repo;
		return (IObservable<ISet<TrainerEmbeddedReleaseId>>)(object)TimingEvent.StartStreamAndConcat<DataChangeNotification<ITrainerPackage>>(repo2.WhenPackageIsChanged).Select<TimingEvent, IObservable<HashSet<TrainerEmbeddedReleaseId>>>((Func<TimingEvent, IObservable<HashSet<TrainerEmbeddedReleaseId>>>)((TimingEvent _) => repo2.GetAllPackages().Select<ITrainerPackage, TrainerEmbeddedReleaseId>((Func<ITrainerPackage, TrainerEmbeddedReleaseId>)((ITrainerPackage p) => p.TrainerPackageId.EmbeddedReleaseCompositeId)).ToHashSet<TrainerEmbeddedReleaseId>())).Switch<HashSet<TrainerEmbeddedReleaseId>>();
	}

	public static IObservable<ISet<TrainerEmbeddedReleaseId>> GetAllEmbeddedReleasesOverTime(this ITrainerPackageRepoEvolved repo)
	{
		return repo.GetEmbeddedReleasesSnapShots().DistinctUntilChanged<ISet<TrainerEmbeddedReleaseId>>((IEqualityComparer<ISet<TrainerEmbeddedReleaseId>>)(object)new SetEquality<TrainerEmbeddedReleaseId>());
	}

	public static IObservable<TrainerEmbeddedReleaseId> GetEmbeddedReleasesOverTime(this ITrainerPackageRepoEvolved repo)
	{
		ITrainerPackageRepoEvolved repo2 = repo;
		return Observable.Defer<TrainerEmbeddedReleaseId>((Func<IObservable<TrainerEmbeddedReleaseId>>)delegate
		{
			HashSet<TrainerEmbeddedReleaseId> seenBefore = new HashSet<TrainerEmbeddedReleaseId>();
			return repo2.GetAllPackages().Merge<ITrainerPackage>(repo2.WhenAPackageIsRegistered()).Select<ITrainerPackage, TrainerEmbeddedReleaseId>((Func<ITrainerPackage, TrainerEmbeddedReleaseId>)((ITrainerPackage package) => package.TrainerPackageId.EmbeddedReleaseCompositeId))
				.Where<TrainerEmbeddedReleaseId>((Func<TrainerEmbeddedReleaseId, bool>)((TrainerEmbeddedReleaseId releaseId) => !seenBefore.Contains(releaseId)))
				.Do<TrainerEmbeddedReleaseId>((Action<TrainerEmbeddedReleaseId>)delegate(TrainerEmbeddedReleaseId releaseId)
				{
					seenBefore.Add(releaseId);
				});
		});
	}

	public static IObservable<IDictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>> GetAllInstallInfoOverTime(this ITrainerPackageRepoEvolved repo)
	{
		ITrainerPackageRepoEvolved repo2 = repo;
		return (IObservable<IDictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>>)(object)repo2.GetEmbeddedReleasesSnapShots().Select<ISet<TrainerEmbeddedReleaseId>, System.Threading.Tasks.Task<Dictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>>>((Func<ISet<TrainerEmbeddedReleaseId>, System.Threading.Tasks.Task<Dictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass12_0._003C_003CGetAllInstallInfoOverTime_003Eb__0_003Ed))] async (ISet<TrainerEmbeddedReleaseId> releases) => Enumerable.ToDictionary<PackageInstallationInfo, TrainerEmbeddedReleaseId>((System.Collections.Generic.IEnumerable<PackageInstallationInfo>)(await System.Threading.Tasks.Task.WhenAll<PackageInstallationInfo>(Enumerable.Select<TrainerEmbeddedReleaseId, System.Threading.Tasks.Task<PackageInstallationInfo>>((System.Collections.Generic.IEnumerable<TrainerEmbeddedReleaseId>)releases, (Func<TrainerEmbeddedReleaseId, System.Threading.Tasks.Task<PackageInstallationInfo>>)((TrainerEmbeddedReleaseId r) => repo2.GetPackageByReleaseId(r)))).DetachContext()), (Func<PackageInstallationInfo, TrainerEmbeddedReleaseId>)((PackageInstallationInfo ii) => ii.EmbeddedReleaseId)))).Switch<Dictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>>();
	}

	public static IObservable<ISet<ITrainerPackage>> GetPrimaryPackagesOverTime(this ITrainerPackageRepoEvolved repo)
	{
		ITrainerPackageRepoEvolved repo2 = repo;
		return ((IObservable<ISet<ITrainerPackage>>)(object)TimingEvent.StartStreamAndConcat<DataChangeNotification<ITrainerPackage>>(repo2.WhenPackageIsChanged).Select<TimingEvent, IObservable<HashSet<ITrainerPackage>>>((Func<TimingEvent, IObservable<HashSet<ITrainerPackage>>>)((TimingEvent _) => (from p in repo2.GetAllPackages().SelectMany<ITrainerPackage, ITrainerPackage>((Func<ITrainerPackage, System.Threading.Tasks.Task<ITrainerPackage>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass13_0._003C_003CGetPrimaryPackagesOverTime_003Eb__1_003Ed))] async (ITrainerPackage p) => await repo2.GetPackageById(p.TrainerPackageId).DetachContext())).WhereClassNotNull<ITrainerPackage>()
			group p by p.TrainerPackageId.EmbeddedReleaseCompositeId).SelectMany<IGroupedObservable<TrainerEmbeddedReleaseId, ITrainerPackage>, ITrainerPackage>((Func<IGroupedObservable<TrainerEmbeddedReleaseId, ITrainerPackage>, IObservable<ITrainerPackage>>)((IGroupedObservable<TrainerEmbeddedReleaseId, ITrainerPackage> g) => ((IObservable<ITrainerPackage>)g).LastAsync<ITrainerPackage>())).ToHashSet<ITrainerPackage>())).Switch<HashSet<ITrainerPackage>>()).DistinctUntilChanged<ISet<ITrainerPackage>>((IEqualityComparer<ISet<ITrainerPackage>>)(object)new SetEquality<ITrainerPackage>());
	}

	public static IObservable<ITrainerPackage?> GetPackageLineage(this ITrainerPackageRepoEvolved repo, TrainerEmbeddedReleaseId embeddedReleaseLine)
	{
		ITrainerPackageRepoEvolved repo2 = repo;
		TrainerEmbeddedReleaseId embeddedReleaseLine2 = embeddedReleaseLine;
		return TimingEvent.StartStreamAndConcat<ITrainerPackage>(repo2.WhenAPackageIsRegistered().Merge<ITrainerPackage>(repo2.WhenAPackageIsUnRegistered()).FilterByEmbeddedReleaseLine(embeddedReleaseLine2)).Select<TimingEvent, IObservable<ITrainerPackage>>((Func<TimingEvent, IObservable<ITrainerPackage>>)((TimingEvent _) => repo2.GetPrimaryPackageForEmbeddedReleaseOrNull(embeddedReleaseLine2))).Switch<ITrainerPackage>()
			.FilterByEmbeddedReleaseLineOrNull(embeddedReleaseLine2)
			.DistinctUntilChanged<ITrainerPackage>();
	}

	public static IObservable<ITrainerPackage?> GetExactPackageOverTime(this ITrainerPackageRepoEvolved repo, TrainerPackageIdentifier trainerPackageIdentifier)
	{
		ITrainerPackageRepoEvolved repo2 = repo;
		TrainerPackageIdentifier trainerPackageIdentifier2 = trainerPackageIdentifier;
		return TimingEvent.StartStreamAndConcat<ITrainerPackage>(repo2.WhenPackageIsChanged.StripChangeType<ITrainerPackage>().FilterByExactPackageId(trainerPackageIdentifier2)).Select<TimingEvent, System.Threading.Tasks.Task<ITrainerPackage>>((Func<TimingEvent, System.Threading.Tasks.Task<ITrainerPackage>>)((TimingEvent updatedPackageOfExactId) => repo2.GetPackageById(trainerPackageIdentifier2))).Switch<ITrainerPackage>()
			.Select<ITrainerPackage, ITrainerPackage>((Func<ITrainerPackage, ITrainerPackage>)((ITrainerPackage p) => (p == null || !(p.TrainerPackageId == trainerPackageIdentifier2)) ? null : p))
			.DistinctUntilChanged<ITrainerPackage>();
	}

	private static IObservable<ITrainerPackage?> GetPrimaryPackageForEmbeddedReleaseOrNull(this ITrainerPackageRepoEvolved repo, TrainerEmbeddedReleaseId embeddedRelease)
	{
		ITrainerPackageRepoEvolved repo2 = repo;
		TrainerEmbeddedReleaseId embeddedRelease2 = embeddedRelease;
		return Observable.FromAsync<ITrainerPackage>((Func<System.Threading.Tasks.Task<ITrainerPackage>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass16_0._003C_003CGetPrimaryPackageForEmbeddedReleaseOrNull_003Eb__0_003Ed))] async () => (await repo2.GetPackageByReleaseId(embeddedRelease2).DetachContext()).PrimaryPackage));
	}
}
