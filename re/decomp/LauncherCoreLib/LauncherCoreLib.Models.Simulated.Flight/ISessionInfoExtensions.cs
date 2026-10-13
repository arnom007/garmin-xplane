using System;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class ISessionInfoExtensions
{
	public static FlightSessionRequestInfo CreationRequestOrThrow(this ISessionInfo sessionInfo)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return sessionInfo.CreationRequest ?? throw new InvalidOperationException("CreationRequest is required by now.");
	}
}
