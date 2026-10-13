using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Models.TrainerPackages;
using LauncherCoreLib.Utility;
using Nito.AsyncEx;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Settings;

public sealed class SettingsRepo : ISettingsRepo, System.IDisposable
{
	private sealed class SettingTreeBinder : System.IDisposable
	{
		private readonly System.IDisposable _cleanup;

		private readonly Dictionary<SettingIdentifier, SettingTreeBinder> _children = new Dictionary<SettingIdentifier, SettingTreeBinder>();

		[field: CompilerGenerated]
		public IModifiableSettingSymbolTable CurrentTable
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public bool Disposed
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		[field: CompilerGenerated]
		public SettingIdentifier FullPath
		{
			[CompilerGenerated]
			get;
		}

		[field: CompilerGenerated]
		public object? Value
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		private SettingTreeBinder(IModifiableSettingSymbolTable parent, SettingIdentifier fullName)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			IModifiableSettingSymbolTable parent2 = parent;
			base._002Ector();
			SettingTreeBinder settingTreeBinder = this;
			FullPath = fullName;
			SettingIdentifier localName = fullName.LocalId;
			CurrentTable = ((fullName == SettingIdentifier.Empty) ? parent2 : parent2.CreateNestedTable(localName));
			_cleanup = Disposable.Create((Action)delegate
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				parent2.RemoveNestedTable(localName);
				Enumerator<SettingIdentifier, SettingTreeBinder> enumerator = settingTreeBinder._children.Values.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.Dispose();
					}
				}
				finally
				{
					((System.IDisposable)enumerator).Dispose();
				}
				settingTreeBinder._children.Clear();
			});
		}

		public static SettingTreeBinder CreateRoot(IModifiableSettingSymbolTable rootSettings, SettingIdentifier rootIdentifier)
		{
			return new SettingTreeBinder(rootSettings, rootIdentifier);
		}

		public SettingTreeBinder Nest(SettingIdentifier nestedName)
		{
			SettingTreeBinder settingTreeBinder = default(SettingTreeBinder);
			if (_children.TryGetValue(nestedName, ref settingTreeBinder) && !settingTreeBinder.Disposed)
			{
				return settingTreeBinder;
			}
			SettingTreeBinder settingTreeBinder2 = new SettingTreeBinder(CurrentTable, FullPath + nestedName);
			_children[nestedName] = settingTreeBinder2;
			return settingTreeBinder2;
		}

		public List<SettingTreeBinder> BindNodesForIdentifier(SettingIdentifier localScope)
		{
			List<SettingTreeBinder> obj = new List<SettingTreeBinder>();
			obj.Add(this);
			List<SettingTreeBinder> val = obj;
			BlazeNodePath(Enumerable.ToList<SettingIdentifier>(localScope.EnumerateParts()), 0, val);
			return val;
		}

		public void Dispose()
		{
			if (!Disposed)
			{
				Disposed = true;
				_cleanup.Dispose();
				if (Value is System.IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}

		public string ToString()
		{
			return FullPath;
		}

		private void BlazeNodePath(List<SettingIdentifier> identParts, int depth, List<SettingTreeBinder> visited)
		{
			if (identParts.Count > depth)
			{
				SettingIdentifier nestedName = identParts[depth];
				SettingTreeBinder settingTreeBinder = Nest(nestedName);
				visited.Add(settingTreeBinder);
				settingTreeBinder.BlazeNodePath(identParts, depth + 1, visited);
			}
		}
	}

	private sealed class BinderNodeCounter : System.IDisposable
	{
		public interface IBoundPath : System.IDisposable
		{
			SettingIdentifier FullIdentifier { get; }

			IModifiableSettingSymbolTable Table { get; }

			T GetOrBindPathResource<T>(Func<IModifiableSettingSymbolTable, T> factory);

			IBoundPath BindRelativeIdent(SettingIdentifier relativeIdentifier);
		}

		private sealed class PathBinder : IBoundPath, System.IDisposable
		{
			private readonly BinderNodeCounter _counter;

			private readonly IImmutableList<SettingTreeBinder> _nodes;

			private bool _disposed;

			public SettingIdentifier FullIdentifier => PrimaryBinder.FullPath;

			public IModifiableSettingSymbolTable Table => PrimaryBinder.CurrentTable;

			private SettingTreeBinder PrimaryBinder => ((System.Collections.Generic.IReadOnlyList<SettingTreeBinder>)_nodes)[((System.Collections.Generic.IReadOnlyCollection<SettingTreeBinder>)_nodes).Count - 1];

			public PathBinder(BinderNodeCounter counter, IImmutableList<SettingTreeBinder> nodes)
			{
				_counter = counter;
				_nodes = nodes;
			}

			public void Dispose()
			{
				if (!_disposed)
				{
					_disposed = true;
					_counter.DecrementNodeReferences((System.Collections.Generic.IEnumerable<SettingTreeBinder>)_nodes);
				}
			}

			public IBoundPath BindRelativeIdent(SettingIdentifier relativeIdentifier)
			{
				return _counter.BindPath(FullIdentifier + relativeIdentifier);
			}

			public T GetOrBindPathResource<T>(Func<IModifiableSettingSymbolTable, T> factory)
			{
				if (PrimaryBinder.Value != null)
				{
					return (T)PrimaryBinder.Value;
				}
				T val = factory.Invoke(Table);
				PrimaryBinder.Value = val;
				return val;
			}
		}

		private readonly SettingTreeBinder _root;

		private readonly IBoundPath _keepRootBound;

		private readonly Dictionary<SettingTreeBinder, int> _referenceCounts = new Dictionary<SettingTreeBinder, int>();

		public BinderNodeCounter(SettingTreeBinder root)
		{
			_root = root;
			_keepRootBound = BindPath(SettingIdentifier.Empty);
		}

		public IBoundPath BindPath(SettingIdentifier relativeIdentifier)
		{
			List<SettingTreeBinder> val = _root.BindNodesForIdentifier(relativeIdentifier);
			IncrementNodeReferences((System.Collections.Generic.IEnumerable<SettingTreeBinder>)val);
			return new PathBinder(this, (IImmutableList<SettingTreeBinder>)(object)ImmutableList.ToImmutableList<SettingTreeBinder>((System.Collections.Generic.IEnumerable<SettingTreeBinder>)val));
		}

		public void Dispose()
		{
			((System.IDisposable)_keepRootBound).Dispose();
		}

		private void IncrementNodeReferences(System.Collections.Generic.IEnumerable<SettingTreeBinder> nodes)
		{
			System.Collections.Generic.IEnumerator<SettingTreeBinder> enumerator = nodes.GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					SettingTreeBinder current = enumerator.Current;
					if (!_referenceCounts.ContainsKey(current))
					{
						_referenceCounts[current] = 1;
						continue;
					}
					Dictionary<SettingTreeBinder, int> referenceCounts = _referenceCounts;
					SettingTreeBinder settingTreeBinder = current;
					int num = referenceCounts[settingTreeBinder];
					referenceCounts[settingTreeBinder] = num + 1;
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}

		private void DecrementNodeReferences(System.Collections.Generic.IEnumerable<SettingTreeBinder> nodes)
		{
			System.Collections.Generic.IEnumerator<SettingTreeBinder> enumerator = nodes.GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					SettingTreeBinder current = enumerator.Current;
					Dictionary<SettingTreeBinder, int> referenceCounts = _referenceCounts;
					SettingTreeBinder settingTreeBinder = current;
					int num = referenceCounts[settingTreeBinder];
					referenceCounts[settingTreeBinder] = num - 1;
					if (_referenceCounts[current] <= 0)
					{
						current.Dispose();
						_referenceCounts.Remove(current);
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
	}

	private sealed class SettingUpdateBinder<T>
	{
		public T Key
		{
			[CompilerGenerated]
			get;
		}

		public System.Exception? Error
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		public System.Threading.Tasks.Task FirstUpdateOccurred
		{
			[CompilerGenerated]
			get;
		}

		public SettingUpdateBinder(T key, IObservable<TimingEvent> updateStream)
		{
			Key = key;
			FirstUpdateOccurred = updateStream.FirstAsync<TimingEvent>().CatchDoRethrow<TimingEvent, System.Exception>([CompilerGenerated] (System.Exception error) =>
			{
				Error = error;
			}).ToTask<TimingEvent>();
		}
	}

	private sealed class SimSettingBinder : CleanupBinder<SimAircraftKey>
	{
		public SimSettingBinder(SimAircraftKey simAircraftKey, ISettingSymbolTable table, System.IDisposable cleanup)
			: base(simAircraftKey, table, cleanup)
		{
		}
	}

	private sealed class FamilyBinder : CleanupBinder<AircraftPackageFamily>
	{
		public FamilyBinder(AircraftPackageFamily entity, ISettingSymbolTable table, System.IDisposable cleanup)
			: base(entity, table, cleanup)
		{
		}
	}

	private class CleanupBinder<T> : ISettingsBinder<T>, System.IDisposable
	{
		private readonly System.IDisposable _cleanup;

		private bool _disposedValue;

		public ISettingSymbolTable Table
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		public T Entity
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			private set;
		}

		public CleanupBinder(T entity, ISettingSymbolTable table, System.IDisposable cleanup)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			System.IDisposable cleanup2 = cleanup;
			base._002Ector();
			CleanupBinder<T> cleanupBinder = this;
			Entity = entity;
			Table = table;
			_cleanup = Disposable.Create((Action)delegate
			{
				cleanupBinder.Table = null;
				cleanup2.Dispose();
			});
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
					_cleanup.Dispose();
				}
				_disposedValue = true;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass10_0
	{
		[StructLayout(3)]
		private struct _003C_003CBindFamilySettings_003Eb__0_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass10_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private void MoveNext()
			{
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
				_003C_003Ec__DisplayClass10_0 _003C_003Ec__DisplayClass10_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass10_._003C_003E4__this._lock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CBindFamilySettings_003Eb__0_003Ed>(ref awaiter, ref this);
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
						((System.IDisposable)_003C_003Ec__DisplayClass10_.familyNode).Dispose();
						((System.IDisposable)_003C_003Ec__DisplayClass10_.aircraftNode).Dispose();
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

		public SettingsRepo _003C_003E4__this;

		public BinderNodeCounter.IBoundPath familyNode;

		public BinderNodeCounter.IBoundPath aircraftNode;

		[AsyncStateMachine(typeof(_003C_003CBindFamilySettings_003Eb__0_003Ed))]
		internal void _003CBindFamilySettings_003Eb__0()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CBindFamilySettings_003Eb__0_003Ed _003C_003CBindFamilySettings_003Eb__0_003Ed = default(_003C_003CBindFamilySettings_003Eb__0_003Ed);
			_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
			_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003E4__this = this;
			_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003E1__state = -1;
			((AsyncVoidMethodBuilder)(ref _003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003CBindFamilySettings_003Eb__0_003Ed>(ref _003C_003CBindFamilySettings_003Eb__0_003Ed);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_0
	{
		[StructLayout(3)]
		private struct _003C_003CBindSimulator_003Eb__4_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<ValueTuple<SimInstallData, ITrainerPackage>> _003C_003Et__builder;

			public SimInstallData simData;

			public _003C_003Ec__DisplayClass11_0 _003C_003E4__this;

			private SimInstallData _003C_003E7__wrap1;

			private ConfiguredTaskAwaiter<ITrainerPackage> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass11_0 _003C_003Ec__DisplayClass11_ = _003C_003E4__this;
				ValueTuple<SimInstallData, ITrainerPackage> result2;
				try
				{
					ConfiguredTaskAwaiter<ITrainerPackage> awaiter;
					if (num != 0)
					{
						_003C_003E7__wrap1 = simData;
						awaiter = _003C_003Ec__DisplayClass11_._003C_003E4__this._packageRepo.GetPackageById(simData.FullPackageIdOrThrow()).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ITrainerPackage>, _003C_003CBindSimulator_003Eb__4_003Ed>(ref awaiter, ref this);
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
					result2 = new ValueTuple<SimInstallData, ITrainerPackage>(_003C_003E7__wrap1, result);
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

		public SettingsRepo _003C_003E4__this;

		public SimAircraftKey simAircraftKey;

		public Func<DataChangeNotification<SimInstallKey>, bool> _003C_003E9__2;

		public Func<TimingEvent, System.Threading.Tasks.Task<SimInstallData>> _003C_003E9__3;

		public Func<SimInstallData, System.Threading.Tasks.Task<ValueTuple<SimInstallData, ITrainerPackage>>> _003C_003E9__4;

		internal bool _003CBindSimulator_003Eb__2(DataChangeNotification<SimInstallKey> sim)
		{
			return sim.Value == simAircraftKey.InstallationKey;
		}

		internal System.Threading.Tasks.Task<SimInstallData> _003CBindSimulator_003Eb__3(TimingEvent _)
		{
			return _003C_003E4__this._simRepo.GetSimulator(simAircraftKey.AircraftIdentifier, simAircraftKey.InstallationKey);
		}

		[AsyncStateMachine(typeof(_003C_003CBindSimulator_003Eb__4_003Ed))]
		internal async System.Threading.Tasks.Task<ValueTuple<SimInstallData, ITrainerPackage>> _003CBindSimulator_003Eb__4(SimInstallData simData)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return new ValueTuple<SimInstallData, ITrainerPackage>(simData, await _003C_003E4__this._packageRepo.GetPackageById(simData.FullPackageIdOrThrow()).DetachContext());
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_1
	{
		[StructLayout(3)]
		private struct _003C_003CBindSimulator_003Eb__1_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncVoidMethodBuilder _003C_003Et__builder;

			public _003C_003Ec__DisplayClass11_1 _003C_003E4__this;

			private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0090: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass11_1 _003C_003Ec__DisplayClass11_ = _003C_003E4__this;
				try
				{
					ConfiguredTaskAwaiter<System.IDisposable> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass11_.CS_0024_003C_003E8__locals1._003C_003E4__this._lock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							((AsyncVoidMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003C_003CBindSimulator_003Eb__1_003Ed>(ref awaiter, ref this);
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
						Enumerator<System.IDisposable> enumerator = _003C_003Ec__DisplayClass11_.nodes.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								enumerator.Current.Dispose();
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

		public List<System.IDisposable> nodes;

		public _003C_003Ec__DisplayClass11_0 CS_0024_003C_003E8__locals1;

		[AsyncStateMachine(typeof(_003C_003CBindSimulator_003Eb__1_003Ed))]
		internal void _003CBindSimulator_003Eb__1()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_003C_003CBindSimulator_003Eb__1_003Ed _003C_003CBindSimulator_003Eb__1_003Ed = default(_003C_003CBindSimulator_003Eb__1_003Ed);
			_003C_003CBindSimulator_003Eb__1_003Ed._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
			_003C_003CBindSimulator_003Eb__1_003Ed._003C_003E4__this = this;
			_003C_003CBindSimulator_003Eb__1_003Ed._003C_003E1__state = -1;
			((AsyncVoidMethodBuilder)(ref _003C_003CBindSimulator_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003CBindSimulator_003Eb__1_003Ed>(ref _003C_003CBindSimulator_003Eb__1_003Ed);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_2
	{
		public BinderNodeCounter.IBoundPath simNode;

		public _003C_003Ec__DisplayClass11_1 CS_0024_003C_003E8__locals2;

		internal SettingUpdateBinder<SimAircraftKey> _003CBindSimulator_003Eb__0(IModifiableSettingSymbolTable slotTable)
		{
			_003C_003Ec__DisplayClass11_3 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass11_3
			{
				CS_0024_003C_003E8__locals3 = this,
				slotTable = slotTable
			};
			IObservable<TimingEvent> updateStream = TimingEvent.StartStreamAndConcat<SimInstallKey>(CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._simRepo.WhenSimulatorIsChangedOnAircraft(CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.AircraftIdentifier).Where<DataChangeNotification<SimInstallKey>>((Func<DataChangeNotification<SimInstallKey>, bool>)((DataChangeNotification<SimInstallKey> sim) => sim.Value == CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.InstallationKey)).OnCreateOrUpdate<SimInstallKey>()).Select<TimingEvent, System.Threading.Tasks.Task<SimInstallData>>((Func<TimingEvent, System.Threading.Tasks.Task<SimInstallData>>)((TimingEvent _) => CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._simRepo.GetSimulator(CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.AircraftIdentifier, CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.InstallationKey))).Switch<SimInstallData>()
				.Select<SimInstallData, System.Threading.Tasks.Task<ValueTuple<SimInstallData, ITrainerPackage>>>((Func<SimInstallData, System.Threading.Tasks.Task<ValueTuple<SimInstallData, ITrainerPackage>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass11_0._003C_003CBindSimulator_003Eb__4_003Ed))] async (SimInstallData simData) => new ValueTuple<SimInstallData, ITrainerPackage>(simData, await CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._packageRepo.GetPackageById(simData.FullPackageIdOrThrow()).DetachContext())))
				.Switch<ValueTuple<SimInstallData, ITrainerPackage>>()
				.ToCurrentAndMaybePrevious<ValueTuple<SimInstallData, ITrainerPackage>>()
				.Do<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>((Action<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>)delegate(CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>> t)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0047: Unknown result type (might be due to invalid IL or missing references)
					//IL_005e: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0029: Unknown result type (might be due to invalid IL or missing references)
					//IL_007a: Unknown result type (might be due to invalid IL or missing references)
					ThrowIfPackageIsNull(t.Current.Item2);
					if (!t.HasPrevious || t.Previous.Item2 != t.Current.Item2)
					{
						SetupFirstClassCitizenSettings(CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals3.simNode.Table, t.Current.Item1);
						AddOrUpdateTableWithDefinitions(CS_0024_003C_003E8__locals0.slotTable, t.Current.Item2.GetSlotSettingDefinitions());
					}
					CS_0024_003C_003E8__locals0.slotTable.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)t.Current.Item1.Settings);
				})
				.ToTimingEventStream<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>();
			return new SettingUpdateBinder<SimAircraftKey>(CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey, updateStream);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_3
	{
		public IModifiableSettingSymbolTable slotTable;

		public _003C_003Ec__DisplayClass11_2 CS_0024_003C_003E8__locals3;

		internal void _003CBindSimulator_003Eb__5(CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>> t)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			ThrowIfPackageIsNull(t.Current.Item2);
			if (!t.HasPrevious || t.Previous.Item2 != t.Current.Item2)
			{
				SetupFirstClassCitizenSettings(CS_0024_003C_003E8__locals3.simNode.Table, t.Current.Item1);
				AddOrUpdateTableWithDefinitions(slotTable, t.Current.Item2.GetSlotSettingDefinitions());
			}
			slotTable.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)t.Current.Item1.Settings);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass16_0
	{
		[StructLayout(3)]
		private struct _003C_003CBindAircraftAndFamilyWithLock_003Eb__6_003Ed : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<FamilyData> _003C_003Et__builder;

			public _003C_003Ec__DisplayClass16_0 _003C_003E4__this;

			private ConfiguredTaskAwaiter<FamilyData> _003C_003Eu__1;

			private void MoveNext()
			{
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				int num = _003C_003E1__state;
				_003C_003Ec__DisplayClass16_0 _003C_003Ec__DisplayClass16_ = _003C_003E4__this;
				FamilyData result;
				try
				{
					ConfiguredTaskAwaiter<FamilyData> awaiter;
					if (num != 0)
					{
						awaiter = _003C_003Ec__DisplayClass16_._003C_003E4__this._familyDataRepo.GetFamilyDataForReleaseOrDefault(_003C_003Ec__DisplayClass16_.aircraftPackageFamily).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<FamilyData>, _003C_003CBindAircraftAndFamilyWithLock_003Eb__6_003Ed>(ref awaiter, ref this);
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

		public SettingsRepo _003C_003E4__this;

		public AircraftPackageFamily aircraftPackageFamily;

		public Func<System.Threading.Tasks.Task<AircraftListData>> _003C_003E9__1;

		public Func<AircraftIdentifier, System.Threading.Tasks.Task<AircraftListData>> _003C_003E9__2;

		public Func<AircraftPackageFamily, bool> _003C_003E9__5;

		public Func<TimingEvent, System.Threading.Tasks.Task<FamilyData>> _003C_003E9__6;

		internal SettingUpdateBinder<AircraftIdentifier> _003CBindAircraftAndFamilyWithLock_003Eb__0(IModifiableSettingSymbolTable aircraftTable)
		{
			_003C_003Ec__DisplayClass16_1 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass16_1
			{
				aircraftTable = aircraftTable
			};
			IObservable<TimingEvent> updateStream = Observable.FromAsync<AircraftListData>((Func<System.Threading.Tasks.Task<AircraftListData>>)(() => _003C_003E4__this._aircraftRepo.GetAircraftListDataById(aircraftPackageFamily.AircraftIdentifier))).Concat<AircraftListData>(_003C_003E4__this._aircraftRepo.AircraftChanges.ForAircraft(aircraftPackageFamily.AircraftIdentifier).StripChangeType<AircraftIdentifier>().Select<AircraftIdentifier, System.Threading.Tasks.Task<AircraftListData>>((Func<AircraftIdentifier, System.Threading.Tasks.Task<AircraftListData>>)((AircraftIdentifier ai) => _003C_003E4__this._aircraftRepo.GetAircraftListDataById(ai)))
				.Switch<AircraftListData>()).Do<AircraftListData>((Action<AircraftListData>)delegate(AircraftListData listData)
			{
				AircraftSettingUtils.SetupAirframeSetting(CS_0024_003C_003E8__locals0.aircraftTable, listData.AirframeCCT);
			})
				.ToTimingEventStream<AircraftListData>();
			return new SettingUpdateBinder<AircraftIdentifier>(aircraftPackageFamily.AircraftIdentifier, updateStream);
		}

		internal System.Threading.Tasks.Task<AircraftListData> _003CBindAircraftAndFamilyWithLock_003Eb__1()
		{
			return _003C_003E4__this._aircraftRepo.GetAircraftListDataById(aircraftPackageFamily.AircraftIdentifier);
		}

		internal System.Threading.Tasks.Task<AircraftListData> _003CBindAircraftAndFamilyWithLock_003Eb__2(AircraftIdentifier ai)
		{
			return _003C_003E4__this._aircraftRepo.GetAircraftListDataById(ai);
		}

		internal SettingUpdateBinder<AircraftPackageFamily> _003CBindAircraftAndFamilyWithLock_003Eb__4(IModifiableSettingSymbolTable familyTable)
		{
			_003C_003Ec__DisplayClass16_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass16_2
			{
				familyTable = familyTable
			};
			IObservable<TimingEvent> updateStream = Observable.CombineLatest<ITrainerPackage, FamilyData, ValueTuple<ITrainerPackage, FamilyData>>(_003C_003E4__this._packageRepo.GetPackageLineage(aircraftPackageFamily.ReleaseId), TimingEvent.StartStreamAndConcat<AircraftPackageFamily>(_003C_003E4__this._familyDataRepo.WhenAircraftFamilyDataChanges.OnCreateOrUpdate<AircraftPackageFamily>().Where<AircraftPackageFamily>((Func<AircraftPackageFamily, bool>)((AircraftPackageFamily apf) => apf == aircraftPackageFamily))).Select<TimingEvent, System.Threading.Tasks.Task<FamilyData>>((Func<TimingEvent, System.Threading.Tasks.Task<FamilyData>>)([AsyncStateMachine(typeof(_003C_003CBindAircraftAndFamilyWithLock_003Eb__6_003Ed))] async (TimingEvent _) => await _003C_003E4__this._familyDataRepo.GetFamilyDataForReleaseOrDefault(aircraftPackageFamily).DetachContext())).Switch<FamilyData>(), (ITrainerPackage package, FamilyData familyData) => new ValueTuple<ITrainerPackage, FamilyData>(package, familyData)).ToCurrentAndMaybePrevious<ValueTuple<ITrainerPackage, FamilyData>>().Do<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>((Action<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>)delegate(CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>> t)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				ITrainerPackage trainerPackage = ThrowIfPackageIsNull(t.Current.Item1);
				if (!t.HasPrevious || trainerPackage != t.Previous.Item1)
				{
					AddOrUpdateTableWithDefinitions(CS_0024_003C_003E8__locals0.familyTable, trainerPackage.GetScopedSettingDefinitions()[ManifestSettingScope.Family]);
				}
				CS_0024_003C_003E8__locals0.familyTable.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)t.Current.Item2.Settings);
			})
				.ToTimingEventStream<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>();
			return new SettingUpdateBinder<AircraftPackageFamily>(aircraftPackageFamily, updateStream);
		}

		internal bool _003CBindAircraftAndFamilyWithLock_003Eb__5(AircraftPackageFamily apf)
		{
			return apf == aircraftPackageFamily;
		}

		[AsyncStateMachine(typeof(_003C_003CBindAircraftAndFamilyWithLock_003Eb__6_003Ed))]
		internal async System.Threading.Tasks.Task<FamilyData> _003CBindAircraftAndFamilyWithLock_003Eb__6(TimingEvent _)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return await _003C_003E4__this._familyDataRepo.GetFamilyDataForReleaseOrDefault(aircraftPackageFamily).DetachContext();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass16_1
	{
		public IModifiableSettingSymbolTable aircraftTable;

		internal void _003CBindAircraftAndFamilyWithLock_003Eb__3(AircraftListData listData)
		{
			AircraftSettingUtils.SetupAirframeSetting(aircraftTable, listData.AirframeCCT);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass16_2
	{
		public IModifiableSettingSymbolTable familyTable;

		internal void _003CBindAircraftAndFamilyWithLock_003Eb__8(CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>> t)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			ITrainerPackage trainerPackage = ThrowIfPackageIsNull(t.Current.Item1);
			if (!t.HasPrevious || trainerPackage != t.Previous.Item1)
			{
				AddOrUpdateTableWithDefinitions(familyTable, trainerPackage.GetScopedSettingDefinitions()[ManifestSettingScope.Family]);
			}
			familyTable.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)t.Current.Item2.Settings);
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CBindAircraftAndFamilyWithLock_003Ed__16 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>> _003C_003Et__builder;

		public SettingsRepo _003C_003E4__this;

		public AircraftPackageFamily aircraftPackageFamily;

		private _003C_003Ec__DisplayClass16_0 _003C_003E8__1;

		private BinderNodeCounter.IBoundPath _003CaircraftNode_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private BinderNodeCounter.IBoundPath _003CfamilyNode_003E5__3;

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
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SettingsRepo settingsRepo = _003C_003E4__this;
			ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath> result;
			try
			{
				ConfiguredTaskAwaitable val;
				ConfiguredTaskAwaiter awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00ee;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass16_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.aircraftPackageFamily = aircraftPackageFamily;
					_003CaircraftNode_003E5__2 = settingsRepo._nodeCounter.BindPath(_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier);
					val = _003CaircraftNode_003E5__2.GetOrBindPathResource<SettingUpdateBinder<AircraftIdentifier>>(delegate(IModifiableSettingSymbolTable aircraftTable)
					{
						_003C_003Ec__DisplayClass16_1 CS_0024_003C_003E8__locals1 = new _003C_003Ec__DisplayClass16_1
						{
							aircraftTable = aircraftTable
						};
						IObservable<TimingEvent> updateStream2 = Observable.FromAsync<AircraftListData>((Func<System.Threading.Tasks.Task<AircraftListData>>)(() => _003C_003E8__1._003C_003E4__this._aircraftRepo.GetAircraftListDataById(_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier))).Concat<AircraftListData>(_003C_003E8__1._003C_003E4__this._aircraftRepo.AircraftChanges.ForAircraft(_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier).StripChangeType<AircraftIdentifier>().Select<AircraftIdentifier, System.Threading.Tasks.Task<AircraftListData>>((Func<AircraftIdentifier, System.Threading.Tasks.Task<AircraftListData>>)((AircraftIdentifier ai) => _003C_003E8__1._003C_003E4__this._aircraftRepo.GetAircraftListDataById(ai)))
							.Switch<AircraftListData>()).Do<AircraftListData>((Action<AircraftListData>)delegate(AircraftListData listData)
						{
							AircraftSettingUtils.SetupAirframeSetting(CS_0024_003C_003E8__locals1.aircraftTable, listData.AirframeCCT);
						})
							.ToTimingEventStream<AircraftListData>();
						return new SettingUpdateBinder<AircraftIdentifier>(_003C_003E8__1.aircraftPackageFamily.AircraftIdentifier, updateStream2);
					}).FirstUpdateOccurred.DetachContext();
					awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
					if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CBindAircraftAndFamilyWithLock_003Ed__16>(ref awaiter, ref this);
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
				goto IL_00ee;
				IL_00ee:
				try
				{
					if (num != 1)
					{
						_003CfamilyNode_003E5__3 = _003CaircraftNode_003E5__2.BindRelativeIdent(((object)_003C_003E8__1.aircraftPackageFamily.ReleaseId).ToString());
					}
					try
					{
						if (num != 1)
						{
							val = _003CfamilyNode_003E5__3.GetOrBindPathResource<SettingUpdateBinder<AircraftPackageFamily>>(delegate(IModifiableSettingSymbolTable familyTable)
							{
								_003C_003Ec__DisplayClass16_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass16_2
								{
									familyTable = familyTable
								};
								IObservable<TimingEvent> updateStream = Observable.CombineLatest<ITrainerPackage, FamilyData, ValueTuple<ITrainerPackage, FamilyData>>(_003C_003E8__1._003C_003E4__this._packageRepo.GetPackageLineage(_003C_003E8__1.aircraftPackageFamily.ReleaseId), TimingEvent.StartStreamAndConcat<AircraftPackageFamily>(_003C_003E8__1._003C_003E4__this._familyDataRepo.WhenAircraftFamilyDataChanges.OnCreateOrUpdate<AircraftPackageFamily>().Where<AircraftPackageFamily>((Func<AircraftPackageFamily, bool>)((AircraftPackageFamily apf) => apf == _003C_003E8__1.aircraftPackageFamily))).Select<TimingEvent, System.Threading.Tasks.Task<FamilyData>>((Func<TimingEvent, System.Threading.Tasks.Task<FamilyData>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass16_0._003C_003CBindAircraftAndFamilyWithLock_003Eb__6_003Ed))] async (TimingEvent _) => await _003C_003E8__1._003C_003E4__this._familyDataRepo.GetFamilyDataForReleaseOrDefault(_003C_003E8__1.aircraftPackageFamily).DetachContext())).Switch<FamilyData>(), (ITrainerPackage package, FamilyData familyData) => new ValueTuple<ITrainerPackage, FamilyData>(package, familyData)).ToCurrentAndMaybePrevious<ValueTuple<ITrainerPackage, FamilyData>>().Do<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>((Action<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>)delegate(CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>> t)
								{
									//IL_0002: Unknown result type (might be due to invalid IL or missing references)
									//IL_001e: Unknown result type (might be due to invalid IL or missing references)
									//IL_0049: Unknown result type (might be due to invalid IL or missing references)
									ITrainerPackage trainerPackage = ThrowIfPackageIsNull(t.Current.Item1);
									if (!t.HasPrevious || trainerPackage != t.Previous.Item1)
									{
										AddOrUpdateTableWithDefinitions(CS_0024_003C_003E8__locals0.familyTable, trainerPackage.GetScopedSettingDefinitions()[ManifestSettingScope.Family]);
									}
									CS_0024_003C_003E8__locals0.familyTable.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)t.Current.Item2.Settings);
								})
									.ToTimingEventStream<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>();
								return new SettingUpdateBinder<AircraftPackageFamily>(_003C_003E8__1.aircraftPackageFamily, updateStream);
							}).FirstUpdateOccurred.DetachContext();
							awaiter = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter)).IsCompleted)
							{
								num = (_003C_003E1__state = 1);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CBindAircraftAndFamilyWithLock_003Ed__16>(ref awaiter, ref this);
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
						result = new ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>(_003CaircraftNode_003E5__2, _003CfamilyNode_003E5__3);
					}
					catch
					{
						((System.IDisposable)_003CfamilyNode_003E5__3).Dispose();
						throw;
					}
				}
				catch
				{
					((System.IDisposable)_003CaircraftNode_003E5__2).Dispose();
					throw;
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CaircraftNode_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003CaircraftNode_003E5__2 = null;
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
	private struct _003CBindFamilySettings_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISettingsBinder<AircraftPackageFamily>> _003C_003Et__builder;

		public SettingsRepo _003C_003E4__this;

		public AircraftPackageFamily aircraftPackageFamily;

		private _003C_003Ec__DisplayClass10_0 _003C_003E8__1;

		private System.IDisposable _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Expected O, but got Unknown
			int num = _003C_003E1__state;
			SettingsRepo settingsRepo = _003C_003E4__this;
			ISettingsBinder<AircraftPackageFamily> result3;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if (num == 1)
					{
						goto IL_00ae;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass10_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					awaiter = settingsRepo._lock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CBindFamilySettings_003Ed__10>(ref awaiter, ref this);
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
				goto IL_00ae;
				IL_00ae:
				try
				{
					ConfiguredTaskAwaiter<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>> awaiter2;
					if (num != 1)
					{
						awaiter2 = settingsRepo.BindAircraftAndFamilyWithLock(aircraftPackageFamily).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>>, _003CBindFamilySettings_003Ed__10>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>>);
						num = (_003C_003E1__state = -1);
					}
					ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath> result2 = awaiter2.GetResult();
					_003C_003E8__1.aircraftNode = result2.Item1;
					_003C_003E8__1.familyNode = result2.Item2;
					try
					{
						result3 = new FamilyBinder(aircraftPackageFamily, _003C_003E8__1.familyNode.Table, Disposable.Create((Action)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass10_0._003C_003CBindFamilySettings_003Eb__0_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass10_0._003C_003CBindFamilySettings_003Eb__0_003Ed _003C_003CBindFamilySettings_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass10_0._003C_003CBindFamilySettings_003Eb__0_003Ed);
							_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
							_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003E4__this = _003C_003E8__1;
							_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003E1__state = -1;
							((AsyncVoidMethodBuilder)(ref _003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass10_0._003C_003CBindFamilySettings_003Eb__0_003Ed>(ref _003C_003CBindFamilySettings_003Eb__0_003Ed);
						})));
					}
					catch
					{
						((System.IDisposable)_003C_003E8__1.familyNode).Dispose();
						((System.IDisposable)_003C_003E8__1.aircraftNode).Dispose();
						throw;
					}
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap1 != null)
					{
						_003C_003E7__wrap1.Dispose();
					}
				}
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
	private struct _003CBindSimulator_003Ed__11 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<ISettingsBinder<SimAircraftKey>> _003C_003Et__builder;

		public SettingsRepo _003C_003E4__this;

		public SimAircraftKey simAircraftKey;

		private _003C_003Ec__DisplayClass11_0 _003C_003E8__1;

		private _003C_003Ec__DisplayClass11_1 _003C_003E8__2;

		private _003C_003Ec__DisplayClass11_2 _003C_003E8__3;

		private System.IDisposable _003C_003E7__wrap1;

		private ConfiguredTaskAwaiter<System.IDisposable> _003C_003Eu__1;

		private ConfiguredTaskAwaiter<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>> _003C_003Eu__2;

		private BinderNodeCounter.IBoundPath _003CsimSupportedSettingsNode_003E5__3;

		private ConfiguredTaskAwaiter _003C_003Eu__3;

		private void MoveNext()
		{
			//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Expected O, but got Unknown
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			SettingsRepo settingsRepo = _003C_003E4__this;
			ISettingsBinder<SimAircraftKey> result3;
			try
			{
				ConfiguredTaskAwaiter<System.IDisposable> awaiter;
				if (num != 0)
				{
					if ((uint)(num - 1) <= 1u)
					{
						goto IL_00c4;
					}
					_003C_003E8__1 = new _003C_003Ec__DisplayClass11_0();
					_003C_003E8__1._003C_003E4__this = _003C_003E4__this;
					_003C_003E8__1.simAircraftKey = simAircraftKey;
					awaiter = settingsRepo._lock.LockAsync().ConfigureAwait(continueOnCapturedContext: false).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<System.IDisposable>, _003CBindSimulator_003Ed__11>(ref awaiter, ref this);
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
				goto IL_00c4;
				IL_00c4:
				try
				{
					ConfiguredTaskAwaiter<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>> awaiter2;
					if (num != 1)
					{
						if (num == 2)
						{
							goto IL_01bd;
						}
						_003C_003E8__2 = new _003C_003Ec__DisplayClass11_1();
						_003C_003E8__2.CS_0024_003C_003E8__locals1 = _003C_003E8__1;
						AircraftPackageFamily aircraftPackageFamily = new AircraftPackageFamily(_003C_003E8__2.CS_0024_003C_003E8__locals1.simAircraftKey.AircraftIdentifier, _003C_003E8__2.CS_0024_003C_003E8__locals1.simAircraftKey.InstallationKey.ReleaseId);
						awaiter2 = settingsRepo.BindAircraftAndFamilyWithLock(aircraftPackageFamily).DetachContext().GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>>, _003CBindSimulator_003Ed__11>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__2;
						_003C_003Eu__2 = default(ConfiguredTaskAwaiter<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>>);
						num = (_003C_003E1__state = -1);
					}
					ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath> result2 = awaiter2.GetResult();
					BinderNodeCounter.IBoundPath item = result2.Item1;
					BinderNodeCounter.IBoundPath item2 = result2.Item2;
					_003C_003Ec__DisplayClass11_1 _003C_003Ec__DisplayClass11_ = _003C_003E8__2;
					List<System.IDisposable> obj = new List<System.IDisposable>();
					obj.Add((System.IDisposable)item);
					obj.Add((System.IDisposable)item2);
					_003C_003Ec__DisplayClass11_.nodes = obj;
					goto IL_01bd;
					IL_01bd:
					try
					{
						ConfiguredTaskAwaiter awaiter3;
						if (num != 2)
						{
							_003C_003E8__3 = new _003C_003Ec__DisplayClass11_2();
							_003C_003E8__3.CS_0024_003C_003E8__locals2 = _003C_003E8__2;
							BinderNodeCounter.IBoundPath boundPath = item2.BindRelativeIdent(((object)_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.InstallationKey.Slot).ToString());
							_003C_003E8__3.CS_0024_003C_003E8__locals2.nodes.Add((System.IDisposable)boundPath);
							_003C_003E8__3.simNode = boundPath.BindRelativeIdent(((object)_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.InstallationKey.ModelId).ToString());
							_003C_003E8__3.CS_0024_003C_003E8__locals2.nodes.Add((System.IDisposable)_003C_003E8__3.simNode);
							_003CsimSupportedSettingsNode_003E5__3 = _003C_003E8__3.simNode.BindRelativeIdent("SupportedSettings");
							_003C_003E8__3.CS_0024_003C_003E8__locals2.nodes.Add((System.IDisposable)_003CsimSupportedSettingsNode_003E5__3);
							SettingUpdateBinder<SimAircraftKey> orBindPathResource = _003CsimSupportedSettingsNode_003E5__3.GetOrBindPathResource<SettingUpdateBinder<SimAircraftKey>>(delegate(IModifiableSettingSymbolTable slotTable)
							{
								_003C_003Ec__DisplayClass11_3 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass11_3
								{
									CS_0024_003C_003E8__locals3 = _003C_003E8__3,
									slotTable = slotTable
								};
								IObservable<TimingEvent> updateStream = TimingEvent.StartStreamAndConcat<SimInstallKey>(_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._simRepo.WhenSimulatorIsChangedOnAircraft(_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.AircraftIdentifier).Where<DataChangeNotification<SimInstallKey>>((Func<DataChangeNotification<SimInstallKey>, bool>)((DataChangeNotification<SimInstallKey> sim) => sim.Value == _003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.InstallationKey)).OnCreateOrUpdate<SimInstallKey>()).Select<TimingEvent, System.Threading.Tasks.Task<SimInstallData>>((Func<TimingEvent, System.Threading.Tasks.Task<SimInstallData>>)((TimingEvent _) => _003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._simRepo.GetSimulator(_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.AircraftIdentifier, _003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey.InstallationKey))).Switch<SimInstallData>()
									.Select<SimInstallData, System.Threading.Tasks.Task<ValueTuple<SimInstallData, ITrainerPackage>>>((Func<SimInstallData, System.Threading.Tasks.Task<ValueTuple<SimInstallData, ITrainerPackage>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass11_0._003C_003CBindSimulator_003Eb__4_003Ed))] async (SimInstallData simData) => new ValueTuple<SimInstallData, ITrainerPackage>(simData, await _003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._packageRepo.GetPackageById(simData.FullPackageIdOrThrow()).DetachContext())))
									.Switch<ValueTuple<SimInstallData, ITrainerPackage>>()
									.ToCurrentAndMaybePrevious<ValueTuple<SimInstallData, ITrainerPackage>>()
									.Do<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>((Action<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>)delegate(CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>> t)
									{
										//IL_0002: Unknown result type (might be due to invalid IL or missing references)
										//IL_0047: Unknown result type (might be due to invalid IL or missing references)
										//IL_005e: Unknown result type (might be due to invalid IL or missing references)
										//IL_001d: Unknown result type (might be due to invalid IL or missing references)
										//IL_0029: Unknown result type (might be due to invalid IL or missing references)
										//IL_007a: Unknown result type (might be due to invalid IL or missing references)
										ThrowIfPackageIsNull(t.Current.Item2);
										if (!t.HasPrevious || t.Previous.Item2 != t.Current.Item2)
										{
											SetupFirstClassCitizenSettings(CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals3.simNode.Table, t.Current.Item1);
											AddOrUpdateTableWithDefinitions(CS_0024_003C_003E8__locals0.slotTable, t.Current.Item2.GetSlotSettingDefinitions());
										}
										CS_0024_003C_003E8__locals0.slotTable.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)t.Current.Item1.Settings);
									})
									.ToTimingEventStream<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>();
								return new SettingUpdateBinder<SimAircraftKey>(_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey, updateStream);
							});
							if (orBindPathResource.Key != _003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey)
							{
								throw new InvalidOperationException("The keys do not match. Was the previous binder torn down properly?");
							}
							ConfiguredTaskAwaitable val = orBindPathResource.FirstUpdateOccurred.DetachContext();
							awaiter3 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
							if (!((ConfiguredTaskAwaiter)(ref awaiter3)).IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__3 = awaiter3;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CBindSimulator_003Ed__11>(ref awaiter3, ref this);
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
						_003C_003E8__3.CS_0024_003C_003E8__locals2.nodes.Reverse();
						result3 = new SimSettingBinder(_003C_003E8__3.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.simAircraftKey, _003CsimSupportedSettingsNode_003E5__3.Table, Disposable.Create((Action)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass11_1._003C_003CBindSimulator_003Eb__1_003Ed))] () =>
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							_003C_003Ec__DisplayClass11_1._003C_003CBindSimulator_003Eb__1_003Ed _003C_003CBindSimulator_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass11_1._003C_003CBindSimulator_003Eb__1_003Ed);
							_003C_003CBindSimulator_003Eb__1_003Ed._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
							_003C_003CBindSimulator_003Eb__1_003Ed._003C_003E4__this = _003C_003E8__3.CS_0024_003C_003E8__locals2;
							_003C_003CBindSimulator_003Eb__1_003Ed._003C_003E1__state = -1;
							((AsyncVoidMethodBuilder)(ref _003C_003CBindSimulator_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass11_1._003C_003CBindSimulator_003Eb__1_003Ed>(ref _003C_003CBindSimulator_003Eb__1_003Ed);
						})));
					}
					catch
					{
						_003C_003E8__2.nodes.Reverse();
						Enumerator<System.IDisposable> enumerator = _003C_003E8__2.nodes.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								enumerator.Current.Dispose();
							}
						}
						finally
						{
							if (num < 0)
							{
								((System.IDisposable)enumerator).Dispose();
							}
						}
						throw;
					}
				}
				finally
				{
					if (num < 0 && _003C_003E7__wrap1 != null)
					{
						_003C_003E7__wrap1.Dispose();
					}
				}
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

	private readonly IModifiableGlobalSettingsTable _globals;

	private readonly IAircraftRepoEvolved _aircraftRepo;

	private readonly ISimulatorRepository _simRepo;

	private readonly ITrainerPackageRepoEvolved _packageRepo;

	private readonly IFamilyDataRepo _familyDataRepo;

	private readonly AsyncLock _lock = new AsyncLock();

	private readonly BinderNodeCounter _nodeCounter;

	public IGlobalSettingsProvider Globals => _globals;

	public SettingsRepo(IModifiableGlobalSettingsTable globals, IAircraftRepoEvolved aircraftRepo, ISimulatorRepository simRepo, ITrainerPackageRepoEvolved packages, IFamilyDataRepo familyDataRepo)
	{
		_globals = globals;
		_aircraftRepo = aircraftRepo;
		_simRepo = simRepo;
		_packageRepo = packages;
		_familyDataRepo = familyDataRepo;
		_nodeCounter = new BinderNodeCounter(SettingTreeBinder.CreateRoot(globals, SettingIdentifier.Empty));
	}

	[AsyncStateMachine(typeof(_003CBindFamilySettings_003Ed__10))]
	public async System.Threading.Tasks.Task<ISettingsBinder<AircraftPackageFamily>> BindFamilySettings(AircraftPackageFamily aircraftPackageFamily)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass10_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass10_0();
		CS_0024_003C_003E8__locals0._003C_003E4__this = this;
		using (await _lock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath> val = await BindAircraftAndFamilyWithLock(aircraftPackageFamily).DetachContext();
			CS_0024_003C_003E8__locals0.aircraftNode = val.Item1;
			CS_0024_003C_003E8__locals0.familyNode = val.Item2;
			try
			{
				return new FamilyBinder(aircraftPackageFamily, CS_0024_003C_003E8__locals0.familyNode.Table, Disposable.Create((Action)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass10_0._003C_003CBindFamilySettings_003Eb__0_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass10_0._003C_003CBindFamilySettings_003Eb__0_003Ed _003C_003CBindFamilySettings_003Eb__0_003Ed = default(_003C_003Ec__DisplayClass10_0._003C_003CBindFamilySettings_003Eb__0_003Ed);
					_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
					_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003E4__this = CS_0024_003C_003E8__locals0;
					_003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003E1__state = -1;
					((AsyncVoidMethodBuilder)(ref _003C_003CBindFamilySettings_003Eb__0_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass10_0._003C_003CBindFamilySettings_003Eb__0_003Ed>(ref _003C_003CBindFamilySettings_003Eb__0_003Ed);
				})));
			}
			catch
			{
				((System.IDisposable)CS_0024_003C_003E8__locals0.familyNode).Dispose();
				((System.IDisposable)CS_0024_003C_003E8__locals0.aircraftNode).Dispose();
				throw;
			}
		}
	}

	[AsyncStateMachine(typeof(_003CBindSimulator_003Ed__11))]
	public unsafe async System.Threading.Tasks.Task<ISettingsBinder<SimAircraftKey>> BindSimulator(SimAircraftKey simAircraftKey)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		SimAircraftKey simAircraftKey2 = simAircraftKey;
		using (await _lock.LockAsync().ConfigureAwait(continueOnCapturedContext: false))
		{
			AircraftPackageFamily aircraftPackageFamily = new AircraftPackageFamily(simAircraftKey2.AircraftIdentifier, simAircraftKey2.InstallationKey.ReleaseId);
			_003F val = await BindAircraftAndFamilyWithLock(aircraftPackageFamily).DetachContext();
			BinderNodeCounter.IBoundPath item = ((ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>*)(&val))->Item1;
			BinderNodeCounter.IBoundPath item2 = ((ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>*)(&val))->Item2;
			List<System.IDisposable> obj = new List<System.IDisposable>();
			obj.Add((System.IDisposable)item);
			obj.Add((System.IDisposable)item2);
			List<System.IDisposable> nodes = obj;
			try
			{
				BinderNodeCounter.IBoundPath boundPath = item2.BindRelativeIdent(((object)simAircraftKey2.InstallationKey.Slot).ToString());
				nodes.Add((System.IDisposable)boundPath);
				BinderNodeCounter.IBoundPath simNode = boundPath.BindRelativeIdent(((object)simAircraftKey2.InstallationKey.ModelId).ToString());
				nodes.Add((System.IDisposable)simNode);
				BinderNodeCounter.IBoundPath simSupportedSettingsNode = simNode.BindRelativeIdent("SupportedSettings");
				nodes.Add((System.IDisposable)simSupportedSettingsNode);
				SettingUpdateBinder<SimAircraftKey> orBindPathResource = simSupportedSettingsNode.GetOrBindPathResource<SettingUpdateBinder<SimAircraftKey>>(delegate(IModifiableSettingSymbolTable slotTable)
				{
					IModifiableSettingSymbolTable slotTable2 = slotTable;
					IObservable<TimingEvent> updateStream = TimingEvent.StartStreamAndConcat<SimInstallKey>(_simRepo.WhenSimulatorIsChangedOnAircraft(simAircraftKey2.AircraftIdentifier).Where<DataChangeNotification<SimInstallKey>>((Func<DataChangeNotification<SimInstallKey>, bool>)((DataChangeNotification<SimInstallKey> sim) => sim.Value == simAircraftKey2.InstallationKey)).OnCreateOrUpdate<SimInstallKey>()).Select<TimingEvent, System.Threading.Tasks.Task<SimInstallData>>((Func<TimingEvent, System.Threading.Tasks.Task<SimInstallData>>)((TimingEvent _) => _simRepo.GetSimulator(simAircraftKey2.AircraftIdentifier, simAircraftKey2.InstallationKey))).Switch<SimInstallData>()
						.Select<SimInstallData, System.Threading.Tasks.Task<ValueTuple<SimInstallData, ITrainerPackage>>>((Func<SimInstallData, System.Threading.Tasks.Task<ValueTuple<SimInstallData, ITrainerPackage>>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass11_0._003C_003CBindSimulator_003Eb__4_003Ed))] async (SimInstallData simData) => new ValueTuple<SimInstallData, ITrainerPackage>(simData, await _packageRepo.GetPackageById(simData.FullPackageIdOrThrow()).DetachContext())))
						.Switch<ValueTuple<SimInstallData, ITrainerPackage>>()
						.ToCurrentAndMaybePrevious<ValueTuple<SimInstallData, ITrainerPackage>>()
						.Do<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>((Action<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>)delegate(CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>> t)
						{
							//IL_0002: Unknown result type (might be due to invalid IL or missing references)
							//IL_0047: Unknown result type (might be due to invalid IL or missing references)
							//IL_005e: Unknown result type (might be due to invalid IL or missing references)
							//IL_001d: Unknown result type (might be due to invalid IL or missing references)
							//IL_0029: Unknown result type (might be due to invalid IL or missing references)
							//IL_007a: Unknown result type (might be due to invalid IL or missing references)
							ThrowIfPackageIsNull(t.Current.Item2);
							if (!t.HasPrevious || t.Previous.Item2 != t.Current.Item2)
							{
								SetupFirstClassCitizenSettings(simNode.Table, t.Current.Item1);
								AddOrUpdateTableWithDefinitions(slotTable2, t.Current.Item2.GetSlotSettingDefinitions());
							}
							slotTable2.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)t.Current.Item1.Settings);
						})
						.ToTimingEventStream<CurrentAndPreviousTuple<ValueTuple<SimInstallData, ITrainerPackage>>>();
					return new SettingUpdateBinder<SimAircraftKey>(simAircraftKey2, updateStream);
				});
				if (orBindPathResource.Key != simAircraftKey2)
				{
					throw new InvalidOperationException("The keys do not match. Was the previous binder torn down properly?");
				}
				await orBindPathResource.FirstUpdateOccurred.DetachContext();
				nodes.Reverse();
				return new SimSettingBinder(simAircraftKey2, simSupportedSettingsNode.Table, Disposable.Create((Action)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass11_1._003C_003CBindSimulator_003Eb__1_003Ed))] () =>
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					_003C_003Ec__DisplayClass11_1._003C_003CBindSimulator_003Eb__1_003Ed _003C_003CBindSimulator_003Eb__1_003Ed = default(_003C_003Ec__DisplayClass11_1._003C_003CBindSimulator_003Eb__1_003Ed);
					_003C_003CBindSimulator_003Eb__1_003Ed._003C_003Et__builder = AsyncVoidMethodBuilder.Create();
					_003C_003Ec__DisplayClass11_1 _003C_003E4__this;
					_003C_003CBindSimulator_003Eb__1_003Ed._003C_003E4__this = _003C_003E4__this;
					_003C_003CBindSimulator_003Eb__1_003Ed._003C_003E1__state = -1;
					((AsyncVoidMethodBuilder)(ref _003C_003CBindSimulator_003Eb__1_003Ed._003C_003Et__builder)).Start<_003C_003Ec__DisplayClass11_1._003C_003CBindSimulator_003Eb__1_003Ed>(ref _003C_003CBindSimulator_003Eb__1_003Ed);
				})));
			}
			catch
			{
				nodes.Reverse();
				Enumerator<System.IDisposable> enumerator = nodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						enumerator.Current.Dispose();
					}
				}
				finally
				{
					((System.IDisposable)enumerator).Dispose();
				}
				throw;
			}
		}
	}

	public void Dispose()
	{
		_nodeCounter.Dispose();
	}

	private static ITrainerPackage ThrowIfPackageIsNull(ITrainerPackage? package)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (package == null)
		{
			throw new InvalidOperationException("Package missing while settings were bound to it.");
		}
		return package;
	}

	private static void AddOrUpdateTableWithDefinitions(IModifiableSettingSymbolTable simulatorSettings, ISettingDefinitionCollection settingDefinitions)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (settingDefinitions == null)
		{
			throw new ArgumentNullException("settingDefinitions", "Setting definitions cannot be null.");
		}
		System.Collections.Generic.IEnumerator<ISettingDefinition> enumerator = Enumerable.Where<ISettingDefinition>(settingDefinitions.Definitions, (Func<ISettingDefinition, bool>)((ISettingDefinition d) => d.ControlType != SettingUIControlType.None)).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				ISettingDefinition current = enumerator.Current;
				simulatorSettings.AddOrReplaceIfDefinitionDoesntMatch(current);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private static void SetupFirstClassCitizenSettings(IModifiableSettingSymbolTable simulatorSettings, SimInstallData simInstallData)
	{
		TrainerModelKey trainerModelKey = simInstallData.ToFullModelId();
		SettingDefinition settingDefinition = new SettingDefinition(((object)simInstallData.FullPackageIdOrThrow()).ToString(), "Models")
		{
			DisplayText = "Selected Model",
			Description = "The model selected during panel installation.",
			ControlType = SettingUIControlType.Picklist,
			DefaultValue = trainerModelKey.ModelId.IdentStringChecked
		};
		settingDefinition.Add(new SettingValueDefinition(settingDefinition.SettingId, SettingIdentifier.FromTrainerIdentifier(trainerModelKey.ModelId))
		{
			SimulatorValue = trainerModelKey.ModelId.IdentStringChecked
		});
		simulatorSettings.AddOrReplaceIfDefinitionDoesntMatch(settingDefinition);
	}

	[AsyncStateMachine(typeof(_003CBindAircraftAndFamilyWithLock_003Ed__16))]
	private async System.Threading.Tasks.Task<ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>> BindAircraftAndFamilyWithLock(AircraftPackageFamily aircraftPackageFamily)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		BinderNodeCounter.IBoundPath aircraftNode = _nodeCounter.BindPath(aircraftPackageFamily.AircraftIdentifier);
		await aircraftNode.GetOrBindPathResource<SettingUpdateBinder<AircraftIdentifier>>(delegate(IModifiableSettingSymbolTable aircraftTable)
		{
			IModifiableSettingSymbolTable aircraftTable2 = aircraftTable;
			IObservable<TimingEvent> updateStream2 = Observable.FromAsync<AircraftListData>((Func<System.Threading.Tasks.Task<AircraftListData>>)(() => _aircraftRepo.GetAircraftListDataById(aircraftPackageFamily.AircraftIdentifier))).Concat<AircraftListData>(_aircraftRepo.AircraftChanges.ForAircraft(aircraftPackageFamily.AircraftIdentifier).StripChangeType<AircraftIdentifier>().Select<AircraftIdentifier, System.Threading.Tasks.Task<AircraftListData>>((Func<AircraftIdentifier, System.Threading.Tasks.Task<AircraftListData>>)((AircraftIdentifier ai) => _aircraftRepo.GetAircraftListDataById(ai)))
				.Switch<AircraftListData>()).Do<AircraftListData>((Action<AircraftListData>)delegate(AircraftListData listData)
			{
				AircraftSettingUtils.SetupAirframeSetting(aircraftTable2, listData.AirframeCCT);
			})
				.ToTimingEventStream<AircraftListData>();
			return new SettingUpdateBinder<AircraftIdentifier>(aircraftPackageFamily.AircraftIdentifier, updateStream2);
		}).FirstUpdateOccurred.DetachContext();
		try
		{
			BinderNodeCounter.IBoundPath familyNode = aircraftNode.BindRelativeIdent(((object)aircraftPackageFamily.ReleaseId).ToString());
			try
			{
				await familyNode.GetOrBindPathResource<SettingUpdateBinder<AircraftPackageFamily>>(delegate(IModifiableSettingSymbolTable familyTable)
				{
					IModifiableSettingSymbolTable familyTable2 = familyTable;
					IObservable<TimingEvent> updateStream = Observable.CombineLatest<ITrainerPackage, FamilyData, ValueTuple<ITrainerPackage, FamilyData>>(_packageRepo.GetPackageLineage(aircraftPackageFamily.ReleaseId), TimingEvent.StartStreamAndConcat<AircraftPackageFamily>(_familyDataRepo.WhenAircraftFamilyDataChanges.OnCreateOrUpdate<AircraftPackageFamily>().Where<AircraftPackageFamily>((Func<AircraftPackageFamily, bool>)((AircraftPackageFamily apf) => apf == aircraftPackageFamily))).Select<TimingEvent, System.Threading.Tasks.Task<FamilyData>>((Func<TimingEvent, System.Threading.Tasks.Task<FamilyData>>)([AsyncStateMachine(typeof(_003C_003Ec__DisplayClass16_0._003C_003CBindAircraftAndFamilyWithLock_003Eb__6_003Ed))] async (TimingEvent _) => await _familyDataRepo.GetFamilyDataForReleaseOrDefault(aircraftPackageFamily).DetachContext())).Switch<FamilyData>(), (ITrainerPackage package, FamilyData familyData) => new ValueTuple<ITrainerPackage, FamilyData>(package, familyData)).ToCurrentAndMaybePrevious<ValueTuple<ITrainerPackage, FamilyData>>().Do<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>((Action<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>)delegate(CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>> t)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_001e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						ITrainerPackage trainerPackage = ThrowIfPackageIsNull(t.Current.Item1);
						if (!t.HasPrevious || trainerPackage != t.Previous.Item1)
						{
							AddOrUpdateTableWithDefinitions(familyTable2, trainerPackage.GetScopedSettingDefinitions()[ManifestSettingScope.Family]);
						}
						familyTable2.LoadPersistedOptionsAndGetInvalids((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)t.Current.Item2.Settings);
					})
						.ToTimingEventStream<CurrentAndPreviousTuple<ValueTuple<ITrainerPackage, FamilyData>>>();
					return new SettingUpdateBinder<AircraftPackageFamily>(aircraftPackageFamily, updateStream);
				}).FirstUpdateOccurred.DetachContext();
				return new ValueTuple<BinderNodeCounter.IBoundPath, BinderNodeCounter.IBoundPath>(aircraftNode, familyNode);
			}
			catch
			{
				((System.IDisposable)familyNode).Dispose();
				throw;
			}
		}
		catch
		{
			((System.IDisposable)aircraftNode).Dispose();
			throw;
		}
	}
}
