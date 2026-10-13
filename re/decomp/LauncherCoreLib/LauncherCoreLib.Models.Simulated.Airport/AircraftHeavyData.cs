using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftHeavyData : RecordBase<AircraftHeavyData>, IHaveAircraftId, IForkByAircraftIdentifier<AircraftHeavyData>
{
	private sealed class DeepEquals : EqualityComparer<AircraftHeavyData>
	{
		public override bool Equals(AircraftHeavyData? x, AircraftHeavyData? y)
		{
			if (x != y)
			{
				if (x != null && y != null)
				{
					return x.AircraftModuleData.IsTrackingSameSource(y.AircraftModuleData);
				}
				return false;
			}
			return true;
		}

		public override int GetHashCode(AircraftHeavyData obj)
		{
			return ((object)obj.AircraftIdentifier).GetHashCode();
		}
	}

	public static readonly IEqualityComparer<AircraftHeavyData> DEEP_EQUALS = (IEqualityComparer<AircraftHeavyData>)(object)new DeepEquals();

	[field: CompilerGenerated]
	public AircraftIdentifier AircraftIdentifier
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public IReadOnlyJsonDataSource AircraftModuleData
	{
		[CompilerGenerated]
		get;
	}

	public AircraftHeavyData(AircraftIdentifier aircraftIdentifier = default(AircraftIdentifier), IReadOnlyJsonDataSource? aircraftModuleData = null)
	{
		AircraftIdentifier = aircraftIdentifier;
		AircraftModuleData = aircraftModuleData ?? new ReadOnlyJsonDataSource();
	}

	public AircraftHeavyData ReplaceModuleData(JObject aircraftModuleData)
	{
		ReadOnlyJsonDataSource wrapper = new ReadOnlyJsonDataSource(aircraftModuleData);
		if (!AircraftModuleData.IsTrackingSameSource(wrapper))
		{
			return this.With(delegate(IPropertyChangeSet<AircraftHeavyData> copy)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(AircraftHeavyData), "_");
				copy.Mutate(Expression.Lambda<Func<AircraftHeavyData, IReadOnlyJsonDataSource>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(AircraftHeavyData)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), (IReadOnlyJsonDataSource)wrapper);
			});
		}
		return this;
	}

	public AircraftHeavyData ReplaceModuleData(ISynchronizedJsonDataSource dataSource)
	{
		IReadOnlyJsonDataSource wrapper = dataSource.ToReadonlySourceSync();
		if (!AircraftModuleData.IsTrackingSameSource(wrapper))
		{
			return this.With(delegate(IPropertyChangeSet<AircraftHeavyData> copy)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(AircraftHeavyData), "_");
				copy.Mutate(Expression.Lambda<Func<AircraftHeavyData, IReadOnlyJsonDataSource>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(AircraftHeavyData)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), wrapper);
			});
		}
		return this;
	}

	public AircraftHeavyData WithNewIdentifier(AircraftIdentifier aircraftIdentifier)
	{
		if (aircraftIdentifier != AircraftIdentifier)
		{
			return this.With(delegate(IPropertyChangeSet<AircraftHeavyData> copy)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(AircraftHeavyData), "_");
				copy.Mutate(Expression.Lambda<Func<AircraftHeavyData, AircraftIdentifier>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(AircraftHeavyData)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), aircraftIdentifier);
			});
		}
		return this;
	}

	public override void ThrowIfConstraintsAreViolated(AircraftHeavyData record)
	{
	}
}
