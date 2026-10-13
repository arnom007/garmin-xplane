namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IRouterCommandConnectionFactory
{
	IRouterCommandConnection CreateCommandConnection();
}
