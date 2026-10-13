using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GTI.Net;
using GtiArtifacts;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LanguageExt;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Models.EventLogging;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.TrainerPackages;

public class AutoInitCatalogStartup : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	private enum AutoInitState
	{
		MissingCatalog,
		NeedsRepair,
		CatalogLooksOld,
		Ready
	}

	private enum CatalogRepairMethod
	{
		Update,
		ReInit
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public AutoInitCatalogStartup _003C_003E4__this;

		public IGtiRunner gtiRunner;

		public CancellationToken shutdown;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_1
	{
		public bool succeeded;

		public CatalogRepairMethod? successfulMethod;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{
		[StructLayout(3)]
		private struct _003C_003CDetermineState_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<AutoInitState> _003C_003Et__builder;

			public GtiListingEntry catalogEntry;

			public _003C_003Ec__DisplayClass8_0 _003C_003E4__this;

			private _003C_003Ec__DisplayClass8_1 _003C_003E8__1;

			private Stream _003CgticStream_003E5__2;

			private ConfiguredTaskAwaiter<Either<GtiArtifactError, CatalogInfo>> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass8_0 CS_0024_003C_003E8__locals0 = _003C_003E4__this;
				AutoInitState result;
				try
				{
					if (num == 0)
					{
						goto IL_0067;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass8_1();
					_003C_003E8__1.catalogEntry = catalogEntry;
					if (_003C_003E8__1.catalogEntry.InstalledVersion != null)
					{
						_003CgticStream_003E5__2 = CS_0024_003C_003E8__locals0._003C_003E4__this._fs.Open(CS_0024_003C_003E8__locals0.gticPath, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)0);
						goto IL_0067;
					}
					result = AutoInitState.NeedsRepair;
					goto end_IL_000e;
					IL_0067:
					try
					{
						ConfiguredTaskAwaiter<Either<GtiArtifactError, CatalogInfo>> awaiter;
						if (num != 0)
						{
							awaiter = GtiOfflineArchive.ParseFromStream(_003CgticStream_003E5__2, CS_0024_003C_003E8__locals0.shutdown).Use<CatalogInfo>((Func<GtiOfflineArchive, EitherAsync<GtiArtifactError, CatalogInfo>>)((GtiOfflineArchive archive) => archive.ReadCatalogInfoFile(CS_0024_003C_003E8__locals0.shutdown).Bind<CatalogInfo>((string infoFileText) => GtiArtifactUtility.ParseCatalogInfo(infoFileText).ToEither().MapLeft<GtiArtifactError>((Seq<GtiArtifactError> e) => e.Head)
								.ToAsync()))).ToEither()
								.ConfigureAwait(false)
								.GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Either<GtiArtifactError, CatalogInfo>>, _003C_003CDetermineState_003Eb__1_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<Either<GtiArtifactError, CatalogInfo>>);
							num = (_003C_003E1__state = -1);
						}
						result = awaiter.GetResult().Map<AutoInitState>(delegate(CatalogInfo catalogInfo)
						{
							if (_003C_003E8__1.catalogEntry.InstalledVersion.Major < 3)
							{
								return AutoInitState.CatalogLooksOld;
							}
							return (catalogInfo.CatalogVersion > _003C_003E8__1.catalogEntry.InstalledVersion) ? AutoInitState.CatalogLooksOld : AutoInitState.Ready;
						}).IfLeft((Func<GtiArtifactError, AutoInitState>)delegate(GtiArtifactError error)
						{
							Logger.Log(LogLevel.Error, $"The included catalog cannot be parsed ({error}) and cannot be used to repair GTI. Proceeding as if the installed catalog is valid and correct.");
							return AutoInitState.Ready;
						});
					}
					finally
					{
						if (num < 0 && _003CgticStream_003E5__2 != null)
						{
							((System.IDisposable)_003CgticStream_003E5__2).Dispose();
						}
					}
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003CgticStream_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CgticStream_003E5__2 = null;
				_003C_003Et__builder.SetResult(result);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public AutoInitCatalogStartup _003C_003E4__this;

		public string gticPath;

		public CancellationToken shutdown;

		public IGtiRunner gtiRunner;

		public Func<GtiOfflineArchive, EitherAsync<GtiArtifactError, CatalogInfo>> _003C_003E9__5;

		[AsyncStateMachine(typeof(_003C_003CDetermineState_003Eb__1_003Ed))]
		internal async System.Threading.Tasks.Task<AutoInitState> _003CDetermineState_003Eb__1(GtiListingEntry catalogEntry)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass8_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass8_1
			{
				catalogEntry = catalogEntry
			};
			if (CS_0024_003C_003E8__locals0.catalogEntry.InstalledVersion == null)
			{
				return AutoInitState.NeedsRepair;
			}
			Stream gticStream = _003C_003E4__this._fs.Open(gticPath, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)0);
			try
			{
				return (await GtiOfflineArchive.ParseFromStream(gticStream, shutdown).Use<CatalogInfo>((Func<GtiOfflineArchive, EitherAsync<GtiArtifactError, CatalogInfo>>)((GtiOfflineArchive archive) => archive.ReadCatalogInfoFile(shutdown).Bind<CatalogInfo>((string infoFileText) => GtiArtifactUtility.ParseCatalogInfo(infoFileText).ToEither().MapLeft<GtiArtifactError>((Seq<GtiArtifactError> e) => e.Head)
					.ToAsync()))).ToEither()
					.ConfigureAwait(false)).Map<AutoInitState>(delegate(CatalogInfo catalogInfo)
				{
					if (CS_0024_003C_003E8__locals0.catalogEntry.InstalledVersion.Major < 3)
					{
						return AutoInitState.CatalogLooksOld;
					}
					return (catalogInfo.CatalogVersion > CS_0024_003C_003E8__locals0.catalogEntry.InstalledVersion) ? AutoInitState.CatalogLooksOld : AutoInitState.Ready;
				}).IfLeft((Func<GtiArtifactError, AutoInitState>)delegate(GtiArtifactError error)
				{
					Logger.Log(LogLevel.Error, $"The included catalog cannot be parsed ({error}) and cannot be used to repair GTI. Proceeding as if the installed catalog is valid and correct.");
					return AutoInitState.Ready;
				});
			}
			finally
			{
				((System.IDisposable)gticStream)?.Dispose();
			}
		}

		internal EitherAsync<GtiArtifactError, CatalogInfo> _003CDetermineState_003Eb__5(GtiOfflineArchive archive)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return archive.ReadCatalogInfoFile(shutdown).Bind<CatalogInfo>((string infoFileText) => GtiArtifactUtility.ParseCatalogInfo(infoFileText).ToEither().MapLeft<GtiArtifactError>((Seq<GtiArtifactError> e) => e.Head)
				.ToAsync());
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_1
	{
		public GtiListingEntry catalogEntry;

		internal AutoInitState _003CDetermineState_003Eb__3(CatalogInfo catalogInfo)
		{
			if (catalogEntry.InstalledVersion.Major < 3)
			{
				return AutoInitState.CatalogLooksOld;
			}
			if (catalogInfo.CatalogVersion > catalogEntry.InstalledVersion)
			{
				return AutoInitState.CatalogLooksOld;
			}
			return AutoInitState.Ready;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CDetermineState_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AutoInitState> _003C_003Et__builder;

		public AutoInitCatalogStartup _003C_003E4__this;

		public string gticPath;

		public CancellationToken shutdown;

		public IGtiRunner gtiRunner;

		private ConfiguredTaskAwaiter<AutoInitState> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AutoInitState result;
			try
			{
				ConfiguredTaskAwaiter<AutoInitState> awaiter;
				if (num != 0)
				{
					_003C_003Ec__DisplayClass8_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass8_0
					{
						_003C_003E4__this = _003C_003E4__this,
						gticPath = gticPath,
						shutdown = shutdown,
						gtiRunner = gtiRunner
					};
					awaiter = CS_0024_003C_003E8__locals0._003CDetermineState_003Eg__GetCatalogEntryOrTerminalState_007C0(3, 1).ToAsync().MatchAsync<AutoInitState>((Func<GtiListingEntry, System.Threading.Tasks.Task<AutoInitState>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CDetermineState_003Eb__1_003Ed))] async (GtiListingEntry catalogEntry) =>
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						_003C_003Ec__DisplayClass8_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass8_1
						{
							catalogEntry = catalogEntry
						};
						if (CS_0024_003C_003E8__locals1.catalogEntry.InstalledVersion == null)
						{
							return AutoInitState.NeedsRepair;
						}
						Stream gticStream = CS_0024_003C_003E8__locals0._003C_003E4__this._fs.Open(CS_0024_003C_003E8__locals0.gticPath, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)0);
						try
						{
							return (await GtiOfflineArchive.ParseFromStream(gticStream, CS_0024_003C_003E8__locals0.shutdown).Use<CatalogInfo>((Func<GtiOfflineArchive, EitherAsync<GtiArtifactError, CatalogInfo>>)((GtiOfflineArchive archive) => archive.ReadCatalogInfoFile(CS_0024_003C_003E8__locals0.shutdown).Bind<CatalogInfo>((string infoFileText) => GtiArtifactUtility.ParseCatalogInfo(infoFileText).ToEither().MapLeft<GtiArtifactError>((Seq<GtiArtifactError> e) => e.Head)
								.ToAsync()))).ToEither()
								.ConfigureAwait(false)).Map<AutoInitState>(delegate(CatalogInfo catalogInfo)
							{
								if (CS_0024_003C_003E8__locals1.catalogEntry.InstalledVersion.Major < 3)
								{
									return AutoInitState.CatalogLooksOld;
								}
								return (catalogInfo.CatalogVersion > CS_0024_003C_003E8__locals1.catalogEntry.InstalledVersion) ? AutoInitState.CatalogLooksOld : AutoInitState.Ready;
							}).IfLeft((Func<GtiArtifactError, AutoInitState>)delegate(GtiArtifactError error)
							{
								Logger.Log(LogLevel.Error, $"The included catalog cannot be parsed ({error}) and cannot be used to repair GTI. Proceeding as if the installed catalog is valid and correct.");
								return AutoInitState.Ready;
							});
						}
						finally
						{
							((System.IDisposable)gticStream)?.Dispose();
						}
					}), (Func<AutoInitState, System.Threading.Tasks.Task<AutoInitState>>)((AutoInitState state) => System.Threading.Tasks.Task.FromResult<AutoInitState>(state)), (Func<AutoInitState>)null)
						.ConfigureAwait(false)
						.GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AutoInitState>, _003CDetermineState_003Ed__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AutoInitState>);
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
	private struct _003COnStartup_003Ed__7 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public AutoInitCatalogStartup _003C_003E4__this;

		public CancellationToken shutdown;

		private _003C_003Ec__DisplayClass7_0 _003C_003E8__1;

		private _003C_003Ec__DisplayClass7_1 _003C_003E8__2;

		private string _003CselectedCatalogPath_003E5__2;

		private ConfiguredTaskAwaiter<AppLifeCycleActionRecord> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<AutoInitState> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<GtiProcessResult> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0601: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AutoInitCatalogStartup autoInitCatalogStartup = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<AppLifeCycleActionRecord> awaiter3;
				ConfiguredTaskAwaiter<AutoInitState> awaiter2;
				ConfiguredTaskAwaiter<GtiProcessResult> awaiter;
				GtiProcessResult result3;
				AutoInitState result4;
				GtiProcessResult result5;
				switch (num)
				{
				default:
					_003C_003E8__1 = new _003C_003Ec__DisplayClass7_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.shutdown = shutdown;
					awaiter3 = autoInitCatalogStartup._startup.WaitForStartupActionToComplete<CheckForGtiOperationStartup>().ConfigureAwait(false).GetAwaiter();
					if (!awaiter3.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter3;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AppLifeCycleActionRecord>, _003COnStartup_003Ed__7>(ref awaiter3, ref this);
						return;
					}
					goto IL_00bc;
				case 0:
					awaiter3 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AppLifeCycleActionRecord>);
					num = (_003C_003E1__state = -1);
					goto IL_00bc;
				case 1:
					awaiter2 = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<AutoInitState>);
					num = (_003C_003E1__state = -1);
					goto IL_0168;
				case 2:
					awaiter = _003C_003Eu__3;
					_003C_003Eu__3 = default(ConfiguredTaskAwaiter<GtiProcessResult>);
					num = (_003C_003E1__state = -1);
					goto IL_0246;
				case 3:
				case 4:
					try
					{
						if (num != 3)
						{
							if (num == 4)
							{
								awaiter = _003C_003Eu__3;
								_003C_003Eu__3 = default(ConfiguredTaskAwaiter<GtiProcessResult>);
								num = (_003C_003E1__state = -1);
								goto IL_0483;
							}
							autoInitCatalogStartup.Log(LogLevel.Error, $"The catalog directory exists but listing it failed. Repairing the catalog at '{_003C_003E8__1.gtiRunner.GtiRootDirectory}' using '{_003CselectedCatalogPath_003E5__2}'");
							awaiter = _003C_003E8__1.gtiRunner.Update(PathHelpers.QuotePathIfNeeded(_003CselectedCatalogPath_003E5__2)).IgnoreStandardOutput().TryToCancelProcessWithToken(_003C_003E8__1.shutdown)
								.ProcessResult.DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 3);
								_003C_003Eu__3 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<GtiProcessResult>, _003COnStartup_003Ed__7>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<GtiProcessResult>);
							num = (_003C_003E1__state = -1);
						}
						GtiProcessResult result = awaiter.GetResult();
						result.LogAnyErrorMessages();
						_003C_003E8__2.succeeded = result.Succeeded;
						if (!result.Succeeded)
						{
							_003COnStartup_003Eg__ThrowIfCanceled_007C7_2(result);
							autoInitCatalogStartup.Log(LogLevel.Info, "Updating the catalog failed. Will try reinitializing it instead.");
							awaiter = _003C_003E8__1._003COnStartup_003Eg__InitCatalog_007C1(_003CselectedCatalogPath_003E5__2).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 4);
								_003C_003Eu__3 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<GtiProcessResult>, _003COnStartup_003Ed__7>(ref awaiter, ref this);
								return;
							}
							goto IL_0483;
						}
						_003C_003E8__2.successfulMethod = CatalogRepairMethod.Update;
						goto end_IL_02c7;
						IL_0483:
						GtiProcessResult result2 = awaiter.GetResult();
						_003C_003E8__2.succeeded = result2.Succeeded;
						if (!result2.Succeeded)
						{
							_003COnStartup_003Eg__ThrowIfCanceled_007C7_2(result2);
							throw new InvalidOperationException("The Trainer catalog could not be repaired. Please try reinstalling the launcher.", (System.Exception)new GenericErrorException("Using " + _003CselectedCatalogPath_003E5__2 + " to update " + _003C_003E8__1.gtiRunner.GtiRootDirectory));
						}
						_003C_003E8__2.successfulMethod = CatalogRepairMethod.ReInit;
						end_IL_02c7:;
					}
					finally
					{
						if (num < 0)
						{
							TrackedEvent.Log(InlineEventDataDescriptor.Create(TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("CATALOG_REPAIR"), LauncherAppTarget.Instance), EventProperties.FromEnumerable(_003C_003E8__2._003COnStartup_003Eg__GenerateProps_007C3()), EventUserDataCensor.All));
						}
					}
					_003C_003E8__2 = null;
					break;
				case 5:
					{
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<GtiProcessResult>);
						num = (_003C_003E1__state = -1);
						goto IL_0656;
					}
					IL_00bc:
					awaiter3.GetResult();
					_003C_003E8__1.gtiRunner = autoInitCatalogStartup._gtiProvider.UnprotectedGtiRunner;
					_003CselectedCatalogPath_003E5__2 = _003C_003E8__1._003COnStartup_003Eg__GetGticFilePath_007C0();
					awaiter2 = autoInitCatalogStartup.DetermineState(_003C_003E8__1.gtiRunner, _003CselectedCatalogPath_003E5__2, _003C_003E8__1.shutdown).ConfigureAwait(false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AutoInitState>, _003COnStartup_003Ed__7>(ref awaiter2, ref this);
						return;
					}
					goto IL_0168;
					IL_0246:
					result3 = awaiter.GetResult();
					if (!result3.Succeeded)
					{
						_003COnStartup_003Eg__ThrowIfCanceled_007C7_2(result3);
						throw new InvalidOperationException("The Trainer catalog could not be initialized.", (System.Exception)new GenericErrorException("Using " + _003CselectedCatalogPath_003E5__2 + " to init " + _003C_003E8__1.gtiRunner.GtiRootDirectory));
					}
					break;
					IL_0168:
					result4 = awaiter2.GetResult();
					if (result4 == AutoInitState.MissingCatalog)
					{
						autoInitCatalogStartup.Log(LogLevel.Info, $"The catalog doesn't exist. Initializing the catalog at '{_003C_003E8__1.gtiRunner.GtiRootDirectory}' using '{_003CselectedCatalogPath_003E5__2}'");
						awaiter = _003C_003E8__1._003COnStartup_003Eg__InitCatalog_007C1(_003CselectedCatalogPath_003E5__2).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<GtiProcessResult>, _003COnStartup_003Ed__7>(ref awaiter, ref this);
							return;
						}
						goto IL_0246;
					}
					if (result4 == AutoInitState.NeedsRepair)
					{
						_003C_003E8__2 = new _003C_003Ec__DisplayClass7_1();
						_003C_003E8__2.succeeded = false;
						_003C_003E8__2.successfulMethod = null;
						goto case 3;
					}
					if (result4 == AutoInitState.CatalogLooksOld)
					{
						autoInitCatalogStartup.Log(LogLevel.Warning, $"The catalog in '{_003C_003E8__1.gtiRunner.GtiRootDirectory}' looks old. Lets try updating it using {_003CselectedCatalogPath_003E5__2}.");
						awaiter = _003C_003E8__1.gtiRunner.Update(PathHelpers.QuotePathIfNeeded(_003CselectedCatalogPath_003E5__2)).IgnoreStandardOutput().TryToCancelProcessWithToken(_003C_003E8__1.shutdown)
							.ProcessResult.DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 5);
							_003C_003Eu__3 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<GtiProcessResult>, _003COnStartup_003Ed__7>(ref awaiter, ref this);
							return;
						}
						goto IL_0656;
					}
					if (result4 != AutoInitState.Ready)
					{
						throw new NotSupportedException(((object)result4).ToString());
					}
					break;
					IL_0656:
					result5 = awaiter.GetResult();
					result5.LogAnyErrorMessages();
					if (!result5.Succeeded)
					{
						_003COnStartup_003Eg__ThrowIfCanceled_007C7_2(result5);
						autoInitCatalogStartup.Log(LogLevel.Warning, "The Trainer catalog could not be updated. Will continue using the existing one.");
					}
					break;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CselectedCatalogPath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CselectedCatalogPath_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	public const int KNOWN_CURRENT_CATALOG_MAJOR_VERSION = 3;

	private readonly IBaseGtiRunnerProvider _gtiProvider;

	private readonly IDeploymentPathProvider _deployment;

	private readonly IExecutionPathProvider _execution;

	private readonly IFileSystem _fs;

	private readonly IStartupRegistry _startup;

	public AutoInitCatalogStartup(IBaseGtiRunnerProvider gtiProvider, IDeploymentPathProvider deployment, IExecutionPathProvider execution, IFileSystem fileSystem, IStartupRegistry startup)
	{
		_gtiProvider = gtiProvider;
		_deployment = deployment;
		_execution = execution;
		_fs = fileSystem;
		_startup = startup;
		base.ActionName = "Auto-Init Catalog";
		_startup = startup;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__7))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__7 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__7);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__7>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CDetermineState_003Ed__8))]
	private async System.Threading.Tasks.Task<AutoInitState> DetermineState(IGtiRunner gtiRunner, string gticPath, CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		string gticPath2 = gticPath;
		IGtiRunner gtiRunner2 = gtiRunner;
		return await GetCatalogEntryOrTerminalState(3, 1).ToAsync().MatchAsync<AutoInitState>((Func<GtiListingEntry, System.Threading.Tasks.Task<AutoInitState>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CDetermineState_003Eb__1_003Ed))] async (GtiListingEntry catalogEntry) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (catalogEntry.InstalledVersion == null)
			{
				return AutoInitState.NeedsRepair;
			}
			Stream gticStream = _fs.Open(gticPath2, (FileMode)3, (FileAccess)1, (FileShare)3, (FileOptions)0);
			try
			{
				return (await GtiOfflineArchive.ParseFromStream(gticStream, shutdown).Use<CatalogInfo>((Func<GtiOfflineArchive, EitherAsync<GtiArtifactError, CatalogInfo>>)((GtiOfflineArchive archive) => archive.ReadCatalogInfoFile(shutdown).Bind<CatalogInfo>((string infoFileText) => GtiArtifactUtility.ParseCatalogInfo(infoFileText).ToEither().MapLeft<GtiArtifactError>((Seq<GtiArtifactError> e) => e.Head)
					.ToAsync()))).ToEither()
					.ConfigureAwait(false)).Map<AutoInitState>(delegate(CatalogInfo catalogInfo)
				{
					if (catalogEntry.InstalledVersion.Major < 3)
					{
						return AutoInitState.CatalogLooksOld;
					}
					return (catalogInfo.CatalogVersion > catalogEntry.InstalledVersion) ? AutoInitState.CatalogLooksOld : AutoInitState.Ready;
				}).IfLeft((Func<GtiArtifactError, AutoInitState>)delegate(GtiArtifactError error)
				{
					Logger.Log(LogLevel.Error, $"The included catalog cannot be parsed ({error}) and cannot be used to repair GTI. Proceeding as if the installed catalog is valid and correct.");
					return AutoInitState.Ready;
				});
			}
			finally
			{
				((System.IDisposable)gticStream)?.Dispose();
			}
		}), (Func<AutoInitState, System.Threading.Tasks.Task<AutoInitState>>)((AutoInitState state) => System.Threading.Tasks.Task.FromResult<AutoInitState>(state)), (Func<AutoInitState>)null).ConfigureAwait(false);
		[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass8_0._003C_003CDetermineState_003Eg__GetCatalogEntryOrTerminalState_007C0_003Ed))]
		async System.Threading.Tasks.Task<Either<AutoInitState, GtiListingEntry>> GetCatalogEntryOrTerminalState(int maxAttempts, int attempt)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (!_fs.PathExists(Path.Combine(gtiRunner2.GtiRootDirectory, ".gti")))
			{
				return AutoInitState.MissingCatalog;
			}
			Either<AutoInitState, GtiListingEntry> result = default(Either<AutoInitState, GtiListingEntry>);
			object obj;
			int num;
			try
			{
				result = Prelude.Optional(Enumerable.FirstOrDefault<GtiListingEntry>((System.Collections.Generic.IEnumerable<GtiListingEntry>)(await gtiRunner2.List().GetListEntries().Where<GtiListingEntry>((Func<GtiListingEntry, bool>)((GtiListingEntry e) => e.IsTheCatalog))
					.ToList<GtiListingEntry>()
					.ToTask<System.Collections.Generic.IList<GtiListingEntry>>(shutdown)
					.DetachContext()))).ToEither(AutoInitState.NeedsRepair);
				return result;
			}
			catch (GtiProcessException ex)
			{
				obj = ex;
				num = 1;
			}
			catch (System.Exception e2)
			{
				Log(e2, "Unknown GTI listing error");
				throw;
			}
			if (num != 1)
			{
				return result;
			}
			GtiProcessException e3 = (GtiProcessException)obj;
			Log(e3, $"GTI listing error. Attempt: {attempt}");
			if (attempt >= maxAttempts)
			{
				return AutoInitState.NeedsRepair;
			}
			return await GetCatalogEntryOrTerminalState(maxAttempts, attempt + 1).ConfigureAwait(false);
		}
	}

	[CompilerGenerated]
	internal static void _003COnStartup_003Eg__ThrowIfCanceled_007C7_2(GtiProcessResult processResult)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (processResult.WasCanceled)
		{
			throw new OperationCanceledException();
		}
	}
}
