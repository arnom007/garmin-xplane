using System;
using System.Collections.Generic;

namespace DataRouterMatrix;

public class EnumerableAssemblyProvider : IDataRouterAssemblyProvider
{
	private readonly System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo> _assemblyEnumerable;

	public EnumerableAssemblyProvider(System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo> assemblyEnumerable)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_assemblyEnumerable = assemblyEnumerable ?? throw new ArgumentNullException("assemblyEnumerable");
	}

	public System.Collections.Generic.IEnumerable<DataRouterAssemblyInfo> GetAll()
	{
		return _assemblyEnumerable;
	}
}
