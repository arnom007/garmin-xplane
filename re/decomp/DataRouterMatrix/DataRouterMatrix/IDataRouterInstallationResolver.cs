using System;
using System.Collections.Generic;

namespace DataRouterMatrix;

public interface IDataRouterInstallationResolver
{
	ValueTuple<DataRouterAssemblyInfo, DataRouterAssemblyInfo> ResolveBestRouterAssemblies(System.Collections.Generic.IEnumerable<Version> trainerClientVersions);
}
