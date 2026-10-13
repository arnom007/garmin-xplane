using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ByteSizeLib;
using LOLA.Compression;
using LOLA.Extensions;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.SoftConfig;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Persistence;
using LauncherCoreLib.Models.Persistence.Models;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Utility;
using Nito.AsyncEx;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class DirectoryBasedHangar : IAircraftRepoEvolved, IFlushable, ISimulatorRepository, IAircraftHistoryRepoV2, IAircraftHistoryRepo, IFamilyDataRepo, IHangarMigrationMonitor, System.IDisposable
{
	private delegate AwaitableDisposable<System.IDisposable> LockSelector(AsyncReaderWriterLock l);

	private sealed class FamilyIdMigration
	{
		[field: CompilerGenerated]
		public InstallationSlot Slot
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public FileSystemEntry DirectoryEntry
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public FullModelIdentifier FullModelId
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		public FamilyIdMigration(InstallationSlot slot, FileSystemEntry fse, FullModelIdentifier fullModelIdentifier)
		{
			Slot = slot;
			DirectoryEntry = fse;
			FullModelId = fullModelIdentifier;
		}

		public bool DirectoryMatchesFamilyIdInSimJson()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (DirectoryEntry == null)
			{
				throw new InvalidOperationException();
			}
			if (FullModelId == null)
			{
				throw new InvalidOperationException();
			}
			return DirectoryEntry.Name == ((object)FullModelId.TrainerPackageId.FamilyId).ToString();
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CAreMigrationsNeeded_003Eb__30_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

		public FileSystemEntry aircraftDirectory;

		public DirectoryBasedHangar _003C_003E4__this;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			bool result;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_00b8;
				}
				if (AircraftIdentifier.IsValidIdentifier(aircraftDirectory.Name))
				{
					_003CrepoInfo_003E5__2 = CreateRepoInfo(aircraftDirectory.FullPath);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CAreMigrationsNeeded_003Eb__30_0_003Ed>(ref awaiter, ref this);
						return;
					}
					goto IL_00b8;
				}
				result = false;
				goto end_IL_000e;
				IL_00b8:
				System.IDisposable result2 = awaiter.GetResult();
				try
				{
					result = directoryBasedHangar._fs.PathExists(_003CrepoInfo_003E5__2.RootPath) && directoryBasedHangar.NeedsAMigration(_003CrepoInfo_003E5__2.RootPath);
				}
				finally
				{
					if (num < 0)
					{
						result2?.Dispose();
					}
				}
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CrepoInfo_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrepoInfo_003E5__2 = null;
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
	private struct _003C_003CGetAircraftListDataById_003Eb__36_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftListData> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftRepoInfo repoInfo;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftListData result;
			try
			{
				ConfiguredTaskAwaiter<AircraftListData> awaiter;
				if (num != 0)
				{
					awaiter = directoryBasedHangar.GetListDataWithLock(repoInfo).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003C_003CGetAircraftListDataById_003Eb__36_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003C_003CGetAllAircraftFamilyDataWithLock_003Eb__83_1_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public FileSystemEntry releaseIdFileEntry;

		private ConfiguredTaskAwaiter<PersistedFamilyData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			ValueTuple<TrainerEmbeddedReleaseId, FamilyData> result2;
			try
			{
				ConfiguredTaskAwaiter<PersistedFamilyData> awaiter;
				if (num != 0)
				{
					awaiter = directoryBasedHangar._persistence.Load<PersistedFamilyData>(releaseIdFileEntry.FullPath).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedFamilyData>, _003C_003CGetAllAircraftFamilyDataWithLock_003Eb__83_1_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedFamilyData>);
					num = (_003C_003E1__state = -1);
				}
				PersistedFamilyData result = awaiter.GetResult();
				result2 = new ValueTuple<TrainerEmbeddedReleaseId, FamilyData>(new TrainerEmbeddedReleaseId(Path.GetFileNameWithoutExtension(releaseIdFileEntry.Name)), new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)result.Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value))));
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
	private struct _003C_003CGetAllAircraftListData_003Eb__35_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public IObserver<AircraftListData> o;

		public CancellationToken token;

		private _003C_003Ec__DisplayClass35_1 _003C_003E8__1;

		private _003C_003Ec__DisplayClass35_3 _003C_003E8__2;

		private _003C_003Ec__DisplayClass35_2 _003C_003E8__3;

		private _003C_003Ec__DisplayClass35_0 _003C_003E8__4;

		private List<System.Exception> _003CdataExceptions_003E5__2;

		private string _003ChangarDirectory_003E5__3;

		private ConfiguredTaskAwaiter<AircraftIdentifier> _003C_003Eu__1;

		private System.Collections.Generic.IEnumerator<FileSystemEntry> _003C_003E7__wrap3;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0549: Unknown result type (might be due to invalid IL or missing references)
			//IL_055d: Expected O, but got Unknown
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Expected O, but got Unknown
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0462: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<AircraftIdentifier> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_01d7;
					}
					_003C_003E8__4 = new _003C_003Ec__DisplayClass35_0();
					_003C_003E8__4._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003E8__4.o = o;
					_003C_003E8__4.token = token;
					_003C_003E8__4.internalOrIoExceptions = new List<System.Exception>();
					_003CdataExceptions_003E5__2 = new List<System.Exception>();
					string path = CS_0024_003C_003E8__locals0._deployment.OldWinfsDirectoryPath();
					try
					{
						if (CS_0024_003C_003E8__locals0._fs.PathExists(path))
						{
							Logger.Log(LogLevel.Info, "Removing old winfs directory. Sorry you'll have to start over. This is the simplest way to make sure old winfs directories are handled.");
							CS_0024_003C_003E8__locals0._fs.DeleteIfExists(path);
						}
					}
					catch (System.Exception ex)
					{
						Logger.LogFormattedException(ex, "Removing old Winfs directory.");
						_003C_003E8__4.internalOrIoExceptions.Add(ex);
					}
					_003ChangarDirectory_003E5__3 = CS_0024_003C_003E8__locals0.RootDirectory;
					if (!CS_0024_003C_003E8__locals0._fs.PathExists(_003ChangarDirectory_003E5__3))
					{
						goto IL_0535;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass35_1();
					_003C_003E8__1.CS_0024_003C_003E8__locals1 = _003C_003E8__4;
					awaiter = CS_0024_003C_003E8__locals0.GetLastSelectedAircraftOrDefault().ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftIdentifier>, _003C_003CGetAllAircraftListData_003Eb__35_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftIdentifier>);
					num = (_003C_003E1__state = -1);
				}
				AircraftIdentifier result = awaiter.GetResult();
				_003C_003E8__1.selectedId = result;
				IOrderedEnumerable<FileSystemEntry> val = Enumerable.ThenBy<FileSystemEntry, string>(Enumerable.OrderByDescending<FileSystemEntry, bool>(CS_0024_003C_003E8__locals0._fs.DirectoriesIn(_003ChangarDirectory_003E5__3), (Func<FileSystemEntry, bool>)((FileSystemEntry ad) => ad.Name == _003C_003E8__1.selectedId.IdentString)), (Func<FileSystemEntry, string>)((FileSystemEntry fse) => fse.Name));
				_003C_003E7__wrap3 = ((System.Collections.Generic.IEnumerable<FileSystemEntry>)val).GetEnumerator();
				goto IL_01d7;
				IL_01d7:
				try
				{
					if (num == 1)
					{
						goto IL_037f;
					}
					goto IL_04fd;
					IL_04fd:
					while (((System.Collections.IEnumerator)_003C_003E7__wrap3).MoveNext())
					{
						_003C_003E8__3 = new _003C_003Ec__DisplayClass35_2();
						_003C_003E8__3.CS_0024_003C_003E8__locals2 = _003C_003E8__1;
						_003C_003E8__3.aircraftDirectory = _003C_003E7__wrap3.Current;
						_003C_003E8__2 = new _003C_003Ec__DisplayClass35_3();
						if (((CancellationToken)(ref _003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.token)).IsCancellationRequested)
						{
							break;
						}
						if (_003C_003E8__3.aircraftDirectory.Name.StartsWith(".tmp_", (StringComparison)4))
						{
							long num2;
							try
							{
								num2 = Enumerable.Aggregate<long, long>(Enumerable.Select<FileSystemEntry, long>(CS_0024_003C_003E8__locals0._fs.RecursiveFilesIn(_003C_003E8__3.aircraftDirectory.FullPath), (Func<FileSystemEntry, long>)([CompilerGenerated] (FileSystemEntry fse) => CS_0024_003C_003E8__locals0._fs.StatIfExists(fse.FullPath).FileSize)), 0L, (Func<long, long, long>)((long p, long c) => p + c));
							}
							catch (System.Exception ex2) when (ex2 is FileNotFoundException || ex2 is DirectoryNotFoundException)
							{
								continue;
							}
							string text = ((object)ByteSize.FromBytes(num2)).ToString();
							Logger.Log(LogLevel.Warning, "Temporary aircraft directory found, could be transient or a stale one from a crash if it appears in multiple times.\n\tName: " + _003C_003E8__3.aircraftDirectory.Name + "\n\tSize: " + text);
							continue;
						}
						if (!AircraftIdentifier.IsValidIdentifier(_003C_003E8__3.aircraftDirectory.Name))
						{
							LogRepoMessage(LogLevel.Error, "List all aircraft", "{0} is not a valid aircraft directory name", _003C_003E8__3.aircraftDirectory.Name);
							continue;
						}
						_003C_003E8__2.aircraftDataErrors = new List<System.Exception>();
						goto IL_037f;
					}
					goto end_IL_01d7;
					IL_037f:
					try
					{
						ConfiguredTaskAwaiter awaiter2;
						if (num != 1)
						{
							ConfiguredTaskAwaitable val2 = ParseListData(delegate(AircraftListData ld)
							{
								_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.o.OnNext(ld);
							}, delegate(System.Exception dataError)
							{
								_003C_003E8__2.aircraftDataErrors.Add(dataError);
							}, delegate(System.Exception internalError)
							{
								_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.internalOrIoExceptions.Add(internalError);
							}).ConfigureAwait(false);
							awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CGetAllAircraftListData_003Eb__35_0_003Ed>(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					}
					finally
					{
						if (num < 0 && _003C_003E8__2.aircraftDataErrors.Count > 0)
						{
							_003CdataExceptions_003E5__2.Add((System.Exception)new AggregateException("Data errors for " + _003C_003E8__3.aircraftDirectory.FullPath, (System.Collections.Generic.IEnumerable<System.Exception>)_003C_003E8__2.aircraftDataErrors));
						}
					}
					_003C_003E8__2 = null;
					_003C_003E8__3 = null;
					goto IL_04fd;
					end_IL_01d7:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap3 != null)
					{
						((System.IDisposable)_003C_003E7__wrap3).Dispose();
					}
				}
				_003C_003E7__wrap3 = null;
				_003C_003E8__1 = null;
				goto IL_0535;
				IL_0535:
				if (_003CdataExceptions_003E5__2.Count > 0)
				{
					Logger.LogFormattedException((System.Exception)new AggregateException((System.Collections.Generic.IEnumerable<System.Exception>)_003CdataExceptions_003E5__2), "Aircraft data exceptions");
				}
				if (_003C_003E8__4.internalOrIoExceptions.Count > 0)
				{
					_003C_003E8__4.o.OnError((System.Exception)new AggregateException("Errors listing aircraft.", (System.Collections.Generic.IEnumerable<System.Exception>)_003C_003E8__4.internalOrIoExceptions));
				}
				else
				{
					_003C_003E8__4.o.OnCompleted();
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__4 = null;
				_003CdataExceptions_003E5__2 = null;
				_003ChangarDirectory_003E5__3 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__4 = null;
			_003CdataExceptions_003E5__2 = null;
			_003ChangarDirectory_003E5__3 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass35_2._003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed))]
			System.Threading.Tasks.Task ParseListData(Action<AircraftListData> onParsed, Action<System.Exception> onDataError, Action<System.Exception> onInternalError)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass35_2._003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed _003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed = default(_003C_003Ec__DisplayClass35_2._003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed);
				_003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed._003C_003E4__this = this;
				_003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed.onParsed = onParsed;
				_003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed.onDataError = onDataError;
				_003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed.onInternalError = onInternalError;
				_003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass35_2._003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed>(ref _003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CGetAllAircraftListData_003Eg__ParseListData_007C6_003Ed._003C_003Et__builder)).Task;
			}
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CGetAllFamilyDataForAircraft_003Eb__77_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<FamilyDataLookup> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftRepoInfo repoInfo;

		private ConfiguredTaskAwaiter<FamilyDataLookup> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			FamilyDataLookup result;
			try
			{
				ConfiguredTaskAwaiter<FamilyDataLookup> awaiter;
				if (num != 0)
				{
					awaiter = directoryBasedHangar.GetAllAircraftFamilyDataWithLock(repoInfo).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<FamilyDataLookup>, _003C_003CGetAllFamilyDataForAircraft_003Eb__77_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<FamilyDataLookup>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003C_003CParseAircraftGCA_003Eb__60_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier exportId;

		public string tempAircraftRootPath;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotData result;
			try
			{
				ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
				if (num != 0)
				{
					awaiter = directoryBasedHangar.GetAircraftSnapshotWithLock(exportId, CreateRepoInfo(tempAircraftRootPath)).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003C_003CParseAircraftGCA_003Eb__60_0_003Ed>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
					num = (_003C_003E1__state = -1);
				}
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

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass101_0
	{
		[StructLayout(3)]
		private struct _003C_003CGetPersistedPanelWithLock_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SimInstallData> _003C_003Et__builder;

			public string simDirPath;

			public _003C_003Ec__DisplayClass101_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter<PersistedSimulator> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass101_0 _003C_003Ec__DisplayClass101_ = _003C_003E4__this;
				SimInstallData result2;
				try
				{
					ConfiguredTaskAwaiter<PersistedSimulator> awaiter;
					if (num != 0)
					{
						string text = Path.Combine(simDirPath, "simulator.json");
						if (!_003C_003Ec__DisplayClass101_._003C_003E4__this._fs.PathExists(text))
						{
							throw new FileNotFoundException("Missing simulator data.", text);
						}
						awaiter = _003C_003Ec__DisplayClass101_._003C_003E4__this._persistence.Load<PersistedSimulator>(text).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedSimulator>, _003C_003CGetPersistedPanelWithLock_003Eb__1_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedSimulator>);
						num = (_003C_003E1__state = -1);
					}
					PersistedSimulator result = awaiter.GetResult();
					SimInstallKey simInstallKey = SimInstallKey.Parse(PathHelpers.CreateRelativePath(_003C_003Ec__DisplayClass101_.avionicsDirPath, simDirPath));
					result2 = new SimInstallData(new SimAircraftKey(_003C_003Ec__DisplayClass101_.aircraftIdent, simInstallKey), new FullModelIdentifier(SettingIdentifier.FromString(result.FullModelId)).TrainerPackageId, (System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>?)Enumerable.ToDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)result.Options, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> o) => new SettingIdentifier(o.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> o) => o.Value)), new ReadOnlyJsonDataSource(result.ModuleData), _003C_003Ec__DisplayClass101_._003C_003E4__this._fs.PathExists(Path.Combine(simDirPath, "winfs_mount.gca")), _003C_003Ec__DisplayClass101_.activeSimKeys.Contains(simInstallKey));
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

		public DirectoryBasedHangar _003C_003E4__this;

		public string avionicsDirPath;

		public AircraftIdentifier aircraftIdent;

		public HashSet<SimInstallKey> activeSimKeys;

		public System.Collections.Generic.IList<SimInstallData> allPersistedSimulators;

		[AsyncStateMachine(typeof(_003C_003CGetPersistedPanelWithLock_003Eb__1_003Ed))]
		internal async System.Threading.Tasks.Task<SimInstallData> _003CGetPersistedPanelWithLock_003Eb__1(string simDirPath)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string text = Path.Combine(simDirPath, "simulator.json");
			if (_003C_003E4__this._fs.PathExists(text))
			{
				PersistedSimulator persistedSimulator = await _003C_003E4__this._persistence.Load<PersistedSimulator>(text).DetachContext();
				SimInstallKey simInstallKey = SimInstallKey.Parse(PathHelpers.CreateRelativePath(avionicsDirPath, simDirPath));
				return new SimInstallData(new SimAircraftKey(aircraftIdent, simInstallKey), new FullModelIdentifier(SettingIdentifier.FromString(persistedSimulator.FullModelId)).TrainerPackageId, (System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>?)Enumerable.ToDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)persistedSimulator.Options, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> o) => new SettingIdentifier(o.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> o) => o.Value)), new ReadOnlyJsonDataSource(persistedSimulator.ModuleData), _003C_003E4__this._fs.PathExists(Path.Combine(simDirPath, "winfs_mount.gca")), activeSimKeys.Contains(simInstallKey));
			}
			throw new FileNotFoundException("Missing simulator data.", text);
		}

		internal bool _003CGetPersistedPanelWithLock_003Eb__0(SimInstallKey activeKey)
		{
			_003C_003Ec__DisplayClass101_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass101_1
			{
				activeKey = activeKey
			};
			return !Enumerable.Any<SimInstallData>((System.Collections.Generic.IEnumerable<SimInstallData>)allPersistedSimulators, (Func<SimInstallData, bool>)((SimInstallData ps) => ps.InstallationKey == CS_0024_003C_003E8__locals0.activeKey));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass101_1
	{
		public SimInstallKey activeKey;

		internal bool _003CGetPersistedPanelWithLock_003Eb__4(SimInstallData ps)
		{
			return ps.InstallationKey == activeKey;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass103_0
	{
		[StructLayout(3)]
		private struct _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass103_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass103_0 CS_0024_003C_003E8__locals0 = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						LogRepoMessage(LogLevel.Info, CS_0024_003C_003E8__locals0.repoAction, "{0} - Saving simulators.", CS_0024_003C_003E8__locals0.aircraftIdentifier);
						ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<KeyValuePair<string, PersistedSimulator>, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<KeyValuePair<string, PersistedSimulator>>)CS_0024_003C_003E8__locals0.updatedSimulatorJsonFiles, (Func<KeyValuePair<string, PersistedSimulator>, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed))] (KeyValuePair<string, PersistedSimulator> kv) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							//IL_0016: Unknown result type (might be due to invalid IL or missing references)
							//IL_0017: Unknown result type (might be due to invalid IL or missing references)
							_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed = default(_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed);
							_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
							_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed.kv = kv;
							_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003Et__builder)).Start<_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed>(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003Et__builder)).Task;
						}))).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					if (CS_0024_003C_003E8__locals0.removedSimulatorJsonFiles.Count > 0)
					{
						LogRepoMessage(LogLevel.Info, CS_0024_003C_003E8__locals0.repoAction, "{0} - Deleting simulators.", CS_0024_003C_003E8__locals0.aircraftIdentifier);
						Enumerator<string> enumerator = CS_0024_003C_003E8__locals0.removedSimulatorJsonFiles.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								string current = enumerator.Current;
								CS_0024_003C_003E8__locals0._003C_003E4__this._fs.DeleteIfExistsAndParentsThatAreEmpty(current, CS_0024_003C_003E8__locals0.repoInfo.CurrentAircraftRoot);
								LogRepoMessage(LogLevel.Info, CS_0024_003C_003E8__locals0.repoAction, "{0} - Deleted simulator: {1}", CS_0024_003C_003E8__locals0.aircraftIdentifier, PathHelpers.CreateRelativePath(CS_0024_003C_003E8__locals0.repoInfo.CurrentAvionicsRoot, current));
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

		[StructLayout(3)]
		private struct _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass103_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass103_0 _003C_003Ec__DisplayClass103_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						LogRepoMessage(LogLevel.Info, _003C_003Ec__DisplayClass103_.repoAction, "{0} - Saving aircraft panel.", _003C_003Ec__DisplayClass103_.aircraftIdentifier);
						string path = Path.Combine(_003C_003Ec__DisplayClass103_.repoInfo.CurrentAircraftRoot, "panel.json");
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass103_._003C_003E4__this._persistence.Commit(_003C_003Ec__DisplayClass103_.panelInfoToSave, path).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					LogRepoMessage(LogLevel.Info, _003C_003Ec__DisplayClass103_.repoAction, "{0} - Updated aircraft panel.", _003C_003Ec__DisplayClass103_.aircraftIdentifier);
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

		[StructLayout(3)]
		private struct _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass103_0 _003C_003E4__this;

			public KeyValuePair<string, PersistedSimulator> kv;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass103_0 _003C_003Ec__DisplayClass103_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass103_._003C_003E4__this._persistence.Commit(kv.Value, kv.Key).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		public AircraftRepoInfo repoInfo;

		public string repoAction;

		public AircraftIdentifier aircraftIdentifier;

		public List<KeyValuePair<string, PersistedSimulator>> updatedSimulatorJsonFiles;

		public DirectoryBasedHangar _003C_003E4__this;

		public List<string> removedSimulatorJsonFiles;

		public PersistedSimPanelInfo panelInfoToSave;

		public Func<KeyValuePair<string, PersistedSimulator>, System.Threading.Tasks.Task> _003C_003E9__4;

		internal KeyValuePair<string, PersistedSimulator> _003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__0(SimInstallData sim)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ConvertToPersistedSimulatorAndLocation(sim, repoInfo);
		}

		internal string _003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__1(SimInstallKey deletedKey)
		{
			return BuildSimJsonFilePath(deletedKey, repoInfo);
		}

		[AsyncStateMachine(typeof(_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed))]
		internal System.Threading.Tasks.Task _003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed = default(_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed);
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003E4__this = this;
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed>(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed))]
		internal System.Threading.Tasks.Task _003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4(KeyValuePair<string, PersistedSimulator> kv)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed = default(_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed);
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003E4__this = this;
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed.kv = kv;
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003Et__builder)).Start<_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed>(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__4_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed))]
		internal System.Threading.Tasks.Task _003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed = default(_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed);
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003E4__this = this;
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed>(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass130_0
	{
		public DirectoryBasedHangar _003C_003E4__this;

		public string avionicsDirectory;

		internal IObservable<Unit> _003CMigrateFamilyIdToSideBySideVersionId_003Eb__0(FileSystemEntry simDir)
		{
			return Observable.FromAsync((Func<CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass130_1
			{
				CS_0024_003C_003E8__locals1 = this,
				simDir = simDir
			}._003CMigrateFamilyIdToSideBySideVersionId_003Eb__1);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass130_1
	{
		[StructLayout(3)]
		private struct _003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass130_1 _003C_003E4__this;

			private string _003CsimDirPath_003E5__2;

			private bool _003CcanForget_003E5__3;

			private ConfiguredTaskAwaiter<PersistedSimulator> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass130_1 _003C_003Ec__DisplayClass130_ = _003C_003E4__this;
				try
				{
					if (num == 0)
					{
						goto IL_0056;
					}
					_003CsimDirPath_003E5__2 = _003C_003Ec__DisplayClass130_.simDir.FullPath;
					string path = Path.Combine(_003CsimDirPath_003E5__2, "simulator.json");
					if (_003C_003Ec__DisplayClass130_.CS_0024_003C_003E8__locals1._003C_003E4__this._fs.PathExists(path))
					{
						_003CcanForget_003E5__3 = false;
						goto IL_0056;
					}
					goto end_IL_000e;
					IL_0056:
					try
					{
						ConfiguredTaskAwaiter<PersistedSimulator> awaiter;
						if (num != 0)
						{
							awaiter = _003C_003Ec__DisplayClass130_.CS_0024_003C_003E8__locals1._003C_003E4__this._persistence.Load<PersistedSimulator>(path).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedSimulator>, _003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedSimulator>);
							num = (_003C_003E1__state = -1);
						}
						PersistedSimulator result = awaiter.GetResult();
						SimInstallKey simInstallKey = SimInstallKey.Parse(PathHelpers.CreateRelativePath(_003C_003Ec__DisplayClass130_.CS_0024_003C_003E8__locals1.avionicsDirectory, _003CsimDirPath_003E5__2));
						TrainerPackageIdentifier trainerPackageId = new FullModelIdentifier(result.FullModelId).TrainerPackageId;
						if (simInstallKey.ReleaseId != trainerPackageId.EmbeddedReleaseCompositeId)
						{
							_003CcanForget_003E5__3 = true;
						}
					}
					catch (System.Exception ex) when (ex is SerializationException || ex is FormatException)
					{
						_003CcanForget_003E5__3 = true;
					}
					if (_003CcanForget_003E5__3)
					{
						_003C_003Ec__DisplayClass130_.CS_0024_003C_003E8__locals1._003C_003E4__this._fs.DeleteIfExistsAndParentsThatAreEmpty(_003CsimDirPath_003E5__2, _003C_003Ec__DisplayClass130_.CS_0024_003C_003E8__locals1.avionicsDirectory);
					}
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CsimDirPath_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CsimDirPath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public FileSystemEntry simDir;

		public _003C_003Ec__DisplayClass130_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003CMigrateFamilyIdToSideBySideVersionId_003Eb__1(CancellationToken _)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed _003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed = default(_003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed);
			_003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed>(ref _003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CMigrateFamilyIdToSideBySideVersionId_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass143_0
	{
		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public AircraftRepoInfo repoInfo;

		public Action<string> _003C_003E9__6;

		internal IObservable<string> _003CPerformAircraftFormatMigrationWithLock_003Eb__5(string dir)
		{
			return Observable.Return(dir).Do<string>((Action<string>)delegate(string d)
			{
				_003C_003E4__this.MigrateSimWinfsDirectoryToWinfsZip(d);
			}).SubscribeOn<string>((IScheduler)Scheduler.Default);
		}

		internal void _003CPerformAircraftFormatMigrationWithLock_003Eb__6(string d)
		{
			_003C_003E4__this.MigrateSimWinfsDirectoryToWinfsZip(d);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass143_1
	{
		[StructLayout(3)]
		private struct _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass143_1 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass143_1 _003C_003Ec__DisplayClass143_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						AircraftListData updatedListData = _003C_003Ec__DisplayClass143_.persistedAircraft.ToListData();
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals1._003C_003E4__this.UpdateListDataWithLock("Aircraft Migration", _003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals1.aircraftIdentifier, _003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals1.repoInfo, updatedListData).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		[StructLayout(3)]
		private struct _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass143_1 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass143_1 _003C_003Ec__DisplayClass143_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						AircraftHeavyData heavyData = _003C_003Ec__DisplayClass143_.persistedAircraft.ToHeavyData();
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals1._003C_003E4__this.UpdateHeavyDataWithLock("Aircraft Migration", _003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals1.aircraftIdentifier, _003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals1.repoInfo, heavyData).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		[StructLayout(3)]
		private struct _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass143_1 _003C_003E4__this;

			private string _003CseparateDataPath_003E5__2;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass143_1 _003C_003Ec__DisplayClass143_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						PersistedSimPanelInfo persistenceModel = _003C_003Ec__DisplayClass143_.persistedAircraft.ToSeparatedSimPanelFile();
						_003CseparateDataPath_003E5__2 = Path.Combine(_003C_003Ec__DisplayClass143_.currentAircraftDirectoryPath, "panel.json");
						Logger.Log(LogLevel.Info, "Migrating to separated sim panel data file {0}", PathHelpers.QuotePathIfNeeded(_003CseparateDataPath_003E5__2));
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals1._003C_003E4__this._persistence.Commit(persistenceModel, _003CseparateDataPath_003E5__2).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					Logger.Log(LogLevel.Info, "Migrated separated sim panel data file {0}", PathHelpers.QuotePathIfNeeded(_003CseparateDataPath_003E5__2));
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CseparateDataPath_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CseparateDataPath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public string aircraftAvionicsDirectoryPath;

		public PersistedAircraft persistedAircraft;

		public string currentAircraftDirectoryPath;

		public _003C_003Ec__DisplayClass143_0 CS_0024_003C_003E8__locals1;

		internal IObservable<Unit> _003CPerformAircraftFormatMigrationWithLock_003Eb__3(FamilyIdMigration v1FamilyDirectory)
		{
			return Observable.FromAsync((Func<System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass143_2
			{
				CS_0024_003C_003E8__locals2 = this,
				v1FamilyDirectory = v1FamilyDirectory
			}._003CPerformAircraftFormatMigrationWithLock_003Eb__4);
		}

		[AsyncStateMachine(typeof(_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CPerformAircraftFormatMigrationWithLock_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed = default(_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed);
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed>(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003CPerformAircraftFormatMigrationWithLock_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed = default(_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed);
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed>(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed))]
		internal System.Threading.Tasks.Task _003CPerformAircraftFormatMigrationWithLock_003Eb__2()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed = default(_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed);
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003E4__this = this;
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed>(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass143_2
	{
		[StructLayout(3)]
		private struct _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass143_2 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass143_2 _003C_003Ec__DisplayClass143_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this.MigrateFamilyIdToSideBySideVersionId(_003C_003Ec__DisplayClass143_.CS_0024_003C_003E8__locals2.aircraftAvionicsDirectoryPath, _003C_003Ec__DisplayClass143_.v1FamilyDirectory).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		public FamilyIdMigration v1FamilyDirectory;

		public _003C_003Ec__DisplayClass143_1 CS_0024_003C_003E8__locals2;

		[AsyncStateMachine(typeof(_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed))]
		internal System.Threading.Tasks.Task _003CPerformAircraftFormatMigrationWithLock_003Eb__4()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed = default(_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed);
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed._003C_003E4__this = this;
			_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed._003C_003Et__builder)).Start<_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed>(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__4_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass32_0
	{
		public DirectoryBasedHangar _003C_003E4__this;

		public string selectedAircraftFilePath;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public IObserver<AircraftListData> o;

		public List<System.Exception> internalOrIoExceptions;

		public CancellationToken token;

		public DirectoryBasedHangar _003C_003E4__this;

		public Action<AircraftListData> _003C_003E9__3;

		public Action<System.Exception> _003C_003E9__5;

		internal void _003CGetAllAircraftListData_003Eb__3(AircraftListData ld)
		{
			o.OnNext(ld);
		}

		internal void _003CGetAllAircraftListData_003Eb__5(System.Exception internalError)
		{
			internalOrIoExceptions.Add(internalError);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_1
	{
		public AircraftIdentifier selectedId;

		public _003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals1;

		internal bool _003CGetAllAircraftListData_003Eb__1(FileSystemEntry ad)
		{
			return ad.Name == selectedId.IdentString;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_2
	{
		public FileSystemEntry aircraftDirectory;

		public _003C_003Ec__DisplayClass35_1 CS_0024_003C_003E8__locals2;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_3
	{
		public List<System.Exception> aircraftDataErrors;

		internal void _003CGetAllAircraftListData_003Eb__4(System.Exception dataError)
		{
			aircraftDataErrors.Add(dataError);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass37_0
	{
		[StructLayout(3)]
		private struct _003C_003CGetAircraftHeavyDataById_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AircraftHeavyData> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass37_0 _003C_003E4__this;

			public AircraftRepoInfo repoInfo;

			private ConfiguredTaskAwaiter<AircraftHeavyData> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass37_0 _003C_003Ec__DisplayClass37_ = _003C_003E4__this;
				AircraftHeavyData result;
				try
				{
					ConfiguredTaskAwaiter<AircraftHeavyData> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass37_._003C_003E4__this.GetHeavyDataWithLock(_003C_003Ec__DisplayClass37_.aircraftIdentifier, repoInfo).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftHeavyData>, _003C_003CGetAircraftHeavyDataById_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftHeavyData>);
						num = (_003C_003E1__state = -1);
					}
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

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		[AsyncStateMachine(typeof(_003C_003CGetAircraftHeavyDataById_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<AircraftHeavyData> _003CGetAircraftHeavyDataById_003Eb__0(AircraftRepoInfo repoInfo)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _003C_003E4__this.GetHeavyDataWithLock(aircraftIdentifier, repoInfo).DetachContext();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass38_0
	{
		[StructLayout(3)]
		private struct _003C_003CGetAircraftSnapshotById_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass38_0 _003C_003E4__this;

			public AircraftRepoInfo repoInfo;

			private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass38_0 _003C_003Ec__DisplayClass38_ = _003C_003E4__this;
				AircraftSnapshotData result;
				try
				{
					ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass38_._003C_003E4__this.GetAircraftSnapshotWithLock(_003C_003Ec__DisplayClass38_.aircraftIdentifier, repoInfo).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003C_003CGetAircraftSnapshotById_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
						num = (_003C_003E1__state = -1);
					}
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

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		[AsyncStateMachine(typeof(_003C_003CGetAircraftSnapshotById_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<AircraftSnapshotData> _003CGetAircraftSnapshotById_003Eb__0(AircraftRepoInfo repoInfo)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _003C_003E4__this.GetAircraftSnapshotWithLock(aircraftIdentifier, repoInfo).DetachContext();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass39_0
	{
		[StructLayout(3)]
		private struct _003C_003CGetSimulatorPanel_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<SimPanel> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass39_0 _003C_003E4__this;

			public AircraftRepoInfo repoInfo;

			private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass39_0 _003C_003Ec__DisplayClass39_ = _003C_003E4__this;
				SimPanel result;
				try
				{
					ConfiguredTaskAwaiter<SimPanel> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass39_._003C_003E4__this.GetPersistedPanelWithLock(_003C_003Ec__DisplayClass39_.aircraftIdentifier, repoInfo).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003C_003CGetSimulatorPanel_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
						num = (_003C_003E1__state = -1);
					}
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

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		[AsyncStateMachine(typeof(_003C_003CGetSimulatorPanel_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<SimPanel> _003CGetSimulatorPanel_003Eb__0(AircraftRepoInfo repoInfo)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _003C_003E4__this.GetPersistedPanelWithLock(aircraftIdentifier, repoInfo).DetachContext();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass41_0<TResult> where TResult : notnull
	{
		public CancellationToken token;

		internal AwaitableDisposable<System.IDisposable> _003CReadAircraftHarness_003Eb__0(AsyncReaderWriterLock l)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return l.ReaderLockAsync(token);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass43_0<TResult> where TResult : notnull
	{
		public CancellationToken token;

		internal AwaitableDisposable<System.IDisposable> _003CReadAircraftHarnessWithRollbackHandling_003Eb__0(AsyncReaderWriterLock l)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return l.ReaderLockAsync(token);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass44_0<TResult> where TResult : notnull
	{
		public CancellationToken token;

		internal AwaitableDisposable<System.IDisposable> _003CWriteAircraftHarnessWithRollbackHandling_003Eb__0(AsyncReaderWriterLock l)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return l.WriterLockAsync(token);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass46_0
	{
		[StructLayout(3)]
		private struct _003C_003CCreateNewAircraft_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass46_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass46_0 _003C_003Ec__DisplayClass46_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass46_._003C_003E4__this.PlaceMigrationFile(_003C_003Ec__DisplayClass46_.repoInfo).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CCreateNewAircraft_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		[StructLayout(3)]
		private struct _003C_003CCreateNewAircraft_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass46_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass46_0 _003C_003Ec__DisplayClass46_ = _003C_003E4__this;
				try
				{
					if (num != 0)
					{
						LogRepoMessage(LogLevel.Info, "Create new aircraft", (string?)_003C_003Ec__DisplayClass46_.aircraftIdentifier + " - Saving to disk.");
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass46_._003C_003E4__this.UpdateListDataWithLock("Create new aircraft", _003C_003Ec__DisplayClass46_.aircraftIdentifier, _003C_003Ec__DisplayClass46_.repoInfo, _003C_003Ec__DisplayClass46_.startingInfo).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CCreateNewAircraft_003Eb__1_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					}
					catch (System.Exception e)
					{
						LogRepoError(e, "Create new aircraft", "Could not serialize new aircraft JSON record '{0}'.", _003C_003Ec__DisplayClass46_.aircraftIdentifier);
						throw;
					}
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

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftRepoInfo repoInfo;

		public AircraftIdentifier aircraftIdentifier;

		public AircraftListData startingInfo;

		[AsyncStateMachine(typeof(_003C_003CCreateNewAircraft_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CCreateNewAircraft_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CCreateNewAircraft_003Eb__0_003Ed _003C_003CCreateNewAircraft_003Eb__0_003Ed = default(_003C_003CCreateNewAircraft_003Eb__0_003Ed);
			_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CCreateNewAircraft_003Eb__0_003Ed>(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CCreateNewAircraft_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003CCreateNewAircraft_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CCreateNewAircraft_003Eb__1_003Ed _003C_003CCreateNewAircraft_003Eb__1_003Ed = default(_003C_003CCreateNewAircraft_003Eb__1_003Ed);
			_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CCreateNewAircraft_003Eb__1_003Ed>(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass48_0
	{
		[StructLayout(3)]
		private struct _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass48_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass48_0 CS_0024_003C_003E8__locals0 = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						List<KeyValuePair<string, PersistedSimulator>> val = Enumerable.ToList<KeyValuePair<string, PersistedSimulator>>(Enumerable.Select<SimInstallData, KeyValuePair<string, PersistedSimulator>>(CS_0024_003C_003E8__locals0.simulator.Yield(), (Func<SimInstallData, KeyValuePair<string, PersistedSimulator>>)((SimInstallData sim) => ConvertToPersistedSimulatorAndLocation(sim, CS_0024_003C_003E8__locals0.repoInfo))));
						LogRepoMessage(LogLevel.Info, "UPDATE_SIMULATOR", "{0} - Saving simulators.", CS_0024_003C_003E8__locals0.aircraftIdentifier);
						ConfiguredTaskAwaitable val2 = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<KeyValuePair<string, PersistedSimulator>, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<KeyValuePair<string, PersistedSimulator>>)val, (Func<KeyValuePair<string, PersistedSimulator>, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed))] (KeyValuePair<string, PersistedSimulator> kv) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							//IL_0016: Unknown result type (might be due to invalid IL or missing references)
							//IL_0017: Unknown result type (might be due to invalid IL or missing references)
							_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed = default(_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed);
							_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
							_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed.kv = kv;
							_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003Et__builder)).Start<_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed>(ref _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003Et__builder)).Task;
						}))).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		[StructLayout(3)]
		private struct _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass48_0 _003C_003E4__this;

			public KeyValuePair<string, PersistedSimulator> kv;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass48_0 _003C_003Ec__DisplayClass48_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass48_._003C_003E4__this._persistence.Commit(kv.Value, kv.Key).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		public SimInstallData simulator;

		public AircraftRepoInfo repoInfo;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public Func<SimInstallData, KeyValuePair<string, PersistedSimulator>> _003C_003E9__3;

		public Func<KeyValuePair<string, PersistedSimulator>, System.Threading.Tasks.Task> _003C_003E9__4;

		internal bool _003CCreateOrUpdateSimulator_003Eb__2(SimInstallKey s)
		{
			return s.Slot == simulator.InstallationKey.Slot;
		}

		[AsyncStateMachine(typeof(_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CCreateOrUpdateSimulator_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed = default(_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed);
			_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed>(ref _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}

		internal KeyValuePair<string, PersistedSimulator> _003CCreateOrUpdateSimulator_003Eb__3(SimInstallData sim)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return ConvertToPersistedSimulatorAndLocation(sim, repoInfo);
		}

		[AsyncStateMachine(typeof(_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed))]
		internal System.Threading.Tasks.Task _003CCreateOrUpdateSimulator_003Eb__4(KeyValuePair<string, PersistedSimulator> kv)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed = default(_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed);
			_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003E4__this = this;
			_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed.kv = kv;
			_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003Et__builder)).Start<_003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed>(ref _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__4_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass48_1
	{
		[StructLayout(3)]
		private struct _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass48_1 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass48_1 _003C_003Ec__DisplayClass48_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_00bf;
					}
					if (_003C_003Ec__DisplayClass48_.activeChanged)
					{
						LogRepoMessage(LogLevel.Info, "UPDATE_SIMULATOR", "{0} - Saving aircraft panel.", _003C_003Ec__DisplayClass48_.CS_0024_003C_003E8__locals1.aircraftIdentifier);
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass48_.CS_0024_003C_003E8__locals1._003C_003E4__this._persistence.Commit(_003C_003Ec__DisplayClass48_.currentPanel, _003C_003Ec__DisplayClass48_.panelPath).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_00bf;
					}
					LogRepoMessage(LogLevel.Info, "UPDATE_SIMULATOR", "{0} - No active sim changes on aircraft.", _003C_003Ec__DisplayClass48_.CS_0024_003C_003E8__locals1.aircraftIdentifier);
					goto end_IL_000e;
					IL_00bf:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					LogRepoMessage(LogLevel.Info, "UPDATE_SIMULATOR", "{0} - Updated aircraft panel.", _003C_003Ec__DisplayClass48_.CS_0024_003C_003E8__locals1.aircraftIdentifier);
					end_IL_000e:;
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

		public bool activeChanged;

		public PersistedSimPanelInfo currentPanel;

		public string panelPath;

		public _003C_003Ec__DisplayClass48_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003CCreateOrUpdateSimulator_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed = default(_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed);
			_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed>(ref _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass49_0
	{
		[StructLayout(3)]
		private struct _003C_003CUpdateAircraft_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass49_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass49_0 _003C_003Ec__DisplayClass49_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass49_._003C_003E4__this.UpdateListDataWithLock("Update aircraft", _003C_003Ec__DisplayClass49_.aircraftIdentifier, _003C_003Ec__DisplayClass49_.repoInfo, _003C_003Ec__DisplayClass49_.aircraftSnapshot.ListData).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CUpdateAircraft_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		[StructLayout(3)]
		private struct _003C_003CUpdateAircraft_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass49_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass49_0 _003C_003Ec__DisplayClass49_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass49_._003C_003E4__this.UpdateHeavyDataWithLock("Update aircraft", _003C_003Ec__DisplayClass49_.aircraftIdentifier, _003C_003Ec__DisplayClass49_.repoInfo, _003C_003Ec__DisplayClass49_.aircraftSnapshot.HeavyData).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CUpdateAircraft_003Eb__1_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		[StructLayout(3)]
		private struct _003C_003CUpdateAircraft_003Eb__2_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass49_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass49_0 _003C_003Ec__DisplayClass49_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<SimPanel> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass49_._003C_003E4__this.GetPreviousPanelAndUpdateWithNewOneWithLock("Update aircraft", _003C_003Ec__DisplayClass49_.aircraftIdentifier, _003C_003Ec__DisplayClass49_.repoInfo, _003C_003Ec__DisplayClass49_.aircraftSnapshot.Panel).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003C_003CUpdateAircraft_003Eb__2_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
						num = (_003C_003E1__state = -1);
					}
					SimPanel result = awaiter.GetResult();
					_003C_003Ec__DisplayClass49_.previousPanel = result;
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

		[StructLayout(3)]
		private struct _003C_003CUpdateAircraft_003Eb__3_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass49_0 _003C_003E4__this;

			private _003C_003Ec__DisplayClass49_1 _003C_003E8__1;

			private System.Collections.Generic.IEnumerable<FileSystemEntry> _003CfilesToDelete_003E5__2;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_019f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_0163: Unknown result type (might be due to invalid IL or missing references)
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				//IL_016c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass49_0 CS_0024_003C_003E8__locals0 = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass49_1();
						_003C_003E8__1.familyDataDir = GetFamilyDataDirPath(CS_0024_003C_003E8__locals0.repoInfo.CurrentAircraftRoot);
						_003C_003E8__1.familyFilesToWrite = Enumerable.ToDictionary<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>, string>(Enumerable.Select<KeyValuePair<TrainerEmbeddedReleaseId, FamilyData>, ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>>((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerEmbeddedReleaseId, FamilyData>>)CS_0024_003C_003E8__locals0.aircraftSnapshot.FamilyLookup.FamilyDataByRelease, (Func<KeyValuePair<TrainerEmbeddedReleaseId, FamilyData>, ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>>)((KeyValuePair<TrainerEmbeddedReleaseId, FamilyData> kv) => new ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>(kv.Key, GetFamilyDataFilePath(_003C_003E8__1.familyDataDir, kv.Key), CreatePersistedFamilyData(kv.Value)))), (Func<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>, string>)((ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData> t) => t.Item2), (IEqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash);
						System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable;
						if (!CS_0024_003C_003E8__locals0._003C_003E4__this._fs.PathExists(_003C_003E8__1.familyDataDir))
						{
							enumerable = Enumerable.Empty<FileSystemEntry>();
						}
						else
						{
							System.Collections.Generic.IEnumerable<FileSystemEntry> enumerable2 = (System.Collections.Generic.IEnumerable<FileSystemEntry>)Enumerable.ToList<FileSystemEntry>(Enumerable.Where<FileSystemEntry>(CS_0024_003C_003E8__locals0._003C_003E4__this._fs.FilesIn(_003C_003E8__1.familyDataDir), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => TrainerEmbeddedReleaseId.CanParse(Path.GetFileNameWithoutExtension(fse.Name)) && !_003C_003E8__1.familyFilesToWrite.ContainsKey(fse.FullPath))));
							enumerable = enumerable2;
						}
						_003CfilesToDelete_003E5__2 = enumerable;
						if (_003C_003E8__1.familyFilesToWrite.Count <= 0)
						{
							goto IL_01c1;
						}
						CS_0024_003C_003E8__locals0._003C_003E4__this._fs.CreateDirectory(_003C_003E8__1.familyDataDir);
						ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>, System.Threading.Tasks.Task>((System.Collections.Generic.IEnumerable<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>>)_003C_003E8__1.familyFilesToWrite.Values, (Func<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003CUpdateAircraft_003Eb__7_003Ed))] (ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData> t) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							//IL_0016: Unknown result type (might be due to invalid IL or missing references)
							//IL_0017: Unknown result type (might be due to invalid IL or missing references)
							_003C_003CUpdateAircraft_003Eb__7_003Ed _003C_003CUpdateAircraft_003Eb__7_003Ed = default(_003C_003CUpdateAircraft_003Eb__7_003Ed);
							_003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
							_003C_003CUpdateAircraft_003Eb__7_003Ed.t = t;
							_003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003Et__builder)).Start<_003C_003CUpdateAircraft_003Eb__7_003Ed>(ref _003C_003CUpdateAircraft_003Eb__7_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003Et__builder)).Task;
						}))).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CUpdateAircraft_003Eb__3_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					goto IL_01c1;
					IL_01c1:
					System.Collections.Generic.IEnumerator<FileSystemEntry> enumerator = _003CfilesToDelete_003E5__2.GetEnumerator();
					try
					{
						while (((System.Collections.IEnumerator)enumerator).MoveNext())
						{
							FileSystemEntry current = enumerator.Current;
							CS_0024_003C_003E8__locals0._003C_003E4__this._fs.DeleteIfExistsAndParentsThatAreEmpty(current.FullPath, CS_0024_003C_003E8__locals0.repoInfo.CurrentAircraftRoot);
						}
					}
					finally
					{
						if (num < 0)
						{
							((System.IDisposable)enumerator)?.Dispose();
						}
					}
					CS_0024_003C_003E8__locals0.familyDataChanges = (System.Collections.Generic.IEnumerable<DataChangeNotification<AircraftPackageFamily>>)Enumerable.ToList<DataChangeNotification<AircraftPackageFamily>>(Enumerable.Concat<DataChangeNotification<AircraftPackageFamily>>(Enumerable.Select<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>, DataChangeNotification<AircraftPackageFamily>>((System.Collections.Generic.IEnumerable<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>>)_003C_003E8__1.familyFilesToWrite.Values, (Func<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>, DataChangeNotification<AircraftPackageFamily>>)((ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData> t) => DataChangeNotification.Create(new AircraftPackageFamily(CS_0024_003C_003E8__locals0.aircraftIdentifier, t.Item1)))), Enumerable.Select<FileSystemEntry, DataChangeNotification<AircraftPackageFamily>>(_003CfilesToDelete_003E5__2, (Func<FileSystemEntry, DataChangeNotification<AircraftPackageFamily>>)((FileSystemEntry fse) => DataChangeNotification.Create(new AircraftPackageFamily(CS_0024_003C_003E8__locals0.aircraftIdentifier, new TrainerEmbeddedReleaseId(Path.GetFileNameWithoutExtension(fse.Name))), DataChangeType.Deleted)))));
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003CfilesToDelete_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CfilesToDelete_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		[StructLayout(3)]
		private struct _003C_003CUpdateAircraft_003Eb__7_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass49_0 _003C_003E4__this;

			public ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData> t;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass49_0 _003C_003Ec__DisplayClass49_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass49_._003C_003E4__this._persistence.Commit(t.Item3, t.Item2).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CUpdateAircraft_003Eb__7_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
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

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public AircraftRepoInfo repoInfo;

		public AircraftSnapshotData aircraftSnapshot;

		public SimPanel previousPanel;

		public System.Collections.Generic.IEnumerable<DataChangeNotification<AircraftPackageFamily>> familyDataChanges;

		public Func<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>, System.Threading.Tasks.Task> _003C_003E9__7;

		public Func<ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>, DataChangeNotification<AircraftPackageFamily>> _003C_003E9__8;

		public Func<FileSystemEntry, DataChangeNotification<AircraftPackageFamily>> _003C_003E9__9;

		[AsyncStateMachine(typeof(_003C_003CUpdateAircraft_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CUpdateAircraft_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CUpdateAircraft_003Eb__0_003Ed _003C_003CUpdateAircraft_003Eb__0_003Ed = default(_003C_003CUpdateAircraft_003Eb__0_003Ed);
			_003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CUpdateAircraft_003Eb__0_003Ed>(ref _003C_003CUpdateAircraft_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CUpdateAircraft_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003CUpdateAircraft_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CUpdateAircraft_003Eb__1_003Ed _003C_003CUpdateAircraft_003Eb__1_003Ed = default(_003C_003CUpdateAircraft_003Eb__1_003Ed);
			_003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CUpdateAircraft_003Eb__1_003Ed>(ref _003C_003CUpdateAircraft_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CUpdateAircraft_003Eb__2_003Ed))]
		internal System.Threading.Tasks.Task _003CUpdateAircraft_003Eb__2()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CUpdateAircraft_003Eb__2_003Ed _003C_003CUpdateAircraft_003Eb__2_003Ed = default(_003C_003CUpdateAircraft_003Eb__2_003Ed);
			_003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003E4__this = this;
			_003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003CUpdateAircraft_003Eb__2_003Ed>(ref _003C_003CUpdateAircraft_003Eb__2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CUpdateAircraft_003Eb__3_003Ed))]
		internal System.Threading.Tasks.Task _003CUpdateAircraft_003Eb__3()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CUpdateAircraft_003Eb__3_003Ed _003C_003CUpdateAircraft_003Eb__3_003Ed = default(_003C_003CUpdateAircraft_003Eb__3_003Ed);
			_003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003E4__this = this;
			_003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003CUpdateAircraft_003Eb__3_003Ed>(ref _003C_003CUpdateAircraft_003Eb__3_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CUpdateAircraft_003Eb__7_003Ed))]
		internal System.Threading.Tasks.Task _003CUpdateAircraft_003Eb__7(ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData> t)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CUpdateAircraft_003Eb__7_003Ed _003C_003CUpdateAircraft_003Eb__7_003Ed = default(_003C_003CUpdateAircraft_003Eb__7_003Ed);
			_003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003E4__this = this;
			_003C_003CUpdateAircraft_003Eb__7_003Ed.t = t;
			_003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003Et__builder)).Start<_003C_003CUpdateAircraft_003Eb__7_003Ed>(ref _003C_003CUpdateAircraft_003Eb__7_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__7_003Ed._003C_003Et__builder)).Task;
		}

		internal DataChangeNotification<AircraftPackageFamily> _003CUpdateAircraft_003Eb__8(ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData> t)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return DataChangeNotification.Create(new AircraftPackageFamily(aircraftIdentifier, t.Item1));
		}

		internal DataChangeNotification<AircraftPackageFamily> _003CUpdateAircraft_003Eb__9(FileSystemEntry fse)
		{
			return DataChangeNotification.Create(new AircraftPackageFamily(aircraftIdentifier, new TrainerEmbeddedReleaseId(Path.GetFileNameWithoutExtension(fse.Name))), DataChangeType.Deleted);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass49_1
	{
		public string familyDataDir;

		public Dictionary<string, ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>> familyFilesToWrite;

		internal ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData> _003CUpdateAircraft_003Eb__4(KeyValuePair<TrainerEmbeddedReleaseId, FamilyData> kv)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			return new ValueTuple<TrainerEmbeddedReleaseId, string, PersistedFamilyData>(kv.Key, GetFamilyDataFilePath(familyDataDir, kv.Key), CreatePersistedFamilyData(kv.Value));
		}

		internal bool _003CUpdateAircraft_003Eb__6(FileSystemEntry fse)
		{
			if (TrainerEmbeddedReleaseId.CanParse(Path.GetFileNameWithoutExtension(fse.Name)))
			{
				return !familyFilesToWrite.ContainsKey(fse.FullPath);
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass54_0
	{
		[StructLayout(3)]
		private struct _003C_003CWriteAircraftHarness_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<int> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass54_0 _003C_003E4__this;

			public AircraftRepoInfo repoInfo;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass54_0 _003C_003Ec__DisplayClass54_ = _003C_003E4__this;
				int result;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass54_.writeAircraft.Invoke(repoInfo).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CWriteAircraftHarness_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					result = 0;
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

		public Func<AircraftRepoInfo, System.Threading.Tasks.Task> writeAircraft;

		[AsyncStateMachine(typeof(_003C_003CWriteAircraftHarness_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<int> _003CWriteAircraftHarness_003Eb__0(AircraftRepoInfo repoInfo)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			await writeAircraft.Invoke(repoInfo).DetachContext();
			return 0;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass55_0
	{
		public AircraftIdentifier aircraftIdentifier;

		internal SimAircraftKey _003CDeleteAircraft_003Eb__0(SimInstallKey simKey)
		{
			return new SimAircraftKey(aircraftIdentifier, simKey);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass56_0
	{
		public SimPanel panel;

		public Func<KeyValuePair<SimInstallKey, string>, bool> _003C_003E9__0;

		internal bool _003CGetAircraftWinfsArchives_003Eb__0(KeyValuePair<SimInstallKey, string> kv)
		{
			return panel.AllSimulators.ContainsKey(kv.Key);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass57_0
	{
		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		internal bool _003CUpdateAircraftWinfsArchives_003Eb__0(string f)
		{
			return !_003C_003E4__this._fs.PathExists(f);
		}

		internal bool _003CUpdateAircraftWinfsArchives_003Eb__1(string f)
		{
			return !_003C_003E4__this.IsValidWinfsArchive(f, "UPDATE_WINFS_ARCHIVE");
		}

		internal SimAircraftKey _003CUpdateAircraftWinfsArchives_003Eb__2(SimInstallKey sim)
		{
			return new SimAircraftKey(aircraftIdentifier, sim);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass57_1
	{
		public SimPanel panel;

		internal bool _003CUpdateAircraftWinfsArchives_003Eb__3(SimInstallKey sk)
		{
			return !panel.AllSimulators.ContainsKey(sk);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass58_0
	{
		public AircraftIdentifier aircraftIdentifier;

		internal SimAircraftKey _003CDeleteAircraftWinfsArchives_003Eb__0(SimInstallKey sim)
		{
			return new SimAircraftKey(aircraftIdentifier, sim);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass61_0
	{
		[StructLayout(3)]
		private struct _003C_003CImportAircaft_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass61_0 _003C_003E4__this;

			public string tempAircraftRootPath;

			public AircraftIdentifier exportId;

			private AircraftIdentifier _003CaircraftIdentifier_003E5__2;

			private AircraftRepoInfo _003CrepoInfo_003E5__3;

			private string _003CaircraftDirectoryPath_003E5__4;

			private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__1;

			private System.IDisposable _003C_003E7__wrap4;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__2;

			private AircraftRepoInfo _003CtempRepoInfo_003E5__6;

			private AircraftListData _003CpersistedListData_003E5__7;

			private ConfiguredTaskAwaiter _003C_003Eu__3;

			private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__4;

			private void MoveNext()
			{
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_0169: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0134: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0149: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0228: Unknown result type (might be due to invalid IL or missing references)
				//IL_022d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0235: Unknown result type (might be due to invalid IL or missing references)
				//IL_0322: Unknown result type (might be due to invalid IL or missing references)
				//IL_0327: Unknown result type (might be due to invalid IL or missing references)
				//IL_032f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0474: Unknown result type (might be due to invalid IL or missing references)
				//IL_0479: Unknown result type (might be due to invalid IL or missing references)
				//IL_0481: Unknown result type (might be due to invalid IL or missing references)
				//IL_053f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0544: Unknown result type (might be due to invalid IL or missing references)
				//IL_054c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
				//IL_020d: Unknown result type (might be due to invalid IL or missing references)
				//IL_020f: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0501: Unknown result type (might be due to invalid IL or missing references)
				//IL_0506: Unknown result type (might be due to invalid IL or missing references)
				//IL_050a: Unknown result type (might be due to invalid IL or missing references)
				//IL_050f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0436: Unknown result type (might be due to invalid IL or missing references)
				//IL_043b: Unknown result type (might be due to invalid IL or missing references)
				//IL_043f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0444: Unknown result type (might be due to invalid IL or missing references)
				//IL_0307: Unknown result type (might be due to invalid IL or missing references)
				//IL_0309: Unknown result type (might be due to invalid IL or missing references)
				//IL_0524: Unknown result type (might be due to invalid IL or missing references)
				//IL_0526: Unknown result type (might be due to invalid IL or missing references)
				//IL_0459: Unknown result type (might be due to invalid IL or missing references)
				//IL_045b: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass61_0 _003C_003Ec__DisplayClass61_ = _003C_003E4__this;
				AircraftSnapshotData result4;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					System.IDisposable result;
					ConfiguredTaskAwaiter<AircraftListData> awaiter2;
					AircraftListData result2;
					switch (num)
					{
					default:
						awaiter2 = HangarUtil.PrepareStartingInfoWithUniqueId(_003C_003Ec__DisplayClass61_.newListData, AircraftIdentifier.GenerateIdentifiers(_003C_003Ec__DisplayClass61_.newListData.AirframeCCT, "R"), _003C_003Ec__DisplayClass61_._003C_003E4__this.ReserveAircraftDirectoryIfNotTaken).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003C_003CImportAircaft_003Eb__0_003Ed>(ref awaiter2, ref this);
							return;
						}
						goto IL_00b7;
					case 0:
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
						num = (_003C_003E1__state = -1);
						goto IL_00b7;
					case 1:
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
						goto IL_0180;
					case 2:
					case 3:
					case 4:
					case 5:
						break;
						IL_0180:
						result = awaiter.GetResult();
						_003C_003E7__wrap4 = result;
						break;
						IL_00b7:
						result2 = awaiter2.GetResult();
						_003C_003Ec__DisplayClass61_.newListData = result2;
						_003CaircraftIdentifier_003E5__2 = _003C_003Ec__DisplayClass61_.newListData.AircraftIdentifier;
						_003CrepoInfo_003E5__3 = _003C_003Ec__DisplayClass61_._003C_003E4__this.CreateRepoInfoFromHangarControlledIdentifier(_003CaircraftIdentifier_003E5__2);
						_003CaircraftDirectoryPath_003E5__4 = _003CrepoInfo_003E5__3.RootPath;
						awaiter = _003C_003Ec__DisplayClass61_._003C_003E4__this.GetLockForPathUnlocked(_003CrepoInfo_003E5__3.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CImportAircaft_003Eb__0_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_0180;
					}
					AircraftSnapshotData result3;
					try
					{
						if ((uint)(num - 2) > 3u)
						{
							_003C_003Ec__DisplayClass61_._003C_003E4__this.ThrowIfAircraftDirectoryIsNotEmpty(_003CaircraftIdentifier_003E5__2, _003CaircraftDirectoryPath_003E5__4);
						}
						try
						{
							ConfiguredTaskAwaiter awaiter4;
							ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter3;
							ConfiguredTaskAwaitable val;
							string destinationPath;
							switch (num)
							{
							default:
								_003CtempRepoInfo_003E5__6 = CreateRepoInfo(tempAircraftRootPath);
								awaiter2 = _003C_003Ec__DisplayClass61_._003C_003E4__this.GetListDataWithLock(_003CtempRepoInfo_003E5__6).DetachContext().GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 2);
									_003C_003Eu__1 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003C_003CImportAircaft_003Eb__0_003Ed>(ref awaiter2, ref this);
									return;
								}
								goto IL_0244;
							case 2:
								awaiter2 = _003C_003Eu__1;
								_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
								num = (_003C_003E1__state = -1);
								goto IL_0244;
							case 3:
								awaiter4 = _003C_003Eu__3;
								_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_033e;
							case 4:
								awaiter4 = _003C_003Eu__3;
								_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_0490;
							case 5:
								{
									awaiter3 = _003C_003Eu__4;
									_003C_003Eu__4 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
									num = (_003C_003E1__state = -1);
									break;
								}
								IL_0398:
								if (_003CpersistedListData_003E5__7.DisplayName != _003C_003Ec__DisplayClass61_.newListData.DisplayName)
								{
									_003CpersistedListData_003E5__7 = _003CpersistedListData_003E5__7.Rename(_003C_003Ec__DisplayClass61_.newListData.DisplayName);
									LogRepoMessage(LogLevel.Info, "Import Aircraft", "Changing display name ({0} -> {1}) for imported GCA: {2}", _003CpersistedListData_003E5__7.DisplayName, _003C_003Ec__DisplayClass61_.newListData.DisplayName, PathHelpers.QuotePathIfNeeded(_003C_003Ec__DisplayClass61_.gcaPath));
									val = _003C_003Ec__DisplayClass61_._003C_003E4__this.UpdateListDataWithLock("Import Aircraft", _003CaircraftIdentifier_003E5__2, _003CtempRepoInfo_003E5__6, _003CpersistedListData_003E5__7).DetachContext();
									awaiter4 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter4)).IsCompleted)
									{
										num = (_003C_003E1__state = 4);
										_003C_003Eu__3 = awaiter4;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CImportAircaft_003Eb__0_003Ed>(ref awaiter4, ref this);
										return;
									}
									goto IL_0490;
								}
								goto IL_0497;
								IL_0244:
								result2 = awaiter2.GetResult();
								_003CpersistedListData_003E5__7 = result2;
								if (_003CaircraftIdentifier_003E5__2 != exportId)
								{
									LogRepoMessage(LogLevel.Info, "Import Aircraft", "Changing identifiers ({0} -> {1}) for imported GCA: {2}", exportId, _003CaircraftIdentifier_003E5__2, PathHelpers.QuotePathIfNeeded(_003C_003Ec__DisplayClass61_.gcaPath));
									_003CpersistedListData_003E5__7 = _003CpersistedListData_003E5__7.WithNewIdentifier(_003CaircraftIdentifier_003E5__2);
									val = _003C_003Ec__DisplayClass61_._003C_003E4__this.UpdateListDataWithLock("Import Aircraft", _003CaircraftIdentifier_003E5__2, _003CtempRepoInfo_003E5__6, _003CpersistedListData_003E5__7).DetachContext();
									awaiter4 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter4)).IsCompleted)
									{
										num = (_003C_003E1__state = 3);
										_003C_003Eu__3 = awaiter4;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CImportAircaft_003Eb__0_003Ed>(ref awaiter4, ref this);
										return;
									}
									goto IL_033e;
								}
								goto IL_0398;
								IL_0490:
								((ConfiguredTaskAwaiter)(ref awaiter4)).GetResult();
								goto IL_0497;
								IL_0497:
								LogRepoMessage(LogLevel.Info, "Import Aircraft", "Moving temporary import directory into the hangar: {0} -> {1}", PathHelpers.QuotePathIfNeeded(tempAircraftRootPath), PathHelpers.QuotePathIfNeeded(_003CaircraftDirectoryPath_003E5__4));
								_003C_003Ec__DisplayClass61_._003C_003E4__this._fs.MoveOrCopyAndDeleteToDifferentDriveAndOverwrite(LolaFsExtensions.DefaultTempFactory, tempAircraftRootPath, _003CaircraftDirectoryPath_003E5__4);
								awaiter3 = _003C_003Ec__DisplayClass61_._003C_003E4__this.GetAircraftSnapshotWithLock(_003CaircraftIdentifier_003E5__2, _003CrepoInfo_003E5__3).DetachContext().GetAwaiter();
								if (!awaiter3.IsCompleted)
								{
									num = (_003C_003E1__state = 5);
									_003C_003Eu__4 = awaiter3;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003C_003CImportAircaft_003Eb__0_003Ed>(ref awaiter3, ref this);
									return;
								}
								break;
								IL_033e:
								((ConfiguredTaskAwaiter)(ref awaiter4)).GetResult();
								destinationPath = Path.Combine(GetParentDirectoryPath(tempAircraftRootPath), _003CaircraftIdentifier_003E5__2.IdentStringChecked);
								_003C_003Ec__DisplayClass61_._003C_003E4__this._fs.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, tempAircraftRootPath, destinationPath);
								tempAircraftRootPath = destinationPath;
								_003CtempRepoInfo_003E5__6 = CreateRepoInfo(tempAircraftRootPath);
								goto IL_0398;
							}
							result3 = awaiter3.GetResult();
							_003CtempRepoInfo_003E5__6 = null;
							_003CpersistedListData_003E5__7 = null;
						}
						catch
						{
							_003C_003Ec__DisplayClass61_._003C_003E4__this._fs.DeleteIfExists(_003CaircraftDirectoryPath_003E5__4);
							throw;
						}
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap4 != null)
						{
							_003C_003E7__wrap4.Dispose();
						}
					}
					_003C_003E7__wrap4 = null;
					_003C_003Ec__DisplayClass61_._003C_003E4__this.PostAircraftChange(_003CaircraftIdentifier_003E5__2, DataChangeType.Created);
					result4 = result3;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CaircraftIdentifier_003E5__2 = default(AircraftIdentifier);
					_003CrepoInfo_003E5__3 = null;
					_003CaircraftDirectoryPath_003E5__4 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CaircraftIdentifier_003E5__2 = default(AircraftIdentifier);
				_003CrepoInfo_003E5__3 = null;
				_003CaircraftDirectoryPath_003E5__4 = null;
				_003C_003Et__builder.SetResult(result4);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public AircraftListData newListData;

		public DirectoryBasedHangar _003C_003E4__this;

		public string gcaPath;

		[AsyncStateMachine(typeof(_003C_003CImportAircaft_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<AircraftSnapshotData> _003CImportAircaft_003Eb__0(string tempAircraftRootPath, AircraftIdentifier exportId)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			newListData = await HangarUtil.PrepareStartingInfoWithUniqueId(newListData, AircraftIdentifier.GenerateIdentifiers(newListData.AirframeCCT, "R"), _003C_003E4__this.ReserveAircraftDirectoryIfNotTaken).DetachContext();
			AircraftIdentifier aircraftIdentifier = newListData.AircraftIdentifier;
			AircraftRepoInfo repoInfo = _003C_003E4__this.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
			string aircraftDirectoryPath = repoInfo.RootPath;
			AircraftSnapshotData result;
			using (await _003C_003E4__this.GetLockForPathUnlocked(repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				_003C_003E4__this.ThrowIfAircraftDirectoryIsNotEmpty(aircraftIdentifier, aircraftDirectoryPath);
				try
				{
					AircraftRepoInfo tempRepoInfo = CreateRepoInfo(tempAircraftRootPath);
					AircraftListData persistedListData = await _003C_003E4__this.GetListDataWithLock(tempRepoInfo).DetachContext();
					if (aircraftIdentifier != exportId)
					{
						LogRepoMessage(LogLevel.Info, "Import Aircraft", "Changing identifiers ({0} -> {1}) for imported GCA: {2}", exportId, aircraftIdentifier, PathHelpers.QuotePathIfNeeded(gcaPath));
						persistedListData = persistedListData.WithNewIdentifier(aircraftIdentifier);
						await _003C_003E4__this.UpdateListDataWithLock("Import Aircraft", aircraftIdentifier, tempRepoInfo, persistedListData).DetachContext();
						string text = Path.Combine(GetParentDirectoryPath(tempAircraftRootPath), aircraftIdentifier.IdentStringChecked);
						_003C_003E4__this._fs.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, tempAircraftRootPath, text);
						tempAircraftRootPath = text;
						tempRepoInfo = CreateRepoInfo(tempAircraftRootPath);
					}
					if (persistedListData.DisplayName != newListData.DisplayName)
					{
						persistedListData = persistedListData.Rename(newListData.DisplayName);
						LogRepoMessage(LogLevel.Info, "Import Aircraft", "Changing display name ({0} -> {1}) for imported GCA: {2}", persistedListData.DisplayName, newListData.DisplayName, PathHelpers.QuotePathIfNeeded(gcaPath));
						await _003C_003E4__this.UpdateListDataWithLock("Import Aircraft", aircraftIdentifier, tempRepoInfo, persistedListData).DetachContext();
					}
					LogRepoMessage(LogLevel.Info, "Import Aircraft", "Moving temporary import directory into the hangar: {0} -> {1}", PathHelpers.QuotePathIfNeeded(tempAircraftRootPath), PathHelpers.QuotePathIfNeeded(aircraftDirectoryPath));
					_003C_003E4__this._fs.MoveOrCopyAndDeleteToDifferentDriveAndOverwrite(LolaFsExtensions.DefaultTempFactory, tempAircraftRootPath, aircraftDirectoryPath);
					result = await _003C_003E4__this.GetAircraftSnapshotWithLock(aircraftIdentifier, repoInfo).DetachContext();
				}
				catch
				{
					_003C_003E4__this._fs.DeleteIfExists(aircraftDirectoryPath);
					throw;
				}
			}
			_003C_003E4__this.PostAircraftChange(aircraftIdentifier, DataChangeType.Created);
			return result;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass62_0
	{
		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public AircraftRepoInfo repoInfo;

		public AircraftSnapshotMetaData metadata;

		public AircraftSnapshotInfo info;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass64_0
	{
		[StructLayout(3)]
		private struct _003C_003CListHistoricalSnapshots_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass64_0 _003C_003E4__this;

			public CancellationToken token;

			public IObserver<AircraftSnapshotInfo> o;

			private ConfiguredTaskAwaiter<AircraftSnapshotInfo> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass64_0 _003C_003Ec__DisplayClass64_ = _003C_003E4__this;
				try
				{
					_003C_003Ec__DisplayClass64_1 _003C_003Ec__DisplayClass64_2 = default(_003C_003Ec__DisplayClass64_1);
					if (num != 0)
					{
						_003C_003Ec__DisplayClass64_2 = new _003C_003Ec__DisplayClass64_1
						{
							CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass64_,
							token = token,
							o = o
						};
					}
					try
					{
						ConfiguredTaskAwaiter<AircraftSnapshotInfo> awaiter;
						if (num != 0)
						{
							awaiter = _003C_003Ec__DisplayClass64_._003C_003E4__this.ReadAircraftHarnessWithRollbackHandling<AircraftSnapshotInfo>(_003C_003Ec__DisplayClass64_.aircraftIdentifier, _003C_003Ec__DisplayClass64_2._003CListHistoricalSnapshots_003Eb__1, _003C_003Ec__DisplayClass64_2.token).ConfigureAwait(false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotInfo>, _003C_003CListHistoricalSnapshots_003Eb__0_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotInfo>);
							num = (_003C_003E1__state = -1);
						}
						awaiter.GetResult();
					}
					catch (OperationCanceledException)
					{
					}
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

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		[AsyncStateMachine(typeof(_003C_003CListHistoricalSnapshots_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CListHistoricalSnapshots_003Eb__0(IObserver<AircraftSnapshotInfo> o, CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed _003C_003CListHistoricalSnapshots_003Eb__0_003Ed = default(_003C_003CListHistoricalSnapshots_003Eb__0_003Ed);
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed.o = o;
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed.token = token;
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CListHistoricalSnapshots_003Eb__0_003Ed>(ref _003C_003CListHistoricalSnapshots_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass64_1
	{
		[StructLayout(3)]
		private struct _003C_003CListHistoricalSnapshots_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AircraftSnapshotInfo> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass64_1 _003C_003E4__this;

			public AircraftRepoInfo repoInfo;

			private ConfiguredTaskAwaiter<AircraftSnapshotInfo> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass64_1 _003C_003Ec__DisplayClass64_ = _003C_003E4__this;
				AircraftSnapshotInfo result;
				try
				{
					ConfiguredTaskAwaiter<AircraftSnapshotInfo> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass64_.CS_0024_003C_003E8__locals1._003C_003E4__this.ListSnapshotsWithLock(_003C_003Ec__DisplayClass64_.CS_0024_003C_003E8__locals1.aircraftIdentifier, repoInfo).TakeUntil<AircraftSnapshotInfo>(_003C_003Ec__DisplayClass64_.token).Do<AircraftSnapshotInfo>(_003C_003Ec__DisplayClass64_.o)
							.LastOrDefaultAsync<AircraftSnapshotInfo>()
							.ToTask<AircraftSnapshotInfo>()
							.ConfigureAwait(false)
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotInfo>, _003C_003CListHistoricalSnapshots_003Eb__1_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotInfo>);
						num = (_003C_003E1__state = -1);
					}
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

		public CancellationToken token;

		public IObserver<AircraftSnapshotInfo> o;

		public _003C_003Ec__DisplayClass64_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CListHistoricalSnapshots_003Eb__1_003Ed))]
		internal async System.Threading.Tasks.Task<AircraftSnapshotInfo> _003CListHistoricalSnapshots_003Eb__1(AircraftRepoInfo repoInfo, AircraftRollbackException? e)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await CS_0024_003C_003E8__locals1._003C_003E4__this.ListSnapshotsWithLock(CS_0024_003C_003E8__locals1.aircraftIdentifier, repoInfo).TakeUntil<AircraftSnapshotInfo>(token).Do<AircraftSnapshotInfo>(o)
				.LastOrDefaultAsync<AircraftSnapshotInfo>()
				.ToTask<AircraftSnapshotInfo>()
				.ConfigureAwait(false);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass65_0
	{
		public SnapshotIdentifier snapshotIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		internal System.Threading.Tasks.Task<int> _003CDeleteSnapshot_003Eb__0(AircraftRepoInfo repoInfo, AircraftRollbackException? e)
		{
			string path = Path.Combine(repoInfo.HistoryAircraftRoot, snapshotIdentifier.Serialized);
			if (!_003C_003E4__this._fs.PathExists(path))
			{
				throw new AircraftSnapshotNotFoundException("The snapshot does not exist.");
			}
			_003C_003E4__this._fs.Delete(path);
			return System.Threading.Tasks.Task.FromResult<int>(0);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass67_0
	{
		[StructLayout(3)]
		private struct _003C_003CMountHistoricalSnapshot_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<TemporaryHangarDirectory> _003C_003Et__builder;

			public AircraftRepoInfo repoInfo;

			public _003C_003Ec__DisplayClass67_0 _003C_003E4__this;

			private TemporaryHangarDirectory _003CtempHangar_003E5__2;

			private ConfiguredTaskAwaiter<AircraftRepoInfo> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass67_0 _003C_003Ec__DisplayClass67_ = _003C_003E4__this;
				TemporaryHangarDirectory result;
				try
				{
					string snapShotArchivePathOrThrow = default(string);
					if (num != 0)
					{
						string text = Path.Combine(repoInfo.HistoryAircraftRoot, _003C_003Ec__DisplayClass67_.snapshotIdentifier.Serialized);
						if (!_003C_003Ec__DisplayClass67_._003C_003E4__this._fs.PathExists(text))
						{
							throw new AircraftSnapshotNotFoundException("The snapshot does not exist.");
						}
						snapShotArchivePathOrThrow = _003C_003Ec__DisplayClass67_._003C_003E4__this.GetSnapShotArchivePathOrThrow(text);
						_003CtempHangar_003E5__2 = TemporaryHangarDirectory.CreateNewUniqueHangar(_003C_003Ec__DisplayClass67_._003C_003E4__this._lowLevel);
					}
					try
					{
						ConfiguredTaskAwaiter<AircraftRepoInfo> awaiter;
						if (num != 0)
						{
							awaiter = _003C_003Ec__DisplayClass67_._003C_003E4__this.ExtractSnapshotIntoTemporaryHangar("MOUNT_SNAP", _003CtempHangar_003E5__2, _003C_003Ec__DisplayClass67_.aircraftIdentifier, snapShotArchivePathOrThrow).ConfigureAwait(false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftRepoInfo>, _003C_003CMountHistoricalSnapshot_003Eb__0_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftRepoInfo>);
							num = (_003C_003E1__state = -1);
						}
						awaiter.GetResult();
						result = _003CtempHangar_003E5__2;
					}
					catch
					{
						_003CtempHangar_003E5__2.Dispose();
						throw;
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CtempHangar_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CtempHangar_003E5__2 = null;
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public SnapshotIdentifier snapshotIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		[AsyncStateMachine(typeof(_003C_003CMountHistoricalSnapshot_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<TemporaryHangarDirectory> _003CMountHistoricalSnapshot_003Eb__0(AircraftRepoInfo repoInfo, AircraftRollbackException? e)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string text = Path.Combine(repoInfo.HistoryAircraftRoot, snapshotIdentifier.Serialized);
			if (!_003C_003E4__this._fs.PathExists(text))
			{
				throw new AircraftSnapshotNotFoundException("The snapshot does not exist.");
			}
			string snapShotArchivePathOrThrow = _003C_003E4__this.GetSnapShotArchivePathOrThrow(text);
			TemporaryHangarDirectory tempHangar = TemporaryHangarDirectory.CreateNewUniqueHangar(_003C_003E4__this._lowLevel);
			try
			{
				await _003C_003E4__this.ExtractSnapshotIntoTemporaryHangar("MOUNT_SNAP", tempHangar, aircraftIdentifier, snapShotArchivePathOrThrow).ConfigureAwait(false);
				return tempHangar;
			}
			catch
			{
				tempHangar.Dispose();
				throw;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass72_0
	{
		[StructLayout(3)]
		private struct _003C_003CListSnapshotsWithLock_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass72_0 _003C_003E4__this;

			public IObserver<AircraftSnapshotInfo> o;

			private _003C_003Ec__DisplayClass72_1 _003C_003E8__1;

			private object _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass72_0 CS_0024_003C_003E8__locals0 = _003C_003E4__this;
				try
				{
					if (num == 0)
					{
						goto IL_005c;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass72_1();
					_003C_003E8__1.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals0;
					_003C_003E8__1.o = o;
					string historyAircraftRoot = CS_0024_003C_003E8__locals0.repoInfo.HistoryAircraftRoot;
					if (CS_0024_003C_003E8__locals0._003C_003E4__this._fs.PathExists(historyAircraftRoot))
					{
						goto IL_005c;
					}
					_003C_003E8__1.o.OnCompleted();
					goto end_IL_000e;
					IL_005c:
					try
					{
						AsyncSubject<AircraftSnapshotInfo> asyncSubject;
						if (num != 0)
						{
							asyncSubject = CS_0024_003C_003E8__locals0._003C_003E4__this._fs.DirectoriesIn(historyAircraftRoot).ToObservable().Where<FileSystemEntry>((Func<FileSystemEntry, bool>)((FileSystemEntry fse) => SnapshotIdentifier.IsValid(fse.Name)))
								.SelectMany<FileSystemEntry, AircraftSnapshotInfo>((Func<FileSystemEntry, System.Threading.Tasks.Task<AircraftSnapshotInfo>>)([AsyncStateMachine(typeof(_003C_003CListSnapshotsWithLock_003Eb__2_003Ed))] async (FileSystemEntry fse) =>
								{
									//IL_0002: Unknown result type (might be due to invalid IL or missing references)
									//IL_0007: Unknown result type (might be due to invalid IL or missing references)
									_003C_003Ec__DisplayClass72_2 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass72_2
									{
										CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals0,
										fse = fse
									};
									SnapshotIdentifier snapshotId = SnapshotIdentifier.Parse(CS_0024_003C_003E8__locals1.fse.Name);
									try
									{
										string snapShotArchivePathOrThrow = CS_0024_003C_003E8__locals0._003C_003E4__this.GetSnapShotArchivePathOrThrow(CS_0024_003C_003E8__locals1.fse.FullPath);
										DateTimeOffset takenAt = CS_0024_003C_003E8__locals0._003C_003E4__this._fs.Stat(snapShotArchivePathOrThrow).ModifiedAt;
										CS_0024_003C_003E8__locals1.prototypeListData = null;
										CS_0024_003C_003E8__locals1.metaData = null;
										await TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed))] () =>
										{
											//IL_0002: Unknown result type (might be due to invalid IL or missing references)
											//IL_0007: Unknown result type (might be due to invalid IL or missing references)
											_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed _003C_003CListSnapshotsWithLock_003Eb__5_003Ed = default(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed);
											_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
											_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals1;
											_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003E1__state = -1;
											((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed);
											return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder)).Task;
										}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed))] () =>
										{
											//IL_0002: Unknown result type (might be due to invalid IL or missing references)
											//IL_0007: Unknown result type (might be due to invalid IL or missing references)
											_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed _003C_003CListSnapshotsWithLock_003Eb__6_003Ed = default(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed);
											_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
											_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals1;
											_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003E1__state = -1;
											((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed);
											return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder)).Task;
										}).DetachContext();
										long snapShotStorageSize = CS_0024_003C_003E8__locals0._003C_003E4__this.GetSnapShotStorageSize(CS_0024_003C_003E8__locals1.fse.FullPath);
										return new AircraftSnapshotInfo(CS_0024_003C_003E8__locals0.aircraftIdentifier, snapshotId, takenAt, CS_0024_003C_003E8__locals1.prototypeListData, CS_0024_003C_003E8__locals1.metaData, snapShotStorageSize);
									}
									catch (System.Exception e)
									{
										Logger.LogFormattedException(e, "Parsing the {0} {1} snapshot", CS_0024_003C_003E8__locals0.aircraftIdentifier, snapshotId);
										return (AircraftSnapshotInfo)null;
									}
								}))
								.Where<AircraftSnapshotInfo>((Func<AircraftSnapshotInfo, bool>)((AircraftSnapshotInfo snapshot) => snapshot != null))
								.Do<AircraftSnapshotInfo>((Action<AircraftSnapshotInfo>)delegate(AircraftSnapshotInfo snapshotInfo)
								{
									_003C_003E8__1.o.OnNext(snapshotInfo);
								})
								.LastOrDefaultAsync<AircraftSnapshotInfo>()
								.GetAwaiter<AircraftSnapshotInfo>();
							if (!asyncSubject.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = asyncSubject;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<AircraftSnapshotInfo>, _003C_003CListSnapshotsWithLock_003Eb__0_003Ed>(ref asyncSubject, ref this);
								return;
							}
						}
						else
						{
							asyncSubject = (AsyncSubject<AircraftSnapshotInfo>)_003C_003Eu__1;
							_003C_003Eu__1 = null;
							num = (_003C_003E1__state = -1);
						}
						asyncSubject.GetResult();
						_003C_003E8__1.o.OnCompleted();
					}
					catch (System.Exception ex)
					{
						_003C_003E8__1.o.OnError(ex);
					}
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		[StructLayout(3)]
		private struct _003C_003CListSnapshotsWithLock_003Eb__2_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AircraftSnapshotInfo> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass72_0 _003C_003E4__this;

			public FileSystemEntry fse;

			private _003C_003Ec__DisplayClass72_2 _003C_003E8__1;

			private SnapshotIdentifier _003CsnapshotId_003E5__2;

			private DateTimeOffset _003CtakenAt_003E5__3;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0119: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass72_0 _003C_003Ec__DisplayClass72_ = _003C_003E4__this;
				AircraftSnapshotInfo result;
				try
				{
					if (num != 0)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass72_2();
						_003C_003E8__1.CS_0024_003C_003E8__locals2 = _003C_003Ec__DisplayClass72_;
						_003C_003E8__1.fse = fse;
						_003CsnapshotId_003E5__2 = SnapshotIdentifier.Parse(_003C_003E8__1.fse.Name);
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							string snapShotArchivePathOrThrow = _003C_003Ec__DisplayClass72_._003C_003E4__this.GetSnapShotArchivePathOrThrow(_003C_003E8__1.fse.FullPath);
							_003CtakenAt_003E5__3 = _003C_003Ec__DisplayClass72_._003C_003E4__this._fs.Stat(snapShotArchivePathOrThrow).ModifiedAt;
							_003C_003E8__1.prototypeListData = null;
							_003C_003E8__1.metaData = null;
							ConfiguredTaskAwaitable val = TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed))] () =>
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed _003C_003CListSnapshotsWithLock_003Eb__5_003Ed = default(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed);
								_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
								_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003E4__this = _003C_003E8__1;
								_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003E1__state = -1;
								((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed);
								return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder)).Task;
							}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed))] () =>
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed _003C_003CListSnapshotsWithLock_003Eb__6_003Ed = default(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed);
								_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
								_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003E4__this = _003C_003E8__1;
								_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003E1__state = -1;
								((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed);
								return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder)).Task;
							}).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CListSnapshotsWithLock_003Eb__2_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
						long snapShotStorageSize = _003C_003Ec__DisplayClass72_._003C_003E4__this.GetSnapShotStorageSize(_003C_003E8__1.fse.FullPath);
						result = new AircraftSnapshotInfo(_003C_003Ec__DisplayClass72_.aircraftIdentifier, _003CsnapshotId_003E5__2, _003CtakenAt_003E5__3, _003C_003E8__1.prototypeListData, _003C_003E8__1.metaData, snapShotStorageSize);
					}
					catch (System.Exception e)
					{
						Logger.LogFormattedException(e, "Parsing the {0} {1} snapshot", _003C_003Ec__DisplayClass72_.aircraftIdentifier, _003CsnapshotId_003E5__2);
						result = null;
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003CsnapshotId_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CsnapshotId_003E5__2 = null;
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public AircraftRepoInfo repoInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public Func<FileSystemEntry, System.Threading.Tasks.Task<AircraftSnapshotInfo>> _003C_003E9__2;

		[AsyncStateMachine(typeof(_003C_003CListSnapshotsWithLock_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CListSnapshotsWithLock_003Eb__0(IObserver<AircraftSnapshotInfo> o, CancellationToken t)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed _003C_003CListSnapshotsWithLock_003Eb__0_003Ed = default(_003C_003CListSnapshotsWithLock_003Eb__0_003Ed);
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed.o = o;
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CListSnapshotsWithLock_003Eb__0_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CListSnapshotsWithLock_003Eb__2_003Ed))]
		internal async System.Threading.Tasks.Task<AircraftSnapshotInfo> _003CListSnapshotsWithLock_003Eb__2(FileSystemEntry fse)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass72_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass72_2
			{
				CS_0024_003C_003E8__locals2 = this,
				fse = fse
			};
			SnapshotIdentifier snapshotId = SnapshotIdentifier.Parse(CS_0024_003C_003E8__locals0.fse.Name);
			try
			{
				string snapShotArchivePathOrThrow = _003C_003E4__this.GetSnapShotArchivePathOrThrow(CS_0024_003C_003E8__locals0.fse.FullPath);
				DateTimeOffset takenAt = _003C_003E4__this._fs.Stat(snapShotArchivePathOrThrow).ModifiedAt;
				CS_0024_003C_003E8__locals0.prototypeListData = null;
				CS_0024_003C_003E8__locals0.metaData = null;
				await TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed _003C_003CListSnapshotsWithLock_003Eb__5_003Ed = default(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed);
					_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__5_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder)).Task;
				}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed _003C_003CListSnapshotsWithLock_003Eb__6_003Ed = default(_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed);
					_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass72_2._003C_003CListSnapshotsWithLock_003Eb__6_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder)).Task;
				}).DetachContext();
				long snapShotStorageSize = _003C_003E4__this.GetSnapShotStorageSize(CS_0024_003C_003E8__locals0.fse.FullPath);
				return new AircraftSnapshotInfo(aircraftIdentifier, snapshotId, takenAt, CS_0024_003C_003E8__locals0.prototypeListData, CS_0024_003C_003E8__locals0.metaData, snapShotStorageSize);
			}
			catch (System.Exception e)
			{
				Logger.LogFormattedException(e, "Parsing the {0} {1} snapshot", aircraftIdentifier, snapshotId);
				return null;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass72_1
	{
		public IObserver<AircraftSnapshotInfo> o;

		public _003C_003Ec__DisplayClass72_0 CS_0024_003C_003E8__locals1;

		internal void _003CListSnapshotsWithLock_003Eb__4(AircraftSnapshotInfo snapshotInfo)
		{
			o.OnNext(snapshotInfo);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass72_2
	{
		[StructLayout(3)]
		private struct _003C_003CListSnapshotsWithLock_003Eb__5_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass72_2 _003C_003E4__this;

			private ConfiguredTaskAwaiter<PersistedAircraftListData> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass72_2 _003C_003Ec__DisplayClass72_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<PersistedAircraftListData> awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedAircraftListData>);
						num = (_003C_003E1__state = -1);
						goto IL_00b2;
					}
					string path = Path.Combine(_003C_003Ec__DisplayClass72_.fse.FullPath, "list.json");
					if (_003C_003Ec__DisplayClass72_.CS_0024_003C_003E8__locals2._003C_003E4__this._fs.PathExists(path))
					{
						awaiter = _003C_003Ec__DisplayClass72_.CS_0024_003C_003E8__locals2._003C_003E4__this._persistence.Load<PersistedAircraftListData>(path).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedAircraftListData>, _003C_003CListSnapshotsWithLock_003Eb__5_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_00b2;
					}
					goto end_IL_000e;
					IL_00b2:
					PersistedAircraftListData result = awaiter.GetResult();
					_003C_003Ec__DisplayClass72_.prototypeListData = result.ToListData();
					end_IL_000e:;
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

		[StructLayout(3)]
		private struct _003C_003CListSnapshotsWithLock_003Eb__6_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass72_2 _003C_003E4__this;

			private ConfiguredTaskAwaiter<PersistedSnapshotMetadata> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass72_2 _003C_003Ec__DisplayClass72_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<PersistedSnapshotMetadata> awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedSnapshotMetadata>);
						num = (_003C_003E1__state = -1);
						goto IL_00b2;
					}
					string path = Path.Combine(_003C_003Ec__DisplayClass72_.fse.FullPath, "metadata.json");
					if (_003C_003Ec__DisplayClass72_.CS_0024_003C_003E8__locals2._003C_003E4__this._fs.PathExists(path))
					{
						awaiter = _003C_003Ec__DisplayClass72_.CS_0024_003C_003E8__locals2._003C_003E4__this._persistence.Load<PersistedSnapshotMetadata>(path).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedSnapshotMetadata>, _003C_003CListSnapshotsWithLock_003Eb__6_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_00b2;
					}
					_003C_003Ec__DisplayClass72_.metaData = new AircraftSnapshotMetaData(null, null);
					goto end_IL_000e;
					IL_00b2:
					PersistedSnapshotMetadata result = awaiter.GetResult();
					_003C_003Ec__DisplayClass72_.metaData = result.ToMetadata();
					end_IL_000e:;
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

		public FileSystemEntry fse;

		public AircraftListData prototypeListData;

		public AircraftSnapshotMetaData metaData;

		public _003C_003Ec__DisplayClass72_0 CS_0024_003C_003E8__locals2;

		[AsyncStateMachine(typeof(_003C_003CListSnapshotsWithLock_003Eb__5_003Ed))]
		internal System.Threading.Tasks.Task _003CListSnapshotsWithLock_003Eb__5()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CListSnapshotsWithLock_003Eb__5_003Ed _003C_003CListSnapshotsWithLock_003Eb__5_003Ed = default(_003C_003CListSnapshotsWithLock_003Eb__5_003Ed);
			_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003E4__this = this;
			_003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder)).Start<_003C_003CListSnapshotsWithLock_003Eb__5_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__5_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CListSnapshotsWithLock_003Eb__6_003Ed))]
		internal System.Threading.Tasks.Task _003CListSnapshotsWithLock_003Eb__6()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CListSnapshotsWithLock_003Eb__6_003Ed _003C_003CListSnapshotsWithLock_003Eb__6_003Ed = default(_003C_003CListSnapshotsWithLock_003Eb__6_003Ed);
			_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003E4__this = this;
			_003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder)).Start<_003C_003CListSnapshotsWithLock_003Eb__6_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__6_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass76_0
	{
		[StructLayout(3)]
		private struct _003C_003CGetFamilyDataForReleaseOrDefault_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<FamilyData> _003C_003Et__builder;

			public AircraftRepoInfo repoInfo;

			public _003C_003Ec__DisplayClass76_0 _003C_003E4__this;

			private string _003CpackageFamilyDataFilePath_003E5__2;

			private ConfiguredTaskAwaiter<PersistedFamilyData> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_01a3: Expected O, but got Unknown
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass76_0 _003C_003Ec__DisplayClass76_ = _003C_003E4__this;
				FamilyData result;
				try
				{
					if (num == 0)
					{
						goto IL_0055;
					}
					string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
					_003CpackageFamilyDataFilePath_003E5__2 = GetFamilyDataFilePath(familyDataDirPath, _003C_003Ec__DisplayClass76_.aircraftPackageFamily.ReleaseId);
					if (_003C_003Ec__DisplayClass76_._003C_003E4__this._fs.PathExists(_003CpackageFamilyDataFilePath_003E5__2))
					{
						goto IL_0055;
					}
					LogRepoMessage(LogLevel.Info, "GET_FAMILY_DATA", $"[{_003C_003Ec__DisplayClass76_.aircraftPackageFamily.AircraftIdentifier} - {_003C_003Ec__DisplayClass76_.aircraftPackageFamily.ReleaseId}] No family data found returning default.");
					result = new FamilyData();
					goto end_IL_000e;
					IL_0055:
					try
					{
						ConfiguredTaskAwaiter<PersistedFamilyData> awaiter;
						if (num != 0)
						{
							awaiter = _003C_003Ec__DisplayClass76_._003C_003E4__this._persistence.Load<PersistedFamilyData>(_003CpackageFamilyDataFilePath_003E5__2).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedFamilyData>, _003C_003CGetFamilyDataForReleaseOrDefault_003Eb__0_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedFamilyData>);
							num = (_003C_003E1__state = -1);
						}
						result = new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)awaiter.GetResult().Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value)));
					}
					catch (SerializationException val)
					{
						LogRepoError((System.Exception)val, "GET_FAMILY_DATA", $"[{_003C_003Ec__DisplayClass76_.aircraftPackageFamily.AircraftIdentifier} - {_003C_003Ec__DisplayClass76_.aircraftPackageFamily.ReleaseId}] Family data could not be parsed from file({_003CpackageFamilyDataFilePath_003E5__2})");
						throw;
					}
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CpackageFamilyDataFilePath_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CpackageFamilyDataFilePath_003E5__2 = null;
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public AircraftPackageFamily aircraftPackageFamily;

		public DirectoryBasedHangar _003C_003E4__this;

		[AsyncStateMachine(typeof(_003C_003CGetFamilyDataForReleaseOrDefault_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<FamilyData> _003CGetFamilyDataForReleaseOrDefault_003Eb__0(AircraftRepoInfo repoInfo)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
			string packageFamilyDataFilePath = GetFamilyDataFilePath(familyDataDirPath, aircraftPackageFamily.ReleaseId);
			if (_003C_003E4__this._fs.PathExists(packageFamilyDataFilePath))
			{
				try
				{
					return new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)(await _003C_003E4__this._persistence.Load<PersistedFamilyData>(packageFamilyDataFilePath).DetachContext()).Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value)));
				}
				catch (SerializationException val)
				{
					LogRepoError((System.Exception)val, "GET_FAMILY_DATA", $"[{aircraftPackageFamily.AircraftIdentifier} - {aircraftPackageFamily.ReleaseId}] Family data could not be parsed from file({packageFamilyDataFilePath})");
					throw;
				}
			}
			LogRepoMessage(LogLevel.Info, "GET_FAMILY_DATA", $"[{aircraftPackageFamily.AircraftIdentifier} - {aircraftPackageFamily.ReleaseId}] No family data found returning default.");
			return new FamilyData();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass78_0
	{
		[StructLayout(3)]
		private struct _003C_003CUpdateFamilyDataForRelease_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<DataChangeType> _003C_003Et__builder;

			public AircraftRepoInfo repoInfo;

			public _003C_003Ec__DisplayClass78_0 _003C_003E4__this;

			private string _003CpackageFamilyDataFilePath_003E5__2;

			private DataChangeType _003Cchange_003E5__3;

			private ConfiguredTaskAwaiter<PersistedFamilyData> _003C_003Eu__1;

			private ConfiguredTaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_019b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01be: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass78_0 _003C_003Ec__DisplayClass78_ = _003C_003E4__this;
				DataChangeType result;
				try
				{
					if (num != 0)
					{
						if (num == 1)
						{
							goto IL_017f;
						}
						string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
						_003CpackageFamilyDataFilePath_003E5__2 = GetFamilyDataFilePath(familyDataDirPath, _003C_003Ec__DisplayClass78_.aircraftPackageFamily.ReleaseId);
						_003C_003Ec__DisplayClass78_._003C_003E4__this._fs.CreateDirectory(familyDataDirPath);
						if (!_003C_003Ec__DisplayClass78_._003C_003E4__this._fs.PathExists(_003CpackageFamilyDataFilePath_003E5__2))
						{
							_003Cchange_003E5__3 = DataChangeType.Created;
							goto IL_0165;
						}
					}
					try
					{
						ConfiguredTaskAwaiter<PersistedFamilyData> awaiter;
						if (num != 0)
						{
							awaiter = _003C_003Ec__DisplayClass78_._003C_003E4__this._persistence.Load<PersistedFamilyData>(_003CpackageFamilyDataFilePath_003E5__2).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedFamilyData>, _003C_003CUpdateFamilyDataForRelease_003Eb__0_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedFamilyData>);
							num = (_003C_003E1__state = -1);
						}
						if (!new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)awaiter.GetResult().Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value))).Equals(_003C_003Ec__DisplayClass78_.data))
						{
							_003Cchange_003E5__3 = DataChangeType.Updated;
						}
						else
						{
							_003Cchange_003E5__3 = DataChangeType.None;
						}
					}
					catch (SerializationException)
					{
						_003Cchange_003E5__3 = DataChangeType.Updated;
					}
					goto IL_0165;
					IL_0165:
					PersistedFamilyData persistenceModel = default(PersistedFamilyData);
					if (_003Cchange_003E5__3 != DataChangeType.None)
					{
						persistenceModel = CreatePersistedFamilyData(_003C_003Ec__DisplayClass78_.data);
						goto IL_017f;
					}
					LogRepoMessage(LogLevel.Info, "UPDATE_FAMILY_DATA", $"[{_003C_003Ec__DisplayClass78_.aircraftPackageFamily.AircraftIdentifier} - {_003C_003Ec__DisplayClass78_.aircraftPackageFamily.ReleaseId}] Family data already up to date.");
					goto IL_033c;
					IL_033c:
					result = _003Cchange_003E5__3;
					goto end_IL_000e;
					IL_017f:
					try
					{
						ConfiguredTaskAwaiter awaiter2;
						if (num != 1)
						{
							ConfiguredTaskAwaitable val2 = _003C_003Ec__DisplayClass78_._003C_003E4__this._persistence.Commit(persistenceModel, _003CpackageFamilyDataFilePath_003E5__2).DetachContext();
							awaiter2 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CUpdateFamilyDataForRelease_003Eb__0_003Ed>(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					}
					catch (System.Exception e)
					{
						LogRepoError(e, "UPDATE_FAMILY_DATA", $"[{_003C_003Ec__DisplayClass78_.aircraftPackageFamily.AircraftIdentifier} - {_003C_003Ec__DisplayClass78_.aircraftPackageFamily.ReleaseId}] Updating family data.");
						throw;
					}
					LogRepoMessage(LogLevel.Info, "UPDATE_FAMILY_DATA", $"[{_003C_003Ec__DisplayClass78_.aircraftPackageFamily.AircraftIdentifier} - {_003C_003Ec__DisplayClass78_.aircraftPackageFamily.ReleaseId}] Updated family data.");
					goto IL_033c;
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CpackageFamilyDataFilePath_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CpackageFamilyDataFilePath_003E5__2 = null;
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public AircraftPackageFamily aircraftPackageFamily;

		public DirectoryBasedHangar _003C_003E4__this;

		public FamilyData data;

		[AsyncStateMachine(typeof(_003C_003CUpdateFamilyDataForRelease_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<DataChangeType> _003CUpdateFamilyDataForRelease_003Eb__0(AircraftRepoInfo repoInfo)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
			string packageFamilyDataFilePath = GetFamilyDataFilePath(familyDataDirPath, aircraftPackageFamily.ReleaseId);
			_003C_003E4__this._fs.CreateDirectory(familyDataDirPath);
			DataChangeType change;
			if (_003C_003E4__this._fs.PathExists(packageFamilyDataFilePath))
			{
				try
				{
					change = (new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)(await _003C_003E4__this._persistence.Load<PersistedFamilyData>(packageFamilyDataFilePath).DetachContext()).Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value))).Equals(data) ? DataChangeType.None : DataChangeType.Updated);
				}
				catch (SerializationException)
				{
					change = DataChangeType.Updated;
				}
			}
			else
			{
				change = DataChangeType.Created;
			}
			if (change != DataChangeType.None)
			{
				PersistedFamilyData persistenceModel = CreatePersistedFamilyData(data);
				try
				{
					await _003C_003E4__this._persistence.Commit(persistenceModel, packageFamilyDataFilePath).DetachContext();
				}
				catch (System.Exception e)
				{
					LogRepoError(e, "UPDATE_FAMILY_DATA", $"[{aircraftPackageFamily.AircraftIdentifier} - {aircraftPackageFamily.ReleaseId}] Updating family data.");
					throw;
				}
				LogRepoMessage(LogLevel.Info, "UPDATE_FAMILY_DATA", $"[{aircraftPackageFamily.AircraftIdentifier} - {aircraftPackageFamily.ReleaseId}] Updated family data.");
			}
			else
			{
				LogRepoMessage(LogLevel.Info, "UPDATE_FAMILY_DATA", $"[{aircraftPackageFamily.AircraftIdentifier} - {aircraftPackageFamily.ReleaseId}] Family data already up to date.");
			}
			return change;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass79_0
	{
		public AircraftPackageFamily aircraftPackageFamily;

		public DirectoryBasedHangar _003C_003E4__this;

		internal System.Threading.Tasks.Task<bool> _003CDeleteFamilyDataForRelease_003Eb__0(AircraftRepoInfo repoInfo)
		{
			string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
			string familyDataFilePath = GetFamilyDataFilePath(familyDataDirPath, aircraftPackageFamily.ReleaseId);
			_003C_003E4__this._fs.CreateDirectory(familyDataDirPath);
			bool flag = _003C_003E4__this._fs.DeleteIfExists(familyDataFilePath);
			LogRepoMessage(LogLevel.Info, "DELETE_FAMILY_DATA", $"[{aircraftPackageFamily.AircraftIdentifier} - {aircraftPackageFamily.ReleaseId}] Family data {(flag ? "was deleted" : "did not exist to delete")}.");
			return System.Threading.Tasks.Task.FromResult<bool>(flag);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CAircraftOpHarness_003Ed__42<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public CancellationToken token;

		public LockSelector lockSelector;

		public Func<AircraftRepoInfo, System.Threading.Tasks.Task<TResult>> readAircraft;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			TResult result2;
			try
			{
				ConfiguredTaskAwaiter awaiter2;
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				System.IDisposable result;
				AsyncReaderWriterLock lockForPathUnlocked;
				switch (num)
				{
				default:
				{
					ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
					_003CrepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
					ConfiguredTaskAwaitable val = directoryBasedHangar.CheckForAndPerformMigration(_003CrepoInfo_003E5__2, token).DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAircraftOpHarness_003Ed__42<TResult>>(ref awaiter2, ref this);
						return;
					}
					goto IL_00ae;
				}
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00ae;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_0132;
				case 2:
					break;
					IL_0132:
					result = awaiter.GetResult();
					_003C_003E7__wrap2 = result;
					break;
					IL_00ae:
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					lockForPathUnlocked = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath);
					awaiter = lockSelector(lockForPathUnlocked).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CAircraftOpHarness_003Ed__42<TResult>>(ref awaiter, ref this);
						return;
					}
					goto IL_0132;
				}
				try
				{
					ConfiguredTaskAwaiter<TResult> awaiter3;
					if (num != 2)
					{
						directoryBasedHangar.ThrowNotFoundIfAircraftDirectoryIsMissing(_003CrepoInfo_003E5__2);
						awaiter3 = ((Func<AircraftRepoInfo, System.Threading.Tasks.Task<AircraftRepoInfo>>)(object)readAircraft).Invoke(_003CrepoInfo_003E5__2).DetachContext().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CAircraftOpHarness_003Ed__42<TResult>>(ref awaiter3, ref this);
							return;
						}
					}
					else
					{
						awaiter3 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<TResult>);
						num = (_003C_003E1__state = -1);
					}
					result2 = awaiter3.GetResult();
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CrepoInfo_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrepoInfo_003E5__2 = null;
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
	private struct _003CAircraftOpHarnessWithRollbackHandling_003Ed__45<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public CancellationToken token;

		public Func<AsyncReaderWriterLock, AwaitableDisposable<System.IDisposable>> lockStategy;

		public Func<AircraftRepoInfo, AircraftRollbackException?, System.Threading.Tasks.Task<TResult>> readAircraft;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private AircraftRollbackException _003CrollbackError_003E5__3;

		private Guid? _003CerrorGuid_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private System.IDisposable _003C_003E7__wrap4;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Expected O, but got Unknown
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			TResult result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				System.IDisposable result;
				switch (num)
				{
				default:
					ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
					_003CrepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
					_003CrollbackError_003E5__3 = null;
					_003CerrorGuid_003E5__4 = null;
					goto case 0;
				case 0:
				{
					try
					{
						ConfiguredTaskAwaiter awaiter2;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = directoryBasedHangar.CheckForAndPerformMigration(_003CrepoInfo_003E5__2, token).DetachContext();
							awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAircraftOpHarnessWithRollbackHandling_003Ed__45<TResult>>(ref awaiter2, ref this);
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
					}
					catch (AircraftRollbackException ex)
					{
						_003CerrorGuid_003E5__4 = Guid.NewGuid();
						Logger.Log(LogLevel.Warning, $"[{_003CerrorGuid_003E5__4}] Migration rollback detected. Continuing to try and read anyways.");
						_003CrollbackError_003E5__3 = ex;
					}
					AsyncReaderWriterLock lockForPathUnlocked = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath);
					awaiter = lockStategy.Invoke(lockForPathUnlocked).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CAircraftOpHarnessWithRollbackHandling_003Ed__45<TResult>>(ref awaiter, ref this);
						return;
					}
					goto IL_01a3;
				}
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_01a3;
				case 2:
					break;
					IL_01a3:
					result = awaiter.GetResult();
					_003C_003E7__wrap4 = result;
					break;
				}
				try
				{
					if (num != 2)
					{
						directoryBasedHangar.ThrowNotFoundIfAircraftDirectoryIsMissing(_003CrepoInfo_003E5__2);
					}
					try
					{
						ConfiguredTaskAwaiter<TResult> awaiter3;
						if (num != 2)
						{
							awaiter3 = ((Func<AircraftRepoInfo, AircraftRollbackException, System.Threading.Tasks.Task<AircraftRepoInfo>>)(object)readAircraft).Invoke(_003CrepoInfo_003E5__2, _003CrollbackError_003E5__3).DetachContext().GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter3;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CAircraftOpHarnessWithRollbackHandling_003Ed__45<TResult>>(ref awaiter3, ref this);
								return;
							}
						}
						else
						{
							awaiter3 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<TResult>);
							num = (_003C_003E1__state = -1);
						}
						result2 = awaiter3.GetResult();
					}
					catch (System.Exception ex2) when (_003CrollbackError_003E5__3 != null)
					{
						AircraftRollbackException ex3 = new AircraftRollbackException("An error occurred and could be due to the migration error.", (System.Exception)new AggregateException(new System.Exception[2] { _003CrollbackError_003E5__3, ex2 }));
						Logger.LogFormattedException(ex3, $"[{_003CerrorGuid_003E5__4}] Read op");
						throw ex3;
					}
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap4 != null)
					{
						_003C_003E7__wrap4.Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CrepoInfo_003E5__2 = null;
				_003CrollbackError_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrepoInfo_003E5__2 = null;
			_003CrollbackError_003E5__3 = null;
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
	private struct _003CAreMigrationsNeeded_003Ed__30 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		private List<System.Threading.Tasks.Task<bool>> _003CcheckTasks_003E5__2;

		private System.Threading.Tasks.Task<bool> _003Cfirst_003E5__3;

		private ConfiguredTaskAwaiter<System.Threading.Tasks.Task<bool>> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			bool result;
			try
			{
				ConfiguredTaskAwaiter<bool> awaiter;
				if (num != 0)
				{
					if (num != 1)
					{
						string rootDirectory = CS_0024_003C_003E8__locals0.RootDirectory;
						if (CS_0024_003C_003E8__locals0._fs.PathExists(rootDirectory))
						{
							_003CcheckTasks_003E5__2 = Enumerable.ToList<System.Threading.Tasks.Task<bool>>(Enumerable.Select<FileSystemEntry, System.Threading.Tasks.Task<bool>>(CS_0024_003C_003E8__locals0._fs.DirectoriesIn(rootDirectory), (Func<FileSystemEntry, System.Threading.Tasks.Task<bool>>)([AsyncStateMachine(typeof(_003C_003CAreMigrationsNeeded_003Eb__30_0_003Ed))] [CompilerGenerated] async (FileSystemEntry aircraftDirectory) =>
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								if (!AircraftIdentifier.IsValidIdentifier(aircraftDirectory.Name))
								{
									return false;
								}
								AircraftRepoInfo repoInfo = CreateRepoInfo(aircraftDirectory.FullPath);
								using (await CS_0024_003C_003E8__locals0.GetLockForPathUnlocked(repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
								{
									if (!CS_0024_003C_003E8__locals0._fs.PathExists(repoInfo.RootPath))
									{
										return false;
									}
									return CS_0024_003C_003E8__locals0.NeedsAMigration(repoInfo.RootPath);
								}
							})));
							goto IL_015c;
						}
						goto IL_0174;
					}
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<bool>);
					num = (_003C_003E1__state = -1);
					goto IL_0136;
				}
				ConfiguredTaskAwaiter<System.Threading.Tasks.Task<bool>> awaiter2 = _003C_003Eu__1;
				_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Threading.Tasks.Task<bool>>);
				num = (_003C_003E1__state = -1);
				goto IL_00c5;
				IL_015c:
				if (_003CcheckTasks_003E5__2.Count > 0)
				{
					awaiter2 = System.Threading.Tasks.Task.WhenAny<bool>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<bool>>)_003CcheckTasks_003E5__2).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task<bool>>, _003CAreMigrationsNeeded_003Ed__30>(ref awaiter2, ref this);
						return;
					}
					goto IL_00c5;
				}
				_003CcheckTasks_003E5__2 = null;
				goto IL_0174;
				IL_0136:
				if (!awaiter.GetResult())
				{
					_003CcheckTasks_003E5__2.Remove(_003Cfirst_003E5__3);
					_003Cfirst_003E5__3 = null;
					goto IL_015c;
				}
				result = true;
				goto end_IL_000e;
				IL_00c5:
				System.Threading.Tasks.Task<bool> result2 = awaiter2.GetResult();
				_003Cfirst_003E5__3 = result2;
				awaiter = _003Cfirst_003E5__3.DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CAreMigrationsNeeded_003Ed__30>(ref awaiter, ref this);
					return;
				}
				goto IL_0136;
				IL_0174:
				result = false;
				end_IL_000e:;
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
	private struct _003CCheckForAndPerformMigration_003Ed__123 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftRepoInfo repoInfo;

		public CancellationToken token;

		private AsyncReaderWriterLock _003CpathLock_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private System.IDisposable _003CupgradeableLock_003E5__3;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				System.IDisposable result;
				System.IDisposable result2;
				bool flag;
				switch (num)
				{
				default:
					_003CpathLock_003E5__2 = directoryBasedHangar.GetLockForPathUnlocked(repoInfo.RootPath);
					awaiter = _003CpathLock_003E5__2.ReaderLockAsync(token).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCheckForAndPerformMigration_003Ed__123>(ref awaiter, ref this);
						return;
					}
					goto IL_00a7;
				case 0:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_00a7;
				case 1:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_0153;
				case 2:
					break;
					IL_0153:
					result = awaiter.GetResult();
					_003CupgradeableLock_003E5__3 = result;
					break;
					IL_00a7:
					result2 = awaiter.GetResult();
					try
					{
						directoryBasedHangar.ThrowNotFoundIfAircraftDirectoryIsMissing(repoInfo);
						flag = directoryBasedHangar.NeedsAMigration(repoInfo.RootPath);
					}
					finally
					{
						if (num < 0)
						{
							result2?.Dispose();
						}
					}
					if (flag)
					{
						awaiter = _003CpathLock_003E5__2.WriterLockAsync(token).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCheckForAndPerformMigration_003Ed__123>(ref awaiter, ref this);
							return;
						}
						goto IL_0153;
					}
					goto end_IL_000e;
				}
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					if (num != 2)
					{
						ConfiguredTaskAwaitable val = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(repoInfo, token).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCheckForAndPerformMigration_003Ed__123>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				}
				finally
				{
					if (num < 0 && _003CupgradeableLock_003E5__3 != null)
					{
						_003CupgradeableLock_003E5__3.Dispose();
					}
				}
				_003CupgradeableLock_003E5__3 = null;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CpathLock_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CpathLock_003E5__2 = null;
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
	private struct _003CCopyAircraft_003Ed__47 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

		public AircraftIdentifier copyIdentifier;

		public AircraftListData startingInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		private AircraftIdentifier _003CaircraftIdentifier_003E5__2;

		private AircraftRepoInfo _003CrepoInfo_003E5__3;

		private string _003CaircraftDirectoryPath_003E5__4;

		private AircraftRepoInfo _003CprotoInfo_003E5__5;

		private string _003CprototypeDirectory_003E5__6;

		private AsyncReaderWriterLock _003CpathToCopyLock_003E5__7;

		private ConfiguredTaskAwaiter<AircraftIdentifier> _003C_003Eu__1;

		private System.IDisposable _003C_003E7__wrap7;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__2;

		private bool _003CalreadyExisted_003E5__9;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private ConfiguredTaskAwaiter<AircraftSnapshotInfo> _003C_003Eu__4;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__5;

		private void MoveNext()
		{
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Expected O, but got Unknown
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0600: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_0548: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0671: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_047a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotData result3;
			try
			{
				ConfiguredTaskAwaiter<AircraftIdentifier> awaiter2;
				AsyncReaderWriterLock lockForPathUnlocked;
				AircraftSnapshotData aircraftSnapshotData;
				AircraftIdentifier result;
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				System.IDisposable result2;
				switch (num)
				{
				default:
					copyIdentifier.ThrowIfDefault();
					_003CaircraftIdentifier_003E5__2 = startingInfo.AircraftIdentifier;
					if (_003CaircraftIdentifier_003E5__2 == AircraftIdentifier.Default || _003CaircraftIdentifier_003E5__2 == copyIdentifier)
					{
						awaiter2 = directoryBasedHangar.ReserveFirstUnusedIdFrom(AircraftIdentifier.GenerateIdentifiers(startingInfo.AirframeCCT, "C")).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftIdentifier>, _003CCopyAircraft_003Ed__47>(ref awaiter2, ref this);
							return;
						}
						goto IL_00eb;
					}
					goto IL_0113;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftIdentifier>);
					num = (_003C_003E1__state = -1);
					goto IL_00eb;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_0214;
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
					break;
					IL_0113:
					_003CrepoInfo_003E5__3 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003CaircraftIdentifier_003E5__2);
					_003CaircraftDirectoryPath_003E5__4 = _003CrepoInfo_003E5__3.RootPath;
					copyIdentifier.ThrowIfDefault((System.Exception?)new ArgumentException("invalid copy aircraft identifier."));
					_003CprotoInfo_003E5__5 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(copyIdentifier);
					_003CprototypeDirectory_003E5__6 = _003CprotoInfo_003E5__5.RootPath;
					_003CpathToCopyLock_003E5__7 = directoryBasedHangar.GetLockForPathUnlocked(_003CprototypeDirectory_003E5__6);
					lockForPathUnlocked = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__3.RootPath);
					if (_003CpathToCopyLock_003E5__7 == lockForPathUnlocked)
					{
						ThrowInternalRepoError();
					}
					aircraftSnapshotData = null;
					awaiter = lockForPathUnlocked.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCopyAircraft_003Ed__47>(ref awaiter, ref this);
						return;
					}
					goto IL_0214;
					IL_00eb:
					result = awaiter2.GetResult();
					_003CaircraftIdentifier_003E5__2 = result;
					startingInfo = startingInfo.WithNewIdentifier(_003CaircraftIdentifier_003E5__2);
					goto IL_0113;
					IL_0214:
					result2 = awaiter.GetResult();
					_003C_003E7__wrap7 = result2;
					break;
				}
				try
				{
					if ((uint)(num - 2) > 4u)
					{
						directoryBasedHangar.ThrowIfAircraftDirectoryIsNotEmpty(_003CaircraftIdentifier_003E5__2, _003CaircraftDirectoryPath_003E5__4);
						_003CalreadyExisted_003E5__9 = directoryBasedHangar._fs.PathExists(_003CaircraftDirectoryPath_003E5__4);
					}
					try
					{
						ConfiguredTaskAwaitable val;
						ConfiguredTaskAwaiter awaiter5;
						ConfiguredTaskAwaiter<AircraftSnapshotInfo> awaiter4;
						ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter3;
						switch (num)
						{
						default:
							try
							{
								if (num != 2)
								{
									val = directoryBasedHangar.CheckForAndPerformMigration(_003CprotoInfo_003E5__5).DetachContext();
									awaiter5 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter5)).IsCompleted)
									{
										num = (_003C_003E1__state = 2);
										_003C_003Eu__3 = awaiter5;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCopyAircraft_003Ed__47>(ref awaiter5, ref this);
										return;
									}
								}
								else
								{
									awaiter5 = _003C_003Eu__3;
									_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
									num = (_003C_003E1__state = -1);
								}
								((ConfiguredTaskAwaiter)(ref awaiter5)).GetResult();
							}
							catch (System.Exception e)
							{
								LogRepoError(e, "Copy aircraft", "Could not migrate the prototype directory '{0}'.", _003CprototypeDirectory_003E5__6);
								throw;
							}
							awaiter = _003CpathToCopyLock_003E5__7.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 3);
								_003C_003Eu__2 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCopyAircraft_003Ed__47>(ref awaiter, ref this);
								return;
							}
							goto IL_037a;
						case 3:
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.IDisposable>);
							num = (_003C_003E1__state = -1);
							goto IL_037a;
						case 4:
							awaiter4 = _003C_003Eu__4;
							_003C_003Eu__4 = default(ConfiguredTaskAwaiter<AircraftSnapshotInfo>);
							num = (_003C_003E1__state = -1);
							goto IL_04d4;
						case 5:
							try
							{
								if (num != 5)
								{
									val = directoryBasedHangar.UpdateListDataWithLock("Copy aircraft", _003CaircraftIdentifier_003E5__2, _003CrepoInfo_003E5__3, startingInfo).DetachContext();
									awaiter5 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
									if (!((ConfiguredTaskAwaiter)(ref awaiter5)).IsCompleted)
									{
										num = (_003C_003E1__state = 5);
										_003C_003Eu__3 = awaiter5;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCopyAircraft_003Ed__47>(ref awaiter5, ref this);
										return;
									}
								}
								else
								{
									awaiter5 = _003C_003Eu__3;
									_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
									num = (_003C_003E1__state = -1);
								}
								((ConfiguredTaskAwaiter)(ref awaiter5)).GetResult();
							}
							catch (System.Exception e2)
							{
								LogRepoError(e2, "Create new aircraft", "Could not serialize new aircraft JSON record '{0}'.", _003CaircraftIdentifier_003E5__2);
								throw;
							}
							awaiter3 = directoryBasedHangar.GetAircraftSnapshotWithLock(_003CaircraftIdentifier_003E5__2, _003CrepoInfo_003E5__3).DetachContext().GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_003C_003E1__state = 6);
								_003C_003Eu__5 = awaiter3;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CCopyAircraft_003Ed__47>(ref awaiter3, ref this);
								return;
							}
							break;
						case 6:
							{
								awaiter3 = _003C_003Eu__5;
								_003C_003Eu__5 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
								num = (_003C_003E1__state = -1);
								break;
							}
							IL_04d4:
							awaiter4.GetResult();
							LogRepoMessage(LogLevel.Error, "Create new aircraft", (string?)_003CaircraftIdentifier_003E5__2 + " - Saving to disk.");
							goto case 5;
							IL_037a:
							result2 = awaiter.GetResult();
							try
							{
								if (!directoryBasedHangar._fs.PathExists(_003CprototypeDirectory_003E5__6))
								{
									LogRepoMessage(LogLevel.Error, "Copy aircraft", "Missing prototype directory '{0}'.", _003CprototypeDirectory_003E5__6);
									throw new DirectoryNotFoundException("Missing aircraft prototype directory: " + _003CprototypeDirectory_003E5__6);
								}
								LogRepoMessage(LogLevel.Info, "Copy aircraft", (string?)_003CaircraftIdentifier_003E5__2 + " - Copying existing aircraft.");
								try
								{
									directoryBasedHangar._fs.CopyDirectory(_003CprototypeDirectory_003E5__6, _003CaircraftDirectoryPath_003E5__4);
									LogRepoMessage(LogLevel.Info, "Copy aircraft", (string?)_003CaircraftIdentifier_003E5__2 + " - Copied existing aircraft.");
								}
								catch (System.Exception e3)
								{
									LogRepoError(e3, "Copy aircraft", "Could not copy prototype directory '{0}'.", _003CprototypeDirectory_003E5__6);
									throw;
								}
							}
							finally
							{
								if (num < 0)
								{
									result2?.Dispose();
								}
							}
							awaiter4 = directoryBasedHangar.TakePrototypeSnapShotWithLock("Copy aircraft", _003CaircraftIdentifier_003E5__2, _003CrepoInfo_003E5__3).DetachContext().GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = (_003C_003E1__state = 4);
								_003C_003Eu__4 = awaiter4;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotInfo>, _003CCopyAircraft_003Ed__47>(ref awaiter4, ref this);
								return;
							}
							goto IL_04d4;
						}
						aircraftSnapshotData = awaiter3.GetResult();
					}
					catch (System.Exception e4)
					{
						LogRepoError(e4, "Copy aircraft");
						if (_003CalreadyExisted_003E5__9)
						{
							directoryBasedHangar.DeleteAircraftDirectory("Create new aircraft", _003CaircraftDirectoryPath_003E5__4);
						}
						throw;
					}
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap7 != null)
					{
						_003C_003E7__wrap7.Dispose();
					}
				}
				_003C_003E7__wrap7 = null;
				if (aircraftSnapshotData == null)
				{
					throw new InvalidOperationException("Internal repo error. Did not set created snapshot.");
				}
				directoryBasedHangar.PostAircraftChange(_003CaircraftIdentifier_003E5__2, DataChangeType.Created);
				result3 = aircraftSnapshotData;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CaircraftIdentifier_003E5__2 = default(AircraftIdentifier);
				_003CrepoInfo_003E5__3 = null;
				_003CaircraftDirectoryPath_003E5__4 = null;
				_003CprotoInfo_003E5__5 = null;
				_003CprototypeDirectory_003E5__6 = null;
				_003CpathToCopyLock_003E5__7 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CaircraftIdentifier_003E5__2 = default(AircraftIdentifier);
			_003CrepoInfo_003E5__3 = null;
			_003CaircraftDirectoryPath_003E5__4 = null;
			_003CprotoInfo_003E5__5 = null;
			_003CprototypeDirectory_003E5__6 = null;
			_003CpathToCopyLock_003E5__7 = null;
			_003C_003Et__builder.SetResult(result3);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreateNewAircraft_003Ed__46 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftListData startingInfo;

		private _003C_003Ec__DisplayClass46_0 _003C_003E8__1;

		private string _003CaircraftDirectoryPath_003E5__2;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__1;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__2;

		private bool _003CalreadyExisted_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotData result3;
			try
			{
				ConfiguredTaskAwaiter<AircraftListData> awaiter2;
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				System.IDisposable result;
				AircraftListData result2;
				AircraftSnapshotData aircraftSnapshotData;
				switch (num)
				{
				default:
					_003C_003E8__1 = new _003C_003Ec__DisplayClass46_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.startingInfo = startingInfo;
					awaiter2 = HangarUtil.PrepareStartingInfoWithUniqueId(_003C_003E8__1.startingInfo, AircraftIdentifier.GenerateIdentifiers(_003C_003E8__1.startingInfo.AirframeCCT, "R"), directoryBasedHangar.ReserveAircraftDirectoryIfNotTaken).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003CCreateNewAircraft_003Ed__46>(ref awaiter2, ref this);
						return;
					}
					goto IL_00dd;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
					num = (_003C_003E1__state = -1);
					goto IL_00dd;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_01c1;
				case 2:
					break;
					IL_01c1:
					result = awaiter.GetResult();
					_003C_003E7__wrap2 = result;
					break;
					IL_00dd:
					result2 = awaiter2.GetResult();
					_003C_003E8__1.startingInfo = result2;
					_003C_003E8__1.aircraftIdentifier = _003C_003E8__1.startingInfo.AircraftIdentifier;
					_003C_003E8__1.repoInfo = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003C_003E8__1.aircraftIdentifier);
					_003CaircraftDirectoryPath_003E5__2 = _003C_003E8__1.repoInfo.RootPath;
					aircraftSnapshotData = null;
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003C_003E8__1.repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCreateNewAircraft_003Ed__46>(ref awaiter, ref this);
						return;
					}
					goto IL_01c1;
				}
				try
				{
					if (num != 2)
					{
						_003CalreadyExisted_003E5__4 = directoryBasedHangar._fs.PathExists(_003CaircraftDirectoryPath_003E5__2);
					}
					try
					{
						ConfiguredTaskAwaiter awaiter3;
						if (num != 2)
						{
							if (!_003CalreadyExisted_003E5__4)
							{
								LogRepoMessage(LogLevel.Info, "Create new aircraft", "Creating aircraft directory '{0}'.", _003CaircraftDirectoryPath_003E5__2);
								directoryBasedHangar._fs.CreateDirectory(_003CaircraftDirectoryPath_003E5__2);
							}
							else
							{
								directoryBasedHangar.ThrowIfAircraftDirectoryIsNotEmpty(_003C_003E8__1.aircraftIdentifier, _003CaircraftDirectoryPath_003E5__2);
								LogRepoMessage(LogLevel.Info, "Create new aircraft", "Using reserved aircraft directory '{0}'.", _003CaircraftDirectoryPath_003E5__2);
							}
							ConfiguredTaskAwaitable val = TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__0_003Ed))] () =>
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__0_003Ed _003C_003CCreateNewAircraft_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__0_003Ed);
								_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
								_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003E4__this = _003C_003E8__1;
								_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003E1__state = -1;
								((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__0_003Ed>(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed);
								return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder)).Task;
							}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__1_003Ed))] () =>
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_0007: Unknown result type (might be due to invalid IL or missing references)
								_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__1_003Ed _003C_003CCreateNewAircraft_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__1_003Ed);
								_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
								_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003E4__this = _003C_003E8__1;
								_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003E1__state = -1;
								((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__1_003Ed>(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed);
								return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder)).Task;
							}).DetachContext();
							awaiter3 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter3;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCreateNewAircraft_003Ed__46>(ref awaiter3, ref this);
								return;
							}
						}
						else
						{
							awaiter3 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
						aircraftSnapshotData = AircraftSnapshotData.FromListData(_003C_003E8__1.startingInfo);
					}
					catch (System.Exception)
					{
						if (_003CalreadyExisted_003E5__4)
						{
							directoryBasedHangar.DeleteAircraftDirectory("Create new aircraft", _003CaircraftDirectoryPath_003E5__2);
						}
						throw;
					}
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				if (aircraftSnapshotData == null)
				{
					throw ThrowInternalRepoError();
				}
				directoryBasedHangar.PostAircraftChange(_003C_003E8__1.aircraftIdentifier, DataChangeType.Created);
				result3 = aircraftSnapshotData;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CaircraftDirectoryPath_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CaircraftDirectoryPath_003E5__2 = null;
			_003C_003Et__builder.SetResult(result3);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCreateOrUpdateSimulator_003Ed__48 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SimInstallData simulator;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		private _003C_003Ec__DisplayClass48_0 _003C_003E8__1;

		private _003C_003Ec__DisplayClass48_1 _003C_003E8__2;

		private SimInstallKey _003CpreviouslyActiveInSlot_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private ConfiguredTaskAwaiter<PersistedSimPanelInfo> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_0419: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 2u)
					{
						goto IL_011f;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass48_0();
					_003C_003E8__1.simulator = simulator;
					_003C_003E8__1.aircraftIdentifier = aircraftIdentifier;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					ThrowIfAircraftIdentifierIsInvalid(_003C_003E8__1.aircraftIdentifier);
					_003C_003E8__1.repoInfo = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003C_003E8__1.aircraftIdentifier);
					string rootPath = _003C_003E8__1.repoInfo.RootPath;
					_003CpreviouslyActiveInSlot_003E5__2 = null;
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(rootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCreateOrUpdateSimulator_003Ed__48>(ref awaiter, ref this);
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
				goto IL_011f;
				IL_011f:
				try
				{
					ConfiguredTaskAwaitable val;
					ConfiguredTaskAwaiter awaiter2;
					ConfiguredTaskAwaiter<PersistedSimPanelInfo> awaiter3;
					PersistedSimPanelInfo persistedSimPanelInfo2;
					PersistedSimPanelInfo persistedSimPanelInfo;
					List<SimInstallKey> val2;
					List<SimInstallKey> val3;
					switch (num)
					{
					default:
						_003C_003E8__2 = new _003C_003Ec__DisplayClass48_1();
						_003C_003E8__2.CS_0024_003C_003E8__locals1 = _003C_003E8__1;
						val = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(_003C_003E8__2.CS_0024_003C_003E8__locals1.repoInfo).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCreateOrUpdateSimulator_003Ed__48>(ref awaiter2, ref this);
							return;
						}
						goto IL_01c9;
					case 1:
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_01c9;
					case 2:
						awaiter3 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<PersistedSimPanelInfo>);
						num = (_003C_003E1__state = -1);
						goto IL_0282;
					case 3:
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							break;
						}
						IL_0294:
						persistedSimPanelInfo2 = persistedSimPanelInfo;
						_003C_003E8__2.currentPanel = persistedSimPanelInfo2;
						val2 = Enumerable.ToList<SimInstallKey>(persistedSimPanelInfo2.ActiveSimulatorKeys);
						val3 = val2;
						if (_003C_003E8__2.CS_0024_003C_003E8__locals1.simulator.IsActiveSimulator != val3.Contains(_003C_003E8__2.CS_0024_003C_003E8__locals1.simulator.InstallationKey))
						{
							val3 = Enumerable.ToList<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)val3);
							if (_003C_003E8__2.CS_0024_003C_003E8__locals1.simulator.IsActiveSimulator)
							{
								SimInstallKey simInstallKey = Enumerable.FirstOrDefault<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)val3, (Func<SimInstallKey, bool>)((SimInstallKey s) => s.Slot == _003C_003E8__2.CS_0024_003C_003E8__locals1.simulator.InstallationKey.Slot));
								if (simInstallKey != null)
								{
									_003CpreviouslyActiveInSlot_003E5__2 = simInstallKey;
									val3.Remove(_003CpreviouslyActiveInSlot_003E5__2);
								}
								val3.Add(_003C_003E8__2.CS_0024_003C_003E8__locals1.simulator.InstallationKey);
							}
							else
							{
								val3.Remove(_003C_003E8__2.CS_0024_003C_003E8__locals1.simulator.InstallationKey);
							}
						}
						_003C_003E8__2.activeChanged = val2 != val3;
						if (_003C_003E8__2.activeChanged)
						{
							_003C_003E8__2.currentPanel = new PersistedSimPanelInfo
							{
								ActiveSimulatorKeys = (System.Collections.Generic.IEnumerable<SimInstallKey>)val3
							};
							_003C_003E8__2.currentPanel = _003C_003E8__2.currentPanel.FixTooManyActiveSimsInSlot();
						}
						val = TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass48_0._003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass48_0._003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass48_0._003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed);
							_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003E4__this = _003C_003E8__2.CS_0024_003C_003E8__locals1;
							_003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass48_0._003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed>(ref _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__0_003Ed._003C_003Et__builder)).Task;
						}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass48_1._003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass48_1._003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass48_1._003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed);
							_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003E4__this = _003C_003E8__2;
							_003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass48_1._003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed>(ref _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateOrUpdateSimulator_003Eb__1_003Ed._003C_003Et__builder)).Task;
						}).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 3);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCreateOrUpdateSimulator_003Ed__48>(ref awaiter2, ref this);
							return;
						}
						break;
						IL_01c9:
						((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
						_003C_003E8__2.panelPath = Path.Combine(_003C_003E8__2.CS_0024_003C_003E8__locals1.repoInfo.CurrentAircraftRoot, "panel.json");
						if (directoryBasedHangar._fs.PathExists(_003C_003E8__2.panelPath))
						{
							awaiter3 = directoryBasedHangar._persistence.Load<PersistedSimPanelInfo>(_003C_003E8__2.panelPath).DetachContext().GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter3;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedSimPanelInfo>, _003CCreateOrUpdateSimulator_003Ed__48>(ref awaiter3, ref this);
								return;
							}
							goto IL_0282;
						}
						persistedSimPanelInfo = new PersistedSimPanelInfo();
						goto IL_0294;
						IL_0282:
						persistedSimPanelInfo = awaiter3.GetResult();
						goto IL_0294;
					}
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					_003C_003E8__2 = null;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				if (_003CpreviouslyActiveInSlot_003E5__2 != null)
				{
					directoryBasedHangar.PostSimulatorChange(new SimAircraftKey(_003C_003E8__1.aircraftIdentifier, _003CpreviouslyActiveInSlot_003E5__2), DataChangeType.Updated);
				}
				directoryBasedHangar.PostSimulatorChange(_003C_003E8__1.simulator.AircraftSimKey, DataChangeType.Updated);
				directoryBasedHangar.PostAircraftChange(_003C_003E8__1.aircraftIdentifier, DataChangeType.Updated);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CpreviouslyActiveInSlot_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CpreviouslyActiveInSlot_003E5__2 = null;
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
	private struct _003CCreateSnapshot_003Ed__62 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotInfo> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public AircraftSnapshotMetaData metadata;

		private _003C_003Ec__DisplayClass62_0 _003C_003E8__1;

		private System.IDisposable _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<AircraftSnapshotInfo> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotInfo info;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_012e;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass62_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.aircraftIdentifier = aircraftIdentifier;
					_003C_003E8__1.metadata = metadata;
					ThrowIfAircraftIdentifierIsInvalid(_003C_003E8__1.aircraftIdentifier);
					if (_003C_003E8__1.metadata == null)
					{
						throw new ArgumentNullException("metadata");
					}
					_003C_003E8__1.repoInfo = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003C_003E8__1.aircraftIdentifier);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003C_003E8__1.repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CCreateSnapshot_003Ed__62>(ref awaiter, ref this);
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
				goto IL_012e;
				IL_012e:
				try
				{
					ConfiguredTaskAwaiter<AircraftSnapshotInfo> awaiter2;
					if (num == 1)
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AircraftSnapshotInfo>);
						num = (_003C_003E1__state = -1);
						goto IL_01cb;
					}
					if (num != 2)
					{
						directoryBasedHangar.ThrowNotFoundIfAircraftDirectoryIsMissing(_003C_003E8__1.repoInfo);
						if (directoryBasedHangar.NeedsAMigration(_003C_003E8__1.repoInfo.RootPath))
						{
							awaiter2 = AttemptToMigrateToSaveTimeOnDecompress().ConfigureAwait(false).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotInfo>, _003CCreateSnapshot_003Ed__62>(ref awaiter2, ref this);
								return;
							}
							goto IL_01cb;
						}
						awaiter2 = directoryBasedHangar.TakeSnapShotWithLock("CREATE SNAPSHOT", _003C_003E8__1.aircraftIdentifier, _003C_003E8__1.repoInfo, _003C_003E8__1.metadata).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotInfo>, _003CCreateSnapshot_003Ed__62>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AircraftSnapshotInfo>);
						num = (_003C_003E1__state = -1);
					}
					AircraftSnapshotInfo result2 = awaiter2.GetResult();
					_003C_003E8__1.info = result2;
					goto end_IL_012e;
					IL_01cb:
					result2 = awaiter2.GetResult();
					_003C_003E8__1.info = result2;
					end_IL_012e:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap1 != null)
					{
						_003C_003E7__wrap1.Dispose();
					}
				}
				_003C_003E7__wrap1 = null;
				directoryBasedHangar._hangarEvents.PostSnapshotChange(DataChangeNotification.Create<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>(new ValueTuple<AircraftIdentifier, SnapshotIdentifier>(_003C_003E8__1.aircraftIdentifier, _003C_003E8__1.info.SnapshotIdentifier), DataChangeType.Created));
				info = _003C_003E8__1.info;
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
			_003C_003Et__builder.SetResult(info);
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass62_0._003C_003CCreateSnapshot_003Eg__AttemptToMigrateToSaveTimeOnDecompress_007C0_003Ed))]
			async System.Threading.Tasks.Task<AircraftSnapshotInfo> AttemptToMigrateToSaveTimeOnDecompress()
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				((_003C_003Ec__DisplayClass62_0)(object)this).info = await ((_003C_003Ec__DisplayClass62_0)(object)this)._003C_003E4__this.TakeSnapShotWithLock("CREATE SNAPSHOT", ((_003C_003Ec__DisplayClass62_0)(object)this).aircraftIdentifier, ((_003C_003Ec__DisplayClass62_0)(object)this).repoInfo, ((_003C_003Ec__DisplayClass62_0)(object)this).metadata).DetachContext();
				try
				{
					await ((_003C_003Ec__DisplayClass62_0)(object)this)._003C_003E4__this.PerformAircraftFormatMigrationWithLock(((_003C_003Ec__DisplayClass62_0)(object)this).repoInfo).DetachContext();
				}
				catch (AircraftRollbackException)
				{
					return ((_003C_003Ec__DisplayClass62_0)(object)this).info;
				}
				string path = Path.Combine(((_003C_003Ec__DisplayClass62_0)(object)this).repoInfo.HistoryAircraftRoot, ((_003C_003Ec__DisplayClass62_0)(object)this).info.SnapshotIdentifier.Serialized);
				((_003C_003Ec__DisplayClass62_0)(object)this)._003C_003E4__this._fs.DeleteIfExists(path);
				return await ((_003C_003Ec__DisplayClass62_0)(object)this)._003C_003E4__this.TakeSnapShotWithLock("CREATE SNAPSHOT", ((_003C_003Ec__DisplayClass62_0)(object)this).aircraftIdentifier, ((_003C_003Ec__DisplayClass62_0)(object)this).repoInfo, ((_003C_003Ec__DisplayClass62_0)(object)this).metadata).DetachContext();
			}
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CDeleteAircraft_003Ed__55 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		private _003C_003Ec__DisplayClass55_0 _003C_003E8__1;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private string _003CaircraftDirectoryPath_003E5__3;

		private AsyncReaderWriterLock _003CpathLock_003E5__4;

		private SimPanel _003CexistingPanel_003E5__5;

		private ConfiguredTaskAwaiter<AircraftIdentifier> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private System.IDisposable _003C_003E7__wrap5;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__3;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_02c2: Expected O, but got Unknown
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<AircraftIdentifier> awaiter3;
				ConfiguredTaskAwaiter awaiter2;
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				AircraftIdentifier result;
				System.IDisposable result3;
				switch (num)
				{
				default:
					_003C_003E8__1 = new _003C_003Ec__DisplayClass55_0();
					_003C_003E8__1.aircraftIdentifier = aircraftIdentifier;
					ThrowIfAircraftIdentifierIsInvalid(_003C_003E8__1.aircraftIdentifier);
					_003CrepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003C_003E8__1.aircraftIdentifier);
					_003CaircraftDirectoryPath_003E5__3 = _003CrepoInfo_003E5__2.RootPath;
					awaiter3 = directoryBasedHangar.GetLastSelectedAircraftOrDefault().DetachContext().GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter3;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftIdentifier>, _003CDeleteAircraft_003Ed__55>(ref awaiter3, ref this);
						return;
					}
					goto IL_00de;
				case 0:
					awaiter3 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftIdentifier>);
					num = (_003C_003E1__state = -1);
					goto IL_00de;
				case 1:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_018a;
				case 2:
					awaiter = _003C_003Eu__3;
					_003C_003Eu__3 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_0219;
				case 3:
					try
					{
						_ = 3;
						try
						{
							ConfiguredTaskAwaiter<SimPanel> awaiter4;
							if (num != 3)
							{
								awaiter4 = directoryBasedHangar.GetPersistedPanelWithLock(_003C_003E8__1.aircraftIdentifier, _003CrepoInfo_003E5__2).DetachContext().GetAwaiter();
								if (!awaiter4.IsCompleted)
								{
									num = (_003C_003E1__state = 3);
									_003C_003Eu__4 = awaiter4;
									((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CDeleteAircraft_003Ed__55>(ref awaiter4, ref this);
									return;
								}
							}
							else
							{
								awaiter4 = _003C_003Eu__4;
								_003C_003Eu__4 = default(ConfiguredTaskAwaiter<SimPanel>);
								num = (_003C_003E1__state = -1);
							}
							SimPanel result2 = awaiter4.GetResult();
							_003CexistingPanel_003E5__5 = result2;
						}
						catch (FormatException val2)
						{
							LogRepoError((System.Exception)val2, "Delete aircraft");
						}
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap5 != null)
						{
							_003C_003E7__wrap5.Dispose();
						}
					}
					_003C_003E7__wrap5 = null;
					if (_003CexistingPanel_003E5__5 != null)
					{
						directoryBasedHangar.PostSimulatorChanges(Enumerable.Select<SimInstallKey, SimAircraftKey>(_003CexistingPanel_003E5__5.AllSimulators.Keys, (Func<SimInstallKey, SimAircraftKey>)((SimInstallKey simKey) => new SimAircraftKey(_003C_003E8__1.aircraftIdentifier, simKey))), DataChangeType.Deleted);
					}
					awaiter = _003CpathLock_003E5__4.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 4);
						_003C_003Eu__3 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CDeleteAircraft_003Ed__55>(ref awaiter, ref this);
						return;
					}
					break;
				case 4:
					{
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_0191:
					_003CpathLock_003E5__4 = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath);
					_003CexistingPanel_003E5__5 = null;
					awaiter = _003CpathLock_003E5__4.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CDeleteAircraft_003Ed__55>(ref awaiter, ref this);
						return;
					}
					goto IL_0219;
					IL_00de:
					result = awaiter3.GetResult();
					if (_003C_003E8__1.aircraftIdentifier == result)
					{
						LogRepoMessage(LogLevel.Info, "Delete aircraft", "The selected aircraft({0}) is being deleted. Defaulting the selected id.", _003C_003E8__1.aircraftIdentifier);
						ConfiguredTaskAwaitable val = directoryBasedHangar.StoreLastSelectedAircraft(AircraftIdentifier.Default).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDeleteAircraft_003Ed__55>(ref awaiter2, ref this);
							return;
						}
						goto IL_018a;
					}
					goto IL_0191;
					IL_0219:
					result3 = awaiter.GetResult();
					_003C_003E7__wrap5 = result3;
					goto case 3;
					IL_018a:
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					goto IL_0191;
				}
				result3 = awaiter.GetResult();
				try
				{
					LogRepoMessage(LogLevel.Info, "Delete aircraft", "{0} - Deleting aircraft", _003C_003E8__1.aircraftIdentifier);
					directoryBasedHangar._fs.DeleteIfExists(_003CaircraftDirectoryPath_003E5__3);
					LogRepoMessage(LogLevel.Info, "Delete aircraft", "{0} - Deleted aircraft", _003C_003E8__1.aircraftIdentifier);
				}
				finally
				{
					if (num < 0)
					{
						result3?.Dispose();
					}
				}
				directoryBasedHangar.PostAircraftChange(_003C_003E8__1.aircraftIdentifier, DataChangeType.Deleted);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CrepoInfo_003E5__2 = null;
				_003CaircraftDirectoryPath_003E5__3 = null;
				_003CpathLock_003E5__4 = null;
				_003CexistingPanel_003E5__5 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CrepoInfo_003E5__2 = null;
			_003CaircraftDirectoryPath_003E5__3 = null;
			_003CpathLock_003E5__4 = null;
			_003CexistingPanel_003E5__5 = null;
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
	private struct _003CDeleteAircraftWinfsArchives_003Ed__58 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public System.Collections.Generic.IEnumerable<SimInstallKey> simsToClear;

		public DirectoryBasedHangar _003C_003E4__this;

		private _003C_003Ec__DisplayClass58_0 _003C_003E8__1;

		private List<SimInstallKey> _003CtoDelete_003E5__2;

		private List<SimInstallKey> _003Cmodified_003E5__3;

		private AircraftRepoInfo _003CrepoInfo_003E5__4;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_0110;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass58_0();
				_003C_003E8__1.aircraftIdentifier = aircraftIdentifier;
				_003C_003E8__1.aircraftIdentifier.ThrowIfDefault();
				if (simsToClear == null)
				{
					throw new ArgumentNullException("simsToClear");
				}
				_003CtoDelete_003E5__2 = Enumerable.ToList<SimInstallKey>(simsToClear);
				if (_003CtoDelete_003E5__2.Count > 0)
				{
					_003Cmodified_003E5__3 = new List<SimInstallKey>();
					_003CrepoInfo_003E5__4 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003C_003E8__1.aircraftIdentifier);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__4.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CDeleteAircraftWinfsArchives_003Ed__58>(ref awaiter, ref this);
						return;
					}
					goto IL_0110;
				}
				goto end_IL_000e;
				IL_0110:
				System.IDisposable result = awaiter.GetResult();
				try
				{
					LogRepoMessage(LogLevel.Info, "DELETE_WINFS_ARCHIVE", _003C_003E8__1.aircraftIdentifier.IdentStringChecked);
					string currentAircraftRoot = _003CrepoInfo_003E5__4.CurrentAircraftRoot;
					Enumerator<SimInstallKey> enumerator = _003CtoDelete_003E5__2.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							SimInstallKey current = enumerator.Current;
							string text = Path.Combine(currentAircraftRoot, "avionics", (string)current.ToStandardRelativePath(), "winfs_mount.gca");
							LogRepoMessage(LogLevel.Info, "DELETE_WINFS_ARCHIVE", text);
							if (directoryBasedHangar._fs.DeleteIfExists(text))
							{
								_003Cmodified_003E5__3.Add(current);
							}
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
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
					}
				}
				directoryBasedHangar.PostSimulatorChanges(Enumerable.Select<SimInstallKey, SimAircraftKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)_003Cmodified_003E5__3, (Func<SimInstallKey, SimAircraftKey>)((SimInstallKey sim) => new SimAircraftKey(_003C_003E8__1.aircraftIdentifier, sim))), DataChangeType.Updated);
				if (_003Cmodified_003E5__3.Count > 0)
				{
					directoryBasedHangar.PostAircraftChange(_003C_003E8__1.aircraftIdentifier, DataChangeType.Updated);
				}
				_003Cmodified_003E5__3 = null;
				_003CrepoInfo_003E5__4 = null;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CtoDelete_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CtoDelete_003E5__2 = null;
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
	private struct _003CDeleteFamilyDataForRelease_003Ed__79 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftPackageFamily aircraftPackageFamily;

		public DirectoryBasedHangar _003C_003E4__this;

		private _003C_003Ec__DisplayClass79_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<bool> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass79_0();
					_003C_003E8__1.aircraftPackageFamily = aircraftPackageFamily;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					awaiter = directoryBasedHangar.WriteAircraftHarness<bool>(_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier, delegate(AircraftRepoInfo repoInfo)
					{
						string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
						string familyDataFilePath = GetFamilyDataFilePath(familyDataDirPath, _003C_003E8__1.aircraftPackageFamily.ReleaseId);
						_003C_003E8__1._003C_003E4__this._fs.CreateDirectory(familyDataDirPath);
						bool flag = _003C_003E8__1._003C_003E4__this._fs.DeleteIfExists(familyDataFilePath);
						LogRepoMessage(LogLevel.Info, "DELETE_FAMILY_DATA", $"[{_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier} - {_003C_003E8__1.aircraftPackageFamily.ReleaseId}] Family data {(flag ? "was deleted" : "did not exist to delete")}.");
						return System.Threading.Tasks.Task.FromResult<bool>(flag);
					}).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CDeleteFamilyDataForRelease_003Ed__79>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<bool>);
					num = (_003C_003E1__state = -1);
				}
				if (awaiter.GetResult())
				{
					directoryBasedHangar._hangarEvents.PostFamilyDataChange(DataChangeNotification.Create(_003C_003E8__1.aircraftPackageFamily, DataChangeType.Deleted));
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
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
	private struct _003CDeleteSnapshot_003Ed__65 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SnapshotIdentifier snapshotIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private _003C_003Ec__DisplayClass65_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<int> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<int> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass65_0();
					_003C_003E8__1.snapshotIdentifier = snapshotIdentifier;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					awaiter = directoryBasedHangar.WriteAircraftHarnessWithRollbackHandling<int>(aircraftIdentifier, delegate(AircraftRepoInfo repoInfo, AircraftRollbackException? e)
					{
						string path = Path.Combine(repoInfo.HistoryAircraftRoot, _003C_003E8__1.snapshotIdentifier.Serialized);
						if (!_003C_003E8__1._003C_003E4__this._fs.PathExists(path))
						{
							throw new AircraftSnapshotNotFoundException("The snapshot does not exist.");
						}
						_003C_003E8__1._003C_003E4__this._fs.Delete(path);
						return System.Threading.Tasks.Task.FromResult<int>(0);
					}).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<int>, _003CDeleteSnapshot_003Ed__65>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<int>);
					num = (_003C_003E1__state = -1);
				}
				awaiter.GetResult();
				directoryBasedHangar._hangarEvents.PostSnapshotChange(DataChangeNotification.Create<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>(new ValueTuple<AircraftIdentifier, SnapshotIdentifier>(aircraftIdentifier, _003C_003E8__1.snapshotIdentifier), DataChangeType.Deleted));
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
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
	private struct _003CDeserializeAircraft_003Ed__140 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<PersistedAircraft> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public string aircraftJsonPath;

		private ConfiguredTaskAwaiter<PersistedAircraft> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			PersistedAircraft result;
			try
			{
				ConfiguredTaskAwaiter<PersistedAircraft> awaiter;
				if (num != 0)
				{
					awaiter = directoryBasedHangar._persistence.Load<PersistedAircraft>(aircraftJsonPath).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedAircraft>, _003CDeserializeAircraft_003Ed__140>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedAircraft>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CExportAircraft_003Ed__59 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public string gcaPath;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private string _003CexportPath_003E5__3;

		private string _003CtemporaryZipPath_003E5__4;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				if ((uint)num > 1u)
				{
					_003CrepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
					string text = directoryBasedHangar._deployment.PerProcessTempDirectory();
					Guid val = Guid.NewGuid();
					string text2 = Path.Combine(text, "ExportAircraft", ((object)(Guid)(ref val)).ToString());
					_003CexportPath_003E5__3 = Path.Combine(text2, "aircraft", aircraftIdentifier.IdentStringChecked);
					_003CtemporaryZipPath_003E5__4 = Path.Combine(text2, "aircraft.tmp.gca");
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					ConfiguredTaskAwaiter<System.IDisposable> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_01e2;
						}
						awaiter2 = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false)
							.GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CExportAircraft_003Ed__59>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
					}
					System.IDisposable result = awaiter2.GetResult();
					try
					{
						directoryBasedHangar.ThrowNotFoundIfAircraftDirectoryIsMissing(_003CrepoInfo_003E5__2);
						directoryBasedHangar._fs.DeleteIfExists(_003CexportPath_003E5__3);
						directoryBasedHangar._fs.CreateDirectory(_003CexportPath_003E5__3);
						directoryBasedHangar._fs.CopyDirectory(_003CrepoInfo_003E5__2.RootPath, _003CexportPath_003E5__3, recursive: true, overwriteExistingFiles: true);
					}
					finally
					{
						if (num < 0)
						{
							result?.Dispose();
						}
					}
					AircraftRepoInfo repoInfo = CreateRepoInfo(_003CexportPath_003E5__3);
					ConfiguredTaskAwaitable val2 = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(repoInfo).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CExportAircraft_003Ed__59>(ref awaiter, ref this);
						return;
					}
					goto IL_01e2;
					IL_01e2:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					string parentDirectoryPath = GetParentDirectoryPath(_003CexportPath_003E5__3);
					directoryBasedHangar._zipCompression.ZipDirectory(parentDirectoryPath, _003CtemporaryZipPath_003E5__4);
					directoryBasedHangar._fs.CreateDirectory(GetParentDirectoryPath(gcaPath));
					directoryBasedHangar._fs.MoveOrCopyAndDeleteToDifferentDriveAndOverwrite(LolaFsExtensions.DefaultTempFactory, _003CtemporaryZipPath_003E5__4, gcaPath);
				}
				finally
				{
					if (num < 0)
					{
						directoryBasedHangar._fs.DeleteIfExists(_003CtemporaryZipPath_003E5__4);
						directoryBasedHangar._fs.DeleteIfExistsAndParentsThatAreEmpty(_003CexportPath_003E5__3, directoryBasedHangar._deployment.PerProcessTempDirectory());
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CrepoInfo_003E5__2 = null;
				_003CexportPath_003E5__3 = null;
				_003CtemporaryZipPath_003E5__4 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrepoInfo_003E5__2 = null;
			_003CexportPath_003E5__3 = null;
			_003CtemporaryZipPath_003E5__4 = null;
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
	private struct _003CExtractSnapshotIntoTemporaryHangar_003Ed__71 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftRepoInfo> _003C_003Et__builder;

		public TemporaryHangarDirectory tempHangar;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public string snapshotArchivePath;

		public string repoAction;

		private string _003CmountPath_003E5__2;

		private string _003CaircraftDirPath_003E5__3;

		private AircraftRepoInfo _003CtempRepoInfo_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftRepoInfo result2;
			try
			{
				ConfiguredTaskAwaitable val;
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaiter<AircraftListData> awaiter2;
				AircraftListData result;
				switch (num)
				{
				default:
				{
					_003CmountPath_003E5__2 = tempHangar.RootDirectory;
					directoryBasedHangar._fs.CreateDirectory(_003CmountPath_003E5__2);
					_003CaircraftDirPath_003E5__3 = Path.Combine(_003CmountPath_003E5__2, aircraftIdentifier.IdentStringChecked);
					string text = Path.Combine(_003CaircraftDirPath_003E5__3, "current");
					string text2 = (IsProtoTypeArchiveFileName(snapshotArchivePath) ? _003CaircraftDirPath_003E5__3 : text);
					directoryBasedHangar._fs.CreateDirectory(text2);
					Logger.Log(LogLevel.Info, "Mounting {0} snapshot {1} at {2}", aircraftIdentifier, snapshotArchivePath, text2);
					directoryBasedHangar._zipCompression.UnZip(snapshotArchivePath, text2);
					_003CtempRepoInfo_003E5__4 = CreateRepoInfo(_003CaircraftDirPath_003E5__3);
					val = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(_003CtempRepoInfo_003E5__4).ConfigureAwait(false);
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CExtractSnapshotIntoTemporaryHangar_003Ed__71>(ref awaiter, ref this);
						return;
					}
					goto IL_0158;
				}
				case 0:
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0158;
				case 1:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AircraftListData>);
					num = (_003C_003E1__state = -1);
					goto IL_01c5;
				case 2:
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_02d2;
					}
					IL_0158:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					awaiter2 = directoryBasedHangar.GetListDataWithLock(_003CtempRepoInfo_003E5__4).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003CExtractSnapshotIntoTemporaryHangar_003Ed__71>(ref awaiter2, ref this);
						return;
					}
					goto IL_01c5;
					IL_02d2:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					break;
					IL_01c5:
					result = awaiter2.GetResult();
					if (!(result.AircraftIdentifier != aircraftIdentifier))
					{
						break;
					}
					if (IsProtoTypeArchiveFileName(snapshotArchivePath))
					{
						Logger.Log(LogLevel.Info, "The Snapshot is from a prototype aircraft. So we have to redo the directory to match the old identifier.");
						string text3 = Path.Combine(_003CmountPath_003E5__2, result.AircraftIdentifier.IdentStringChecked);
						directoryBasedHangar._fs.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, _003CaircraftDirPath_003E5__3, text3);
						_003CtempRepoInfo_003E5__4 = CreateRepoInfo(text3);
						break;
					}
					Logger.Log(LogLevel.Info, "The Snapshot is from a temporary aircraft. So we have to redo the list data to match the current identifier.");
					result = result.WithNewIdentifier(aircraftIdentifier);
					val = directoryBasedHangar.UpdateListDataWithLock(repoAction, aircraftIdentifier, _003CtempRepoInfo_003E5__4, result).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CExtractSnapshotIntoTemporaryHangar_003Ed__71>(ref awaiter, ref this);
						return;
					}
					goto IL_02d2;
				}
				result2 = _003CtempRepoInfo_003E5__4;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CmountPath_003E5__2 = null;
				_003CaircraftDirPath_003E5__3 = null;
				_003CtempRepoInfo_003E5__4 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CmountPath_003E5__2 = null;
			_003CaircraftDirPath_003E5__3 = null;
			_003CtempRepoInfo_003E5__4 = null;
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
	private struct _003CGetAircraftHeavyDataById_003Ed__37 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftHeavyData> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private ConfiguredTaskAwaiter<AircraftHeavyData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftHeavyData result;
			try
			{
				ConfiguredTaskAwaiter<AircraftHeavyData> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass37_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass37_0
					{
						_003C_003E4__this = _003C_003E4__this,
						aircraftIdentifier = aircraftIdentifier
					};
					awaiter = directoryBasedHangar.ReadAircraftHarness<AircraftHeavyData>(CS_0024_003C_003E8__locals0.aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass37_0._003C_003CGetAircraftHeavyDataById_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) => await CS_0024_003C_003E8__locals0._003C_003E4__this.GetHeavyDataWithLock(CS_0024_003C_003E8__locals0.aircraftIdentifier, repoInfo).DetachContext()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftHeavyData>, _003CGetAircraftHeavyDataById_003Ed__37>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftHeavyData>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CGetAircraftListDataById_003Ed__36 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftListData> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			AircraftListData result;
			try
			{
				ConfiguredTaskAwaiter<AircraftListData> awaiter;
				if (num != 0)
				{
					awaiter = CS_0024_003C_003E8__locals0.ReadAircraftHarnessWithRollbackHandling<AircraftListData>(aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003CGetAircraftListDataById_003Eb__36_0_003Ed))] [CompilerGenerated] async (AircraftRepoInfo repoInfo, AircraftRollbackException? re) => await CS_0024_003C_003E8__locals0.GetListDataWithLock(repoInfo).DetachContext()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003CGetAircraftListDataById_003Ed__36>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CGetAircraftSnapshotById_003Ed__38 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotData result;
			try
			{
				ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass38_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass38_0
					{
						_003C_003E4__this = _003C_003E4__this,
						aircraftIdentifier = aircraftIdentifier
					};
					awaiter = directoryBasedHangar.ReadAircraftHarness<AircraftSnapshotData>(CS_0024_003C_003E8__locals0.aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass38_0._003C_003CGetAircraftSnapshotById_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) => await CS_0024_003C_003E8__locals0._003C_003E4__this.GetAircraftSnapshotWithLock(CS_0024_003C_003E8__locals0.aircraftIdentifier, repoInfo).DetachContext()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CGetAircraftSnapshotById_003Ed__38>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CGetAircraftSnapshotWithLock_003Ed__98 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

		public AircraftIdentifier aircraftIdent;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftRepoInfo repoInfo;

		private System.Threading.Tasks.Task<AircraftHeavyData> _003CheavyTask_003E5__2;

		private System.Threading.Tasks.Task<SimPanel> _003CpanelTask_003E5__3;

		private System.Threading.Tasks.Task<FamilyDataLookup> _003CfamilyTask_003E5__4;

		private AircraftSnapshotData _003C_003E7__wrap4;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<AircraftHeavyData> _003C_003Eu__3;

		private ConfiguredTaskAwaiter<FamilyDataLookup> _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotData result5;
			try
			{
				ConfiguredTaskAwaiter<AircraftListData> awaiter4;
				ConfiguredTaskAwaiter<SimPanel> awaiter3;
				ConfiguredTaskAwaiter<AircraftHeavyData> awaiter2;
				ConfiguredTaskAwaiter<FamilyDataLookup> awaiter;
				SimPanel result;
				AircraftListData result2;
				AircraftHeavyData result3;
				AircraftSnapshotData aircraftSnapshotData;
				switch (num)
				{
				default:
				{
					aircraftSnapshotData = new AircraftSnapshotData(aircraftIdent);
					System.Threading.Tasks.Task<AircraftListData> listDataWithLock = directoryBasedHangar.GetListDataWithLock(repoInfo);
					_003CheavyTask_003E5__2 = directoryBasedHangar.GetHeavyDataWithLock(aircraftIdent, repoInfo);
					_003CpanelTask_003E5__3 = directoryBasedHangar.GetPersistedPanelWithLock(aircraftIdent, repoInfo);
					_003CfamilyTask_003E5__4 = directoryBasedHangar.GetAllAircraftFamilyDataWithLock(repoInfo);
					_003C_003E7__wrap4 = aircraftSnapshotData;
					awaiter4 = listDataWithLock.DetachContext().GetAwaiter();
					if (!awaiter4.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter4;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003CGetAircraftSnapshotWithLock_003Ed__98>(ref awaiter4, ref this);
						return;
					}
					goto IL_00df;
				}
				case 0:
					awaiter4 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftListData>);
					num = (_003C_003E1__state = -1);
					goto IL_00df;
				case 1:
					awaiter3 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
					goto IL_0164;
				case 2:
					awaiter2 = _003C_003Eu__3;
					_003C_003Eu__3 = default(ConfiguredTaskAwaiter<AircraftHeavyData>);
					num = (_003C_003E1__state = -1);
					goto IL_01e9;
				case 3:
					{
						awaiter = _003C_003Eu__4;
						_003C_003Eu__4 = default(ConfiguredTaskAwaiter<FamilyDataLookup>);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_0164:
					result = awaiter3.GetResult();
					aircraftSnapshotData = _003C_003E7__wrap4.ReplacePanel(result);
					_003C_003E7__wrap4 = null;
					_003C_003E7__wrap4 = aircraftSnapshotData;
					awaiter2 = _003CheavyTask_003E5__2.DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftHeavyData>, _003CGetAircraftSnapshotWithLock_003Ed__98>(ref awaiter2, ref this);
						return;
					}
					goto IL_01e9;
					IL_00df:
					result2 = awaiter4.GetResult();
					aircraftSnapshotData = _003C_003E7__wrap4.Relist(result2);
					_003C_003E7__wrap4 = null;
					_003C_003E7__wrap4 = aircraftSnapshotData;
					awaiter3 = _003CpanelTask_003E5__3.DetachContext().GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter3;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetAircraftSnapshotWithLock_003Ed__98>(ref awaiter3, ref this);
						return;
					}
					goto IL_0164;
					IL_01e9:
					result3 = awaiter2.GetResult();
					aircraftSnapshotData = _003C_003E7__wrap4.ReplaceHeavyData(result3);
					_003C_003E7__wrap4 = null;
					_003C_003E7__wrap4 = aircraftSnapshotData;
					awaiter = _003CfamilyTask_003E5__4.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 3);
						_003C_003Eu__4 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<FamilyDataLookup>, _003CGetAircraftSnapshotWithLock_003Ed__98>(ref awaiter, ref this);
						return;
					}
					break;
				}
				FamilyDataLookup result4 = awaiter.GetResult();
				aircraftSnapshotData = _003C_003E7__wrap4.ReplaceFamilyLookup(result4);
				_003C_003E7__wrap4 = null;
				result5 = aircraftSnapshotData;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CheavyTask_003E5__2 = null;
				_003CpanelTask_003E5__3 = null;
				_003CfamilyTask_003E5__4 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CheavyTask_003E5__2 = null;
			_003CpanelTask_003E5__3 = null;
			_003CfamilyTask_003E5__4 = null;
			_003C_003Et__builder.SetResult(result5);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetAircraftWinfsArchives_003Ed__56 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Collections.Generic.IList<SimInstallKey>> _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public Dictionary<SimInstallKey, string> simWinfsArchiveOutputMap;

		public DirectoryBasedHangar _003C_003E4__this;

		private _003C_003Ec__DisplayClass56_0 _003C_003E8__1;

		private List<SimInstallKey> _003Cfound_003E5__2;

		private AircraftRepoInfo _003CrepoInfo_003E5__3;

		private System.IDisposable _003C_003E7__wrap3;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			System.Collections.Generic.IList<SimInstallKey> result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00f1;
					}
					ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
					if (simWinfsArchiveOutputMap == null)
					{
						throw new ArgumentNullException("simWinfsArchiveOutputMap");
					}
					_003Cfound_003E5__2 = new List<SimInstallKey>();
					if (simWinfsArchiveOutputMap.Count <= 0)
					{
						goto IL_02e7;
					}
					_003CrepoInfo_003E5__3 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__3.RootPath).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetAircraftWinfsArchives_003Ed__56>(ref awaiter, ref this);
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
				_003C_003E7__wrap3 = result;
				goto IL_00f1;
				IL_02e7:
				result2 = (System.Collections.Generic.IList<SimInstallKey>)_003Cfound_003E5__2;
				goto end_IL_000e;
				IL_00f1:
				try
				{
					ConfiguredTaskAwaiter<SimPanel> awaiter2;
					if (num == 1)
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPanel>);
						num = (_003C_003E1__state = -1);
						goto IL_01a9;
					}
					LogRepoMessage(LogLevel.Info, "GET_WINFS_ARCHIVE", aircraftIdentifier.IdentStringChecked);
					string rootPath = _003CrepoInfo_003E5__3.RootPath;
					if (directoryBasedHangar._fs.PathExists(rootPath))
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass56_0();
						awaiter2 = directoryBasedHangar.GetPersistedPanelWithLock(aircraftIdentifier, _003CrepoInfo_003E5__3).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetAircraftWinfsArchives_003Ed__56>(ref awaiter2, ref this);
							return;
						}
						goto IL_01a9;
					}
					goto end_IL_00f1;
					IL_01a9:
					SimPanel result3 = awaiter2.GetResult();
					_003C_003E8__1.panel = result3;
					string currentAircraftRoot = _003CrepoInfo_003E5__3.CurrentAircraftRoot;
					System.Collections.Generic.IEnumerator<KeyValuePair<SimInstallKey, string>> enumerator = Enumerable.Where<KeyValuePair<SimInstallKey, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, string>>)simWinfsArchiveOutputMap, (Func<KeyValuePair<SimInstallKey, string>, bool>)((KeyValuePair<SimInstallKey, string> kv) => _003C_003E8__1.panel.AllSimulators.ContainsKey(kv.Key))).GetEnumerator();
					try
					{
						while (((System.Collections.IEnumerator)enumerator).MoveNext())
						{
							KeyValuePair<SimInstallKey, string> current = enumerator.Current;
							string text = Path.Combine(currentAircraftRoot, "avionics", (string)current.Key.ToStandardRelativePath(), "winfs_mount.gca");
							if (directoryBasedHangar._fs.PathExists(text))
							{
								LogRepoMessage(LogLevel.Info, "GET_WINFS_ARCHIVE", "Copying '{0}' -> '{1}'", text, current.Value);
								directoryBasedHangar._fs.CopyAndOverwriteFile(text, current.Value);
								_003Cfound_003E5__2.Add(current.Key);
							}
						}
					}
					finally
					{
						if (num < 0)
						{
							((System.IDisposable)enumerator)?.Dispose();
						}
					}
					_003C_003E8__1 = null;
					end_IL_00f1:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap3 != null)
					{
						_003C_003E7__wrap3.Dispose();
					}
				}
				_003C_003E7__wrap3 = null;
				_003CrepoInfo_003E5__3 = null;
				goto IL_02e7;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cfound_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cfound_003E5__2 = null;
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
	private struct _003CGetAllAircraftFamilyDataWithLock_003Ed__83 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<FamilyDataLookup> _003C_003Et__builder;

		public AircraftRepoInfo repoInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		private ConfiguredTaskAwaiter<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>[]> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			FamilyDataLookup result;
			try
			{
				ConfiguredTaskAwaiter<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>[]> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>[]>);
					num = (_003C_003E1__state = -1);
					goto IL_00e0;
				}
				string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
				if (CS_0024_003C_003E8__locals0._fs.PathExists(familyDataDirPath))
				{
					awaiter = System.Threading.Tasks.Task.WhenAll<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>>)Enumerable.ToList<System.Threading.Tasks.Task<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>>(Enumerable.Select<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>>(Enumerable.Where<FileSystemEntry>((System.Collections.Generic.IEnumerable<FileSystemEntry>)Enumerable.ToList<FileSystemEntry>(CS_0024_003C_003E8__locals0._fs.FilesIn(familyDataDirPath)), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => TrainerEmbeddedReleaseId.CanParse(Path.GetFileNameWithoutExtension(fse.Name)))), (Func<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>>)([AsyncStateMachine(typeof(_003C_003CGetAllAircraftFamilyDataWithLock_003Eb__83_1_003Ed))] [CompilerGenerated] async (FileSystemEntry releaseIdFileEntry) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						PersistedFamilyData persistedFamilyData = await CS_0024_003C_003E8__locals0._persistence.Load<PersistedFamilyData>(releaseIdFileEntry.FullPath).DetachContext();
						return new ValueTuple<TrainerEmbeddedReleaseId, FamilyData>(new TrainerEmbeddedReleaseId(Path.GetFileNameWithoutExtension(releaseIdFileEntry.Name)), new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)persistedFamilyData.Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value))));
					})))).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>[]>, _003CGetAllAircraftFamilyDataWithLock_003Ed__83>(ref awaiter, ref this);
						return;
					}
					goto IL_00e0;
				}
				result = new FamilyDataLookup();
				goto end_IL_000e;
				IL_00e0:
				result = new FamilyDataLookup(ImmutableDictionary.ToImmutableDictionary<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>, TrainerEmbeddedReleaseId, FamilyData>((System.Collections.Generic.IEnumerable<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>)awaiter.GetResult(), (Func<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>, TrainerEmbeddedReleaseId>)((ValueTuple<TrainerEmbeddedReleaseId, FamilyData> t) => t.Item1), (Func<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>, FamilyData>)((ValueTuple<TrainerEmbeddedReleaseId, FamilyData> t) => t.Item2)));
				end_IL_000e:;
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
	private struct _003CGetAllFamilyDataForAircraft_003Ed__77 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<FamilyDataLookup> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraft;

		private ConfiguredTaskAwaiter<FamilyDataLookup> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			FamilyDataLookup result;
			try
			{
				ConfiguredTaskAwaiter<FamilyDataLookup> awaiter;
				if (num != 0)
				{
					awaiter = CS_0024_003C_003E8__locals0.ReadAircraftHarness<FamilyDataLookup>(aircraft, [AsyncStateMachine(typeof(_003C_003CGetAllFamilyDataForAircraft_003Eb__77_0_003Ed))] [CompilerGenerated] async (AircraftRepoInfo repoInfo) => await CS_0024_003C_003E8__locals0.GetAllAircraftFamilyDataWithLock(repoInfo).DetachContext()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<FamilyDataLookup>, _003CGetAllFamilyDataForAircraft_003Ed__77>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<FamilyDataLookup>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CGetFamilyDataForReleaseOrDefault_003Ed__76 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<FamilyData> _003C_003Et__builder;

		public AircraftPackageFamily aircraftPackageFamily;

		public DirectoryBasedHangar _003C_003E4__this;

		private ConfiguredTaskAwaiter<FamilyData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			FamilyData result;
			try
			{
				ConfiguredTaskAwaiter<FamilyData> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass76_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass76_0
					{
						aircraftPackageFamily = aircraftPackageFamily,
						_003C_003E4__this = _003C_003E4__this
					};
					awaiter = directoryBasedHangar.ReadAircraftHarness<FamilyData>(CS_0024_003C_003E8__locals0.aircraftPackageFamily.AircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass76_0._003C_003CGetFamilyDataForReleaseOrDefault_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
						string packageFamilyDataFilePath = GetFamilyDataFilePath(familyDataDirPath, CS_0024_003C_003E8__locals0.aircraftPackageFamily.ReleaseId);
						if (CS_0024_003C_003E8__locals0._003C_003E4__this._fs.PathExists(packageFamilyDataFilePath))
						{
							try
							{
								return new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)(await CS_0024_003C_003E8__locals0._003C_003E4__this._persistence.Load<PersistedFamilyData>(packageFamilyDataFilePath).DetachContext()).Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value)));
							}
							catch (SerializationException val)
							{
								LogRepoError((System.Exception)val, "GET_FAMILY_DATA", $"[{CS_0024_003C_003E8__locals0.aircraftPackageFamily.AircraftIdentifier} - {CS_0024_003C_003E8__locals0.aircraftPackageFamily.ReleaseId}] Family data could not be parsed from file({packageFamilyDataFilePath})");
								throw;
							}
						}
						LogRepoMessage(LogLevel.Info, "GET_FAMILY_DATA", $"[{CS_0024_003C_003E8__locals0.aircraftPackageFamily.AircraftIdentifier} - {CS_0024_003C_003E8__locals0.aircraftPackageFamily.ReleaseId}] No family data found returning default.");
						return new FamilyData();
					}).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<FamilyData>, _003CGetFamilyDataForReleaseOrDefault_003Ed__76>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<FamilyData>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CGetHeavyDataWithLock_003Ed__100 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftHeavyData> _003C_003Et__builder;

		public AircraftRepoInfo repoInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private ConfiguredTaskAwaiter<PersistedAircraftHeavyData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftHeavyData result;
			try
			{
				ConfiguredTaskAwaiter<PersistedAircraftHeavyData> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedAircraftHeavyData>);
					num = (_003C_003E1__state = -1);
					goto IL_0098;
				}
				string path = Path.Combine(repoInfo.CurrentAircraftRoot, "heavy.json");
				if (directoryBasedHangar._fs.PathExists(path))
				{
					awaiter = directoryBasedHangar._persistence.Load<PersistedAircraftHeavyData>(path).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedAircraftHeavyData>, _003CGetHeavyDataWithLock_003Ed__100>(ref awaiter, ref this);
						return;
					}
					goto IL_0098;
				}
				result = new AircraftHeavyData(aircraftIdentifier);
				goto end_IL_000e;
				IL_0098:
				result = awaiter.GetResult().ToRuntime(aircraftIdentifier);
				end_IL_000e:;
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
	private struct _003CGetLastSelectedAircraft_003Ed__32 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftIdentifier> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		private AircraftIdentifier _003CcachedSelectedId_003E5__2;

		private AircraftIdentifier _003Cresult_003E5__3;

		private ConfiguredTaskAwaiter<PersistedSelectedAircraft> _003C_003Eu__1;

		private AircraftRepoInfo _003CrepoInfo_003E5__4;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Expected O, but got Unknown
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftIdentifier result3;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				_003C_003Ec__DisplayClass32_0 CS_0024_003C_003E8__locals0;
				ConfiguredTaskAwaiter<PersistedSelectedAircraft> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.IDisposable>);
						num = (_003C_003E1__state = -1);
						goto IL_0183;
					}
					CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass32_0
					{
						_003C_003E4__this = _003C_003E4__this
					};
					_003CcachedSelectedId_003E5__2 = directoryBasedHangar._cachedSelectedAircraftId;
					CS_0024_003C_003E8__locals0.selectedAircraftFilePath = directoryBasedHangar.BuildSelectedAircraftFilepath();
					_003Cresult_003E5__3 = AircraftIdentifier.Default;
					if (!directoryBasedHangar._fs.PathExists(CS_0024_003C_003E8__locals0.selectedAircraftFilePath))
					{
						goto IL_01e2;
					}
					awaiter2 = GetCurrentSelectedAircraftFromFile().DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedSelectedAircraft>, _003CGetLastSelectedAircraft_003Ed__32>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedSelectedAircraft>);
					num = (_003C_003E1__state = -1);
				}
				PersistedSelectedAircraft result = awaiter2.GetResult();
				if (AircraftIdentifier.IsValidIdentifier(result.SelectedAircraft))
				{
					_003Cresult_003E5__3 = new AircraftIdentifier(result.SelectedAircraft);
					_003CrepoInfo_003E5__4 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003Cresult_003E5__3);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__4.RootPath).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetLastSelectedAircraft_003Ed__32>(ref awaiter, ref this);
						return;
					}
					goto IL_0183;
				}
				goto IL_01e2;
				IL_0183:
				System.IDisposable result2 = awaiter.GetResult();
				try
				{
					if (!directoryBasedHangar._fs.PathExists(_003CrepoInfo_003E5__4.RootPath))
					{
						LogRepoError((System.Exception)new DirectoryNotFoundException(_003CrepoInfo_003E5__4.RootPath), "GET SELECTED AIRCRAFT");
						_003Cresult_003E5__3 = AircraftIdentifier.Default;
					}
				}
				finally
				{
					if (num < 0)
					{
						result2?.Dispose();
					}
				}
				_003CrepoInfo_003E5__4 = null;
				goto IL_01e2;
				IL_01e2:
				if (_003Cresult_003E5__3 == AircraftIdentifier.Default)
				{
					LogRepoMessage(LogLevel.Info, "GET SELECTED AIRCRAFT", "Could not determine last selected aircraft.");
				}
				if (_003CcachedSelectedId_003E5__2 != _003Cresult_003E5__3)
				{
					directoryBasedHangar.PostSelectedAircaftChange(_003Cresult_003E5__3);
				}
				result3 = _003Cresult_003E5__3;
				[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_0._003C_003CGetLastSelectedAircraft_003Eg__GetCurrentSelectedAircraftFromFile_007C0_003Ed))]
				async System.Threading.Tasks.Task<PersistedSelectedAircraft> GetCurrentSelectedAircraftFromFile()
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					using (await CS_0024_003C_003E8__locals0._003C_003E4__this._selectedLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
					{
						return await CS_0024_003C_003E8__locals0._003C_003E4__this._persistence.Load<PersistedSelectedAircraft>(CS_0024_003C_003E8__locals0.selectedAircraftFilePath).DetachContext();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CcachedSelectedId_003E5__2 = default(AircraftIdentifier);
				_003Cresult_003E5__3 = default(AircraftIdentifier);
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CcachedSelectedId_003E5__2 = default(AircraftIdentifier);
			_003Cresult_003E5__3 = default(AircraftIdentifier);
			_003C_003Et__builder.SetResult(result3);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetListDataWithLock_003Ed__99 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftListData> _003C_003Et__builder;

		public AircraftRepoInfo repoInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		private ConfiguredTaskAwaiter<PersistedAircraftListData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftListData result;
			try
			{
				ConfiguredTaskAwaiter<PersistedAircraftListData> awaiter;
				if (num != 0)
				{
					string path = Path.Combine(repoInfo.CurrentAircraftRoot, "list.json");
					awaiter = directoryBasedHangar._persistence.Load<PersistedAircraftListData>(path).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedAircraftListData>, _003CGetListDataWithLock_003Ed__99>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedAircraftListData>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToListData();
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
	private struct _003CGetPersistedPanelWithLock_003Ed__101 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SimPanel> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdent;

		public AircraftRepoInfo repoInfo;

		private _003C_003Ec__DisplayClass101_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<PersistedSimPanelInfo> _003C_003Eu__1;

		private object _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0210: Expected O, but got Unknown
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			SimPanel result2;
			try
			{
				AsyncSubject<System.Collections.Generic.IList<SimInstallData>> asyncSubject;
				PersistedSimPanelInfo persistedSimPanelInfo;
				ConfiguredTaskAwaiter<PersistedSimPanelInfo> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						asyncSubject = (AsyncSubject<System.Collections.Generic.IList<SimInstallData>>)_003C_003Eu__2;
						_003C_003Eu__2 = null;
						num = (_003C_003E1__state = -1);
						goto IL_0192;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass101_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.aircraftIdent = aircraftIdent;
					_003C_003E8__1.avionicsDirPath = repoInfo.CurrentAvionicsRoot;
					string path = Path.Combine(repoInfo.CurrentAircraftRoot, "panel.json");
					if (!directoryBasedHangar._fs.PathExists(path))
					{
						persistedSimPanelInfo = new PersistedSimPanelInfo();
						goto IL_00fa;
					}
					awaiter = directoryBasedHangar._persistence.Load<PersistedSimPanelInfo>(path).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedSimPanelInfo>, _003CGetPersistedPanelWithLock_003Ed__101>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedSimPanelInfo>);
					num = (_003C_003E1__state = -1);
				}
				persistedSimPanelInfo = awaiter.GetResult();
				goto IL_00fa;
				IL_0192:
				System.Collections.Generic.IList<SimInstallData> result = asyncSubject.GetResult();
				_003C_003E8__1.allPersistedSimulators = result;
				System.Collections.Generic.IEnumerable<SimInstallKey> enumerable = Enumerable.Where<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)_003C_003E8__1.activeSimKeys, (Func<SimInstallKey, bool>)delegate(SimInstallKey activeKey)
				{
					_003C_003Ec__DisplayClass101_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass101_1
					{
						activeKey = activeKey
					};
					return !Enumerable.Any<SimInstallData>((System.Collections.Generic.IEnumerable<SimInstallData>)_003C_003E8__1.allPersistedSimulators, (Func<SimInstallData, bool>)((SimInstallData ps) => ps.InstallationKey == CS_0024_003C_003E8__locals0.activeKey));
				});
				if (Enumerable.Any<SimInstallKey>(enumerable))
				{
					throw new FormatException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Active simulators are missing from disk: [{0}]", (object)string.Join<SimInstallKey>(", ", enumerable)));
				}
				try
				{
					result2 = new SimPanel().WithUpdatedSimulators((System.Collections.Generic.IEnumerable<SimInstallData>)_003C_003E8__1.allPersistedSimulators);
				}
				catch (ArgumentException val)
				{
					ArgumentException val2 = val;
					throw new FormatException("Corrupted Panel files detected.", (System.Exception)(object)val2);
				}
				goto end_IL_000e;
				IL_00fa:
				PersistedSimPanelInfo persistedSimPanelInfo2 = persistedSimPanelInfo;
				_003C_003E8__1.activeSimKeys = Enumerable.ToHashSet<SimInstallKey>(persistedSimPanelInfo2.ActiveSimulatorKeys);
				asyncSubject = directoryBasedHangar.ListSimDirectories(_003C_003E8__1.avionicsDirPath).SelectMany<string, SimInstallData>((Func<string, System.Threading.Tasks.Task<SimInstallData>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass101_0._003C_003CGetPersistedPanelWithLock_003Eb__1_003Ed))] async (string simDirPath) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					string text = Path.Combine(simDirPath, "simulator.json");
					if (_003C_003E8__1._003C_003E4__this._fs.PathExists(text))
					{
						PersistedSimulator persistedSimulator = await _003C_003E8__1._003C_003E4__this._persistence.Load<PersistedSimulator>(text).DetachContext();
						SimInstallKey simInstallKey = SimInstallKey.Parse(PathHelpers.CreateRelativePath(_003C_003E8__1.avionicsDirPath, simDirPath));
						return new SimInstallData(new SimAircraftKey(_003C_003E8__1.aircraftIdent, simInstallKey), new FullModelIdentifier(SettingIdentifier.FromString(persistedSimulator.FullModelId)).TrainerPackageId, (System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>?)Enumerable.ToDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)persistedSimulator.Options, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> o) => new SettingIdentifier(o.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> o) => o.Value)), new ReadOnlyJsonDataSource(persistedSimulator.ModuleData), _003C_003E8__1._003C_003E4__this._fs.PathExists(Path.Combine(simDirPath, "winfs_mount.gca")), _003C_003E8__1.activeSimKeys.Contains(simInstallKey));
					}
					throw new FileNotFoundException("Missing simulator data.", text);
				})).ToList<SimInstallData>()
					.GetAwaiter<System.Collections.Generic.IList<SimInstallData>>();
				if (!asyncSubject.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = asyncSubject;
					_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<System.Collections.Generic.IList<SimInstallData>>, _003CGetPersistedPanelWithLock_003Ed__101>(ref asyncSubject, ref this);
					return;
				}
				goto IL_0192;
				end_IL_000e:;
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Ed__103 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SimPanel> _003C_003Et__builder;

		public AircraftRepoInfo repoInfo;

		public string repoAction;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		private _003C_003Ec__DisplayClass103_0 _003C_003E8__1;

		public SimPanel simulatorPanel;

		private SimPanel _003CpreviousPanel_003E5__2;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			SimPanel result2;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				ConfiguredTaskAwaiter<SimPanel> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_020d;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass103_0();
					_003C_003E8__1.repoInfo = repoInfo;
					_003C_003E8__1.repoAction = repoAction;
					_003C_003E8__1.aircraftIdentifier = aircraftIdentifier;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					awaiter2 = directoryBasedHangar.GetPersistedPanelWithLock(_003C_003E8__1.aircraftIdentifier, _003C_003E8__1.repoInfo).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Ed__103>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
				}
				SimPanel result = awaiter2.GetResult();
				_003CpreviousPanel_003E5__2 = result;
				_003C_003E8__1.updatedSimulatorJsonFiles = Enumerable.ToList<KeyValuePair<string, PersistedSimulator>>(Enumerable.Select<SimInstallData, KeyValuePair<string, PersistedSimulator>>(simulatorPanel.AllSimulators.Values, (Func<SimInstallData, KeyValuePair<string, PersistedSimulator>>)((SimInstallData sim) => ConvertToPersistedSimulatorAndLocation(sim, _003C_003E8__1.repoInfo))));
				_003C_003E8__1.removedSimulatorJsonFiles = Enumerable.ToList<string>(Enumerable.Select<SimInstallKey, string>(_003CpreviousPanel_003E5__2.SimulatorChanges(simulatorPanel).FilterByChangeType(DataChangeType.Deleted).StripChangeType(), (Func<SimInstallKey, string>)((SimInstallKey deletedKey) => BuildSimJsonFilePath(deletedKey, _003C_003E8__1.repoInfo))));
				_003C_003E8__1.panelInfoToSave = PersistedSimPanelInfo.FromRuntime(simulatorPanel);
				ConfiguredTaskAwaitable val = TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed);
					_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003E4__this = _003C_003E8__1;
					_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed>(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder)).Task;
				}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed);
					_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003E4__this = _003C_003E8__1;
					_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed>(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder)).Task;
				}).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Ed__103>(ref awaiter, ref this);
					return;
				}
				goto IL_020d;
				IL_020d:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				result2 = _003CpreviousPanel_003E5__2;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CpreviousPanel_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CpreviousPanel_003E5__2 = null;
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
	private struct _003CGetSimulator_003Ed__40 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SimInstallData> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public SimInstallKey simulatorKey;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			SimInstallData result2;
			try
			{
				ConfiguredTaskAwaiter<SimPanel> awaiter;
				if (num != 0)
				{
					awaiter = directoryBasedHangar.GetSimulatorPanel(aircraftIdentifier).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetSimulator_003Ed__40>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
				}
				SimPanel result = awaiter.GetResult();
				if (!result.AllSimulators.ContainsKey(simulatorKey))
				{
					throw new SimulatorNotFoundException("The simulator was not found in the aircraft. " + (object)simulatorKey);
				}
				result2 = result.AllSimulators[simulatorKey];
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
	private struct _003CGetSimulatorPanel_003Ed__39 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<SimPanel> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			SimPanel result;
			try
			{
				ConfiguredTaskAwaiter<SimPanel> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass39_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass39_0
					{
						_003C_003E4__this = _003C_003E4__this,
						aircraftIdentifier = aircraftIdentifier
					};
					awaiter = directoryBasedHangar.ReadAircraftHarness<SimPanel>(CS_0024_003C_003E8__locals0.aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass39_0._003C_003CGetSimulatorPanel_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) => await CS_0024_003C_003E8__locals0._003C_003E4__this.GetPersistedPanelWithLock(CS_0024_003C_003E8__locals0.aircraftIdentifier, repoInfo).DetachContext()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CGetSimulatorPanel_003Ed__39>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CImportAircaft_003Ed__61 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

		public AircraftListData newListData;

		public DirectoryBasedHangar _003C_003E4__this;

		public string gcaPath;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotData result;
			try
			{
				ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass61_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass61_0
					{
						newListData = newListData,
						_003C_003E4__this = _003C_003E4__this,
						gcaPath = gcaPath
					};
					awaiter = directoryBasedHangar.TemporarilyUnzipAircraftGca<AircraftSnapshotData>(CS_0024_003C_003E8__locals0.gcaPath, "Import Aircraft", [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass61_0._003C_003CImportAircaft_003Eb__0_003Ed))] async (string tempAircraftRootPath, AircraftIdentifier exportId) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						CS_0024_003C_003E8__locals0.newListData = await HangarUtil.PrepareStartingInfoWithUniqueId(CS_0024_003C_003E8__locals0.newListData, AircraftIdentifier.GenerateIdentifiers(CS_0024_003C_003E8__locals0.newListData.AirframeCCT, "R"), CS_0024_003C_003E8__locals0._003C_003E4__this.ReserveAircraftDirectoryIfNotTaken).DetachContext();
						AircraftIdentifier aircraftIdentifier = CS_0024_003C_003E8__locals0.newListData.AircraftIdentifier;
						AircraftRepoInfo repoInfo = CS_0024_003C_003E8__locals0._003C_003E4__this.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
						string aircraftDirectoryPath = repoInfo.RootPath;
						AircraftSnapshotData result2;
						using (await CS_0024_003C_003E8__locals0._003C_003E4__this.GetLockForPathUnlocked(repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
						{
							CS_0024_003C_003E8__locals0._003C_003E4__this.ThrowIfAircraftDirectoryIsNotEmpty(aircraftIdentifier, aircraftDirectoryPath);
							try
							{
								AircraftRepoInfo tempRepoInfo = CreateRepoInfo(tempAircraftRootPath);
								AircraftListData persistedListData = await CS_0024_003C_003E8__locals0._003C_003E4__this.GetListDataWithLock(tempRepoInfo).DetachContext();
								if (aircraftIdentifier != exportId)
								{
									LogRepoMessage(LogLevel.Info, "Import Aircraft", "Changing identifiers ({0} -> {1}) for imported GCA: {2}", exportId, aircraftIdentifier, PathHelpers.QuotePathIfNeeded(CS_0024_003C_003E8__locals0.gcaPath));
									persistedListData = persistedListData.WithNewIdentifier(aircraftIdentifier);
									await CS_0024_003C_003E8__locals0._003C_003E4__this.UpdateListDataWithLock("Import Aircraft", aircraftIdentifier, tempRepoInfo, persistedListData).DetachContext();
									string text = Path.Combine(GetParentDirectoryPath(tempAircraftRootPath), aircraftIdentifier.IdentStringChecked);
									CS_0024_003C_003E8__locals0._003C_003E4__this._fs.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, tempAircraftRootPath, text);
									tempAircraftRootPath = text;
									tempRepoInfo = CreateRepoInfo(tempAircraftRootPath);
								}
								if (persistedListData.DisplayName != CS_0024_003C_003E8__locals0.newListData.DisplayName)
								{
									persistedListData = persistedListData.Rename(CS_0024_003C_003E8__locals0.newListData.DisplayName);
									LogRepoMessage(LogLevel.Info, "Import Aircraft", "Changing display name ({0} -> {1}) for imported GCA: {2}", persistedListData.DisplayName, CS_0024_003C_003E8__locals0.newListData.DisplayName, PathHelpers.QuotePathIfNeeded(CS_0024_003C_003E8__locals0.gcaPath));
									await CS_0024_003C_003E8__locals0._003C_003E4__this.UpdateListDataWithLock("Import Aircraft", aircraftIdentifier, tempRepoInfo, persistedListData).DetachContext();
								}
								LogRepoMessage(LogLevel.Info, "Import Aircraft", "Moving temporary import directory into the hangar: {0} -> {1}", PathHelpers.QuotePathIfNeeded(tempAircraftRootPath), PathHelpers.QuotePathIfNeeded(aircraftDirectoryPath));
								CS_0024_003C_003E8__locals0._003C_003E4__this._fs.MoveOrCopyAndDeleteToDifferentDriveAndOverwrite(LolaFsExtensions.DefaultTempFactory, tempAircraftRootPath, aircraftDirectoryPath);
								result2 = await CS_0024_003C_003E8__locals0._003C_003E4__this.GetAircraftSnapshotWithLock(aircraftIdentifier, repoInfo).DetachContext();
							}
							catch
							{
								CS_0024_003C_003E8__locals0._003C_003E4__this._fs.DeleteIfExists(aircraftDirectoryPath);
								throw;
							}
						}
						CS_0024_003C_003E8__locals0._003C_003E4__this.PostAircraftChange(aircraftIdentifier, DataChangeType.Created);
						return result2;
					}).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CImportAircaft_003Ed__61>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CMigrateFamilyIdToSideBySideVersionId_003Ed__130 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public string avionicsDirectory;

		public FamilyIdMigration familyMigration;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_013b: Expected O, but got Unknown
			//IL_010b: Expected O, but got Unknown
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				AsyncSubject<Unit> asyncSubject;
				if (num == 0)
				{
					asyncSubject = (AsyncSubject<Unit>)_003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
					goto IL_01b5;
				}
				_003C_003Ec__DisplayClass130_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass130_0
				{
					_003C_003E4__this = _003C_003E4__this,
					avionicsDirectory = avionicsDirectory
				};
				InstallationSlot slot = familyMigration.Slot;
				FullModelIdentifier fullModelId = familyMigration.FullModelId;
				string text = Path.Combine(CS_0024_003C_003E8__locals0.avionicsDirectory, slot.SerializedSlot, fullModelId.TrainerPackageId.FamilyId.IdentStringChecked);
				if (directoryBasedHangar._fs.PathExists(text))
				{
					string text2 = Path.Combine(CS_0024_003C_003E8__locals0.avionicsDirectory, slot.SerializedSlot, (string)fullModelId.TrainerPackageId.EmbeddedReleaseCompositeId);
					if (!directoryBasedHangar._fs.PathExists(text2))
					{
						Logger.Log(LogLevel.Info, "Renaming the V1(FamilyId) sim directory '{0}' to V2(SbS version) directory '{1}'.", text, Path.GetFileName(text2));
						try
						{
							directoryBasedHangar._fs.Move(text, text2);
						}
						catch (IOException val)
						{
							Logger.LogFormattedException((System.Exception)val, "Could not rename the V1(FamilyId) sim directory '{0}' to the V2(SbS version) directory '{1}'.", text, Path.GetFileName(text2));
							try
							{
								directoryBasedHangar._fs.Move(text, text + ".backup");
							}
							catch (IOException val2)
							{
								Logger.LogFormattedException((System.Exception)val2, "Could not backup the V1(FamilyId) sim directory '{0}'", text);
								throw;
							}
						}
						asyncSubject = SafeDirectoryStream(directoryBasedHangar._fs, text2).Select<FileSystemEntry, IObservable<Unit>>((Func<FileSystemEntry, IObservable<Unit>>)((FileSystemEntry simDir) => Observable.FromAsync((Func<CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass130_1
						{
							CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals0,
							simDir = simDir
						}._003CMigrateFamilyIdToSideBySideVersionId_003Eb__1))).Merge<Unit>().LastOrDefaultAsync<Unit>()
							.GetAwaiter<Unit>();
						if (!asyncSubject.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = asyncSubject;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<Unit>, _003CMigrateFamilyIdToSideBySideVersionId_003Ed__130>(ref asyncSubject, ref this);
							return;
						}
						goto IL_01b5;
					}
					Logger.Log(LogLevel.Warning, "Could not migrate the V1(FamilyId) sim directory '{0}' because the V2(SbS version) directory '{1}'already exists.", text, text2);
				}
				goto end_IL_000e;
				IL_01b5:
				asyncSubject.GetResult();
				end_IL_000e:;
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CMountHistoricalSnapshot_003Ed__67 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TemporaryHangarDirectory> _003C_003Et__builder;

		public SnapshotIdentifier snapshotIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private ConfiguredTaskAwaiter<TemporaryHangarDirectory> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			TemporaryHangarDirectory result;
			try
			{
				ConfiguredTaskAwaiter<TemporaryHangarDirectory> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass67_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass67_0
					{
						snapshotIdentifier = snapshotIdentifier,
						_003C_003E4__this = _003C_003E4__this,
						aircraftIdentifier = aircraftIdentifier
					};
					awaiter = directoryBasedHangar.ReadAircraftHarnessWithRollbackHandling<TemporaryHangarDirectory>(CS_0024_003C_003E8__locals0.aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass67_0._003C_003CMountHistoricalSnapshot_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo, AircraftRollbackException? e) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						string text = Path.Combine(repoInfo.HistoryAircraftRoot, CS_0024_003C_003E8__locals0.snapshotIdentifier.Serialized);
						if (!CS_0024_003C_003E8__locals0._003C_003E4__this._fs.PathExists(text))
						{
							throw new AircraftSnapshotNotFoundException("The snapshot does not exist.");
						}
						string snapShotArchivePathOrThrow = CS_0024_003C_003E8__locals0._003C_003E4__this.GetSnapShotArchivePathOrThrow(text);
						TemporaryHangarDirectory tempHangar = TemporaryHangarDirectory.CreateNewUniqueHangar(CS_0024_003C_003E8__locals0._003C_003E4__this._lowLevel);
						try
						{
							await CS_0024_003C_003E8__locals0._003C_003E4__this.ExtractSnapshotIntoTemporaryHangar("MOUNT_SNAP", tempHangar, CS_0024_003C_003E8__locals0.aircraftIdentifier, snapShotArchivePathOrThrow).ConfigureAwait(false);
							return tempHangar;
						}
						catch
						{
							tempHangar.Dispose();
							throw;
						}
					}).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TemporaryHangarDirectory>, _003CMountHistoricalSnapshot_003Ed__67>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TemporaryHangarDirectory>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CParseAircraftGCA_003Ed__60 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotData> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public string gcaPath;

		private ConfiguredTaskAwaiter<AircraftSnapshotData> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			AircraftSnapshotData result;
			try
			{
				ConfiguredTaskAwaiter<AircraftSnapshotData> awaiter;
				if (num != 0)
				{
					awaiter = CS_0024_003C_003E8__locals0.TemporarilyUnzipAircraftGca<AircraftSnapshotData>(gcaPath, "Parse GCA", [AsyncStateMachine(typeof(_003C_003CParseAircraftGCA_003Eb__60_0_003Ed))] [CompilerGenerated] async (string tempAircraftRootPath, AircraftIdentifier exportId) => await CS_0024_003C_003E8__locals0.GetAircraftSnapshotWithLock(exportId, CreateRepoInfo(tempAircraftRootPath)).DetachContext()).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftSnapshotData>, _003CParseAircraftGCA_003Ed__60>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftSnapshotData>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CPerformAircraftFormatMigrationWithLock_003Ed__143 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftRepoInfo repoInfo;

		private _003C_003Ec__DisplayClass143_1 _003C_003E8__1;

		public CancellationToken token;

		private _003C_003Ec__DisplayClass143_0 _003C_003E8__2;

		private string _003ColdAircraftFilePath_003E5__2;

		private object _003C_003Eu__1;

		private ConfiguredTaskAwaiter<PersistedAircraft> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0454: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				if ((uint)num <= 4u)
				{
					goto IL_0131;
				}
				_003C_003E8__2 = new _003C_003Ec__DisplayClass143_0();
				_003C_003E8__2._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__2.repoInfo = repoInfo;
				string rootPath = _003C_003E8__2.repoInfo.RootPath;
				if (directoryBasedHangar.NeedsAMigration(rootPath))
				{
					string fileName = Path.GetFileName(rootPath);
					_003C_003E8__2.aircraftIdentifier = (AircraftIdentifier)fileName;
					if (!_003C_003E8__2.aircraftIdentifier.IsValid)
					{
						throw new FormatException("Aircraft directories must be named with their identifier.");
					}
					LogRepoMessage(LogLevel.Info, "Aircraft Migration", $"Starting '{_003C_003E8__2.aircraftIdentifier}'");
					int? migrationLevel = directoryBasedHangar.GetMigrationLevel(rootPath);
					if (migrationLevel.GetValueOrDefault() > 1)
					{
						Logger.Log(LogLevel.Warning, $"An aircraft from a future version of the Launcher was encountered. Format level: {migrationLevel.GetValueOrDefault()}");
						throw new AircraftRollbackException("This aircraft is too new for this version of the launcher.");
					}
					goto IL_0131;
				}
				goto end_IL_000e;
				IL_0131:
				try
				{
					AsyncSubject<Unit> asyncSubject2;
					AsyncSubject<string> asyncSubject;
					ConfiguredTaskAwaiter<PersistedAircraft> awaiter2;
					ConfiguredTaskAwaiter awaiter;
					ConfiguredTaskAwaitable val;
					PersistedAircraft result;
					switch (num)
					{
					default:
						_003C_003E8__1 = new _003C_003Ec__DisplayClass143_1();
						_003C_003E8__1.CS_0024_003C_003E8__locals1 = _003C_003E8__2;
						_003C_003E8__1.aircraftAvionicsDirectoryPath = directoryBasedHangar.MigrateRootSlotsIntoAvionicsDirectory(_003C_003E8__1.CS_0024_003C_003E8__locals1.aircraftIdentifier, _003C_003E8__1.CS_0024_003C_003E8__locals1.repoInfo.RootPath);
						directoryBasedHangar.MigratePrototypeToHistoryDirectory(_003C_003E8__1.CS_0024_003C_003E8__locals1.repoInfo);
						_003C_003E8__1.aircraftAvionicsDirectoryPath = directoryBasedHangar.MigrateAvionicsDirIntoCurrentAndHistoryFormat(_003C_003E8__1.CS_0024_003C_003E8__locals1.repoInfo);
						asyncSubject2 = directoryBasedHangar.ListOldFamilyIdDirectoriesIn(_003C_003E8__1.aircraftAvionicsDirectoryPath).Select<FamilyIdMigration, IObservable<Unit>>((Func<FamilyIdMigration, IObservable<Unit>>)((FamilyIdMigration v1FamilyDirectory) => Observable.FromAsync((Func<System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass143_2
						{
							CS_0024_003C_003E8__locals2 = _003C_003E8__1,
							v1FamilyDirectory = v1FamilyDirectory
						}._003CPerformAircraftFormatMigrationWithLock_003Eb__4))).Concat<Unit>()
							.LastOrDefaultAsync<Unit>()
							.GetAwaiter<Unit>();
						if (!asyncSubject2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = asyncSubject2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<Unit>, _003CPerformAircraftFormatMigrationWithLock_003Ed__143>(ref asyncSubject2, ref this);
							return;
						}
						goto IL_025b;
					case 0:
						asyncSubject2 = (AsyncSubject<Unit>)_003C_003Eu__1;
						_003C_003Eu__1 = null;
						num = (_003C_003E1__state = -1);
						goto IL_025b;
					case 1:
						asyncSubject = (AsyncSubject<string>)_003C_003Eu__1;
						_003C_003Eu__1 = null;
						num = (_003C_003E1__state = -1);
						goto IL_02f0;
					case 2:
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<PersistedAircraft>);
						num = (_003C_003E1__state = -1);
						goto IL_03b7;
					case 3:
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_046b;
					case 4:
						{
							awaiter = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							break;
						}
						IL_046b:
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
						directoryBasedHangar._fs.DeleteIfExists(_003ColdAircraftFilePath_003E5__2);
						val = directoryBasedHangar.PlaceMigrationFile(_003C_003E8__1.CS_0024_003C_003E8__locals1.repoInfo).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 4);
							_003C_003Eu__3 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPerformAircraftFormatMigrationWithLock_003Ed__143>(ref awaiter, ref this);
							return;
						}
						break;
						IL_025b:
						asyncSubject2.GetResult();
						asyncSubject = directoryBasedHangar.ListSimDirectoriesContainingLegacyWinfsMountDirs(_003C_003E8__1.aircraftAvionicsDirectoryPath).CancelWithToken<string>(token).SelectMany<string, string>((Func<string, IObservable<string>>)((string dir) => Observable.Return(dir).Do<string>((Action<string>)delegate(string d)
						{
							_003C_003E8__1.CS_0024_003C_003E8__locals1._003C_003E4__this.MigrateSimWinfsDirectoryToWinfsZip(d);
						}).SubscribeOn<string>((IScheduler)Scheduler.Default)))
							.LastOrDefaultAsync<string>()
							.GetAwaiter<string>();
						if (!asyncSubject.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = asyncSubject;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<string>, _003CPerformAircraftFormatMigrationWithLock_003Ed__143>(ref asyncSubject, ref this);
							return;
						}
						goto IL_02f0;
						IL_03b7:
						result = awaiter2.GetResult();
						_003C_003E8__1.persistedAircraft = result;
						val = TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed);
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003E4__this = _003C_003E8__1;
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed>(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__0_003Ed._003C_003Et__builder)).Task;
						}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed);
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003E4__this = _003C_003E8__1;
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed>(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__1_003Ed._003C_003Et__builder)).Task;
						}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed);
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003E4__this = _003C_003E8__1;
							_003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass143_1._003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed>(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CPerformAircraftFormatMigrationWithLock_003Eb__2_003Ed._003C_003Et__builder)).Task;
						}).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 3);
							_003C_003Eu__3 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPerformAircraftFormatMigrationWithLock_003Ed__143>(ref awaiter, ref this);
							return;
						}
						goto IL_046b;
						IL_02f0:
						asyncSubject.GetResult();
						_003C_003E8__1.currentAircraftDirectoryPath = _003C_003E8__1.CS_0024_003C_003E8__locals1.repoInfo.CurrentAircraftRoot;
						_003ColdAircraftFilePath_003E5__2 = Path.Combine(_003C_003E8__1.currentAircraftDirectoryPath, "aircraft.json");
						if (!directoryBasedHangar._fs.PathExists(_003ColdAircraftFilePath_003E5__2))
						{
							throw new FormatException("Could not migrate because a required file is missing.");
						}
						awaiter2 = directoryBasedHangar.DeserializeAircraft(_003ColdAircraftFilePath_003E5__2).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedAircraft>, _003CPerformAircraftFormatMigrationWithLock_003Ed__143>(ref awaiter2, ref this);
							return;
						}
						goto IL_03b7;
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					_003C_003E8__1 = null;
					_003ColdAircraftFilePath_003E5__2 = null;
				}
				catch (System.Exception e)
				{
					LogRepoError(e, "Aircraft Migration");
					throw;
				}
				LogRepoMessage(LogLevel.Info, "Aircraft Migration", $"Finished '{_003C_003E8__2.aircraftIdentifier}'");
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__2 = null;
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
	private struct _003CPlaceMigrationFile_003Ed__126 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftRepoInfo repoInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		private string _003CmigrationPath_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				if (num != 0)
				{
					_003CmigrationPath_003E5__2 = Path.Combine(repoInfo.CurrentAircraftRoot, "hangarFormat.dat");
					Logger.Log(LogLevel.Info, "Writing the migration file({0}) at format level {1}", PathHelpers.QuotePathIfNeeded(_003CmigrationPath_003E5__2), 1);
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						directoryBasedHangar._fs.CreateDirectory(GetParentDirectoryPath(_003CmigrationPath_003E5__2));
						ConfiguredTaskAwaitable val = directoryBasedHangar._fs.WriteAllText(_003CmigrationPath_003E5__2, 1.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture)).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPlaceMigrationFile_003Ed__126>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				}
				catch (System.Exception e)
				{
					Logger.LogFormattedException(e, "Writing migration file: {0}", PathHelpers.QuotePathIfNeeded(_003CmigrationPath_003E5__2));
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CmigrationPath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CmigrationPath_003E5__2 = null;
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
	private struct _003CReadAircraftHarness_003Ed__41<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public CancellationToken token;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public Func<AircraftRepoInfo, System.Threading.Tasks.Task<TResult>> readAircraft;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			TResult result;
			try
			{
				ConfiguredTaskAwaiter<TResult> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass41_0<TResult> CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass41_0<TResult>
					{
						token = token
					};
					awaiter = directoryBasedHangar.AircraftOpHarness<TResult>((AsyncReaderWriterLock l) => l.ReaderLockAsync(CS_0024_003C_003E8__locals0.token), aircraftIdentifier, readAircraft, CS_0024_003C_003E8__locals0.token).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CReadAircraftHarness_003Ed__41<TResult>>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TResult>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CReadAircraftHarnessWithRollbackHandling_003Ed__43<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public CancellationToken token;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public Func<AircraftRepoInfo, AircraftRollbackException?, System.Threading.Tasks.Task<TResult>> readAircraft;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			TResult result;
			try
			{
				ConfiguredTaskAwaiter<TResult> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass43_0<TResult> CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass43_0<TResult>
					{
						token = token
					};
					awaiter = directoryBasedHangar.AircraftOpHarnessWithRollbackHandling<TResult>((AsyncReaderWriterLock l) => l.ReaderLockAsync(CS_0024_003C_003E8__locals0.token), aircraftIdentifier, readAircraft, CS_0024_003C_003E8__locals0.token).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CReadAircraftHarnessWithRollbackHandling_003Ed__43<TResult>>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TResult>);
					num = (_003C_003E1__state = -1);
				}
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
	private struct _003CReserveAircraftDirectoryIfNotTaken_003Ed__111 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier id;

		private AircraftRepoInfo _003CtmpRepoInfo_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			bool result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					_003CtmpRepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(id);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CtmpRepoInfo_003E5__2.RootPath).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CReserveAircraftDirectoryIfNotTaken_003Ed__111>(ref awaiter, ref this);
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
					if (directoryBasedHangar._fs.PathExists(_003CtmpRepoInfo_003E5__2.RootPath))
					{
						LogRepoMessage(LogLevel.Debug, "Create new aircraft", ": 'Aircraft id already taken: {0}'", id);
						goto end_IL_00a5;
					}
					directoryBasedHangar._fs.CreateDirectory(_003CtmpRepoInfo_003E5__2.RootPath);
					LogRepoMessage(LogLevel.Info, "Create new aircraft", "Found new unused aircraft id: '{0}'", id);
					result2 = true;
					goto end_IL_000e;
					end_IL_00a5:;
				}
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
					}
				}
				result2 = false;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CtmpRepoInfo_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CtmpRepoInfo_003E5__2 = null;
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
	private struct _003CReserveFirstUnusedIdFrom_003Ed__110 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftIdentifier> _003C_003Et__builder;

		public System.Collections.Generic.IEnumerable<AircraftIdentifier> possibleIds;

		public DirectoryBasedHangar _003C_003E4__this;

		private ConfiguredTaskAwaiter<AircraftIdentifier> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftIdentifier result;
			try
			{
				ConfiguredTaskAwaiter<AircraftIdentifier> awaiter;
				if (num != 0)
				{
					awaiter = HangarUtil.ReserveFirstUnusedIdFrom(possibleIds, directoryBasedHangar.ReserveAircraftDirectoryIfNotTaken).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftIdentifier>, _003CReserveFirstUnusedIdFrom_003Ed__110>(ref awaiter, ref this);
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
	private struct _003CRevertAircraftToSnapshot_003Ed__66 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public SnapshotIdentifier snapshotIdentifier;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private TemporaryHangarDirectory _003CtempHangar_003E5__4;

		private AircraftRepoInfo _003CextractedRepoInfo_003E5__5;

		private ConfiguredTaskAwaiter<AircraftRepoInfo> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<AircraftListData> _003C_003Eu__3;

		private ConfiguredTaskAwaiter _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 2u)
					{
						goto IL_00c1;
					}
					ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
					_003CrepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CRevertAircraftToSnapshot_003Ed__66>(ref awaiter, ref this);
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
				goto IL_00c1;
				IL_00c1:
				try
				{
					string snapShotArchivePathOrThrow = default(string);
					if ((uint)(num - 1) > 2u)
					{
						directoryBasedHangar.ThrowNotFoundIfAircraftDirectoryIsMissing(_003CrepoInfo_003E5__2);
						string text = Path.Combine(_003CrepoInfo_003E5__2.HistoryAircraftRoot, snapshotIdentifier.Serialized);
						if (!directoryBasedHangar._fs.PathExists(text))
						{
							throw new AircraftSnapshotNotFoundException("The snapshot does not exist.");
						}
						snapShotArchivePathOrThrow = directoryBasedHangar.GetSnapShotArchivePathOrThrow(text);
						if (IsProtoTypeArchiveFileName(snapShotArchivePathOrThrow))
						{
							throw new InvalidOperationException("Prototypes cannot be restored because they are typically incomplete.");
						}
						_003CtempHangar_003E5__4 = TemporaryHangarDirectory.CreateNewUniqueHangar(directoryBasedHangar._lowLevel);
					}
					try
					{
						ConfiguredTaskAwaiter<AircraftRepoInfo> awaiter4;
						ConfiguredTaskAwaiter<AircraftListData> awaiter3;
						ConfiguredTaskAwaiter awaiter2;
						AircraftListData result2;
						ConfiguredTaskAwaitable val;
						AircraftRepoInfo result3;
						switch (num)
						{
						default:
							awaiter4 = directoryBasedHangar.ExtractSnapshotIntoTemporaryHangar("REVERT_SNAP", _003CtempHangar_003E5__4, aircraftIdentifier, snapShotArchivePathOrThrow).ConfigureAwait(false).GetAwaiter();
							if (!awaiter4.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter4;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftRepoInfo>, _003CRevertAircraftToSnapshot_003Ed__66>(ref awaiter4, ref this);
								return;
							}
							goto IL_01c2;
						case 1:
							awaiter4 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AircraftRepoInfo>);
							num = (_003C_003E1__state = -1);
							goto IL_01c2;
						case 2:
							awaiter3 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<AircraftListData>);
							num = (_003C_003E1__state = -1);
							goto IL_023a;
						case 3:
							{
								awaiter2 = _003C_003Eu__4;
								_003C_003Eu__4 = default(ConfiguredTaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_033f;
							}
							IL_023a:
							result2 = awaiter3.GetResult();
							if (!(result2.AircraftIdentifier != aircraftIdentifier))
							{
								break;
							}
							LogRepoMessage(LogLevel.Info, "REVERT_SNAP", $"[{aircraftIdentifier}-{snapshotIdentifier}] Adjusting aircraft identifier to match this aircraft. This usually happens when a prototype is used. But can also happen when a temporary hangar is used for a transactional create.");
							result2 = result2.WithNewIdentifier(aircraftIdentifier);
							val = directoryBasedHangar.UpdateListDataWithLock("REVERT_SNAP", aircraftIdentifier, _003CextractedRepoInfo_003E5__5, result2).ConfigureAwait(false);
							awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
							{
								num = (_003C_003E1__state = 3);
								_003C_003Eu__4 = awaiter2;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRevertAircraftToSnapshot_003Ed__66>(ref awaiter2, ref this);
								return;
							}
							goto IL_033f;
							IL_01c2:
							result3 = awaiter4.GetResult();
							_003CextractedRepoInfo_003E5__5 = result3;
							awaiter3 = directoryBasedHangar.GetListDataWithLock(_003CextractedRepoInfo_003E5__5).ConfigureAwait(false).GetAwaiter();
							if (!awaiter3.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter3;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftListData>, _003CRevertAircraftToSnapshot_003Ed__66>(ref awaiter3, ref this);
								return;
							}
							goto IL_023a;
							IL_033f:
							((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
							break;
						}
						string currentAircraftRoot = _003CrepoInfo_003E5__2.CurrentAircraftRoot;
						string text2 = currentAircraftRoot + ".backup";
						directoryBasedHangar._fs.DeleteIfExists(text2);
						try
						{
							LogRepoMessage(LogLevel.Info, "REVERT_SNAP", $"[{aircraftIdentifier}-{snapshotIdentifier}] Backing up current directory.");
							directoryBasedHangar._fs.MoveAndCreatePath(currentAircraftRoot, text2);
							LogRepoMessage(LogLevel.Info, "REVERT_SNAP", $"[{aircraftIdentifier}-{snapshotIdentifier}] Moving current directory from extracted snapshot.");
							directoryBasedHangar._fs.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, _003CextractedRepoInfo_003E5__5.CurrentAircraftRoot, currentAircraftRoot);
						}
						catch (System.Exception)
						{
							if (directoryBasedHangar._fs.PathExists(text2))
							{
								LogRepoMessage(LogLevel.Error, "REVERT_SNAP", $"[{aircraftIdentifier}-{snapshotIdentifier}] Restoring current directory with backup.");
								directoryBasedHangar._fs.MoveAndOverwrite(text2, currentAircraftRoot);
							}
							throw;
						}
						LogRepoMessage(LogLevel.Info, "REVERT_SNAP", $"[{aircraftIdentifier}-{snapshotIdentifier}] Deleting current directory backup.");
						directoryBasedHangar._fs.Delete(text2);
					}
					finally
					{
						if (num < 0 && _003CtempHangar_003E5__4 != null)
						{
							((System.IDisposable)_003CtempHangar_003E5__4).Dispose();
						}
					}
					_003CtempHangar_003E5__4 = null;
					_003CextractedRepoInfo_003E5__5 = null;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				LogRepoMessage(LogLevel.Info, "REVERT_SNAP", $"[{aircraftIdentifier}-{snapshotIdentifier}] Done.");
				directoryBasedHangar._hangarEvents.PostAircraftChange(DataChangeNotification.Create(aircraftIdentifier));
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CrepoInfo_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrepoInfo_003E5__2 = null;
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
	private struct _003CStartMigrations_003Ed__31 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public CancellationToken token;

		private System.Exception _003Cexception_003E5__2;

		private System.Collections.Generic.IEnumerator<FileSystemEntry> _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				if (num == 0)
				{
					goto IL_006c;
				}
				string rootDirectory = directoryBasedHangar.RootDirectory;
				if (directoryBasedHangar._fs.PathExists(rootDirectory))
				{
					_003Cexception_003E5__2 = null;
					_003C_003E7__wrap2 = Enumerable.Where<FileSystemEntry>(directoryBasedHangar._fs.DirectoriesIn(rootDirectory), (Func<FileSystemEntry, bool>)((FileSystemEntry ad) => AircraftIdentifier.IsValidIdentifier(ad.Name))).GetEnumerator();
					goto IL_006c;
				}
				goto end_IL_000e;
				IL_006c:
				try
				{
					if (num == 0)
					{
						goto IL_00bf;
					}
					goto IL_0161;
					IL_0161:
					AircraftRepoInfo repoInfo = default(AircraftRepoInfo);
					if (((System.Collections.IEnumerator)_003C_003E7__wrap2).MoveNext())
					{
						FileSystemEntry current = _003C_003E7__wrap2.Current;
						if (((CancellationToken)(ref token)).CanBeCanceled && ((CancellationToken)(ref token)).IsCancellationRequested)
						{
							if (_003Cexception_003E5__2 != null)
							{
								throw _003Cexception_003E5__2;
							}
							((CancellationToken)(ref token)).ThrowIfCancellationRequested();
						}
						repoInfo = CreateRepoInfo(current.FullPath);
						goto IL_00bf;
					}
					goto end_IL_006c;
					IL_00bf:
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = directoryBasedHangar.CheckForAndPerformMigration(repoInfo, token).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CStartMigrations_003Ed__31>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					}
					catch (OperationCanceledException)
					{
						if (_003Cexception_003E5__2 != null)
						{
							throw _003Cexception_003E5__2;
						}
						throw;
					}
					catch (System.Exception next)
					{
						_003Cexception_003E5__2 = _003Cexception_003E5__2.BuildUpException(next, "Migration Errors");
					}
					goto IL_0161;
					end_IL_006c:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						((System.IDisposable)_003C_003E7__wrap2).Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				if (_003Cexception_003E5__2 != null)
				{
					throw _003Cexception_003E5__2;
				}
				_003Cexception_003E5__2 = null;
				end_IL_000e:;
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CStoreLastSelectedAircraft_003Ed__33 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		private PersistedSelectedAircraft _003CpersistedSelectedAircraft_003E5__2;

		private string _003CselectedAircraftFilePath_003E5__3;

		private AircraftIdentifier _003CpreviousCachedSelectedId_003E5__4;

		private System.IDisposable _003C_003E7__wrap4;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00d5;
					}
					_003CpersistedSelectedAircraft_003E5__2 = new PersistedSelectedAircraft
					{
						SelectedAircraft = (aircraftIdentifier.IdentString ?? string.Empty)
					};
					directoryBasedHangar._fs.CreateDirectory(directoryBasedHangar.RootDirectory);
					_003CselectedAircraftFilePath_003E5__3 = directoryBasedHangar.BuildSelectedAircraftFilepath();
					awaiter = directoryBasedHangar._selectedLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CStoreLastSelectedAircraft_003Ed__33>(ref awaiter, ref this);
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
				_003C_003E7__wrap4 = result;
				goto IL_00d5;
				IL_00d5:
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					if (num == 1)
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0182;
					}
					_003CpreviousCachedSelectedId_003E5__4 = directoryBasedHangar._cachedSelectedAircraftId;
					if (_003CpreviousCachedSelectedId_003E5__4 != aircraftIdentifier || !directoryBasedHangar._fs.PathExists(_003CselectedAircraftFilePath_003E5__3))
					{
						ConfiguredTaskAwaitable val = directoryBasedHangar._persistence.Commit(_003CpersistedSelectedAircraft_003E5__2, _003CselectedAircraftFilePath_003E5__3).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CStoreLastSelectedAircraft_003Ed__33>(ref awaiter2, ref this);
							return;
						}
						goto IL_0182;
					}
					goto end_IL_00d5;
					IL_0182:
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					directoryBasedHangar._cachedSelectedAircraftId = aircraftIdentifier;
					end_IL_00d5:;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap4 != null)
					{
						_003C_003E7__wrap4.Dispose();
					}
				}
				_003C_003E7__wrap4 = null;
				if (aircraftIdentifier != _003CpreviousCachedSelectedId_003E5__4)
				{
					directoryBasedHangar.PostSelectedAircaftChange(aircraftIdentifier);
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CpersistedSelectedAircraft_003E5__2 = null;
				_003CselectedAircraftFilePath_003E5__3 = null;
				_003CpreviousCachedSelectedId_003E5__4 = default(AircraftIdentifier);
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CpersistedSelectedAircraft_003E5__2 = null;
			_003CselectedAircraftFilePath_003E5__3 = null;
			_003CpreviousCachedSelectedId_003E5__4 = default(AircraftIdentifier);
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
	private struct _003CTakePrototypeSnapShotWithLock_003Ed__121 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotInfo> _003C_003Et__builder;

		public string repoAction;

		public AircraftRepoInfo repoInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private DateTimeOffset _003CtakenAt_003E5__2;

		private SnapshotIdentifier _003CsnapshotId_003E5__3;

		private string _003CsnapshotDirPath_003E5__4;

		private string _003CtemporaryArchivePath_003E5__5;

		private PersistedAircraftListData _003CprototypeListData_003E5__6;

		private AircraftSnapshotMetaData _003Cmetadata_003E5__7;

		private ConfiguredTaskAwaiter<PersistedAircraftListData> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotInfo result2;
			try
			{
				string rootPath = default(string);
				if ((uint)num > 1u)
				{
					_003CtakenAt_003E5__2 = DateTimeOffset.Now;
					_003CsnapshotId_003E5__3 = new SnapshotIdentifier(_003CtakenAt_003E5__2);
					rootPath = repoInfo.RootPath;
					_003CsnapshotDirPath_003E5__4 = Path.Combine(repoInfo.HistoryAircraftRoot, _003CsnapshotId_003E5__3.Serialized);
					_003CtemporaryArchivePath_003E5__5 = Path.Combine(directoryBasedHangar.RootDirectory, ".tmp", aircraftIdentifier.IdentStringChecked, "prototype.gca");
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					ConfiguredTaskAwaiter<PersistedAircraftListData> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_0271;
						}
						directoryBasedHangar._fs.CreateDirectory(GetParentDirectoryPath(_003CtemporaryArchivePath_003E5__5));
						directoryBasedHangar._zipCompression.ZipDirectory(rootPath, _003CtemporaryArchivePath_003E5__5);
						directoryBasedHangar._fs.DeleteIfExists(repoInfo.HistoryAircraftRoot);
						string text = Path.Combine(_003CsnapshotDirPath_003E5__4, "prototype.gca");
						directoryBasedHangar._fs.CreateDirectory(GetParentDirectoryPath(text));
						directoryBasedHangar._fs.MoveOrCopyAndDeleteToDifferentDriveAndOverwrite(LolaFsExtensions.DefaultTempFactory, _003CtemporaryArchivePath_003E5__5, text);
						directoryBasedHangar._fs.SetFileTimes(text, new FileTimes(_003CtakenAt_003E5__2));
						string source = Path.Combine(repoInfo.CurrentAircraftRoot, "list.json");
						string text2 = Path.Combine(_003CsnapshotDirPath_003E5__4, "list.json");
						directoryBasedHangar._fs.CopyFile(source, text2, overwrite: false);
						awaiter2 = directoryBasedHangar._persistence.Load<PersistedAircraftListData>(text2).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PersistedAircraftListData>, _003CTakePrototypeSnapShotWithLock_003Ed__121>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<PersistedAircraftListData>);
						num = (_003C_003E1__state = -1);
					}
					PersistedAircraftListData result = awaiter2.GetResult();
					_003CprototypeListData_003E5__6 = result;
					_003Cmetadata_003E5__7 = new AircraftSnapshotMetaData("Prototype", null);
					string path = Path.Combine(_003CsnapshotDirPath_003E5__4, "metadata.json");
					ConfiguredTaskAwaitable val = directoryBasedHangar._persistence.Commit(PersistedSnapshotMetadata.FromRuntime(new AircraftSnapshotMetaData("Prototype", null)), path).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CTakePrototypeSnapShotWithLock_003Ed__121>(ref awaiter, ref this);
						return;
					}
					goto IL_0271;
					IL_0271:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					long snapShotStorageSize = directoryBasedHangar.GetSnapShotStorageSize(_003CsnapshotDirPath_003E5__4);
					result2 = new AircraftSnapshotInfo(aircraftIdentifier, _003CsnapshotId_003E5__3, _003CtakenAt_003E5__2, _003CprototypeListData_003E5__6.ToListData(), _003Cmetadata_003E5__7, snapShotStorageSize);
				}
				catch
				{
					directoryBasedHangar._fs.DeleteIfExists(_003CsnapshotDirPath_003E5__4);
					throw;
				}
				finally
				{
					if (num < 0)
					{
						directoryBasedHangar._fs.DeleteIfExistsAndParentsThatAreEmpty(_003CtemporaryArchivePath_003E5__5, directoryBasedHangar.RootDirectory);
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CsnapshotId_003E5__3 = null;
				_003CsnapshotDirPath_003E5__4 = null;
				_003CtemporaryArchivePath_003E5__5 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CsnapshotId_003E5__3 = null;
			_003CsnapshotDirPath_003E5__4 = null;
			_003CtemporaryArchivePath_003E5__5 = null;
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
	private struct _003CTakeSnapShotWithLock_003Ed__122 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AircraftSnapshotInfo> _003C_003Et__builder;

		public AircraftRepoInfo repoInfo;

		public string repoAction;

		public AircraftIdentifier aircraftIdentifier;

		public AircraftSnapshotMetaData metadata;

		public DirectoryBasedHangar _003C_003E4__this;

		private DateTimeOffset _003CtakenAt_003E5__2;

		private SnapshotIdentifier _003CsnapshotId_003E5__3;

		private string _003CsnapshotDir_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			AircraftSnapshotInfo result;
			try
			{
				string currentAircraftRoot = default(string);
				if (num != 0)
				{
					_003CtakenAt_003E5__2 = DateTimeOffset.Now;
					_003CsnapshotId_003E5__3 = new SnapshotIdentifier(_003CtakenAt_003E5__2);
					currentAircraftRoot = repoInfo.CurrentAircraftRoot;
					_003CsnapshotDir_003E5__4 = Path.Combine(repoInfo.HistoryAircraftRoot, _003CsnapshotId_003E5__3.Serialized);
					LogRepoMessage(LogLevel.Info, repoAction, $"Taking a snapshot of {aircraftIdentifier}. Description: {metadata.Description}");
				}
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						string text = Path.Combine(_003CsnapshotDir_003E5__4, "snapshot.gca");
						directoryBasedHangar._fs.CreateDirectory(GetParentDirectoryPath(text));
						directoryBasedHangar._zipCompression.ZipDirectory(currentAircraftRoot, text);
						directoryBasedHangar._fs.SetFileTimes(text, new FileTimes(_003CtakenAt_003E5__2));
						string path = Path.Combine(_003CsnapshotDir_003E5__4, "metadata.json");
						ConfiguredTaskAwaitable val = directoryBasedHangar._persistence.Commit(PersistedSnapshotMetadata.FromRuntime(metadata), path).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CTakeSnapShotWithLock_003Ed__122>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					long snapShotStorageSize = directoryBasedHangar.GetSnapShotStorageSize(_003CsnapshotDir_003E5__4);
					result = new AircraftSnapshotInfo(aircraftIdentifier, _003CsnapshotId_003E5__3, _003CtakenAt_003E5__2, null, metadata, snapShotStorageSize);
				}
				catch
				{
					directoryBasedHangar._fs.DeleteIfExists(_003CsnapshotDir_003E5__4);
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CsnapshotId_003E5__3 = null;
				_003CsnapshotDir_003E5__4 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CsnapshotId_003E5__3 = null;
			_003CsnapshotDir_003E5__4 = null;
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
	private struct _003CTemporarilyUnzipAircraftGca_003Ed__106<T> : IAsyncStateMachine where T : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<T> _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public string gcaPath;

		public string repoAction;

		public Func<string, AircraftIdentifier, System.Threading.Tasks.Task<T>> useDecompressedGca;

		private string _003CtempAircraftPath_003E5__2;

		private string _003CidentString_003E5__3;

		private string _003CaircraftRootDirPath_003E5__4;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<T> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			T result;
			try
			{
				if ((uint)num > 1u)
				{
					directoryBasedHangar.ThrowIfGcaDoesNotExistOrIsNotAFile(gcaPath, repoAction);
					string text = directoryBasedHangar._deployment.PerProcessTempDirectory();
					string text2 = repoAction.Replace(" ", string.Empty, (StringComparison)4);
					Guid val = Guid.NewGuid();
					_003CtempAircraftPath_003E5__2 = Path.Combine(text, text2, ((object)(Guid)(ref val)).ToString());
					directoryBasedHangar._fs.CreateDirectory(_003CtempAircraftPath_003E5__2);
				}
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
							goto IL_021b;
						}
						directoryBasedHangar._zipCompression.UnZip(gcaPath, _003CtempAircraftPath_003E5__2);
						List<FileSystemEntry> val2 = Enumerable.ToList<FileSystemEntry>(directoryBasedHangar._fs.EntriesIn(_003CtempAircraftPath_003E5__2));
						if (val2.Count != 1)
						{
							throw new FormatException("Aircraft GCA's should only have the root aircraft directory in the root of the archive.");
						}
						if (val2[0].EntryType != FileSystemEntryType.Directory)
						{
							throw new FormatException("Aircraft GCA's only root entry should be the aircraft directory with the identifier as the name.");
						}
						_003CidentString_003E5__3 = val2[0].Name;
						if (!AircraftIdentifier.IsValidIdentifier(_003CidentString_003E5__3))
						{
							throw new FormatException("The Aircraft GCA aircraft directory name is not a valid identifier.");
						}
						_003CaircraftRootDirPath_003E5__4 = val2[0].FullPath;
						AircraftRepoInfo repoInfo = CreateRepoInfo(_003CaircraftRootDirPath_003E5__4);
						ConfiguredTaskAwaitable val3 = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(repoInfo).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val3)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CTemporarilyUnzipAircraftGca_003Ed__106<T>>(ref awaiter2, ref this);
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
					awaiter = ((Func<string, AircraftIdentifier, System.Threading.Tasks.Task<string>>)(object)useDecompressedGca).Invoke(_003CaircraftRootDirPath_003E5__4, new AircraftIdentifier(_003CidentString_003E5__3)).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<T>, _003CTemporarilyUnzipAircraftGca_003Ed__106<T>>(ref awaiter, ref this);
						return;
					}
					goto IL_021b;
					IL_021b:
					result = awaiter.GetResult();
				}
				finally
				{
					if (num < 0)
					{
						directoryBasedHangar._fs.DeleteIfExistsAndParentsThatAreEmpty(_003CtempAircraftPath_003E5__2, directoryBasedHangar._deployment.PerProcessTempDirectory());
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CtempAircraftPath_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CtempAircraftPath_003E5__2 = null;
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
	private struct _003CUpdateAircraft_003Ed__49 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftSnapshotData aircraftSnapshot;

		private _003C_003Ec__DisplayClass49_0 _003C_003E8__1;

		private System.IDisposable _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_0139;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass49_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.aircraftSnapshot = aircraftSnapshot;
					_003C_003E8__1.aircraftIdentifier = _003C_003E8__1.aircraftSnapshot.AircraftIdentifier;
					ThrowIfAircraftIdentifierIsInvalid(_003C_003E8__1.aircraftIdentifier);
					_003C_003E8__1.repoInfo = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003C_003E8__1.aircraftIdentifier);
					_003C_003E8__1.previousPanel = null;
					_003C_003E8__1.familyDataChanges = Enumerable.Empty<DataChangeNotification<AircraftPackageFamily>>();
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003C_003E8__1.repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CUpdateAircraft_003Ed__49>(ref awaiter, ref this);
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
				goto IL_0139;
				IL_0139:
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					ConfiguredTaskAwaitable val;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_0272;
						}
						val = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(_003C_003E8__1.repoInfo).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateAircraft_003Ed__49>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					val = TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__0_003Ed))] () =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__0_003Ed _003C_003CUpdateAircraft_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__0_003Ed);
						_003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003E4__this = _003C_003E8__1;
						_003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__0_003Ed>(ref _003C_003CUpdateAircraft_003Eb__0_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__0_003Ed._003C_003Et__builder)).Task;
					}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__1_003Ed))] () =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__1_003Ed _003C_003CUpdateAircraft_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__1_003Ed);
						_003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003E4__this = _003C_003E8__1;
						_003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__1_003Ed>(ref _003C_003CUpdateAircraft_003Eb__1_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__1_003Ed._003C_003Et__builder)).Task;
					}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__2_003Ed))] () =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__2_003Ed _003C_003CUpdateAircraft_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__2_003Ed);
						_003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003E4__this = _003C_003E8__1;
						_003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__2_003Ed>(ref _003C_003CUpdateAircraft_003Eb__2_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__2_003Ed._003C_003Et__builder)).Task;
					}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__3_003Ed))] () =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__3_003Ed _003C_003CUpdateAircraft_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__3_003Ed);
						_003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
						_003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003E4__this = _003C_003E8__1;
						_003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003E1__state = -1;
						((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass49_0._003C_003CUpdateAircraft_003Eb__3_003Ed>(ref _003C_003CUpdateAircraft_003Eb__3_003Ed);
						return ((AsyncTaskMethodBuilder)(ref _003C_003CUpdateAircraft_003Eb__3_003Ed._003C_003Et__builder)).Task;
					}).DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__2 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateAircraft_003Ed__49>(ref awaiter2, ref this);
						return;
					}
					goto IL_0272;
					IL_0272:
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					LogRepoMessage(LogLevel.Info, "Update aircraft", "{0} - Updated aircraft.", _003C_003E8__1.aircraftIdentifier);
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap1 != null)
					{
						_003C_003E7__wrap1.Dispose();
					}
				}
				_003C_003E7__wrap1 = null;
				if (_003C_003E8__1.previousPanel == null)
				{
					throw ThrowInternalRepoError();
				}
				directoryBasedHangar._hangarEvents.PostFamilyDataChanges(_003C_003E8__1.familyDataChanges);
				directoryBasedHangar.PostSimulatorChanges(_003C_003E8__1.previousPanel.SimulatorChanges(_003C_003E8__1.aircraftSnapshot.Panel).ToAircraftKeysChanged(_003C_003E8__1.aircraftIdentifier));
				directoryBasedHangar.PostAircraftChange(_003C_003E8__1.aircraftIdentifier, DataChangeType.Updated);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
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
	private struct _003CUpdateAircraftHeavyData_003Ed__51 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftHeavyData heavyData;

		public DirectoryBasedHangar _003C_003E4__this;

		private AircraftIdentifier _003CaircraftIdentifier_003E5__2;

		private AircraftRepoInfo _003CrepoInfo_003E5__3;

		private System.IDisposable _003C_003E7__wrap3;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_00d2;
					}
					_003CaircraftIdentifier_003E5__2 = heavyData.AircraftIdentifier;
					ThrowIfAircraftIdentifierIsInvalid(_003CaircraftIdentifier_003E5__2);
					_003CrepoInfo_003E5__3 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003CaircraftIdentifier_003E5__2);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__3.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CUpdateAircraftHeavyData_003Ed__51>(ref awaiter, ref this);
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
				_003C_003E7__wrap3 = result;
				goto IL_00d2;
				IL_00d2:
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					ConfiguredTaskAwaitable val;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_01cb;
						}
						val = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(_003CrepoInfo_003E5__3).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateAircraftHeavyData_003Ed__51>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					val = directoryBasedHangar.UpdateHeavyDataWithLock("Update Aircraft Heavy Data", _003CaircraftIdentifier_003E5__2, _003CrepoInfo_003E5__3, heavyData).DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__2 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateAircraftHeavyData_003Ed__51>(ref awaiter2, ref this);
						return;
					}
					goto IL_01cb;
					IL_01cb:
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap3 != null)
					{
						_003C_003E7__wrap3.Dispose();
					}
				}
				_003C_003E7__wrap3 = null;
				directoryBasedHangar.PostAircraftChange(_003CaircraftIdentifier_003E5__2, DataChangeType.Updated);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CaircraftIdentifier_003E5__2 = default(AircraftIdentifier);
				_003CrepoInfo_003E5__3 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CaircraftIdentifier_003E5__2 = default(AircraftIdentifier);
			_003CrepoInfo_003E5__3 = null;
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
	private struct _003CUpdateAircraftListData_003Ed__50 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftListData updatedListData;

		public DirectoryBasedHangar _003C_003E4__this;

		private AircraftIdentifier _003CaircraftIdentifier_003E5__2;

		private AircraftRepoInfo _003CrepoInfo_003E5__3;

		private System.IDisposable _003C_003E7__wrap3;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_00d2;
					}
					_003CaircraftIdentifier_003E5__2 = updatedListData.AircraftIdentifier;
					ThrowIfAircraftIdentifierIsInvalid(_003CaircraftIdentifier_003E5__2);
					_003CrepoInfo_003E5__3 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003CaircraftIdentifier_003E5__2);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__3.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CUpdateAircraftListData_003Ed__50>(ref awaiter, ref this);
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
				_003C_003E7__wrap3 = result;
				goto IL_00d2;
				IL_00d2:
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					ConfiguredTaskAwaitable val;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_01cb;
						}
						val = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(_003CrepoInfo_003E5__3).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateAircraftListData_003Ed__50>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					val = directoryBasedHangar.UpdateListDataWithLock("Update Aircraft List Data", _003CaircraftIdentifier_003E5__2, _003CrepoInfo_003E5__3, updatedListData).DetachContext();
					awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__2 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateAircraftListData_003Ed__50>(ref awaiter2, ref this);
						return;
					}
					goto IL_01cb;
					IL_01cb:
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap3 != null)
					{
						_003C_003E7__wrap3.Dispose();
					}
				}
				_003C_003E7__wrap3 = null;
				directoryBasedHangar.PostAircraftChange(_003CaircraftIdentifier_003E5__2, DataChangeType.Updated);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CaircraftIdentifier_003E5__2 = default(AircraftIdentifier);
				_003CrepoInfo_003E5__3 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CaircraftIdentifier_003E5__2 = default(AircraftIdentifier);
			_003CrepoInfo_003E5__3 = null;
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
	private struct _003CUpdateAircraftWinfsArchives_003Ed__57 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public Dictionary<SimInstallKey, string> simWinfsArchiveInputMap;

		private _003C_003Ec__DisplayClass57_0 _003C_003E8__1;

		private _003C_003Ec__DisplayClass57_1 _003C_003E8__2;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_0175;
				}
				if (num == 1)
				{
					goto IL_0187;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass57_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__1.aircraftIdentifier = aircraftIdentifier;
				ThrowIfAircraftIdentifierIsInvalid(_003C_003E8__1.aircraftIdentifier);
				if (simWinfsArchiveInputMap == null)
				{
					throw new ArgumentNullException("simWinfsArchiveInputMap");
				}
				if (simWinfsArchiveInputMap.Count != 0)
				{
					string text = Enumerable.FirstOrDefault<string>((System.Collections.Generic.IEnumerable<string>)simWinfsArchiveInputMap.Values, (Func<string, bool>)((string f) => !_003C_003E8__1._003C_003E4__this._fs.PathExists(f)));
					if (text != null)
					{
						throw new FileNotFoundException("There is a missing compressed winfs mount file.", text);
					}
					string text2 = Enumerable.FirstOrDefault<string>((System.Collections.Generic.IEnumerable<string>)simWinfsArchiveInputMap.Values, (Func<string, bool>)((string f) => !_003C_003E8__1._003C_003E4__this.IsValidWinfsArchive(f, "UPDATE_WINFS_ARCHIVE")));
					if (text2 != null)
					{
						throw new FormatException("Invalid winfs mount archive: " + text2);
					}
					_003CrepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(_003C_003E8__1.aircraftIdentifier);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CUpdateAircraftWinfsArchives_003Ed__57>(ref awaiter, ref this);
						return;
					}
					goto IL_0175;
				}
				goto end_IL_000e;
				IL_0187:
				try
				{
					ConfiguredTaskAwaiter<SimPanel> awaiter2;
					if (num != 1)
					{
						_003C_003E8__2 = new _003C_003Ec__DisplayClass57_1();
						LogRepoMessage(LogLevel.Info, "UPDATE_WINFS_ARCHIVE", _003C_003E8__1.aircraftIdentifier.IdentStringChecked);
						string rootPath = _003CrepoInfo_003E5__2.RootPath;
						if (!directoryBasedHangar._fs.PathExists(rootPath))
						{
							throw new InvalidOperationException("The '" + (string?)_003C_003E8__1.aircraftIdentifier + "' aircraft was not found. Store it before updating WinFs files.");
						}
						awaiter2 = directoryBasedHangar.GetPersistedPanelWithLock(_003C_003E8__1.aircraftIdentifier, _003CrepoInfo_003E5__2).ConfigureAwait(false).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CUpdateAircraftWinfsArchives_003Ed__57>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPanel>);
						num = (_003C_003E1__state = -1);
					}
					SimPanel result = awaiter2.GetResult();
					_003C_003E8__2.panel = result;
					List<SimInstallKey> val = Enumerable.ToList<SimInstallKey>(Enumerable.Where<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)simWinfsArchiveInputMap.Keys, (Func<SimInstallKey, bool>)((SimInstallKey sk) => !_003C_003E8__2.panel.AllSimulators.ContainsKey(sk))));
					if (val.Count > 0)
					{
						string text3 = string.Join<SimInstallKey>(", ", (System.Collections.Generic.IEnumerable<SimInstallKey>)val);
						throw new SimulatorNotFoundException("There is no simulator data to track the WINFS archive for [" + text3 + "]");
					}
					string currentAircraftRoot = _003CrepoInfo_003E5__2.CurrentAircraftRoot;
					Enumerator<SimInstallKey, string> enumerator = simWinfsArchiveInputMap.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<SimInstallKey, string> current = enumerator.Current;
							string text4 = Path.Combine(currentAircraftRoot, "avionics", (string)current.Key.ToStandardRelativePath(), "winfs_mount.gca");
							LogRepoMessage(LogLevel.Info, "UPDATE_WINFS_ARCHIVE", "Updating '{0}' -> '{1}'", current.Value, text4);
							directoryBasedHangar._fs.CopyAndOverwriteFile(current.Value, text4);
						}
					}
					finally
					{
						if (num < 0)
						{
							((System.IDisposable)enumerator).Dispose();
						}
					}
					_003C_003E8__2 = null;
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				directoryBasedHangar.PostSimulatorChanges(Enumerable.Select<SimInstallKey, SimAircraftKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)simWinfsArchiveInputMap.Keys, (Func<SimInstallKey, SimAircraftKey>)((SimInstallKey sim) => new SimAircraftKey(_003C_003E8__1.aircraftIdentifier, sim))), DataChangeType.Updated);
				directoryBasedHangar.PostAircraftChange(_003C_003E8__1.aircraftIdentifier, DataChangeType.Updated);
				goto end_IL_000e;
				IL_0175:
				System.IDisposable result2 = awaiter.GetResult();
				_003C_003E7__wrap2 = result2;
				goto IL_0187;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CrepoInfo_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CrepoInfo_003E5__2 = null;
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
	private struct _003CUpdateFamilyDataForRelease_003Ed__78 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftPackageFamily aircraftPackageFamily;

		public DirectoryBasedHangar _003C_003E4__this;

		public FamilyData data;

		private _003C_003Ec__DisplayClass78_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<DataChangeType> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<DataChangeType> awaiter;
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass78_0();
					_003C_003E8__1.aircraftPackageFamily = aircraftPackageFamily;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.data = data;
					awaiter = directoryBasedHangar.WriteAircraftHarness<DataChangeType>(_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass78_0._003C_003CUpdateFamilyDataForRelease_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
						string packageFamilyDataFilePath = GetFamilyDataFilePath(familyDataDirPath, _003C_003E8__1.aircraftPackageFamily.ReleaseId);
						_003C_003E8__1._003C_003E4__this._fs.CreateDirectory(familyDataDirPath);
						DataChangeType change;
						if (_003C_003E8__1._003C_003E4__this._fs.PathExists(packageFamilyDataFilePath))
						{
							try
							{
								change = (new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)(await _003C_003E8__1._003C_003E4__this._persistence.Load<PersistedFamilyData>(packageFamilyDataFilePath).DetachContext()).Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value))).Equals(_003C_003E8__1.data) ? DataChangeType.None : DataChangeType.Updated);
							}
							catch (SerializationException)
							{
								change = DataChangeType.Updated;
							}
						}
						else
						{
							change = DataChangeType.Created;
						}
						if (change != DataChangeType.None)
						{
							PersistedFamilyData persistenceModel = CreatePersistedFamilyData(_003C_003E8__1.data);
							try
							{
								await _003C_003E8__1._003C_003E4__this._persistence.Commit(persistenceModel, packageFamilyDataFilePath).DetachContext();
							}
							catch (System.Exception e)
							{
								LogRepoError(e, "UPDATE_FAMILY_DATA", $"[{_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier} - {_003C_003E8__1.aircraftPackageFamily.ReleaseId}] Updating family data.");
								throw;
							}
							LogRepoMessage(LogLevel.Info, "UPDATE_FAMILY_DATA", $"[{_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier} - {_003C_003E8__1.aircraftPackageFamily.ReleaseId}] Updated family data.");
						}
						else
						{
							LogRepoMessage(LogLevel.Info, "UPDATE_FAMILY_DATA", $"[{_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier} - {_003C_003E8__1.aircraftPackageFamily.ReleaseId}] Family data already up to date.");
						}
						return change;
					}).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<DataChangeType>, _003CUpdateFamilyDataForRelease_003Ed__78>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<DataChangeType>);
					num = (_003C_003E1__state = -1);
				}
				DataChangeType result = awaiter.GetResult();
				if (result != DataChangeType.None)
				{
					directoryBasedHangar._hangarEvents.PostFamilyDataChange(DataChangeNotification.Create(_003C_003E8__1.aircraftPackageFamily, result));
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
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
	private struct _003CUpdateHeavyDataWithLock_003Ed__102 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftHeavyData heavyData;

		public string repoAction;

		public AircraftIdentifier aircraftIdentifier;

		public AircraftRepoInfo repoInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					PersistedAircraftHeavyData persistenceModel = PersistedAircraftHeavyData.FromRuntime(heavyData);
					LogRepoMessage(LogLevel.Info, repoAction, "{0} - Saving aircraft heavy data.", aircraftIdentifier);
					string path = Path.Combine(repoInfo.CurrentAircraftRoot, "heavy.json");
					ConfiguredTaskAwaitable val = directoryBasedHangar._persistence.Commit(persistenceModel, path).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateHeavyDataWithLock_003Ed__102>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				LogRepoMessage(LogLevel.Info, repoAction, "{0} - Updated aircraft heavy data.", aircraftIdentifier);
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CUpdateListDataWithLock_003Ed__104 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftListData updatedListData;

		public string repoAction;

		public AircraftIdentifier aircraftIdentifier;

		public AircraftRepoInfo repoInfo;

		public DirectoryBasedHangar _003C_003E4__this;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					PersistedAircraftListData persistenceModel = PersistedAircraftListData.FromRuntime(updatedListData);
					LogRepoMessage(LogLevel.Info, repoAction, "{0} - Saving aircraft list data.", aircraftIdentifier);
					string path = Path.Combine(repoInfo.CurrentAircraftRoot, "list.json");
					ConfiguredTaskAwaitable val = directoryBasedHangar._persistence.Commit(persistenceModel, path).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateListDataWithLock_003Ed__104>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				LogRepoMessage(LogLevel.Info, repoAction, "{0} - Updated aircraft list data.", aircraftIdentifier);
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CUpdateSimulatorPanel_003Ed__52 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public SimPanel simulatorPanel;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_00c4;
					}
					ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
					_003CrepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CUpdateSimulatorPanel_003Ed__52>(ref awaiter, ref this);
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
				goto IL_00c4;
				IL_00c4:
				SimPanel result2;
				try
				{
					ConfiguredTaskAwaiter<SimPanel> awaiter2;
					ConfiguredTaskAwaiter awaiter3;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<SimPanel>);
							num = (_003C_003E1__state = -1);
							goto IL_01bd;
						}
						ConfiguredTaskAwaitable val = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(_003CrepoInfo_003E5__2).DetachContext();
						awaiter3 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter3;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CUpdateSimulatorPanel_003Ed__52>(ref awaiter3, ref this);
							return;
						}
					}
					else
					{
						awaiter3 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
					awaiter2 = directoryBasedHangar.GetPreviousPanelAndUpdateWithNewOneWithLock("Update aircraft", aircraftIdentifier, _003CrepoInfo_003E5__2, simulatorPanel).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CUpdateSimulatorPanel_003Ed__52>(ref awaiter2, ref this);
						return;
					}
					goto IL_01bd;
					IL_01bd:
					result2 = awaiter2.GetResult();
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				if (result2 == null)
				{
					throw ThrowInternalRepoError();
				}
				directoryBasedHangar.PostSimulatorChanges(result2.SimulatorChanges(simulatorPanel).ToAircraftKeysChanged(aircraftIdentifier));
				directoryBasedHangar.PostAircraftChange(aircraftIdentifier, DataChangeType.Updated);
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CrepoInfo_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrepoInfo_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	[CompilerGenerated]
	private sealed class _003CValidatePersistedAircraftListData_003Ed__96 : System.Collections.Generic.IEnumerable<System.Exception>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<System.Exception>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private System.Exception _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private PersistedAircraftListData persistedAircraft;

		public PersistedAircraftListData _003C_003E3__persistedAircraft;

		private string aircraftJsonPath;

		public string _003C_003E3__aircraftJsonPath;

		private string directoryName;

		public string _003C_003E3__directoryName;

		System.Exception System.Collections.Generic.IEnumerator<System.Exception>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CValidatePersistedAircraftListData_003Ed__96(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Expected O, but got Unknown
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Expected O, but got Unknown
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				if (!TrainerIdentifier.IsValidIdentifier(persistedAircraft.AircraftIdentifier))
				{
					string text = "The 'AircraftIdentifier' property in '" + aircraftJsonPath + "' is not formatted correctly.";
					if (SoftConfigurations.IsDebug)
					{
						text += " (^[A-Za-z0-9_]+$)";
					}
					_003C_003E2__current = (System.Exception)new FormatException(text);
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_0082;
			case 1:
				_003C_003E1__state = -1;
				goto IL_0082;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00ef;
			case 3:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0082:
				if ((string?)persistedAircraft.AircraftIdentifier != directoryName)
				{
					_003C_003E2__current = (System.Exception)new FormatException(string.Concat(new string[5] { "The 'AircraftIdentifier' property in '", aircraftJsonPath, "' must match the directory name: '", directoryName, "'." }));
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_00ef;
				IL_00ef:
				if (string.IsNullOrEmpty(persistedAircraft.DisplayName))
				{
					_003C_003E2__current = (System.Exception)new FormatException("The 'DisplayName' property in '" + aircraftJsonPath + "' cannot be null or empty");
					_003C_003E1__state = 3;
					return true;
				}
				break;
			}
			return false;
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<System.Exception> System.Collections.Generic.IEnumerable<System.Exception>.GetEnumerator()
		{
			_003CValidatePersistedAircraftListData_003Ed__96 _003CValidatePersistedAircraftListData_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CValidatePersistedAircraftListData_003Ed__ = this;
			}
			else
			{
				_003CValidatePersistedAircraftListData_003Ed__ = new _003CValidatePersistedAircraftListData_003Ed__96(0);
			}
			_003CValidatePersistedAircraftListData_003Ed__.persistedAircraft = _003C_003E3__persistedAircraft;
			_003CValidatePersistedAircraftListData_003Ed__.directoryName = _003C_003E3__directoryName;
			_003CValidatePersistedAircraftListData_003Ed__.aircraftJsonPath = _003C_003E3__aircraftJsonPath;
			return _003CValidatePersistedAircraftListData_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<System.Exception>)this).GetEnumerator();
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CWriteAircraftHarness_003Ed__53<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public AircraftIdentifier aircraftIdentifier;

		public DirectoryBasedHangar _003C_003E4__this;

		public Func<AircraftRepoInfo, System.Threading.Tasks.Task<TResult>> writeAircraft;

		private AircraftRepoInfo _003CrepoInfo_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			TResult result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_00c4;
					}
					ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
					_003CrepoInfo_003E5__2 = directoryBasedHangar.CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
					awaiter = directoryBasedHangar.GetLockForPathUnlocked(_003CrepoInfo_003E5__2.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CWriteAircraftHarness_003Ed__53<TResult>>(ref awaiter, ref this);
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
				goto IL_00c4;
				IL_00c4:
				try
				{
					ConfiguredTaskAwaiter<TResult> awaiter2;
					ConfiguredTaskAwaiter awaiter3;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<TResult>);
							num = (_003C_003E1__state = -1);
							goto IL_01ba;
						}
						directoryBasedHangar.ThrowNotFoundIfAircraftDirectoryIsMissing(_003CrepoInfo_003E5__2);
						ConfiguredTaskAwaitable val = directoryBasedHangar.PerformAircraftFormatMigrationWithLock(_003CrepoInfo_003E5__2).DetachContext();
						awaiter3 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CWriteAircraftHarness_003Ed__53<TResult>>(ref awaiter3, ref this);
							return;
						}
					}
					else
					{
						awaiter3 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
					awaiter2 = ((Func<AircraftRepoInfo, System.Threading.Tasks.Task<AircraftRepoInfo>>)(object)writeAircraft).Invoke(_003CrepoInfo_003E5__2).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CWriteAircraftHarness_003Ed__53<TResult>>(ref awaiter2, ref this);
						return;
					}
					goto IL_01ba;
					IL_01ba:
					result2 = awaiter2.GetResult();
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CrepoInfo_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrepoInfo_003E5__2 = null;
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
	private struct _003CWriteAircraftHarness_003Ed__54 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public Func<AircraftRepoInfo, System.Threading.Tasks.Task> writeAircraft;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		private ConfiguredTaskAwaiter<int> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<int> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass54_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass54_0
					{
						writeAircraft = writeAircraft
					};
					awaiter = directoryBasedHangar.WriteAircraftHarness<int>(aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass54_0._003C_003CWriteAircraftHarness_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						await CS_0024_003C_003E8__locals0.writeAircraft.Invoke(repoInfo).DetachContext();
						return 0;
					}).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<int>, _003CWriteAircraftHarness_003Ed__54>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<int>);
					num = (_003C_003E1__state = -1);
				}
				awaiter.GetResult();
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

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CWriteAircraftHarnessWithRollbackHandling_003Ed__44<TResult> : IAsyncStateMachine where TResult : notnull
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TResult> _003C_003Et__builder;

		public CancellationToken token;

		public DirectoryBasedHangar _003C_003E4__this;

		public AircraftIdentifier aircraftIdentifier;

		public Func<AircraftRepoInfo, AircraftRollbackException?, System.Threading.Tasks.Task<TResult>> writeAircraft;

		private ConfiguredTaskAwaiter<TResult> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			DirectoryBasedHangar directoryBasedHangar = _003C_003E4__this;
			TResult result;
			try
			{
				ConfiguredTaskAwaiter<TResult> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass44_0<TResult> CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass44_0<TResult>
					{
						token = token
					};
					awaiter = directoryBasedHangar.AircraftOpHarnessWithRollbackHandling<TResult>((AsyncReaderWriterLock l) => l.WriterLockAsync(CS_0024_003C_003E8__locals0.token), aircraftIdentifier, writeAircraft, CS_0024_003C_003E8__locals0.token).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TResult>, _003CWriteAircraftHarnessWithRollbackHandling_003Ed__44<TResult>>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TResult>);
					num = (_003C_003E1__state = -1);
				}
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

	private const string SELECTED_AIRCRAFT_FILE_NAME = "selected.json";

	private const string SIMULATOR_JSON_FILE_NAME = "simulator.json";

	private const string CREATE_NEW_AIRCRAFT_REPO_ACTION = "Create new aircraft";

	private const int CURRENT_HANGAR_FORMAT_LEVEL = 1;

	private const string HANGAR_FORMAT_FILE_PATH = "hangarFormat.dat";

	private const string SNAPSHOT_ARCHIVE_FILE_NAME = "snapshot.gca";

	private const string PROTOTYPE_ARCHIVE_FILE_NAME = "prototype.gca";

	private readonly ILowLevelDal _lowLevel;

	private readonly IDeploymentPathProvider _deployment;

	private readonly IFileSystem _fs;

	private readonly IZipCompression _zipCompression;

	private readonly IPersistArbitraryObjects _persistence;

	private readonly HangarEvents _hangarEvents = new HangarEvents();

	private readonly AircraftBasedLocking _aircraftLocks = new AircraftBasedLocking();

	private readonly Nito.AsyncEx.AsyncLock _selectedLock = new Nito.AsyncEx.AsyncLock();

	private AircraftIdentifier _cachedSelectedAircraftId;

	private bool _disposedValue;

	[Obsolete("This file is deprecated and will be migrated on launcher startup.")]
	private const string AIRCRAFT_JSON_FILE_NAME = "aircraft.json";

	public IObservable<AircraftIdentifier> WhenAircraftIsUpdated => AircraftChanges.Select<DataChangeNotification<AircraftIdentifier>, AircraftIdentifier>((Func<DataChangeNotification<AircraftIdentifier>, AircraftIdentifier>)((DataChangeNotification<AircraftIdentifier> cn) => cn.Value));

	[field: CompilerGenerated]
	public string RootDirectory
	{
		[CompilerGenerated]
		get;
	}

	public IObservable<DataChangeNotification<AircraftIdentifier>> AircraftChanges => _hangarEvents.WhenAircraftChanges;

	public IObservable<AircraftIdentifier> WhenSelectedAircraftChanges => _hangarEvents.WhenSelectedAircraftChanges;

	public IObservable<DataChangeNotification<AircraftPackageFamily>> WhenAircraftFamilyDataChanges => _hangarEvents.WhenAircraftFamilyDataChanges;

	public DirectoryBasedHangar(ILowLevelDal lowLevelDal, string rootDirectory)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(rootDirectory) || !PathHelpers.IsValidPath(rootDirectory))
		{
			throw new ArgumentException("rootDirectory is invalid.");
		}
		_lowLevel = lowLevelDal ?? throw new ArgumentNullException("lowLevelDal");
		RootDirectory = rootDirectory;
		_deployment = lowLevelDal.Deployment;
		_fs = lowLevelDal.FileSystem;
		_zipCompression = lowLevelDal.Compression;
		_persistence = lowLevelDal.ObjectPersistence;
		_fs.CreateDirectory(RootDirectory);
	}

	[AsyncStateMachine(typeof(_003CAreMigrationsNeeded_003Ed__30))]
	public async System.Threading.Tasks.Task<bool> AreMigrationsNeeded()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string rootDirectory = RootDirectory;
		if (_fs.PathExists(rootDirectory))
		{
			List<System.Threading.Tasks.Task<bool>> checkTasks = Enumerable.ToList<System.Threading.Tasks.Task<bool>>(Enumerable.Select<FileSystemEntry, System.Threading.Tasks.Task<bool>>(_fs.DirectoriesIn(rootDirectory), (Func<FileSystemEntry, System.Threading.Tasks.Task<bool>>)([AsyncStateMachine(typeof(_003C_003CAreMigrationsNeeded_003Eb__30_0_003Ed))] [CompilerGenerated] async (FileSystemEntry aircraftDirectory) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				if (!AircraftIdentifier.IsValidIdentifier(aircraftDirectory.Name))
				{
					return false;
				}
				AircraftRepoInfo repoInfo = CreateRepoInfo(aircraftDirectory.FullPath);
				using (await GetLockForPathUnlocked(repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
				{
					if (!_fs.PathExists(repoInfo.RootPath))
					{
						return false;
					}
					return NeedsAMigration(repoInfo.RootPath);
				}
			})));
			while (checkTasks.Count > 0)
			{
				System.Threading.Tasks.Task<bool> first = await System.Threading.Tasks.Task.WhenAny<bool>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<bool>>)checkTasks).DetachContext();
				if (await first.DetachContext())
				{
					return true;
				}
				checkTasks.Remove(first);
			}
		}
		return false;
	}

	[AsyncStateMachine(typeof(_003CStartMigrations_003Ed__31))]
	public System.Threading.Tasks.Task StartMigrations(CancellationToken token = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003CStartMigrations_003Ed__31 _003CStartMigrations_003Ed__ = default(_003CStartMigrations_003Ed__31);
		_003CStartMigrations_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CStartMigrations_003Ed__._003C_003E4__this = this;
		_003CStartMigrations_003Ed__.token = token;
		_003CStartMigrations_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CStartMigrations_003Ed__._003C_003Et__builder)).Start<_003CStartMigrations_003Ed__31>(ref _003CStartMigrations_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CStartMigrations_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CGetLastSelectedAircraft_003Ed__32))]
	public async System.Threading.Tasks.Task<AircraftIdentifier> GetLastSelectedAircraft()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AircraftIdentifier cachedSelectedId = _cachedSelectedAircraftId;
		string selectedAircraftFilePath = BuildSelectedAircraftFilepath();
		AircraftIdentifier result = AircraftIdentifier.Default;
		if (_fs.PathExists(selectedAircraftFilePath))
		{
			PersistedSelectedAircraft persistedSelectedAircraft = await GetCurrentSelectedAircraftFromFile().DetachContext();
			if (AircraftIdentifier.IsValidIdentifier(persistedSelectedAircraft.SelectedAircraft))
			{
				result = new AircraftIdentifier(persistedSelectedAircraft.SelectedAircraft);
				AircraftRepoInfo repoInfo = CreateRepoInfoFromHangarControlledIdentifier(result);
				using (await GetLockForPathUnlocked(repoInfo.RootPath).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
				{
					if (!_fs.PathExists(repoInfo.RootPath))
					{
						LogRepoError((System.Exception)new DirectoryNotFoundException(repoInfo.RootPath), "GET SELECTED AIRCRAFT");
						result = AircraftIdentifier.Default;
					}
				}
			}
		}
		if (result == AircraftIdentifier.Default)
		{
			LogRepoMessage(LogLevel.Info, "GET SELECTED AIRCRAFT", "Could not determine last selected aircraft.");
		}
		if (cachedSelectedId != result)
		{
			PostSelectedAircaftChange(result);
		}
		return result;
		[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_0._003C_003CGetLastSelectedAircraft_003Eg__GetCurrentSelectedAircraftFromFile_007C0_003Ed))]
		async System.Threading.Tasks.Task<PersistedSelectedAircraft> GetCurrentSelectedAircraftFromFile()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			using (await _selectedLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				return await _persistence.Load<PersistedSelectedAircraft>(selectedAircraftFilePath).DetachContext();
			}
		}
	}

	[AsyncStateMachine(typeof(_003CStoreLastSelectedAircraft_003Ed__33))]
	public System.Threading.Tasks.Task StoreLastSelectedAircraft(AircraftIdentifier aircraftIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CStoreLastSelectedAircraft_003Ed__33 _003CStoreLastSelectedAircraft_003Ed__ = default(_003CStoreLastSelectedAircraft_003Ed__33);
		_003CStoreLastSelectedAircraft_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CStoreLastSelectedAircraft_003Ed__._003C_003E4__this = this;
		_003CStoreLastSelectedAircraft_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CStoreLastSelectedAircraft_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CStoreLastSelectedAircraft_003Ed__._003C_003Et__builder)).Start<_003CStoreLastSelectedAircraft_003Ed__33>(ref _003CStoreLastSelectedAircraft_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CStoreLastSelectedAircraft_003Ed__._003C_003Et__builder)).Task;
	}

	public IObservable<AircraftIdentifier> GetAllAircraftIds()
	{
		return SafeDirectoryStream(_fs, RootDirectory).Select<FileSystemEntry, AircraftIdentifier>((Func<FileSystemEntry, AircraftIdentifier>)((FileSystemEntry fse) => AircraftIdentifier.FromString(fse.Name))).Where<AircraftIdentifier>((Func<AircraftIdentifier, bool>)((AircraftIdentifier ai) => ai.IsValid));
	}

	public IObservable<AircraftListData> GetAllAircraftListData()
	{
		return Observable.Create<AircraftListData>((Func<IObserver<AircraftListData>, CancellationToken, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed))] [CompilerGenerated] (IObserver<AircraftListData> o, CancellationToken token) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed _003C_003CGetAllAircraftListData_003Eb__35_0_003Ed = default(_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed);
			_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed._003C_003E4__this = this;
			_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed.o = o;
			_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed.token = token;
			_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CGetAllAircraftListData_003Eb__35_0_003Ed._003C_003Et__builder)).Start<_003C_003CGetAllAircraftListData_003Eb__35_0_003Ed>(ref _003C_003CGetAllAircraftListData_003Eb__35_0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CGetAllAircraftListData_003Eb__35_0_003Ed._003C_003Et__builder)).Task;
		}));
	}

	[AsyncStateMachine(typeof(_003CGetAircraftListDataById_003Ed__36))]
	public async System.Threading.Tasks.Task<AircraftListData> GetAircraftListDataById(AircraftIdentifier aircraftIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await ReadAircraftHarnessWithRollbackHandling<AircraftListData>(aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003CGetAircraftListDataById_003Eb__36_0_003Ed))] [CompilerGenerated] async (AircraftRepoInfo repoInfo, AircraftRollbackException? re) => await GetListDataWithLock(repoInfo).DetachContext()).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CGetAircraftHeavyDataById_003Ed__37))]
	public async System.Threading.Tasks.Task<AircraftHeavyData> GetAircraftHeavyDataById(AircraftIdentifier aircraftIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await ReadAircraftHarness<AircraftHeavyData>(aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass37_0._003C_003CGetAircraftHeavyDataById_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) => await GetHeavyDataWithLock(aircraftIdentifier, repoInfo).DetachContext()).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CGetAircraftSnapshotById_003Ed__38))]
	public async System.Threading.Tasks.Task<AircraftSnapshotData> GetAircraftSnapshotById(AircraftIdentifier aircraftIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await ReadAircraftHarness<AircraftSnapshotData>(aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass38_0._003C_003CGetAircraftSnapshotById_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) => await GetAircraftSnapshotWithLock(aircraftIdentifier, repoInfo).DetachContext()).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CGetSimulatorPanel_003Ed__39))]
	public async System.Threading.Tasks.Task<SimPanel> GetSimulatorPanel(AircraftIdentifier aircraftIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await ReadAircraftHarness<SimPanel>(aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass39_0._003C_003CGetSimulatorPanel_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) => await GetPersistedPanelWithLock(aircraftIdentifier, repoInfo).DetachContext()).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CGetSimulator_003Ed__40))]
	public async System.Threading.Tasks.Task<SimInstallData> GetSimulator(AircraftIdentifier aircraftIdentifier, SimInstallKey simulatorKey)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SimPanel obj = await GetSimulatorPanel(aircraftIdentifier).DetachContext();
		if (!obj.AllSimulators.ContainsKey(simulatorKey))
		{
			throw new SimulatorNotFoundException("The simulator was not found in the aircraft. " + (object)simulatorKey);
		}
		return obj.AllSimulators[simulatorKey];
	}

	[AsyncStateMachine(typeof(_003CReadAircraftHarness_003Ed__41<>))]
	private async System.Threading.Tasks.Task<TResult> ReadAircraftHarness<TResult>(AircraftIdentifier aircraftIdentifier, Func<AircraftRepoInfo, System.Threading.Tasks.Task<TResult>> readAircraft, CancellationToken token = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return await AircraftOpHarness<TResult>((AsyncReaderWriterLock l) => l.ReaderLockAsync(token), aircraftIdentifier, readAircraft, token).ConfigureAwait(false);
	}

	[AsyncStateMachine(typeof(_003CAircraftOpHarness_003Ed__42<>))]
	private async System.Threading.Tasks.Task<TResult> AircraftOpHarness<TResult>(LockSelector lockSelector, AircraftIdentifier aircraftIdentifier, Func<AircraftRepoInfo, System.Threading.Tasks.Task<TResult>> readAircraft, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
		AircraftRepoInfo repoInfo = CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
		await CheckForAndPerformMigration(repoInfo, token).DetachContext();
		AsyncReaderWriterLock lockForPathUnlocked = GetLockForPathUnlocked(repoInfo.RootPath);
		using (await lockSelector(lockForPathUnlocked).ConfigureAwait(continueOnCapturedContext: false))
		{
			ThrowNotFoundIfAircraftDirectoryIsMissing(repoInfo);
			return await readAircraft.Invoke(repoInfo).DetachContext();
		}
	}

	[AsyncStateMachine(typeof(_003CReadAircraftHarnessWithRollbackHandling_003Ed__43<>))]
	private async System.Threading.Tasks.Task<TResult> ReadAircraftHarnessWithRollbackHandling<TResult>(AircraftIdentifier aircraftIdentifier, Func<AircraftRepoInfo, AircraftRollbackException?, System.Threading.Tasks.Task<TResult>> readAircraft, CancellationToken token = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return await AircraftOpHarnessWithRollbackHandling<TResult>((AsyncReaderWriterLock l) => l.ReaderLockAsync(token), aircraftIdentifier, readAircraft, token).ConfigureAwait(false);
	}

	[AsyncStateMachine(typeof(_003CWriteAircraftHarnessWithRollbackHandling_003Ed__44<>))]
	private async System.Threading.Tasks.Task<TResult> WriteAircraftHarnessWithRollbackHandling<TResult>(AircraftIdentifier aircraftIdentifier, Func<AircraftRepoInfo, AircraftRollbackException?, System.Threading.Tasks.Task<TResult>> writeAircraft, CancellationToken token = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return await AircraftOpHarnessWithRollbackHandling<TResult>((AsyncReaderWriterLock l) => l.WriterLockAsync(token), aircraftIdentifier, writeAircraft, token).ConfigureAwait(false);
	}

	[AsyncStateMachine(typeof(_003CAircraftOpHarnessWithRollbackHandling_003Ed__45<>))]
	private async System.Threading.Tasks.Task<TResult> AircraftOpHarnessWithRollbackHandling<TResult>(Func<AsyncReaderWriterLock, AwaitableDisposable<System.IDisposable>> lockStategy, AircraftIdentifier aircraftIdentifier, Func<AircraftRepoInfo, AircraftRollbackException?, System.Threading.Tasks.Task<TResult>> readAircraft, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
		AircraftRepoInfo repoInfo = CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
		AircraftRollbackException rollbackError = null;
		Guid? errorGuid = null;
		try
		{
			await CheckForAndPerformMigration(repoInfo, token).DetachContext();
		}
		catch (AircraftRollbackException ex)
		{
			errorGuid = Guid.NewGuid();
			Logger.Log(LogLevel.Warning, $"[{errorGuid}] Migration rollback detected. Continuing to try and read anyways.");
			rollbackError = ex;
		}
		AsyncReaderWriterLock lockForPathUnlocked = GetLockForPathUnlocked(repoInfo.RootPath);
		using (await lockStategy.Invoke(lockForPathUnlocked).ConfigureAwait(continueOnCapturedContext: false))
		{
			ThrowNotFoundIfAircraftDirectoryIsMissing(repoInfo);
			try
			{
				return await readAircraft.Invoke(repoInfo, rollbackError).DetachContext();
			}
			catch (System.Exception ex2) when (rollbackError != null)
			{
				AircraftRollbackException ex3 = new AircraftRollbackException("An error occurred and could be due to the migration error.", (System.Exception)new AggregateException(new System.Exception[2] { rollbackError, ex2 }));
				Logger.LogFormattedException(ex3, $"[{errorGuid}] Read op");
				throw ex3;
			}
		}
	}

	[AsyncStateMachine(typeof(_003CCreateNewAircraft_003Ed__46))]
	public async System.Threading.Tasks.Task<AircraftSnapshotData> CreateNewAircraft(AircraftListData startingInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass46_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass46_0();
		CS_0024_003C_003E8__locals0._003C_003E4__this = this;
		CS_0024_003C_003E8__locals0.startingInfo = startingInfo;
		CS_0024_003C_003E8__locals0.startingInfo = await HangarUtil.PrepareStartingInfoWithUniqueId(CS_0024_003C_003E8__locals0.startingInfo, AircraftIdentifier.GenerateIdentifiers(CS_0024_003C_003E8__locals0.startingInfo.AirframeCCT, "R"), ReserveAircraftDirectoryIfNotTaken).DetachContext();
		CS_0024_003C_003E8__locals0.aircraftIdentifier = CS_0024_003C_003E8__locals0.startingInfo.AircraftIdentifier;
		CS_0024_003C_003E8__locals0.repoInfo = CreateRepoInfoFromHangarControlledIdentifier(CS_0024_003C_003E8__locals0.aircraftIdentifier);
		string aircraftDirectoryPath = CS_0024_003C_003E8__locals0.repoInfo.RootPath;
		AircraftSnapshotData aircraftSnapshotData = null;
		using (await GetLockForPathUnlocked(CS_0024_003C_003E8__locals0.repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			bool alreadyExisted = _fs.PathExists(aircraftDirectoryPath);
			try
			{
				if (!alreadyExisted)
				{
					LogRepoMessage(LogLevel.Info, "Create new aircraft", "Creating aircraft directory '{0}'.", aircraftDirectoryPath);
					_fs.CreateDirectory(aircraftDirectoryPath);
				}
				else
				{
					ThrowIfAircraftDirectoryIsNotEmpty(CS_0024_003C_003E8__locals0.aircraftIdentifier, aircraftDirectoryPath);
					LogRepoMessage(LogLevel.Info, "Create new aircraft", "Using reserved aircraft directory '{0}'.", aircraftDirectoryPath);
				}
				await TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__0_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__0_003Ed _003C_003CCreateNewAircraft_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__0_003Ed);
					_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__0_003Ed>(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__0_003Ed._003C_003Et__builder)).Task;
				}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__1_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__1_003Ed _003C_003CCreateNewAircraft_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__1_003Ed);
					_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass46_0._003C_003CCreateNewAircraft_003Eb__1_003Ed>(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CCreateNewAircraft_003Eb__1_003Ed._003C_003Et__builder)).Task;
				}).DetachContext();
				aircraftSnapshotData = AircraftSnapshotData.FromListData(CS_0024_003C_003E8__locals0.startingInfo);
			}
			catch (System.Exception)
			{
				if (alreadyExisted)
				{
					DeleteAircraftDirectory("Create new aircraft", aircraftDirectoryPath);
				}
				throw;
			}
		}
		if (aircraftSnapshotData == null)
		{
			throw ThrowInternalRepoError();
		}
		PostAircraftChange(CS_0024_003C_003E8__locals0.aircraftIdentifier, DataChangeType.Created);
		return aircraftSnapshotData;
	}

	[AsyncStateMachine(typeof(_003CCopyAircraft_003Ed__47))]
	public async System.Threading.Tasks.Task<AircraftSnapshotData> CopyAircraft(AircraftListData startingInfo, AircraftIdentifier copyIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		copyIdentifier.ThrowIfDefault();
		AircraftIdentifier aircraftIdentifier = startingInfo.AircraftIdentifier;
		if (aircraftIdentifier == AircraftIdentifier.Default || aircraftIdentifier == copyIdentifier)
		{
			aircraftIdentifier = await ReserveFirstUnusedIdFrom(AircraftIdentifier.GenerateIdentifiers(startingInfo.AirframeCCT, "C")).DetachContext();
			startingInfo = startingInfo.WithNewIdentifier(aircraftIdentifier);
		}
		AircraftRepoInfo repoInfo = CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
		string aircraftDirectoryPath = repoInfo.RootPath;
		copyIdentifier.ThrowIfDefault((System.Exception?)new ArgumentException("invalid copy aircraft identifier."));
		AircraftRepoInfo protoInfo = CreateRepoInfoFromHangarControlledIdentifier(copyIdentifier);
		string prototypeDirectory = protoInfo.RootPath;
		AsyncReaderWriterLock pathToCopyLock = GetLockForPathUnlocked(prototypeDirectory);
		AsyncReaderWriterLock lockForPathUnlocked = GetLockForPathUnlocked(repoInfo.RootPath);
		if (pathToCopyLock == lockForPathUnlocked)
		{
			ThrowInternalRepoError();
		}
		AircraftSnapshotData aircraftSnapshotData = null;
		using (await lockForPathUnlocked.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			ThrowIfAircraftDirectoryIsNotEmpty(aircraftIdentifier, aircraftDirectoryPath);
			bool alreadyExisted = _fs.PathExists(aircraftDirectoryPath);
			try
			{
				try
				{
					await CheckForAndPerformMigration(protoInfo).DetachContext();
				}
				catch (System.Exception e)
				{
					LogRepoError(e, "Copy aircraft", "Could not migrate the prototype directory '{0}'.", prototypeDirectory);
					throw;
				}
				using (await pathToCopyLock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
				{
					if (!_fs.PathExists(prototypeDirectory))
					{
						LogRepoMessage(LogLevel.Error, "Copy aircraft", "Missing prototype directory '{0}'.", prototypeDirectory);
						throw new DirectoryNotFoundException("Missing aircraft prototype directory: " + prototypeDirectory);
					}
					LogRepoMessage(LogLevel.Info, "Copy aircraft", (string?)aircraftIdentifier + " - Copying existing aircraft.");
					try
					{
						_fs.CopyDirectory(prototypeDirectory, aircraftDirectoryPath);
						LogRepoMessage(LogLevel.Info, "Copy aircraft", (string?)aircraftIdentifier + " - Copied existing aircraft.");
					}
					catch (System.Exception e2)
					{
						LogRepoError(e2, "Copy aircraft", "Could not copy prototype directory '{0}'.", prototypeDirectory);
						throw;
					}
				}
				await TakePrototypeSnapShotWithLock("Copy aircraft", aircraftIdentifier, repoInfo).DetachContext();
				LogRepoMessage(LogLevel.Error, "Create new aircraft", (string?)aircraftIdentifier + " - Saving to disk.");
				try
				{
					await UpdateListDataWithLock("Copy aircraft", aircraftIdentifier, repoInfo, startingInfo).DetachContext();
				}
				catch (System.Exception e3)
				{
					LogRepoError(e3, "Create new aircraft", "Could not serialize new aircraft JSON record '{0}'.", aircraftIdentifier);
					throw;
				}
				aircraftSnapshotData = await GetAircraftSnapshotWithLock(aircraftIdentifier, repoInfo).DetachContext();
			}
			catch (System.Exception e4)
			{
				LogRepoError(e4, "Copy aircraft");
				if (alreadyExisted)
				{
					DeleteAircraftDirectory("Create new aircraft", aircraftDirectoryPath);
				}
				throw;
			}
		}
		if (aircraftSnapshotData == null)
		{
			throw new InvalidOperationException("Internal repo error. Did not set created snapshot.");
		}
		PostAircraftChange(aircraftIdentifier, DataChangeType.Created);
		return aircraftSnapshotData;
	}

	[AsyncStateMachine(typeof(_003CCreateOrUpdateSimulator_003Ed__48))]
	public System.Threading.Tasks.Task CreateOrUpdateSimulator(AircraftIdentifier aircraftIdentifier, SimInstallData simulator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CCreateOrUpdateSimulator_003Ed__48 _003CCreateOrUpdateSimulator_003Ed__ = default(_003CCreateOrUpdateSimulator_003Ed__48);
		_003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCreateOrUpdateSimulator_003Ed__._003C_003E4__this = this;
		_003CCreateOrUpdateSimulator_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CCreateOrUpdateSimulator_003Ed__.simulator = simulator;
		_003CCreateOrUpdateSimulator_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder)).Start<_003CCreateOrUpdateSimulator_003Ed__48>(ref _003CCreateOrUpdateSimulator_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCreateOrUpdateSimulator_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateAircraft_003Ed__49))]
	public System.Threading.Tasks.Task UpdateAircraft(AircraftSnapshotData aircraftSnapshot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateAircraft_003Ed__49 _003CUpdateAircraft_003Ed__ = default(_003CUpdateAircraft_003Ed__49);
		_003CUpdateAircraft_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateAircraft_003Ed__._003C_003E4__this = this;
		_003CUpdateAircraft_003Ed__.aircraftSnapshot = aircraftSnapshot;
		_003CUpdateAircraft_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateAircraft_003Ed__._003C_003Et__builder)).Start<_003CUpdateAircraft_003Ed__49>(ref _003CUpdateAircraft_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateAircraft_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateAircraftListData_003Ed__50))]
	public System.Threading.Tasks.Task UpdateAircraftListData(AircraftListData updatedListData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateAircraftListData_003Ed__50 _003CUpdateAircraftListData_003Ed__ = default(_003CUpdateAircraftListData_003Ed__50);
		_003CUpdateAircraftListData_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateAircraftListData_003Ed__._003C_003E4__this = this;
		_003CUpdateAircraftListData_003Ed__.updatedListData = updatedListData;
		_003CUpdateAircraftListData_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateAircraftListData_003Ed__._003C_003Et__builder)).Start<_003CUpdateAircraftListData_003Ed__50>(ref _003CUpdateAircraftListData_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateAircraftListData_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateAircraftHeavyData_003Ed__51))]
	public System.Threading.Tasks.Task UpdateAircraftHeavyData(AircraftHeavyData heavyData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateAircraftHeavyData_003Ed__51 _003CUpdateAircraftHeavyData_003Ed__ = default(_003CUpdateAircraftHeavyData_003Ed__51);
		_003CUpdateAircraftHeavyData_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateAircraftHeavyData_003Ed__._003C_003E4__this = this;
		_003CUpdateAircraftHeavyData_003Ed__.heavyData = heavyData;
		_003CUpdateAircraftHeavyData_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateAircraftHeavyData_003Ed__._003C_003Et__builder)).Start<_003CUpdateAircraftHeavyData_003Ed__51>(ref _003CUpdateAircraftHeavyData_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateAircraftHeavyData_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateSimulatorPanel_003Ed__52))]
	public System.Threading.Tasks.Task UpdateSimulatorPanel(AircraftIdentifier aircraftIdentifier, SimPanel simulatorPanel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateSimulatorPanel_003Ed__52 _003CUpdateSimulatorPanel_003Ed__ = default(_003CUpdateSimulatorPanel_003Ed__52);
		_003CUpdateSimulatorPanel_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateSimulatorPanel_003Ed__._003C_003E4__this = this;
		_003CUpdateSimulatorPanel_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CUpdateSimulatorPanel_003Ed__.simulatorPanel = simulatorPanel;
		_003CUpdateSimulatorPanel_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateSimulatorPanel_003Ed__._003C_003Et__builder)).Start<_003CUpdateSimulatorPanel_003Ed__52>(ref _003CUpdateSimulatorPanel_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateSimulatorPanel_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CWriteAircraftHarness_003Ed__53<>))]
	private async System.Threading.Tasks.Task<TResult> WriteAircraftHarness<TResult>(AircraftIdentifier aircraftIdentifier, Func<AircraftRepoInfo, System.Threading.Tasks.Task<TResult>> writeAircraft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
		AircraftRepoInfo repoInfo = CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
		using (await GetLockForPathUnlocked(repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			ThrowNotFoundIfAircraftDirectoryIsMissing(repoInfo);
			await PerformAircraftFormatMigrationWithLock(repoInfo).DetachContext();
			return await writeAircraft.Invoke(repoInfo).DetachContext();
		}
	}

	[AsyncStateMachine(typeof(_003CWriteAircraftHarness_003Ed__54))]
	private System.Threading.Tasks.Task WriteAircraftHarness(AircraftIdentifier aircraftIdentifier, Func<AircraftRepoInfo, System.Threading.Tasks.Task> writeAircraft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CWriteAircraftHarness_003Ed__54 _003CWriteAircraftHarness_003Ed__ = default(_003CWriteAircraftHarness_003Ed__54);
		_003CWriteAircraftHarness_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CWriteAircraftHarness_003Ed__._003C_003E4__this = this;
		_003CWriteAircraftHarness_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CWriteAircraftHarness_003Ed__.writeAircraft = writeAircraft;
		_003CWriteAircraftHarness_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CWriteAircraftHarness_003Ed__._003C_003Et__builder)).Start<_003CWriteAircraftHarness_003Ed__54>(ref _003CWriteAircraftHarness_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CWriteAircraftHarness_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CDeleteAircraft_003Ed__55))]
	public System.Threading.Tasks.Task DeleteAircraft(AircraftIdentifier aircraftIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CDeleteAircraft_003Ed__55 _003CDeleteAircraft_003Ed__ = default(_003CDeleteAircraft_003Ed__55);
		_003CDeleteAircraft_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CDeleteAircraft_003Ed__._003C_003E4__this = this;
		_003CDeleteAircraft_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CDeleteAircraft_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CDeleteAircraft_003Ed__._003C_003Et__builder)).Start<_003CDeleteAircraft_003Ed__55>(ref _003CDeleteAircraft_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CDeleteAircraft_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CGetAircraftWinfsArchives_003Ed__56))]
	public async System.Threading.Tasks.Task<System.Collections.Generic.IList<SimInstallKey>> GetAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, Dictionary<SimInstallKey, string> simWinfsArchiveOutputMap)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
		if (simWinfsArchiveOutputMap == null)
		{
			throw new ArgumentNullException("simWinfsArchiveOutputMap");
		}
		List<SimInstallKey> found = new List<SimInstallKey>();
		if (simWinfsArchiveOutputMap.Count > 0)
		{
			AircraftRepoInfo repoInfo = CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
			using (await GetLockForPathUnlocked(repoInfo.RootPath).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				LogRepoMessage(LogLevel.Info, "GET_WINFS_ARCHIVE", aircraftIdentifier.IdentStringChecked);
				string rootPath = repoInfo.RootPath;
				if (_fs.PathExists(rootPath))
				{
					SimPanel panel = await GetPersistedPanelWithLock(aircraftIdentifier, repoInfo).DetachContext();
					string currentAircraftRoot = repoInfo.CurrentAircraftRoot;
					System.Collections.Generic.IEnumerator<KeyValuePair<SimInstallKey, string>> enumerator = Enumerable.Where<KeyValuePair<SimInstallKey, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, string>>)simWinfsArchiveOutputMap, (Func<KeyValuePair<SimInstallKey, string>, bool>)((KeyValuePair<SimInstallKey, string> kv) => panel.AllSimulators.ContainsKey(kv.Key))).GetEnumerator();
					try
					{
						while (((System.Collections.IEnumerator)enumerator).MoveNext())
						{
							KeyValuePair<SimInstallKey, string> current = enumerator.Current;
							string text = Path.Combine(currentAircraftRoot, "avionics", (string)current.Key.ToStandardRelativePath(), "winfs_mount.gca");
							if (_fs.PathExists(text))
							{
								LogRepoMessage(LogLevel.Info, "GET_WINFS_ARCHIVE", "Copying '{0}' -> '{1}'", text, current.Value);
								_fs.CopyAndOverwriteFile(text, current.Value);
								found.Add(current.Key);
							}
						}
					}
					finally
					{
						((System.IDisposable)enumerator)?.Dispose();
					}
				}
			}
		}
		return (System.Collections.Generic.IList<SimInstallKey>)found;
	}

	[AsyncStateMachine(typeof(_003CUpdateAircraftWinfsArchives_003Ed__57))]
	public System.Threading.Tasks.Task UpdateAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, Dictionary<SimInstallKey, string> simWinfsArchiveInputMap)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateAircraftWinfsArchives_003Ed__57 _003CUpdateAircraftWinfsArchives_003Ed__ = default(_003CUpdateAircraftWinfsArchives_003Ed__57);
		_003CUpdateAircraftWinfsArchives_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateAircraftWinfsArchives_003Ed__._003C_003E4__this = this;
		_003CUpdateAircraftWinfsArchives_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CUpdateAircraftWinfsArchives_003Ed__.simWinfsArchiveInputMap = simWinfsArchiveInputMap;
		_003CUpdateAircraftWinfsArchives_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateAircraftWinfsArchives_003Ed__._003C_003Et__builder)).Start<_003CUpdateAircraftWinfsArchives_003Ed__57>(ref _003CUpdateAircraftWinfsArchives_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateAircraftWinfsArchives_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CDeleteAircraftWinfsArchives_003Ed__58))]
	public System.Threading.Tasks.Task DeleteAircraftWinfsArchives(AircraftIdentifier aircraftIdentifier, System.Collections.Generic.IEnumerable<SimInstallKey> simsToClear)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CDeleteAircraftWinfsArchives_003Ed__58 _003CDeleteAircraftWinfsArchives_003Ed__ = default(_003CDeleteAircraftWinfsArchives_003Ed__58);
		_003CDeleteAircraftWinfsArchives_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CDeleteAircraftWinfsArchives_003Ed__._003C_003E4__this = this;
		_003CDeleteAircraftWinfsArchives_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CDeleteAircraftWinfsArchives_003Ed__.simsToClear = simsToClear;
		_003CDeleteAircraftWinfsArchives_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CDeleteAircraftWinfsArchives_003Ed__._003C_003Et__builder)).Start<_003CDeleteAircraftWinfsArchives_003Ed__58>(ref _003CDeleteAircraftWinfsArchives_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CDeleteAircraftWinfsArchives_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CExportAircraft_003Ed__59))]
	public System.Threading.Tasks.Task ExportAircraft(AircraftIdentifier aircraftIdentifier, string gcaPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CExportAircraft_003Ed__59 _003CExportAircraft_003Ed__ = default(_003CExportAircraft_003Ed__59);
		_003CExportAircraft_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CExportAircraft_003Ed__._003C_003E4__this = this;
		_003CExportAircraft_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CExportAircraft_003Ed__.gcaPath = gcaPath;
		_003CExportAircraft_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CExportAircraft_003Ed__._003C_003Et__builder)).Start<_003CExportAircraft_003Ed__59>(ref _003CExportAircraft_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CExportAircraft_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CParseAircraftGCA_003Ed__60))]
	public async System.Threading.Tasks.Task<AircraftSnapshotData> ParseAircraftGCA(string gcaPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await TemporarilyUnzipAircraftGca<AircraftSnapshotData>(gcaPath, "Parse GCA", [AsyncStateMachine(typeof(_003C_003CParseAircraftGCA_003Eb__60_0_003Ed))] [CompilerGenerated] async (string tempAircraftRootPath, AircraftIdentifier exportId) => await GetAircraftSnapshotWithLock(exportId, CreateRepoInfo(tempAircraftRootPath)).DetachContext()).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CImportAircaft_003Ed__61))]
	public async System.Threading.Tasks.Task<AircraftSnapshotData> ImportAircaft(AircraftListData newListData, string gcaPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AircraftListData newListData2 = newListData;
		string gcaPath2 = gcaPath;
		return await TemporarilyUnzipAircraftGca<AircraftSnapshotData>(gcaPath2, "Import Aircraft", [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass61_0._003C_003CImportAircaft_003Eb__0_003Ed))] async (string tempAircraftRootPath, AircraftIdentifier exportId) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			newListData2 = await HangarUtil.PrepareStartingInfoWithUniqueId(newListData2, AircraftIdentifier.GenerateIdentifiers(newListData2.AirframeCCT, "R"), ReserveAircraftDirectoryIfNotTaken).DetachContext();
			AircraftIdentifier aircraftIdentifier = newListData2.AircraftIdentifier;
			AircraftRepoInfo repoInfo = CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
			string aircraftDirectoryPath = repoInfo.RootPath;
			AircraftSnapshotData result;
			using (await GetLockForPathUnlocked(repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				ThrowIfAircraftDirectoryIsNotEmpty(aircraftIdentifier, aircraftDirectoryPath);
				try
				{
					AircraftRepoInfo tempRepoInfo = CreateRepoInfo(tempAircraftRootPath);
					AircraftListData persistedListData = await GetListDataWithLock(tempRepoInfo).DetachContext();
					if (aircraftIdentifier != exportId)
					{
						LogRepoMessage(LogLevel.Info, "Import Aircraft", "Changing identifiers ({0} -> {1}) for imported GCA: {2}", exportId, aircraftIdentifier, PathHelpers.QuotePathIfNeeded(gcaPath2));
						persistedListData = persistedListData.WithNewIdentifier(aircraftIdentifier);
						await UpdateListDataWithLock("Import Aircraft", aircraftIdentifier, tempRepoInfo, persistedListData).DetachContext();
						string text = Path.Combine(GetParentDirectoryPath(tempAircraftRootPath), aircraftIdentifier.IdentStringChecked);
						_fs.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, tempAircraftRootPath, text);
						tempAircraftRootPath = text;
						tempRepoInfo = CreateRepoInfo(tempAircraftRootPath);
					}
					if (persistedListData.DisplayName != newListData2.DisplayName)
					{
						persistedListData = persistedListData.Rename(newListData2.DisplayName);
						LogRepoMessage(LogLevel.Info, "Import Aircraft", "Changing display name ({0} -> {1}) for imported GCA: {2}", persistedListData.DisplayName, newListData2.DisplayName, PathHelpers.QuotePathIfNeeded(gcaPath2));
						await UpdateListDataWithLock("Import Aircraft", aircraftIdentifier, tempRepoInfo, persistedListData).DetachContext();
					}
					LogRepoMessage(LogLevel.Info, "Import Aircraft", "Moving temporary import directory into the hangar: {0} -> {1}", PathHelpers.QuotePathIfNeeded(tempAircraftRootPath), PathHelpers.QuotePathIfNeeded(aircraftDirectoryPath));
					_fs.MoveOrCopyAndDeleteToDifferentDriveAndOverwrite(LolaFsExtensions.DefaultTempFactory, tempAircraftRootPath, aircraftDirectoryPath);
					result = await GetAircraftSnapshotWithLock(aircraftIdentifier, repoInfo).DetachContext();
				}
				catch
				{
					_fs.DeleteIfExists(aircraftDirectoryPath);
					throw;
				}
			}
			PostAircraftChange(aircraftIdentifier, DataChangeType.Created);
			return result;
		}).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CCreateSnapshot_003Ed__62))]
	public async System.Threading.Tasks.Task<AircraftSnapshotInfo> CreateSnapshot(AircraftIdentifier aircraftIdentifier, AircraftSnapshotMetaData metadata)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AircraftSnapshotMetaData metadata2 = metadata;
		ThrowIfAircraftIdentifierIsInvalid(aircraftIdentifier);
		if (metadata2 == null)
		{
			throw new ArgumentNullException("metadata");
		}
		AircraftRepoInfo repoInfo = CreateRepoInfoFromHangarControlledIdentifier(aircraftIdentifier);
		AircraftSnapshotInfo info;
		using (await GetLockForPathUnlocked(repoInfo.RootPath).WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			ThrowNotFoundIfAircraftDirectoryIsMissing(repoInfo);
			if (NeedsAMigration(repoInfo.RootPath))
			{
				info = await AttemptToMigrateToSaveTimeOnDecompress().ConfigureAwait(false);
			}
			else
			{
				info = await TakeSnapShotWithLock("CREATE SNAPSHOT", aircraftIdentifier, repoInfo, metadata2).DetachContext();
			}
		}
		_hangarEvents.PostSnapshotChange(DataChangeNotification.Create<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>(new ValueTuple<AircraftIdentifier, SnapshotIdentifier>(aircraftIdentifier, info.SnapshotIdentifier), DataChangeType.Created));
		return info;
		[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass62_0._003C_003CCreateSnapshot_003Eg__AttemptToMigrateToSaveTimeOnDecompress_007C0_003Ed))]
		async System.Threading.Tasks.Task<AircraftSnapshotInfo> AttemptToMigrateToSaveTimeOnDecompress()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			info = await TakeSnapShotWithLock("CREATE SNAPSHOT", aircraftIdentifier, repoInfo, metadata2).DetachContext();
			try
			{
				await PerformAircraftFormatMigrationWithLock(repoInfo).DetachContext();
			}
			catch (AircraftRollbackException)
			{
				return info;
			}
			string path = Path.Combine(repoInfo.HistoryAircraftRoot, info.SnapshotIdentifier.Serialized);
			_fs.DeleteIfExists(path);
			return await TakeSnapShotWithLock("CREATE SNAPSHOT", aircraftIdentifier, repoInfo, metadata2).DetachContext();
		}
	}

	public IObservable<DataChangeNotification<SnapshotIdentifier>> WhenSnapshotChanges(AircraftIdentifier aircraftIdentifier)
	{
		return _hangarEvents.WhenSnapshotChanges.Where<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>>((Func<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>, bool>)((DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>> c) => c.Value.Item1 == aircraftIdentifier)).Select<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>, DataChangeNotification<SnapshotIdentifier>>((Func<DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>>, DataChangeNotification<SnapshotIdentifier>>)((DataChangeNotification<ValueTuple<AircraftIdentifier, SnapshotIdentifier>> c) => DataChangeNotification.Create(c.Value.Item2, c.Change)));
	}

	public IObservable<AircraftSnapshotInfo> ListHistoricalSnapshots(AircraftIdentifier aircraftIdentifier)
	{
		_003C_003Ec__DisplayClass64_0 _003C_003Ec__DisplayClass64_ = new _003C_003Ec__DisplayClass64_0();
		_003C_003Ec__DisplayClass64_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass64_.aircraftIdentifier = aircraftIdentifier;
		_003C_003Ec__DisplayClass64_.aircraftIdentifier.ThrowIfDefault();
		return Observable.Create<AircraftSnapshotInfo>((Func<IObserver<AircraftSnapshotInfo>, CancellationToken, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass64_0._003C_003CListHistoricalSnapshots_003Eb__0_003Ed))] (IObserver<AircraftSnapshotInfo> o, CancellationToken token) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass64_0._003C_003CListHistoricalSnapshots_003Eb__0_003Ed _003C_003CListHistoricalSnapshots_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass64_0._003C_003CListHistoricalSnapshots_003Eb__0_003Ed);
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003E4__this = _003C_003Ec__DisplayClass64_;
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed.o = o;
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed.token = token;
			_003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass64_0._003C_003CListHistoricalSnapshots_003Eb__0_003Ed>(ref _003C_003CListHistoricalSnapshots_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CListHistoricalSnapshots_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}));
	}

	[AsyncStateMachine(typeof(_003CDeleteSnapshot_003Ed__65))]
	public System.Threading.Tasks.Task DeleteSnapshot(AircraftIdentifier aircraftIdentifier, SnapshotIdentifier snapshotIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CDeleteSnapshot_003Ed__65 _003CDeleteSnapshot_003Ed__ = default(_003CDeleteSnapshot_003Ed__65);
		_003CDeleteSnapshot_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CDeleteSnapshot_003Ed__._003C_003E4__this = this;
		_003CDeleteSnapshot_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CDeleteSnapshot_003Ed__.snapshotIdentifier = snapshotIdentifier;
		_003CDeleteSnapshot_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CDeleteSnapshot_003Ed__._003C_003Et__builder)).Start<_003CDeleteSnapshot_003Ed__65>(ref _003CDeleteSnapshot_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CDeleteSnapshot_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CRevertAircraftToSnapshot_003Ed__66))]
	public System.Threading.Tasks.Task RevertAircraftToSnapshot(AircraftIdentifier aircraftIdentifier, SnapshotIdentifier snapshotIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CRevertAircraftToSnapshot_003Ed__66 _003CRevertAircraftToSnapshot_003Ed__ = default(_003CRevertAircraftToSnapshot_003Ed__66);
		_003CRevertAircraftToSnapshot_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CRevertAircraftToSnapshot_003Ed__._003C_003E4__this = this;
		_003CRevertAircraftToSnapshot_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CRevertAircraftToSnapshot_003Ed__.snapshotIdentifier = snapshotIdentifier;
		_003CRevertAircraftToSnapshot_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CRevertAircraftToSnapshot_003Ed__._003C_003Et__builder)).Start<_003CRevertAircraftToSnapshot_003Ed__66>(ref _003CRevertAircraftToSnapshot_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CRevertAircraftToSnapshot_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CMountHistoricalSnapshot_003Ed__67))]
	public async System.Threading.Tasks.Task<TemporaryHangarDirectory> MountHistoricalSnapshot(AircraftIdentifier aircraftIdentifier, SnapshotIdentifier snapshotIdentifier)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SnapshotIdentifier snapshotIdentifier2 = snapshotIdentifier;
		return await ReadAircraftHarnessWithRollbackHandling<TemporaryHangarDirectory>(aircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass67_0._003C_003CMountHistoricalSnapshot_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo, AircraftRollbackException? e) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string text = Path.Combine(repoInfo.HistoryAircraftRoot, snapshotIdentifier2.Serialized);
			if (!_fs.PathExists(text))
			{
				throw new AircraftSnapshotNotFoundException("The snapshot does not exist.");
			}
			string snapShotArchivePathOrThrow = GetSnapShotArchivePathOrThrow(text);
			TemporaryHangarDirectory tempHangar = TemporaryHangarDirectory.CreateNewUniqueHangar(_lowLevel);
			try
			{
				await ExtractSnapshotIntoTemporaryHangar("MOUNT_SNAP", tempHangar, aircraftIdentifier, snapShotArchivePathOrThrow).ConfigureAwait(false);
				return tempHangar;
			}
			catch
			{
				tempHangar.Dispose();
				throw;
			}
		}).DetachContext();
	}

	public System.Threading.Tasks.Task Flush()
	{
		return System.Threading.Tasks.Task.CompletedTask;
	}

	private static System.Exception CreateMissingSnapShotArchiveError()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		return (System.Exception)new FileNotFoundException("Neither snapshot archive exists.");
	}

	private static bool IsProtoTypeArchiveFileName(string snapshotArchivePath)
	{
		return Path.GetFileName(snapshotArchivePath) == "prototype.gca";
	}

	[AsyncStateMachine(typeof(_003CExtractSnapshotIntoTemporaryHangar_003Ed__71))]
	private async System.Threading.Tasks.Task<AircraftRepoInfo> ExtractSnapshotIntoTemporaryHangar(string repoAction, TemporaryHangarDirectory tempHangar, AircraftIdentifier aircraftIdentifier, string snapshotArchivePath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string mountPath = tempHangar.RootDirectory;
		_fs.CreateDirectory(mountPath);
		string aircraftDirPath = Path.Combine(mountPath, aircraftIdentifier.IdentStringChecked);
		string text = Path.Combine(aircraftDirPath, "current");
		string text2 = (IsProtoTypeArchiveFileName(snapshotArchivePath) ? aircraftDirPath : text);
		_fs.CreateDirectory(text2);
		Logger.Log(LogLevel.Info, "Mounting {0} snapshot {1} at {2}", aircraftIdentifier, snapshotArchivePath, text2);
		_zipCompression.UnZip(snapshotArchivePath, text2);
		AircraftRepoInfo tempRepoInfo = CreateRepoInfo(aircraftDirPath);
		await PerformAircraftFormatMigrationWithLock(tempRepoInfo).ConfigureAwait(false);
		AircraftListData aircraftListData = await GetListDataWithLock(tempRepoInfo).DetachContext();
		if (aircraftListData.AircraftIdentifier != aircraftIdentifier)
		{
			if (IsProtoTypeArchiveFileName(snapshotArchivePath))
			{
				Logger.Log(LogLevel.Info, "The Snapshot is from a prototype aircraft. So we have to redo the directory to match the old identifier.");
				string text3 = Path.Combine(mountPath, aircraftListData.AircraftIdentifier.IdentStringChecked);
				_fs.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, aircraftDirPath, text3);
				tempRepoInfo = CreateRepoInfo(text3);
			}
			else
			{
				Logger.Log(LogLevel.Info, "The Snapshot is from a temporary aircraft. So we have to redo the list data to match the current identifier.");
				aircraftListData = aircraftListData.WithNewIdentifier(aircraftIdentifier);
				await UpdateListDataWithLock(repoAction, aircraftIdentifier, tempRepoInfo, aircraftListData).DetachContext();
			}
		}
		return tempRepoInfo;
	}

	private IObservable<AircraftSnapshotInfo> ListSnapshotsWithLock(AircraftIdentifier aircraftIdentifier, AircraftRepoInfo repoInfo)
	{
		_003C_003Ec__DisplayClass72_0 _003C_003Ec__DisplayClass72_ = new _003C_003Ec__DisplayClass72_0();
		_003C_003Ec__DisplayClass72_.repoInfo = repoInfo;
		_003C_003Ec__DisplayClass72_._003C_003E4__this = this;
		_003C_003Ec__DisplayClass72_.aircraftIdentifier = aircraftIdentifier;
		return Observable.Create<AircraftSnapshotInfo>((Func<IObserver<AircraftSnapshotInfo>, CancellationToken, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass72_0._003C_003CListSnapshotsWithLock_003Eb__0_003Ed))] (IObserver<AircraftSnapshotInfo> o, CancellationToken t) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass72_0._003C_003CListSnapshotsWithLock_003Eb__0_003Ed _003C_003CListSnapshotsWithLock_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass72_0._003C_003CListSnapshotsWithLock_003Eb__0_003Ed);
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003E4__this = _003C_003Ec__DisplayClass72_;
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed.o = o;
			_003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass72_0._003C_003CListSnapshotsWithLock_003Eb__0_003Ed>(ref _003C_003CListSnapshotsWithLock_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CListSnapshotsWithLock_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}));
	}

	private long GetSnapShotStorageSize(string snapshotDirectoryPath)
	{
		return Enumerable.Aggregate<FileSystemEntry, long>(_fs.RecursiveFilesIn(snapshotDirectoryPath), 0L, (Func<long, FileSystemEntry, long>)([CompilerGenerated] (long p, FileSystemEntry fse) => p + _fs.Stat(fse.FullPath).FileSize));
	}

	private string GetSnapShotArchivePathOrThrow(string snapshotDirPath)
	{
		string text = Path.Combine(snapshotDirPath, "prototype.gca");
		if (!_fs.PathExists(text))
		{
			text = Path.Combine(snapshotDirPath, "snapshot.gca");
			if (!_fs.PathExists(text))
			{
				throw CreateMissingSnapShotArchiveError();
			}
		}
		return text;
	}

	public IObservable<DataChangeNotification<SimInstallKey>> WhenSimulatorIsChangedOnAircraft(AircraftIdentifier aircraftIdentifier)
	{
		return HangarUtil.WhenSimulatorIsChangedOnAircraft(_hangarEvents.WhenSimulatorChanges, aircraftIdentifier);
	}

	[AsyncStateMachine(typeof(_003CGetFamilyDataForReleaseOrDefault_003Ed__76))]
	public async System.Threading.Tasks.Task<FamilyData> GetFamilyDataForReleaseOrDefault(AircraftPackageFamily aircraftPackageFamily)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await ReadAircraftHarness<FamilyData>(aircraftPackageFamily.AircraftIdentifier, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass76_0._003C_003CGetFamilyDataForReleaseOrDefault_003Eb__0_003Ed))] async (AircraftRepoInfo repoInfo) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
			string packageFamilyDataFilePath = GetFamilyDataFilePath(familyDataDirPath, aircraftPackageFamily.ReleaseId);
			if (_fs.PathExists(packageFamilyDataFilePath))
			{
				try
				{
					return new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)(await _persistence.Load<PersistedFamilyData>(packageFamilyDataFilePath).DetachContext()).Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value)));
				}
				catch (SerializationException val)
				{
					LogRepoError((System.Exception)val, "GET_FAMILY_DATA", $"[{aircraftPackageFamily.AircraftIdentifier} - {aircraftPackageFamily.ReleaseId}] Family data could not be parsed from file({packageFamilyDataFilePath})");
					throw;
				}
			}
			LogRepoMessage(LogLevel.Info, "GET_FAMILY_DATA", $"[{aircraftPackageFamily.AircraftIdentifier} - {aircraftPackageFamily.ReleaseId}] No family data found returning default.");
			return new FamilyData();
		}).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CGetAllFamilyDataForAircraft_003Ed__77))]
	public async System.Threading.Tasks.Task<FamilyDataLookup> GetAllFamilyDataForAircraft(AircraftIdentifier aircraft)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await ReadAircraftHarness<FamilyDataLookup>(aircraft, [AsyncStateMachine(typeof(_003C_003CGetAllFamilyDataForAircraft_003Eb__77_0_003Ed))] [CompilerGenerated] async (AircraftRepoInfo repoInfo) => await GetAllAircraftFamilyDataWithLock(repoInfo).DetachContext()).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CUpdateFamilyDataForRelease_003Ed__78))]
	public System.Threading.Tasks.Task UpdateFamilyDataForRelease(AircraftPackageFamily aircraftPackageFamily, FamilyData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateFamilyDataForRelease_003Ed__78 _003CUpdateFamilyDataForRelease_003Ed__ = default(_003CUpdateFamilyDataForRelease_003Ed__78);
		_003CUpdateFamilyDataForRelease_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateFamilyDataForRelease_003Ed__._003C_003E4__this = this;
		_003CUpdateFamilyDataForRelease_003Ed__.aircraftPackageFamily = aircraftPackageFamily;
		_003CUpdateFamilyDataForRelease_003Ed__.data = data;
		_003CUpdateFamilyDataForRelease_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateFamilyDataForRelease_003Ed__._003C_003Et__builder)).Start<_003CUpdateFamilyDataForRelease_003Ed__78>(ref _003CUpdateFamilyDataForRelease_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateFamilyDataForRelease_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CDeleteFamilyDataForRelease_003Ed__79))]
	public System.Threading.Tasks.Task DeleteFamilyDataForRelease(AircraftPackageFamily aircraftPackageFamily)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CDeleteFamilyDataForRelease_003Ed__79 _003CDeleteFamilyDataForRelease_003Ed__ = default(_003CDeleteFamilyDataForRelease_003Ed__79);
		_003CDeleteFamilyDataForRelease_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CDeleteFamilyDataForRelease_003Ed__._003C_003E4__this = this;
		_003CDeleteFamilyDataForRelease_003Ed__.aircraftPackageFamily = aircraftPackageFamily;
		_003CDeleteFamilyDataForRelease_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CDeleteFamilyDataForRelease_003Ed__._003C_003Et__builder)).Start<_003CDeleteFamilyDataForRelease_003Ed__79>(ref _003CDeleteFamilyDataForRelease_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CDeleteFamilyDataForRelease_003Ed__._003C_003Et__builder)).Task;
	}

	private static PersistedFamilyData CreatePersistedFamilyData(FamilyData data)
	{
		return new PersistedFamilyData
		{
			Settings = Enumerable.ToDictionary<KeyValuePair<SettingIdentifier, string>, string, string>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)data.Settings, (Func<KeyValuePair<SettingIdentifier, string>, string>)((KeyValuePair<SettingIdentifier, string> kv) => ((object)kv.Key).ToString()), (Func<KeyValuePair<SettingIdentifier, string>, string>)((KeyValuePair<SettingIdentifier, string> kv) => kv.Value))
		};
	}

	private static string GetFamilyDataDirPath(string aircraftRootDir)
	{
		return Path.Combine(aircraftRootDir, "family_data");
	}

	private static string GetFamilyDataFilePath(string familyDataDir, TrainerEmbeddedReleaseId embeddedRelease)
	{
		return Path.Combine(familyDataDir, ((object)embeddedRelease).ToString() + ".json");
	}

	[AsyncStateMachine(typeof(_003CGetAllAircraftFamilyDataWithLock_003Ed__83))]
	private async System.Threading.Tasks.Task<FamilyDataLookup> GetAllAircraftFamilyDataWithLock(AircraftRepoInfo repoInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string familyDataDirPath = GetFamilyDataDirPath(repoInfo.CurrentAircraftRoot);
		if (_fs.PathExists(familyDataDirPath))
		{
			return new FamilyDataLookup(ImmutableDictionary.ToImmutableDictionary<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>, TrainerEmbeddedReleaseId, FamilyData>((System.Collections.Generic.IEnumerable<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>)(await System.Threading.Tasks.Task.WhenAll<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>((System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>>)Enumerable.ToList<System.Threading.Tasks.Task<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>>(Enumerable.Select<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>>(Enumerable.Where<FileSystemEntry>((System.Collections.Generic.IEnumerable<FileSystemEntry>)Enumerable.ToList<FileSystemEntry>(_fs.FilesIn(familyDataDirPath)), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => TrainerEmbeddedReleaseId.CanParse(Path.GetFileNameWithoutExtension(fse.Name)))), (Func<FileSystemEntry, System.Threading.Tasks.Task<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>>>)([AsyncStateMachine(typeof(_003C_003CGetAllAircraftFamilyDataWithLock_003Eb__83_1_003Ed))] [CompilerGenerated] async (FileSystemEntry releaseIdFileEntry) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				PersistedFamilyData persistedFamilyData = await _persistence.Load<PersistedFamilyData>(releaseIdFileEntry.FullPath).DetachContext();
				return new ValueTuple<TrainerEmbeddedReleaseId, FamilyData>(new TrainerEmbeddedReleaseId(Path.GetFileNameWithoutExtension(releaseIdFileEntry.Name)), new FamilyData(ImmutableDictionary.ToImmutableDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)persistedFamilyData.Settings, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> kv) => SettingIdentifier.FromString(kv.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => kv.Value))));
			})))).DetachContext()), (Func<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>, TrainerEmbeddedReleaseId>)((ValueTuple<TrainerEmbeddedReleaseId, FamilyData> t) => t.Item1), (Func<ValueTuple<TrainerEmbeddedReleaseId, FamilyData>, FamilyData>)((ValueTuple<TrainerEmbeddedReleaseId, FamilyData> t) => t.Item2)));
		}
		return new FamilyDataLookup();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize((object)this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				_hangarEvents.Dispose();
			}
			_disposedValue = true;
		}
	}

	private static InvalidOperationException ThrowInternalRepoError()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		throw new InvalidOperationException("Internal repo error.");
	}

	private static KeyValuePair<string, PersistedSimulator> ConvertToPersistedSimulatorAndLocation(SimInstallData sim, AircraftRepoInfo repoInfo)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if ((object)sim.FullPackageId == null)
		{
			throw new ArgumentException("sim data must have package reference.");
		}
		PersistedSimulator persistedSimulator = new PersistedSimulator
		{
			FamilyId = sim.InstallationKey.ReleaseId.FamilyId.IdentStringChecked,
			FullModelId = ((object)sim.ToFullModelId().ToSettingIdentifier()).ToString(),
			InstallationVersion = ((object)sim.FullPackageId.BinaryVersion).ToString(),
			ModuleData = sim.ModuleData.CopyAllDataAsObject(),
			Options = Enumerable.ToDictionary<KeyValuePair<SettingIdentifier, string>, string, string>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)sim.Settings, (Func<KeyValuePair<SettingIdentifier, string>, string>)((KeyValuePair<SettingIdentifier, string> setting) => setting.Key.FullId), (Func<KeyValuePair<SettingIdentifier, string>, string>)((KeyValuePair<SettingIdentifier, string> setting) => setting.Value))
		};
		return new KeyValuePair<string, PersistedSimulator>(BuildSimJsonFilePath(sim.InstallationKey, repoInfo), persistedSimulator);
	}

	private static string BuildSimJsonFilePath(SimInstallKey simKey, AircraftRepoInfo repoInfo)
	{
		return Path.Combine(repoInfo.CurrentAircraftRoot, "avionics", simKey.ToStandardRelativePath().RelPath, "simulator.json");
	}

	private static IObservable<FileSystemEntry> SafeDirectoryStream(IFileSystem fs, string directoryPath)
	{
		IFileSystem fs2 = fs;
		string directoryPath2 = directoryPath;
		return Observable.Defer<FileSystemEntry>((Func<IObservable<FileSystemEntry>>)(() => (!fs2.PathExists(directoryPath2)) ? Observable.Empty<FileSystemEntry>() : fs2.DirectoriesIn(directoryPath2).ToObservable())).Catch<FileSystemEntry, FileNotFoundException>((Func<FileNotFoundException, IObservable<FileSystemEntry>>)((FileNotFoundException error) => SafeDirectoryStream(fs2, directoryPath2))).Catch<FileSystemEntry, DirectoryNotFoundException>((Func<DirectoryNotFoundException, IObservable<FileSystemEntry>>)((DirectoryNotFoundException error) => SafeDirectoryStream(fs2, directoryPath2)))
			.Distinct<FileSystemEntry, string>((FileSystemEntry fse) => fse.FullPath);
	}

	private static void ThrowIfAircraftIdentifierIsInvalid(AircraftIdentifier aircraftIdent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		aircraftIdent.ThrowIfDefault((System.Exception?)new ArgumentException("Invalid aircraft Identifier."));
	}

	private static bool IsInstallationSlotDirectoryName(FileSystemEntry fse)
	{
		return InstallationSlot.ParseOrInvalid(fse.Name) != InstallationSlot.Invalid;
	}

	private static AircraftRepoInfo CreateRepoInfo(string aircraftDirectoryPath)
	{
		string text = Path.Combine(aircraftDirectoryPath, "current");
		return new AircraftRepoInfo
		{
			RootPath = aircraftDirectoryPath,
			CurrentAircraftRoot = text,
			CurrentAvionicsRoot = Path.Combine(text, "avionics"),
			HistoryAircraftRoot = Path.Combine(aircraftDirectoryPath, "history")
		};
	}

	private static void LogRepoMessage(LogLevel level, string repoAction, string formattedMessage, params object[] formatArgs)
	{
		Logger.Log(level, "[Aircraft Repo] [{0}] {1}", repoAction, string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, formattedMessage, formatArgs));
	}

	private static void LogRepoError(System.Exception e, string repoAction)
	{
		LogRepoError(e, repoAction, string.Empty);
	}

	private static void LogRepoError(System.Exception e, string repoAction, string formattedMessage, params object[] formatArgs)
	{
		Logger.LogFormattedException(e, "[Aircraft Repo] [{0}] {1}", repoAction, string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, formattedMessage, formatArgs));
	}

	[IteratorStateMachine(typeof(_003CValidatePersistedAircraftListData_003Ed__96))]
	private static System.Collections.Generic.IEnumerable<System.Exception> ValidatePersistedAircraftListData(PersistedAircraftListData persistedAircraft, string directoryName, string aircraftJsonPath)
	{
		if (!TrainerIdentifier.IsValidIdentifier(persistedAircraft.AircraftIdentifier))
		{
			string text = "The 'AircraftIdentifier' property in '" + aircraftJsonPath + "' is not formatted correctly.";
			if (SoftConfigurations.IsDebug)
			{
				text += " (^[A-Za-z0-9_]+$)";
			}
			yield return (System.Exception)new FormatException(text);
		}
		if ((string?)persistedAircraft.AircraftIdentifier != directoryName)
		{
			yield return (System.Exception)new FormatException(string.Concat(new string[5] { "The 'AircraftIdentifier' property in '", aircraftJsonPath, "' must match the directory name: '", directoryName, "'." }));
		}
		if (string.IsNullOrEmpty(persistedAircraft.DisplayName))
		{
			yield return (System.Exception)new FormatException("The 'DisplayName' property in '" + aircraftJsonPath + "' cannot be null or empty");
		}
	}

	private static string GetParentDirectoryPath(string path)
	{
		return LauncherPathHelpers.NonRootPathParentDirectory(path);
	}

	[AsyncStateMachine(typeof(_003CGetAircraftSnapshotWithLock_003Ed__98))]
	private async System.Threading.Tasks.Task<AircraftSnapshotData> GetAircraftSnapshotWithLock(AircraftIdentifier aircraftIdent, AircraftRepoInfo repoInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AircraftSnapshotData aircraftSnapshotData = new AircraftSnapshotData(aircraftIdent);
		System.Threading.Tasks.Task<AircraftListData> listDataWithLock = GetListDataWithLock(repoInfo);
		System.Threading.Tasks.Task<AircraftHeavyData> heavyTask = GetHeavyDataWithLock(aircraftIdent, repoInfo);
		System.Threading.Tasks.Task<SimPanel> panelTask = GetPersistedPanelWithLock(aircraftIdent, repoInfo);
		System.Threading.Tasks.Task<FamilyDataLookup> familyTask = GetAllAircraftFamilyDataWithLock(repoInfo);
		AircraftSnapshotData aircraftSnapshotData2 = aircraftSnapshotData;
		aircraftSnapshotData = aircraftSnapshotData2.Relist(await listDataWithLock.DetachContext());
		aircraftSnapshotData2 = aircraftSnapshotData;
		aircraftSnapshotData = aircraftSnapshotData2.ReplacePanel(await panelTask.DetachContext());
		aircraftSnapshotData2 = aircraftSnapshotData;
		aircraftSnapshotData = aircraftSnapshotData2.ReplaceHeavyData(await heavyTask.DetachContext());
		aircraftSnapshotData2 = aircraftSnapshotData;
		return aircraftSnapshotData2.ReplaceFamilyLookup(await familyTask.DetachContext());
	}

	[AsyncStateMachine(typeof(_003CGetListDataWithLock_003Ed__99))]
	private async System.Threading.Tasks.Task<AircraftListData> GetListDataWithLock(AircraftRepoInfo repoInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string path = Path.Combine(repoInfo.CurrentAircraftRoot, "list.json");
		return (await _persistence.Load<PersistedAircraftListData>(path).DetachContext()).ToListData();
	}

	[AsyncStateMachine(typeof(_003CGetHeavyDataWithLock_003Ed__100))]
	private async System.Threading.Tasks.Task<AircraftHeavyData> GetHeavyDataWithLock(AircraftIdentifier aircraftIdentifier, AircraftRepoInfo repoInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string path = Path.Combine(repoInfo.CurrentAircraftRoot, "heavy.json");
		if (_fs.PathExists(path))
		{
			return (await _persistence.Load<PersistedAircraftHeavyData>(path).DetachContext()).ToRuntime(aircraftIdentifier);
		}
		return new AircraftHeavyData(aircraftIdentifier);
	}

	[AsyncStateMachine(typeof(_003CGetPersistedPanelWithLock_003Ed__101))]
	private async System.Threading.Tasks.Task<SimPanel> GetPersistedPanelWithLock(AircraftIdentifier aircraftIdent, AircraftRepoInfo repoInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string avionicsDirPath = repoInfo.CurrentAvionicsRoot;
		string path = Path.Combine(repoInfo.CurrentAircraftRoot, "panel.json");
		PersistedSimPanelInfo persistedSimPanelInfo = ((!_fs.PathExists(path)) ? new PersistedSimPanelInfo() : (await _persistence.Load<PersistedSimPanelInfo>(path).DetachContext()));
		PersistedSimPanelInfo persistedSimPanelInfo2 = persistedSimPanelInfo;
		HashSet<SimInstallKey> activeSimKeys = Enumerable.ToHashSet<SimInstallKey>(persistedSimPanelInfo2.ActiveSimulatorKeys);
		System.Collections.Generic.IList<SimInstallData> allPersistedSimulators = await ListSimDirectories(avionicsDirPath).SelectMany<string, SimInstallData>((Func<string, System.Threading.Tasks.Task<SimInstallData>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass101_0._003C_003CGetPersistedPanelWithLock_003Eb__1_003Ed))] async (string simDirPath) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string text = Path.Combine(simDirPath, "simulator.json");
			if (_fs.PathExists(text))
			{
				PersistedSimulator persistedSimulator = await _persistence.Load<PersistedSimulator>(text).DetachContext();
				SimInstallKey simInstallKey = SimInstallKey.Parse(PathHelpers.CreateRelativePath(avionicsDirPath, simDirPath));
				return new SimInstallData(new SimAircraftKey(aircraftIdent, simInstallKey), new FullModelIdentifier(SettingIdentifier.FromString(persistedSimulator.FullModelId)).TrainerPackageId, (System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>?)Enumerable.ToDictionary<KeyValuePair<string, string>, SettingIdentifier, string>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)persistedSimulator.Options, (Func<KeyValuePair<string, string>, SettingIdentifier>)((KeyValuePair<string, string> o) => new SettingIdentifier(o.Key)), (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> o) => o.Value)), new ReadOnlyJsonDataSource(persistedSimulator.ModuleData), _fs.PathExists(Path.Combine(simDirPath, "winfs_mount.gca")), activeSimKeys.Contains(simInstallKey));
			}
			throw new FileNotFoundException("Missing simulator data.", text);
		})).ToList<SimInstallData>();
		System.Collections.Generic.IEnumerable<SimInstallKey> enumerable = Enumerable.Where<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)activeSimKeys, (Func<SimInstallKey, bool>)((SimInstallKey activeKey) => !Enumerable.Any<SimInstallData>((System.Collections.Generic.IEnumerable<SimInstallData>)allPersistedSimulators, (Func<SimInstallData, bool>)((SimInstallData ps) => ps.InstallationKey == activeKey))));
		if (Enumerable.Any<SimInstallKey>(enumerable))
		{
			throw new FormatException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Active simulators are missing from disk: [{0}]", (object)string.Join<SimInstallKey>(", ", enumerable)));
		}
		try
		{
			return new SimPanel().WithUpdatedSimulators((System.Collections.Generic.IEnumerable<SimInstallData>)allPersistedSimulators);
		}
		catch (ArgumentException val)
		{
			ArgumentException val2 = val;
			throw new FormatException("Corrupted Panel files detected.", (System.Exception)(object)val2);
		}
	}

	[AsyncStateMachine(typeof(_003CUpdateHeavyDataWithLock_003Ed__102))]
	private System.Threading.Tasks.Task UpdateHeavyDataWithLock(string repoAction, AircraftIdentifier aircraftIdentifier, AircraftRepoInfo repoInfo, AircraftHeavyData heavyData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateHeavyDataWithLock_003Ed__102 _003CUpdateHeavyDataWithLock_003Ed__ = default(_003CUpdateHeavyDataWithLock_003Ed__102);
		_003CUpdateHeavyDataWithLock_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateHeavyDataWithLock_003Ed__._003C_003E4__this = this;
		_003CUpdateHeavyDataWithLock_003Ed__.repoAction = repoAction;
		_003CUpdateHeavyDataWithLock_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CUpdateHeavyDataWithLock_003Ed__.repoInfo = repoInfo;
		_003CUpdateHeavyDataWithLock_003Ed__.heavyData = heavyData;
		_003CUpdateHeavyDataWithLock_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateHeavyDataWithLock_003Ed__._003C_003Et__builder)).Start<_003CUpdateHeavyDataWithLock_003Ed__102>(ref _003CUpdateHeavyDataWithLock_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateHeavyDataWithLock_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Ed__103))]
	private async System.Threading.Tasks.Task<SimPanel> GetPreviousPanelAndUpdateWithNewOneWithLock(string repoAction, AircraftIdentifier aircraftIdentifier, AircraftRepoInfo repoInfo, SimPanel simulatorPanel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass103_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass103_0();
		CS_0024_003C_003E8__locals0.repoInfo = repoInfo;
		CS_0024_003C_003E8__locals0.repoAction = repoAction;
		CS_0024_003C_003E8__locals0.aircraftIdentifier = aircraftIdentifier;
		CS_0024_003C_003E8__locals0._003C_003E4__this = this;
		SimPanel previousPanel = await GetPersistedPanelWithLock(CS_0024_003C_003E8__locals0.aircraftIdentifier, CS_0024_003C_003E8__locals0.repoInfo).DetachContext();
		CS_0024_003C_003E8__locals0.updatedSimulatorJsonFiles = Enumerable.ToList<KeyValuePair<string, PersistedSimulator>>(Enumerable.Select<SimInstallData, KeyValuePair<string, PersistedSimulator>>(simulatorPanel.AllSimulators.Values, (Func<SimInstallData, KeyValuePair<string, PersistedSimulator>>)((SimInstallData sim) => ConvertToPersistedSimulatorAndLocation(sim, CS_0024_003C_003E8__locals0.repoInfo))));
		CS_0024_003C_003E8__locals0.removedSimulatorJsonFiles = Enumerable.ToList<string>(Enumerable.Select<SimInstallKey, string>(previousPanel.SimulatorChanges(simulatorPanel).FilterByChangeType(DataChangeType.Deleted).StripChangeType(), (Func<SimInstallKey, string>)((SimInstallKey deletedKey) => BuildSimJsonFilePath(deletedKey, CS_0024_003C_003E8__locals0.repoInfo))));
		CS_0024_003C_003E8__locals0.panelInfoToSave = PersistedSimPanelInfo.FromRuntime(simulatorPanel);
		await TaskInvoke.AllInParallel([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed))] () =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed);
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed>(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__2_003Ed._003C_003Et__builder)).Task;
		}, [AsyncStateMachine(typeof(_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed))] () =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed);
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
			_003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass103_0._003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed>(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CGetPreviousPanelAndUpdateWithNewOneWithLock_003Eb__3_003Ed._003C_003Et__builder)).Task;
		}).DetachContext();
		return previousPanel;
	}

	[AsyncStateMachine(typeof(_003CUpdateListDataWithLock_003Ed__104))]
	private System.Threading.Tasks.Task UpdateListDataWithLock(string repoAction, AircraftIdentifier aircraftIdentifier, AircraftRepoInfo repoInfo, AircraftListData updatedListData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateListDataWithLock_003Ed__104 _003CUpdateListDataWithLock_003Ed__ = default(_003CUpdateListDataWithLock_003Ed__104);
		_003CUpdateListDataWithLock_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CUpdateListDataWithLock_003Ed__._003C_003E4__this = this;
		_003CUpdateListDataWithLock_003Ed__.repoAction = repoAction;
		_003CUpdateListDataWithLock_003Ed__.aircraftIdentifier = aircraftIdentifier;
		_003CUpdateListDataWithLock_003Ed__.repoInfo = repoInfo;
		_003CUpdateListDataWithLock_003Ed__.updatedListData = updatedListData;
		_003CUpdateListDataWithLock_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CUpdateListDataWithLock_003Ed__._003C_003Et__builder)).Start<_003CUpdateListDataWithLock_003Ed__104>(ref _003CUpdateListDataWithLock_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CUpdateListDataWithLock_003Ed__._003C_003Et__builder)).Task;
	}

	private void ThrowIfAircraftDirectoryIsNotEmpty(AircraftIdentifier aircraftIdentifier, string aircraftDirectoryPath)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!_fs.IsDirectoryEmpty(aircraftDirectoryPath))
		{
			throw new DuplicateNameException("The " + (string?)aircraftIdentifier + " aircraft already exists.");
		}
	}

	[AsyncStateMachine(typeof(_003CTemporarilyUnzipAircraftGca_003Ed__106<>))]
	private async System.Threading.Tasks.Task<T> TemporarilyUnzipAircraftGca<T>(string gcaPath, string repoAction, Func<string, AircraftIdentifier, System.Threading.Tasks.Task<T>> useDecompressedGca)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfGcaDoesNotExistOrIsNotAFile(gcaPath, repoAction);
		string text = _deployment.PerProcessTempDirectory();
		string text2 = repoAction.Replace(" ", string.Empty, (StringComparison)4);
		Guid val = Guid.NewGuid();
		string tempAircraftPath = Path.Combine(text, text2, ((object)(Guid)(ref val)).ToString());
		_fs.CreateDirectory(tempAircraftPath);
		try
		{
			_zipCompression.UnZip(gcaPath, tempAircraftPath);
			List<FileSystemEntry> val2 = Enumerable.ToList<FileSystemEntry>(_fs.EntriesIn(tempAircraftPath));
			if (val2.Count != 1)
			{
				throw new FormatException("Aircraft GCA's should only have the root aircraft directory in the root of the archive.");
			}
			if (val2[0].EntryType != FileSystemEntryType.Directory)
			{
				throw new FormatException("Aircraft GCA's only root entry should be the aircraft directory with the identifier as the name.");
			}
			string identString = val2[0].Name;
			if (!AircraftIdentifier.IsValidIdentifier(identString))
			{
				throw new FormatException("The Aircraft GCA aircraft directory name is not a valid identifier.");
			}
			string aircraftRootDirPath = val2[0].FullPath;
			AircraftRepoInfo repoInfo = CreateRepoInfo(aircraftRootDirPath);
			await PerformAircraftFormatMigrationWithLock(repoInfo).DetachContext();
			return await useDecompressedGca.Invoke(aircraftRootDirPath, new AircraftIdentifier(identString)).DetachContext();
		}
		finally
		{
			_fs.DeleteIfExistsAndParentsThatAreEmpty(tempAircraftPath, _deployment.PerProcessTempDirectory());
		}
	}

	private void ThrowIfGcaDoesNotExistOrIsNotAFile(string gcaPath, string repoAction)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		FileStats fileStats = _fs.StatIfExists(gcaPath);
		if (!fileStats.Exists)
		{
			FileNotFoundException val = new FileNotFoundException(PathHelpers.QuotePathIfNeeded(gcaPath) + " does not exist.", gcaPath);
			LogRepoError((System.Exception)val, repoAction);
			throw val;
		}
		if (fileStats.Entry.EntryType == FileSystemEntryType.Directory)
		{
			throw new ArgumentException(PathHelpers.QuotePathIfNeeded(gcaPath) + " is a directory.");
		}
		if (fileStats.Entry.EntryType != FileSystemEntryType.File)
		{
			throw new ArgumentException(PathHelpers.QuotePathIfNeeded(gcaPath) + " is not a file.");
		}
	}

	private AircraftRepoInfo CreateRepoInfoFromHangarControlledIdentifier(TrainerIdentifier aircraftIdentifier)
	{
		return CreateRepoInfo(Path.Combine(RootDirectory, aircraftIdentifier.IdentStringChecked));
	}

	private void ThrowNotFoundIfAircraftDirectoryIsMissing(AircraftRepoInfo repoInfo)
	{
		if (!_fs.PathExists(repoInfo.RootPath))
		{
			throw new AircraftNotFoundException();
		}
	}

	[AsyncStateMachine(typeof(_003CReserveFirstUnusedIdFrom_003Ed__110))]
	private async System.Threading.Tasks.Task<AircraftIdentifier> ReserveFirstUnusedIdFrom(System.Collections.Generic.IEnumerable<AircraftIdentifier> possibleIds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await HangarUtil.ReserveFirstUnusedIdFrom(possibleIds, ReserveAircraftDirectoryIfNotTaken).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CReserveAircraftDirectoryIfNotTaken_003Ed__111))]
	private async System.Threading.Tasks.Task<bool> ReserveAircraftDirectoryIfNotTaken(AircraftIdentifier id)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		AircraftRepoInfo tmpRepoInfo = CreateRepoInfoFromHangarControlledIdentifier(id);
		using (await GetLockForPathUnlocked(tmpRepoInfo.RootPath).ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			if (!_fs.PathExists(tmpRepoInfo.RootPath))
			{
				_fs.CreateDirectory(tmpRepoInfo.RootPath);
				LogRepoMessage(LogLevel.Info, "Create new aircraft", "Found new unused aircraft id: '{0}'", id);
				return true;
			}
			LogRepoMessage(LogLevel.Debug, "Create new aircraft", ": 'Aircraft id already taken: {0}'", id);
		}
		return false;
	}

	private void PostSelectedAircaftChange(AircraftIdentifier result)
	{
		_hangarEvents.PostSelectedAircraftChange(result);
	}

	private string BuildSelectedAircraftFilepath()
	{
		return Path.Combine(RootDirectory, "selected.json");
	}

	private System.Collections.Generic.IEnumerable<FileSystemEntry> RootDirectorySlotsThatNeedMigrating(string aircraftRootDirectory)
	{
		string pattern = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "({0})|({1})_\\d+", (object)AvionicsType.Display, (object)AvionicsType.Navigator);
		return Enumerable.Where<FileSystemEntry>(_fs.EntriesIn(aircraftRootDirectory), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => Regex.IsMatch(fse.Name, pattern)));
	}

	private AsyncReaderWriterLock GetLockForPathUnlocked(string aircraftDirectoryPath)
	{
		return _aircraftLocks.GetLockForAircraftUnlocked((AircraftIdentifier)Path.GetFileName(aircraftDirectoryPath));
	}

	private void PostAircraftChange(AircraftIdentifier aircraftIdent, DataChangeType change)
	{
		_hangarEvents.PostAircraftChange(DataChangeNotification.Create(aircraftIdent, change));
	}

	private void PostSimulatorChange(SimAircraftKey aircraftSimKey, DataChangeType change)
	{
		_hangarEvents.PostSimulatorChange(DataChangeNotification.Create(aircraftSimKey, change));
	}

	private void PostSimulatorChanges(System.Collections.Generic.IEnumerable<SimAircraftKey> aircraftSimKeys, DataChangeType change)
	{
		PostSimulatorChanges(Enumerable.Select<SimAircraftKey, DataChangeNotification<SimAircraftKey>>(aircraftSimKeys, (Func<SimAircraftKey, DataChangeNotification<SimAircraftKey>>)((SimAircraftKey simAircraft) => DataChangeNotification.Create(simAircraft, change))));
	}

	private void PostSimulatorChanges(System.Collections.Generic.IEnumerable<DataChangeNotification<SimAircraftKey>> simChanges)
	{
		_hangarEvents.PostSimulatorChanges(simChanges);
	}

	private bool IsValidWinfsArchive(string winfsArchivePath, string repoAction)
	{
		FileStats fileStats = _fs.Stat(winfsArchivePath);
		if (fileStats.Entry.EntryType == FileSystemEntryType.None)
		{
			LogRepoMessage(LogLevel.Error, repoAction, "File does not exist: {0}", winfsArchivePath);
			return false;
		}
		if (fileStats.Entry.EntryType == FileSystemEntryType.Directory)
		{
			LogRepoMessage(LogLevel.Error, repoAction, "Attempting to update the winfs archive with a directory. {0}", winfsArchivePath);
			return false;
		}
		if (fileStats.Entry.EntryType != FileSystemEntryType.File)
		{
			LogRepoMessage(LogLevel.Error, repoAction, "Attempting to update the winfs archive with an unknown filesystem type. {0}", winfsArchivePath);
			return false;
		}
		try
		{
			ZipArchive val = _zipCompression.OpenArchive(winfsArchivePath, (ZipArchiveMode)0);
			try
			{
			}
			finally
			{
				((System.IDisposable)val)?.Dispose();
			}
		}
		catch (System.Exception ex) when (ex is FileNotFoundException || ex is InvalidDataException)
		{
			Logger.LogFormattedException(ex, "Could not open a winfs mount archive.");
			return false;
		}
		return true;
	}

	[AsyncStateMachine(typeof(_003CTakePrototypeSnapShotWithLock_003Ed__121))]
	private async System.Threading.Tasks.Task<AircraftSnapshotInfo> TakePrototypeSnapShotWithLock(string repoAction, AircraftIdentifier aircraftIdentifier, AircraftRepoInfo repoInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DateTimeOffset takenAt = DateTimeOffset.Now;
		SnapshotIdentifier snapshotId = new SnapshotIdentifier(takenAt);
		string rootPath = repoInfo.RootPath;
		string snapshotDirPath = Path.Combine(repoInfo.HistoryAircraftRoot, snapshotId.Serialized);
		string temporaryArchivePath = Path.Combine(RootDirectory, ".tmp", aircraftIdentifier.IdentStringChecked, "prototype.gca");
		try
		{
			_fs.CreateDirectory(GetParentDirectoryPath(temporaryArchivePath));
			_zipCompression.ZipDirectory(rootPath, temporaryArchivePath);
			_fs.DeleteIfExists(repoInfo.HistoryAircraftRoot);
			string text = Path.Combine(snapshotDirPath, "prototype.gca");
			_fs.CreateDirectory(GetParentDirectoryPath(text));
			_fs.MoveOrCopyAndDeleteToDifferentDriveAndOverwrite(LolaFsExtensions.DefaultTempFactory, temporaryArchivePath, text);
			_fs.SetFileTimes(text, new FileTimes(takenAt));
			string source = Path.Combine(repoInfo.CurrentAircraftRoot, "list.json");
			string text2 = Path.Combine(snapshotDirPath, "list.json");
			_fs.CopyFile(source, text2, overwrite: false);
			PersistedAircraftListData prototypeListData = await _persistence.Load<PersistedAircraftListData>(text2).DetachContext();
			AircraftSnapshotMetaData metadata = new AircraftSnapshotMetaData("Prototype", null);
			string path = Path.Combine(snapshotDirPath, "metadata.json");
			await _persistence.Commit(PersistedSnapshotMetadata.FromRuntime(new AircraftSnapshotMetaData("Prototype", null)), path).DetachContext();
			long snapShotStorageSize = GetSnapShotStorageSize(snapshotDirPath);
			return new AircraftSnapshotInfo(aircraftIdentifier, snapshotId, takenAt, prototypeListData.ToListData(), metadata, snapShotStorageSize);
		}
		catch
		{
			_fs.DeleteIfExists(snapshotDirPath);
			throw;
		}
		finally
		{
			_fs.DeleteIfExistsAndParentsThatAreEmpty(temporaryArchivePath, RootDirectory);
		}
	}

	[AsyncStateMachine(typeof(_003CTakeSnapShotWithLock_003Ed__122))]
	private async System.Threading.Tasks.Task<AircraftSnapshotInfo> TakeSnapShotWithLock(string repoAction, AircraftIdentifier aircraftIdentifier, AircraftRepoInfo repoInfo, AircraftSnapshotMetaData metadata)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DateTimeOffset takenAt = DateTimeOffset.Now;
		SnapshotIdentifier snapshotId = new SnapshotIdentifier(takenAt);
		string currentAircraftRoot = repoInfo.CurrentAircraftRoot;
		string snapshotDir = Path.Combine(repoInfo.HistoryAircraftRoot, snapshotId.Serialized);
		LogRepoMessage(LogLevel.Info, repoAction, $"Taking a snapshot of {aircraftIdentifier}. Description: {metadata.Description}");
		try
		{
			string text = Path.Combine(snapshotDir, "snapshot.gca");
			_fs.CreateDirectory(GetParentDirectoryPath(text));
			_zipCompression.ZipDirectory(currentAircraftRoot, text);
			_fs.SetFileTimes(text, new FileTimes(takenAt));
			string path = Path.Combine(snapshotDir, "metadata.json");
			await _persistence.Commit(PersistedSnapshotMetadata.FromRuntime(metadata), path).DetachContext();
			long snapShotStorageSize = GetSnapShotStorageSize(snapshotDir);
			return new AircraftSnapshotInfo(aircraftIdentifier, snapshotId, takenAt, null, metadata, snapShotStorageSize);
		}
		catch
		{
			_fs.DeleteIfExists(snapshotDir);
			throw;
		}
	}

	[AsyncStateMachine(typeof(_003CCheckForAndPerformMigration_003Ed__123))]
	private System.Threading.Tasks.Task CheckForAndPerformMigration(AircraftRepoInfo repoInfo, CancellationToken token = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_003CCheckForAndPerformMigration_003Ed__123 _003CCheckForAndPerformMigration_003Ed__ = default(_003CCheckForAndPerformMigration_003Ed__123);
		_003CCheckForAndPerformMigration_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCheckForAndPerformMigration_003Ed__._003C_003E4__this = this;
		_003CCheckForAndPerformMigration_003Ed__.repoInfo = repoInfo;
		_003CCheckForAndPerformMigration_003Ed__.token = token;
		_003CCheckForAndPerformMigration_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCheckForAndPerformMigration_003Ed__._003C_003Et__builder)).Start<_003CCheckForAndPerformMigration_003Ed__123>(ref _003CCheckForAndPerformMigration_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCheckForAndPerformMigration_003Ed__._003C_003Et__builder)).Task;
	}

	private bool NeedsAMigration(string aircraftRootDirectoryPath)
	{
		if (!Enumerable.Any<FileSystemEntry>(_fs.EntriesIn(aircraftRootDirectoryPath)))
		{
			return false;
		}
		return GetMigrationLevel(aircraftRootDirectoryPath).GetValueOrDefault() != 1;
	}

	private int? GetMigrationLevel(string aircraftRootDirectoryPath)
	{
		string text = Path.Combine(aircraftRootDirectoryPath, "current", "hangarFormat.dat");
		if (!_fs.PathExists(text))
		{
			return null;
		}
		try
		{
			return Convert.ToInt32(_fs.ReadAllTextSync(text), (IFormatProvider)(object)CultureInfo.InvariantCulture);
		}
		catch (System.Exception ex) when (ex is FormatException || ex is OverflowException)
		{
			Logger.LogFormattedException(ex, "Invalid aircraft hangar format file: {0}.", PathHelpers.QuotePathIfNeeded(text));
			return null;
		}
	}

	[AsyncStateMachine(typeof(_003CPlaceMigrationFile_003Ed__126))]
	private System.Threading.Tasks.Task PlaceMigrationFile(AircraftRepoInfo repoInfo)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CPlaceMigrationFile_003Ed__126 _003CPlaceMigrationFile_003Ed__ = default(_003CPlaceMigrationFile_003Ed__126);
		_003CPlaceMigrationFile_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CPlaceMigrationFile_003Ed__._003C_003E4__this = this;
		_003CPlaceMigrationFile_003Ed__.repoInfo = repoInfo;
		_003CPlaceMigrationFile_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CPlaceMigrationFile_003Ed__._003C_003Et__builder)).Start<_003CPlaceMigrationFile_003Ed__126>(ref _003CPlaceMigrationFile_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CPlaceMigrationFile_003Ed__._003C_003Et__builder)).Task;
	}

	private string MigrateAvionicsDirIntoCurrentAndHistoryFormat(AircraftRepoInfo repoInfo)
	{
		//IL_00e9: Expected O, but got Unknown
		_fs.CreateDirectory(repoInfo.CurrentAircraftRoot);
		System.Collections.Generic.IEnumerator<string> enumerator = Enumerable.Select<FileSystemEntry, string>(Enumerable.Where<FileSystemEntry>(_fs.EntriesIn(repoInfo.RootPath), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => !string.Equals(fse.Name, "current", (StringComparison)5) && !string.Equals(fse.Name, "history", (StringComparison)5))), (Func<FileSystemEntry, string>)((FileSystemEntry itemToMove) => itemToMove.FullPath)).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				string current = enumerator.Current;
				string text = Path.Combine(repoInfo.CurrentAircraftRoot, PathHelpers.CreateRelativePath(repoInfo.RootPath, current));
				if (!_fs.PathExists(current))
				{
					continue;
				}
				if (!_fs.PathExists(text))
				{
					Logger.Log(LogLevel.Info, "Migrating '{0}' to '{1}'.", current, text);
					try
					{
						_fs.MoveIfExists(current, text);
					}
					catch (IOException val)
					{
						Logger.LogFormattedException((System.Exception)val, "Migrating root aircraft directory item");
						throw;
					}
				}
				else
				{
					Logger.Log(LogLevel.Info, "Cannot migrate '{0}' because '{1}' already exists. Deleting '{0}'.", current, text);
					_fs.DeleteIfExists(current);
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		return Path.Combine(repoInfo.CurrentAircraftRoot, "avionics");
	}

	private void MigratePrototypeToHistoryDirectory(AircraftRepoInfo repoInfo)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		string text = Path.Combine(repoInfo.RootPath, "prototype.zip");
		if (_fs.PathExists(text))
		{
			SnapshotIdentifier snapshotIdentifier = new SnapshotIdentifier(_fs.Stat(text).ModifiedAt);
			string text2 = Path.Combine(repoInfo.HistoryAircraftRoot, snapshotIdentifier.Serialized);
			string destination = Path.Combine(text2, "prototype.gca");
			if (!_fs.PathExists(text2))
			{
				Logger.Log(LogLevel.Info, "Migrating '{0}' to '{1}'.", text, text2);
				_fs.CreateDirectory(text2);
				_fs.MoveIfExists(text, destination);
			}
			else
			{
				Logger.Log(LogLevel.Info, "Cannot migrate '{0}' to '{1}' because '{1}' already exists. Deleting '{0}'.", text, text2);
				_fs.DeleteIfExists(text);
			}
		}
	}

	private IObservable<FamilyIdMigration> ListOldFamilyIdDirectoriesIn(string aircraftAvionicsDirectoryPath)
	{
		return SafeDirectoryStream(_fs, aircraftAvionicsDirectoryPath).Where<FileSystemEntry>((Func<FileSystemEntry, bool>)IsInstallationSlotDirectoryName).SelectMany<FileSystemEntry, FamilyIdMigration>((Func<FileSystemEntry, IObservable<FamilyIdMigration>>)([CompilerGenerated] (FileSystemEntry slotDir) =>
		{
			InstallationSlot slot = InstallationSlot.Parse(slotDir.Name);
			return (from migration in SafeDirectoryStream(_fs, slotDir.FullPath).Where<FileSystemEntry>((Func<FileSystemEntry, bool>)((FileSystemEntry familyIdDir) => !TrainerEmbeddedReleaseId.CanParse(familyIdDir.Name))).SelectMany<FileSystemEntry, FamilyIdMigration>((Func<FileSystemEntry, IObservable<FamilyIdMigration>>)((FileSystemEntry possibleV1Dir) => SafeDirectoryStream(_fs, possibleV1Dir.FullPath).Select<FileSystemEntry, string>((Func<FileSystemEntry, string>)((FileSystemEntry modelDir) => Path.Combine(modelDir.FullPath, "simulator.json"))).Where<string>((Func<string, bool>)([CompilerGenerated] (string simJsonPath) => _fs.PathExists(simJsonPath))).SelectMany<string, FamilyIdMigration>((Func<string, System.Threading.Tasks.Task<FamilyIdMigration>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass129_1._003C_003CListOldFamilyIdDirectoriesIn_003Eb__9_003Ed))] async (string simJsonPath) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					try
					{
						PersistedSimulator persistedSimulator = await _persistence.Load<PersistedSimulator>(simJsonPath).DetachContext();
						return new FamilyIdMigration(slot, possibleV1Dir, new FullModelIdentifier(persistedSimulator.FullModelId));
					}
					catch (SerializationException val)
					{
						Logger.LogFormattedException((System.Exception)val, "Discovering V1 Sim directories.");
						return (FamilyIdMigration)null;
					}
				}))
					.WhereClassNotNull<FamilyIdMigration>()
					.Where<FamilyIdMigration>((Func<FamilyIdMigration, bool>)((FamilyIdMigration migration) => migration.FullModelId.TrainerPackageId.IsValid)))).Do<FamilyIdMigration>((Action<FamilyIdMigration>)delegate(FamilyIdMigration migration)
				{
					if (!migration.DirectoryMatchesFamilyIdInSimJson())
					{
						Logger.Log(LogLevel.Warning, "Unexpected family directory found: '{0}'", migration.DirectoryEntry.FullPath);
					}
				})
					.Where<FamilyIdMigration>((Func<FamilyIdMigration, bool>)((FamilyIdMigration migration) => migration.DirectoryMatchesFamilyIdInSimJson()))
				group migration by migration.FullModelId.TrainerPackageId.FamilyId).SelectMany<IGroupedObservable<TrainerIdentifier, FamilyIdMigration>, FamilyIdMigration>((Func<IGroupedObservable<TrainerIdentifier, FamilyIdMigration>, IObservable<FamilyIdMigration>>)((IGroupedObservable<TrainerIdentifier, FamilyIdMigration> legacyFamily) => ((IObservable<FamilyIdMigration>)legacyFamily).MaxBy<FamilyIdMigration, Version>((FamilyIdMigration migration) => migration.FullModelId.TrainerPackageId.BinaryVersion).Select<System.Collections.Generic.IList<FamilyIdMigration>, FamilyIdMigration>((Func<System.Collections.Generic.IList<FamilyIdMigration>, FamilyIdMigration>)((System.Collections.Generic.IList<FamilyIdMigration> allMax) => allMax[0]))));
		}));
	}

	[AsyncStateMachine(typeof(_003CMigrateFamilyIdToSideBySideVersionId_003Ed__130))]
	private System.Threading.Tasks.Task MigrateFamilyIdToSideBySideVersionId(string avionicsDirectory, FamilyIdMigration familyMigration)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CMigrateFamilyIdToSideBySideVersionId_003Ed__130 _003CMigrateFamilyIdToSideBySideVersionId_003Ed__ = default(_003CMigrateFamilyIdToSideBySideVersionId_003Ed__130);
		_003CMigrateFamilyIdToSideBySideVersionId_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CMigrateFamilyIdToSideBySideVersionId_003Ed__._003C_003E4__this = this;
		_003CMigrateFamilyIdToSideBySideVersionId_003Ed__.avionicsDirectory = avionicsDirectory;
		_003CMigrateFamilyIdToSideBySideVersionId_003Ed__.familyMigration = familyMigration;
		_003CMigrateFamilyIdToSideBySideVersionId_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CMigrateFamilyIdToSideBySideVersionId_003Ed__._003C_003Et__builder)).Start<_003CMigrateFamilyIdToSideBySideVersionId_003Ed__130>(ref _003CMigrateFamilyIdToSideBySideVersionId_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CMigrateFamilyIdToSideBySideVersionId_003Ed__._003C_003Et__builder)).Task;
	}

	private IObservable<SimInstallKey> ListPersistedSimulatorKeysFromDirectories(string aircraftAvionicsDirectoryPath)
	{
		string aircraftAvionicsDirectoryPath2 = aircraftAvionicsDirectoryPath;
		return ListSimDirectories(aircraftAvionicsDirectoryPath2).Select<string, SimInstallKey>((Func<string, SimInstallKey>)((string simDirPath) => SimInstallKey.Parse(PathHelpers.CreateRelativePath(aircraftAvionicsDirectoryPath2, simDirPath))));
	}

	private IObservable<string> ListSimDirectoriesContainingLegacyWinfsMountDirs(string aircraftAvionicsDirectoryPath)
	{
		return ListSimDirectories(aircraftAvionicsDirectoryPath).Where<string>((Func<string, bool>)SimDirectoryContainsLegacyWinfsDirectory);
	}

	private IObservable<string> ListSimDirectories(string aircraftAvionicsDirectoryPath)
	{
		return SafeDirectoryStream(_fs, aircraftAvionicsDirectoryPath).Where<FileSystemEntry>((Func<FileSystemEntry, bool>)IsInstallationSlotDirectoryName).SelectMany<FileSystemEntry, FileSystemEntry>((Func<FileSystemEntry, IObservable<FileSystemEntry>>)([CompilerGenerated] (FileSystemEntry installSlotDir) => ListSimDirectoriesInSlotDirectory(installSlotDir.FullPath))).Select<FileSystemEntry, string>((Func<FileSystemEntry, string>)((FileSystemEntry fse) => fse.FullPath));
	}

	private IObservable<FileSystemEntry> ListSimDirectoriesInSlotDirectory(string slotDirectoryPath)
	{
		return SafeDirectoryStream(_fs, slotDirectoryPath).Where<FileSystemEntry>((Func<FileSystemEntry, bool>)IsEmbeddedVersionReleaseDirectoryName).SelectMany<FileSystemEntry, FileSystemEntry>((Func<FileSystemEntry, IObservable<FileSystemEntry>>)([CompilerGenerated] (FileSystemEntry embeddedReleaseDir) => SafeDirectoryStream(_fs, embeddedReleaseDir.FullPath).Where<FileSystemEntry>((Func<FileSystemEntry, bool>)([CompilerGenerated] (FileSystemEntry fse) => _fs.StatIfExists(Path.Combine(fse.FullPath, "simulator.json")).Entry.EntryType == FileSystemEntryType.File))));
	}

	private bool IsEmbeddedVersionReleaseDirectoryName(FileSystemEntry fse)
	{
		return TrainerEmbeddedReleaseId.CanParse(fse.Name);
	}

	private bool SimDirectoryContainsLegacyWinfsDirectory(string directoryPath)
	{
		string path = Path.Combine(directoryPath, "winfs_mount");
		return _fs.StatIfExists(path).Entry.EntryType == FileSystemEntryType.Directory;
	}

	private string MigrateRootSlotsIntoAvionicsDirectory(TrainerIdentifier aircraftIdentifier, string aircraftRootDirectoryPath)
	{
		string text = Path.Combine(aircraftRootDirectoryPath, "avionics");
		System.Collections.Generic.IEnumerator<FileSystemEntry> enumerator = RootDirectorySlotsThatNeedMigrating(aircraftRootDirectoryPath).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				FileSystemEntry current = enumerator.Current;
				InstallationSlot installationSlot = InstallationSlot.Parse(current.Name);
				_fs.CreateDirectory(text);
				string text2 = Path.Combine(text, installationSlot.SerializedSlot);
				if (!_fs.PathExists(text2))
				{
					Logger.Log(LogLevel.Info, "Migrating {0} simulator {1}", aircraftIdentifier, installationSlot);
					_fs.Move(current.FullPath, text2);
				}
				else
				{
					string message = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Could not migrate {0} simulator {1} because it has already been migrated. Deleting the old files.", (object)aircraftIdentifier, (object)installationSlot);
					Logger.Log(LogLevel.Warning, message);
					_fs.Delete(current.FullPath);
				}
			}
			return text;
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private void MigrateSimWinfsDirectoryToWinfsZip(string simulatorDirectory)
	{
		string text = Path.Combine(simulatorDirectory, "winfs_mount");
		if (!_fs.PathExists(text))
		{
			return;
		}
		string text2 = Path.Combine(simulatorDirectory, "winfs_mount.gca");
		string text3 = text + ".could_not_migrate.gca";
		string text4 = text2;
		if (_fs.PathExists(text2))
		{
			_fs.DeleteIfExists(text3);
			text4 = text3;
		}
		try
		{
			_zipCompression.ZipDirectory(text, text4);
			if (text4 == text3)
			{
				string message = string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Could not migrate old repo based WINFS mount directory '{0}' to '{1}' because the destination already exists. Compressed it for safe keeping at {2}", (object)text, (object)text2, (object)text3);
				Logger.Log(LogLevel.Error, message);
			}
			else
			{
				Logger.Log(LogLevel.Info, "Migrated old Repo based WINFS mount '{0}' to {1}", text, text2);
			}
		}
		catch (System.Exception e)
		{
			Logger.LogFormattedException(e, "Could not compress old sim directory.");
		}
		_fs.DeleteIfExists(text);
	}

	private void DeleteAircraftDirectory(string repoAction, string aircraftDirectorypath)
	{
		LogRepoMessage(LogLevel.Info, repoAction, "Deleting Aircraft directory '{0}'", aircraftDirectorypath);
		_fs.DeleteIfExists(aircraftDirectorypath);
	}

	[AsyncStateMachine(typeof(_003CDeserializeAircraft_003Ed__140))]
	[Obsolete("Use for migration only.")]
	private async System.Threading.Tasks.Task<PersistedAircraft> DeserializeAircraft(string aircraftJsonPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return await _persistence.Load<PersistedAircraft>(aircraftJsonPath).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CPerformAircraftFormatMigrationWithLock_003Ed__143))]
	private System.Threading.Tasks.Task PerformAircraftFormatMigrationWithLock(AircraftRepoInfo repoInfo, CancellationToken token = default(CancellationToken))
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_003CPerformAircraftFormatMigrationWithLock_003Ed__143 _003CPerformAircraftFormatMigrationWithLock_003Ed__ = default(_003CPerformAircraftFormatMigrationWithLock_003Ed__143);
		_003CPerformAircraftFormatMigrationWithLock_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CPerformAircraftFormatMigrationWithLock_003Ed__._003C_003E4__this = this;
		_003CPerformAircraftFormatMigrationWithLock_003Ed__.repoInfo = repoInfo;
		_003CPerformAircraftFormatMigrationWithLock_003Ed__.token = token;
		_003CPerformAircraftFormatMigrationWithLock_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CPerformAircraftFormatMigrationWithLock_003Ed__._003C_003Et__builder)).Start<_003CPerformAircraftFormatMigrationWithLock_003Ed__143>(ref _003CPerformAircraftFormatMigrationWithLock_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CPerformAircraftFormatMigrationWithLock_003Ed__._003C_003Et__builder)).Task;
	}
}
