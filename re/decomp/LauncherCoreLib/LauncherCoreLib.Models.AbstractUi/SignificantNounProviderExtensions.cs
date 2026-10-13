using System.Collections.Generic;

namespace LauncherCoreLib.Models.AbstractUi;

public static class SignificantNounProviderExtensions
{
	private const string TRAINER_KEY = "TRAINER";

	private const string LAUNCHER_KEY = "LAUNCHER";

	private const string AIRCRAFT_KEY = "AIRCRAFT";

	public static SignificantNoun GetLauncherNoun(this ISignificantNounProvider provider)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return provider.GetNounByKeyOrNull("LAUNCHER") ?? throw new KeyNotFoundException("Launcher noun.");
	}

	public static SignificantNoun GetTrainerNoun(this ISignificantNounProvider provider)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return provider.GetNounByKeyOrNull("TRAINER") ?? throw new KeyNotFoundException("Trainer noun.");
	}

	public static SignificantNoun GetAircraftNoun(this ISignificantNounProvider provider)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return provider.GetNounByKeyOrNull("AIRCRAFT") ?? throw new KeyNotFoundException("Aircraft noun.");
	}

	public static SignificantNounProvider SetLauncherNoun(this SignificantNounProvider provider, SignificantNoun sigNoun)
	{
		return provider.SetNoun("LAUNCHER", sigNoun);
	}

	public static SignificantNounProvider SetTrainerNoun(this SignificantNounProvider provider, SignificantNoun sigNoun)
	{
		return provider.SetNoun("TRAINER", sigNoun);
	}

	public static SignificantNounProvider SetAircraftNoun(this SignificantNounProvider provider, SignificantNoun sigNoun)
	{
		return provider.SetNoun("AIRCRAFT", sigNoun);
	}
}
