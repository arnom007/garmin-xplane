using System;
using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal sealed class SessionInfo : ISessionInfo, IEquatable<SessionInfo>
{
	[field: CompilerGenerated]
	public int SessionIndex
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public string SessionPathRoot
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public string SessionJsonPath
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public DateTimeOffset StartedAt
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public AirframeCategoryClassType AirframeCCT
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public string ConfigurationName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[field: CompilerGenerated]
	public FlightSessionRequestInfo? CreationRequest
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public bool Equals(SessionInfo? other)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (this != other)
		{
			if (other != null && SessionIndex == other.SessionIndex && SessionPathRoot == other.SessionPathRoot && SessionJsonPath == other.SessionJsonPath && StartedAt == other.StartedAt && AirframeCCT == other.AirframeCCT && ConfigurationName == other.ConfigurationName)
			{
				return CreationRequest == other.CreationRequest;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is SessionInfo other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return SessionIndex.GetHashCode();
	}
}
