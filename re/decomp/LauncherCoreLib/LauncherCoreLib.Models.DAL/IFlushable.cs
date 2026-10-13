using System.Threading.Tasks;

namespace LauncherCoreLib.Models.DAL;

public interface IFlushable
{
	System.Threading.Tasks.Task Flush();
}
