using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public static class AircraftRepoEvolvedExtensions
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetLastSelectedAircraftOrDefault_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftIdentifier> _003C_003Et__builder;

		public IAircraftRepoEvolved repo;

		private ConfiguredTaskAwaiter<AircraftIdentifier> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0085: Expected O, but got Unknown
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AircraftIdentifier result;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<AircraftIdentifier> awaiter;
					if (num != 0)
					{
						awaiter = repo.GetLastSelectedAircraft().ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftIdentifier>, _003CGetLastSelectedAircraftOrDefault_003Ed__0>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftIdentifier>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
				}
				catch (SerializationException val)
				{
					Logger.LogFormattedException((System.Exception)val, "Retrieving the selected aircraft.");
					result = AircraftIdentifier.Default;
				}
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

	[AsyncStateMachine(typeof(_003CGetLastSelectedAircraftOrDefault_003Ed__0))]
	public static async System.Threading.Tasks.Task<AircraftIdentifier> GetLastSelectedAircraftOrDefault(this IAircraftRepoEvolved repo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return await repo.GetLastSelectedAircraft().ConfigureAwait(false);
		}
		catch (SerializationException val)
		{
			Logger.LogFormattedException((System.Exception)val, "Retrieving the selected aircraft.");
			return AircraftIdentifier.Default;
		}
	}

	public static IObservable<AircraftIdentifier> ContinualSelectedAircraftOrDefault(this IAircraftRepoEvolved repo)
	{
		IAircraftRepoEvolved repo2 = repo;
		return Observable.FromAsync<AircraftIdentifier>((Func<System.Threading.Tasks.Task<AircraftIdentifier>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass1_0._003C_003CContinualSelectedAircraftOrDefault_003Eb__0_003Ed))] async () => await repo2.GetLastSelectedAircraftOrDefault().ConfigureAwait(false))).Concat<AircraftIdentifier>(repo2.WhenSelectedAircraftChanges).DistinctUntilChanged<AircraftIdentifier>();
	}

	public static IObservable<DataChangeNotification<AircraftIdentifier>> ForAircraft(this IObservable<DataChangeNotification<AircraftIdentifier>> aircraftUpdates, AircraftIdentifier identifier)
	{
		return aircraftUpdates.Where<DataChangeNotification<AircraftIdentifier>>((Func<DataChangeNotification<AircraftIdentifier>, bool>)((DataChangeNotification<AircraftIdentifier> change) => change.Value == identifier));
	}

	public static IObservable<AircraftSnapshotData> AircraftUsingPackage(this IAircraftRepoEvolved repo, TrainerPackageIdentifier packageId)
	{
		IAircraftRepoEvolved repo2 = repo;
		TrainerPackageIdentifier packageId2 = packageId;
		return repo2.GetAllAircraftListData().SelectMany<AircraftListData, AircraftSnapshotData>((Func<AircraftListData, System.Threading.Tasks.Task<AircraftSnapshotData>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass3_0._003C_003CAircraftUsingPackage_003Eb__0_003Ed))] async (AircraftListData listData) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				return await repo2.GetAircraftSnapshotById(listData.AircraftIdentifier).DetachContext();
			}
			catch (RepoItemNotFoundException)
			{
				return (AircraftSnapshotData)null;
			}
		})).WhereClassNotNull<AircraftSnapshotData>()
			.Where<AircraftSnapshotData>((Func<AircraftSnapshotData, bool>)((AircraftSnapshotData snapShot) => Enumerable.Any<SimInstallData>(snapShot.Panel.AllSimulators.Values, (Func<SimInstallData, bool>)((SimInstallData s) => s.FullPackageId != null && s.FullPackageId.IsSameEmbeddedRelease(packageId2)))));
	}

	public static IObservable<AircraftSnapshotData> AircraftActivelyUsingPackage(this IAircraftRepoEvolved repo, TrainerPackageIdentifier packageId)
	{
		IAircraftRepoEvolved repo2 = repo;
		TrainerPackageIdentifier packageId2 = packageId;
		return repo2.GetAllAircraftListData().SelectMany<AircraftListData, AircraftSnapshotData>((Func<AircraftListData, System.Threading.Tasks.Task<AircraftSnapshotData>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass4_0._003C_003CAircraftActivelyUsingPackage_003Eb__0_003Ed))] async (AircraftListData listData) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				return await repo2.GetAircraftSnapshotById(listData.AircraftIdentifier).DetachContext();
			}
			catch (RepoItemNotFoundException)
			{
				return (AircraftSnapshotData)null;
			}
		})).WhereClassNotNull<AircraftSnapshotData>()
			.Where<AircraftSnapshotData>((Func<AircraftSnapshotData, bool>)((AircraftSnapshotData snapShot) => Enumerable.Any<SimInstallData>(snapShot.Panel.ActiveSims, (Func<SimInstallData, bool>)((SimInstallData s) => s.FullPackageIdOrThrow().IsSameEmbeddedRelease(packageId2)))));
	}
}
