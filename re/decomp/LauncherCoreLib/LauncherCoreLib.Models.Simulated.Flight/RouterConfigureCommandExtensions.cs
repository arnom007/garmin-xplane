using System;
using System.Collections.Generic;
using System.Linq;
using LauncherCoreLib.Models.Simulated.Flight.Serialized;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class RouterConfigureCommandExtensions
{
	public static RoarRouteConfig[] LoadConfigurationAndReturnBrokenRoutes(this IRouterConfigureCommands commands, RoarUdpOnlyConfiguration roarConfig)
	{
		IRouterConfigureCommands commands2 = commands;
		Dictionary<string, RoarUdpEndpoint> endpoints = Enumerable.ToDictionary<RoarUdpEndpoint, string>(Enumerable.Select<string, RoarUdpEndpoint>((System.Collections.Generic.IEnumerable<string>)roarConfig.Endpoints.Keys, (Func<string, RoarUdpEndpoint>)((string udpEndpointName) => commands2.GetOrCreateUdpEndpoint(udpEndpointName))), (Func<RoarUdpEndpoint, string>)((RoarUdpEndpoint ep) => ep.Name));
		return Enumerable.ToArray<RoarRouteConfig>(Enumerable.Select<ValueTuple<RoarRouteConfig, bool>, RoarRouteConfig>(Enumerable.Where<ValueTuple<RoarRouteConfig, bool>>(Enumerable.Select<RoarRouteConfig, ValueTuple<RoarRouteConfig, bool>>((System.Collections.Generic.IEnumerable<RoarRouteConfig>)roarConfig.Routes.Values, (Func<RoarRouteConfig, ValueTuple<RoarRouteConfig, bool>>)((RoarRouteConfig rr) => new ValueTuple<RoarRouteConfig, bool>(rr, commands2.AddRoute(rr.ActualFormat, endpoints[rr.SourceEndpoint].CreateRoutePoint(rr.SourceChannel), endpoints[rr.DestinationEndpoint].CreateRoutePoint(rr.DestinationChannel))))), (Func<ValueTuple<RoarRouteConfig, bool>, bool>)((ValueTuple<RoarRouteConfig, bool> result) => !result.Item2)), (Func<ValueTuple<RoarRouteConfig, bool>, RoarRouteConfig>)((ValueTuple<RoarRouteConfig, bool> result) => result.Item1)));
	}
}
