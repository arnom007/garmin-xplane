using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using LauncherCoreLib.Models.Settings;
using LauncherCoreLib.Models.Simulated.Airport;
using LauncherCoreLib.Utility;
using RecordShims;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class SimInstallData : IRecord<SimInstallData>, IEquatable<SimInstallData>, IForkByAircraftIdentifier<SimInstallData>
{
	[field: CompilerGenerated]
	public SimAircraftKey AircraftSimKey
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerPackageIdentifier? FullPackageId
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public ImmutableDictionary<SettingIdentifier, string> Settings
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IReadOnlyJsonDataSource ModuleData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public bool HasArchivedWinfsMount
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public bool IsActiveSimulator
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public SimInstallKey InstallationKey => AircraftSimKey.InstallationKey;

	public SimInstallData(SimAircraftKey aircraftSimKey, TrainerPackageIdentifier? fullPackageId = null, System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>? settings = null, IReadOnlyJsonDataSource? moduleData = null, bool hasArchivedWinfsMount = false, bool isActiveSimulator = false)
	{
		AircraftSimKey = aircraftSimKey;
		FullPackageId = fullPackageId;
		Settings = GetSettings(settings);
		ModuleData = moduleData ?? new ReadOnlyJsonDataSource();
		HasArchivedWinfsMount = hasArchivedWinfsMount;
		IsActiveSimulator = isActiveSimulator;
		ThisClassesValidation(this);
	}

	public TrainerPackageIdentifier FullPackageIdOrThrow()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return FullPackageId ?? throw new InvalidOperationException("Package reference is required.");
	}

	public TrainerModelKey ToFullModelId()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (FullPackageId == null)
		{
			throw new InvalidOperationException("Must provide a package id to get a full model id.");
		}
		return new TrainerModelKey(FullPackageId, InstallationKey.ModelId.IdentStringChecked);
	}

	public SimInstallData ToActive(bool isActive)
	{
		if (IsActiveSimulator != isActive)
		{
			SimInstallData obj = (SimInstallData)base.MemberwiseClone();
			obj.IsActiveSimulator = isActive;
			return obj;
		}
		return this;
	}

	public bool Equals(SimInstallData? other)
	{
		if (this != other)
		{
			if (other != null && AircraftSimKey == other.AircraftSimKey && FullPackageId == other.FullPackageId && HasArchivedWinfsMount == other.HasArchivedWinfsMount && IsActiveSimulator == other.IsActiveSimulator && ((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)Settings).SetEquals((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)other.Settings))
			{
				return ModuleData.IsTrackingSameSource(other.ModuleData);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SimInstallData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)AircraftSimKey).GetHashCode();
	}

	public SimInstallData WithNewIdentifier(AircraftIdentifier aircraftIdentifier)
	{
		if (aircraftIdentifier != AircraftSimKey.AircraftIdentifier)
		{
			SimAircraftKey newKey = AircraftSimKey.WithNewIdentifier(aircraftIdentifier);
			return this.With(delegate(IPropertyChangeSet<SimInstallData> copy)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "c");
				copy.Mutate(Expression.Lambda<Func<SimInstallData, SimAircraftKey>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), newKey);
			});
		}
		return this;
	}

	public SimInstallData WithNewRelease(TrainerEmbeddedReleaseId releaseId)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		TrainerEmbeddedReleaseId releaseId2 = releaseId;
		if (releaseId2 != InstallationKey.ReleaseId)
		{
			if (FullPackageId != null && releaseId2 != FullPackageId.EmbeddedReleaseCompositeId)
			{
				throw new InvalidOperationException("Must replace the full package.");
			}
			return this.With(delegate(IPropertyChangeSet<SimInstallData> copy)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "c");
				copy.Mutate(Expression.Lambda<Func<SimInstallData, SimAircraftKey>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), new SimAircraftKey(AircraftSimKey.AircraftIdentifier, InstallationKey.WithNewRelease(releaseId2)));
			});
		}
		return this;
	}

	public SimInstallData WithNewPackage(TrainerPackageIdentifier packageId)
	{
		TrainerPackageIdentifier packageId2 = packageId;
		if (packageId2 != FullPackageId)
		{
			return this.With(delegate(IPropertyChangeSet<SimInstallData> copy)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Expected O, but got Unknown
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Expected O, but got Unknown
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Expected O, but got Unknown
				ParameterExpression val = Expression.Parameter(typeof(SimInstallData), "c");
				ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi = copy.Mutate(Expression.Lambda<Func<SimInstallData, SimAircraftKey>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), new SimAircraftKey(AircraftSimKey.AircraftIdentifier, InstallationKey.WithNewRelease(packageId2.EmbeddedReleaseCompositeId)));
				val = Expression.Parameter(typeof(SimInstallData), "c");
				ISubsequentPropertyChangeSetApi<SimInstallData> subsequentPropertyChangeSetApi2 = subsequentPropertyChangeSetApi.AndMutate(Expression.Lambda<Func<SimInstallData, TrainerPackageIdentifier>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), packageId2);
				val = Expression.Parameter(typeof(SimInstallData), "c");
				subsequentPropertyChangeSetApi2.AndMutate<ImmutableDictionary<SettingIdentifier, string>>(Expression.Lambda<Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>>((Expression)(object)Expression.Property((Expression)(object)val, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), (ParameterExpression[])(object)new ParameterExpression[1] { val }), (Func<SimInstallData, ImmutableDictionary<SettingIdentifier, string>>)((SimInstallData c) => ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>(Enumerable.Select<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>((System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>)c.Settings, (Func<KeyValuePair<SettingIdentifier, string>, KeyValuePair<SettingIdentifier, string>>)((KeyValuePair<SettingIdentifier, string> setting) => setting.ToUpdatedPackageReference(packageId2))))));
			});
		}
		return this;
	}

	public SimInstallData ShallowCopy()
	{
		return RecordUtil.MemberwiseClone(this);
	}

	public void ThrowIfConstraintsAreViolated(SimInstallData record)
	{
		ThisClassesValidation(record);
	}

	private static void ThisClassesValidation(SimInstallData copy)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if ((object)copy.AircraftSimKey == null)
		{
			throw new ArgumentNullException("copy", "AircraftSimKey");
		}
		if (copy.FullPackageId != null && copy.FullPackageId.EmbeddedReleaseCompositeId != copy.AircraftSimKey.InstallationKey.ReleaseId)
		{
			throw new ArgumentException("Embedded release ids don't match: " + (string)copy.FullPackageId.EmbeddedReleaseCompositeId + " vs " + (string)copy.AircraftSimKey.InstallationKey.ReleaseId);
		}
	}

	private static ImmutableDictionary<SettingIdentifier, string> GetSettings(System.Collections.Generic.IEnumerable<KeyValuePair<SettingIdentifier, string>>? settings)
	{
		if (settings == null)
		{
			return ImmutableDictionary<SettingIdentifier, string>.Empty;
		}
		if (settings is ImmutableDictionary<SettingIdentifier, string> result)
		{
			return result;
		}
		return ImmutableDictionary.ToImmutableDictionary<SettingIdentifier, string>(settings);
	}
}
