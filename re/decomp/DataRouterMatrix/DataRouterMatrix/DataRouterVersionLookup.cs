using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataRouterMatrix;

public class DataRouterVersionLookup
{
	[field: CompilerGenerated]
	public VersionLookupFormat Format
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public VersionRangeSelector<Version> Lookup
	{
		[CompilerGenerated]
		get;
	}

	public DataRouterVersionLookup(VersionLookupFormat format, VersionRangeSelector<Version> lookup)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Format = format;
		Lookup = lookup ?? throw new ArgumentNullException("lookup");
	}

	public static DataRouterVersionLookup Empty()
	{
		return new DataRouterVersionLookup(VersionLookupFormat.RouterBased, new VersionRangeSelector<Version>(Enumerable.Empty<KeyValuePair<Version, string>>()));
	}
}
