using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace LauncherCoreLib.Models.Simulated.Flight.Serialized;

public sealed class RoarUdpOnlyConfiguration
{
	[JsonProperty("endpoints")]
	[field: CompilerGenerated]
	public Dictionary<string, RoarUdpEndpointConfig> Endpoints
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new Dictionary<string, RoarUdpEndpointConfig>();


	[JsonProperty("routes")]
	[field: CompilerGenerated]
	public Dictionary<string, RoarRouteConfig> Routes
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = new Dictionary<string, RoarRouteConfig>();

}
