using System;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface ISessionInfo
{
	int SessionIndex { get; }

	string SessionPathRoot { get; }

	string SessionJsonPath { get; }

	DateTimeOffset StartedAt { get; }

	AirframeCategoryClassType AirframeCCT { get; }

	string ConfigurationName { get; }

	FlightSessionRequestInfo? CreationRequest { get; }
}
