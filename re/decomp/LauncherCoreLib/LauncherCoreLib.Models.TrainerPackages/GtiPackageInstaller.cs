using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AppCenter.EventLogging;
using GTI.Net;
using LOLA.Extensions;
using LOLA.Logging;
using LOLA.Reactive;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LanguageExt;
using LauncherCoreLib.Models.AppLifeCycle;
using LauncherCoreLib.Models.DAL;
using LauncherCoreLib.Models.EventLogging;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Flight;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages.PackageUpgrade;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public class GtiPackageInstaller : ITrainerPackageInstaller
{
	private sealed class GtiIndividualPackageManagement
	{
		private readonly List<GtiPackageOperation> _operations = new List<GtiPackageOperation>();

		private readonly object _operationsMutex = new object();

		[field: CompilerGenerated]
		public TrainerIdentifier EmbeddedReleaseId
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		public bool IsEmpty
		{
			get
			{
				lock (_operationsMutex)
				{
					return _operations.Count == 0;
				}
			}
		}

		public GtiPackageOperation CurrentOperation
		{
			get
			{
				lock (_operationsMutex)
				{
					return MostRecentLockedOperation;
				}
			}
		}

		private GtiPackageOperation MostRecentLockedOperation => _operations[_operations.Count - 1];

		public GtiIndividualPackageManagement(TrainerIdentifier embeddedReleaseId, Func<GtiPackageOperation> startingPackageFactory)
		{
			EmbeddedReleaseId = embeddedReleaseId;
			_operations.Add(startingPackageFactory.Invoke());
		}

		public System.Collections.Generic.IList<GtiPackageOperation> GetAllOperations()
		{
			lock (_operationsMutex)
			{
				return (System.Collections.Generic.IList<GtiPackageOperation>)Enumerable.ToList<GtiPackageOperation>((System.Collections.Generic.IEnumerable<GtiPackageOperation>)_operations);
			}
		}

		public GtiIndividualPackageManagement GetCreateOrThrow(TrainerPackageOperation operation, Func<GtiPackageOperation, bool> canAddAfterCompleted, Func<GtiPackageOperation> factory)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			lock (_operationsMutex)
			{
				GtiPackageOperation mostRecentLockedOperation = MostRecentLockedOperation;
				if (mostRecentLockedOperation == null)
				{
					AddOperation(factory.Invoke());
				}
				else if (!mostRecentLockedOperation.HasCompleted())
				{
					if (mostRecentLockedOperation.RequestedOperation != operation)
					{
						throw new InvalidOperationException("Must wait for the in progress operation to complete.");
					}
				}
				else if (mostRecentLockedOperation.RequestedOperation != operation || canAddAfterCompleted.Invoke(mostRecentLockedOperation))
				{
					AddOperation(factory.Invoke());
				}
				return this;
			}
		}

		public void DismissErrors()
		{
			lock (_operationsMutex)
			{
				_operations.RemoveAll((Predicate<GtiPackageOperation>)((GtiPackageOperation gpo) => gpo.IsFailure()));
			}
		}

		public void Inherit(GtiIndividualPackageManagement operation)
		{
			lock (_operationsMutex)
			{
				System.Collections.Generic.IEnumerator<GtiPackageOperation> enumerator = ((System.Collections.Generic.IEnumerable<GtiPackageOperation>)operation.GetAllOperations()).GetEnumerator();
				try
				{
					while (((System.Collections.IEnumerator)enumerator).MoveNext())
					{
						GtiPackageOperation current = enumerator.Current;
						_operations.Insert(0, current);
					}
				}
				finally
				{
					((System.IDisposable)enumerator)?.Dispose();
				}
			}
		}

		private GtiIndividualPackageManagement AddOperation(GtiPackageOperation operation)
		{
			_operations.Add(operation);
			return this;
		}
	}

	private sealed class GtiPackageOperation : ITrainerPackageInstallationSession
	{
		private readonly IConnectableObservable<TrainerInstallationStatus> _publishedStream;

		private readonly ICancellation _cancellation;

		private readonly SingleAssignmentDisposable _sharedSubscription;

		[field: CompilerGenerated]
		public Guid SessionId
		{
			[CompilerGenerated]
			get;
		} = Guid.NewGuid();


		[field: CompilerGenerated]
		public ITrainerPackage Package
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public IObservable<TrainerInstallationStatus> StatusStream
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public System.Exception? Error
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		public bool Failed
		{
			get
			{
				if (this.CompletedAt().HasValue)
				{
					return Error != null;
				}
				return false;
			}
		}

		public ITrainerPackage RequestingPackage => Package;

		[field: CompilerGenerated]
		public System.Threading.Tasks.Task<DateTimeOffset> SessionCompletion
		{
			[CompilerGenerated]
			get;
		}

		public IObservable<TrainerInstallationStatus> StatusUpdates => StatusStream;

		[field: CompilerGenerated]
		public TrainerPackageOperation RequestedOperation
		{
			[CompilerGenerated]
			get;
		}

		public GtiPackageOperation(ITrainerPackage package, TrainerPackageOperation operation, Func<CancellationToken, IObservable<TrainerInstallationStatus>> statusStreamFactory)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			Package = package;
			RequestedOperation = operation;
			_cancellation = CancellationFactory.Create();
			_sharedSubscription = new SingleAssignmentDisposable();
			IObservable<TrainerInstallationStatus> source = statusStreamFactory.Invoke(_cancellation.Token);
			StatusStream = (IObservable<TrainerInstallationStatus>)(_publishedStream = source.Replay<TrainerInstallationStatus>(1));
			SessionCompletion = TaskHelpers.Finally<DateTimeOffset>(StatusStream.Do<TrainerInstallationStatus>((Action<TrainerInstallationStatus>)([CompilerGenerated] (TrainerInstallationStatus su) =>
			{
				if (su.OnDemandState == TrainerPackageOperationState.Error)
				{
					Error = su.Error;
				}
			})).CatchAndComplete<TrainerInstallationStatus>([CompilerGenerated] (System.Exception e) =>
			{
				Error = e;
			}).IgnoreElements<TrainerInstallationStatus>()
				.SelectDefault<TrainerInstallationStatus, DateTimeOffset>(default(DateTimeOffset))
				.Concat<DateTimeOffset>(Observable.Return<DateTimeOffset>(DateTimeOffset.Now))
				.ToTask<DateTimeOffset>(), (Action)([CompilerGenerated] () =>
			{
				_sharedSubscription.Dispose();
				((System.IDisposable)_cancellation).Dispose();
			}));
		}

		public bool Start()
		{
			lock (_sharedSubscription)
			{
				if (_sharedSubscription.Disposable == null)
				{
					_sharedSubscription.Disposable = _publishedStream.Connect();
					return true;
				}
			}
			return false;
		}

		public void AttemptCancel()
		{
			_cancellation.Cancel();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass28_0
	{
		[StructLayout(3)]
		private struct _003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass28_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass28_0 _003C_003Ec__DisplayClass28_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter awaiter;
					if (num != 0)
					{
						ConfiguredTaskAwaitable val = CancelGtiProcess(_003C_003Ec__DisplayClass28_.gtiSession).ConfigureAwait(false);
						awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed>(ref awaiter, ref this);
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
					((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public IGtiProcessExecution gtiSession;

		public ITrainerPackage package;

		[AsyncStateMachine(typeof(_003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed))]
		internal void _003CHandleGtiOutputAndCancelations_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed _003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed = default(_003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed);
			_003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
			_003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncVoidMethodBuilder)(ref _003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed>(ref _003C_003CHandleGtiOutputAndCancelations_003Eb__0_003Ed);
		}

		internal TrainerInstallationStatus _003CHandleGtiOutputAndCancelations_003Eb__1(GtiProgress pu)
		{
			return TrainerInstallationStatus.CreateProgress(package, pu);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass34_0
	{
		public ITrainerPackage package;

		internal bool _003CPowerDownAircraftIfPackageIsInUse_003Eb__0(SimInstallData s)
		{
			return s.FullPackageId.IsSameEmbeddedRelease(package.TrainerPackageId);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		[StructLayout(3)]
		private struct _003C_003CInstallGtiName_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<TrainerInstallationStatus> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass35_0 _003C_003E4__this;

			private PotentialPackageDirectory _003CpotentialDir_003E5__2;

			private ITrainerPackage _003CmatchingInstalledPackage_003E5__3;

			private ConfiguredTaskAwaiter<GtiListingEntry> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__2;

			private ITrainerPackage _003CnewPackage_003E5__4;

			private bool _003CisUpdate_003E5__5;

			private ConfiguredTaskAwaiter<System.Collections.Generic.IList<UpgradeDecisionLog>> _003C_003Eu__3;

			private ConfiguredTaskAwaiter _003C_003Eu__4;

			private ConfiguredTaskAwaiter<bool> _003C_003Eu__5;

			private void MoveNext()
			{
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_014b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0150: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c1: Expected O, but got Unknown
				//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02db: Unknown result type (might be due to invalid IL or missing references)
				//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_042c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0431: Unknown result type (might be due to invalid IL or missing references)
				//IL_0439: Unknown result type (might be due to invalid IL or missing references)
				//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Expected O, but got Unknown
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01db: Unknown result type (might be due to invalid IL or missing references)
				//IL_0130: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0477: Unknown result type (might be due to invalid IL or missing references)
				//IL_047c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0480: Unknown result type (might be due to invalid IL or missing references)
				//IL_0485: Unknown result type (might be due to invalid IL or missing references)
				//IL_0411: Unknown result type (might be due to invalid IL or missing references)
				//IL_0413: Unknown result type (might be due to invalid IL or missing references)
				//IL_0377: Unknown result type (might be due to invalid IL or missing references)
				//IL_037c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0380: Unknown result type (might be due to invalid IL or missing references)
				//IL_0385: Unknown result type (might be due to invalid IL or missing references)
				//IL_049a: Unknown result type (might be due to invalid IL or missing references)
				//IL_049c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0290: Unknown result type (might be due to invalid IL or missing references)
				//IL_0295: Unknown result type (might be due to invalid IL or missing references)
				//IL_0299: Unknown result type (might be due to invalid IL or missing references)
				//IL_029e: Unknown result type (might be due to invalid IL or missing references)
				//IL_039a: Unknown result type (might be due to invalid IL or missing references)
				//IL_039c: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass35_0 _003C_003Ec__DisplayClass35_ = _003C_003E4__this;
				TrainerInstallationStatus result3;
				try
				{
					ConfiguredTaskAwaiter<GtiListingEntry> awaiter2;
					ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
					ITrainerPackage result;
					GtiListingEntry result2;
					switch (num)
					{
					default:
						awaiter2 = _003C_003Ec__DisplayClass35_._003C_003E4__this.GetMatchingListEntry(_003C_003Ec__DisplayClass35_.gtiName, _003C_003Ec__DisplayClass35_.requestedPackage).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<GtiListingEntry>, _003C_003CInstallGtiName_003Eb__1_003Ed>(ref awaiter2, ref this);
							return;
						}
						goto IL_00a1;
					case 0:
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<GtiListingEntry>);
						num = (_003C_003E1__state = -1);
						goto IL_00a1;
					case 1:
						awaiter = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
						num = (_003C_003E1__state = -1);
						goto IL_0167;
					case 2:
					case 3:
					case 4:
					case 5:
					case 6:
						{
							try
							{
								ConfiguredTaskAwaiter<System.Collections.Generic.IList<UpgradeDecisionLog>> awaiter5;
								ConfiguredTaskAwaiter awaiter4;
								ConfiguredTaskAwaiter<bool> awaiter3;
								System.Collections.Generic.IList<UpgradeDecisionLog> result4;
								PackageUpgradeVersions packageUpgradeVersions;
								switch (num)
								{
								default:
									awaiter = _003C_003Ec__DisplayClass35_._003C_003E4__this._parser.ParsePackage(_003CpotentialDir_003E5__2).DetachContext().GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = (_003C_003E1__state = 2);
										_003C_003Eu__2 = awaiter;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CInstallGtiName_003Eb__1_003Ed>(ref awaiter, ref this);
										return;
									}
									goto IL_0210;
								case 2:
									awaiter = _003C_003Eu__2;
									_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
									num = (_003C_003E1__state = -1);
									goto IL_0210;
								case 3:
									awaiter5 = _003C_003Eu__3;
									_003C_003Eu__3 = default(ConfiguredTaskAwaiter<System.Collections.Generic.IList<UpgradeDecisionLog>>);
									num = (_003C_003E1__state = -1);
									goto IL_02ea;
								case 4:
									awaiter4 = _003C_003Eu__4;
									_003C_003Eu__4 = default(ConfiguredTaskAwaiter);
									num = (_003C_003E1__state = -1);
									goto IL_03d1;
								case 5:
									awaiter = _003C_003Eu__2;
									_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
									num = (_003C_003E1__state = -1);
									goto IL_0448;
								case 6:
									{
										awaiter3 = _003C_003Eu__5;
										_003C_003Eu__5 = default(ConfiguredTaskAwaiter<bool>);
										num = (_003C_003E1__state = -1);
										goto IL_04d1;
									}
									IL_0448:
									result = awaiter.GetResult();
									_003CmatchingInstalledPackage_003E5__3 = result;
									if (!_003CisUpdate_003E5__5)
									{
										break;
									}
									awaiter3 = _003C_003Ec__DisplayClass35_._003C_003E4__this._packageRepo.UnRegisterPackage(_003C_003Ec__DisplayClass35_.currentPackage).DetachContext().GetAwaiter();
									if (!awaiter3.IsCompleted)
									{
										num = (_003C_003E1__state = 6);
										_003C_003Eu__5 = awaiter3;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003C_003CInstallGtiName_003Eb__1_003Ed>(ref awaiter3, ref this);
										return;
									}
									goto IL_04d1;
									IL_0210:
									result = awaiter.GetResult();
									_003CnewPackage_003E5__4 = result;
									_003CisUpdate_003E5__5 = _003C_003Ec__DisplayClass35_.currentPackage != null && _003C_003Ec__DisplayClass35_.currentPackage.BaseInfo.PackageType == TrainerPackageType.InstalledByGti && _003CnewPackage_003E5__4.IsSuccessorOf(_003C_003Ec__DisplayClass35_.currentPackage);
									if (_003CisUpdate_003E5__5)
									{
										awaiter5 = _003C_003Ec__DisplayClass35_._003C_003E4__this._unitOfWork.PackageRepos.UpgradeDecisions.GetPriorUserUpgradeDecisions(_003C_003Ec__DisplayClass35_.requestedPackage.TrainerPackageId.EmbeddedReleaseCompositeId.Yield()).DetachContext().GetAwaiter();
										if (!awaiter5.IsCompleted)
										{
											num = (_003C_003E1__state = 3);
											_003C_003Eu__3 = awaiter5;
											_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Collections.Generic.IList<UpgradeDecisionLog>>, _003C_003CInstallGtiName_003Eb__1_003Ed>(ref awaiter5, ref this);
											return;
										}
										goto IL_02ea;
									}
									goto IL_03d8;
									IL_03d8:
									awaiter = _003C_003Ec__DisplayClass35_._003C_003E4__this._packageRepo.RegisterPackage(_003CnewPackage_003E5__4).DetachContext().GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = (_003C_003E1__state = 5);
										_003C_003Eu__2 = awaiter;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CInstallGtiName_003Eb__1_003Ed>(ref awaiter, ref this);
										return;
									}
									goto IL_0448;
									IL_04d1:
									awaiter3.GetResult();
									break;
									IL_02ea:
									result4 = awaiter5.GetResult();
									packageUpgradeVersions = new PackageUpgradeVersions(_003C_003Ec__DisplayClass35_.requestedPackage.TrainerPackageId.EmbeddedReleaseCompositeId, new VersionRange(_003C_003Ec__DisplayClass35_.currentPackage.TrainerPackageId.BinaryVersion, _003C_003Ec__DisplayClass35_.requestedPackage.TrainerPackageId.BinaryVersion));
									if (((System.Collections.Generic.ICollection<UpgradeDecisionLog>)result4).Count == 1 && result4[0].GetDecision(packageUpgradeVersions) != UpgradeUserDecision.ApprovedUpgrade)
									{
										ConfiguredTaskAwaitable val = _003C_003Ec__DisplayClass35_._003C_003E4__this._unitOfWork.PackageRepos.UpgradeDecisions.StoreUserUpgradeDecision(new PackageUpgradeVersions[1] { packageUpgradeVersions }, UpgradeUserDecision.ApprovedUpgrade).DetachContext();
										awaiter4 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
										if (!((ConfiguredTaskAwaiter)(ref awaiter4)).IsCompleted)
										{
											num = (_003C_003E1__state = 4);
											_003C_003Eu__4 = awaiter4;
											_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CInstallGtiName_003Eb__1_003Ed>(ref awaiter4, ref this);
											return;
										}
										goto IL_03d1;
									}
									goto IL_03d8;
									IL_03d1:
									((ConfiguredTaskAwaiter)(ref awaiter4)).GetResult();
									goto IL_03d8;
								}
								_003CnewPackage_003E5__4 = null;
							}
							catch (System.Exception error)
							{
								result3 = TrainerInstallationStatus.CreateError(_003C_003Ec__DisplayClass35_.requestedPackage, error);
								goto end_IL_000e;
							}
							break;
						}
						IL_0167:
						result = awaiter.GetResult();
						_003CmatchingInstalledPackage_003E5__3 = result;
						if (_003CmatchingInstalledPackage_003E5__3 != null)
						{
							break;
						}
						goto case 2;
						IL_00a1:
						result2 = awaiter2.GetResult();
						if (result2 == null)
						{
							result3 = TrainerInstallationStatus.CreateError(_003C_003Ec__DisplayClass35_.requestedPackage, (System.Exception)new KeyNotFoundException("GTI did not list the package after it installed it. This usually occurs when an updated version was discovered during the install/update operation. Please retry the install/update."));
						}
						else
						{
							string installedPath = result2.InstalledPath;
							if (installedPath != null)
							{
								_003CpotentialDir_003E5__2 = PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.InstalledByGti, installedPath);
								awaiter = _003C_003Ec__DisplayClass35_._003C_003E4__this.GetRegisteredPackageOrNullIfError(_003CpotentialDir_003E5__2).DetachContext().GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 1);
									_003C_003Eu__2 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CInstallGtiName_003Eb__1_003Ed>(ref awaiter, ref this);
									return;
								}
								goto IL_0167;
							}
							result3 = TrainerInstallationStatus.CreateError(_003C_003Ec__DisplayClass35_.requestedPackage, (System.Exception)new KeyNotFoundException("GTI did not report the package installed after it just installed it. This usually occurs when an updated version was discovered during the install/update operation. Please retry the install/update."));
						}
						goto end_IL_000e;
					}
					result3 = TrainerInstallationStatus.CreateReady(_003C_003Ec__DisplayClass35_.requestedPackage, _003CmatchingInstalledPackage_003E5__3);
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CpotentialDir_003E5__2 = null;
					_003CmatchingInstalledPackage_003E5__3 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CpotentialDir_003E5__2 = null;
				_003CmatchingInstalledPackage_003E5__3 = null;
				_003C_003Et__builder.SetResult(result3);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public string gtiName;

		public CancellationToken userCancel;

		public ITrainerPackage requestedPackage;

		public GtiPackageInstaller _003C_003E4__this;

		public ITrainerPackage currentPackage;

		internal IObservable<TrainerInstallationStatus> _003CInstallGtiName_003Eb__0(ILauncherGtiRunner gtiExe)
		{
			return Observable.Create<TrainerInstallationStatus>((Func<IObserver<TrainerInstallationStatus>, CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass35_1
			{
				CS_0024_003C_003E8__locals1 = this,
				gtiExe = gtiExe
			}._003CInstallGtiName_003Eb__3);
		}

		[AsyncStateMachine(typeof(_003C_003CInstallGtiName_003Eb__1_003Ed))]
		internal async System.Threading.Tasks.Task<TrainerInstallationStatus> _003CInstallGtiName_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			GtiListingEntry gtiListingEntry = await _003C_003E4__this.GetMatchingListEntry(gtiName, requestedPackage).DetachContext();
			if (gtiListingEntry == null)
			{
				return TrainerInstallationStatus.CreateError(requestedPackage, (System.Exception)new KeyNotFoundException("GTI did not list the package after it installed it. This usually occurs when an updated version was discovered during the install/update operation. Please retry the install/update."));
			}
			string installedPath = gtiListingEntry.InstalledPath;
			if (installedPath == null)
			{
				return TrainerInstallationStatus.CreateError(requestedPackage, (System.Exception)new KeyNotFoundException("GTI did not report the package installed after it just installed it. This usually occurs when an updated version was discovered during the install/update operation. Please retry the install/update."));
			}
			PotentialPackageDirectory potentialDir = PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.InstalledByGti, installedPath);
			ITrainerPackage matchingInstalledPackage = await _003C_003E4__this.GetRegisteredPackageOrNullIfError(potentialDir).DetachContext();
			if (matchingInstalledPackage == null)
			{
				try
				{
					ITrainerPackage newPackage = await _003C_003E4__this._parser.ParsePackage(potentialDir).DetachContext();
					bool isUpdate = currentPackage != null && currentPackage.BaseInfo.PackageType == TrainerPackageType.InstalledByGti && newPackage.IsSuccessorOf(currentPackage);
					if (isUpdate)
					{
						System.Collections.Generic.IList<UpgradeDecisionLog> list = await _003C_003E4__this._unitOfWork.PackageRepos.UpgradeDecisions.GetPriorUserUpgradeDecisions(requestedPackage.TrainerPackageId.EmbeddedReleaseCompositeId.Yield()).DetachContext();
						PackageUpgradeVersions packageUpgradeVersions = new PackageUpgradeVersions(requestedPackage.TrainerPackageId.EmbeddedReleaseCompositeId, new VersionRange(currentPackage.TrainerPackageId.BinaryVersion, requestedPackage.TrainerPackageId.BinaryVersion));
						if (((System.Collections.Generic.ICollection<UpgradeDecisionLog>)list).Count == 1 && list[0].GetDecision(packageUpgradeVersions) != UpgradeUserDecision.ApprovedUpgrade)
						{
							await _003C_003E4__this._unitOfWork.PackageRepos.UpgradeDecisions.StoreUserUpgradeDecision(new PackageUpgradeVersions[1] { packageUpgradeVersions }, UpgradeUserDecision.ApprovedUpgrade).DetachContext();
						}
					}
					matchingInstalledPackage = await _003C_003E4__this._packageRepo.RegisterPackage(newPackage).DetachContext();
					if (isUpdate)
					{
						await _003C_003E4__this._packageRepo.UnRegisterPackage(currentPackage).DetachContext();
					}
				}
				catch (System.Exception error)
				{
					return TrainerInstallationStatus.CreateError(requestedPackage, error);
				}
			}
			return TrainerInstallationStatus.CreateReady(requestedPackage, matchingInstalledPackage);
		}

		internal IObservable<TrainerInstallationStatus> _003CInstallGtiName_003Eb__2(System.Exception error)
		{
			return _003C_003E4__this.EndWithErrorOrCanceled(requestedPackage, error);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_1
	{
		[StructLayout(3)]
		private struct _003C_003CInstallGtiName_003Eb__3_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass35_1 _003C_003E4__this;

			public CancellationToken unsubscribed;

			public IObserver<TrainerInstallationStatus> o;

			private ICancellation _003Ccombined_003E5__2;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass35_1 _003C_003Ec__DisplayClass35_ = _003C_003E4__this;
				try
				{
					IGtiProcessExecution gtiSession = default(IGtiProcessExecution);
					if (num != 0)
					{
						gtiSession = _003C_003Ec__DisplayClass35_.gtiExe.Install(_003C_003Ec__DisplayClass35_.CS_0024_003C_003E8__locals1.gtiName);
						_003Ccombined_003E5__2 = CancellationFactory.CreateLinked(_003C_003Ec__DisplayClass35_.CS_0024_003C_003E8__locals1.userCancel, unsubscribed);
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = HandleGtiOutputAndCancelations(_003C_003Ec__DisplayClass35_.CS_0024_003C_003E8__locals1.requestedPackage, o, gtiSession, _003Ccombined_003E5__2.Token).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CInstallGtiName_003Eb__3_003Ed>(ref awaiter, ref this);
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
					finally
					{
						if (num < 0 && _003Ccombined_003E5__2 != null)
						{
							((System.IDisposable)_003Ccombined_003E5__2).Dispose();
						}
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003Ccombined_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003Ccombined_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public ILauncherGtiRunner gtiExe;

		public _003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CInstallGtiName_003Eb__3_003Ed))]
		internal System.Threading.Tasks.Task _003CInstallGtiName_003Eb__3(IObserver<TrainerInstallationStatus> o, CancellationToken unsubscribed)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CInstallGtiName_003Eb__3_003Ed _003C_003CInstallGtiName_003Eb__3_003Ed = default(_003C_003CInstallGtiName_003Eb__3_003Ed);
			_003C_003CInstallGtiName_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CInstallGtiName_003Eb__3_003Ed._003C_003E4__this = this;
			_003C_003CInstallGtiName_003Eb__3_003Ed.o = o;
			_003C_003CInstallGtiName_003Eb__3_003Ed.unsubscribed = unsubscribed;
			_003C_003CInstallGtiName_003Eb__3_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CInstallGtiName_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003CInstallGtiName_003Eb__3_003Ed>(ref _003C_003CInstallGtiName_003Eb__3_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CInstallGtiName_003Eb__3_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass37_0
	{
		[StructLayout(3)]
		private struct _003C_003CUninstallGtiName_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<TrainerInstallationStatus> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass37_0 _003C_003E4__this;

			private string _003CcatalogPath_003E5__2;

			private ConfiguredTaskAwaiter<GtiListingEntry> _003C_003Eu__1;

			private ConfiguredTaskAwaiter<IRescuedCatalogPackage> _003C_003Eu__2;

			private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__3;

			private PotentialPackageDirectory _003CpotentialDir_003E5__3;

			private void MoveNext()
			{
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0248: Unknown result type (might be due to invalid IL or missing references)
				//IL_024d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0255: Unknown result type (might be due to invalid IL or missing references)
				//IL_0300: Unknown result type (might be due to invalid IL or missing references)
				//IL_0305: Unknown result type (might be due to invalid IL or missing references)
				//IL_030d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_020a: Unknown result type (might be due to invalid IL or missing references)
				//IL_020f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0213: Unknown result type (might be due to invalid IL or missing references)
				//IL_0218: Unknown result type (might be due to invalid IL or missing references)
				//IL_028f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0299: Expected O, but got Unknown
				//IL_038c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0391: Unknown result type (might be due to invalid IL or missing references)
				//IL_0399: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_022d: Unknown result type (might be due to invalid IL or missing references)
				//IL_022f: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0401: Unknown result type (might be due to invalid IL or missing references)
				//IL_0406: Unknown result type (might be due to invalid IL or missing references)
				//IL_040e: Unknown result type (might be due to invalid IL or missing references)
				//IL_034e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0353: Unknown result type (might be due to invalid IL or missing references)
				//IL_0357: Unknown result type (might be due to invalid IL or missing references)
				//IL_035c: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0371: Unknown result type (might be due to invalid IL or missing references)
				//IL_0373: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Expected O, but got Unknown
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0193: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_019c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass37_0 _003C_003Ec__DisplayClass37_ = _003C_003E4__this;
				TrainerInstallationStatus result2;
				try
				{
					ConfiguredTaskAwaiter<GtiListingEntry> awaiter3;
					ConfiguredTaskAwaiter<IRescuedCatalogPackage> awaiter2;
					ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
					ITrainerPackage result5;
					ITrainerPackage result;
					GtiListingEntry result3;
					IRescuedCatalogPackage result4;
					switch (num)
					{
					default:
						awaiter3 = _003C_003Ec__DisplayClass37_._003C_003E4__this.GetMatchingListEntry(_003C_003Ec__DisplayClass37_.gtiName, _003C_003Ec__DisplayClass37_.package).DetachContext().GetAwaiter();
						if (!awaiter3.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter3;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<GtiListingEntry>, _003C_003CUninstallGtiName_003Eb__1_003Ed>(ref awaiter3, ref this);
							return;
						}
						goto IL_009d;
					case 0:
						awaiter3 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<GtiListingEntry>);
						num = (_003C_003E1__state = -1);
						goto IL_009d;
					case 1:
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<IRescuedCatalogPackage>);
						num = (_003C_003E1__state = -1);
						goto IL_01e8;
					case 2:
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
						num = (_003C_003E1__state = -1);
						goto IL_0264;
					case 3:
						awaiter = _003C_003Eu__3;
						_003C_003Eu__3 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
						num = (_003C_003E1__state = -1);
						goto IL_031c;
					case 4:
					case 5:
						{
							try
							{
								if (num != 4)
								{
									if (num == 5)
									{
										awaiter = _003C_003Eu__3;
										_003C_003Eu__3 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
										num = (_003C_003E1__state = -1);
										goto IL_041d;
									}
									awaiter = _003C_003Ec__DisplayClass37_._003C_003E4__this._parser.ParsePackage(_003CpotentialDir_003E5__3).DetachContext().GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = (_003C_003E1__state = 4);
										_003C_003Eu__3 = awaiter;
										_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CUninstallGtiName_003Eb__1_003Ed>(ref awaiter, ref this);
										return;
									}
								}
								else
								{
									awaiter = _003C_003Eu__3;
									_003C_003Eu__3 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
									num = (_003C_003E1__state = -1);
								}
								ITrainerPackage result6 = awaiter.GetResult();
								awaiter = _003C_003Ec__DisplayClass37_._003C_003E4__this._packageRepo.RegisterPackage(result6).DetachContext().GetAwaiter();
								if (!awaiter.IsCompleted)
								{
									num = (_003C_003E1__state = 5);
									_003C_003Eu__3 = awaiter;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CUninstallGtiName_003Eb__1_003Ed>(ref awaiter, ref this);
									return;
								}
								goto IL_041d;
								IL_041d:
								result5 = awaiter.GetResult();
							}
							catch (System.Exception error)
							{
								result2 = TrainerInstallationStatus.CreateError(_003C_003Ec__DisplayClass37_.package, error);
								goto end_IL_000e;
							}
							break;
						}
						IL_0264:
						result = awaiter.GetResult();
						result2 = ((result == null) ? TrainerInstallationStatus.CreateError(_003C_003Ec__DisplayClass37_.package, (System.Exception)new KeyNotFoundException("The GTI package was copied to the safe location but not registered. Please report this to Garmin.")) : TrainerInstallationStatus.CreateReady(_003C_003Ec__DisplayClass37_.package, result));
						goto end_IL_000e;
						IL_009d:
						result3 = awaiter3.GetResult();
						if (result3 == null || result3.IsDiscontinued)
						{
							result2 = TrainerInstallationStatus.CreateAcceptableResultMissing(_003C_003Ec__DisplayClass37_.package);
						}
						else
						{
							List<string> val = new List<string>();
							_003CcatalogPath_003E5__2 = result3.CatalogedPath;
							if (_003CcatalogPath_003E5__2 == null)
							{
								val.Add("GTI did not report a cataloged path for the " + result3.EntryName + " entry.");
							}
							Version catalogedVersion = result3.CatalogedVersion;
							if (catalogedVersion == null)
							{
								val.Add("GTI did not report a cataloged version for the " + result3.EntryName + " entry.");
							}
							if (val.Count <= 0 && _003CcatalogPath_003E5__2 != null && catalogedVersion != null)
							{
								awaiter2 = _003C_003Ec__DisplayClass37_._003C_003E4__this._packageRefuge.GetRescuedPackageByGtiInfoOrNull(result3.EntryName, catalogedVersion).DetachContext().GetAwaiter();
								if (!awaiter2.IsCompleted)
								{
									num = (_003C_003E1__state = 1);
									_003C_003Eu__2 = awaiter2;
									_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IRescuedCatalogPackage>, _003C_003CUninstallGtiName_003Eb__1_003Ed>(ref awaiter2, ref this);
									return;
								}
								goto IL_01e8;
							}
							string text = string.Join(",\n\t", (System.Collections.Generic.IEnumerable<string>)val);
							Logger.Log(LogLevel.Error, "Catalog corruption detected:\n\t" + text + ".");
							result2 = TrainerInstallationStatus.CreateError(_003C_003Ec__DisplayClass37_.package, (System.Exception)new KeyNotFoundException("Catalog corruption detected. Please restart the Launcher."));
						}
						goto end_IL_000e;
						IL_01e8:
						result4 = awaiter2.GetResult();
						if (result4 != null)
						{
							awaiter = _003C_003Ec__DisplayClass37_._003C_003E4__this.GetRegisteredPackageOrNullIfError(result4.RescuedDirectory).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CUninstallGtiName_003Eb__1_003Ed>(ref awaiter, ref this);
								return;
							}
							goto IL_0264;
						}
						_003CpotentialDir_003E5__3 = PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, _003CcatalogPath_003E5__2);
						awaiter = _003C_003Ec__DisplayClass37_._003C_003E4__this.GetRegisteredPackageOrNullIfError(_003CpotentialDir_003E5__3).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 3);
							_003C_003Eu__3 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CUninstallGtiName_003Eb__1_003Ed>(ref awaiter, ref this);
							return;
						}
						goto IL_031c;
						IL_031c:
						result5 = awaiter.GetResult();
						if (result5 != null)
						{
							break;
						}
						goto case 4;
					}
					result2 = TrainerInstallationStatus.CreateReady(_003C_003Ec__DisplayClass37_.package, result5);
					end_IL_000e:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003CcatalogPath_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003CcatalogPath_003E5__2 = null;
				_003C_003Et__builder.SetResult(result2);
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}
		}

		public string gtiName;

		public CancellationToken userCancel;

		public ITrainerPackage package;

		public GtiPackageInstaller _003C_003E4__this;

		internal IObservable<TrainerInstallationStatus> _003CUninstallGtiName_003Eb__0(ILauncherGtiRunner gtiExe)
		{
			return Observable.Create<TrainerInstallationStatus>((Func<IObserver<TrainerInstallationStatus>, CancellationToken, System.Threading.Tasks.Task>)new _003C_003Ec__DisplayClass37_1
			{
				CS_0024_003C_003E8__locals1 = this,
				gtiExe = gtiExe
			}._003CUninstallGtiName_003Eb__3);
		}

		[AsyncStateMachine(typeof(_003C_003CUninstallGtiName_003Eb__1_003Ed))]
		internal async System.Threading.Tasks.Task<TrainerInstallationStatus> _003CUninstallGtiName_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			GtiListingEntry gtiListingEntry = await _003C_003E4__this.GetMatchingListEntry(gtiName, package).DetachContext();
			if (gtiListingEntry == null || gtiListingEntry.IsDiscontinued)
			{
				return TrainerInstallationStatus.CreateAcceptableResultMissing(package);
			}
			List<string> val = new List<string>();
			string catalogPath = gtiListingEntry.CatalogedPath;
			if (catalogPath == null)
			{
				val.Add("GTI did not report a cataloged path for the " + gtiListingEntry.EntryName + " entry.");
			}
			Version catalogedVersion = gtiListingEntry.CatalogedVersion;
			if (catalogedVersion == null)
			{
				val.Add("GTI did not report a cataloged version for the " + gtiListingEntry.EntryName + " entry.");
			}
			if (val.Count > 0 || catalogPath == null || catalogedVersion == null)
			{
				string text = string.Join(",\n\t", (System.Collections.Generic.IEnumerable<string>)val);
				Logger.Log(LogLevel.Error, "Catalog corruption detected:\n\t" + text + ".");
				return TrainerInstallationStatus.CreateError(package, (System.Exception)new KeyNotFoundException("Catalog corruption detected. Please restart the Launcher."));
			}
			IRescuedCatalogPackage rescuedCatalogPackage = await _003C_003E4__this._packageRefuge.GetRescuedPackageByGtiInfoOrNull(gtiListingEntry.EntryName, catalogedVersion).DetachContext();
			if (rescuedCatalogPackage != null)
			{
				ITrainerPackage trainerPackage = await _003C_003E4__this.GetRegisteredPackageOrNullIfError(rescuedCatalogPackage.RescuedDirectory).DetachContext();
				if (trainerPackage != null)
				{
					return TrainerInstallationStatus.CreateReady(package, trainerPackage);
				}
				return TrainerInstallationStatus.CreateError(package, (System.Exception)new KeyNotFoundException("The GTI package was copied to the safe location but not registered. Please report this to Garmin."));
			}
			PotentialPackageDirectory potentialDir = PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, catalogPath);
			ITrainerPackage trainerPackage2 = await _003C_003E4__this.GetRegisteredPackageOrNullIfError(potentialDir).DetachContext();
			if (trainerPackage2 == null)
			{
				try
				{
					ITrainerPackage trainerPackage3 = await _003C_003E4__this._parser.ParsePackage(potentialDir).DetachContext();
					trainerPackage2 = await _003C_003E4__this._packageRepo.RegisterPackage(trainerPackage3).DetachContext();
				}
				catch (System.Exception error)
				{
					return TrainerInstallationStatus.CreateError(package, error);
				}
			}
			return TrainerInstallationStatus.CreateReady(package, trainerPackage2);
		}

		internal IObservable<TrainerInstallationStatus> _003CUninstallGtiName_003Eb__2(System.Exception error)
		{
			return _003C_003E4__this.EndWithErrorOrCanceled(package, error);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass37_1
	{
		[StructLayout(3)]
		private struct _003C_003CUninstallGtiName_003Eb__3_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass37_1 _003C_003E4__this;

			public CancellationToken unsubscribed;

			public IObserver<TrainerInstallationStatus> o;

			private ICancellation _003Ccombined_003E5__2;

			private ConfiguredTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass37_1 _003C_003Ec__DisplayClass37_ = _003C_003E4__this;
				try
				{
					IGtiProcessExecution gtiSession = default(IGtiProcessExecution);
					if (num != 0)
					{
						gtiSession = _003C_003Ec__DisplayClass37_.gtiExe.Uninstall(_003C_003Ec__DisplayClass37_.CS_0024_003C_003E8__locals1.gtiName);
						_003Ccombined_003E5__2 = CancellationFactory.CreateLinked(_003C_003Ec__DisplayClass37_.CS_0024_003C_003E8__locals1.userCancel, unsubscribed);
					}
					try
					{
						ConfiguredTaskAwaiter awaiter;
						if (num != 0)
						{
							ConfiguredTaskAwaitable val = HandleGtiOutputAndCancelations(_003C_003Ec__DisplayClass37_.CS_0024_003C_003E8__locals1.package, o, gtiSession, _003Ccombined_003E5__2.Token).DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003C_003CUninstallGtiName_003Eb__3_003Ed>(ref awaiter, ref this);
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
					finally
					{
						if (num < 0 && _003Ccombined_003E5__2 != null)
						{
							((System.IDisposable)_003Ccombined_003E5__2).Dispose();
						}
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003Ccombined_003E5__2 = null;
					((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003Ccombined_003E5__2 = null;
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetResult();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
			}
		}

		public ILauncherGtiRunner gtiExe;

		public _003C_003Ec__DisplayClass37_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CUninstallGtiName_003Eb__3_003Ed))]
		internal System.Threading.Tasks.Task _003CUninstallGtiName_003Eb__3(IObserver<TrainerInstallationStatus> o, CancellationToken unsubscribed)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CUninstallGtiName_003Eb__3_003Ed _003C_003CUninstallGtiName_003Eb__3_003Ed = default(_003C_003CUninstallGtiName_003Eb__3_003Ed);
			_003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
			_003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003E4__this = this;
			_003C_003CUninstallGtiName_003Eb__3_003Ed.o = o;
			_003C_003CUninstallGtiName_003Eb__3_003Ed.unsubscribed = unsubscribed;
			_003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003E1__state = -1;
			((AsyncTaskMethodBuilder)(ref _003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003CUninstallGtiName_003Eb__3_003Ed>(ref _003C_003CUninstallGtiName_003Eb__3_003Ed);
			return ((AsyncTaskMethodBuilder)(ref _003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003Et__builder)).Task;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass41_0
	{
		public Func<GtiListingEntry, bool> predicate;

		public string gtiName;

		public GtiPackageInstaller _003C_003E4__this;

		public ITrainerPackage package;

		internal bool _003CGetMatchingListEntry_003Eb__2(GtiListingEntry le)
		{
			return predicate.Invoke(le);
		}

		internal bool _003CGetMatchingListEntry_003Eb__3(GtiListingEntry listEntry)
		{
			if (!((EqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash).Equals(listEntry.CatalogedPath, package.BaseInfo.PackageRootPath))
			{
				return ((EqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash).Equals(listEntry.InstalledPath, package.BaseInfo.PackageRootPath);
			}
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass41_1
	{
		public Func<GtiListingEntry, bool> baseClosure;

		public _003C_003Ec__DisplayClass41_0 CS_0024_003C_003E8__locals1;

		internal bool _003CGetMatchingListEntry_003Eb__4(GtiListingEntry listEntry)
		{
			if (!(listEntry.EntryName == CS_0024_003C_003E8__locals1.gtiName))
			{
				return baseClosure.Invoke(listEntry);
			}
			return true;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCancelGtiProcess_003Ed__27 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IGtiProcessExecution gtiSession;

		private ConfiguredTaskAwaiter<bool> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<System.Threading.Tasks.Task> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				if (num == 0)
				{
					goto IL_0027;
				}
				ConfiguredTaskAwaiter<System.Threading.Tasks.Task> awaiter;
				if (num == 1)
				{
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<System.Threading.Tasks.Task>);
					num = (_003C_003E1__state = -1);
					goto IL_011d;
				}
				if (!((System.Threading.Tasks.Task)gtiSession.ProcessResult).IsCompleted)
				{
					goto IL_0027;
				}
				goto end_IL_0007;
				IL_0027:
				bool flag;
				try
				{
					ConfiguredTaskAwaiter<bool> awaiter2;
					if (num != 0)
					{
						awaiter2 = gtiSession.SendCancelSignal().DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<bool>, _003CCancelGtiProcess_003Ed__27>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<bool>);
						num = (_003C_003E1__state = -1);
					}
					flag = awaiter2.GetResult();
				}
				catch (ObjectDisposedException)
				{
					flag = false;
				}
				if (flag)
				{
					awaiter = System.Threading.Tasks.Task.WhenAny((System.Threading.Tasks.Task)gtiSession.ProcessResult, System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10.0))).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.Threading.Tasks.Task>, _003CCancelGtiProcess_003Ed__27>(ref awaiter, ref this);
						return;
					}
					goto IL_011d;
				}
				goto IL_0125;
				IL_0125:
				if (!((System.Threading.Tasks.Task)gtiSession.ProcessResult).IsCompleted)
				{
					Logger.Log(LogLevel.Info, "Force killing GTI because the nice signal took to long to stop it.");
					gtiSession.Process.ForceClose();
				}
				goto end_IL_0007;
				IL_011d:
				awaiter.GetResult();
				goto IL_0125;
				end_IL_0007:;
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
	private struct _003CGetInstalledPackageForListing_003Ed__38 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ITrainerPackage> _003C_003Et__builder;

		public GtiListingEntry installedMatchingListEntry;

		public GtiPackageInstaller _003C_003E4__this;

		private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			GtiPackageInstaller gtiPackageInstaller = _003C_003E4__this;
			ITrainerPackage result;
			try
			{
				ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
					num = (_003C_003E1__state = -1);
					goto IL_0090;
				}
				if (installedMatchingListEntry.InstalledPath != null)
				{
					awaiter = gtiPackageInstaller.GetRegisteredPackageOrNullIfError(PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.InstalledByGti, installedMatchingListEntry.InstalledPath)).DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CGetInstalledPackageForListing_003Ed__38>(ref awaiter, ref this);
						return;
					}
					goto IL_0090;
				}
				result = null;
				goto end_IL_000e;
				IL_0090:
				result = awaiter.GetResult();
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
	private struct _003CGetMatchingListEntry_003Ed__41 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<GtiListingEntry> _003C_003Et__builder;

		public string gtiName;

		public GtiPackageInstaller _003C_003E4__this;

		public ITrainerPackage package;

		private _003C_003Ec__DisplayClass41_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<Func<GtiListingEntry, bool>> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ILauncherGtiRunner> _003C_003Eu__2;

		private object _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			GtiPackageInstaller gtiPackageInstaller = _003C_003E4__this;
			GtiListingEntry result2;
			try
			{
				ConfiguredTaskAwaiter<Func<GtiListingEntry, bool>> awaiter2;
				ConfiguredTaskAwaiter<ILauncherGtiRunner> awaiter;
				AsyncSubject<GtiListingEntry> asyncSubject;
				Func<GtiListingEntry, bool> result;
				switch (num)
				{
				default:
					_003C_003E8__1 = new _003C_003Ec__DisplayClass41_0();
					_003C_003E8__1.gtiName = gtiName;
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.package = package;
					awaiter2 = CreateListEntryMatcher().DetachContext().GetAwaiter();
					if (!awaiter2.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter2;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<Func<GtiListingEntry, bool>>, _003CGetMatchingListEntry_003Ed__41>(ref awaiter2, ref this);
						return;
					}
					goto IL_00c3;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<Func<GtiListingEntry, bool>>);
					num = (_003C_003E1__state = -1);
					goto IL_00c3;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ILauncherGtiRunner>);
					num = (_003C_003E1__state = -1);
					goto IL_013c;
				case 2:
					{
						asyncSubject = (AsyncSubject<GtiListingEntry>)_003C_003Eu__3;
						_003C_003Eu__3 = null;
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_013c:
					asyncSubject = awaiter.GetResult().List().GetListEntries()
						.Catch<GtiListingEntry, GtiProcessException>((GtiProcessException e) => Observable.Throw<GtiListingEntry>(new GenericErrorException("Catalog installation failure detected. Please restart the Launcher.", e)))
						.FirstOrDefaultAsync<GtiListingEntry>((GtiListingEntry le) => _003C_003E8__1.predicate.Invoke(le))
						.GetAwaiter<GtiListingEntry>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = asyncSubject;
						_003C_003Et__builder.AwaitOnCompleted<AsyncSubject<GtiListingEntry>, _003CGetMatchingListEntry_003Ed__41>(ref asyncSubject, ref this);
						return;
					}
					break;
					IL_00c3:
					result = awaiter2.GetResult();
					_003C_003E8__1.predicate = result;
					awaiter = gtiPackageInstaller._gtiProvider.GtiExe.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ILauncherGtiRunner>, _003CGetMatchingListEntry_003Ed__41>(ref awaiter, ref this);
						return;
					}
					goto IL_013c;
				}
				result2 = asyncSubject.GetResult();
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
			[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass41_0._003C_003CGetMatchingListEntry_003Eg__CreateListEntryMatcher_007C0_003Ed))]
			async System.Threading.Tasks.Task<Func<GtiListingEntry, bool>> CreateListEntryMatcher()
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				if (string.IsNullOrEmpty(((_003C_003Ec__DisplayClass41_0)(object)this).gtiName))
				{
					((_003C_003Ec__DisplayClass41_0)(object)this).gtiName = (await ((_003C_003Ec__DisplayClass41_0)(object)this)._003C_003E4__this._packageRefuge.GetRescuedPackageByPathOrNull(((_003C_003Ec__DisplayClass41_0)(object)this).package.BaseInfo.PackageRootPath).DetachContext())?.GtiName ?? string.Empty;
				}
				Func<GtiListingEntry, bool> val = (GtiListingEntry listEntry) => ((EqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash).Equals(listEntry.CatalogedPath, ((_003C_003Ec__DisplayClass41_0)(object)this).package.BaseInfo.PackageRootPath) || ((EqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash).Equals(listEntry.InstalledPath, ((_003C_003Ec__DisplayClass41_0)(object)this).package.BaseInfo.PackageRootPath);
				if (((_003C_003Ec__DisplayClass41_0)(object)this).gtiName.Length > 0)
				{
					val = new _003C_003Ec__DisplayClass41_1
					{
						CS_0024_003C_003E8__locals1 = this,
						baseClosure = val
					}._003CGetMatchingListEntry_003Eb__4;
				}
				return val;
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
	private struct _003CGetRegisteredPackageOrNullIfError_003Ed__39 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ITrainerPackage> _003C_003Et__builder;

		public PotentialPackageDirectory packageDirectory;

		public GtiPackageInstaller _003C_003E4__this;

		private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			GtiPackageInstaller gtiPackageInstaller = _003C_003E4__this;
			ITrainerPackage result;
			try
			{
				if (num != 0 && !packageDirectory.HasPotential)
				{
					result = null;
				}
				else
				{
					try
					{
						ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
						if (num != 0)
						{
							awaiter = gtiPackageInstaller._packageRepo.GetPackageByLocationOrNull(packageDirectory.PackageDirectory.FullPath).DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003CGetRegisteredPackageOrNullIfError_003Ed__39>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<ITrainerPackage>);
							num = (_003C_003E1__state = -1);
						}
						result = awaiter.GetResult();
					}
					catch (KeyNotFoundException)
					{
						result = null;
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
	private struct _003CHandleGtiOutputAndCancelations_003Ed__28 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public IGtiProcessExecution gtiSession;

		public ITrainerPackage package;

		public CancellationToken token;

		public IObserver<TrainerInstallationStatus> o;

		private CancellationTokenRegistration _003C_003E7__wrap1;

		private System.IDisposable _003C_003E7__wrap2;

		private ConfiguredTaskAwaiter<GtiProcessResult> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				_003C_003Ec__DisplayClass28_0 _003C_003Ec__DisplayClass28_ = default(_003C_003Ec__DisplayClass28_0);
				if (num != 0)
				{
					_003C_003Ec__DisplayClass28_ = new _003C_003Ec__DisplayClass28_0
					{
						gtiSession = gtiSession,
						package = package
					};
					_003C_003E7__wrap1 = ((CancellationToken)(ref token)).Register(new Action(_003C_003Ec__DisplayClass28_._003CHandleGtiOutputAndCancelations_003Eb__0), false);
				}
				try
				{
					if (num != 0)
					{
						_003C_003E7__wrap2 = _003C_003Ec__DisplayClass28_.gtiSession.GetProgressUpdates().Select<GtiProgress, TrainerInstallationStatus>((Func<GtiProgress, TrainerInstallationStatus>)_003C_003Ec__DisplayClass28_._003CHandleGtiOutputAndCancelations_003Eb__1).SubscribeSafe<TrainerInstallationStatus>(o);
					}
					try
					{
						ConfiguredTaskAwaiter<GtiProcessResult> awaiter;
						if (num != 0)
						{
							awaiter = _003C_003Ec__DisplayClass28_.gtiSession.ProcessResult.Catch().DetachContext().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<GtiProcessResult>, _003CHandleGtiOutputAndCancelations_003Ed__28>(ref awaiter, ref this);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(ConfiguredTaskAwaiter<GtiProcessResult>);
							num = (_003C_003E1__state = -1);
						}
						awaiter.GetResult();
					}
					finally
					{
						if (num < 0 && _003C_003E7__wrap2 != null)
						{
							_003C_003E7__wrap2.Dispose();
						}
					}
					_003C_003E7__wrap2 = null;
				}
				finally
				{
					if (num < 0)
					{
						((System.IDisposable)(CancellationTokenRegistration)(ref _003C_003E7__wrap1)).Dispose();
					}
				}
				_003C_003E7__wrap1 = default(CancellationTokenRegistration);
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
	private struct _003CPowerDownAircraftIfPackageIsInUse_003Ed__34 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public ITrainerPackage package;

		public GtiPackageInstaller _003C_003E4__this;

		private _003C_003Ec__DisplayClass34_0 _003C_003E8__1;

		private bool _003CneedToPowerDown_003E5__2;

		private ConfiguredTaskAwaiter<AircraftIdentifier> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<SimPanel> _003C_003Eu__2;

		private object _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			GtiPackageInstaller gtiPackageInstaller = _003C_003E4__this;
			try
			{
				ConfiguredTaskAwaiter<AircraftIdentifier> awaiter2;
				ConfiguredTaskAwaiter<SimPanel> awaiter;
				AsyncSubject<bool> asyncSubject;
				AircraftIdentifier result;
				switch (num)
				{
				default:
					_003C_003E8__1 = new _003C_003Ec__DisplayClass34_0();
					_003C_003E8__1.package = package;
					if (gtiPackageInstaller._simRunner.IsRunning)
					{
						_003CneedToPowerDown_003E5__2 = false;
						awaiter2 = gtiPackageInstaller._unitOfWork.Aircraft.GetLastSelectedAircraftOrDefault().DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<AircraftIdentifier>, _003CPowerDownAircraftIfPackageIsInUse_003Ed__34>(ref awaiter2, ref this);
							return;
						}
						goto IL_00ba;
					}
					goto end_IL_000e;
				case 0:
					awaiter2 = _003C_003Eu__1;
					_003C_003Eu__1 = default(ConfiguredTaskAwaiter<AircraftIdentifier>);
					num = (_003C_003E1__state = -1);
					goto IL_00ba;
				case 1:
					awaiter = _003C_003Eu__2;
					_003C_003Eu__2 = default(ConfiguredTaskAwaiter<SimPanel>);
					num = (_003C_003E1__state = -1);
					goto IL_0139;
				case 2:
					{
						asyncSubject = (AsyncSubject<bool>)_003C_003Eu__3;
						_003C_003Eu__3 = null;
						num = (_003C_003E1__state = -1);
						break;
					}
					IL_0139:
					if (Enumerable.Any<SimInstallData>(awaiter.GetResult().ActiveSims, (Func<SimInstallData, bool>)((SimInstallData s) => s.FullPackageId.IsSameEmbeddedRelease(_003C_003E8__1.package.TrainerPackageId))))
					{
						_003CneedToPowerDown_003E5__2 = true;
					}
					goto IL_0164;
					IL_0164:
					if (_003CneedToPowerDown_003E5__2)
					{
						gtiPackageInstaller._simRunner.Abort();
					}
					asyncSubject = gtiPackageInstaller._simRunner.WhenIsRunningChanges.WhereEqual(operand: false).FirstAsync<bool>().GetAwaiter<bool>();
					if (!asyncSubject.IsCompleted)
					{
						num = (_003C_003E1__state = 2);
						_003C_003Eu__3 = asyncSubject;
						((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitOnCompleted<AsyncSubject<bool>, _003CPowerDownAircraftIfPackageIsInUse_003Ed__34>(ref asyncSubject, ref this);
						return;
					}
					break;
					IL_00ba:
					result = awaiter2.GetResult();
					if (result.IsValid)
					{
						awaiter = gtiPackageInstaller._unitOfWork.Simulators.GetSimulatorPanel(result).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							((AsyncTaskMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<SimPanel>, _003CPowerDownAircraftIfPackageIsInUse_003Ed__34>(ref awaiter, ref this);
							return;
						}
						goto IL_0139;
					}
					goto IL_0164;
				}
				asyncSubject.GetResult();
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

	private readonly ILauncherGtiProvider _gtiProvider;

	private readonly ITrainerPackageRepoEvolved _packageRepo;

	private readonly ITrainerPackageParser _parser;

	private readonly IGtiDirectorySuppressor _gtiSuppressor;

	private readonly ICatalogPackageRefuge _packageRefuge;

	private readonly IUnitOfWork _unitOfWork;

	private readonly IRunSimulators _simRunner;

	private readonly INetworkDetector _networkDetector;

	private readonly ISubject<ITrainerPackageInstallationSession> _sessionEvent = new Subject<ITrainerPackageInstallationSession>();

	private readonly ConcurrentDictionary<TrainerIdentifier, GtiIndividualPackageManagement> _cachedOperations = new ConcurrentDictionary<TrainerIdentifier, GtiIndividualPackageManagement>();

	private readonly IPendingTaskTracker _pendingTask;

	private readonly TrackedEvent.SelectAppIdBasedOnTrainer _selectAppId;

	private readonly TrackedEvent.LogTrackedEvent _sendLog;

	public System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession> CurrentSessions => (System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession>)Enumerable.Select<KeyValuePair<TrainerIdentifier, GtiIndividualPackageManagement>, GtiPackageOperation>((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerIdentifier, GtiIndividualPackageManagement>>)_cachedOperations, (Func<KeyValuePair<TrainerIdentifier, GtiIndividualPackageManagement>, GtiPackageOperation>)((KeyValuePair<TrainerIdentifier, GtiIndividualPackageManagement> kv) => kv.Value.CurrentOperation));

	public IObservable<ITrainerPackageInstallationSession> FutureSessions => ((IObservable<ITrainerPackageInstallationSession>)_sessionEvent).AsObservable<ITrainerPackageInstallationSession>();

	public GtiPackageInstaller(ILauncherGtiProvider gtiProvider, IGtiDirectorySuppressor gtiSuppressor, ICatalogPackageRefuge packageRefuge, IUnitOfWork unitOfwork, ITrainerPackageParser parser, IRunSimulators simRunner, INetworkDetector networkDetector, IPendingTaskTracker pendingTask, TrackedEvent.SelectAppIdBasedOnTrainer selectAppId, TrackedEvent.LogTrackedEvent sendLog)
	{
		_gtiProvider = gtiProvider;
		_packageRepo = unitOfwork.Packages;
		_parser = parser;
		_gtiSuppressor = gtiSuppressor;
		_packageRefuge = packageRefuge;
		_unitOfWork = unitOfwork;
		_simRunner = simRunner;
		_networkDetector = networkDetector;
		_pendingTask = pendingTask;
		_selectAppId = selectAppId;
		_sendLog = sendLog;
	}

	public System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession> GetAllSessions()
	{
		return (System.Collections.Generic.IEnumerable<ITrainerPackageInstallationSession>)Enumerable.SelectMany<KeyValuePair<TrainerIdentifier, GtiIndividualPackageManagement>, GtiPackageOperation>((System.Collections.Generic.IEnumerable<KeyValuePair<TrainerIdentifier, GtiIndividualPackageManagement>>)_cachedOperations, (Func<KeyValuePair<TrainerIdentifier, GtiIndividualPackageManagement>, System.Collections.Generic.IEnumerable<GtiPackageOperation>>)((KeyValuePair<TrainerIdentifier, GtiIndividualPackageManagement> kv) => (System.Collections.Generic.IEnumerable<GtiPackageOperation>)kv.Value.GetAllOperations()));
	}

	public ITrainerPackageInstallationSession Install(ITrainerPackage package, bool forceRetry = false)
	{
		ITrainerPackage package2 = package;
		ThrowIfInvalidPackage(package2);
		GtiIndividualPackageManagement gtiIndividualPackageManagement = AddOrUpdateEmbeddedReleaseList(package2, TrainerPackageOperation.Install, InsertInstallPlan, (GtiPackageOperation completedOperation) => forceRetry || package2.IsSuccessorOf(completedOperation.Package) || (completedOperation.Failed && CompletedTimeoutElapsed(completedOperation, TimeSpan.FromSeconds(30.0))));
		ITrainerPackageInstallationSession trainerPackageInstallationSession = StartReportAndReturn(gtiIndividualPackageManagement.CurrentOperation);
		_pendingTask.TrackTask(WaitForInstallStatusThenSendEvent(package2, trainerPackageInstallationSession, (ITrainerPackage pkg) => _selectAppId(pkg)));
		return trainerPackageInstallationSession;
	}

	public ITrainerPackageInstallationSession Uninstall(ITrainerPackage package, bool forceRetry = false)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ThrowIfInvalidPackage(package);
		if (package.BaseInfo.PackageType == TrainerPackageType.LegacyPermanentInstall || package.BaseInfo.PackageType == TrainerPackageType.TemporaryPackage)
		{
			throw new ArgumentException("Cannot uninstall this package.");
		}
		Option<AppCenterAppId> appId = _selectAppId(package);
		GtiIndividualPackageManagement gtiIndividualPackageManagement = AddOrUpdateEmbeddedReleaseList(package, TrainerPackageOperation.Uninstall, InsertUninstallPlan, (GtiPackageOperation completeOperation) => forceRetry);
		ITrainerPackageInstallationSession trainerPackageInstallationSession = StartReportAndReturn(gtiIndividualPackageManagement.CurrentOperation);
		_pendingTask.TrackTask(WaitForInstallStatusThenSendEvent(package, trainerPackageInstallationSession, (ITrainerPackage pkg) => appId));
		return trainerPackageInstallationSession;
	}

	public ITrainerPackageInstallationSession? GetCurrentSessionForPackage(ITrainerPackage package)
	{
		GtiIndividualPackageManagement gtiIndividualPackageManagement = default(GtiIndividualPackageManagement);
		if (_cachedOperations.TryGetValue(GetEmbeddedReleaseIdFromPackage(package), ref gtiIndividualPackageManagement))
		{
			return gtiIndividualPackageManagement.CurrentOperation;
		}
		return null;
	}

	public void DismissErroredSession(ITrainerPackageInstallationSession failedSession)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!failedSession.IsFailure())
		{
			throw new ArgumentException("Must be an errored session.");
		}
		TrainerIdentifier embeddedReleaseIdFromPackage = GetEmbeddedReleaseIdFromPackage(failedSession.RequestingPackage);
		GtiIndividualPackageManagement gtiIndividualPackageManagement = default(GtiIndividualPackageManagement);
		if (!_cachedOperations.TryRemove(embeddedReleaseIdFromPackage, ref gtiIndividualPackageManagement))
		{
			return;
		}
		gtiIndividualPackageManagement.DismissErrors();
		if (gtiIndividualPackageManagement.IsEmpty)
		{
			return;
		}
		GtiIndividualPackageManagement gtiIndividualPackageManagement2 = default(GtiIndividualPackageManagement);
		while (!_cachedOperations.TryAdd(embeddedReleaseIdFromPackage, gtiIndividualPackageManagement))
		{
			if (_cachedOperations.TryGetValue(embeddedReleaseIdFromPackage, ref gtiIndividualPackageManagement2))
			{
				gtiIndividualPackageManagement2.Inherit(gtiIndividualPackageManagement);
				if (_cachedOperations.TryUpdate(embeddedReleaseIdFromPackage, gtiIndividualPackageManagement2, gtiIndividualPackageManagement2))
				{
					break;
				}
			}
		}
	}

	private static TrainerIdentifier GetEmbeddedReleaseIdFromPackage(ITrainerPackage package)
	{
		return package.TrainerPackageId.EmbeddedReleaseCompositeId;
	}

	private static void ThrowIfInvalidPackage(ITrainerPackage package)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (package == null || package.BaseInfo == null || string.IsNullOrEmpty(package.BaseInfo.PackageRootPath))
		{
			throw new ArgumentException("Invalid package.");
		}
	}

	private static bool CompletedTimeoutElapsed(GtiPackageOperation previous, TimeSpan timeout)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		DateTimeOffset? val = previous.CompletedAt();
		if (!val.HasValue)
		{
			throw new ArgumentException("Operation must be completed.");
		}
		DateTimeOffset now = DateTimeOffset.Now;
		DateTimeOffset? val2 = val;
		TimeSpan? val3 = (val2.HasValue ? new TimeSpan?(now - val2.GetValueOrDefault()) : null);
		if (!val3.HasValue)
		{
			return false;
		}
		return val3.GetValueOrDefault() >= timeout;
	}

	private static IObservable<TrainerInstallationStatus> ImmediateProgress(ITrainerPackage package)
	{
		return Observable.Return(TrainerInstallationStatus.CreateProgress(package, 0.0));
	}

	[AsyncStateMachine(typeof(_003CCancelGtiProcess_003Ed__27))]
	private static System.Threading.Tasks.Task CancelGtiProcess(IGtiProcessExecution gtiSession)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CCancelGtiProcess_003Ed__27 _003CCancelGtiProcess_003Ed__ = default(_003CCancelGtiProcess_003Ed__27);
		_003CCancelGtiProcess_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CCancelGtiProcess_003Ed__.gtiSession = gtiSession;
		_003CCancelGtiProcess_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CCancelGtiProcess_003Ed__._003C_003Et__builder)).Start<_003CCancelGtiProcess_003Ed__27>(ref _003CCancelGtiProcess_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CCancelGtiProcess_003Ed__._003C_003Et__builder)).Task;
	}

	[AsyncStateMachine(typeof(_003CHandleGtiOutputAndCancelations_003Ed__28))]
	private static System.Threading.Tasks.Task HandleGtiOutputAndCancelations(ITrainerPackage package, IObserver<TrainerInstallationStatus> o, IGtiProcessExecution gtiSession, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_003CHandleGtiOutputAndCancelations_003Ed__28 _003CHandleGtiOutputAndCancelations_003Ed__ = default(_003CHandleGtiOutputAndCancelations_003Ed__28);
		_003CHandleGtiOutputAndCancelations_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CHandleGtiOutputAndCancelations_003Ed__.package = package;
		_003CHandleGtiOutputAndCancelations_003Ed__.o = o;
		_003CHandleGtiOutputAndCancelations_003Ed__.gtiSession = gtiSession;
		_003CHandleGtiOutputAndCancelations_003Ed__.token = token;
		_003CHandleGtiOutputAndCancelations_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CHandleGtiOutputAndCancelations_003Ed__._003C_003Et__builder)).Start<_003CHandleGtiOutputAndCancelations_003Ed__28>(ref _003CHandleGtiOutputAndCancelations_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CHandleGtiOutputAndCancelations_003Ed__._003C_003Et__builder)).Task;
	}

	private static string GetGtiNameForPackage(GtiListingEntry matchingListEntry, ITrainerPackage package)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (matchingListEntry == null)
		{
			throw new KeyNotFoundException("GTI did not list a package for: " + package.BaseInfo.PackageRootPath);
		}
		return matchingListEntry.EntryName;
	}

	private ITrainerPackageInstallationSession StartReportAndReturn(GtiPackageOperation currentOperation)
	{
		if (currentOperation.Start())
		{
			((IObserver<ITrainerPackageInstallationSession>)_sessionEvent).OnNext((ITrainerPackageInstallationSession)currentOperation);
		}
		return currentOperation;
	}

	private GtiIndividualPackageManagement AddOrUpdateEmbeddedReleaseList(ITrainerPackage package, TrainerPackageOperation operation, Func<ITrainerPackage, GtiPackageOperation> operationFactory, Func<GtiPackageOperation, bool> canRetryPredicate)
	{
		Func<ITrainerPackage, GtiPackageOperation> operationFactory2 = operationFactory;
		ITrainerPackage package2 = package;
		Func<GtiPackageOperation, bool> canRetryPredicate2 = canRetryPredicate;
		return _cachedOperations.AddOrUpdate(GetEmbeddedReleaseIdFromPackage(package2), (Func<TrainerIdentifier, GtiIndividualPackageManagement>)((TrainerIdentifier pi) => new GtiIndividualPackageManagement(pi, () => operationFactory2.Invoke(package2))), (Func<TrainerIdentifier, GtiIndividualPackageManagement, GtiIndividualPackageManagement>)((TrainerIdentifier _, GtiIndividualPackageManagement previous) => previous.GetCreateOrThrow(operation, canRetryPredicate2, () => operationFactory2.Invoke(package2))));
	}

	private GtiPackageOperation InsertInstallPlan(ITrainerPackage package)
	{
		ITrainerPackage package2 = package;
		return new GtiPackageOperation(package2, TrainerPackageOperation.Install, (CancellationToken cancel) => Observable.Defer<TrainerInstallationStatus>((Func<IObservable<TrainerInstallationStatus>>)(() => package2.BaseInfo.PackageType.IsCompleteOnDisk() ? Observable.Return(TrainerInstallationStatus.CreateReady(package2, package2)) : ImmediateProgress(package2).Concat<TrainerInstallationStatus>(Observable.FromAsync<GtiListingEntry>((Func<System.Threading.Tasks.Task<GtiListingEntry>>)(() => GetMatchingListEntry(string.Empty, package2))).SelectMany<GtiListingEntry, IObservable<TrainerInstallationStatus>>((Func<GtiListingEntry, System.Threading.Tasks.Task<IObservable<TrainerInstallationStatus>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass32_1._003C_003CInsertInstallPlan_003Eb__3_003Ed))] async (GtiListingEntry gtiListing) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (gtiListing == null)
			{
				return Observable.Return(TrainerInstallationStatus.CreateError(package2, (System.Exception)new KeyNotFoundException("This version is no longer listed. This usually occurs when a new version is discovered during an operation. Please retry.")));
			}
			try
			{
				await PowerDownAircraftIfPackageIsInUse(package2).DetachContext();
			}
			catch (System.Exception innerException)
			{
				return Observable.Return(TrainerInstallationStatus.CreateError(package2, new GenericErrorException("Could not power down the avionics that depend on the package.", innerException)));
			}
			ITrainerPackage trainerPackage = await GetInstalledPackageForListing(gtiListing).DetachContext();
			if (trainerPackage != null && !package2.IsBugFixUpdateOf(trainerPackage))
			{
				Logger.Log(LogLevel.Info, "Found a matching package ({0}) already installed. No need to run GTI.", trainerPackage.TrainerPackageId);
				return Observable.Return(TrainerInstallationStatus.CreateReady(package2, trainerPackage));
			}
			string gtiNameForPackage = GetGtiNameForPackage(gtiListing, package2);
			return InstallGtiName(package2, gtiNameForPackage, trainerPackage, cancel);
		})).Merge<TrainerInstallationStatus>()))));
	}

	private GtiPackageOperation InsertUninstallPlan(ITrainerPackage package)
	{
		ITrainerPackage package2 = package;
		return new GtiPackageOperation(package2, TrainerPackageOperation.Uninstall, (CancellationToken cancel) => Observable.Defer<TrainerInstallationStatus>((Func<IObservable<TrainerInstallationStatus>>)(() => (!package2.BaseInfo.PackageType.IsCompleteOnDisk()) ? Observable.Return(TrainerInstallationStatus.CreateReady(package2, package2)) : ImmediateProgress(package2).Concat<TrainerInstallationStatus>(Observable.Timer(TimeSpan.FromSeconds(3.0)).IgnoreElements<long>().SelectDefault<long, TrainerInstallationStatus>(null)).Concat<TrainerInstallationStatus>(Observable.FromAsync<GtiListingEntry>((Func<System.Threading.Tasks.Task<GtiListingEntry>>)(() => GetMatchingListEntry(string.Empty, package2))).SelectMany<GtiListingEntry, IObservable<TrainerInstallationStatus>>((Func<GtiListingEntry, System.Threading.Tasks.Task<IObservable<TrainerInstallationStatus>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass33_1._003C_003CInsertUninstallPlan_003Eb__3_003Ed))] async (GtiListingEntry gtiListing) =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				await PowerDownAircraftIfPackageIsInUse(package2).DetachContext();
			}
			catch (System.Exception innerException)
			{
				return Observable.Return(TrainerInstallationStatus.CreateError(package2, new GenericErrorException("Could not power down the avionics that depend on the package.", innerException)));
			}
			System.IDisposable suppression = _gtiSuppressor.SuppressGtiPackageAtDirectory(package2.BaseInfo.PackageRootPath);
			try
			{
				bool wasInstalled = await _packageRepo.UnRegisterPackage(package2).DetachContext();
				if (!wasInstalled)
				{
					suppression.Dispose();
					Logger.Log(LogLevel.Info, "Could not find a matching package ({0}). It must have already been uninstalled. No need to run GTI.", package2.TrainerPackageId);
					return Observable.Return(TrainerInstallationStatus.CreateAcceptableResultMissing(package2));
				}
				await _unitOfWork.Avionics.RemoveAllForPackage(package2.TrainerPackageId).DetachContext();
				string gtiNameForPackage = GetGtiNameForPackage(gtiListing, package2);
				return UninstallGtiName(package2, gtiNameForPackage, cancel).SelectMany<TrainerInstallationStatus, TrainerInstallationStatus>((Func<TrainerInstallationStatus, System.Threading.Tasks.Task<TrainerInstallationStatus>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass33_3._003C_003CInsertUninstallPlan_003Eb__4_003Ed))] async (TrainerInstallationStatus status) =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					if (status.Error != null && wasInstalled)
					{
						try
						{
							await _packageRepo.RegisterPackage(package2).DetachContext();
						}
						catch (System.Exception ex)
						{
							return TrainerInstallationStatus.CreateError(package2, (System.Exception)new AggregateException("Could not re-register the package after failed uninstallation.", new System.Exception[2] { ex, status.Error }));
						}
					}
					return status;
				})).Finally<TrainerInstallationStatus>((Action)delegate
				{
					suppression.Dispose();
				});
			}
			catch
			{
				suppression.Dispose();
				throw;
			}
		})).Merge<TrainerInstallationStatus>()))));
	}

	[AsyncStateMachine(typeof(_003CPowerDownAircraftIfPackageIsInUse_003Ed__34))]
	private System.Threading.Tasks.Task PowerDownAircraftIfPackageIsInUse(ITrainerPackage package)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003CPowerDownAircraftIfPackageIsInUse_003Ed__34 _003CPowerDownAircraftIfPackageIsInUse_003Ed__ = default(_003CPowerDownAircraftIfPackageIsInUse_003Ed__34);
		_003CPowerDownAircraftIfPackageIsInUse_003Ed__._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
		_003CPowerDownAircraftIfPackageIsInUse_003Ed__._003C_003E4__this = this;
		_003CPowerDownAircraftIfPackageIsInUse_003Ed__.package = package;
		_003CPowerDownAircraftIfPackageIsInUse_003Ed__._003C_003E1__state = -1;
		((AsyncTaskMethodBuilder)(ref _003CPowerDownAircraftIfPackageIsInUse_003Ed__._003C_003Et__builder)).Start<_003CPowerDownAircraftIfPackageIsInUse_003Ed__34>(ref _003CPowerDownAircraftIfPackageIsInUse_003Ed__);
		return ((AsyncTaskMethodBuilder)(ref _003CPowerDownAircraftIfPackageIsInUse_003Ed__._003C_003Et__builder)).Task;
	}

	private IObservable<TrainerInstallationStatus> InstallGtiName(ITrainerPackage requestedPackage, string gtiName, ITrainerPackage? currentPackage, CancellationToken userCancel)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		string gtiName2 = gtiName;
		ITrainerPackage requestedPackage2 = requestedPackage;
		ITrainerPackage currentPackage2 = currentPackage;
		_003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals0;
		return _gtiProvider.GtiExeAsSingleValueInStream().SelectMany<ILauncherGtiRunner, TrainerInstallationStatus>((Func<ILauncherGtiRunner, IObservable<TrainerInstallationStatus>>)delegate(ILauncherGtiRunner gtiExe)
		{
			_003C_003Ec__DisplayClass35_1 _003C_003Ec__DisplayClass35_ = new _003C_003Ec__DisplayClass35_1();
			_003C_003Ec__DisplayClass35_.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals0;
			_003C_003Ec__DisplayClass35_.gtiExe = gtiExe;
			return Observable.Create<TrainerInstallationStatus>((Func<IObserver<TrainerInstallationStatus>, CancellationToken, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass35_1._003C_003CInstallGtiName_003Eb__3_003Ed))] (IObserver<TrainerInstallationStatus> o, CancellationToken unsubscribed) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass35_1._003C_003CInstallGtiName_003Eb__3_003Ed _003C_003CInstallGtiName_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass35_1._003C_003CInstallGtiName_003Eb__3_003Ed);
				_003C_003CInstallGtiName_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CInstallGtiName_003Eb__3_003Ed._003C_003E4__this = _003C_003Ec__DisplayClass35_;
				_003C_003CInstallGtiName_003Eb__3_003Ed.o = o;
				_003C_003CInstallGtiName_003Eb__3_003Ed.unsubscribed = unsubscribed;
				_003C_003CInstallGtiName_003Eb__3_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CInstallGtiName_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass35_1._003C_003CInstallGtiName_003Eb__3_003Ed>(ref _003C_003CInstallGtiName_003Eb__3_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CInstallGtiName_003Eb__3_003Ed._003C_003Et__builder)).Task;
			}));
		}).Concat<TrainerInstallationStatus>(Observable.FromAsync<TrainerInstallationStatus>((Func<System.Threading.Tasks.Task<TrainerInstallationStatus>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass35_0._003C_003CInstallGtiName_003Eb__1_003Ed))] async () =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			GtiListingEntry gtiListingEntry = await GetMatchingListEntry(gtiName2, requestedPackage2).DetachContext();
			if (gtiListingEntry == null)
			{
				return TrainerInstallationStatus.CreateError(requestedPackage2, (System.Exception)new KeyNotFoundException("GTI did not list the package after it installed it. This usually occurs when an updated version was discovered during the install/update operation. Please retry the install/update."));
			}
			string installedPath = gtiListingEntry.InstalledPath;
			if (installedPath == null)
			{
				return TrainerInstallationStatus.CreateError(requestedPackage2, (System.Exception)new KeyNotFoundException("GTI did not report the package installed after it just installed it. This usually occurs when an updated version was discovered during the install/update operation. Please retry the install/update."));
			}
			PotentialPackageDirectory potentialDir = PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.InstalledByGti, installedPath);
			ITrainerPackage matchingInstalledPackage = await GetRegisteredPackageOrNullIfError(potentialDir).DetachContext();
			if (matchingInstalledPackage == null)
			{
				try
				{
					ITrainerPackage newPackage = await _parser.ParsePackage(potentialDir).DetachContext();
					bool isUpdate = currentPackage2 != null && currentPackage2.BaseInfo.PackageType == TrainerPackageType.InstalledByGti && newPackage.IsSuccessorOf(currentPackage2);
					if (isUpdate)
					{
						System.Collections.Generic.IList<UpgradeDecisionLog> list = await _unitOfWork.PackageRepos.UpgradeDecisions.GetPriorUserUpgradeDecisions(requestedPackage2.TrainerPackageId.EmbeddedReleaseCompositeId.Yield()).DetachContext();
						PackageUpgradeVersions packageUpgradeVersions = new PackageUpgradeVersions(requestedPackage2.TrainerPackageId.EmbeddedReleaseCompositeId, new VersionRange(currentPackage2.TrainerPackageId.BinaryVersion, requestedPackage2.TrainerPackageId.BinaryVersion));
						if (((System.Collections.Generic.ICollection<UpgradeDecisionLog>)list).Count == 1 && list[0].GetDecision(packageUpgradeVersions) != UpgradeUserDecision.ApprovedUpgrade)
						{
							await _unitOfWork.PackageRepos.UpgradeDecisions.StoreUserUpgradeDecision(new PackageUpgradeVersions[1] { packageUpgradeVersions }, UpgradeUserDecision.ApprovedUpgrade).DetachContext();
						}
					}
					matchingInstalledPackage = await _packageRepo.RegisterPackage(newPackage).DetachContext();
					if (isUpdate)
					{
						await _packageRepo.UnRegisterPackage(currentPackage2).DetachContext();
					}
				}
				catch (System.Exception error2)
				{
					return TrainerInstallationStatus.CreateError(requestedPackage2, error2);
				}
			}
			return TrainerInstallationStatus.CreateReady(requestedPackage2, matchingInstalledPackage);
		})))
			.Catch<TrainerInstallationStatus, System.Exception>((System.Exception error) => EndWithErrorOrCanceled(requestedPackage2, error));
	}

	private IObservable<TrainerInstallationStatus> EndWithErrorOrCanceled(ITrainerPackage package, System.Exception error)
	{
		GtiProcessException ex = error as GtiProcessException;
		TrainerInstallationStatus value;
		if (ex != null && ex.ProcessResult.WasCanceled)
		{
			value = TrainerInstallationStatus.CreateCanceled(package);
		}
		else
		{
			System.Exception error2 = ((ex != null) ? ReformatGtiError(package, ex) : error);
			value = TrainerInstallationStatus.CreateError(package, error2);
		}
		return Observable.Return(value);
	}

	private IObservable<TrainerInstallationStatus> UninstallGtiName(ITrainerPackage package, string gtiName, CancellationToken userCancel)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		string gtiName2 = gtiName;
		ITrainerPackage package2 = package;
		_003C_003Ec__DisplayClass37_0 CS_0024_003C_003E8__locals0;
		return _gtiProvider.GtiExeAsSingleValueInStream().SelectMany<ILauncherGtiRunner, TrainerInstallationStatus>((Func<ILauncherGtiRunner, IObservable<TrainerInstallationStatus>>)delegate(ILauncherGtiRunner gtiExe)
		{
			_003C_003Ec__DisplayClass37_1 _003C_003Ec__DisplayClass37_ = new _003C_003Ec__DisplayClass37_1();
			_003C_003Ec__DisplayClass37_.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals0;
			_003C_003Ec__DisplayClass37_.gtiExe = gtiExe;
			return Observable.Create<TrainerInstallationStatus>((Func<IObserver<TrainerInstallationStatus>, CancellationToken, System.Threading.Tasks.Task>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass37_1._003C_003CUninstallGtiName_003Eb__3_003Ed))] (IObserver<TrainerInstallationStatus> o, CancellationToken unsubscribed) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				_003C_003Ec__DisplayClass37_1._003C_003CUninstallGtiName_003Eb__3_003Ed _003C_003CUninstallGtiName_003Eb__3_003Ed = default(_003C_003Ec__DisplayClass37_1._003C_003CUninstallGtiName_003Eb__3_003Ed);
				_003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				_003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003E4__this = _003C_003Ec__DisplayClass37_;
				_003C_003CUninstallGtiName_003Eb__3_003Ed.o = o;
				_003C_003CUninstallGtiName_003Eb__3_003Ed.unsubscribed = unsubscribed;
				_003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003E1__state = -1;
				((AsyncTaskMethodBuilder)(ref _003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass37_1._003C_003CUninstallGtiName_003Eb__3_003Ed>(ref _003C_003CUninstallGtiName_003Eb__3_003Ed);
				return ((AsyncTaskMethodBuilder)(ref _003C_003CUninstallGtiName_003Eb__3_003Ed._003C_003Et__builder)).Task;
			}));
		}).Concat<TrainerInstallationStatus>(Observable.FromAsync<TrainerInstallationStatus>((Func<System.Threading.Tasks.Task<TrainerInstallationStatus>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass37_0._003C_003CUninstallGtiName_003Eb__1_003Ed))] async () =>
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			GtiListingEntry gtiListingEntry = await GetMatchingListEntry(gtiName2, package2).DetachContext();
			if (gtiListingEntry == null || gtiListingEntry.IsDiscontinued)
			{
				return TrainerInstallationStatus.CreateAcceptableResultMissing(package2);
			}
			List<string> val = new List<string>();
			string catalogPath = gtiListingEntry.CatalogedPath;
			if (catalogPath == null)
			{
				val.Add("GTI did not report a cataloged path for the " + gtiListingEntry.EntryName + " entry.");
			}
			Version catalogedVersion = gtiListingEntry.CatalogedVersion;
			if (catalogedVersion == null)
			{
				val.Add("GTI did not report a cataloged version for the " + gtiListingEntry.EntryName + " entry.");
			}
			if (val.Count > 0 || catalogPath == null || catalogedVersion == null)
			{
				string text = string.Join(",\n\t", (System.Collections.Generic.IEnumerable<string>)val);
				Logger.Log(LogLevel.Error, "Catalog corruption detected:\n\t" + text + ".");
				return TrainerInstallationStatus.CreateError(package2, (System.Exception)new KeyNotFoundException("Catalog corruption detected. Please restart the Launcher."));
			}
			IRescuedCatalogPackage rescuedCatalogPackage = await _packageRefuge.GetRescuedPackageByGtiInfoOrNull(gtiListingEntry.EntryName, catalogedVersion).DetachContext();
			if (rescuedCatalogPackage != null)
			{
				ITrainerPackage trainerPackage = await GetRegisteredPackageOrNullIfError(rescuedCatalogPackage.RescuedDirectory).DetachContext();
				if (trainerPackage != null)
				{
					return TrainerInstallationStatus.CreateReady(package2, trainerPackage);
				}
				return TrainerInstallationStatus.CreateError(package2, (System.Exception)new KeyNotFoundException("The GTI package was copied to the safe location but not registered. Please report this to Garmin."));
			}
			PotentialPackageDirectory potentialDir = PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, catalogPath);
			ITrainerPackage trainerPackage2 = await GetRegisteredPackageOrNullIfError(potentialDir).DetachContext();
			if (trainerPackage2 == null)
			{
				try
				{
					ITrainerPackage package3 = await _parser.ParsePackage(potentialDir).DetachContext();
					trainerPackage2 = await _packageRepo.RegisterPackage(package3).DetachContext();
				}
				catch (System.Exception error2)
				{
					return TrainerInstallationStatus.CreateError(package2, error2);
				}
			}
			return TrainerInstallationStatus.CreateReady(package2, trainerPackage2);
		})))
			.Catch<TrainerInstallationStatus, System.Exception>((System.Exception error) => EndWithErrorOrCanceled(package2, error));
	}

	[AsyncStateMachine(typeof(_003CGetInstalledPackageForListing_003Ed__38))]
	private async System.Threading.Tasks.Task<ITrainerPackage?> GetInstalledPackageForListing(GtiListingEntry installedMatchingListEntry)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (installedMatchingListEntry.InstalledPath == null)
		{
			return null;
		}
		return await GetRegisteredPackageOrNullIfError(PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.InstalledByGti, installedMatchingListEntry.InstalledPath)).DetachContext();
	}

	[AsyncStateMachine(typeof(_003CGetRegisteredPackageOrNullIfError_003Ed__39))]
	private async System.Threading.Tasks.Task<ITrainerPackage?> GetRegisteredPackageOrNullIfError(PotentialPackageDirectory packageDirectory)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!packageDirectory.HasPotential)
		{
			return null;
		}
		try
		{
			return await _packageRepo.GetPackageByLocationOrNull(packageDirectory.PackageDirectory.FullPath).DetachContext();
		}
		catch (KeyNotFoundException)
		{
			return null;
		}
	}

	private System.Exception ReformatGtiError(ITrainerPackage package, GtiProcessException gtiException)
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		Logger.LogFormattedException(gtiException, "[GTI] Package: {0}", package.BaseInfo.TrainerPackageId);
		GtiProcessResult processResult = gtiException.ProcessResult;
		System.Exception ex = ((((System.Collections.Generic.IReadOnlyCollection<string>)processResult.StandardErrorMessages).Count <= 0) ? new GenericErrorException("No Installer error output. It must have been forcefully closed.") : new GenericErrorException("Installer error output:" + Environment.NewLine + string.Join(Environment.NewLine, (System.Collections.Generic.IEnumerable<string>)processResult.StandardErrorMessages)));
		if (processResult.IsGarminServiceIssue)
		{
			return new GenericErrorException("Garmin service error. Garmin's servers are not working as expected. Please contact support to report this issue, and try again later.", ex);
		}
		if (processResult.IsInternetIssue)
		{
			if (!_networkDetector.IsNetworkAvailable)
			{
				return new GenericErrorException("An Internet connection is required to download modules. Please connect to the Internet and retry.");
			}
			return new GenericErrorException("Internet related error. Please check your Internet connection, firewall, and proxy settings and then retry.", ex);
		}
		if (processResult.IsDiskSpaceIssue)
		{
			return new GenericErrorException("Drive space is low. Please free up space on the " + Path.GetPathRoot(gtiException.ProcessResult.GtiRootDirectory) + " drive and try again.");
		}
		if (processResult.ExitCode == 1)
		{
			return new GenericErrorException("Generic Installer Error", ex);
		}
		return (System.Exception)new AggregateException(new System.Exception[2]
		{
			new GenericErrorException("Unrecognized installer error code: " + ((object)gtiException.ProcessResult).ToString()),
			ex
		});
	}

	[AsyncStateMachine(typeof(_003CGetMatchingListEntry_003Ed__41))]
	private async System.Threading.Tasks.Task<GtiListingEntry> GetMatchingListEntry(string gtiName, ITrainerPackage package)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		string gtiName2 = gtiName;
		ITrainerPackage package2 = package;
		Func<GtiListingEntry, bool> predicate = await CreateListEntryMatcher().DetachContext();
		return await (await _gtiProvider.GtiExe.DetachContext()).List().GetListEntries().Catch<GtiListingEntry, GtiProcessException>((GtiProcessException e) => Observable.Throw<GtiListingEntry>(new GenericErrorException("Catalog installation failure detected. Please restart the Launcher.", e)))
			.FirstOrDefaultAsync<GtiListingEntry>((GtiListingEntry le) => predicate.Invoke(le));
		[AsyncStateMachine(typeof(_003C_003Ec__DisplayClass41_0._003C_003CGetMatchingListEntry_003Eg__CreateListEntryMatcher_007C0_003Ed))]
		async System.Threading.Tasks.Task<Func<GtiListingEntry, bool>> CreateListEntryMatcher()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(gtiName2))
			{
				gtiName2 = (await _packageRefuge.GetRescuedPackageByPathOrNull(package2.BaseInfo.PackageRootPath).DetachContext())?.GtiName ?? string.Empty;
			}
			Func<GtiListingEntry, bool> val = (GtiListingEntry listEntry) => ((EqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash).Equals(listEntry.CatalogedPath, package2.BaseInfo.PackageRootPath) || ((EqualityComparer<string>)(object)PathHelpers.AnyCaseAnySlash).Equals(listEntry.InstalledPath, package2.BaseInfo.PackageRootPath);
			if (gtiName2.Length > 0)
			{
				Func<GtiListingEntry, bool> baseClosure = val;
				val = (GtiListingEntry listEntry) => listEntry.EntryName == gtiName2 || baseClosure.Invoke(listEntry);
			}
			return val;
		}
	}

	private System.Threading.Tasks.Task WaitForInstallStatusThenSendEvent(ITrainerPackage package, ITrainerPackageInstallationSession installationSession, Func<ITrainerPackage, Option<AppCenterAppId>> getAppId)
	{
		ITrainerPackage package2 = package;
		Func<ITrainerPackage, Option<AppCenterAppId>> getAppId2 = getAppId;
		TrainerPackageOperation operation = installationSession.RequestedOperation;
		switch (operation)
		{
		case TrainerPackageOperation.Install:
		case TrainerPackageOperation.Uninstall:
			return installationSession.StatusUpdates.Where<TrainerInstallationStatus>((Func<TrainerInstallationStatus, bool>)((TrainerInstallationStatus status) => status.OnDemandState != TrainerPackageOperationState.InProgress)).Select<TrainerInstallationStatus, Unit>((Func<TrainerInstallationStatus, Unit>)((TrainerInstallationStatus status) => Prelude.Seq<ITrainerPackage>(package2, status.ResultingPackage).WhereClassNotNull().Find((ITrainerPackage p) => p.BaseInfo.PackageType == TrainerPackageType.InstalledByGti)
				.Bind<AppCenterAppId>((ITrainerPackage fullPackageOnDisk) => getAppId2.Invoke(fullPackageOnDisk))
				.IfSome((Action<AppCenterAppId>)delegate(AppCenterAppId appId)
				{
					_sendLog(new TrainerInstallOrUninstallDataDescriptor(new CachedCustomAppIdTargetApp(appId, package2.BaseInfo.BinaryVersion, ((object)package2.BaseInfo.BinaryVersion).ToString(), ((object)package2.BaseInfo.FamilyId).ToString()), status.OnDemandState, ((object)operation).ToString()));
				}))).Take<Unit>(1)
				.ToTask<Unit>();
		case TrainerPackageOperation.None:
			return System.Threading.Tasks.Task.CompletedTask;
		default:
		{
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(operation);
			System.Threading.Tasks.Task result = default(System.Threading.Tasks.Task);
			return result;
		}
		}
	}
}
