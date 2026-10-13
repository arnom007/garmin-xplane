using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class SessionDataBuilder
{
	[field: CompilerGenerated]
	public IImmutableDictionary<string, string> EnvironmentSettings
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public IImmutableDictionary<TrainerIdentifier, TrainerSessionData> TrainerData
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	public SessionDataBuilder()
	{
		EnvironmentSettings = (IImmutableDictionary<string, string>)(object)ImmutableDictionary<string, string>.Empty;
		TrainerData = (IImmutableDictionary<TrainerIdentifier, TrainerSessionData>)(object)ImmutableDictionary<TrainerIdentifier, TrainerSessionData>.Empty;
	}

	public SessionDataBuilder(SessionData copyThis)
	{
		EnvironmentSettings = copyThis.EnvironmentSettings;
		TrainerData = copyThis.TrainerData;
	}

	public SessionData ToImmutable()
	{
		return new SessionData(this);
	}
}
