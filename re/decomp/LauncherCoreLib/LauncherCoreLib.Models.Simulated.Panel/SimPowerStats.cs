using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using RecordShims;

namespace LauncherCoreLib.Models.Simulated.Panel;

public class SimPowerStats : RecordBase<SimPowerStats>
{
	[JsonProperty]
	[field: CompilerGenerated]
	public int TotalPowerUps
	{
		[CompilerGenerated]
		get;
	}

	[JsonProperty]
	[field: CompilerGenerated]
	public int PowerUpsSinceReset
	{
		[CompilerGenerated]
		get;
	}

	public SimPowerStats()
		: this(0, 0)
	{
	}

	[JsonConstructor]
	public SimPowerStats(int totalPowerUps, int powerUpsSinceReset)
	{
		TotalPowerUps = totalPowerUps;
		PowerUpsSinceReset = powerUpsSinceReset;
	}

	public SimPowerStats IncrementBoth()
	{
		return this.With([CompilerGenerated] (IPropertyChangeSet<SimPowerStats> copy) =>
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected O, but got Unknown
			ParameterExpression val = Expression.Parameter(typeof(SimPowerStats), "_");
			ISubsequentPropertyChangeSetApi<SimPowerStats> subsequentPropertyChangeSetApi = copy.Mutate<int>(Expression.Lambda<Func<SimPowerStats, int>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(SimPowerStats)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), (SimPowerStats c) => c.TotalPowerUps + 1);
			val = Expression.Parameter(typeof(SimPowerStats), "_");
			subsequentPropertyChangeSetApi.AndMutate<int>(Expression.Lambda<Func<SimPowerStats, int>>((Expression)(object)Expression.Property((Expression)(object)Expression.Constant((object)this, typeof(SimPowerStats)), (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), (SimPowerStats c) => c.PowerUpsSinceReset + 1);
		});
	}

	public override void ThrowIfConstraintsAreViolated(SimPowerStats record)
	{
	}
}
