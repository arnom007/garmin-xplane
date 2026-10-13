using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace LauncherCoreLib.Models.Simulated.Flight.Serialized;

public sealed class RoarUdpEndpointConfig
{
	[JsonProperty("type")]
	[field: CompilerGenerated]
	public string EndpointType
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = "UDPEndPoint";


	[JsonProperty("src_addr")]
	[field: CompilerGenerated]
	public string ClientAddress
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("src_port")]
	[field: CompilerGenerated]
	public int ClientPort
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[JsonProperty("dest_addr")]
	[field: CompilerGenerated]
	public string MasterAddress
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("dest_port")]
	[field: CompilerGenerated]
	public int MasterPort
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[JsonProperty("exclusive")]
	[field: CompilerGenerated]
	public bool IsExclusive
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}
}
