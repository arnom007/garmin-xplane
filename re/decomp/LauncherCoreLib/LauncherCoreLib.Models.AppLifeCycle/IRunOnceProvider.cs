using System.Threading.Tasks;

namespace LauncherCoreLib.Models.AppLifeCycle;

public interface IRunOnceProvider
{
	System.Threading.Tasks.Task<IRunOnce> GetByOperationName(string operationName);

	IRunOnce GetByOperationNameSync(string operationName);
}
