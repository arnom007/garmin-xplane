using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Extensions;
using LOLA.Reactive;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LauncherCoreLib.Utility;
using Nito.AsyncEx;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class PackageRepoEvolved : ITrainerPackageRepoEvolved
{
	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public PackageRepoEvolved _003C_003E4__this;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			PackageRepoEvolved packageRepoEvolved = _003C_003E4__this;
			try
			{
				AsyncSubject<ITrainerPackage> asyncSubject;
				if (num != 0)
				{
					asyncSubject = packageRepoEvolved._startingPackages.CatchAndComplete<ITrainerPackage>().LastOrDefaultAsync<ITrainerPackage>().GetAwaiter<ITrainerPackage>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = asyncSubject;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<ITrainerPackage>, _003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed>(ref asyncSubject, ref this);
						return;
					}
				}
				else
				{
					asyncSubject = (AsyncSubject<ITrainerPackage>)_003C_003Eu__1;
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
	[CompilerGenerated]
	private struct _003C_003CGetAllPackages_003Eb__12_4_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<IObservable<ITrainerPackage>> _003C_003Et__builder;

		public PackageRepoEvolved _003C_003E4__this;

		public CancellationToken token;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageRepoEvolved packageRepoEvolved = _003C_003E4__this;
			IObservable<ITrainerPackage> result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					awaiter = packageRepoEvolved._packageLock.ReaderLockAsync(token).ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CGetAllPackages_003Eb__12_4_003Ed>(ref awaiter, ref this);
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
				List<ITrainerPackage> source;
				try
				{
					source = Enumerable.ToList<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)packageRepoEvolved._locationLookup.Values);
				}
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
					}
				}
				result2 = ((System.Collections.Generic.IEnumerable<ITrainerPackage>)source).ToObservable();
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
	private struct _003CGetPackageByLocationOrNull_003Ed__14 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ITrainerPackage> _003C_003Et__builder;

		public PackageRepoEvolved _003C_003E4__this;

		public string packageRootDirectory;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageRepoEvolved packageRepoEvolved = _003C_003E4__this;
			ITrainerPackage result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					awaiter = packageRepoEvolved._packageLock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetPackageByLocationOrNull_003Ed__14>(ref awaiter, ref this);
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
					ITrainerPackage trainerPackage = default(ITrainerPackage);
					result2 = (packageRepoEvolved._locationLookup.TryGetValue(packageRootDirectory, ref trainerPackage) ? trainerPackage : null);
				}
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
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
	private struct _003CGetPackageByReleaseId_003Ed__13 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<PackageInstallationInfo> _003C_003Et__builder;

		public PackageRepoEvolved _003C_003E4__this;

		public TrainerEmbeddedReleaseId embeddedReleaseId;

		private List<ITrainerPackage> _003ClookupList_003E5__2;

		private ITrainerPackage _003CmostRecentInstalled_003E5__3;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ITrainerInstallerListingInfo> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageRepoEvolved packageRepoEvolved = _003C_003E4__this;
			PackageInstallationInfo result3;
			try
			{
				ConfiguredTaskAwaiter<ITrainerInstallerListingInfo> awaiter;
				ConfiguredTaskAwaiter<System.IDisposable> awaiter2;
				if (num != 0)
				{
					if (num == 1)
					{
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerInstallerListingInfo>);
						num = (_003C_003E1__state = -1);
						goto IL_01ba;
					}
					awaiter2 = packageRepoEvolved._packageLock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CGetPackageByReleaseId_003Ed__13>(ref awaiter2, ref this);
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
					IDictionary<Version, List<ITrainerPackage>> val = default(IDictionary<Version, List<ITrainerPackage>>);
					if (packageRepoEvolved._releaseLookup.TryGetValue(embeddedReleaseId, ref val))
					{
						_003ClookupList_003E5__2 = Enumerable.ToList<ITrainerPackage>(Enumerable.SelectMany<KeyValuePair<Version, List<ITrainerPackage>>, ITrainerPackage>((System.Collections.Generic.IEnumerable<KeyValuePair<Version, List<ITrainerPackage>>>)val, (Func<KeyValuePair<Version, List<ITrainerPackage>>, System.Collections.Generic.IEnumerable<ITrainerPackage>>)((KeyValuePair<Version, List<ITrainerPackage>> kv) => (System.Collections.Generic.IEnumerable<ITrainerPackage>)kv.Value)));
					}
					else
					{
						_003ClookupList_003E5__2 = new List<ITrainerPackage>();
					}
				}
				finally
				{
					if (num < 0)
					{
						result?.Dispose();
					}
				}
				_003CmostRecentInstalled_003E5__3 = null;
				int num2 = _003ClookupList_003E5__2.Count - 1;
				while (num2 >= 0 && _003CmostRecentInstalled_003E5__3 == null)
				{
					ITrainerPackage trainerPackage = _003ClookupList_003E5__2[num2];
					if (trainerPackage.BaseInfo.PackageType.IsCompleteOnDisk())
					{
						_003CmostRecentInstalled_003E5__3 = trainerPackage;
					}
					num2--;
				}
				awaiter = packageRepoEvolved._installerListingInfoProvider.GetInstallerListingForRelease(embeddedReleaseId, (System.Collections.Generic.IEnumerable<ITrainerPackage>)_003ClookupList_003E5__2.AsReadOnly()).DetachContext().GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (_003C_003E1__state = 1);
					_003C_003Eu__2 = awaiter;
					_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerInstallerListingInfo>, _003CGetPackageByReleaseId_003Ed__13>(ref awaiter, ref this);
					return;
				}
				goto IL_01ba;
				IL_01ba:
				ITrainerInstallerListingInfo result2 = awaiter.GetResult();
				List<ITrainerPackage> val2 = Enumerable.ToList<ITrainerPackage>(Enumerable.Where<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)_003ClookupList_003E5__2, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.IsACatalogPackage())));
				if (val2.Count == 0)
				{
					if (_003CmostRecentInstalled_003E5__3 != null)
					{
						MissingCatalogReason missingCatalogReason = DetermineMissingReason(_003CmostRecentInstalled_003E5__3);
						result3 = new PackageInstallationInfo(_003CmostRecentInstalled_003E5__3, missingCatalogReason, result2);
					}
					else
					{
						result3 = new PackageInstallationInfo(embeddedReleaseId, (!result2.IsDiscontinued) ? MissingCatalogReason.NotInTheCatalog : MissingCatalogReason.Discontinued, result2);
					}
				}
				else
				{
					if (_003CmostRecentInstalled_003E5__3 != null)
					{
						val2.Add(_003CmostRecentInstalled_003E5__3);
					}
					result3 = new PackageInstallationInfo((System.Collections.Generic.IReadOnlyCollection<ITrainerPackage>)val2, MissingCatalogReason.NotMissing, result2);
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003ClookupList_003E5__2 = null;
				_003CmostRecentInstalled_003E5__3 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003ClookupList_003E5__2 = null;
			_003CmostRecentInstalled_003E5__3 = null;
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
	private struct _003CRegisterPackage_003Ed__15 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ITrainerPackage> _003C_003Et__builder;

		public ITrainerPackage package;

		public PackageRepoEvolved _003C_003E4__this;

		private ITrainerPackage _003Cresult_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageRepoEvolved packageRepoEvolved = _003C_003E4__this;
			ITrainerPackage result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					ThrowIfPackageIsInvalid(package);
					_003Cresult_003E5__2 = package;
					awaiter = packageRepoEvolved._packageLock.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CRegisterPackage_003Ed__15>(ref awaiter, ref this);
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
				DataChangeType? changeType;
				try
				{
					TrainerEmbeddedReleaseId embeddedReleaseCompositeId = package.TrainerPackageId.EmbeddedReleaseCompositeId;
					Version binaryVersion = package.TrainerPackageId.BinaryVersion;
					List<ITrainerPackage> val2 = default(List<ITrainerPackage>);
					IDictionary<Version, List<ITrainerPackage>> val = default(IDictionary<Version, List<ITrainerPackage>>);
					if (packageRepoEvolved._releaseLookup.TryGetValue(embeddedReleaseCompositeId, ref val))
					{
						if (!val.TryGetValue(binaryVersion, ref val2))
						{
							val2 = (val[binaryVersion] = new List<ITrainerPackage>());
						}
					}
					else
					{
						val = (IDictionary<Version, List<ITrainerPackage>>)(object)new SortedDictionary<Version, List<ITrainerPackage>>((IComparer<Version>)(object)Comparer<Version>.Default);
						val2 = (val[binaryVersion] = new List<ITrainerPackage>());
						packageRepoEvolved._releaseLookup[embeddedReleaseCompositeId] = val;
					}
					int indexOfMatchingPackage = GetIndexOfMatchingPackage(val2, package);
					if (indexOfMatchingPackage < 0)
					{
						val2.Add(package);
						packageRepoEvolved._locationLookup[package.BaseInfo.PackageRootPath] = package;
						changeType = DataChangeType.Created;
					}
					else
					{
						ITrainerPackage trainerPackage = val2[indexOfMatchingPackage];
						if (!((EqualityComparer<ITrainerPackage>)(object)PackageDeepEquality.Instance).Equals(trainerPackage, package))
						{
							val2[indexOfMatchingPackage] = package;
							packageRepoEvolved._locationLookup[package.BaseInfo.PackageRootPath] = package;
							changeType = DataChangeType.Updated;
						}
						else
						{
							changeType = null;
							_003Cresult_003E5__2 = trainerPackage;
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
				packageRepoEvolved.RaiseChangeIfHasValue(_003Cresult_003E5__2, changeType);
				result2 = _003Cresult_003E5__2;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003Cresult_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003Cresult_003E5__2 = null;
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
	private struct _003CUnRegisterPackage_003Ed__16 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

		public ITrainerPackage package;

		public PackageRepoEvolved _003C_003E4__this;

		private DataChangeType? _003CchangeType_003E5__2;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PackageRepoEvolved packageRepoEvolved = _003C_003E4__this;
			bool result2;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					ThrowIfPackageIsInvalid(package);
					_003CchangeType_003E5__2 = null;
					awaiter = packageRepoEvolved._packageLock.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CUnRegisterPackage_003Ed__16>(ref awaiter, ref this);
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
					TrainerEmbeddedReleaseId embeddedReleaseCompositeId = package.TrainerPackageId.EmbeddedReleaseCompositeId;
					Version binaryVersion = package.TrainerPackageId.BinaryVersion;
					IDictionary<Version, List<ITrainerPackage>> val = default(IDictionary<Version, List<ITrainerPackage>>);
					List<ITrainerPackage> val2 = default(List<ITrainerPackage>);
					if (packageRepoEvolved._releaseLookup.TryGetValue(embeddedReleaseCompositeId, ref val) && val.TryGetValue(binaryVersion, ref val2))
					{
						int indexOfMatchingPackage = GetIndexOfMatchingPackage(val2, package);
						if (indexOfMatchingPackage >= 0)
						{
							val2.RemoveAt(indexOfMatchingPackage);
							packageRepoEvolved._locationLookup.Remove(package.BaseInfo.PackageRootPath);
							if (val2.Count == 0)
							{
								val.Remove(binaryVersion);
							}
							if (((System.Collections.Generic.ICollection<KeyValuePair<Version, List<ITrainerPackage>>>)val).Count == 0)
							{
								packageRepoEvolved._releaseLookup.Remove(embeddedReleaseCompositeId);
							}
							_003CchangeType_003E5__2 = DataChangeType.Deleted;
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
				packageRepoEvolved.RaiseChangeIfHasValue(package, _003CchangeType_003E5__2);
				result2 = _003CchangeType_003E5__2.HasValue && _003CchangeType_003E5__2.Value == DataChangeType.Deleted;
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

	private readonly IObservable<ITrainerPackage> _startingPackages;

	private readonly IInstallerListingInfoProvider _installerListingInfoProvider;

	private readonly Lazy<System.Threading.Tasks.Task> _startingPackagesComplete;

	private readonly AsyncReaderWriterLock _packageLock = new AsyncReaderWriterLock();

	private readonly Dictionary<TrainerEmbeddedReleaseId, IDictionary<Version, List<ITrainerPackage>>> _releaseLookup = new Dictionary<TrainerEmbeddedReleaseId, IDictionary<Version, List<ITrainerPackage>>>();

	private readonly Dictionary<string, ITrainerPackage> _locationLookup = new Dictionary<string, ITrainerPackage>((IEqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash);

	private readonly ISubject<DataChangeNotification<ITrainerPackage>> _changes = new Subject<DataChangeNotification<ITrainerPackage>>();

	public IObservable<DataChangeNotification<ITrainerPackage>> WhenPackageIsChanged => ((IObservable<DataChangeNotification<ITrainerPackage>>)_changes).AsObservable<DataChangeNotification<ITrainerPackage>>();

	public System.Threading.Tasks.Task LoadingComplete => _startingPackagesComplete.Value;

	public PackageRepoEvolved(IStartingPackageProvider startingPackages, IInstallerListingInfoProvider installerListingInfoProvider)
	{
		List<ITrainerPackage> installed = new List<ITrainerPackage>();
		_installerListingInfoProvider = installerListingInfoProvider;
		_startingPackages = startingPackages.StartingPackages.SelectMany<ITrainerPackage, ITrainerPackage>((Func<ITrainerPackage, System.Threading.Tasks.Task<ITrainerPackage>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass11_0._003C_003C_002Dctor_003Eb__0_003Ed))] async (ITrainerPackage p) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			lock (installed)
			{
				flag = ((System.Collections.Generic.IList<ITrainerPackage>)installed).BinarySearchIndexOfBy<ITrainerPackage, string, string>(p.BaseInfo.PackageRootPath, (ITrainerPackage pack) => pack.BaseInfo.PackageRootPath, (ListSortDirection)0, (string packRootpath) => packRootpath) >= 0;
			}
			if (!flag)
			{
				ITrainerPackage element = await RegisterPackage(p).DetachContext();
				lock (installed)
				{
					((System.Collections.Generic.IList<ITrainerPackage>)installed).InsertInUniqueSortedPosition<ITrainerPackage, string>(element, (ListSortDirection)0, (ITrainerPackage pack) => pack.BaseInfo.PackageRootPath);
				}
				return p;
			}
			return (ITrainerPackage)null;
		})).Where<ITrainerPackage>((Func<ITrainerPackage, bool>)((ITrainerPackage p) => p != null)).Publish<ITrainerPackage>()
			.RefCount();
		_startingPackagesComplete = new Lazy<System.Threading.Tasks.Task>((Func<System.Threading.Tasks.Task>)WaitForStartingPackagesToBeRegistered);
		[AsyncStateMachine(typeof(_003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed))]
		[CompilerGenerated]
		System.Threading.Tasks.Task WaitForStartingPackagesToBeRegistered()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed _003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed = default(_003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed);
			_003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed._003C_003E4__this = this;
			_003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed._003C_003Et__builder)).Start<_003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed>(ref _003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003C_002Dctor_003Eg__WaitForStartingPackagesToBeRegistered_007C11_2_003Ed._003C_003Et__builder)).Task;
		}
	}

	public IObservable<ITrainerPackage> GetAllPackages()
	{
		return Observable.Defer<ITrainerPackage>((Func<IObservable<ITrainerPackage>>)([CompilerGenerated] () => (IObservable<ITrainerPackage>)((_startingPackagesComplete.IsValueCreated && _startingPackagesComplete.Value.IsCompleted) ? (_startingPackagesComplete.Value.IsFaulted ? ((object)Observable.FromAsync((Func<System.Threading.Tasks.Task>)([CompilerGenerated] () => _startingPackagesComplete.Value)).IgnoreElements<Unit>().SelectDefault<Unit, ITrainerPackage>(null)
			.WhereClassNotNull<ITrainerPackage>()) : ((object)GetAllRepoPackages())) : _startingPackages.Merge<ITrainerPackage>(Observable.FromAsync((Func<System.Threading.Tasks.Task>)([CompilerGenerated] () => _startingPackagesComplete.Value)).IgnoreElements<Unit>().SelectDefault<Unit, ITrainerPackage>(null)).Concat<ITrainerPackage>(GetAllRepoPackages()).Distinct<ITrainerPackage>())));
		[CompilerGenerated]
		IObservable<ITrainerPackage> GetAllRepoPackages()
		{
			return Observable.DeferAsync<ITrainerPackage>([AsyncStateMachine(typeof(_003C_003CGetAllPackages_003Eb__12_4_003Ed))] [CompilerGenerated] async (CancellationToken token) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				List<ITrainerPackage> source;
				using (await _packageLock.ReaderLockAsync(token).ConfigureAwait(continueOnCapturedContext: false))
				{
					source = Enumerable.ToList<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)_locationLookup.Values);
				}
				return ((System.Collections.Generic.IEnumerable<ITrainerPackage>)source).ToObservable();
			});
		}
	}

	[AsyncStateMachine(typeof(_003CGetPackageByReleaseId_003Ed__13))]
	public async System.Threading.Tasks.Task<PackageInstallationInfo> GetPackageByReleaseId(TrainerEmbeddedReleaseId embeddedReleaseId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<ITrainerPackage> lookupList;
		using (await _packageLock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			IDictionary<Version, List<ITrainerPackage>> val = default(IDictionary<Version, List<ITrainerPackage>>);
			lookupList = ((!_releaseLookup.TryGetValue(embeddedReleaseId, ref val)) ? new List<ITrainerPackage>() : Enumerable.ToList<ITrainerPackage>(Enumerable.SelectMany<KeyValuePair<Version, List<ITrainerPackage>>, ITrainerPackage>((System.Collections.Generic.IEnumerable<KeyValuePair<Version, List<ITrainerPackage>>>)val, (Func<KeyValuePair<Version, List<ITrainerPackage>>, System.Collections.Generic.IEnumerable<ITrainerPackage>>)((KeyValuePair<Version, List<ITrainerPackage>> kv) => (System.Collections.Generic.IEnumerable<ITrainerPackage>)kv.Value))));
		}
		ITrainerPackage mostRecentInstalled = null;
		int num = lookupList.Count - 1;
		while (num >= 0 && mostRecentInstalled == null)
		{
			ITrainerPackage trainerPackage = lookupList[num];
			if (trainerPackage.BaseInfo.PackageType.IsCompleteOnDisk())
			{
				mostRecentInstalled = trainerPackage;
			}
			num--;
		}
		ITrainerInstallerListingInfo trainerInstallerListingInfo = await _installerListingInfoProvider.GetInstallerListingForRelease(embeddedReleaseId, (System.Collections.Generic.IEnumerable<ITrainerPackage>)lookupList.AsReadOnly()).DetachContext();
		List<ITrainerPackage> val2 = Enumerable.ToList<ITrainerPackage>(Enumerable.Where<ITrainerPackage>((System.Collections.Generic.IEnumerable<ITrainerPackage>)lookupList, (Func<ITrainerPackage, bool>)((ITrainerPackage p) => p.IsACatalogPackage())));
		if (val2.Count == 0)
		{
			if (mostRecentInstalled != null)
			{
				MissingCatalogReason missingCatalogReason = DetermineMissingReason(mostRecentInstalled);
				return new PackageInstallationInfo(mostRecentInstalled, missingCatalogReason, trainerInstallerListingInfo);
			}
			return new PackageInstallationInfo(embeddedReleaseId, (!trainerInstallerListingInfo.IsDiscontinued) ? MissingCatalogReason.NotInTheCatalog : MissingCatalogReason.Discontinued, trainerInstallerListingInfo);
		}
		if (mostRecentInstalled != null)
		{
			val2.Add(mostRecentInstalled);
		}
		return new PackageInstallationInfo((System.Collections.Generic.IReadOnlyCollection<ITrainerPackage>)val2, MissingCatalogReason.NotMissing, trainerInstallerListingInfo);
	}

	[AsyncStateMachine(typeof(_003CGetPackageByLocationOrNull_003Ed__14))]
	public async System.Threading.Tasks.Task<ITrainerPackage?> GetPackageByLocationOrNull(string packageRootDirectory)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		using (await _packageLock.ReaderLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			ITrainerPackage trainerPackage = default(ITrainerPackage);
			return _locationLookup.TryGetValue(packageRootDirectory, ref trainerPackage) ? trainerPackage : null;
		}
	}

	[AsyncStateMachine(typeof(_003CRegisterPackage_003Ed__15))]
	public async System.Threading.Tasks.Task<ITrainerPackage> RegisterPackage(ITrainerPackage package)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfPackageIsInvalid(package);
		ITrainerPackage result = package;
		DataChangeType? changeType;
		using (await _packageLock.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			TrainerEmbeddedReleaseId embeddedReleaseCompositeId = package.TrainerPackageId.EmbeddedReleaseCompositeId;
			Version binaryVersion = package.TrainerPackageId.BinaryVersion;
			List<ITrainerPackage> val2 = default(List<ITrainerPackage>);
			IDictionary<Version, List<ITrainerPackage>> val = default(IDictionary<Version, List<ITrainerPackage>>);
			if (_releaseLookup.TryGetValue(embeddedReleaseCompositeId, ref val))
			{
				if (!val.TryGetValue(binaryVersion, ref val2))
				{
					val2 = (val[binaryVersion] = new List<ITrainerPackage>());
				}
			}
			else
			{
				val = (IDictionary<Version, List<ITrainerPackage>>)(object)new SortedDictionary<Version, List<ITrainerPackage>>((IComparer<Version>)(object)Comparer<Version>.Default);
				val2 = (val[binaryVersion] = new List<ITrainerPackage>());
				_releaseLookup[embeddedReleaseCompositeId] = val;
			}
			int indexOfMatchingPackage = GetIndexOfMatchingPackage(val2, package);
			if (indexOfMatchingPackage < 0)
			{
				val2.Add(package);
				_locationLookup[package.BaseInfo.PackageRootPath] = package;
				changeType = DataChangeType.Created;
			}
			else
			{
				ITrainerPackage trainerPackage = val2[indexOfMatchingPackage];
				if (!((EqualityComparer<ITrainerPackage>)(object)PackageDeepEquality.Instance).Equals(trainerPackage, package))
				{
					val2[indexOfMatchingPackage] = package;
					_locationLookup[package.BaseInfo.PackageRootPath] = package;
					changeType = DataChangeType.Updated;
				}
				else
				{
					changeType = null;
					result = trainerPackage;
				}
			}
		}
		RaiseChangeIfHasValue(result, changeType);
		return result;
	}

	[AsyncStateMachine(typeof(_003CUnRegisterPackage_003Ed__16))]
	public async System.Threading.Tasks.Task<bool> UnRegisterPackage(ITrainerPackage package)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfPackageIsInvalid(package);
		DataChangeType? changeType = null;
		using (await _packageLock.WriterLockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			TrainerEmbeddedReleaseId embeddedReleaseCompositeId = package.TrainerPackageId.EmbeddedReleaseCompositeId;
			Version binaryVersion = package.TrainerPackageId.BinaryVersion;
			IDictionary<Version, List<ITrainerPackage>> val = default(IDictionary<Version, List<ITrainerPackage>>);
			List<ITrainerPackage> val2 = default(List<ITrainerPackage>);
			if (_releaseLookup.TryGetValue(embeddedReleaseCompositeId, ref val) && val.TryGetValue(binaryVersion, ref val2))
			{
				int indexOfMatchingPackage = GetIndexOfMatchingPackage(val2, package);
				if (indexOfMatchingPackage >= 0)
				{
					val2.RemoveAt(indexOfMatchingPackage);
					_locationLookup.Remove(package.BaseInfo.PackageRootPath);
					if (val2.Count == 0)
					{
						val.Remove(binaryVersion);
					}
					if (((System.Collections.Generic.ICollection<KeyValuePair<Version, List<ITrainerPackage>>>)val).Count == 0)
					{
						_releaseLookup.Remove(embeddedReleaseCompositeId);
					}
					changeType = DataChangeType.Deleted;
				}
			}
		}
		RaiseChangeIfHasValue(package, changeType);
		return changeType.HasValue && changeType.Value == DataChangeType.Deleted;
	}

	private static int GetIndexOfMatchingPackage(List<ITrainerPackage> lookupList, ITrainerPackage package)
	{
		ITrainerPackage package2 = package;
		return lookupList.FindIndex((Predicate<ITrainerPackage>)((ITrainerPackage p) => p == package2 || p.BaseInfo.PackageType == package2.BaseInfo.PackageType));
	}

	private static void ThrowIfPackageIsInvalid(ITrainerPackage package)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (package == null)
		{
			throw new ArgumentNullException("package");
		}
		if ((object)package.TrainerPackageId == null)
		{
			throw new ArgumentException("Package is missing its identifier.");
		}
		if (package.BaseInfo.PackageRootPath == null)
		{
			throw new ArgumentException("Package is missing a root directory path.");
		}
	}

	private static MissingCatalogReason DetermineMissingReason(ITrainerPackage mostRecentInstalled)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (mostRecentInstalled == null)
		{
			throw new ArgumentException("If the most recent installed is null then we can't produce a valid installation info object anyways.");
		}
		if (mostRecentInstalled.BaseInfo.PackageType == TrainerPackageType.LegacyPermanentInstall)
		{
			return MissingCatalogReason.LegacyInstall;
		}
		if (mostRecentInstalled.BaseInfo.PackageType == TrainerPackageType.TemporaryPackage)
		{
			return MissingCatalogReason.Development;
		}
		if (mostRecentInstalled.BaseInfo.PackageType == TrainerPackageType.InstalledByGti)
		{
			return MissingCatalogReason.Discontinued;
		}
		return MissingCatalogReason.NotInTheCatalog;
	}

	private void RaiseChangeIfHasValue(ITrainerPackage package, DataChangeType? changeType)
	{
		if (changeType.HasValue)
		{
			lock (_changes)
			{
				((IObserver<DataChangeNotification<ITrainerPackage>>)_changes).OnNext(DataChangeNotification.Create(package, changeType.Value));
			}
		}
	}
}
