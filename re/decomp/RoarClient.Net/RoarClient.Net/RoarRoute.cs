using System;
using RoarClient.Net.Native;

namespace RoarClient.Net;

public sealed class RoarRoute
{
	public RoutePoint Source { get; }

	public RoutePoint Destination { get; }

	public ROAR_prtcl_fmt_type Format { get; }

	public RoarRoute(RoutePoint source, RoutePoint destination, ROAR_prtcl_fmt_type format)
	{
		Source = source ?? throw new ArgumentNullException("source");
		Destination = destination;
		Format = format;
	}

	public static RoarRoute Create(RoutePoint source, RoutePoint destination, ROAR_prtcl_fmt_type format)
	{
		return new RoarRoute(source, destination, format);
	}

	public ROAR_route_args CreateSourceRouteArgs()
	{
		return Source.CreateRouteArgs();
	}

	public ROAR_route_args CreateDestinationRouteArgs()
	{
		return Destination.CreateRouteArgs();
	}

	public RoarRoute ToReverseRoute()
	{
		return new RoarRoute(Destination, Source, Format);
	}
}
