using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public sealed class SessionData
{
	[field: CompilerGenerated]
	public IImmutableDictionary<string, string> EnvironmentSettings
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IImmutableDictionary<TrainerIdentifier, TrainerSessionData> TrainerData
	{
		[CompilerGenerated]
		get;
	}

	public SessionData()
		: this(new SessionDataBuilder())
	{
	}

	public SessionData(SessionDataBuilder copythis)
	{
		EnvironmentSettings = copythis.EnvironmentSettings;
		TrainerData = copythis.TrainerData;
	}

	public SessionDataBuilder ToBuilder()
	{
		return new SessionDataBuilder(this);
	}
}
