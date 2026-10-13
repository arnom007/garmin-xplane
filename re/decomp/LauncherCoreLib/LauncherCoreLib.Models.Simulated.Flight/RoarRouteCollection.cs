using System;
using System.Collections.Generic;
using System.Linq;
using LauncherCoreLib.Models.Simulated.Flight.Serialized;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class RoarRouteCollection : IRoarRouteCollection
{
	private readonly System.Collections.Generic.IReadOnlyCollection<RoarRouteConfig> _routes;

	public RoarRouteCollection(System.Collections.Generic.IReadOnlyCollection<RoarRouteConfig> routes)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_routes = routes ?? throw new ArgumentNullException("routes");
	}

	public System.Collections.Generic.IList<RoarRouteConfig> GetRoutes(Func<RoarRouteConfig, bool> predicate)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		return (System.Collections.Generic.IList<RoarRouteConfig>)Enumerable.ToList<RoarRouteConfig>(Enumerable.Select<RoarRouteConfig, RoarRouteConfig>(Enumerable.Where<RoarRouteConfig>((System.Collections.Generic.IEnumerable<RoarRouteConfig>)_routes, predicate), (Func<RoarRouteConfig, RoarRouteConfig>)((RoarRouteConfig config) => config.CreateCopy())));
	}
}
