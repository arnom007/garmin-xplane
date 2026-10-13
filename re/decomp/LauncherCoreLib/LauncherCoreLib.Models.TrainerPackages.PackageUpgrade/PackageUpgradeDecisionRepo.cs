using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.TrainerPackages.Serialized;
using Nito.AsyncEx;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;

public class PackageUpgradeDecisionRepo : IPackageUpgradeDecisionRepo
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass4_0
	{
		[StructLayout(3)]
		private struct _003C_003CGetPriorUserUpgradeDecisions_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<UpgradeDecisionLog> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass4_0 _003C_003E4__this;

			public TrainerEmbeddedReleaseId ur;

			private ConfiguredTaskAwaiter<EmbeddedReleaseDecisionLog> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass4_0 _003C_003Ec__DisplayClass4_ = _003C_003E4__this;
				UpgradeDecisionLog result;
				try
				{
					string path = default(string);
					EmbeddedReleaseDecisionLog embeddedReleaseDecisionLog;
					if (num != 0)
					{
						path = Path.Combine(_003C_003Ec__DisplayClass4_.decisioncsDirPath, ((object)ur).ToString() + ".json");
						if (!_003C_003Ec__DisplayClass4_._003C_003E4__this._lowLevel.FileSystem.PathExists(path))
						{
							embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
							goto IL_00e8;
						}
					}
					try
					{
						ConfiguredTaskAwaiter<EmbeddedReleaseDecisionLog> awaiter;
						if (num != 0)
						{
							awaiter = _003C_003Ec__DisplayClass4_._003C_003E4__this._lowLevel.ObjectPersistence.Load<EmbeddedReleaseDecisionLog>(path).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<EmbeddedReleaseDecisionLog>, _003C_003CGetPriorUserUpgradeDecisions_003Eb__0_003Ed>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<EmbeddedReleaseDecisionLog>);
							num = (_003C_003E1__state = -1);
						}
						embeddedReleaseDecisionLog = awaiter.GetResult();
						if (embeddedReleaseDecisionLog == null)
						{
							embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
						}
					}
					catch (FileNotFoundException)
					{
						embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
					}
					goto IL_00e8;
					IL_00e8:
					result = new UpgradeDecisionLog(ur, embeddedReleaseDecisionLog);
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

		public PackageUpgradeDecisionRepo _003C_003E4__this;

		public string decisioncsDirPath;

		[AsyncStateMachine(typeof(_003C_003CGetPriorUserUpgradeDecisions_003Eb__0_003Ed))]
		internal async System.Threading.Tasks.Task<UpgradeDecisionLog> _003CGetPriorUserUpgradeDecisions_003Eb__0(TrainerEmbeddedReleaseId ur)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string path = Path.Combine(decisioncsDirPath, ((object)ur).ToString() + ".json");
			EmbeddedReleaseDecisionLog embeddedReleaseDecisionLog;
			if (_003C_003E4__this._lowLevel.FileSystem.PathExists(path))
			{
				try
				{
					embeddedReleaseDecisionLog = await _003C_003E4__this._lowLevel.ObjectPersistence.Load<EmbeddedReleaseDecisionLog>(path).DetachContext();
					if (embeddedReleaseDecisionLog == null)
					{
						embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
					}
				}
				catch (FileNotFoundException)
				{
					embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
				}
			}
			else
			{
				embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
			}
			return new UpgradeDecisionLog(ur, embeddedReleaseDecisionLog);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		[StructLayout(3)]
		private struct _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass5_0 _003C_003E4__this;

			public IGrouping<TrainerEmbeddedReleaseId, PackageUpgradeVersions> groupedByEmbeddedRelease;

			private _003C_003Ec__DisplayClass5_1 _003C_003E8__1;

			private string _003CreleaseFilePath_003E5__2;

			private ConfiguredTaskAwaiter<EmbeddedReleaseDecisionLog> _003C_003Eu__1;

			private ConfiguredTaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_019c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0118: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0161: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0184: Unknown result type (might be due to invalid IL or missing references)
				//IL_0186: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass5_0 _003C_003Ec__DisplayClass5_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					EmbeddedReleaseDecisionLog embeddedReleaseDecisionLog;
					ConfiguredTaskAwaiter<EmbeddedReleaseDecisionLog> awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
							num = (_003C_003E1__state = -1);
							goto IL_01b8;
						}
						_003C_003E8__1 = new _003C_003Ec__DisplayClass5_1();
						_003C_003E8__1.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass5_;
						_003CreleaseFilePath_003E5__2 = Path.Combine(_003C_003Ec__DisplayClass5_.decisioncsDirPath, ((object)groupedByEmbeddedRelease.Key).ToString() + ".json");
						embeddedReleaseDecisionLog = null;
						if (!_003C_003Ec__DisplayClass5_._003C_003E4__this._lowLevel.FileSystem.PathExists(_003CreleaseFilePath_003E5__2))
						{
							embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
							goto IL_00fe;
						}
						awaiter2 = _003C_003Ec__DisplayClass5_._003C_003E4__this._lowLevel.ObjectPersistence.Load<EmbeddedReleaseDecisionLog>(_003CreleaseFilePath_003E5__2).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<EmbeddedReleaseDecisionLog>, _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<EmbeddedReleaseDecisionLog>);
						num = (_003C_003E1__state = -1);
					}
					embeddedReleaseDecisionLog = awaiter2.GetResult();
					goto IL_00fe;
					IL_01b8:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					goto end_IL_000e;
					IL_00fe:
					_003C_003Ec__DisplayClass5_1 _003C_003Ec__DisplayClass5_2 = _003C_003E8__1;
					DateTimeOffset now = _003C_003Ec__DisplayClass5_._003C_003E4__this._dateTimeScheduler.Now;
					_003C_003Ec__DisplayClass5_2.decidedAt = ((DateTimeOffset)(ref now)).ToLocalTime();
					embeddedReleaseDecisionLog.AddEntries(Enumerable.Select<PackageUpgradeVersions, EmbeddedReleaseDecisionLog.Entry>((System.Collections.Generic.IEnumerable<PackageUpgradeVersions>)groupedByEmbeddedRelease, (Func<PackageUpgradeVersions, EmbeddedReleaseDecisionLog.Entry>)((PackageUpgradeVersions d) => new EmbeddedReleaseDecisionLog.Entry
					{
						LocalVersion = d.LocalVersion,
						UpgradeVersion = d.UpgradeVersion,
						Decision = _003C_003E8__1.CS_0024_003C_003E8__locals1.selectedOption,
						DecidedAt = _003C_003E8__1.decidedAt
					})));
					ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass5_._003C_003E4__this._lowLevel.ObjectPersistence.Commit(embeddedReleaseDecisionLog, _003CreleaseFilePath_003E5__2).DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed>(ref awaiter, ref this);
						return;
					}
					goto IL_01b8;
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003CreleaseFilePath_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CreleaseFilePath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public PackageUpgradeDecisionRepo _003C_003E4__this;

		public UpgradeUserDecision selectedOption;

		public string decisioncsDirPath;

		[AsyncStateMachine(typeof(_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed))]
		internal System.Threading.Tasks.Task _003CStoreUserUpgradeDecision_003Eb__1(IGrouping<TrainerEmbeddedReleaseId, PackageUpgradeVersions> groupedByEmbeddedRelease)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed = default(_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed);
			_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed.groupedByEmbeddedRelease = groupedByEmbeddedRelease;
			_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed>(ref _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_1
	{
		public DateTimeOffset decidedAt;

		public _003C_003Ec__DisplayClass5_0 CS_0024_003C_003E8__locals1;

		internal EmbeddedReleaseDecisionLog.Entry _003CStoreUserUpgradeDecision_003Eb__2(PackageUpgradeVersions d)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			return new EmbeddedReleaseDecisionLog.Entry
			{
				LocalVersion = d.LocalVersion,
				UpgradeVersion = d.UpgradeVersion,
				Decision = CS_0024_003C_003E8__locals1.selectedOption,
				DecidedAt = decidedAt
			};
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CGetPriorUserUpgradeDecisions_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<System.Collections.Generic.IList<UpgradeDecisionLog>> _003C_003Et__builder;

		public PackageUpgradeDecisionRepo _003C_003E4__this;

		public System.Collections.Generic.IEnumerable<TrainerEmbeddedReleaseId> releases;

		private _003C_003Ec__DisplayClass4_0 _003C_003E8__1;

		private List<TrainerEmbeddedReleaseId> _003CuniqueReleases_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<UpgradeDecisionLog[]> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageUpgradeDecisionRepo packageUpgradeDecisionRepo = _003C_003E4__this;
			System.Collections.Generic.IList<UpgradeDecisionLog> result;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_00c8;
				}
				if (num == 1)
				{
					goto IL_00d8;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass4_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003CuniqueReleases_003E5__2 = Enumerable.ToList<TrainerEmbeddedReleaseId>(Enumerable.Distinct<TrainerEmbeddedReleaseId>(releases));
				if (_003CuniqueReleases_003E5__2.Count > 0)
				{
					awaiter = packageUpgradeDecisionRepo._lock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetPriorUserUpgradeDecisions_003Ed__4>(ref awaiter, ref this);
						return;
					}
					goto IL_00c8;
				}
				result = (System.Collections.Generic.IList<UpgradeDecisionLog>)new List<UpgradeDecisionLog>();
				goto end_IL_000e;
				IL_00d8:
				try
				{
					ConfiguredTaskAwaiter<UpgradeDecisionLog[]> awaiter2;
					if (num != 1)
					{
						_003C_003E8__1.decisioncsDirPath = Path.Combine(packageUpgradeDecisionRepo._lowLevel.Deployment.UserAppDataDirPath, "PackageUpgradeDecisions");
						awaiter2 = System.Threading.Tasks.Task.WhenAll<UpgradeDecisionLog>(Enumerable.Select<TrainerEmbeddedReleaseId, System.Threading.Tasks.Task<UpgradeDecisionLog>>((System.Collections.Generic.IEnumerable<TrainerEmbeddedReleaseId>)_003CuniqueReleases_003E5__2, (Func<TrainerEmbeddedReleaseId, System.Threading.Tasks.Task<UpgradeDecisionLog>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass4_0._003C_003CGetPriorUserUpgradeDecisions_003Eb__0_003Ed))] async (TrainerEmbeddedReleaseId ur) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							string path = Path.Combine(_003C_003E8__1.decisioncsDirPath, ((object)ur).ToString() + ".json");
							EmbeddedReleaseDecisionLog embeddedReleaseDecisionLog;
							if (_003C_003E8__1._003C_003E4__this._lowLevel.FileSystem.PathExists(path))
							{
								try
								{
									embeddedReleaseDecisionLog = await _003C_003E8__1._003C_003E4__this._lowLevel.ObjectPersistence.Load<EmbeddedReleaseDecisionLog>(path).DetachContext();
									if (embeddedReleaseDecisionLog == null)
									{
										embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
									}
								}
								catch (FileNotFoundException)
								{
									embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
								}
							}
							else
							{
								embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
							}
							return new UpgradeDecisionLog(ur, embeddedReleaseDecisionLog);
						}))).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<UpgradeDecisionLog[]>, _003CGetPriorUserUpgradeDecisions_003Ed__4>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<UpgradeDecisionLog[]>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter2.GetResult();
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				goto end_IL_000e;
				IL_00c8:
				System.IDisposable result2 = awaiter.GetResult();
				_003C_003E7__wrap2 = result2;
				goto IL_00d8;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CuniqueReleases_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CuniqueReleases_003E5__2 = null;
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
	private struct _003CStoreUserUpgradeDecision_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public PackageUpgradeDecisionRepo _003C_003E4__this;

		public UpgradeUserDecision selectedOption;

		public System.Collections.Generic.IEnumerable<PackageUpgradeVersions> versionsInDecision;

		private _003C_003Ec__DisplayClass5_0 _003C_003E8__1;

		private List<PackageUpgradeVersions> _003CallUpgrades_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageUpgradeDecisionRepo packageUpgradeDecisionRepo = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<System.IDisposable>);
					num = (_003C_003E1__state = -1);
					goto IL_00d1;
				}
				if (num == 1)
				{
					goto IL_00e1;
				}
				_003C_003E8__1 = new _003C_003Ec__DisplayClass5_0();
				_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
				_003C_003E8__1.selectedOption = selectedOption;
				_003CallUpgrades_003E5__2 = Enumerable.ToList<PackageUpgradeVersions>(versionsInDecision);
				if (_003CallUpgrades_003E5__2.Count > 0)
				{
					awaiter = packageUpgradeDecisionRepo._lock.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CStoreUserUpgradeDecision_003Ed__5>(ref awaiter, ref this);
						return;
					}
					goto IL_00d1;
				}
				goto end_IL_000e;
				IL_00e1:
				try
				{
					ConfiguredTaskAwaiter awaiter2;
					if (num != 1)
					{
						_003C_003E8__1.decisioncsDirPath = Path.Combine(packageUpgradeDecisionRepo._lowLevel.Deployment.UserAppDataDirPath, "PackageUpgradeDecisions");
						packageUpgradeDecisionRepo._lowLevel.FileSystem.CreateDirectory(_003C_003E8__1.decisioncsDirPath);
						ConfiguredTaskAwaitable val = System.Threading.Tasks.Task.WhenAll(Enumerable.Select<IGrouping<TrainerEmbeddedReleaseId, PackageUpgradeVersions>, System.Threading.Tasks.Task>(Enumerable.GroupBy<PackageUpgradeVersions, TrainerEmbeddedReleaseId>((System.Collections.Generic.IEnumerable<PackageUpgradeVersions>)_003CallUpgrades_003E5__2, (Func<PackageUpgradeVersions, TrainerEmbeddedReleaseId>)((PackageUpgradeVersions up) => up.EmbeddedReleaseId)), (Func<IGrouping<TrainerEmbeddedReleaseId, PackageUpgradeVersions>, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass5_0._003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed))] (IGrouping<TrainerEmbeddedReleaseId, PackageUpgradeVersions> groupedByEmbeddedRelease) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass5_0._003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass5_0._003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed);
							_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
							_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003E4__this = _003C_003E8__1;
							_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed.groupedByEmbeddedRelease = groupedByEmbeddedRelease;
							_003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003E1__state = -1;
							((AsyncTaskMethodBuilder)(ref _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass5_0._003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed>(ref _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed);
							return ((AsyncTaskMethodBuilder)(ref _003C_003CStoreUserUpgradeDecision_003Eb__1_003Ed._003C_003Et__builder)).Task;
						}))).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CStoreUserUpgradeDecision_003Ed__5>(ref awaiter2, ref this);
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
					if (num < 0 && _003C_003E7__wrap2 != null)
					{
						_003C_003E7__wrap2.Dispose();
					}
				}
				_003C_003E7__wrap2 = null;
				goto end_IL_000e;
				IL_00d1:
				System.IDisposable result = awaiter.GetResult();
				_003C_003E7__wrap2 = result;
				goto IL_00e1;
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CallUpgrades_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CallUpgrades_003E5__2 = null;
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private readonly ILowLevelDal _lowLevel;

	private readonly IScheduler _dateTimeScheduler;

	private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();

	public PackageUpgradeDecisionRepo(ILowLevelDal lowLevel, IScheduler scheduler)
	{
		_lowLevel = lowLevel;
		_dateTimeScheduler = scheduler;
	}

	[AsyncStateMachine(typeof(_003CGetPriorUserUpgradeDecisions_003Ed__4))]
	public async System.Threading.Tasks.Task<System.Collections.Generic.IList<UpgradeDecisionLog>> GetPriorUserUpgradeDecisions(System.Collections.Generic.IEnumerable<TrainerEmbeddedReleaseId> releases)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<TrainerEmbeddedReleaseId> uniqueReleases = Enumerable.ToList<TrainerEmbeddedReleaseId>(Enumerable.Distinct<TrainerEmbeddedReleaseId>(releases));
		if (uniqueReleases.Count > 0)
		{
			using (await _lock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				string decisioncsDirPath = Path.Combine(_lowLevel.Deployment.UserAppDataDirPath, "PackageUpgradeDecisions");
				return await System.Threading.Tasks.Task.WhenAll<UpgradeDecisionLog>(Enumerable.Select<TrainerEmbeddedReleaseId, System.Threading.Tasks.Task<UpgradeDecisionLog>>((System.Collections.Generic.IEnumerable<TrainerEmbeddedReleaseId>)uniqueReleases, (Func<TrainerEmbeddedReleaseId, System.Threading.Tasks.Task<UpgradeDecisionLog>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass4_0._003C_003CGetPriorUserUpgradeDecisions_003Eb__0_003Ed))] async (TrainerEmbeddedReleaseId ur) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					string path = Path.Combine(decisioncsDirPath, ((object)ur).ToString() + ".json");
					EmbeddedReleaseDecisionLog embeddedReleaseDecisionLog;
					if (_lowLevel.FileSystem.PathExists(path))
					{
						try
						{
							embeddedReleaseDecisionLog = await _lowLevel.ObjectPersistence.Load<EmbeddedReleaseDecisionLog>(path).DetachContext();
							if (embeddedReleaseDecisionLog == null)
							{
								embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
							}
						}
						catch (FileNotFoundException)
						{
							embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
						}
					}
					else
					{
						embeddedReleaseDecisionLog = new EmbeddedReleaseDecisionLog();
					}
					return new UpgradeDecisionLog(ur, embeddedReleaseDecisionLog);
				}))).DetachContext();
			}
		}
		return (System.Collections.Generic.IList<UpgradeDecisionLog>)new List<UpgradeDecisionLog>();
	}

	[AsyncStateMachine(typeof(_003CStoreUserUpgradeDecision_003Ed__5))]
	public System.Threading.Tasks.Task StoreUserUpgradeDecision(System.Collections.Generic.IEnumerable<PackageUpgradeVersions> versionsInDecision, UpgradeUserDecision selectedOption)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CStoreUserUpgradeDecision_003Ed__5 _003CStoreUserUpgradeDecision_003Ed__ = default(_003CStoreUserUpgradeDecision_003Ed__5);
		_003CStoreUserUpgradeDecision_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CStoreUserUpgradeDecision_003Ed__._003C_003E4__this = this;
		_003CStoreUserUpgradeDecision_003Ed__.versionsInDecision = versionsInDecision;
		_003CStoreUserUpgradeDecision_003Ed__.selectedOption = selectedOption;
		_003CStoreUserUpgradeDecision_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CStoreUserUpgradeDecision_003Ed__._003C_003Et__builder)).Start<_003CStoreUserUpgradeDecision_003Ed__5>(ref _003CStoreUserUpgradeDecision_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CStoreUserUpgradeDecision_003Ed__._003C_003Et__builder)).Task;
	}
}
