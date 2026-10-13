using System;

namespace RoarClient.Net;

public static class RoarCommandConnectionExtensions
{
	public static IRoarResult AddRoute(this IRoarCommandConnection connection, RoutePoint source, RoutePoint destination, ROAR_prtcl_fmt_type format, bool two_way = false)
	{
		if (connection == null)
		{
			throw new ArgumentNullException("connection");
		}
		RoarRoute roarRoute = RoarRoute.Create(source, destination, format);
		IRoarResult result = connection.AddRoute(roarRoute);
		if (result.IsTypicalSuccess() && two_way)
		{
			return connection.AddRoute(roarRoute.ToReverseRoute());
		}
		return result;
	}
}
