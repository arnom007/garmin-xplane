using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LOLA.Logging;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class ConfigureRoarWrapper : IRouterConfigureCommands
{
	private readonly IRoarCommandConnection _connection;

	private readonly ConcurrentDictionary<string, RoarUdpEndpoint> _knownEndpoints = new ConcurrentDictionary<string, RoarUdpEndpoint>();

	public System.Collections.Generic.IEnumerable<RoarUdpEndpoint> KnownEndpoints => Enumerable.Select<KeyValuePair<string, RoarUdpEndpoint>, RoarUdpEndpoint>((System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarUdpEndpoint>>)_knownEndpoints, (Func<KeyValuePair<string, RoarUdpEndpoint>, RoarUdpEndpoint>)((KeyValuePair<string, RoarUdpEndpoint> kv) => kv.Value));

	public ConfigureRoarWrapper(IRoarCommandConnection roarCommandConnection)
	{
		_connection = roarCommandConnection;
	}

	public bool AddRoute(ROAR_prtcl_fmt_type format, RoutePoint source, RoutePoint destination, bool createReverseRoute = false)
	{
		IRoarResult roarResult = _connection.AddRoute(source, destination, format, createReverseRoute);
		if (!roarResult.IsTypicalSuccess())
		{
			string text = "--->";
			if (createReverseRoute)
			{
				text = "<-->";
			}
			Logger.LogFormattedException(roarResult.BuildException(), "[Format: {0, -20}] {1} {2} {3}", ((object)format).ToString(), FormatRoutePoint(source), text, FormatRoutePoint(destination));
			return false;
		}
		return true;
	}

	public RoarUdpEndpoint GetOrCreateUdpEndpoint(string endpointName)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		RoarUdpEndpoint udp = _connection.GetOrAddUdpEndpoint(endpointName, 0, 0);
		if (udp != null)
		{
			return _knownEndpoints.AddOrUpdate(endpointName, udp, (Func<string, RoarUdpEndpoint, RoarUdpEndpoint>)((string _, RoarUdpEndpoint __) => udp));
		}
		throw new InvalidOperationException("Could not create a UDP endpoint with a name of '" + endpointName + "'.");
	}

	public bool IsEndpointNameTaken(string endpointName)
	{
		return _knownEndpoints.ContainsKey(endpointName);
	}

	public bool StartRouting()
	{
		_connection.StartRouting();
		return true;
	}

	private static string FormatRoutePoint(RoutePoint routePoint)
	{
		return routePoint.Endpoint.Name + " - " + routePoint.Channel;
	}
}
