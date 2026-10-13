using System.Threading.Tasks;

namespace LauncherCoreLib.Models.Winfs;

public interface ISourcedBatchFilter
{
	System.Threading.Tasks.Task<bool> Keep(SourcedBatchTransform transform);

	System.Threading.Tasks.Task<bool> ThrowAway(SourcedBatchTransform transform);
}
