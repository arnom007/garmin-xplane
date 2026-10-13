using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IAutomaticShutdownStrategy
{
	System.Threading.Tasks.Task AutomaticShutdownEvent(ITrainingSession session);
}
