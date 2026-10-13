using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Airport;

public static class AircraftIdExtensions
{
	public static bool IsIdEqual(this IHaveAircraftId lhs, IHaveAircraftId rhs)
	{
		return lhs.IsIdEqual(rhs.AircraftIdentifier);
	}

	public static bool IsIdEqual(this IHaveAircraftId lhs, TrainerIdentifier rhs)
	{
		return lhs.AircraftIdentifier == rhs;
	}
}
