using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.FileSystem;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;
using Nito.AsyncEx;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class PowerStatsRepo : IPowerStatsRepo
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass10_0
	{
		[StructLayout(3)]
		private struct _003C_003CIncrementPowerUpsForPanel_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<KeyValuePair<SimAircraftKey, SimPowerStats>> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass10_0 _003C_003E4__this;

			public SimAircraftKey simulator;

			private string _003CjsonPath_003E5__2;

			private SimPowerStats _003Cstats_003E5__3;

			private ConfiguredTaskAwaiter<SimPowerStats> _003C_003Eu__1;

			private ConfiguredTaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_0169: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_0218: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass10_0 _003C_003Ec__DisplayClass10_ = _003C_003E4__this;
				KeyValuePair<SimAircraftKey, SimPowerStats> result;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					SimPowerStats simPowerStats;
					ConfiguredTaskAwaiter<SimPowerStats> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_01ba;
						}
						_003CjsonPath_003E5__2 = Path.Combine(_003C_003Ec__DisplayClass10_._003C_003E4__this.RootDirectory, simulator.AircraftIdentifier.IdentStringChecked, simulator.InstallationKey.ToStandardRelativePath().RelPath, "stats.json");
						if (!_003C_003Ec__DisplayClass10_._003C_003E4__this._lowLevel.FileSystem.PathExists(_003CjsonPath_003E5__2))
						{
							simPowerStats = new SimPowerStats();
							goto IL_0105;
						}
						awaiter2 = _003C_003Ec__DisplayClass10_._003C_003E4__this._lowLevel.ObjectPersistence.Load<SimPowerStats>(_003CjsonPath_003E5__2).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPowerStats>, _003C_003CIncrementPowerUpsForPanel_003Eb__1_003Ed>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPowerStats>);
						num = (_003C_003E1__state = -1);
					}
					simPowerStats = awaiter2.GetResult();
					goto IL_0105;
					IL_01ba:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					result = new KeyValuePair<SimAircraftKey, SimPowerStats>(simulator, _003Cstats_003E5__3);
					goto end_IL_000e;
					IL_0105:
					_003Cstats_003E5__3 = simPowerStats;
					_003Cstats_003E5__3 = _003Cstats_003E5__3.IncrementBoth();
					_003C_003Ec__DisplayClass10_._003C_003E4__this._lowLevel.FileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(_003CjsonPath_003E5__2));
					ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass10_._003C_003E4__this._lowLevel.ObjectPersistence.Commit(_003Cstats_003E5__3, _003CjsonPath_003E5__2).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CIncrementPowerUpsForPanel_003Eb__1_003Ed>(ref awaiter, ref this);
						return;
					}
					goto IL_01ba;
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CjsonPath_003E5__2 = null;
					_003Cstats_003E5__3 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CjsonPath_003E5__2 = null;
				_003Cstats_003E5__3 = null;
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public SimPanel panel;

		public PowerStatsRepo _003C_003E4__this;

		internal SimAircraftKey _003CIncrementPowerUpsForPanel_003Eb__0(SimInstallKey s)
		{
			return new SimAircraftKey(panel.AircraftIdentifier, s);
		}

		[AsyncStateMachine(typeof(_003C_003CIncrementPowerUpsForPanel_003Eb__1_003Ed))]
		internal async System.Threading.Tasks.Task<KeyValuePair<SimAircraftKey, SimPowerStats>> _003CIncrementPowerUpsForPanel_003Eb__1(SimAircraftKey simulator)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string jsonPath = Path.Combine(_003C_003E4__this.RootDirectory, simulator.AircraftIdentifier.IdentStringChecked, simulator.InstallationKey.ToStandardRelativePath().RelPath, "stats.json");
			SimPowerStats simPowerStats = ((!_003C_003E4__this._lowLevel.FileSystem.PathExists(jsonPath)) ? new SimPowerStats() : (await _003C_003E4__this._lowLevel.ObjectPersistence.Load<SimPowerStats>(jsonPath).DetachContext()));
			SimPowerStats stats2 = simPowerStats;
			stats2 = stats2.IncrementBoth();
			_003C_003E4__this._lowLevel.FileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(jsonPath));
			await _003C_003E4__this._lowLevel.ObjectPersistence.Commit(stats2, jsonPath).DetachContext();
			return new KeyValuePair<SimAircraftKey, SimPowerStats>(simulator, stats2);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{
		public AircraftIdentifier aircraft;

		public string aircraftPath;

		internal SimAircraftKey _003CClearAllStatsForAircraft_003Eb__1(FileSystemEntry fse)
		{
			return new SimAircraftKey(aircraft, SimInstallKey.Parse(PathHelpers.CreateRelativePath(aircraftPath, LauncherPathHelpers.NonRootPathParentDirectory(fse.FullPath))));
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CClearAllStatsForAircraft_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftIdentifier aircraft;

		public PowerStatsRepo _003C_003E4__this;

		private _003C_003Ec__DisplayClass8_0 _003C_003E8__1;

		private List<SimAircraftKey> _003CdeletedStats_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PowerStatsRepo powerStatsRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass8_0();
					_003C_003E8__1.aircraft = aircraft;
					_003C_003E8__1.aircraft.ThrowIfDefault();
					_003CdeletedStats_003E5__2 = new List<SimAircraftKey>();
					awaiter = powerStatsRepo._directoryLock.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CClearAllStatsForAircraft_003Ed__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
				}
				System.IDisposable result = awaiter.GetResult();
				try
				{
					_003C_003E8__1.aircraftPath = Path.Combine(powerStatsRepo.RootDirectory, _003C_003E8__1.aircraft.IdentStringChecked);
					if (powerStatsRepo._lowLevel.FileSystem.PathExists(_003C_003E8__1.aircraftPath))
					{
						try
						{
							_003CdeletedStats_003E5__2 = Enumerable.ToList<SimAircraftKey>(Enumerable.Select<FileSystemEntry, SimAircraftKey>(Enumerable.Where<FileSystemEntry>(powerStatsRepo._lowLevel.FileSystem.RecursiveFilesIn(_003C_003E8__1.aircraftPath), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => fse.Name == "stats.json")), (Func<FileSystemEntry, SimAircraftKey>)((FileSystemEntry fse) => new SimAircraftKey(_003C_003E8__1.aircraft, SimInstallKey.Parse(PathHelpers.CreateRelativePath(_003C_003E8__1.aircraftPath, LauncherPathHelpers.NonRootPathParentDirectory(fse.FullPath)))))));
							powerStatsRepo._lowLevel.FileSystem.DeleteIfExists(_003C_003E8__1.aircraftPath);
						}
						catch (DirectoryNotFoundException)
						{
						}
					}
				}
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
					}
				}
				ISubject<AircraftIdentifier> deleted = powerStatsRepo._deleted;
				bool flag = false;
				try
				{
					Monitor.Enter((object)deleted, ref flag);
					((IObserver<AircraftIdentifier>)powerStatsRepo._deleted).OnNext(_003C_003E8__1.aircraft);
				}
				finally
				{
					if (num < 0 && flag)
					{
						Monitor.Exit((object)deleted);
					}
				}
				Enumerator<SimAircraftKey> enumerator = _003CdeletedStats_003E5__2.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						SimAircraftKey current = enumerator.Current;
						powerStatsRepo.PostUpdate(current, null);
					}
				}
				finally
				{
					if (num < 0)
					{
						((System.IDisposable)enumerator).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CdeletedStats_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CdeletedStats_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetPowerStats_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SimPowerStats> _003C_003Et__builder;

		public SimAircraftKey simulator;

		public PowerStatsRepo _003C_003E4__this;

		private System.IDisposable _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<SimPowerStats> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PowerStatsRepo powerStatsRepo = _003C_003E4__this;
			SimPowerStats result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_009e;
					}
					simulator.ThrowIfInvalid();
					awaiter = powerStatsRepo._directoryLock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetPowerStats_003Ed__9>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
				}
				System.IDisposable result = awaiter.GetResult();
				_003C_003E7__wrap1 = result;
				goto IL_009e;
				IL_009e:
				try
				{
					ConfiguredTaskAwaiter<SimPowerStats> awaiter2;
					if (num == 1)
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPowerStats>);
						num = (_003C_003E1__state = -1);
						goto IL_0168;
					}
					string path = Path.Combine(powerStatsRepo.RootDirectory, simulator.AircraftIdentifier.IdentStringChecked, simulator.InstallationKey.ToStandardRelativePath().RelPath, "stats.json");
					if (powerStatsRepo._lowLevel.FileSystem.PathExists(path))
					{
						awaiter2 = powerStatsRepo._lowLevel.ObjectPersistence.Load<SimPowerStats>(path).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPowerStats>, _003CGetPowerStats_003Ed__9>(ref awaiter2, ref this);
							return;
						}
						goto IL_0168;
					}
					result2 = new SimPowerStats();
					goto end_IL_009e;
					IL_0168:
					result2 = awaiter2.GetResult();
					end_IL_009e:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap1 != null)
					{
						_003C_003E7__wrap1.Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CIncrementPowerUpsForPanel_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SimPanel panel;

		public PowerStatsRepo _003C_003E4__this;

		private _003C_003Ec__DisplayClass10_0 _003C_003E8__1;

		private List<SimAircraftKey> _003CactiveKeys_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<KeyValuePair<SimAircraftKey, SimPowerStats>[]> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PowerStatsRepo powerStatsRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_010f;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass10_0();
					_003C_003E8__1.panel = panel;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.panel.AircraftIdentifier.ThrowIfDefault();
					_003CactiveKeys_003E5__2 = Enumerable.ToList<SimAircraftKey>(Enumerable.Select<SimInstallKey, SimAircraftKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)_003C_003E8__1.panel.ActiveSimKeys, (Func<SimInstallKey, SimAircraftKey>)((SimInstallKey s) => new SimAircraftKey(_003C_003E8__1.panel.AircraftIdentifier, s))));
					awaiter = powerStatsRepo._directoryLock.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CIncrementPowerUpsForPanel_003Ed__10>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
				}
				System.IDisposable result = awaiter.GetResult();
				_003C_003E7__wrap2 = result;
				goto IL_010f;
				IL_010f:
				Dictionary<SimAircraftKey, SimPowerStats> val;
				try
				{
					ConfiguredTaskAwaiter<KeyValuePair<SimAircraftKey, SimPowerStats>[]> awaiter2;
					if (num != 1)
					{
						awaiter2 = System.Threading.Tasks.Task.WhenAll<KeyValuePair<SimAircraftKey, SimPowerStats>>(Enumerable.Select<SimAircraftKey, System.Threading.Tasks.Task<KeyValuePair<SimAircraftKey, SimPowerStats>>>((System.Collections.Generic.IEnumerable<SimAircraftKey>)_003CactiveKeys_003E5__2, (Func<SimAircraftKey, System.Threading.Tasks.Task<KeyValuePair<SimAircraftKey, SimPowerStats>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass10_0._003C_003CIncrementPowerUpsForPanel_003Eb__1_003Ed))] async (SimAircraftKey simulator) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							string jsonPath = Path.Combine(_003C_003E8__1._003C_003E4__this.RootDirectory, simulator.AircraftIdentifier.IdentStringChecked, simulator.InstallationKey.ToStandardRelativePath().RelPath, "stats.json");
							SimPowerStats simPowerStats = ((!_003C_003E8__1._003C_003E4__this._lowLevel.FileSystem.PathExists(jsonPath)) ? new SimPowerStats() : (await _003C_003E8__1._003C_003E4__this._lowLevel.ObjectPersistence.Load<SimPowerStats>(jsonPath).DetachContext()));
							SimPowerStats stats2 = simPowerStats;
							stats2 = stats2.IncrementBoth();
							_003C_003E8__1._003C_003E4__this._lowLevel.FileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(jsonPath));
							await _003C_003E8__1._003C_003E4__this._lowLevel.ObjectPersistence.Commit(stats2, jsonPath).DetachContext();
							return new KeyValuePair<SimAircraftKey, SimPowerStats>(simulator, stats2);
						}))).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<KeyValuePair<SimAircraftKey, SimPowerStats>[]>, _003CIncrementPowerUpsForPanel_003Ed__10>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<KeyValuePair<SimAircraftKey, SimPowerStats>[]>);
						num = (_003C_003E1__state = -1);
					}
					val = awaiter2.GetResult().ToDictionary();
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				Enumerator<SimAircraftKey, SimPowerStats> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<SimAircraftKey, SimPowerStats> current = enumerator.Current;
						powerStatsRepo.PostUpdate(current.Key, current.Value);
					}
				}
				finally
				{
					if (num < 0)
					{
						((System.IDisposable)enumerator).Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CactiveKeys_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CactiveKeys_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CResetPowerUps_003Ed__11 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SimAircraftKey simulator;

		public PowerStatsRepo _003C_003E4__this;

		private SimPowerStats _003Cstats_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private string _003CjsonPath_003E5__4;

		private ConfiguredTaskAwaiter<SimPowerStats> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PowerStatsRepo powerStatsRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_009d;
					}
					simulator.ThrowIfInvalid();
					awaiter = powerStatsRepo._directoryLock.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CResetPowerUps_003Ed__11>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
				}
				System.IDisposable result = awaiter.GetResult();
				_003C_003E7__wrap2 = result;
				goto IL_009d;
				IL_009d:
				try
				{
					ConfiguredTaskAwaiter<SimPowerStats> awaiter2;
					if (num == 1)
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPowerStats>);
						num = (_003C_003E1__state = -1);
						goto IL_0177;
					}
					ConfiguredTaskAwaiter awaiter3;
					if (num == 2)
					{
						awaiter3 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_025a;
					}
					_003CjsonPath_003E5__4 = Path.Combine(powerStatsRepo.RootDirectory, simulator.AircraftIdentifier.IdentStringChecked, simulator.InstallationKey.ToStandardRelativePath().RelPath, "stats.json");
					if (powerStatsRepo._lowLevel.FileSystem.PathExists(_003CjsonPath_003E5__4))
					{
						awaiter2 = powerStatsRepo._lowLevel.ObjectPersistence.Load<SimPowerStats>(_003CjsonPath_003E5__4).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPowerStats>, _003CResetPowerUps_003Ed__11>(ref awaiter2, ref this);
							return;
						}
						goto IL_0177;
					}
					goto end_IL_009d;
					IL_0261:
					_003CjsonPath_003E5__4 = null;
					goto IL_0282;
					IL_0177:
					SimPowerStats result2 = awaiter2.GetResult();
					_003Cstats_003E5__2 = result2;
					if (_003Cstats_003E5__2.PowerUpsSinceReset != 0)
					{
						_003Cstats_003E5__2 = _003Cstats_003E5__2.With(delegate(IPropertyChangeSet<SimPowerStats> _)
						{
							//IL_0021: Unknown result type (might be due to invalid IL or missing references)
							//IL_002b: Expected O, but got Unknown
							ParameterExpression val2 = Expression.Parameter(typeof(SimPowerStats), "s");
							_.Mutate(Expression.Lambda<Func<SimPowerStats, int>>((Expression)(object)Expression.Property((Expression)(object)val2, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val2 }), 0);
						});
						powerStatsRepo._lowLevel.FileSystem.CreateDirectory(LauncherPathHelpers.NonRootPathParentDirectory(_003CjsonPath_003E5__4));
						ConfiguredTaskAwaitable val = powerStatsRepo._lowLevel.ObjectPersistence.Commit(_003Cstats_003E5__2, _003CjsonPath_003E5__4).DetachContext();
						awaiter3 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter3;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CResetPowerUps_003Ed__11>(ref awaiter3, ref this);
							return;
						}
						goto IL_025a;
					}
					goto IL_0261;
					IL_025a:
					((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
					goto IL_0261;
					end_IL_009d:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				goto end_IL_000e;
				IL_0282:
				_003C_003E7__wrap2 = null;
				powerStatsRepo.PostUpdate(simulator, _003Cstats_003E5__2);
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cstats_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cstats_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private readonly ILowLevelDal _lowLevel;

	private readonly ISubject<KeyValuePair<SimAircraftKey, SimPowerStats?>> _statChanges = new Subject<KeyValuePair<SimAircraftKey, SimPowerStats>>();

	private readonly ISubject<AircraftIdentifier> _deleted = new Subject<AircraftIdentifier>();

	private readonly AsyncReaderWriterLock _directoryLock = new AsyncReaderWriterLock();

	[field: CompilerGenerated]
	protected string RootDirectory
	{
		[CompilerGenerated]
		get;
	}

	public PowerStatsRepo(ILowLevelDal lowlevel)
	{
		_lowLevel = lowlevel;
		RootDirectory = Path.Combine(lowlevel.Deployment.UserAppDataDirPath, "PowerStats");
	}

	[AsyncStateMachine(typeof(_003CClearAllStatsForAircraft_003Ed__8))]
	public System.Threading.Tasks.Task ClearAllStatsForAircraft(AircraftIdentifier aircraft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CClearAllStatsForAircraft_003Ed__8 _003CClearAllStatsForAircraft_003Ed__ = default(_003CClearAllStatsForAircraft_003Ed__8);
		_003CClearAllStatsForAircraft_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CClearAllStatsForAircraft_003Ed__._003C_003E4__this = this;
		_003CClearAllStatsForAircraft_003Ed__.aircraft = aircraft;
		_003CClearAllStatsForAircraft_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CClearAllStatsForAircraft_003Ed__._003C_003Et__builder)).Start<_003CClearAllStatsForAircraft_003Ed__8>(ref _003CClearAllStatsForAircraft_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CClearAllStatsForAircraft_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CGetPowerStats_003Ed__9))]
	public async System.Threading.Tasks.Task<SimPowerStats> GetPowerStats(SimAircraftKey simulator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		simulator.ThrowIfInvalid();
		using (await _directoryLock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			string path = Path.Combine(RootDirectory, simulator.AircraftIdentifier.IdentStringChecked, simulator.InstallationKey.ToStandardRelativePath().RelPath, "stats.json");
			if (!_lowLevel.FileSystem.PathExists(path))
			{
				return new SimPowerStats();
			}
			return await _lowLevel.ObjectPersistence.Load<SimPowerStats>(path).DetachContext();
		}
	}

	[AsyncStateMachine(typeof(_003CIncrementPowerUpsForPanel_003Ed__10))]
	public System.Threading.Tasks.Task IncrementPowerUpsForPanel(SimPanel panel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CIncrementPowerUpsForPanel_003Ed__10 _003CIncrementPowerUpsForPanel_003Ed__ = default(_003CIncrementPowerUpsForPanel_003Ed__10);
		_003CIncrementPowerUpsForPanel_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CIncrementPowerUpsForPanel_003Ed__._003C_003E4__this = this;
		_003CIncrementPowerUpsForPanel_003Ed__.panel = panel;
		_003CIncrementPowerUpsForPanel_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CIncrementPowerUpsForPanel_003Ed__._003C_003Et__builder)).Start<_003CIncrementPowerUpsForPanel_003Ed__10>(ref _003CIncrementPowerUpsForPanel_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CIncrementPowerUpsForPanel_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CResetPowerUps_003Ed__11))]
	public System.Threading.Tasks.Task ResetPowerUps(SimAircraftKey simulator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CResetPowerUps_003Ed__11 _003CResetPowerUps_003Ed__ = default(_003CResetPowerUps_003Ed__11);
		_003CResetPowerUps_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CResetPowerUps_003Ed__._003C_003E4__this = this;
		_003CResetPowerUps_003Ed__.simulator = simulator;
		_003CResetPowerUps_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CResetPowerUps_003Ed__._003C_003Et__builder)).Start<_003CResetPowerUps_003Ed__11>(ref _003CResetPowerUps_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CResetPowerUps_003Ed__._003C_003Et__builder)).Task;
	}

	public IObservable<SimPowerStats> WhenSimulatorPowerStatsChange(SimAircraftKey simulator)
	{
		SimAircraftKey simulator2 = simulator;
		simulator2.ThrowIfInvalid();
		return Observable.FromAsync<SimPowerStats>((Func<System.Threading.Tasks.Task<SimPowerStats>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass12_0._003C_003CWhenSimulatorPowerStatsChange_003Eb__0_003Ed))] async () => await GetPowerStats(simulator2).DetachContext())).Concat<SimPowerStats>(((IObservable<KeyValuePair<SimAircraftKey, SimPowerStats>>)_statChanges).Where<KeyValuePair<SimAircraftKey, SimPowerStats>>((Func<KeyValuePair<SimAircraftKey, SimPowerStats>, bool>)((KeyValuePair<SimAircraftKey, SimPowerStats> kv) => kv.Value != null && kv.Key == simulator2)).Select<KeyValuePair<SimAircraftKey, SimPowerStats>, SimPowerStats>((Func<KeyValuePair<SimAircraftKey, SimPowerStats>, SimPowerStats>)((KeyValuePair<SimAircraftKey, SimPowerStats> kv) => kv.Value)).TakeUntil<SimPowerStats, AircraftIdentifier>(((IObservable<AircraftIdentifier>)_deleted).Where<AircraftIdentifier>((Func<AircraftIdentifier, bool>)((AircraftIdentifier aircraft) => aircraft == simulator2.AircraftIdentifier))));
	}

	private void PostUpdate(SimAircraftKey simulator, SimPowerStats? stats)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		lock (_statChanges)
		{
			((IObserver<KeyValuePair<SimAircraftKey, SimPowerStats>>)_statChanges).OnNext(new KeyValuePair<SimAircraftKey, SimPowerStats>(simulator, stats));
		}
	}
}
