using System.Collections.Generic;

namespace DataRouterMatrix;

public interface IDataRouterAssemblyProvider
{
	System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo> GetAll();
}
