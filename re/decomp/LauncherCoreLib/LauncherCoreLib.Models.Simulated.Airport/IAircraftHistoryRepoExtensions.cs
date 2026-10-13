using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.FileSystem;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherCoreLib.Models.Simulated.Airport;

public static class IAircraftHistoryRepoExtensions
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_0
	{
		public string tempDir;

		internal string _003CRevertAircraftToSnapshot_003Eb__4(SimInstallKey k)
		{
			return Path.Combine(tempDir, k.SerializedKey + ".gca");
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_1
	{
		public System.Collections.Generic.IList<SimInstallKey> winfMounts;

		internal bool _003CRevertAircraftToSnapshot_003Eb__5(KeyValuePair<SimInstallKey, string> kv)
		{
			return ((System.Collections.Generic.ICollection<SimInstallKey>)winfMounts).Contains(kv.Key);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CRevertAircraftToSnapshot_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IAircraftHistoryRepo repo;

		public AircraftIdentifier aircraftIdentifier;

		public SnapshotIdentifier snapshotIdentifier;

		public IUnitOfWork targetUnitOfWork;

		private _003C_003Ec__DisplayClass0_1 _003C_003E8__1;

		private _003C_003Ec__DisplayClass0_0 _003C_003E8__2;

		private TemporaryHangarDirectory _003CtempHangar_003E5__2;

		private ConfiguredTaskAwaiter<TemporaryHangarDirectory> _003C_003Eu__1;

		private List<SimInstallKey> _003CwinfsMountsToDelete_003E5__3;

		private List<SimInstallKey> _003CwinfsMountsToCopy_003E5__4;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private Dictionary<SimInstallKey, string> _003CmountMap_003E5__5;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>> _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter<TemporaryHangarDirectory> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 4u)
					{
						goto IL_008e;
					}
					awaiter = repo.MountHistoricalSnapshot(aircraftIdentifier, snapshotIdentifier).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TemporaryHangarDirectory>, _003CRevertAircraftToSnapshot_003Ed__0>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TemporaryHangarDirectory>);
					num = (_003C_003E1__state = -1);
				}
				TemporaryHangarDirectory result = awaiter.GetResult();
				_003CtempHangar_003E5__2 = result;
				goto IL_008e;
				IL_008e:
				try
				{
					ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter3;
					ConfiguredTaskAwaiter awaiter2;
					ConfiguredTaskAwaitable val2;
					AircraftSnapshotData result2;
					ILookup<bool, SimInstallData> val;
					string text;
					switch (num)
					{
					default:
						awaiter3 = _003CtempHangar_003E5__2.GetAircraftSnapshotById(aircraftIdentifier).ConfigureAwait(false).GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter3;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CRevertAircraftToSnapshot_003Ed__0>(ref awaiter3, ref this);
							return;
						}
						goto IL_0116;
					case 1:
						awaiter3 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
						num = (_003C_003E1__state = -1);
						goto IL_0116;
					case 2:
						awaiter2 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0231;
					case 3:
						awaiter2 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_02bd;
					case 4:
					case 5:
						{
							try
							{
								ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>> awaiter4;
								if (num != 4)
								{
									if (num == 5)
									{
										awaiter2 = _003C_003Eu__3;
										_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
										num = (_003C_003E1__state = -1);
										goto IL_04b8;
									}
									_003C_003E8__1 = new _003C_003Ec__DisplayClass0_1();
									targetUnitOfWork.LowLevel.FileSystem.CreateDirectory(_003C_003E8__2.tempDir);
									awaiter4 = _003CtempHangar_003E5__2.GetAircraftWinfsArchives(aircraftIdentifier, _003CmountMap_003E5__5).ConfigureAwait(false).GetAwaiter();
									if (!awaiter4.IsCompleted)
									{
										num = (_003C_003E1__state = 4);
										_003C_003Eu__4 = awaiter4;
										((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>>, _003CRevertAircraftToSnapshot_003Ed__0>(ref awaiter4, ref this);
										return;
									}
								}
								else
								{
									awaiter4 = _003C_003Eu__4;
									_003C_003Eu__4 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>>);
									num = (_003C_003E1__state = -1);
								}
								System.Collections.Generic.IList<SimInstallKey> result3 = awaiter4.GetResult();
								_003C_003E8__1.winfMounts = result3;
								Dictionary<SimInstallKey, string> simWinfsArchiveInputMap = Enumerable.Where<KeyValuePair<SimInstallKey, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, string>>)_003CmountMap_003E5__5, (Func<KeyValuePair<SimInstallKey, string>, bool>)((KeyValuePair<SimInstallKey, string> kv) => ((System.Collections.Generic.ICollection<SimInstallKey>)_003C_003E8__1.winfMounts).Contains(kv.Key))).ToDictionary();
								val2 = targetUnitOfWork.Simulators.UpdateAircraftWinfsArchives(aircraftIdentifier, simWinfsArchiveInputMap).ConfigureAwait(false);
								awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
								if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
								{
									num = (_003C_003E1__state = 5);
									_003C_003Eu__3 = awaiter2;
									((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRevertAircraftToSnapshot_003Ed__0>(ref awaiter2, ref this);
									return;
								}
								goto IL_04b8;
								IL_04b8:
								((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
								_003C_003E8__1 = null;
							}
							finally
							{
								if (num < 0)
								{
									targetUnitOfWork.LowLevel.FileSystem.DeleteIfExistsAndParentsThatAreEmpty(_003C_003E8__2.tempDir, targetUnitOfWork.LowLevel.Deployment.AppTempDirectoryPath);
								}
							}
							_003C_003E8__2 = null;
							_003CmountMap_003E5__5 = null;
							break;
						}
						IL_02bd:
						((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
						goto IL_02c4;
						IL_0116:
						result2 = awaiter3.GetResult();
						val = Enumerable.ToLookup<SimInstallData, bool>(result2.Panel.AllSimulators.Values, (Func<SimInstallData, bool>)((SimInstallData s) => s.HasArchivedWinfsMount));
						_003CwinfsMountsToDelete_003E5__3 = Enumerable.ToList<SimInstallKey>(Enumerable.Select<SimInstallData, SimInstallKey>(val[false], (Func<SimInstallData, SimInstallKey>)((SimInstallData sim) => sim.AircraftSimKey.InstallationKey)));
						_003CwinfsMountsToCopy_003E5__4 = Enumerable.ToList<SimInstallKey>(Enumerable.Select<SimInstallData, SimInstallKey>(val[true], (Func<SimInstallData, SimInstallKey>)((SimInstallData sim) => sim.InstallationKey)));
						val2 = targetUnitOfWork.Aircraft.UpdateAircraft(result2).ConfigureAwait(false);
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRevertAircraftToSnapshot_003Ed__0>(ref awaiter2, ref this);
							return;
						}
						goto IL_0231;
						IL_02c4:
						if (_003CwinfsMountsToCopy_003E5__4.Count <= 0)
						{
							break;
						}
						_003C_003E8__2 = new _003C_003Ec__DisplayClass0_0();
						text = Path.Combine(targetUnitOfWork.LowLevel.Deployment.PerProcessTempDirectory(), "Winfs_rollback");
						_003C_003E8__2.tempDir = Path.Combine(text, snapshotIdentifier.Serialized);
						_003CmountMap_003E5__5 = Enumerable.ToDictionary<SimInstallKey, SimInstallKey, string>((System.Collections.Generic.IEnumerable<SimInstallKey>)_003CwinfsMountsToCopy_003E5__4, (Func<SimInstallKey, SimInstallKey>)((SimInstallKey k) => k), (Func<SimInstallKey, string>)((SimInstallKey k) => Path.Combine(_003C_003E8__2.tempDir, k.SerializedKey + ".gca")));
						goto case 4;
						IL_0231:
						((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
						if (_003CwinfsMountsToDelete_003E5__3.Count > 0)
						{
							val2 = targetUnitOfWork.Simulators.DeleteAircraftWinfsArchives(aircraftIdentifier, (System.Collections.Generic.IEnumerable<SimInstallKey>)_003CwinfsMountsToDelete_003E5__3).ConfigureAwait(false);
							awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
							{
								num = (_003C_003E1__state = 3);
								_003C_003Eu__3 = awaiter2;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRevertAircraftToSnapshot_003Ed__0>(ref awaiter2, ref this);
								return;
							}
							goto IL_02bd;
						}
						goto IL_02c4;
					}
					_003CwinfsMountsToDelete_003E5__3 = null;
					_003CwinfsMountsToCopy_003E5__4 = null;
				}
				finally
				{
					if (num < 0 && _003CtempHangar_003E5__2 != null)
					{
						((System.IDisposable)_003CtempHangar_003E5__2).Dispose();
					}
				}
				_003CtempHangar_003E5__2 = null;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	[AsyncStateMachine(typeof(_003CRevertAircraftToSnapshot_003Ed__0))]
	public static System.Threading.Tasks.Task RevertAircraftToSnapshot(this IAircraftHistoryRepo repo, AircraftIdentifier aircraftIdentifier, SnapshotIdentifier snapshotIdentifier, IUnitOfWork targetUnitOfWork)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CRevertAircraftToSnapshot_003Ed__0 _003CRevertAircraftToSnapshot_003Ed__ = default(_003CRevertAircraftToSnapshot_003Ed__0);
		_003CRevertAircraftToSnapshot_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CRevertAircraftToSnapshot_003Ed__.repo = repo;
		_003CRevertAircraftToSnapshot_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CRevertAircraftToSnapshot_003Ed__.snapshotIdentifier = snapshotIdentifier;
		_003CRevertAircraftToSnapshot_003Ed__.targetUnitOfWork = targetUnitOfWork;
		_003CRevertAircraftToSnapshot_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CRevertAircraftToSnapshot_003Ed__._003C_003Et__builder)).Start<_003CRevertAircraftToSnapshot_003Ed__0>(ref _003CRevertAircraftToSnapshot_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CRevertAircraftToSnapshot_003Ed__._003C_003Et__builder)).Task;
	}
}
