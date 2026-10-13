namespace LauncherCoreLib.Models.Simulated.Airport;

public interface IForkByAircraftIdentifier<out T>
{
	T WithNewIdentifier(AircraftIdentifier aircraftIdentifier);
}
