using System;
using System.Runtime.CompilerServices;

namespace DataRouterMatrix;

public class ConstantDataRouterVersionLookupProvider : IDataRouterVersionLookupProvider
{
	[field: CompilerGenerated]
	public DataRouterVersionLookup Lookup
	{
		[CompilerGenerated]
		get;
	}

	public ConstantDataRouterVersionLookupProvider(DataRouterVersionLookup lookup)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Lookup = lookup ?? throw new ArgumentNullException("lookup");
	}
}
