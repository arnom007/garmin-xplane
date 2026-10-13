using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Winfs;

public interface IWinfsTransformProcessor
{
	System.Threading.Tasks.Task<WinfsTransformReport> ProcessRequest(WinfsTransformRequest request);
}
