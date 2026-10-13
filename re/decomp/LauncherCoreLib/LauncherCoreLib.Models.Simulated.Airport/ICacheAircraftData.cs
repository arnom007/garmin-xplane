namespace LauncherCoreLib.Models.Simulated.Airport;

public interface ICacheAircraftData
{
	void InvalidateAircraftData(AircraftIdentifier aircraftIdentifier);
}
