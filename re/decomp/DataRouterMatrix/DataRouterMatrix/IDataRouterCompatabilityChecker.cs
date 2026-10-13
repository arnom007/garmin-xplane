using System;
using System.Threading.Tasks;

namespace DataRouterMatrix;

public interface IDataRouterCompatabilityChecker
{
	System.Threading.Tasks.Task<Compatibility> GetClientCompatibility(Version clientVersion);
}
