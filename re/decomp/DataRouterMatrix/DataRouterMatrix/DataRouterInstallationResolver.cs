using System;
using System.Collections.Generic;
using System.Linq;

namespace DataRouterMatrix;

public class DataRouterInstallationResolver : IDataRouterInstallationResolver
{
	private readonly IDataRouterAssemblyTypes _routerTypes;

	private readonly IDataRouterVersionResolver _versionResolver;

	public DataRouterInstallationResolver(IDataRouterAssemblyTypes dataRouterTypes, IDataRouterVersionResolver versionResolver)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_routerTypes = dataRouterTypes ?? throw new ArgumentNullException("dataRouterTypes");
		_versionResolver = versionResolver ?? throw new ArgumentNullException("versionResolver");
	}

	public ValueTuple<DataRouterAssemblyInfo, DataRouterAssemblyInfo> ResolveBestRouterAssemblies(System.Collections.Generic.IEnumerable<Version> trainerClientVersions)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		List<Version> val = Enumerable.ToList<Version>(trainerClientVersions);
		if (val.Count == 0)
		{
			throw new ArgumentException(Resources.NoTrainerClientVersionsToCheck);
		}
		Dictionary<Version, DataRouterAssemblyInfo> val2 = Enumerable.ToDictionary<DataRouterAssemblyInfo, Version>(_routerTypes.Routers.GetAll(), (Func<DataRouterAssemblyInfo, Version>)((DataRouterAssemblyInfo av) => av.NativeVersion));
		Dictionary<Version, DataRouterAssemblyInfo> val3 = Enumerable.ToDictionary<DataRouterAssemblyInfo, Version>(_routerTypes.Clients.GetAll(), (Func<DataRouterAssemblyInfo, Version>)((DataRouterAssemblyInfo av) => av.NativeVersion));
		ValueTuple<Version, Version> val4 = _versionResolver.ChooseNewestAssemblies((System.Collections.Generic.IReadOnlyCollection<Version>)val, (System.Collections.Generic.IReadOnlyCollection<Version>)val2.Keys, (System.Collections.Generic.IReadOnlyCollection<Version>)val3.Keys);
		Version item = val4.Item1;
		Version item2 = val4.Item2;
		DataRouterAssemblyInfo dataRouterAssemblyInfo = default(DataRouterAssemblyInfo);
		DataRouterAssemblyInfo dataRouterAssemblyInfo2 = default(DataRouterAssemblyInfo);
		if (val2.TryGetValue(item, ref dataRouterAssemblyInfo) && val3.TryGetValue(item2, ref dataRouterAssemblyInfo2))
		{
			return new ValueTuple<DataRouterAssemblyInfo, DataRouterAssemblyInfo>(dataRouterAssemblyInfo, dataRouterAssemblyInfo2);
		}
		throw new InvalidOperationException(Resources.DataRouterClientResolutionInternalError);
	}
}
