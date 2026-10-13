using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Compression;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LauncherCoreLib.Models.AbstractUi;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Winfs;

internal class WinfsCompressionSessionWorkflow : ISessionWorkflow
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass10_0
	{
		[StructLayout(3)]
		private struct _003C_003CPostFlight_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass10_0 _003C_003E4__this;

			private object _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass10_0 CS_0024_003C_003E8__locals0 = _003C_003E4__this;
				try
				{
					AsyncSubject<KeyValuePair<SimInstallKey, string>> asyncSubject;
					if (num != 0)
					{
						asyncSubject = ((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)CS_0024_003C_003E8__locals0.sessionEnvironment.Data.TrainerData).Values.ToObservable().SelectMany<TrainerSessionData, KeyValuePair<SimInstallKey, string>>((Func<TrainerSessionData, System.Threading.Tasks.Task<KeyValuePair<SimInstallKey, string>>>)([AsyncStateMachine(typeof(_003C_003CPostFlight_003Eb__1_003Ed))] async (TrainerSessionData tsd) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							string text = await CS_0024_003C_003E8__locals0._003C_003E4__this.CompressSimWinfsToTempArchive(tsd, CS_0024_003C_003E8__locals0.tempWinfsRootPath).DetachContext();
							return new KeyValuePair<SimInstallKey, string>(tsd.InstallData.InstallationKey, text);
						})).Where<KeyValuePair<SimInstallKey, string>>((Func<KeyValuePair<SimInstallKey, string>, bool>)((KeyValuePair<SimInstallKey, string> kv) => kv.Value != null && CS_0024_003C_003E8__locals0._003C_003E4__this._fs.PathExists(kv.Value)))
							.Do<KeyValuePair<SimInstallKey, string>>((Action<KeyValuePair<SimInstallKey, string>>)delegate(KeyValuePair<SimInstallKey, string> kv)
							{
								CS_0024_003C_003E8__locals0.winfsArchivesToUpdate.Add(kv.Key, kv.Value);
							})
							.GetAwaiter<KeyValuePair<SimInstallKey, string>>();
						if (!asyncSubject.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = asyncSubject;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<KeyValuePair<SimInstallKey, string>>, _003C_003CPostFlight_003Eb__0_003Ed>(ref asyncSubject, ref this);
							return;
						}
					}
					else
					{
						asyncSubject = (AsyncSubject<KeyValuePair<SimInstallKey, string>>)_003C_003Eu__1;
						_003C_003Eu__1 = null;
						num = (_003C_003E1__state = -1);
					}
					asyncSubject.GetResult();
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
		private struct _003C_003CPostFlight_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<KeyValuePair<SimInstallKey, string>> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass10_0 _003C_003E4__this;

			public TrainerSessionData tsd;

			private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass10_0 _003C_003Ec__DisplayClass10_ = _003C_003E4__this;
				KeyValuePair<SimInstallKey, string> result2;
				try
				{
					ConfiguredTaskAwaiter<string> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass10_._003C_003E4__this.CompressSimWinfsToTempArchive(tsd, _003C_003Ec__DisplayClass10_.tempWinfsRootPath).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003C_003CPostFlight_003Eb__1_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
						num = (_003C_003E1__state = -1);
					}
					string result = awaiter.GetResult();
					result2 = new KeyValuePair<SimInstallKey, string>(tsd.InstallData.InstallationKey, result);
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

		public ISessionEnvironment sessionEnvironment;

		public WinfsCompressionSessionWorkflow _003C_003E4__this;

		public string tempWinfsRootPath;

		public Dictionary<SimInstallKey, string> winfsArchivesToUpdate;

		public Func<TrainerSessionData, System.Threading.Tasks.Task<KeyValuePair<SimInstallKey, string>>> _003C_003E9__1;

		public Func<KeyValuePair<SimInstallKey, string>, bool> _003C_003E9__2;

		public Action<KeyValuePair<SimInstallKey, string>> _003C_003E9__3;

		[AsyncStateMachine(typeof(_003C_003CPostFlight_003Eb__0_003Ed))]
		internal System.Threading.Tasks.Task _003CPostFlight_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPostFlight_003Eb__0_003Ed _003C_003CPostFlight_003Eb__0_003Ed = default(_003C_003CPostFlight_003Eb__0_003Ed);
			_003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPostFlight_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CPostFlight_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CPostFlight_003Eb__0_003Ed>(ref _003C_003CPostFlight_003Eb__0_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Task;
		}

		[AsyncStateMachine(typeof(_003C_003CPostFlight_003Eb__1_003Ed))]
		internal async System.Threading.Tasks.Task<KeyValuePair<SimInstallKey, string>> _003CPostFlight_003Eb__1(TrainerSessionData tsd)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string text = await _003C_003E4__this.CompressSimWinfsToTempArchive(tsd, tempWinfsRootPath).DetachContext();
			return new KeyValuePair<SimInstallKey, string>(tsd.InstallData.InstallationKey, text);
		}

		internal bool _003CPostFlight_003Eb__2(KeyValuePair<SimInstallKey, string> kv)
		{
			if (kv.Value != null)
			{
				return _003C_003E4__this._fs.PathExists(kv.Value);
			}
			return false;
		}

		internal void _003CPostFlight_003Eb__3(KeyValuePair<SimInstallKey, string> kv)
		{
			winfsArchivesToUpdate.Add(kv.Key, kv.Value);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass16_0
	{
		public WinfsCompressionSessionWorkflow _003C_003E4__this;

		public string simCompressedWinfsPath;

		public string uncompressedWinfsMountPath;

		internal void _003CPrepareSim_003Eb__0()
		{
			_003C_003E4__this._zipCompression.UnZip(simCompressedWinfsPath, uncompressedWinfsMountPath);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass17_0
	{
		public WinfsCompressionSessionWorkflow _003C_003E4__this;

		public string winfsMountPath;

		public string compressedWinfsTempPath;

		internal void _003CCompressSimWinfsToTempArchive_003Eb__0()
		{
			_003C_003E4__this._zipCompression.ZipDirectory(winfsMountPath, compressedWinfsTempPath);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		[StructLayout(3)]
		private struct _003C_003CPreFlight_003Eb__2_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass9_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0094: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass9_0 CS_0024_003C_003E8__locals0 = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<TrainerSessionData, System.Threading.Tasks.Task>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)CS_0024_003C_003E8__locals0.sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, System.Threading.Tasks.Task>)delegate(TrainerSessionData tsd)
						{
							SimInstallKey installationKey = tsd.InstallData.InstallationKey;
							if (((System.Collections.Generic.ICollection<SimInstallKey>)CS_0024_003C_003E8__locals0.simsWithAnArchiveWinfs).Contains(installationKey))
							{
								if (CS_0024_003C_003E8__locals0.winfsArchiveMap.ContainsKey(installationKey))
								{
									string simCompressedWinfsPath = CS_0024_003C_003E8__locals0.winfsArchiveMap[installationKey];
									return CS_0024_003C_003E8__locals0._003C_003E4__this.PrepareSim(tsd, simCompressedWinfsPath);
								}
								Logger.Log(LogLevel.Error, "the {0} sim instance was not found in the winfs map.", tsd.EndpointName);
							}
							return System.Threading.Tasks.Task.CompletedTask;
						})).DetachContext();
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CPreFlight_003Eb__2_003Ed>(ref awaiter, ref this);
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

		public string tempWinfsRootPath;

		public ISessionEnvironment sessionEnvironment;

		public System.Collections.Generic.IList<SimInstallKey> simsWithAnArchiveWinfs;

		public Dictionary<SimInstallKey, string> winfsArchiveMap;

		public WinfsCompressionSessionWorkflow _003C_003E4__this;

		public Func<TrainerSessionData, System.Threading.Tasks.Task> _003C_003E9__3;

		internal string _003CPreFlight_003Eb__1(TrainerSessionData sim)
		{
			return Path.Combine(tempWinfsRootPath, (string?)sim.EndpointName + "_winfs.gca");
		}

		[AsyncStateMachine(typeof(_003C_003CPreFlight_003Eb__2_003Ed))]
		internal System.Threading.Tasks.Task _003CPreFlight_003Eb__2()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CPreFlight_003Eb__2_003Ed _003C_003CPreFlight_003Eb__2_003Ed = default(_003C_003CPreFlight_003Eb__2_003Ed);
			_003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPreFlight_003Eb__2_003Ed._003C_003E4__this = this;
			_003C_003CPreFlight_003Eb__2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003CPreFlight_003Eb__2_003Ed>(ref _003C_003CPreFlight_003Eb__2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder)).Task;
		}

		internal System.Threading.Tasks.Task _003CPreFlight_003Eb__3(TrainerSessionData tsd)
		{
			SimInstallKey installationKey = tsd.InstallData.InstallationKey;
			if (((System.Collections.Generic.ICollection<SimInstallKey>)simsWithAnArchiveWinfs).Contains(installationKey))
			{
				if (winfsArchiveMap.ContainsKey(installationKey))
				{
					string simCompressedWinfsPath = winfsArchiveMap[installationKey];
					return _003C_003E4__this.PrepareSim(tsd, simCompressedWinfsPath);
				}
				Logger.Log(LogLevel.Error, "the {0} sim instance was not found in the winfs map.", tsd.EndpointName);
			}
			return System.Threading.Tasks.Task.CompletedTask;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCompressSimWinfsToTempArchive_003Ed__17 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

		public WinfsCompressionSessionWorkflow _003C_003E4__this;

		public TrainerSessionData tsd;

		private _003C_003Ec__DisplayClass17_0 _003C_003E8__1;

		public string tempWinfsArchivePath;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Expected O, but got Unknown
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			WinfsCompressionSessionWorkflow winfsCompressionSessionWorkflow = _003C_003E4__this;
			string result;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_00c0;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass17_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__1.winfsMountPath = tsd.WinfsMountPath;
				if (winfsCompressionSessionWorkflow._fs.PathExists(_003C_003E8__1.winfsMountPath))
				{
					ConfiguredTaskAwaitable val = YieldToMoveOntoThreadPool().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CCompressSimWinfsToTempArchive_003Ed__17>(ref awaiter, ref this);
						return;
					}
					goto IL_00c0;
				}
				result = null;
				goto end_IL_000e;
				IL_00c0:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				_003C_003E8__1.compressedWinfsTempPath = Path.Combine(tempWinfsArchivePath, (string?)tsd.EndpointName + "_winfs.gca");
				Logger.Log(LogLevel.Info, "Compressing WINFS mount files into '{0}'", _003C_003E8__1.compressedWinfsTempPath);
				winfsCompressionSessionWorkflow._fs.DeleteIfExists(_003C_003E8__1.compressedWinfsTempPath);
				TimeSpan val2 = WinfsCompressionSessionWorkflow.TimeTask((Action)delegate
				{
					_003C_003E8__1._003C_003E4__this._zipCompression.ZipDirectory(_003C_003E8__1.winfsMountPath, _003C_003E8__1.compressedWinfsTempPath);
				});
				Logger.Log(LogLevel.Info, CompressionStat("Spent {0} zipping {1}"), ((TimeSpan)(ref val2)).ToString("mm\\:ss\\.fff", (IFormatProvider)(object)CultureInfo.InvariantCulture), tsd.EndpointName);
				object cleanupLock = winfsCompressionSessionWorkflow._cleanupLock;
				bool flag = false;
				try
				{
					Monitor.Enter(cleanupLock, ref flag);
					winfsCompressionSessionWorkflow._fs.DeleteIfExistsAndParentsThatAreEmpty(_003C_003E8__1.winfsMountPath, winfsCompressionSessionWorkflow._deployment.AsciiSafeDirectory());
				}
				finally
				{
					if (num < 0 && flag)
					{
						Monitor.Exit(cleanupLock);
					}
				}
				if (winfsCompressionSessionWorkflow._fs.PathExists(_003C_003E8__1.winfsMountPath))
				{
					Logger.Log(LogLevel.Error, "The " + _003C_003E8__1.winfsMountPath + " directory was left on disk. This could cause problems for subsequent simulators that reuse the location.");
					throw new IOException("Could not clean up the temporary directory: " + PathHelpers.QuotePathIfNeeded(winfsCompressionSessionWorkflow._deployment.AsciiSafeDirectory()) + "\nPlease ensure:\n    1. Sub-folders are not open in Windows File Explorer or its navigation tree.\n    2. Files under that directory are closed in file editors.\n    3. Anti-Virus/Malware software is not currently scanning.\nCause:");
				}
				result = _003C_003E8__1.compressedWinfsTempPath;
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
	private struct _003CPostFlight_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public ISessionEnvironment sessionEnvironment;

		public WinfsCompressionSessionWorkflow _003C_003E4__this;

		private _003C_003Ec__DisplayClass10_0 _003C_003E8__1;

		private System.Exception _003CcompressionError_003E5__2;

		private ConfiguredTaskAwaiter<TimeSpan> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			WinfsCompressionSessionWorkflow winfsCompressionSessionWorkflow = _003C_003E4__this;
			ISessionEnvironment result2;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_01de;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass10_0();
					_003C_003E8__1.sessionEnvironment = sessionEnvironment;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.tempWinfsRootPath = Path.Combine(_003C_003E8__1.sessionEnvironment.Inputs.SessionPathRoot, "winfs", "postflight");
					winfsCompressionSessionWorkflow._fs.CreateDirectory(_003C_003E8__1.tempWinfsRootPath);
					_003C_003E8__1.winfsArchivesToUpdate = new Dictionary<SimInstallKey, string>();
					_003CcompressionError_003E5__2 = null;
				}
				try
				{
					ConfiguredTaskAwaiter<TimeSpan> awaiter2;
					if (num != 0)
					{
						awaiter2 = TimeTask((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass10_0._003C_003CPostFlight_003Eb__0_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass10_0._003C_003CPostFlight_003Eb__0_003Ed _003C_003CPostFlight_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass10_0._003C_003CPostFlight_003Eb__0_003Ed);
							_003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CPostFlight_003Eb__0_003Ed._003C_003E4__this = _003C_003E8__1;
							_003C_003CPostFlight_003Eb__0_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass10_0._003C_003CPostFlight_003Eb__0_003Ed>(ref _003C_003CPostFlight_003Eb__0_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Task;
						})).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TimeSpan>, _003CPostFlight_003Ed__10>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<TimeSpan>);
						num = (_003C_003E1__state = -1);
					}
					TimeSpan result = awaiter2.GetResult();
					Logger.Log(LogLevel.Info, CompressionStat("Spent {0} zipping all simulators"), ((TimeSpan)(ref result)).ToString("mm\\:ss\\.fff", (IFormatProvider)(object)CultureInfo.InvariantCulture));
				}
				catch (System.Exception ex)
				{
					_003CcompressionError_003E5__2 = ex;
				}
				ConfiguredTaskAwaitable val = winfsCompressionSessionWorkflow._unitOfWork.Simulators.UpdateAircraftWinfsArchives(GetAircraftIdentifier(_003C_003E8__1.sessionEnvironment), _003C_003E8__1.winfsArchivesToUpdate).DetachContext();
				awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
				if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPostFlight_003Ed__10>(ref awaiter, ref this);
					return;
				}
				goto IL_01de;
				IL_01de:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				if (_003CcompressionError_003E5__2 != null)
				{
					throw new InvalidOperationException("Not all changes made to avionics could be saved. You may notice some differences on your next run.", _003CcompressionError_003E5__2);
				}
				result2 = _003C_003E8__1.sessionEnvironment;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CcompressionError_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CcompressionError_003E5__2 = null;
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
	private struct _003CPreFlight_003Ed__9 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISessionEnvironment> _003C_003Et__builder;

		public ISessionEnvironment sessionEnvironment;

		public WinfsCompressionSessionWorkflow _003C_003E4__this;

		private _003C_003Ec__DisplayClass9_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<TimeSpan> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			WinfsCompressionSessionWorkflow winfsCompressionSessionWorkflow = _003C_003E4__this;
			ISessionEnvironment result3;
			try
			{
				ConfiguredTaskAwaiter<TimeSpan> awaiter;
				ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<TimeSpan>);
						num = (_003C_003E1__state = -1);
						goto IL_01f3;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass9_0();
					_003C_003E8__1.sessionEnvironment = sessionEnvironment;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.tempWinfsRootPath = Path.Combine(_003C_003E8__1.sessionEnvironment.Inputs.SessionPathRoot, "winfs", "preflight");
					winfsCompressionSessionWorkflow._fs.CreateDirectory(_003C_003E8__1.tempWinfsRootPath);
					_003C_003E8__1.winfsArchiveMap = Enumerable.ToDictionary<TrainerSessionData, SimInstallKey, string>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)_003C_003E8__1.sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, SimInstallKey>)((TrainerSessionData sim) => sim.InstallData.InstallationKey), (Func<TrainerSessionData, string>)((TrainerSessionData sim) => Path.Combine(_003C_003E8__1.tempWinfsRootPath, (string?)sim.EndpointName + "_winfs.gca")));
					awaiter2 = winfsCompressionSessionWorkflow._unitOfWork.Simulators.GetAircraftWinfsArchives(GetAircraftIdentifier(_003C_003E8__1.sessionEnvironment), _003C_003E8__1.winfsArchiveMap).DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>>, _003CPreFlight_003Ed__9>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IList<SimInstallKey>>);
					num = (_003C_003E1__state = -1);
				}
				System.Collections.Generic.IList<SimInstallKey> result = awaiter2.GetResult();
				_003C_003E8__1.simsWithAnArchiveWinfs = result;
				awaiter = TimeTask((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_0._003C_003CPreFlight_003Eb__2_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass9_0._003C_003CPreFlight_003Eb__2_003Ed _003C_003CPreFlight_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass9_0._003C_003CPreFlight_003Eb__2_003Ed);
					_003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
					_003C_003CPreFlight_003Eb__2_003Ed._003C_003E4__this = _003C_003E8__1;
					_003C_003CPreFlight_003Eb__2_003Ed._003C_003E1__state = -1;
					((AsyncTaskMethodBuilder)(ref _003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass9_0._003C_003CPreFlight_003Eb__2_003Ed>(ref _003C_003CPreFlight_003Eb__2_003Ed);
					return ((AsyncTaskMethodBuilder)(ref _003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder)).Task;
				})).DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<TimeSpan>, _003CPreFlight_003Ed__9>(ref awaiter, ref this);
					return;
				}
				goto IL_01f3;
				IL_01f3:
				TimeSpan result2 = awaiter.GetResult();
				Logger.Log(LogLevel.Info, CompressionStat("Spent {0} unzipping all simulators"), ((TimeSpan)(ref result2)).ToString("mm\\:ss\\.fff", (IFormatProvider)(object)CultureInfo.InvariantCulture));
				result3 = _003C_003E8__1.sessionEnvironment;
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
	private struct _003CPrepareSim_003Ed__16 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public WinfsCompressionSessionWorkflow _003C_003E4__this;

		public string simCompressedWinfsPath;

		public TrainerSessionData tsd;

		private _003C_003Ec__DisplayClass16_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_019d: Expected O, but got Unknown
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Expected O, but got Unknown
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			WinfsCompressionSessionWorkflow winfsCompressionSessionWorkflow = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0106;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass16_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__1.simCompressedWinfsPath = simCompressedWinfsPath;
				_003C_003E8__1.uncompressedWinfsMountPath = tsd.WinfsMountPath;
				if (winfsCompressionSessionWorkflow._fs.PathExists(_003C_003E8__1.simCompressedWinfsPath) && (!winfsCompressionSessionWorkflow._fs.PathExists(_003C_003E8__1.uncompressedWinfsMountPath) || !Enumerable.Any<FileSystemEntry>(winfsCompressionSessionWorkflow._fs.RecursiveFilesIn(_003C_003E8__1.uncompressedWinfsMountPath))))
				{
					ConfiguredTaskAwaitable val = YieldToMoveOntoThreadPool().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CPrepareSim_003Ed__16>(ref awaiter, ref this);
						return;
					}
					goto IL_0106;
				}
				goto end_IL_000e;
				IL_0106:
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				Logger.Log(LogLevel.Info, "Uncompressing previous WINFS mount files from '{0}'", _003C_003E8__1.simCompressedWinfsPath);
				winfsCompressionSessionWorkflow._fs.CreateDirectory(_003C_003E8__1.uncompressedWinfsMountPath);
				TimeSpan val2;
				try
				{
					val2 = WinfsCompressionSessionWorkflow.TimeTask((Action)delegate
					{
						_003C_003E8__1._003C_003E4__this._zipCompression.UnZip(_003C_003E8__1.simCompressedWinfsPath, _003C_003E8__1.uncompressedWinfsMountPath);
					});
				}
				catch (InvalidDataException val3)
				{
					Logger.LogFormattedException((System.Exception)val3, $"Corrupt simulator archive: {tsd.InstallData.AircraftSimKey}");
					throw new IOException($"Corrupt {winfsCompressionSessionWorkflow._significantNounProvider.GetTrainerNoun().Singular} data detected. Please reset the {winfsCompressionSessionWorkflow._significantNounProvider.GetTrainerNoun().Singular} in the {tsd.InstallSlot.ToUiString()} slot and then retry power up to continue.");
				}
				Logger.Log(LogLevel.Info, CompressionStat("Spent {0} unzipping {1}"), ((TimeSpan)(ref val2)).ToString("mm\\:ss\\.fff", (IFormatProvider)(object)CultureInfo.InvariantCulture), tsd.EndpointName);
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
	[CompilerGenerated]
	private struct _003CTimeTask_003Ed__15 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<TimeSpan> _003C_003Et__builder;

		public Func<System.Threading.Tasks.Task> executeTask;

		private DateTimeOffset _003Cstart_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			TimeSpan result;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					_003Cstart_003E5__2 = DateTimeOffset.Now;
					ConfiguredTaskAwaitable val = executeTask.Invoke().DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CTimeTask_003Ed__15>(ref awaiter, ref this);
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
				result = DateTimeOffset.Now - _003Cstart_003E5__2;
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
	private struct _003CYieldToMoveOntoThreadPool_003Ed__12 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.Delay(1).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CYieldToMoveOntoThreadPool_003Ed__12>(ref awaiter, ref this);
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

	private const string SESSION_WINFS_DIR_NAME = "winfs";

	private const string MINUTE_SECOND_FRACTION_FORMAT = "mm\\:ss\\.fff";

	private readonly IDeploymentPathProvider _deployment;

	private readonly IFileSystem _fs;

	private readonly IZipCompression _zipCompression;

	private readonly IUnitOfWork _unitOfWork;

	private readonly ISignificantNounProvider _significantNounProvider;

	private readonly object _cleanupLock = new object();

	public WinfsCompressionSessionWorkflow(IDeploymentPathProvider deploymentPathProvider, IFileSystem fs, IZipCompression zipCompression, IUnitOfWork unitOfWork, ISignificantNounProvider significantNounProvider)
	{
		_deployment = deploymentPathProvider;
		_fs = fs;
		_zipCompression = zipCompression;
		_unitOfWork = unitOfWork;
		_significantNounProvider = significantNounProvider;
	}

	[AsyncStateMachine(typeof(_003CPreFlight_003Ed__9))]
	public async System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass9_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass9_0();
		CS_0024_003C_003E8__locals0.sessionEnvironment = sessionEnvironment;
		CS_0024_003C_003E8__locals0._003C_003E4__this = this;
		CS_0024_003C_003E8__locals0.tempWinfsRootPath = Path.Combine(CS_0024_003C_003E8__locals0.sessionEnvironment.Inputs.SessionPathRoot, "winfs", "preflight");
		_fs.CreateDirectory(CS_0024_003C_003E8__locals0.tempWinfsRootPath);
		CS_0024_003C_003E8__locals0.winfsArchiveMap = Enumerable.ToDictionary<TrainerSessionData, SimInstallKey, string>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)CS_0024_003C_003E8__locals0.sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, SimInstallKey>)((TrainerSessionData sim) => sim.InstallData.InstallationKey), (Func<TrainerSessionData, string>)((TrainerSessionData sim) => Path.Combine(CS_0024_003C_003E8__locals0.tempWinfsRootPath, (string?)sim.EndpointName + "_winfs.gca")));
		CS_0024_003C_003E8__locals0.simsWithAnArchiveWinfs = await _unitOfWork.Simulators.GetAircraftWinfsArchives(GetAircraftIdentifier(CS_0024_003C_003E8__locals0.sessionEnvironment), CS_0024_003C_003E8__locals0.winfsArchiveMap).DetachContext();
		TimeSpan val = await TimeTask((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass9_0._003C_003CPreFlight_003Eb__2_003Ed))] () =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass9_0._003C_003CPreFlight_003Eb__2_003Ed _003C_003CPreFlight_003Eb__2_003Ed = default(_003C_003Ec__DisplayClass9_0._003C_003CPreFlight_003Eb__2_003Ed);
			_003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CPreFlight_003Eb__2_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
			_003C_003CPreFlight_003Eb__2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass9_0._003C_003CPreFlight_003Eb__2_003Ed>(ref _003C_003CPreFlight_003Eb__2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CPreFlight_003Eb__2_003Ed._003C_003Et__builder)).Task;
		})).DetachContext();
		Logger.Log(LogLevel.Info, CompressionStat("Spent {0} unzipping all simulators"), ((TimeSpan)(ref val)).ToString("mm\\:ss\\.fff", (IFormatProvider)(object)CultureInfo.InvariantCulture));
		return CS_0024_003C_003E8__locals0.sessionEnvironment;
	}

	[AsyncStateMachine(typeof(_003CPostFlight_003Ed__10))]
	public async System.Threading.Tasks.Task<ISessionEnvironment> PostFlight(ISessionEnvironment sessionEnvironment)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass10_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass10_0();
		CS_0024_003C_003E8__locals0.sessionEnvironment = sessionEnvironment;
		CS_0024_003C_003E8__locals0._003C_003E4__this = this;
		CS_0024_003C_003E8__locals0.tempWinfsRootPath = Path.Combine(CS_0024_003C_003E8__locals0.sessionEnvironment.Inputs.SessionPathRoot, "winfs", "postflight");
		_fs.CreateDirectory(CS_0024_003C_003E8__locals0.tempWinfsRootPath);
		CS_0024_003C_003E8__locals0.winfsArchivesToUpdate = new Dictionary<SimInstallKey, string>();
		System.Exception compressionError = null;
		try
		{
			TimeSpan val = await TimeTask((Func<System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass10_0._003C_003CPostFlight_003Eb__0_003Ed))] () =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass10_0._003C_003CPostFlight_003Eb__0_003Ed _003C_003CPostFlight_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass10_0._003C_003CPostFlight_003Eb__0_003Ed);
				_003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CPostFlight_003Eb__0_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
				_003C_003CPostFlight_003Eb__0_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass10_0._003C_003CPostFlight_003Eb__0_003Ed>(ref _003C_003CPostFlight_003Eb__0_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CPostFlight_003Eb__0_003Ed._003C_003Et__builder)).Task;
			})).DetachContext();
			Logger.Log(LogLevel.Info, CompressionStat("Spent {0} zipping all simulators"), ((TimeSpan)(ref val)).ToString("mm\\:ss\\.fff", (IFormatProvider)(object)CultureInfo.InvariantCulture));
		}
		catch (System.Exception ex)
		{
			compressionError = ex;
		}
		await _unitOfWork.Simulators.UpdateAircraftWinfsArchives(GetAircraftIdentifier(CS_0024_003C_003E8__locals0.sessionEnvironment), CS_0024_003C_003E8__locals0.winfsArchivesToUpdate).DetachContext();
		if (compressionError != null)
		{
			throw new InvalidOperationException("Not all changes made to avionics could be saved. You may notice some differences on your next run.", compressionError);
		}
		return CS_0024_003C_003E8__locals0.sessionEnvironment;
	}

	private static AircraftIdentifier GetAircraftIdentifier(ISessionEnvironment sessionEnvironment)
	{
		return sessionEnvironment.Inputs.CreationRequestOrThrow().AircraftSnapShot.AircraftIdentifier;
	}

	[AsyncStateMachine(typeof(_003CYieldToMoveOntoThreadPool_003Ed__12))]
	private static System.Threading.Tasks.Task YieldToMoveOntoThreadPool()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CYieldToMoveOntoThreadPool_003Ed__12 _003CYieldToMoveOntoThreadPool_003Ed__ = default(_003CYieldToMoveOntoThreadPool_003Ed__12);
		_003CYieldToMoveOntoThreadPool_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CYieldToMoveOntoThreadPool_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CYieldToMoveOntoThreadPool_003Ed__._003C_003Et__builder)).Start<_003CYieldToMoveOntoThreadPool_003Ed__12>(ref _003CYieldToMoveOntoThreadPool_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CYieldToMoveOntoThreadPool_003Ed__._003C_003Et__builder)).Task;
	}

	private static string CompressionStat(string statString)
	{
		return "[Compression Stats] " + statString;
	}

	private static TimeSpan TimeTask(Action executeTask)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		DateTimeOffset now = DateTimeOffset.Now;
		executeTask.Invoke();
		return DateTimeOffset.Now - now;
	}

	[AsyncStateMachine(typeof(_003CTimeTask_003Ed__15))]
	private static async System.Threading.Tasks.Task<TimeSpan> TimeTask(Func<System.Threading.Tasks.Task> executeTask)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DateTimeOffset start = DateTimeOffset.Now;
		await executeTask.Invoke().DetachContext();
		return DateTimeOffset.Now - start;
	}

	[AsyncStateMachine(typeof(_003CPrepareSim_003Ed__16))]
	private System.Threading.Tasks.Task PrepareSim(TrainerSessionData tsd, string simCompressedWinfsPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CPrepareSim_003Ed__16 _003CPrepareSim_003Ed__ = default(_003CPrepareSim_003Ed__16);
		_003CPrepareSim_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CPrepareSim_003Ed__._003C_003E4__this = this;
		_003CPrepareSim_003Ed__.tsd = tsd;
		_003CPrepareSim_003Ed__.simCompressedWinfsPath = simCompressedWinfsPath;
		_003CPrepareSim_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CPrepareSim_003Ed__._003C_003Et__builder)).Start<_003CPrepareSim_003Ed__16>(ref _003CPrepareSim_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CPrepareSim_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CCompressSimWinfsToTempArchive_003Ed__17))]
	private async System.Threading.Tasks.Task<string?> CompressSimWinfsToTempArchive(TrainerSessionData tsd, string tempWinfsArchivePath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string winfsMountPath = tsd.WinfsMountPath;
		if (_fs.PathExists(winfsMountPath))
		{
			await YieldToMoveOntoThreadPool().DetachContext();
			string compressedWinfsTempPath = Path.Combine(tempWinfsArchivePath, (string?)tsd.EndpointName + "_winfs.gca");
			Logger.Log(LogLevel.Info, "Compressing WINFS mount files into '{0}'", compressedWinfsTempPath);
			_fs.DeleteIfExists(compressedWinfsTempPath);
			TimeSpan val = TimeTask((Action)delegate
			{
				_zipCompression.ZipDirectory(winfsMountPath, compressedWinfsTempPath);
			});
			Logger.Log(LogLevel.Info, CompressionStat("Spent {0} zipping {1}"), ((TimeSpan)(ref val)).ToString("mm\\:ss\\.fff", (IFormatProvider)(object)CultureInfo.InvariantCulture), tsd.EndpointName);
			lock (_cleanupLock)
			{
				_fs.DeleteIfExistsAndParentsThatAreEmpty(winfsMountPath, _deployment.AsciiSafeDirectory());
			}
			if (_fs.PathExists(winfsMountPath))
			{
				Logger.Log(LogLevel.Error, "The " + winfsMountPath + " directory was left on disk. This could cause problems for subsequent simulators that reuse the location.");
				throw new IOException("Could not clean up the temporary directory: " + PathHelpers.QuotePathIfNeeded(_deployment.AsciiSafeDirectory()) + "\nPlease ensure:\n    1. Sub-folders are not open in Windows File Explorer or its navigation tree.\n    2. Files under that directory are closed in file editors.\n    3. Anti-Virus/Malware software is not currently scanning.\nCause:");
			}
			return compressedWinfsTempPath;
		}
		return null;
	}
}
