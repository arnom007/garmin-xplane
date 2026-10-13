using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Tasks;
using LauncherCoreLib.Models.TrainerPackages;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class AvionicsRepo : IAvionicsRepo
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass4_0
	{
		public AvionicsModel cached;

		public Func<AvionicsModel, bool> _003C_003E9__0;

		internal bool _003CGetByModelKey_003Eb__0(AvionicsModel am)
		{
			return am.Package == cached.Package;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CAwaitPackageQueryAndThrowIfNull_003Ed__6 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ITrainerPackage> _003C_003Et__builder;

		public System.Threading.Tasks.Task<ITrainerPackage?> packageFound;

		private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__1;

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
			ITrainerPackage result;
			try
			{
				ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
				if (num != 0)
				{
					awaiter = packageFound.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CAwaitPackageQueryAndThrowIfNull_003Ed__6>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult() ?? throw new AvionicsNotFoundException("The package for this model could not be found.");
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
	private struct _003CGetByModelKey_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<AvionicsModel> _003C_003Et__builder;

		public AvionicsRepo _003C_003E4__this;

		public TrainerModelKey trainerModelKey;

		private _003C_003Ec__DisplayClass4_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			AvionicsRepo avionicsRepo = _003C_003E4__this;
			AvionicsModel cached;
			try
			{
				ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
				if (num != 0)
				{
					System.Threading.Tasks.Task<ITrainerPackage> packageById = default(System.Threading.Tasks.Task<ITrainerPackage>);
					if (num != 1)
					{
						_003C_003E8__1 = new _003C_003Ec__DisplayClass4_0();
						packageById = avionicsRepo._packages.GetPackageById(trainerModelKey.TrainerPackageId);
						if (!avionicsRepo._avionicsByModelKey.TryGetValue(trainerModelKey, ref _003C_003E8__1.cached))
						{
							awaiter = AwaitPackageQueryAndThrowIfNull(packageById).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CGetByModelKey_003Ed__4>(ref awaiter, ref this);
								return;
							}
							goto IL_00be;
						}
					}
					try
					{
						if (num != 1)
						{
							awaiter = AwaitPackageQueryAndThrowIfNull(packageById).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CGetByModelKey_003Ed__4>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
							num = (_003C_003E1__state = -1);
						}
						ITrainerPackage result = awaiter.GetResult();
						if (result != _003C_003E8__1.cached.Package)
						{
							_003C_003E8__1.cached = avionicsRepo.AddOrUpdateCache(trainerModelKey, result);
						}
					}
					catch (AvionicsNotFoundException)
					{
						if ((object)_003C_003E8__1.cached != null)
						{
							System.Collections.Generic.IEnumerator<AvionicsModel> enumerator = Enumerable.Where<AvionicsModel>(avionicsRepo._avionicsByModelKey.NonBlockingValues<TrainerModelKey, AvionicsModel>(), (Func<AvionicsModel, bool>)((AvionicsModel am) => am.Package == _003C_003E8__1.cached.Package)).GetEnumerator();
							try
							{
								AvionicsModel avionicsModel = default(AvionicsModel);
								while (((System.Collections.IEnumerator)enumerator).MoveNext())
								{
									AvionicsModel current = enumerator.Current;
									avionicsRepo._avionicsByModelKey.TryRemove(current.Key, ref avionicsModel);
								}
							}
							finally
							{
								if (num < 0)
								{
									((System.IDisposable)enumerator)?.Dispose();
								}
							}
						}
						throw;
					}
					goto IL_021a;
				}
				awaiter = _003C_003Eu__1;
				_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
				num = (_003C_003E1__state = -1);
				goto IL_00be;
				IL_021a:
				if ((object)_003C_003E8__1.cached == null)
				{
					throw new AvionicsNotFoundException();
				}
				cached = _003C_003E8__1.cached;
				goto end_IL_000e;
				IL_00be:
				ITrainerPackage result2 = awaiter.GetResult();
				_003C_003E8__1.cached = avionicsRepo.AddOrUpdateCache(trainerModelKey, result2);
				goto IL_021a;
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
			_003C_003Et__builder.SetResult(cached);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	private readonly ITrainerPackageRepoEvolved _packages;

	private readonly ConcurrentDictionary<TrainerModelKey, AvionicsModel> _avionicsByModelKey = new ConcurrentDictionary<TrainerModelKey, AvionicsModel>();

	public AvionicsRepo(ITrainerPackageRepoEvolved packages)
	{
		_packages = packages;
	}

	public IObservable<AvionicsModel> AllModels()
	{
		return _packages.GetAllInstallInfoOverTime().Take<IDictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>>(1).SelectMany<IDictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>, ITrainerPackage>((Func<IDictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo>, System.Collections.Generic.IEnumerable<ITrainerPackage>>)((IDictionary<TrainerEmbeddedReleaseId, PackageInstallationInfo> installInfoDictionary) => Enumerable.Select<PackageInstallationInfo, ITrainerPackage>((System.Collections.Generic.IEnumerable<PackageInstallationInfo>)installInfoDictionary.Values, (Func<PackageInstallationInfo, ITrainerPackage>)((PackageInstallationInfo installInfo) => installInfo.PrimaryPackageChecked))))
			.SelectMany<ITrainerPackage, AvionicsModel>((Func<ITrainerPackage, System.Collections.Generic.IEnumerable<AvionicsModel>>)((ITrainerPackage primaryPackage) => AvionicsModel.CreateFromPackage(primaryPackage)))
			.Select<AvionicsModel, AvionicsModel>((Func<AvionicsModel, AvionicsModel>)([CompilerGenerated] (AvionicsModel model) => KeepCachedOrReplaceIfPreviousPackageIsDifferent(model)));
	}

	[AsyncStateMachine(typeof(_003CGetByModelKey_003Ed__4))]
	public async System.Threading.Tasks.Task<AvionicsModel> GetByModelKey(TrainerModelKey trainerModelKey)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		System.Threading.Tasks.Task<ITrainerPackage> packageById = _packages.GetPackageById(trainerModelKey.TrainerPackageId);
		AvionicsModel cached = default(AvionicsModel);
		if (!_avionicsByModelKey.TryGetValue(trainerModelKey, ref cached))
		{
			cached = AddOrUpdateCache(trainerModelKey, await AwaitPackageQueryAndThrowIfNull(packageById).DetachContext());
		}
		else
		{
			try
			{
				ITrainerPackage trainerPackage = await AwaitPackageQueryAndThrowIfNull(packageById).DetachContext();
				if (trainerPackage != cached.Package)
				{
					cached = AddOrUpdateCache(trainerModelKey, trainerPackage);
				}
			}
			catch (AvionicsNotFoundException)
			{
				if ((object)cached != null)
				{
					System.Collections.Generic.IEnumerator<AvionicsModel> enumerator = Enumerable.Where<AvionicsModel>(_avionicsByModelKey.NonBlockingValues<TrainerModelKey, AvionicsModel>(), (Func<AvionicsModel, bool>)((AvionicsModel am) => am.Package == cached.Package)).GetEnumerator();
					try
					{
						AvionicsModel avionicsModel = default(AvionicsModel);
						while (((System.Collections.IEnumerator)enumerator).MoveNext())
						{
							AvionicsModel current = enumerator.Current;
							_avionicsByModelKey.TryRemove(current.Key, ref avionicsModel);
						}
					}
					finally
					{
						((System.IDisposable)enumerator)?.Dispose();
					}
				}
				throw;
			}
		}
		if ((object)cached == null)
		{
			throw new AvionicsNotFoundException();
		}
		return cached;
	}

	public System.Threading.Tasks.Task<System.Collections.Generic.IList<AvionicsModel>> RemoveAllForPackage(TrainerPackageIdentifier trainerPackageId)
	{
		TrainerPackageIdentifier trainerPackageId2 = trainerPackageId;
		List<AvionicsModel> val = new List<AvionicsModel>();
		System.Collections.Generic.IEnumerator<TrainerModelKey> enumerator = Enumerable.Where<TrainerModelKey>(_avionicsByModelKey.NonBlockingKeys<TrainerModelKey, AvionicsModel>(), (Func<TrainerModelKey, bool>)((TrainerModelKey k) => k.TrainerPackageId == trainerPackageId2)).GetEnumerator();
		try
		{
			AvionicsModel avionicsModel = default(AvionicsModel);
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				TrainerModelKey current = enumerator.Current;
				if (_avionicsByModelKey.TryRemove(current, ref avionicsModel))
				{
					val.Add(avionicsModel);
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		return System.Threading.Tasks.Task.FromResult<System.Collections.Generic.IList<AvionicsModel>>((System.Collections.Generic.IList<AvionicsModel>)val);
	}

	[AsyncStateMachine(typeof(_003CAwaitPackageQueryAndThrowIfNull_003Ed__6))]
	private static async System.Threading.Tasks.Task<ITrainerPackage> AwaitPackageQueryAndThrowIfNull(System.Threading.Tasks.Task<ITrainerPackage?> packageFound)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return (await packageFound.DetachContext()) ?? throw new AvionicsNotFoundException("The package for this model could not be found.");
	}

	private AvionicsModel? AddOrUpdateCache(TrainerModelKey trainerModelKey, ITrainerPackage package)
	{
		AvionicsModel result = null;
		System.Collections.Generic.IEnumerator<AvionicsModel> enumerator = AvionicsModel.CreateFromPackage(package).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				AvionicsModel current = enumerator.Current;
				AvionicsModel avionicsModel = KeepCachedOrReplaceIfPreviousPackageIsDifferent(current);
				if (avionicsModel.Key == trainerModelKey)
				{
					result = avionicsModel;
				}
			}
			return result;
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private AvionicsModel KeepCachedOrReplaceIfPreviousPackageIsDifferent(AvionicsModel am)
	{
		AvionicsModel am2 = am;
		return _avionicsByModelKey.AddOrUpdate(am2.Key, am2, (Func<TrainerModelKey, AvionicsModel, AvionicsModel>)((TrainerModelKey k, AvionicsModel previousModel) => (previousModel.Package != am2.Package) ? am2 : previousModel));
	}
}
