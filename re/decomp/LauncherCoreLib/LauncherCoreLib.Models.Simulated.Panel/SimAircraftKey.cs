using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Airport;

namespace LauncherCoreLib.Models.Simulated.Panel;

public sealed class SimAircraftKey : IHaveAircraftId, IForkByAircraftIdentifier<SimAircraftKey>, IEquatable<SimAircraftKey>
{
	[field: CompilerGenerated]
	public AircraftIdentifier AircraftIdentifier
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		private set;
	}

	[field: CompilerGenerated]
	public SimInstallKey InstallationKey
	{
		[CompilerGenerated]
		get;
	}

	public SimAircraftKey(AircraftIdentifier aircraftIdentifier, SimInstallKey simInstallKey)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		AircraftIdentifier = aircraftIdentifier;
		InstallationKey = simInstallKey ?? throw new ArgumentNullException("simInstallKey");
	}

	public static bool operator ==(SimAircraftKey? lhs, SimAircraftKey? rhs)
	{
		return EqualityComparer<SimAircraftKey>.Default.Equals(lhs, rhs);
	}

	public static bool operator !=(SimAircraftKey? lhs, SimAircraftKey? rhs)
	{
		return !EqualityComparer<SimAircraftKey>.Default.Equals(lhs, rhs);
	}

	public bool Equals(SimAircraftKey? other)
	{
		if ((object)this != other)
		{
			if (other != null && AircraftIdentifier == other.AircraftIdentifier)
			{
				return InstallationKey == other.InstallationKey;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SimAircraftKey)
		{
			return Equals((SimAircraftKey)obj);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)AircraftIdentifier).GetHashCode() ^ ((object)InstallationKey).GetHashCode();
	}

	public override string ToString()
	{
		return AircraftIdentifier.IdentString + "-" + InstallationKey.SerializedKey;
	}

	public SimAircraftKey WithNewIdentifier(AircraftIdentifier aircraftIdentifier)
	{
		if (aircraftIdentifier != AircraftIdentifier)
		{
			return new SimAircraftKey(aircraftIdentifier, InstallationKey);
		}
		return this;
	}

	public void ThrowIfInvalid(System.Exception? error = null)
	{
		AircraftIdentifier.ThrowIfDefault(error);
	}
}
