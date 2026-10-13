using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using GTI.Net;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.AppLifeCycle;

namespace LauncherCoreLib.Models.TrainerPackages;

public class SyncCatalogRefugeStartup : AppLifeCycleActionBase, IStartupAction, IAppLifeCycleAction
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_0
	{
		public SyncCatalogRefugeStartup _003C_003E4__this;

		public System.Threading.Tasks.Task<Version> catalogVersionTask;

		internal IObservable<Unit> _003COnStartup_003Eb__3(GtiListingEntry entry)
		{
			return Observable.FromAsync((Func<System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass3_1
			{
				CS_0024_003C_003E8__locals1 = this,
				entry = entry
			}._003COnStartup_003Eb__4);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass3_1
	{
		[StructLayout(3)]
		private struct _003C_003COnStartup_003Eb__4_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass3_1 _003C_003E4__this;

			private CatalogRefugeHarness _003C_003E7__wrap1;

			private ConfiguredTaskAwaiter<Version> _003C_003Eu__1;

			private ConfiguredTaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass3_1 _003C_003Ec__DisplayClass3_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					ConfiguredTaskAwaiter<Version> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_0101;
						}
						_003C_003E7__wrap1 = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals1._003C_003E4__this._catalogRefugeHarness;
						awaiter2 = _003C_003Ec__DisplayClass3_.CS_0024_003C_003E8__locals1.catalogVersionTask.DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Version>, _003C_003COnStartup_003Eb__4_003Ed>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<Version>);
						num = (_003C_003E1__state = -1);
					}
					Version result = awaiter2.GetResult();
					ConfiguredTaskAwaitable val = _003C_003E7__wrap1.HandleGtiListing(result, _003C_003Ec__DisplayClass3_.entry).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003COnStartup_003Eb__4_003Ed>(ref awaiter, ref this);
						return;
					}
					goto IL_0101;
					IL_0101:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					_003C_003E7__wrap1 = null;
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

		public GtiListingEntry entry;

		public _003C_003Ec__DisplayClass3_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003COnStartup_003Eb__4_003Ed))]
		internal System.Threading.Tasks.Task _003COnStartup_003Eb__4()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003COnStartup_003Eb__4_003Ed _003C_003COnStartup_003Eb__4_003Ed = default(_003C_003COnStartup_003Eb__4_003Ed);
			_003C_003COnStartup_003Eb__4_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003COnStartup_003Eb__4_003Ed._003C_003E4__this = this;
			_003C_003COnStartup_003Eb__4_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__4_003Ed._003C_003Et__builder)).Start<_003C_003COnStartup_003Eb__4_003Ed>(ref _003C_003COnStartup_003Eb__4_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003COnStartup_003Eb__4_003Ed._003C_003Et__builder)).Task;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003COnStartup_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public SyncCatalogRefugeStartup _003C_003E4__this;

		private _003C_003Ec__DisplayClass3_0 _003C_003E8__1;

		public CancellationToken shutdown;

		private ConfiguredTaskAwaiter<ILauncherGtiRunner> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<Unit> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SyncCatalogRefugeStartup syncCatalogRefugeStartup = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<Unit> awaiter;
				ConfiguredTaskAwaiter<ILauncherGtiRunner> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<Unit>);
						num = (_003C_003E1__state = -1);
						goto IL_01d9;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass3_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					awaiter2 = syncCatalogRefugeStartup._launcherGtiProvider.GtiExe.ConfigureAwait(false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ILauncherGtiRunner>, _003COnStartup_003Ed__3>(ref awaiter2, ref this);
						return;
					}
				}
				else
				{
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ILauncherGtiRunner>);
					num = (_003C_003E1__state = -1);
				}
				ILauncherGtiRunner result = awaiter2.GetResult();
				syncCatalogRefugeStartup.Log(LogLevel.Debug, "Obtained the GTI runner.");
				IObservable<GtiListingEntry> source = result.List().GetListEntries().Replay<GtiListingEntry>()
					.RefCount();
				_003C_003E8__1.catalogVersionTask = source.FirstAsync<GtiListingEntry>((GtiListingEntry e) => e.IsTheCatalog).Select<GtiListingEntry, Version>((Func<GtiListingEntry, Version>)((GtiListingEntry e) => e.InstalledVersion ?? throw new InvalidDataException("Catalog must have an installed version value."))).ToTask<Version>();
				awaiter = source.TakeUntil<GtiListingEntry>(shutdown).Where<GtiListingEntry>((Func<GtiListingEntry, bool>)((GtiListingEntry e) => !e.IsTheCatalog)).Select<GtiListingEntry, IObservable<Unit>>((Func<GtiListingEntry, IObservable<Unit>>)((GtiListingEntry entry) => Observable.FromAsync((Func<System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass3_1
				{
					CS_0024_003C_003E8__locals1 = _003C_003E8__1,
					entry = entry
				}._003COnStartup_003Eb__4)))
					.Merge<Unit>()
					.TakeUntil<Unit>(shutdown)
					.LastOrDefaultAsync<Unit>()
					.ToTask<Unit>()
					.DetachContext()
					.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Unit>, _003COnStartup_003Ed__3>(ref awaiter, ref this);
					return;
				}
				goto IL_01d9;
				IL_01d9:
				awaiter.GetResult();
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

	private readonly ILauncherGtiProvider _launcherGtiProvider;

	private readonly CatalogRefugeHarness _catalogRefugeHarness;

	public SyncCatalogRefugeStartup(ILauncherGtiProvider launcherGtiProvider, CatalogRefugeHarness catalogRefugeHarness)
	{
		_launcherGtiProvider = launcherGtiProvider;
		_catalogRefugeHarness = catalogRefugeHarness;
	}

	[AsyncStateMachine(typeof(_003COnStartup_003Ed__3))]
	public System.Threading.Tasks.Task OnStartup(CancellationToken shutdown)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_003COnStartup_003Ed__3 _003COnStartup_003Ed__ = default(_003COnStartup_003Ed__3);
		_003COnStartup_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003COnStartup_003Ed__._003C_003E4__this = this;
		_003COnStartup_003Ed__.shutdown = shutdown;
		_003COnStartup_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Start<_003COnStartup_003Ed__3>(ref _003COnStartup_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003COnStartup_003Ed__._003C_003Et__builder)).Task;
	}
}
