using System;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Airport;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

public readonly struct AircraftPackageFamily : IEquatable<AircraftPackageFamily>, IHaveAircraftId
{
	[field: CompilerGenerated]
	public AircraftIdentifier AircraftIdentifier
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public TrainerEmbeddedReleaseId ReleaseId
	{
		[CompilerGenerated]
		get;
	}

	public AircraftPackageFamily(AircraftIdentifier aircraft, TrainerEmbeddedReleaseId releaseId)
	{
		this = default(AircraftPackageFamily);
		AircraftIdentifier = aircraft;
		ReleaseId = releaseId;
	}

	public static bool operator ==(AircraftPackageFamily lhs, AircraftPackageFamily rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(AircraftPackageFamily lhs, AircraftPackageFamily rhs)
	{
		return !lhs.Equals(rhs);
	}

	public bool Equals(AircraftPackageFamily other)
	{
		if (AircraftIdentifier.Equals(other.AircraftIdentifier))
		{
			return ReleaseId == other.ReleaseId;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is AircraftPackageFamily other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<AircraftIdentifier, TrainerEmbeddedReleaseId>(AircraftIdentifier, ReleaseId);
	}
}
