using System;

namespace RoarClient.Net;

public abstract class RoarEndpoint
{
	public const int MAX_ENDPOINT_NAME_LENGTH = 30;

	public int Id { get; }

	public string Name { get; }

	protected RoarEndpoint(int id, string name)
	{
		if (!IsEndpointNameValid(name))
		{
			throw new ArgumentException("Not a valid endpoint name.", "name");
		}
		Id = id;
		Name = name;
	}

	public static bool IsEndpointNameValid(string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			return name.Length <= 30;
		}
		return false;
	}

	public virtual RoutePoint CreateRoutePoint(int channel)
	{
		return RoutePoint.Create(this, channel);
	}
}
