using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using LOLA.Extensions;
using LauncherCoreLib.Models.Simulated.Panel;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class TrainerSessionData : IEquatable<TrainerSessionData>
{
	[field: CompilerGenerated]
	public SimInstallData InstallData
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerIdentifier EndpointName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	public InstallationSlot InstallSlot => InstallData.InstallationKey.Slot;

	[field: CompilerGenerated]
	public AvionicsModel PackageInfo
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public string WinfsMountPath
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IImmutableDictionary<string, string> Settings
	{
		[CompilerGenerated]
		get;
	}

	public TrainerSessionData(TrainerSessionDataBuilder copyThis)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		InstallData = copyThis.InstallData ?? throw new ArgumentNullException("copyThis", "InstallData");
		EndpointName = copyThis.EndpointName;
		PackageInfo = copyThis.PackageInfo ?? throw new ArgumentNullException("copyThis", "PackageInfo");
		Settings = (IImmutableDictionary<string, string>)(((object)copyThis.Settings) ?? ((object)ImmutableDictionary<string, string>.Empty));
		WinfsMountPath = copyThis.WinfsMountPath;
	}

	public static TrainerIdentifier SessionDataKeySelector(TrainerSessionData trainerSessionData)
	{
		return trainerSessionData.EndpointName;
	}

	public TrainerSessionDataBuilder ToBuilder()
	{
		return new TrainerSessionDataBuilder(this);
	}

	public bool Equals(TrainerSessionData? other)
	{
		if (this != other)
		{
			if (other != null && EqualityComparer<TrainerIdentifier>.Default.Equals(EndpointName, other.EndpointName) && InstallData == other.InstallData && EqualityComparer<AvionicsModel>.Default.Equals(PackageInfo, other.PackageInfo) && WinfsMountPath == other.WinfsMountPath)
			{
				return ((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)Settings).SetEquals((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)other.Settings);
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TrainerSessionData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)InstallSlot).GetHashCode();
	}
}
