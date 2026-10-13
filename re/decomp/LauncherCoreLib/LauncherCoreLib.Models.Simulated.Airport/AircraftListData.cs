using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Utility;
using RecordShims;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public class AircraftListData : RecordBase<AircraftListData>, IHaveAircraftId, IForkByAircraftIdentifier<AircraftListData>
{
	private sealed class DeepEquality : EqualityComparer<AircraftListData>
	{
		public override bool Equals(AircraftListData? x, AircraftListData? y)
		{
			if (x != y)
			{
				if (x != null && y != null && x.AircraftIdentifier == y.AircraftIdentifier && x.DisplayName == y.DisplayName && x.AirframeCCT == y.AirframeCCT && x.CanBeDeleted == y.CanBeDeleted && x.Description == y.Description)
				{
					return x.ListModuleData.IsTrackingSameSource(y.ListModuleData);
				}
				return false;
			}
			return true;
		}

		public override int GetHashCode(AircraftListData obj)
		{
			return ((object)obj.AircraftIdentifier).GetHashCode();
		}
	}

	public static readonly IEqualityComparer<AircraftListData> DEEP_EQUALS = (IEqualityComparer<AircraftListData>)(object)new DeepEquality();

	[field: CompilerGenerated]
	public AircraftIdentifier AircraftIdentifier
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public string DisplayName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public string Description
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public AirframeCategoryClassType AirframeCCT
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReadOnlyJsonDataSource ListModuleData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public bool IsHidden
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public bool CanBeDeleted
	{
		[CompilerGenerated]
		get;
	}

	public AircraftListData(AircraftIdentifier aircraftIdentifier = default(AircraftIdentifier), string displayName = "", string description = "", AirframeCategoryClassType airframeCCT = AirframeCategoryClassType.Unconfigured, IReadOnlyJsonDataSource? listModuleData = null, bool isHidden = false, bool canBeDeleted = false)
	{
		AircraftIdentifier = aircraftIdentifier;
		DisplayName = displayName ?? string.Empty;
		Description = description ?? string.Empty;
		AirframeCCT = airframeCCT;
		ListModuleData = listModuleData ?? new ReadOnlyJsonDataSource();
		IsHidden = isHidden;
		CanBeDeleted = canBeDeleted;
	}

	public AircraftListData Rename(string newName)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(newName))
		{
			throw new ArgumentNullException("newName");
		}
		if (DisplayName != newName)
		{
			AircraftListData obj = (AircraftListData)((object)this).MemberwiseClone();
			obj.DisplayName = newName;
			return obj;
		}
		return this;
	}

	public AircraftListData WithNewIdentifier(AircraftIdentifier aircraftIdentifier)
	{
		if (aircraftIdentifier != AircraftIdentifier)
		{
			return this.With(delegate(IPropertyChangeSet<AircraftListData> copy)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(AircraftListData), "ld");
				copy.Mutate(Expression.Lambda<Func<AircraftListData, AircraftIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), aircraftIdentifier);
			});
		}
		return this;
	}

	public override void ThrowIfConstraintsAreViolated(AircraftListData record)
	{
	}
}
