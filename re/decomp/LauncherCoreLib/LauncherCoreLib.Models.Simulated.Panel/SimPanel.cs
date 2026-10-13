using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class SimPanel : IHaveAircraftId, IForkByAircraftIdentifier<SimPanel>
{
	private sealed class DeepEquality : EqualityComparer<SimPanel>
	{
		public override bool Equals(SimPanel? x, SimPanel? y)
		{
			if (x != y)
			{
				if (x != null && y != null)
				{
					return !Enumerable.Any<SimInstallKey>(x.SimulatorsWithDifferences(y));
				}
				return false;
			}
			return true;
		}

		public override int GetHashCode(SimPanel obj)
		{
			return ((object)Enumerable.FirstOrDefault<SimInstallData>(obj.AllSimulators.Values)?.AircraftSimKey.AircraftIdentifier).GetHashCode() ?? 0;
		}
	}

	[CompilerGenerated]
	private sealed class _003CSimulatorChanges_003Ed__26 : System.Collections.Generic.IEnumerable<DataChangeNotification<SimInstallKey>>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<DataChangeNotification<SimInstallKey>>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private DataChangeNotification<SimInstallKey> _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		public SimPanel _003C_003E4__this;

		private SimPanel updatedPanel;

		public SimPanel _003C_003E3__updatedPanel;

		private System.Collections.Generic.IEnumerator<SimInstallKey> _003C_003E7__wrap1;

		DataChangeNotification<SimInstallKey> System.Collections.Generic.IEnumerator<DataChangeNotification<SimInstallKey>>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CSimulatorChanges_003Ed__26(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
		}

		private bool MoveNext()
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				int num = _003C_003E1__state;
				SimPanel simPanel = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E7__wrap1 = simPanel.SimulatorsWithDifferences(updatedPanel).GetEnumerator();
					_003C_003E1__state = -3;
					break;
				case 1:
					_003C_003E1__state = -3;
					break;
				}
				if (((System.Collections.IEnumerator)_003C_003E7__wrap1).MoveNext())
				{
					SimInstallKey current = _003C_003E7__wrap1.Current;
					bool flag = simPanel.AllSimulators.ContainsKey(current);
					bool flag2 = updatedPanel.AllSimulators.ContainsKey(current);
					DataChangeType change;
					if (flag && flag2)
					{
						change = DataChangeType.Updated;
					}
					else if (flag && !flag2)
					{
						change = DataChangeType.Deleted;
					}
					else
					{
						if (!(!flag && flag2))
						{
							throw new NotSupportedException("That's data change is impossible!");
						}
						change = DataChangeType.Created;
					}
					_003C_003E2__current = DataChangeNotification.Create(current, change);
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003Em__Finally1();
				_003C_003E7__wrap1 = null;
				return false;
			}
			catch
			{
				//try-fault
				((System.IDisposable)this).Dispose();
				throw;
			}
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap1 != null)
			{
				((System.IDisposable)_003C_003E7__wrap1).Dispose();
			}
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<DataChangeNotification<SimInstallKey>> System.Collections.Generic.IEnumerable<DataChangeNotification<SimInstallKey>>.GetEnumerator()
		{
			_003CSimulatorChanges_003Ed__26 _003CSimulatorChanges_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CSimulatorChanges_003Ed__ = this;
			}
			else
			{
				_003CSimulatorChanges_003Ed__ = new _003CSimulatorChanges_003Ed__26(0)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			_003CSimulatorChanges_003Ed__.updatedPanel = _003C_003E3__updatedPanel;
			return _003CSimulatorChanges_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<DataChangeNotification<SimInstallKey>>)this).GetEnumerator();
		}
	}

	public static readonly IEqualityComparer<SimPanel> DEEP_EQUALS = (IEqualityComparer<SimPanel>)(object)new DeepEquality();

	private readonly ImmutableArray<SimInstallKey> _activeKeys;

	[field: CompilerGenerated]
	public ImmutableDictionary<SimInstallKey, SimInstallData> AllSimulators
	{
		[CompilerGenerated]
		get;
	}

	public System.Collections.Generic.IReadOnlyList<SimInstallKey> ActiveSimKeys => (System.Collections.Generic.IReadOnlyList<SimInstallKey>)(object)_activeKeys;

	public System.Collections.Generic.IEnumerable<SimInstallData> ActiveSims => Enumerable.Select<KeyValuePair<SimInstallKey, SimInstallData>, SimInstallData>(FilterActivePairs((System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>>)AllSimulators), (Func<KeyValuePair<SimInstallKey, SimInstallData>, SimInstallData>)((KeyValuePair<SimInstallKey, SimInstallData> kv) => kv.Value));

	[field: CompilerGenerated]
	public AircraftIdentifier AircraftIdentifier
	{
		[CompilerGenerated]
		get;
	}

	public System.Collections.Generic.IEnumerable<TrainerPackageIdentifier> AllPackageReferences => Enumerable.Distinct<TrainerPackageIdentifier>(Enumerable.Select<SimInstallData, TrainerPackageIdentifier>(AllSimulators.Values, (Func<SimInstallData, TrainerPackageIdentifier>)((SimInstallData s) => s.FullPackageId)).WhereNotNull());

	public System.Collections.Generic.IEnumerable<TrainerPackageIdentifier> AllActivePackageReferences => Enumerable.Distinct<TrainerPackageIdentifier>(Enumerable.Select<SimInstallData, TrainerPackageIdentifier>(ActiveSims, (Func<SimInstallData, TrainerPackageIdentifier>)((SimInstallData s) => s.FullPackageId)).WhereNotNull());

	public SimPanel()
		: this(ImmutableDictionary<SimInstallKey, SimInstallData>.Empty)
	{
	}

	public SimPanel(System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>> allSims)
		: this((allSims != null) ? ImmutableDictionary.ToImmutableDictionary<SimInstallKey, SimInstallData>(allSims) : ImmutableDictionary<SimInstallKey, SimInstallData>.Empty)
	{
	}

	public SimPanel(ImmutableDictionary<SimInstallKey, SimInstallData> allSims)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		AllSimulators = allSims ?? ImmutableDictionary<SimInstallKey, SimInstallData>.Empty;
		_activeKeys = EnumerateCurrentActiveKeys();
		AircraftIdentifier = Enumerable.SingleOrDefault<AircraftIdentifier>(Enumerable.Distinct<AircraftIdentifier>(Enumerable.Select<SimInstallData, AircraftIdentifier>(AllSimulators.Values, (Func<SimInstallData, AircraftIdentifier>)((SimInstallData s) => s.AircraftSimKey.AircraftIdentifier))));
	}

	public static System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>> ToDictionaryEntries(System.Collections.Generic.IEnumerable<SimInstallData> simInstalls)
	{
		return Enumerable.Select<SimInstallData, KeyValuePair<SimInstallKey, SimInstallData>>(simInstalls, (Func<SimInstallData, KeyValuePair<SimInstallKey, SimInstallData>>)((SimInstallData s) => new KeyValuePair<SimInstallKey, SimInstallData>(s.InstallationKey, s)));
	}

	public static System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>> FilterActivePairs(System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>> simulatorMap)
	{
		if (simulatorMap == null)
		{
			return Enumerable.Empty<KeyValuePair<SimInstallKey, SimInstallData>>();
		}
		return Enumerable.Where<KeyValuePair<SimInstallKey, SimInstallData>>(simulatorMap, (Func<KeyValuePair<SimInstallKey, SimInstallData>, bool>)((KeyValuePair<SimInstallKey, SimInstallData> kv) => kv.Value != null && kv.Value.IsActiveSimulator));
	}

	public static System.Collections.Generic.IEnumerable<SimInstallKey> FilterActiveSimulators(System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>> simulatorMap)
	{
		return Enumerable.Select<KeyValuePair<SimInstallKey, SimInstallData>, SimInstallKey>(FilterActivePairs(simulatorMap), (Func<KeyValuePair<SimInstallKey, SimInstallData>, SimInstallKey>)((KeyValuePair<SimInstallKey, SimInstallData> kv) => kv.Key));
	}

	public SimInstallData? GetSimulatorDataOrNull(SimInstallKey key)
	{
		SimInstallData result = default(SimInstallData);
		if (!AllSimulators.TryGetValue(key, ref result))
		{
			return null;
		}
		return result;
	}

	public System.Collections.Generic.IEnumerable<SimInstallKey> ActiveSimKeysForCategory(AvionicsType category)
	{
		return Enumerable.Where<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)ActiveSimKeys, (Func<SimInstallKey, bool>)((SimInstallKey s) => s.Slot.Category == category));
	}

	public System.Collections.Generic.IEnumerable<SimInstallData> ActiveSimsForCategory(AvionicsType category)
	{
		return Enumerable.Where<SimInstallData>(ActiveSims, (Func<SimInstallData, bool>)((SimInstallData s) => s.InstallationKey.Slot.Category == category));
	}

	public System.Collections.Generic.IEnumerable<SimInstallKey> SimulatorsWithDifferences(SimPanel otherPanel)
	{
		return Enumerable.Select<SimInstallData, SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallData>)AllSimulators.Values.SymmetricDifference(otherPanel.AllSimulators.Values), (Func<SimInstallData, SimInstallKey>)((SimInstallData simData) => simData.InstallationKey));
	}

	[IteratorStateMachine(typeof(_003CSimulatorChanges_003Ed__26))]
	public System.Collections.Generic.IEnumerable<DataChangeNotification<SimInstallKey>> SimulatorChanges(SimPanel updatedPanel)
	{
		System.Collections.Generic.IEnumerator<SimInstallKey> enumerator = SimulatorsWithDifferences(updatedPanel).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				SimInstallKey current = enumerator.Current;
				bool flag = AllSimulators.ContainsKey(current);
				bool flag2 = updatedPanel.AllSimulators.ContainsKey(current);
				DataChangeType change;
				if (flag && flag2)
				{
					change = DataChangeType.Updated;
				}
				else if (flag && !flag2)
				{
					change = DataChangeType.Deleted;
				}
				else
				{
					if (!(!flag && flag2))
					{
						throw new NotSupportedException("That's data change is impossible!");
					}
					change = DataChangeType.Created;
				}
				yield return DataChangeNotification.Create(current, change);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	public bool ContainsSimulatorKey(SimAircraftKey aircraftSimKey)
	{
		return AllSimulators.ContainsKey(aircraftSimKey.InstallationKey);
	}

	public SimPanel WithUpdatedSimulators(SimInstallData simUpdate, params SimInstallData[] moreUpdates)
	{
		return WithUpdatedSimulators(Enumerable.Concat<SimInstallData>(simUpdate.Yield(), (System.Collections.Generic.IEnumerable<SimInstallData>)moreUpdates));
	}

	public SimPanel WithUpdatedSimulators(System.Collections.Generic.IEnumerable<SimInstallData> simUpdates)
	{
		return WithUpdatedSimulators(AllSimulators.SetItems(ToDictionaryEntries(simUpdates)));
	}

	public SimPanel WithUpdatedSimulators(ImmutableDictionary<SimInstallKey, SimInstallData> updatedSims)
	{
		if (AllSimulators == updatedSims)
		{
			return this;
		}
		return new SimPanel(updatedSims);
	}

	public SimPanel InstallOrReplaceActiveSimulator(SimInstallData installedSim)
	{
		SimInstallData installedSim2 = installedSim;
		System.Collections.Generic.IEnumerable<SimInstallData> enumerable = Enumerable.Select<SimInstallKey, SimInstallData>(Enumerable.Where<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)ActiveSimKeys, (Func<SimInstallKey, bool>)((SimInstallKey s) => s.Slot == installedSim2.InstallationKey.Slot)), (Func<SimInstallKey, SimInstallData>)((SimInstallKey s) => AllSimulators[s].ToActive(isActive: false)));
		installedSim2 = installedSim2.ToActive(isActive: true);
		return WithUpdatedSimulators(Enumerable.Concat<SimInstallData>(enumerable, installedSim2.Yield()));
	}

	public SimPanel DeactivateSimulator(SimInstallKey simToDeactivate)
	{
		if (AllSimulators.ContainsKey(simToDeactivate))
		{
			SimInstallData simInstallData = AllSimulators[simToDeactivate].ToActive(isActive: false);
			return WithUpdatedSimulators(AllSimulators.SetItem(simToDeactivate, simInstallData));
		}
		return this;
	}

	public SimPanel WithDeletedSimulators(System.Collections.Generic.IEnumerable<SimInstallKey> deletedKeys)
	{
		return WithUpdatedSimulators(AllSimulators.RemoveRange(deletedKeys));
	}

	public SimInstallData? GetActiveSimInSlot(InstallationSlot installSlot)
	{
		InstallationSlot installSlot2 = installSlot;
		SimInstallKey simInstallKey = Enumerable.FirstOrDefault<SimInstallKey>((System.Collections.Generic.IEnumerable<SimInstallKey>)ActiveSimKeys, (Func<SimInstallKey, bool>)((SimInstallKey s) => s.Slot == installSlot2));
		if (simInstallKey != null)
		{
			return AllSimulators[simInstallKey];
		}
		return null;
	}

	public SimPanel WithNewIdentifier(AircraftIdentifier aircraftIdentifier)
	{
		if (AllSimulators.Count > 0 && Enumerable.Any<SimInstallData>(AllSimulators.Values, (Func<SimInstallData, bool>)((SimInstallData s) => s.AircraftSimKey.AircraftIdentifier != aircraftIdentifier)))
		{
			return new SimPanel((System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>>)Enumerable.ToDictionary<KeyValuePair<SimInstallKey, SimInstallData>, SimInstallKey, SimInstallData>((System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>>)AllSimulators, (Func<KeyValuePair<SimInstallKey, SimInstallData>, SimInstallKey>)((KeyValuePair<SimInstallKey, SimInstallData> kv) => kv.Key), (Func<KeyValuePair<SimInstallKey, SimInstallData>, SimInstallData>)((KeyValuePair<SimInstallKey, SimInstallData> kv) => kv.Value.WithNewIdentifier(aircraftIdentifier))));
		}
		return this;
	}

	private ImmutableArray<SimInstallKey> EnumerateCurrentActiveKeys()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		ImmutableArray<SimInstallKey> val = ImmutableArray.ToImmutableArray<SimInstallKey>(FilterActiveSimulators((System.Collections.Generic.IEnumerable<KeyValuePair<SimInstallKey, SimInstallData>>)AllSimulators));
		List<InstallationSlot> val2 = Enumerable.ToList<InstallationSlot>(Enumerable.Select<IGrouping<InstallationSlot, SimInstallKey>, InstallationSlot>(Enumerable.Where<IGrouping<InstallationSlot, SimInstallKey>>(Enumerable.GroupBy<SimInstallKey, InstallationSlot>((System.Collections.Generic.IEnumerable<SimInstallKey>)(object)val, (Func<SimInstallKey, InstallationSlot>)((SimInstallKey k) => k.Slot)), (Func<IGrouping<InstallationSlot, SimInstallKey>, bool>)((IGrouping<InstallationSlot, SimInstallKey> groupedBySlot) => ((System.Collections.Generic.IEnumerable<SimInstallKey>)groupedBySlot).HasAtLeast(2))), (Func<IGrouping<InstallationSlot, SimInstallKey>, InstallationSlot>)((IGrouping<InstallationSlot, SimInstallKey> g) => g.Key)));
		if (val2.Count > 0)
		{
			throw new ArgumentException("More than 1 active sims in the following slots: [" + string.Join<InstallationSlot>(", ", (System.Collections.Generic.IEnumerable<InstallationSlot>)val2) + "]");
		}
		return val;
	}
}
