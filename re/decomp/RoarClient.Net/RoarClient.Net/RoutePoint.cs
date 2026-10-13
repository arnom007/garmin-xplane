using System;
using System.Collections.Generic;
using RoarClient.Net.Native;

namespace RoarClient.Net;

public sealed class RoutePoint : IEquatable<RoutePoint>
{
	public RoarEndpoint Endpoint { get; }

	public int Channel { get; }

	private RoutePoint(RoarEndpoint endpoint, int channel)
	{
		Endpoint = endpoint ?? throw new ArgumentNullException("endpoint");
		Channel = channel;
	}

	public static RoutePoint Create(RoarEndpoint endpoint, int channel)
	{
		return new RoutePoint(endpoint, channel);
	}

	public ROAR_route_args CreateRouteArgs()
	{
		return new ROAR_route_args(Endpoint.Name, Channel);
	}

	public bool Equals(RoutePoint other)
	{
		if (this != other)
		{
			if (other != null && EqualityComparer<RoarEndpoint>.Default.Equals(Endpoint, other.Endpoint))
			{
				return Channel == other.Channel;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (obj is RoutePoint other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Endpoint.GetHashCode() + Channel;
	}
}
