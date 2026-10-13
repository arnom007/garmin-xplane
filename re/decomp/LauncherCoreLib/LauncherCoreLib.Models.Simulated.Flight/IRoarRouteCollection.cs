using System;
using System.Collections.Generic;
using LauncherCoreLib.Models.Simulated.Flight.Serialized;

namespace LauncherCoreLib.Models.Simulated.Flight;

public interface IRoarRouteCollection
{
	System.Collections.Generic.IList<RoarRouteConfig> GetRoutes(Func<RoarRouteConfig, bool> predicate);
}
