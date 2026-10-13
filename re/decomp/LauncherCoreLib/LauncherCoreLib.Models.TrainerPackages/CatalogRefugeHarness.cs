using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GTI.Net;
using LOLA.Tasks;
using LauncherCoreLib.Models.DAL;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class CatalogRefugeHarness
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass5_0
	{
		public CatalogRefugeHarness _003C_003E4__this;

		public IRescuedCatalogPackage previousRescue;
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CHandleGtiListing_003Ed__5 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CatalogRefugeHarness _003C_003E4__this;

		public GtiListingEntry entry;

		private _003C_003Ec__DisplayClass5_0 _003C_003E8__1;

		public Version catalogVersion;

		private ConfiguredTaskAwaiter<IRescuedCatalogPackage> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__2;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__3;

		private ConfiguredTaskAwaiter<PotentialPackageDirectory> _003C_003Eu__4;

		private void MoveNext()
		{
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_0597: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_055f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0501: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			CatalogRefugeHarness catalogRefugeHarness = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<PotentialPackageDirectory> awaiter4;
				ConfiguredTaskAwaiter<IRescuedCatalogPackage> awaiter3;
				ConfiguredTaskAwaiter<bool> awaiter2;
				ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
				PotentialPackageDirectory result;
				ITrainerPackage result2;
				ITrainerPackage result3;
				IRescuedCatalogPackage result4;
				switch (num)
				{
				default:
					_003C_003E8__1 = new _003C_003Ec__DisplayClass5_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					if (entry.CatalogedVersion != (Version)null && entry.CatalogedPath != null && catalogRefugeHarness._unitOfWork.LowLevel.FileSystem.PathExists(entry.CatalogedPath))
					{
						awaiter3 = catalogRefugeHarness._packageRefuge.GetRescuedPackageByGtiInfoOrNull(entry.EntryName, entry.CatalogedVersion).DetachContext().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter3;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IRescuedCatalogPackage>, _003CHandleGtiListing_003Ed__5>(ref awaiter3, ref this);
							return;
						}
						goto IL_011b;
					}
					goto end_IL_000e;
				case 0:
					awaiter3 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<IRescuedCatalogPackage>);
					num = (_003C_003E1__state = -1);
					goto IL_011b;
				case 1:
				case 2:
					try
					{
						if (num != 1)
						{
							if (num == 2)
							{
								awaiter2 = _003C_003Eu__3;
								_003C_003Eu__3 = default(ConfiguredTaskAwaiter<bool>);
								num = (_003C_003E1__state = -1);
								goto IL_0231;
							}
							awaiter = catalogRefugeHarness._packageParsing.ParsePackage(PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, entry.CatalogedPath)).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__2 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CHandleGtiListing_003Ed__5>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
							num = (_003C_003E1__state = -1);
						}
						ITrainerPackage result5 = awaiter.GetResult();
						awaiter2 = catalogRefugeHarness._packageScanner.CanUsePackage(result5).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 2);
							_003C_003Eu__3 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CHandleGtiListing_003Ed__5>(ref awaiter2, ref this);
							return;
						}
						goto IL_0231;
						IL_0231:
						if (awaiter2.GetResult())
						{
							goto end_IL_0140;
						}
						goto end_IL_000e;
						end_IL_0140:;
					}
					catch (TrainerPackageLoadException)
					{
						goto end_IL_000e;
					}
					awaiter4 = catalogRefugeHarness._packageRefuge.CopyToRefugeAndFallbackOnCatalog(catalogVersion, entry).DetachContext().GetAwaiter();
					if (!awaiter4.IsCompleted)
					{
						num = (_003C_003E1__state = 3);
						_003C_003Eu__4 = awaiter4;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<PotentialPackageDirectory>, _003CHandleGtiListing_003Ed__5>(ref awaiter4, ref this);
						return;
					}
					goto IL_02b8;
				case 3:
					awaiter4 = _003C_003Eu__4;
					_003C_003Eu__4 = default(ConfiguredTaskAwaiter<PotentialPackageDirectory>);
					num = (_003C_003E1__state = -1);
					goto IL_02b8;
				case 4:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
					num = (_003C_003E1__state = -1);
					goto IL_0328;
				case 5:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
					num = (_003C_003E1__state = -1);
					goto IL_03a2;
				case 6:
					awaiter3 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<IRescuedCatalogPackage>);
					num = (_003C_003E1__state = -1);
					goto IL_0443;
				case 7:
					awaiter2 = _003C_003Eu__3;
					_003C_003Eu__3 = default(ConfiguredTaskAwaiter<bool>);
					num = (_003C_003E1__state = -1);
					goto IL_04b5;
				case 8:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
					num = (_003C_003E1__state = -1);
					goto IL_0536;
				case 9:
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_02b8:
					result = awaiter4.GetResult();
					awaiter = catalogRefugeHarness._packageParsing.ParsePackage(result).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 4);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CHandleGtiListing_003Ed__5>(ref awaiter, ref this);
						return;
					}
					goto IL_0328;
					IL_04b5:
					if (!awaiter2.GetResult())
					{
						awaiter = catalogRefugeHarness._packageParsing.ParsePackage(_003C_003E8__1.previousRescue.RescuedDirectory).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 8);
							_003C_003Eu__2 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CHandleGtiListing_003Ed__5>(ref awaiter, ref this);
							return;
						}
						goto IL_0536;
					}
					goto end_IL_000e;
					IL_0328:
					result2 = awaiter.GetResult();
					awaiter = catalogRefugeHarness._unitOfWork.PackageRepos.Packages.RegisterPackage(result2).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 5);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CHandleGtiListing_003Ed__5>(ref awaiter, ref this);
						return;
					}
					goto IL_03a2;
					IL_0536:
					result3 = awaiter.GetResult();
					awaiter = catalogRefugeHarness._unitOfWork.PackageRepos.Packages.RegisterPackage(result3).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 9);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CHandleGtiListing_003Ed__5>(ref awaiter, ref this);
						return;
					}
					break;
					IL_011b:
					result4 = awaiter3.GetResult();
					_003C_003E8__1.previousRescue = result4;
					if (_003C_003E8__1.previousRescue == null)
					{
						goto case 1;
					}
					if (ImmutableArray.BinarySearch<Version>(_003C_003E8__1.previousRescue.CatalogVersions, catalogVersion) < 0)
					{
						awaiter3 = catalogRefugeHarness._packageRefuge.RecordAnotherCatalogVersion(_003C_003E8__1.previousRescue, catalogVersion).DetachContext().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 6);
							_003C_003Eu__1 = awaiter3;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IRescuedCatalogPackage>, _003CHandleGtiListing_003Ed__5>(ref awaiter3, ref this);
							return;
						}
						goto IL_0443;
					}
					awaiter2 = DirectoryIsRegistered().DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 7);
						_003C_003Eu__3 = awaiter2;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CHandleGtiListing_003Ed__5>(ref awaiter2, ref this);
						return;
					}
					goto IL_04b5;
					IL_03a2:
					awaiter.GetResult();
					goto end_IL_000e;
					IL_0443:
					awaiter3.GetResult();
					goto end_IL_000e;
				}
				awaiter.GetResult();
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
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass5_0._003C_003CHandleGtiListing_003Eg__DirectoryIsRegistered_007C0_003Ed))]
			async System.Threading.Tasks.Task<bool> DirectoryIsRegistered()
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				try
				{
					return await ((_003C_003Ec__DisplayClass5_0)(object)this)._003C_003E4__this._unitOfWork.PackageRepos.Packages.GetPackageByLocationOrNull(((_003C_003Ec__DisplayClass5_0)(object)this).previousRescue.RescuedDirectory.PackageDirectory.FullPath).DetachContext() != null;
				}
				catch (KeyNotFoundException)
				{
					return false;
				}
			}
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}
	}

	private readonly IUnitOfWork _unitOfWork;

	private readonly ICatalogPackageRefuge _packageRefuge;

	private readonly ITrainerPackageParser _packageParsing;

	private readonly IPackageScanner _packageScanner;

	public CatalogRefugeHarness(IUnitOfWork unitOfWork, ICatalogPackageRefuge copier, ITrainerPackageParser packageParsing, IPackageScanner scanner)
	{
		_unitOfWork = unitOfWork;
		_packageRefuge = copier;
		_packageParsing = packageParsing;
		_packageScanner = scanner;
	}

	[AsyncStateMachine(typeof(_003CHandleGtiListing_003Ed__5))]
	public System.Threading.Tasks.Task HandleGtiListing(Version catalogVersion, GtiListingEntry entry)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CHandleGtiListing_003Ed__5 _003CHandleGtiListing_003Ed__ = default(_003CHandleGtiListing_003Ed__5);
		_003CHandleGtiListing_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CHandleGtiListing_003Ed__._003C_003E4__this = this;
		_003CHandleGtiListing_003Ed__.catalogVersion = catalogVersion;
		_003CHandleGtiListing_003Ed__.entry = entry;
		_003CHandleGtiListing_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CHandleGtiListing_003Ed__._003C_003Et__builder)).Start<_003CHandleGtiListing_003Ed__5>(ref _003CHandleGtiListing_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CHandleGtiListing_003Ed__._003C_003Et__builder)).Task;
	}
}
