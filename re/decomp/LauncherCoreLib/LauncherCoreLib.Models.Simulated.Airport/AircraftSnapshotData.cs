using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Panel;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftSnapshotData : RecordBase<AircraftSnapshotData>, IHaveAircraftId, IForkByAircraftIdentifier<AircraftSnapshotData>
{
	private sealed class DeepSnapshotEquality : EqualityComparer<AircraftSnapshotData>
	{
		public override bool Equals(AircraftSnapshotData? x, AircraftSnapshotData? y)
		{
			if (x != y)
			{
				if (x != null && y != null && AircraftListData.DEEP_EQUALS.Equals(x.ListData, y.ListData) && AircraftHeavyData.DEEP_EQUALS.Equals(x.HeavyData, y.HeavyData) && SimPanel.DEEP_EQUALS.Equals(x.Panel, y.Panel))
				{
					return FamilyDataLookup.DEEP_EQUALS.Equals(x.FamilyLookup, y.FamilyLookup);
				}
				return false;
			}
			return true;
		}

		public override int GetHashCode(AircraftSnapshotData obj)
		{
			return ((object)obj.ListData).GetHashCode() ^ ((object)obj.Panel).GetHashCode() ^ ((object)obj.HeavyData).GetHashCode() ^ ((object)obj.FamilyLookup).GetHashCode();
		}
	}

	public static readonly IEqualityComparer<AircraftSnapshotData> DEEP_EQUALS = (IEqualityComparer<AircraftSnapshotData>)(object)new DeepSnapshotEquality();

	[field: CompilerGenerated]
	public AircraftListData ListData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public AircraftHeavyData HeavyData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public FamilyDataLookup FamilyLookup
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public SimPanel Panel
	{
		[CompilerGenerated]
		get;
	}

	public AircraftIdentifier AircraftIdentifier => ListData.AircraftIdentifier;

	public AircraftSnapshotData(AircraftIdentifier aircraftidentifier)
		: this(new AircraftListData(aircraftidentifier), new AircraftHeavyData(aircraftidentifier), new SimPanel(), new FamilyDataLookup())
	{
	}

	public AircraftSnapshotData(AircraftListData listdata, AircraftHeavyData heavy, SimPanel panel, FamilyDataLookup familyLookup)
	{
		ListData = listdata;
		HeavyData = heavy;
		Panel = panel;
		FamilyLookup = familyLookup;
		ThisClassesValidation(this);
	}

	public static AircraftSnapshotData CreateAircraftStartingData(AircraftIdentifier aircraftIdentifier)
	{
		return new AircraftSnapshotData(aircraftIdentifier);
	}

	public static AircraftSnapshotData FromListData(AircraftListData aircraftListData)
	{
		return new AircraftSnapshotData(aircraftListData.AircraftIdentifier)
		{
			ListData = aircraftListData
		};
	}

	public AircraftSnapshotData WithNewIdentifier(AircraftIdentifier aircraftIdentifier)
	{
		if (aircraftIdentifier != AircraftIdentifier)
		{
			return new AircraftSnapshotData(ListData.WithNewIdentifier(aircraftIdentifier), HeavyData.WithNewIdentifier(aircraftIdentifier), Panel.WithNewIdentifier(aircraftIdentifier), FamilyLookup);
		}
		return this;
	}

	public AircraftSnapshotData Relist(AircraftListData aircraftListData)
	{
		AircraftListData aircraftListData2 = aircraftListData;
		if (aircraftListData2 != ListData)
		{
			ThrowIfIdentifierDoesntMatch(aircraftListData2);
			return this.With(delegate(IPropertyChangeSet<AircraftSnapshotData> copy)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(AircraftSnapshotData), "_");
				copy.Mutate(Expression.Lambda<Func<AircraftSnapshotData, AircraftListData>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(AircraftSnapshotData)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), aircraftListData2);
			});
		}
		return this;
	}

	public AircraftSnapshotData ReplaceHeavyData(AircraftHeavyData heavyData)
	{
		AircraftHeavyData heavyData2 = heavyData;
		if (heavyData2 != HeavyData)
		{
			ThrowIfIdentifierDoesntMatch(heavyData2);
			return this.With(delegate(IPropertyChangeSet<AircraftSnapshotData> copy)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(AircraftSnapshotData), "_");
				copy.Mutate(Expression.Lambda<Func<AircraftSnapshotData, AircraftHeavyData>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(AircraftSnapshotData)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), heavyData2);
			});
		}
		return this;
	}

	public AircraftSnapshotData ReplacePanel(SimPanel simPanel)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		SimPanel simPanel2 = simPanel;
		if (simPanel2 != Panel)
		{
			if (Enumerable.Any<SimInstallData>(simPanel2.AllSimulators.Values, (Func<SimInstallData, bool>)((SimInstallData s) => s.AircraftSimKey.AircraftIdentifier != AircraftIdentifier)))
			{
				throw new ArgumentException("At least one simulator does not match this aircraft identifier.");
			}
			return this.With(delegate(IPropertyChangeSet<AircraftSnapshotData> copy)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(AircraftSnapshotData), "_");
				copy.Mutate(Expression.Lambda<Func<AircraftSnapshotData, SimPanel>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(AircraftSnapshotData)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), simPanel2);
			});
		}
		return this;
	}

	public AircraftSnapshotData ReplaceFamilyLookup(FamilyDataLookup familyLookup)
	{
		FamilyDataLookup familyLookup2 = familyLookup;
		if (familyLookup2 != FamilyLookup)
		{
			return this.With(delegate(IPropertyChangeSet<AircraftSnapshotData> copy)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(AircraftSnapshotData), "_");
				copy.Mutate(Expression.Lambda<Func<AircraftSnapshotData, FamilyDataLookup>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(AircraftSnapshotData)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), familyLookup2);
			});
		}
		return this;
	}

	public override void ThrowIfConstraintsAreViolated(AircraftSnapshotData record)
	{
		ThisClassesValidation(record);
	}

	private static void ThisClassesValidation(AircraftSnapshotData copy)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (copy.ListData == null)
		{
			throw new ArgumentNullException("copy", "ListData");
		}
		if (copy.HeavyData == null)
		{
			throw new ArgumentNullException("copy", "HeavyData");
		}
		if (copy.Panel == null)
		{
			throw new ArgumentNullException("copy", "Panel");
		}
		if (copy.FamilyLookup == null)
		{
			throw new ArgumentNullException("copy", "FamilyLookup");
		}
	}

	private void ThrowIfIdentifierDoesntMatch(IHaveAircraftId aircraftIdProvider)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (aircraftIdProvider.AircraftIdentifier != AircraftIdentifier)
		{
			throw new ArgumentException("Aircraft identifiers must match.");
		}
	}
}
