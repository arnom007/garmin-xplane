using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.FileSystem;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.StringHelpers.Paths;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.DeployedEnv;
using LauncherCoreLib.Utility;
using Nito.AsyncEx;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class CatalogPackageRefuge : ICatalogPackageRefuge
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003CListRescuedPackages_003Eb__18_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IObservable<IRescuedCatalogPackage>> _003C_003Et__builder;

		public CatalogPackageRefuge _003C_003E4__this;

		public CancellationToken t;

		private _003C_003Ec__DisplayClass18_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Expected O, but got Unknown
			int num = _003C_003E1__state;
			CatalogPackageRefuge CS_0024_003C_003E8__locals0 = _003C_003E4__this;
			IObservable<IRescuedCatalogPackage> result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if (!CS_0024_003C_003E8__locals0._lowLevelDal.FileSystem.PathExists(CS_0024_003C_003E8__locals0.RefugeRootDir))
					{
						goto IL_018b;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass18_0();
					awaiter = CS_0024_003C_003E8__locals0._refugeLock.LockAsync(t).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CListRescuedPackages_003Eb__18_0_003Ed>(ref awaiter, ref this);
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
				_003C_003E8__1.l = result;
				try
				{
					result2 = (IObservable<IRescuedCatalogPackage>)(object)Enumerable.Where<FileSystemEntry>(CS_0024_003C_003E8__locals0._lowLevelDal.FileSystem.DirectoriesIn(CS_0024_003C_003E8__locals0.RefugeRootDir), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => fse.Name != ".tmp" && !fse.Name.StartsWith(".tmp_", (StringComparison)4))).ToObservable().SelectMany<FileSystemEntry, FileSystemEntry>((Func<FileSystemEntry, System.Collections.Generic.IEnumerable<FileSystemEntry>>)([CompilerGenerated] (FileSystemEntry gtiAppDir) => CS_0024_003C_003E8__locals0._lowLevelDal.FileSystem.DirectoriesIn(gtiAppDir.FullPath)))
						.Where<FileSystemEntry>((Func<FileSystemEntry, bool>)((FileSystemEntry fse) => !fse.Name.StartsWith(".tmp_", (StringComparison)4)))
						.SelectMany<FileSystemEntry, RescuedCatalogPackage>((Func<FileSystemEntry, System.Threading.Tasks.Task<RescuedCatalogPackage>>)([AsyncStateMachine(typeof(_003C_003CListRescuedPackages_003Eb__18_4_003Ed))] [CompilerGenerated] async (FileSystemEntry safePackDir) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							try
							{
								return await CS_0024_003C_003E8__locals0.ParseDirectoryWithLock(safePackDir.FullPath).ConfigureAwait(false);
							}
							catch (FormatException val2)
							{
								Logger.LogFormattedException((System.Exception)val2, "Listing refuge directories");
								return (RescuedCatalogPackage)null;
							}
						}))
						.WhereClassNotNull<RescuedCatalogPackage>()
						.Finally<RescuedCatalogPackage>((Action)delegate
						{
							_003C_003E8__1.l.Dispose();
						});
				}
				catch (DirectoryNotFoundException)
				{
					_003C_003E8__1.l.Dispose();
					goto IL_0184;
				}
				catch (System.Exception)
				{
					_003C_003E8__1.l.Dispose();
					throw;
				}
				goto end_IL_000e;
				IL_0184:
				_003C_003E8__1 = null;
				goto IL_018b;
				IL_018b:
				result2 = Observable.Empty<IRescuedCatalogPackage>();
				end_IL_000e:;
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
	private struct _003C_003CListRescuedPackages_003Eb__18_4_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<RescuedCatalogPackage> _003C_003Et__builder;

		public CatalogPackageRefuge _003C_003E4__this;

		public FileSystemEntry safePackDir;

		private ConfiguredTaskAwaiter<RescuedCatalogPackage> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0093: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogPackageRefuge catalogPackageRefuge = _003C_003E4__this;
			RescuedCatalogPackage result;
			try
			{
				try
				{
					ConfiguredTaskAwaiter<RescuedCatalogPackage> awaiter;
					if (num != 0)
					{
						awaiter = catalogPackageRefuge.ParseDirectoryWithLock(safePackDir.FullPath).ConfigureAwait(false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RescuedCatalogPackage>, _003C_003CListRescuedPackages_003Eb__18_4_003Ed>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<RescuedCatalogPackage>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult();
				}
				catch (FormatException val)
				{
					Logger.LogFormattedException((System.Exception)val, "Listing refuge directories");
					result = null;
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

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public System.IDisposable l;

		internal void _003CListRescuedPackages_003Eb__5()
		{
			l.Dispose();
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CAddCatalogVersionWithLock_003Ed__22 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public string targetDirPath;

		public CatalogPackageRefuge _003C_003E4__this;

		public Version catalogVersion;

		private string _003CcatalogRecordPath_003E5__2;

		private ConfiguredTaskAwaiter<List<Version>> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogPackageRefuge catalogPackageRefuge = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<List<Version>> awaiter2;
				ConfiguredTaskAwaitable val;
				ConfiguredTaskAwaiter awaiter;
				switch (num)
				{
				default:
					_003CcatalogRecordPath_003E5__2 = GetCatalogRecordPath(targetDirPath);
					if (catalogPackageRefuge._lowLevelDal.FileSystem.PathExists(_003CcatalogRecordPath_003E5__2))
					{
						awaiter2 = catalogPackageRefuge.ParseVersionsWithLock(_003CcatalogRecordPath_003E5__2).ConfigureAwait(false).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<List<Version>>, _003CAddCatalogVersionWithLock_003Ed__22>(ref awaiter2, ref this);
							return;
						}
						goto IL_00af;
					}
					val = catalogPackageRefuge._lowLevelDal.FileSystem.WriteAllText(_003CcatalogRecordPath_003E5__2, _003CAddCatalogVersionWithLock_003Eg__FormatVersionLine_007C22_0(catalogVersion), REFUGE_FILE_ENCODING).ConfigureAwait(false);
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAddCatalogVersionWithLock_003Ed__22>(ref awaiter, ref this);
						return;
					}
					break;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<List<Version>>);
					num = (_003C_003E1__state = -1);
					goto IL_00af;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0147;
				case 2:
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_00af:
					if (!awaiter2.GetResult().Contains(catalogVersion))
					{
						val = catalogPackageRefuge._lowLevelDal.FileSystem.AppendText(_003CcatalogRecordPath_003E5__2, _003CAddCatalogVersionWithLock_003Eg__FormatVersionLine_007C22_0(catalogVersion), REFUGE_FILE_ENCODING).ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CAddCatalogVersionWithLock_003Ed__22>(ref awaiter, ref this);
							return;
						}
						goto IL_0147;
					}
					goto end_IL_000e;
					IL_0147:
					((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
					goto end_IL_000e;
				}
				((ConfiguredTaskAwaiter)(ref awaiter)).GetResult();
				end_IL_000e:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CcatalogRecordPath_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CcatalogRecordPath_003E5__2 = null;
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
	private struct _003CGetRescuedPackageByGtiInfoOrNull_003Ed__16 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IRescuedCatalogPackage> _003C_003Et__builder;

		public CatalogPackageRefuge _003C_003E4__this;

		public string gtiAppName;

		public Version appCatalogedVersion;

		private string _003CtargetPath_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<RescuedCatalogPackage> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogPackageRefuge catalogPackageRefuge = _003C_003E4__this;
			IRescuedCatalogPackage result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00aa;
					}
					_003CtargetPath_003E5__2 = catalogPackageRefuge.GetRescueDirPath(gtiAppName, appCatalogedVersion);
					awaiter = catalogPackageRefuge._refugeLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetRescuedPackageByGtiInfoOrNull_003Ed__16>(ref awaiter, ref this);
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
				goto IL_00aa;
				IL_00aa:
				try
				{
					if (num != 1 && !catalogPackageRefuge._lowLevelDal.FileSystem.StatIfExists(_003CtargetPath_003E5__2).Exists)
					{
						goto IL_0144;
					}
					try
					{
						ConfiguredTaskAwaiter<RescuedCatalogPackage> awaiter2;
						if (num != 1)
						{
							awaiter2 = catalogPackageRefuge.ParseDirectoryWithLock(_003CtargetPath_003E5__2).ConfigureAwait(false).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RescuedCatalogPackage>, _003CGetRescuedPackageByGtiInfoOrNull_003Ed__16>(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<RescuedCatalogPackage>);
							num = (_003C_003E1__state = -1);
						}
						result2 = awaiter2.GetResult();
					}
					catch (FormatException)
					{
						goto IL_0144;
					}
					goto end_IL_00aa;
					IL_0144:
					result2 = null;
					end_IL_00aa:;
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
				_003CtargetPath_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CtargetPath_003E5__2 = null;
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
	private struct _003CGetRescuedPackageByPathOrNull_003Ed__17 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IRescuedCatalogPackage> _003C_003Et__builder;

		public CatalogPackageRefuge _003C_003E4__this;

		public string packagePath;

		private System.IDisposable _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<RescuedCatalogPackage> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogPackageRefuge catalogPackageRefuge = _003C_003E4__this;
			IRescuedCatalogPackage result2;
			try
			{
				if ((uint)num > 1u && !catalogPackageRefuge.TryGetGtiDetailsFromPath(packagePath, out string _, out Version _))
				{
					goto IL_0138;
				}
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						if (num == 1)
						{
							goto IL_00ab;
						}
						awaiter = catalogPackageRefuge._refugeLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetRescuedPackageByPathOrNull_003Ed__17>(ref awaiter, ref this);
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
					goto IL_00ab;
					IL_00ab:
					try
					{
						ConfiguredTaskAwaiter<RescuedCatalogPackage> awaiter2;
						if (num != 1)
						{
							awaiter2 = catalogPackageRefuge.ParseDirectoryWithLock(packagePath).ConfigureAwait(false).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RescuedCatalogPackage>, _003CGetRescuedPackageByPathOrNull_003Ed__17>(ref awaiter2, ref this);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<RescuedCatalogPackage>);
							num = (_003C_003E1__state = -1);
						}
						result2 = awaiter2.GetResult();
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap1 != null)
						{
							_003C_003E7__wrap1.Dispose();
						}
					}
				}
				catch (FormatException)
				{
					goto IL_0138;
				}
				goto end_IL_000e;
				IL_0138:
				result2 = null;
				end_IL_000e:;
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
	private struct _003CParseDirectoryWithLock_003Ed__25 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<RescuedCatalogPackage> _003C_003Et__builder;

		public CatalogPackageRefuge _003C_003E4__this;

		public string rescueDirectory;

		private string _003CgtiName_003E5__2;

		private Version _003CgtiVersion_003E5__3;

		private ConfiguredTaskAwaiter<List<Version>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogPackageRefuge catalogPackageRefuge = _003C_003E4__this;
			RescuedCatalogPackage result2;
			try
			{
				ConfiguredTaskAwaiter<List<Version>> awaiter;
				if (num != 0)
				{
					if (!catalogPackageRefuge.TryGetGtiDetailsFromPath(rescueDirectory, out _003CgtiName_003E5__2, out _003CgtiVersion_003E5__3))
					{
						throw new FormatException("Invalid rescue directory path.");
					}
					if (!catalogPackageRefuge._lowLevelDal.FileSystem.PathExists(rescueDirectory))
					{
						throw new FormatException("Rescue directory does not exist.");
					}
					string catalogRecordPath = GetCatalogRecordPath(rescueDirectory);
					if (!catalogPackageRefuge._lowLevelDal.FileSystem.PathExists(catalogRecordPath))
					{
						throw new FormatException("Rescue directory catalog record file does not exist.");
					}
					awaiter = catalogPackageRefuge.ParseVersionsWithLock(catalogRecordPath).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<List<Version>>, _003CParseDirectoryWithLock_003Ed__25>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<List<Version>>);
					num = (_003C_003E1__state = -1);
				}
				List<Version> result = awaiter.GetResult();
				if (result.Count == 0)
				{
					throw new FormatException("No recorded catalog versions.");
				}
				result2 = new RescuedCatalogPackage(_003CgtiName_003E5__2, _003CgtiVersion_003E5__3, (System.Collections.Generic.IEnumerable<Version>)result, PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, rescueDirectory));
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CgtiName_003E5__2 = null;
				_003CgtiVersion_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CgtiName_003E5__2 = null;
			_003CgtiVersion_003E5__3 = null;
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
	private struct _003CParseVersionsWithLock_003Ed__26 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<List<Version>> _003C_003Et__builder;

		public CatalogPackageRefuge _003C_003E4__this;

		public string catalogRecordPath;

		private ConfiguredTaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogPackageRefuge catalogPackageRefuge = _003C_003E4__this;
			List<Version> result;
			try
			{
				ConfiguredTaskAwaiter<string> awaiter;
				if (num != 0)
				{
					awaiter = catalogPackageRefuge._lowLevelDal.FileSystem.ReadAllText(catalogRecordPath, REFUGE_FILE_ENCODING).ConfigureAwait(false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<string>, _003CParseVersionsWithLock_003Ed__26>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<string>);
					num = (_003C_003E1__state = -1);
				}
				result = Enumerable.ToList<Version>(Enumerable.Select<string, Version>((System.Collections.Generic.IEnumerable<string>)awaiter.GetResult().Split(new string[2] { "\r\n", "\n" }, (StringSplitOptions)1), (Func<string, Version>)delegate(string vl)
				{
					Version result2 = default(Version);
					if (Version.TryParse(vl.Trim(), ref result2))
					{
						return result2;
					}
					Logger.Log(LogLevel.Warning, "Invalid Catalog version recorded: '" + vl + "'");
					return (Version)null;
				}).WhereClassNotNull());
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
	private struct _003CRecordAnotherCatalogVersion_003Ed__20 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IRescuedCatalogPackage> _003C_003Et__builder;

		public IRescuedCatalogPackage rescuedPackage;

		public CatalogPackageRefuge _003C_003E4__this;

		public Version catalogVersion;

		private string _003CrefugePath_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter _003C_003Eu__2;

		private ConfiguredTaskAwaiter<RescuedCatalogPackage> _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogPackageRefuge catalogPackageRefuge = _003C_003E4__this;
			IRescuedCatalogPackage result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_00d6;
					}
					_003CrefugePath_003E5__2 = rescuedPackage.RescuedDirectory.PackageDirectory.FullPath;
					if (!catalogPackageRefuge.TryGetGtiDetailsFromPath(_003CrefugePath_003E5__2, out string _, out Version _))
					{
						throw new ArgumentException("This package was not retrieved from this refuge.", "rescuedPackage");
					}
					awaiter = catalogPackageRefuge._refugeLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CRecordAnotherCatalogVersion_003Ed__20>(ref awaiter, ref this);
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
				goto IL_00d6;
				IL_00d6:
				try
				{
					ConfiguredTaskAwaiter<RescuedCatalogPackage> awaiter2;
					ConfiguredTaskAwaiter awaiter3;
					if (num != 1)
					{
						if (num == 2)
						{
							awaiter2 = _003C_003Eu__3;
							_003C_003Eu__3 = default(ConfiguredTaskAwaiter<RescuedCatalogPackage>);
							num = (_003C_003E1__state = -1);
							goto IL_01b9;
						}
						ConfiguredTaskAwaitable val = catalogPackageRefuge.AddCatalogVersionWithLock(_003CrefugePath_003E5__2, catalogVersion).ConfigureAwait(false);
						awaiter3 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRecordAnotherCatalogVersion_003Ed__20>(ref awaiter3, ref this);
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
					awaiter2 = catalogPackageRefuge.ParseDirectoryWithLock(_003CrefugePath_003E5__2).ConfigureAwait(false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RescuedCatalogPackage>, _003CRecordAnotherCatalogVersion_003Ed__20>(ref awaiter2, ref this);
						return;
					}
					goto IL_01b9;
					IL_01b9:
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
				_003CrefugePath_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CrefugePath_003E5__2 = null;
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
	private struct _003CRescuePackage_003Ed__19 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IRescuedCatalogPackage> _003C_003Et__builder;

		public CatalogPackageRefuge _003C_003E4__this;

		public string gtiAppName;

		public Version appCatalogedVersion;

		public Version catalogVersion;

		public string catalogDirPath;

		private string _003CtargetDirPath_003E5__2;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private string _003CtmpDirPath_003E5__4;

		private ConfiguredTaskAwaiter<RescuedCatalogPackage> _003C_003Eu__2;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogPackageRefuge catalogPackageRefuge = _003C_003E4__this;
			IRescuedCatalogPackage result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 4u)
					{
						goto IL_00ac;
					}
					_003CtargetDirPath_003E5__2 = catalogPackageRefuge.GetRescueDirPath(gtiAppName, appCatalogedVersion);
					awaiter = catalogPackageRefuge._refugeLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CRescuePackage_003Ed__19>(ref awaiter, ref this);
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
				goto IL_00ac;
				IL_00ac:
				try
				{
					ConfiguredTaskAwaitable val2;
					ConfiguredTaskAwaiter awaiter3;
					ConfiguredTaskAwaiter<RescuedCatalogPackage> awaiter2;
					string tmpRootDir;
					Guid val3;
					switch (num)
					{
					default:
						if (catalogPackageRefuge._lowLevelDal.FileSystem.PathExists(_003CtargetDirPath_003E5__2) && Enumerable.Any<FileSystemEntry>(catalogPackageRefuge._lowLevelDal.FileSystem.RecursiveEntriesIn(_003CtargetDirPath_003E5__2)))
						{
							goto case 1;
						}
						goto IL_02a8;
					case 1:
					{
						bool flag;
						try
						{
							if (num != 1)
							{
								awaiter2 = catalogPackageRefuge.ParseDirectoryWithLock(_003CtargetDirPath_003E5__2).ConfigureAwait(false).GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 1);
									_003C_003Eu__2 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RescuedCatalogPackage>, _003CRescuePackage_003Ed__19>(ref awaiter2, ref this);
									return;
								}
							}
							else
							{
								awaiter2 = _003C_003Eu__2;
								_003C_003Eu__2 = default(ConfiguredTaskAwaiter<RescuedCatalogPackage>);
								num = (_003C_003E1__state = -1);
							}
							awaiter2.GetResult();
							flag = true;
						}
						catch (FormatException)
						{
							Logger.Log(LogLevel.Warning, "Deleting broken rescue directory: " + _003CtargetDirPath_003E5__2);
							catalogPackageRefuge._lowLevelDal.FileSystem.DeleteAnythingNotLocked(_003CtargetDirPath_003E5__2);
							flag = false;
						}
						if (flag)
						{
							Logger.Log(LogLevel.Warning, "Duplicate rescue detected. The caller should just use RecordAnotherCatalogVersion.");
							val2 = catalogPackageRefuge.AddCatalogVersionWithLock(_003CtargetDirPath_003E5__2, catalogVersion).ConfigureAwait(false);
							awaiter3 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter3;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRescuePackage_003Ed__19>(ref awaiter3, ref this);
								return;
							}
							goto IL_022d;
						}
						goto IL_02a8;
					}
					case 2:
						awaiter3 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_022d;
					case 3:
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<RescuedCatalogPackage>);
						num = (_003C_003E1__state = -1);
						goto IL_029b;
					case 4:
						awaiter3 = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0370;
					case 5:
						{
							awaiter2 = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<RescuedCatalogPackage>);
							num = (_003C_003E1__state = -1);
							break;
						}
						IL_022d:
						((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
						awaiter2 = catalogPackageRefuge.ParseDirectoryWithLock(_003CtargetDirPath_003E5__2).ConfigureAwait(false).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 3);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RescuedCatalogPackage>, _003CRescuePackage_003Ed__19>(ref awaiter2, ref this);
							return;
						}
						goto IL_029b;
						IL_0370:
						((ConfiguredTaskAwaiter)(ref awaiter3)).GetResult();
						if (!catalogPackageRefuge._lowLevelDal.FileSystem.DeleteDirectoryIfEmpty(_003CtargetDirPath_003E5__2))
						{
							Logger.Log(LogLevel.Error, "A refuge package already existed at (" + _003CtargetDirPath_003E5__2 + ") and attempts to fix it failed. The directory or a file in it may be locked by another program.");
						}
						catalogPackageRefuge._lowLevelDal.FileSystem.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, _003CtmpDirPath_003E5__4, _003CtargetDirPath_003E5__2);
						awaiter2 = catalogPackageRefuge.ParseDirectoryWithLock(_003CtargetDirPath_003E5__2).ConfigureAwait(false).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 5);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<RescuedCatalogPackage>, _003CRescuePackage_003Ed__19>(ref awaiter2, ref this);
							return;
						}
						break;
						IL_029b:
						result2 = awaiter2.GetResult();
						goto end_IL_00ac;
						IL_02a8:
						tmpRootDir = catalogPackageRefuge.TmpRootDir;
						val3 = Guid.NewGuid();
						_003CtmpDirPath_003E5__4 = Path.Combine(tmpRootDir, ((object)(Guid)(ref val3)).ToString());
						catalogPackageRefuge._lowLevelDal.FileSystem.CreateDirectory(_003CtmpDirPath_003E5__4);
						catalogPackageRefuge._lowLevelDal.FileSystem.CopyDirectory(catalogDirPath, _003CtmpDirPath_003E5__4);
						val2 = catalogPackageRefuge.AddCatalogVersionWithLock(_003CtmpDirPath_003E5__4, catalogVersion).ConfigureAwait(false);
						awaiter3 = ((ConfiguredTaskAwaitable)(ref val2)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
						{
							num = (_003C_003E1__state = 4);
							_003C_003Eu__3 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CRescuePackage_003Ed__19>(ref awaiter3, ref this);
							return;
						}
						goto IL_0370;
					}
					result2 = awaiter2.GetResult();
					end_IL_00ac:;
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
				_003CtargetDirPath_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CtargetDirPath_003E5__2 = null;
			_003C_003Et__builder.SetResult(result2);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private const string TEMP_DIRECTORY_NAME = ".tmp";

	private static readonly Encoding REFUGE_FILE_ENCODING = Encoding.UTF8;

	private readonly ILowLevelDal _lowLevelDal;

	private readonly object _deadListLock = new object();

	private readonly AsyncLock _refugeLock = new AsyncLock();

	[field: CompilerGenerated]
	private string RefugeRootDir
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	private string TmpRootDir
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	private string DeadListPath
	{
		[CompilerGenerated]
		get;
	}

	public CatalogPackageRefuge(ILowLevelDal lowLevelDal)
	{
		_lowLevelDal = lowLevelDal;
		RefugeRootDir = Path.Combine(_lowLevelDal.Deployment.UserAppDataDirPath, "SafePacks");
		string path = Path.Combine(RefugeRootDir, ".tmp");
		TmpRootDir = Path.Combine(lowLevelDal.Deployment.PerProcessTempDirectory(), "SafePacks");
		DeadListPath = Path.Combine(RefugeRootDir, "dead.txt");
		_lowLevelDal.FileSystem.DeleteAnythingNotLocked(path);
		if (!_lowLevelDal.FileSystem.PathExists(DeadListPath))
		{
			return;
		}
		List<string> val = Enumerable.ToList<string>(Enumerable.Where<string>(Enumerable.Select<string, string>(Enumerable.Where<string>(Enumerable.Select<string, string>(_lowLevelDal.FileSystem.ReadLinesSync(DeadListPath, REFUGE_FILE_ENCODING).WhereClassNotNull(), (Func<string, string>)([CompilerGenerated] (string line) => Path.Combine(RefugeRootDir, line))), (Func<string, bool>)([CompilerGenerated] (string refugePath) => _lowLevelDal.FileSystem.PathExists(refugePath))), (Func<string, string>)([CompilerGenerated] (string refugePath) =>
		{
			try
			{
				_lowLevelDal.FileSystem.DeleteAnythingNotLocked(refugePath);
				_lowLevelDal.FileSystem.DeleteDirectoryIfEmptyAndParentsThatAreEmpty(refugePath, RefugeRootDir);
				return _lowLevelDal.FileSystem.PathExists(refugePath) ? refugePath : string.Empty;
			}
			catch (System.Exception ex2) when (ex2 is FileNotFoundException || ex2 is DirectoryNotFoundException)
			{
				return string.Empty;
			}
		})), (Func<string, bool>)((string refugePath) => refugePath.Length != 0)));
		if (val.Count > 0)
		{
			_lowLevelDal.FileSystem.WriteAllTextSync(DeadListPath, string.Join("\n", (System.Collections.Generic.IEnumerable<string>)val), REFUGE_FILE_ENCODING);
			return;
		}
		try
		{
			_lowLevelDal.FileSystem.DeleteAnythingNotLocked(DeadListPath);
		}
		catch (System.Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
		{
		}
	}

	public void MarkDeadPackage(PotentialPackageDirectory deadPackage)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		string text = PathHelpers.CreateRelativePath(RefugeRootDir, deadPackage.PackageDirectory.FullPath);
		if (!PathHelpers.PathsAreOnTheSameDrive(RefugeRootDir, deadPackage.PackageDirectory.FullPath) || PathHelpers.PathEscapesIntoParent(text))
		{
			throw new ArgumentException("This package directory did not come from the refuge.");
		}
		lock (_deadListLock)
		{
			_lowLevelDal.FileSystem.AppendTextSync(DeadListPath, text, REFUGE_FILE_ENCODING);
		}
	}

	[AsyncStateMachine(typeof(_003CGetRescuedPackageByGtiInfoOrNull_003Ed__16))]
	public async System.Threading.Tasks.Task<IRescuedCatalogPackage?> GetRescuedPackageByGtiInfoOrNull(string gtiAppName, Version appCatalogedVersion)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string targetPath = GetRescueDirPath(gtiAppName, appCatalogedVersion);
		using (await _refugeLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			if (_lowLevelDal.FileSystem.StatIfExists(targetPath).Exists)
			{
				try
				{
					return await ParseDirectoryWithLock(targetPath).ConfigureAwait(false);
				}
				catch (FormatException)
				{
				}
			}
			return null;
		}
	}

	[AsyncStateMachine(typeof(_003CGetRescuedPackageByPathOrNull_003Ed__17))]
	public async System.Threading.Tasks.Task<IRescuedCatalogPackage?> GetRescuedPackageByPathOrNull(string packagePath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetGtiDetailsFromPath(packagePath, out string _, out Version _))
		{
			try
			{
				using (await _refugeLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
				{
					return await ParseDirectoryWithLock(packagePath).ConfigureAwait(false);
				}
			}
			catch (FormatException)
			{
			}
		}
		return null;
	}

	public IObservable<IRescuedCatalogPackage> ListRescuedPackages()
	{
		return Observable.DeferAsync<IRescuedCatalogPackage>([AsyncStateMachine(typeof(_003C_003CListRescuedPackages_003Eb__18_0_003Ed))] [CompilerGenerated] async (CancellationToken t) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (_lowLevelDal.FileSystem.PathExists(RefugeRootDir))
			{
				System.IDisposable i = await _refugeLock.LockAsync(t).ConfigureAwait(continueOnCapturedContext: false);
				try
				{
					return (IObservable<IRescuedCatalogPackage>)(object)Enumerable.Where<FileSystemEntry>(_lowLevelDal.FileSystem.DirectoriesIn(RefugeRootDir), (Func<FileSystemEntry, bool>)((FileSystemEntry fse) => fse.Name != ".tmp" && !fse.Name.StartsWith(".tmp_", (StringComparison)4))).ToObservable().SelectMany<FileSystemEntry, FileSystemEntry>((Func<FileSystemEntry, System.Collections.Generic.IEnumerable<FileSystemEntry>>)([CompilerGenerated] (FileSystemEntry gtiAppDir) => _lowLevelDal.FileSystem.DirectoriesIn(gtiAppDir.FullPath)))
						.Where<FileSystemEntry>((Func<FileSystemEntry, bool>)((FileSystemEntry fse) => !fse.Name.StartsWith(".tmp_", (StringComparison)4)))
						.SelectMany<FileSystemEntry, RescuedCatalogPackage>((Func<FileSystemEntry, System.Threading.Tasks.Task<RescuedCatalogPackage>>)([AsyncStateMachine(typeof(_003C_003CListRescuedPackages_003Eb__18_4_003Ed))] [CompilerGenerated] async (FileSystemEntry safePackDir) =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							try
							{
								return await ParseDirectoryWithLock(safePackDir.FullPath).ConfigureAwait(false);
							}
							catch (FormatException val2)
							{
								Logger.LogFormattedException((System.Exception)val2, "Listing refuge directories");
								return (RescuedCatalogPackage)null;
							}
						}))
						.WhereClassNotNull<RescuedCatalogPackage>()
						.Finally<RescuedCatalogPackage>((Action)delegate
						{
							i.Dispose();
						});
				}
				catch (DirectoryNotFoundException)
				{
					i.Dispose();
				}
				catch (System.Exception)
				{
					i.Dispose();
					throw;
				}
			}
			return Observable.Empty<IRescuedCatalogPackage>();
		});
	}

	[AsyncStateMachine(typeof(_003CRescuePackage_003Ed__19))]
	public async System.Threading.Tasks.Task<IRescuedCatalogPackage> RescuePackage(Version catalogVersion, string gtiAppName, Version appCatalogedVersion, string catalogDirPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string targetDirPath = GetRescueDirPath(gtiAppName, appCatalogedVersion);
		using (await _refugeLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			if (_lowLevelDal.FileSystem.PathExists(targetDirPath) && Enumerable.Any<FileSystemEntry>(_lowLevelDal.FileSystem.RecursiveEntriesIn(targetDirPath)))
			{
				bool flag;
				try
				{
					await ParseDirectoryWithLock(targetDirPath).ConfigureAwait(false);
					flag = true;
				}
				catch (FormatException)
				{
					Logger.Log(LogLevel.Warning, "Deleting broken rescue directory: " + targetDirPath);
					_lowLevelDal.FileSystem.DeleteAnythingNotLocked(targetDirPath);
					flag = false;
				}
				if (flag)
				{
					Logger.Log(LogLevel.Warning, "Duplicate rescue detected. The caller should just use RecordAnotherCatalogVersion.");
					await AddCatalogVersionWithLock(targetDirPath, catalogVersion).ConfigureAwait(false);
					return await ParseDirectoryWithLock(targetDirPath).ConfigureAwait(false);
				}
			}
			string tmpRootDir = TmpRootDir;
			Guid val2 = Guid.NewGuid();
			string tmpDirPath = Path.Combine(tmpRootDir, ((object)(Guid)(ref val2)).ToString());
			_lowLevelDal.FileSystem.CreateDirectory(tmpDirPath);
			_lowLevelDal.FileSystem.CopyDirectory(catalogDirPath, tmpDirPath);
			await AddCatalogVersionWithLock(tmpDirPath, catalogVersion).ConfigureAwait(false);
			if (!_lowLevelDal.FileSystem.DeleteDirectoryIfEmpty(targetDirPath))
			{
				Logger.Log(LogLevel.Error, "A refuge package already existed at (" + targetDirPath + ") and attempts to fix it failed. The directory or a file in it may be locked by another program.");
			}
			_lowLevelDal.FileSystem.MoveOrCopyAndDeleteToDifferentDrive(LolaFsExtensions.DefaultTempFactory, tmpDirPath, targetDirPath);
			return await ParseDirectoryWithLock(targetDirPath).ConfigureAwait(false);
		}
	}

	[AsyncStateMachine(typeof(_003CRecordAnotherCatalogVersion_003Ed__20))]
	public async System.Threading.Tasks.Task<IRescuedCatalogPackage> RecordAnotherCatalogVersion(IRescuedCatalogPackage rescuedPackage, Version catalogVersion)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string refugePath = rescuedPackage.RescuedDirectory.PackageDirectory.FullPath;
		if (!TryGetGtiDetailsFromPath(refugePath, out string _, out Version _))
		{
			throw new ArgumentException("This package was not retrieved from this refuge.", "rescuedPackage");
		}
		using (await _refugeLock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			await AddCatalogVersionWithLock(refugePath, catalogVersion).ConfigureAwait(false);
			return await ParseDirectoryWithLock(refugePath).ConfigureAwait(false);
		}
	}

	private static string GetCatalogRecordPath(string rescueDirectory)
	{
		return Path.Combine(rescueDirectory, ".catalogs");
	}

	[AsyncStateMachine(typeof(_003CAddCatalogVersionWithLock_003Ed__22))]
	private System.Threading.Tasks.Task AddCatalogVersionWithLock(string targetDirPath, Version catalogVersion)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CAddCatalogVersionWithLock_003Ed__22 _003CAddCatalogVersionWithLock_003Ed__ = default(_003CAddCatalogVersionWithLock_003Ed__22);
		_003CAddCatalogVersionWithLock_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CAddCatalogVersionWithLock_003Ed__._003C_003E4__this = this;
		_003CAddCatalogVersionWithLock_003Ed__.targetDirPath = targetDirPath;
		_003CAddCatalogVersionWithLock_003Ed__.catalogVersion = catalogVersion;
		_003CAddCatalogVersionWithLock_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CAddCatalogVersionWithLock_003Ed__._003C_003Et__builder)).Start<_003CAddCatalogVersionWithLock_003Ed__22>(ref _003CAddCatalogVersionWithLock_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CAddCatalogVersionWithLock_003Ed__._003C_003Et__builder)).Task;
	}

	private bool TryGetGtiDetailsFromPath(string packagePath, out string gtiAppName, out Version appCatalogedVersion)
	{
		string path = PathHelpers.CreateRelativePath(RefugeRootDir, packagePath);
		if (PathHelpers.PathsAreOnTheSameDrive(RefugeRootDir, packagePath) && !PathHelpers.PathEscapesIntoParent(path))
		{
			string[] array = PathHelpers.SplitPath(path);
			Version val = default(Version);
			if (array.Length == 2 && Version.TryParse(array[1], ref val))
			{
				gtiAppName = array[0];
				appCatalogedVersion = val;
				return true;
			}
		}
		gtiAppName = string.Empty;
		appCatalogedVersion = null;
		return false;
	}

	private string GetRescueDirPath(string gtiAppName, Version appCatalogedVersion)
	{
		return Path.Combine(RefugeRootDir, gtiAppName, ((object)appCatalogedVersion).ToString());
	}

	[AsyncStateMachine(typeof(_003CParseDirectoryWithLock_003Ed__25))]
	private async System.Threading.Tasks.Task<RescuedCatalogPackage> ParseDirectoryWithLock(string rescueDirectory)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetGtiDetailsFromPath(rescueDirectory, out string gtiName, out Version gtiVersion))
		{
			throw new FormatException("Invalid rescue directory path.");
		}
		if (!_lowLevelDal.FileSystem.PathExists(rescueDirectory))
		{
			throw new FormatException("Rescue directory does not exist.");
		}
		string catalogRecordPath = GetCatalogRecordPath(rescueDirectory);
		if (!_lowLevelDal.FileSystem.PathExists(catalogRecordPath))
		{
			throw new FormatException("Rescue directory catalog record file does not exist.");
		}
		List<Version> val = await ParseVersionsWithLock(catalogRecordPath).ConfigureAwait(false);
		if (val.Count == 0)
		{
			throw new FormatException("No recorded catalog versions.");
		}
		return new RescuedCatalogPackage(gtiName, gtiVersion, (System.Collections.Generic.IEnumerable<Version>)val, PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, rescueDirectory));
	}

	[AsyncStateMachine(typeof(_003CParseVersionsWithLock_003Ed__26))]
	private async System.Threading.Tasks.Task<List<Version>> ParseVersionsWithLock(string catalogRecordPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Enumerable.ToList<Version>(Enumerable.Select<string, Version>((System.Collections.Generic.IEnumerable<string>)(await _lowLevelDal.FileSystem.ReadAllText(catalogRecordPath, REFUGE_FILE_ENCODING).ConfigureAwait(false)).Split(new string[2] { "\r\n", "\n" }, (StringSplitOptions)1), (Func<string, Version>)delegate(string vl)
		{
			Version result = default(Version);
			if (Version.TryParse(vl.Trim(), ref result))
			{
				return result;
			}
			Logger.Log(LogLevel.Warning, "Invalid Catalog version recorded: '" + vl + "'");
			return (Version)null;
		}).WhereClassNotNull());
	}

	[CompilerGenerated]
	internal static string _003CAddCatalogVersionWithLock_003Eg__FormatVersionLine_007C22_0(Version version)
	{
		return $"{version}\n";
	}
}
