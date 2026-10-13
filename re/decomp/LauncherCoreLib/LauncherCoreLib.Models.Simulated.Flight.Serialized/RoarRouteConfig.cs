using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using RoarClient.Net;
using RoarClient.Net.Native;

namespace LauncherCoreLib.Models.Simulated.Flight.Serialized;

public sealed class RoarRouteConfig : IEquatable<RoarRouteConfig>
{
	[JsonProperty("format")]
	[field: CompilerGenerated]
	public string Format
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = ((object)ROAR_prtcl_fmt_type.ROAR_SIM_NO_FMT).ToString().Substring(5);


	[JsonIgnore]
	public ROAR_prtcl_fmt_type ActualFormat
	{
		get
		{
			return (ROAR_prtcl_fmt_type)System.Enum.Parse(typeof(ROAR_prtcl_fmt_type), "ROAR_" + Format);
		}
		set
		{
			Format = ((object)value).ToString().Substring(5);
		}
	}

	[JsonProperty("rx_ep")]
	[field: CompilerGenerated]
	public string SourceEndpoint
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("rx_chnl")]
	[field: CompilerGenerated]
	public int SourceChannel
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[JsonIgnore]
	public ROAR_rte_chnl_type NativeSourceChannel
	{
		get
		{
			return Convert.ToUInt16(SourceChannel);
		}
		set
		{
			SourceChannel = (ushort)value;
		}
	}

	[JsonProperty("tx_ep")]
	[field: CompilerGenerated]
	public string DestinationEndpoint
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	[JsonProperty("tx_chnl")]
	[field: CompilerGenerated]
	public int DestinationChannel
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[JsonIgnore]
	public ROAR_rte_chnl_type NativeDestinationChannel
	{
		get
		{
			return Convert.ToUInt16(DestinationChannel);
		}
		set
		{
			DestinationChannel = (ushort)value;
		}
	}

	public static RoarRouteConfig CreateReverseRoute(RoarRouteConfig config)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
		return new RoarRouteConfig
		{
			ActualFormat = config.ActualFormat,
			DestinationChannel = config.SourceChannel,
			DestinationEndpoint = config.SourceEndpoint,
			SourceChannel = config.DestinationChannel,
			SourceEndpoint = config.DestinationEndpoint
		};
	}

	public static RoarRouteConfig CopyRoute(RoarRouteConfig config)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (config == null)
		{
			throw new ArgumentNullException("config");
		}
		return new RoarRouteConfig
		{
			ActualFormat = config.ActualFormat,
			SourceChannel = config.SourceChannel,
			SourceEndpoint = config.SourceEndpoint,
			DestinationChannel = config.DestinationChannel,
			DestinationEndpoint = config.DestinationEndpoint
		};
	}

	public RoarRouteConfig CreateCopy()
	{
		return CopyRoute(this);
	}

	public RoarRouteConfig ReverseRoute()
	{
		return CreateReverseRoute(this);
	}

	public override string ToString()
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0}: {1} -> {2}", (object)Format, (object)Combine(SourceEndpoint, SourceChannel), (object)Combine(DestinationEndpoint, DestinationChannel));
	}

	public bool Equals(RoarRouteConfig? other)
	{
		if (this != other)
		{
			if (other != null && Format == other.Format && SourceEndpoint == other.SourceEndpoint && SourceChannel == other.SourceChannel && DestinationEndpoint == other.DestinationEndpoint)
			{
				return DestinationChannel == other.DestinationChannel;
			}
			return false;
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is RoarRouteConfig other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine<string, int, int>(Format, SourceChannel, DestinationChannel);
	}

	private static string Combine(string destinationEndpoint, int destinationChannel)
	{
		return destinationEndpoint + ":" + destinationChannel.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture);
	}
}
