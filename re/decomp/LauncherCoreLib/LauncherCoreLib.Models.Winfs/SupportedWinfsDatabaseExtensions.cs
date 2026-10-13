using TrainerPackageLib;

namespace LauncherCoreLib.Models.Winfs;

public static class SupportedWinfsDatabaseExtensions
{
	public static bool IsNavigationDatabase(this SupportedWinfsDatabase database)
	{
		if (database != SupportedWinfsDatabase.JEPP_NAV && database != SupportedWinfsDatabase.GARMIN_NAV && database != SupportedWinfsDatabase.JEPP_ROTOR_NAV)
		{
			return database == SupportedWinfsDatabase.GARMIN_ROTOR_NAV;
		}
		return true;
	}

	public static bool IsObstacleDatabase(this SupportedWinfsDatabase database)
	{
		if (database != SupportedWinfsDatabase.FIXED_OBSTACLE)
		{
			return database == SupportedWinfsDatabase.ROTOR_OBSTACLE;
		}
		return true;
	}

	public static bool ObstacleDatabaseMatchesAirframe(this SupportedWinfsDatabase database, AirframeCategoryClassType airframeCategoryClassType)
	{
		return ((airframeCategoryClassType == AirframeCategoryClassType.Rotorcraft) ? 6 : 5) == (int)database;
	}
}
