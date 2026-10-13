using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using LOLA.Extensions;
using LOLA.Logging;
using LauncherCoreLib.Models.Simulated.Flight.Serialized;
using LauncherCoreLib.Models.Simulated.Panel;
using LauncherCoreLib.Utility;
using Newtonsoft.Json.Linq;
using RoarClient.Net;
using TrainerPackageLib;
using TrainerPackageLib.Serialized;

namespace LauncherCoreLib.Models.Simulated.Flight;

internal class DefaultSessionRouteTableFactory : ISessionRouteTableFactory
{
	private sealed class FormatMatching
	{
		[field: CompilerGenerated]
		public TrainerSessionData TrainerData
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public Dictionary<string, int> TxChannels
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public Dictionary<string, int> RxChannels
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		public FormatMatching(TrainerSessionData data, Dictionary<string, int> tx, Dictionary<string, int> rx)
		{
			TrainerData = data;
			TxChannels = tx;
			RxChannels = rx;
		}
	}

	private sealed class HsdbMatching
	{
		[field: CompilerGenerated]
		public TrainerSessionData TrainerData
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		public int NumberOfHsdbPorts
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		public string EndpointName => TrainerData.EndpointName.IdentStringChecked;

		public InstallationSlot InstallSlot => TrainerData.InstallSlot;

		public HsdbMatching(TrainerSessionData tsd)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (!tsd.EndpointName.IsValid)
			{
				throw new ArgumentException("Must have valid endpoint name.", "tsd");
			}
			TrainerData = tsd;
			NumberOfHsdbPorts = GetNumberofHsdbPorts(tsd);
		}

		private static int GetNumberofHsdbPorts(TrainerSessionData td)
		{
			Data_formats dataFormats = GetDataFormats(td);
			JToken jToken = default(JToken);
			if (dataFormats != null && dataFormats.Instances.AdditionalProperties.TryGetValue(td.InstallSlot.InstanceString, ref jToken))
			{
				return (jToken.ToObject<Data_format_directions>() ?? throw CreateNullDataFormatException(td)).HSDB;
			}
			return 0;
		}
	}

	[DefaultMember("Item")]
	private sealed class HsdbNode
	{
		[CompilerGenerated]
		private sealed class _003CCreateTwoWayHSDBRoute_003Ed__16 : System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarRouteConfig>>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<KeyValuePair<string, RoarRouteConfig>>, System.Collections.IEnumerator, System.IDisposable
		{
			private int _003C_003E1__state;

			private KeyValuePair<string, RoarRouteConfig> _003C_003E2__current;

			private int _003C_003El__initialThreadId;

			private HsdbNode source;

			public HsdbNode _003C_003E3__source;

			private HsdbNode destination;

			public HsdbNode _003C_003E3__destination;

			KeyValuePair<string, RoarRouteConfig> System.Collections.Generic.IEnumerator<KeyValuePair<string, RoarRouteConfig>>.Current
			{
				[DebuggerHidden]
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return _003C_003E2__current;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return _003C_003E2__current;
				}
			}

			[DebuggerHidden]
			public _003CCreateTwoWayHSDBRoute_003Ed__16(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
				_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
			}

			[DebuggerHidden]
			void System.IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E2__current = CreateHSDBRoute(source, destination);
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					_003C_003E2__current = CreateHSDBRoute(destination, source);
					_003C_003E1__state = 2;
					return true;
				case 2:
					_003C_003E1__state = -1;
					return false;
				}
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void System.Collections.IEnumerator.Reset()
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			System.Collections.Generic.IEnumerator<KeyValuePair<string, RoarRouteConfig>> System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarRouteConfig>>.GetEnumerator()
			{
				_003CCreateTwoWayHSDBRoute_003Ed__16 _003CCreateTwoWayHSDBRoute_003Ed__;
				if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
				{
					_003C_003E1__state = 0;
					_003CCreateTwoWayHSDBRoute_003Ed__ = this;
				}
				else
				{
					_003CCreateTwoWayHSDBRoute_003Ed__ = new _003CCreateTwoWayHSDBRoute_003Ed__16(0);
				}
				_003CCreateTwoWayHSDBRoute_003Ed__.source = _003C_003E3__source;
				_003CCreateTwoWayHSDBRoute_003Ed__.destination = _003C_003E3__destination;
				return _003CCreateTwoWayHSDBRoute_003Ed__;
			}

			[DebuggerHidden]
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarRouteConfig>>)this).GetEnumerator();
			}
		}

		private readonly HsdbNode[] _neighbors;

		[field: CompilerGenerated]
		public HsdbMatching HSDBInfo
		{
			[CompilerGenerated]
			get;
		}

		public System.Collections.Generic.IReadOnlyList<HsdbNode> Neighbors => _neighbors;

		public System.Collections.Generic.IEnumerable<KeyValuePair<int, HsdbNode>> ValidNeighbors => GetValidNodes();

		public int NumberOfHsdbPorts => _neighbors.Length;

		public string EndpointName => HSDBInfo.EndpointName;

		public HsdbNode this[int port]
		{
			get
			{
				return _neighbors[port];
			}
			private set
			{
				_neighbors[port] = value;
			}
		}

		public HsdbNode(HsdbMatching hsdbInfo)
		{
			HSDBInfo = hsdbInfo;
			_neighbors = new HsdbNode[HSDBInfo.NumberOfHsdbPorts];
		}

		[IteratorStateMachine(typeof(_003CCreateTwoWayHSDBRoute_003Ed__16))]
		public static System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarRouteConfig>> CreateTwoWayHSDBRoute(HsdbNode source, HsdbNode destination)
		{
			yield return CreateHSDBRoute(source, destination);
			yield return CreateHSDBRoute(destination, source);
		}

		public static System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarRouteConfig>> CreateTwoWayHSDBRoute(Tuple<HsdbNode, HsdbNode> nodeTuple)
		{
			return CreateTwoWayHSDBRoute(nodeTuple.Item1, nodeTuple.Item2);
		}

		public static KeyValuePair<string, RoarRouteConfig> CreateHSDBRoute(HsdbNode source, HsdbNode destination)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			RoarRouteConfig roarRouteConfig = new RoarRouteConfig
			{
				ActualFormat = ROAR_prtcl_fmt_type.ROAR_HSDB_RAW_FMT,
				SourceEndpoint = source.EndpointName,
				SourceChannel = source.PortOfNeighbor(destination),
				DestinationEndpoint = destination.EndpointName,
				DestinationChannel = destination.PortOfNeighbor(source)
			};
			if (roarRouteConfig.SourceChannel == -1 || roarRouteConfig.DestinationChannel == -1)
			{
				throw new InvalidOperationException("HSDB nodes must be connected to create a data route between them. Invalid Route: " + ((object)roarRouteConfig).ToString());
			}
			return new KeyValuePair<string, RoarRouteConfig>(((object)roarRouteConfig).ToString(), roarRouteConfig);
		}

		public static KeyValuePair<string, RoarRouteConfig> CreateHSDBRoute(Tuple<HsdbNode, HsdbNode> nodeTuple)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return CreateHSDBRoute(nodeTuple.Item1, nodeTuple.Item2);
		}

		public static bool IsValidPort(HsdbNode neighbor, int port)
		{
			if (port >= 0)
			{
				return port < neighbor.HSDBInfo.NumberOfHsdbPorts;
			}
			return false;
		}

		public static bool IsValidNode(HsdbNode? hsdbNode)
		{
			return hsdbNode != null;
		}

		public int PortOfNeighbor(HsdbNode neighbor)
		{
			return System.Array.IndexOf<HsdbNode>(_neighbors, neighbor);
		}

		public Tuple<HsdbNode, HsdbNode> ConnectToNode(HsdbNode neighbor)
		{
			ThrowIfSameNode(neighbor);
			int num = NextOpenPort();
			int num2 = neighbor.NextOpenPort();
			if (!IsValidPort(num))
			{
				ThrowIfOutOfPorts(this);
			}
			if (!neighbor.IsValidPort(num2))
			{
				ThrowIfOutOfPorts(neighbor);
			}
			return ConnectValidatedNodes(neighbor, num, num2);
		}

		public Tuple<HsdbNode, HsdbNode> ConnectToNode(HsdbNode neighbor, int localPort, int remotePort, bool force = false)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			ThrowIfSameNode(neighbor);
			if (!IsValidPort(this, localPort))
			{
				throw new ArgumentOutOfRangeException("localPort", (object)localPort, "Invalid local port for " + GetEndpointName(this));
			}
			if (!IsValidPort(neighbor, remotePort))
			{
				throw new ArgumentOutOfRangeException("remotePort", (object)remotePort, "Invalid remote port for " + GetEndpointName(this));
			}
			if (!force)
			{
				if (IsValidNode(this[localPort]))
				{
					throw new InvalidOperationException("Local port " + localPort + " already taken on " + GetEndpointName(this));
				}
				if (IsValidNode(neighbor[remotePort]))
				{
					throw new InvalidOperationException("Remote port " + remotePort + "already taken on " + GetEndpointName(neighbor));
				}
			}
			return ConnectValidatedNodes(neighbor, localPort, remotePort);
		}

		public bool IsValidPort(int port)
		{
			return IsValidPort(this, port);
		}

		public int NextOpenPort()
		{
			return System.Array.FindIndex<HsdbNode>(_neighbors, (Predicate<HsdbNode>)((HsdbNode n) => !IsValidNode(n)));
		}

		public string ToString()
		{
			return EndpointName;
		}

		private static void ThrowIfOutOfPorts(HsdbNode neighbor)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			throw new InvalidOperationException(GetEndpointName(neighbor) + " is out of HSDB ports.");
		}

		private static string GetEndpointName(HsdbNode neighbor)
		{
			return neighbor.EndpointName;
		}

		private void ThrowIfSameNode(HsdbNode neighbor)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (this == neighbor)
			{
				throw new InvalidOperationException("Cannot connect a " + GetEndpointName(this) + " to itself. ");
			}
		}

		private System.Collections.Generic.IEnumerable<KeyValuePair<int, HsdbNode>> GetValidNodes()
		{
			return Enumerable.Where<KeyValuePair<int, HsdbNode>>(Enumerable.Select<HsdbNode, KeyValuePair<int, HsdbNode>>((System.Collections.Generic.IEnumerable<HsdbNode>)Neighbors, (Func<HsdbNode, int, KeyValuePair<int, HsdbNode>>)((HsdbNode n, int i) => new KeyValuePair<int, HsdbNode>(i, n))), (Func<KeyValuePair<int, HsdbNode>, bool>)((KeyValuePair<int, HsdbNode> n) => IsValidNode(n.Value)));
		}

		private Tuple<HsdbNode, HsdbNode> ConnectValidatedNodes(HsdbNode neighbor, int localPort, int remotePort)
		{
			this[localPort] = neighbor;
			neighbor[remotePort] = this;
			return Tuple.Create<HsdbNode, HsdbNode>(this, neighbor);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass13_0
	{
		public ROAR_prtcl_fmt_type format;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass13_1
	{
		public FormatMatching trainer;

		public _003C_003Ec__DisplayClass13_0 CS_0024_003C_003E8__locals1;
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass13_2
	{
		public KeyValuePair<string, int> txChannel;

		public _003C_003Ec__DisplayClass13_1 CS_0024_003C_003E8__locals2;

		internal bool _003CMatchUpDataFormats_003Eb__0(FormatMatching td)
		{
			if (td != CS_0024_003C_003E8__locals2.trainer)
			{
				return td.RxChannels.ContainsKey(txChannel.Key);
			}
			return false;
		}

		internal RoarRouteConfig _003CMatchUpDataFormats_003Eb__1(FormatMatching match)
		{
			return new RoarRouteConfig
			{
				ActualFormat = CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.format,
				SourceEndpoint = CS_0024_003C_003E8__locals2.trainer.TrainerData.EndpointName.IdentStringChecked,
				SourceChannel = txChannel.Value,
				DestinationEndpoint = match.TrainerData.EndpointName.IdentStringChecked,
				DestinationChannel = match.RxChannels[txChannel.Key]
			};
		}
	}

	[CompilerGenerated]
	private sealed class _003CMatchUpDataFormats_003Ed__13 : System.Collections.Generic.IEnumerable<RoarRouteConfig>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<RoarRouteConfig>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private RoarRouteConfig _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private ROAR_prtcl_fmt_type format;

		public ROAR_prtcl_fmt_type _003C_003E3__format;

		private List<FormatMatching> trainersWithDataFormats;

		public List<FormatMatching> _003C_003E3__trainersWithDataFormats;

		private _003C_003Ec__DisplayClass13_0 _003C_003E8__1;

		private _003C_003Ec__DisplayClass13_1 _003C_003E8__2;

		private Enumerator<FormatMatching> _003C_003E7__wrap1;

		private Enumerator<string, int> _003C_003E7__wrap2;

		private System.Collections.Generic.IEnumerator<RoarRouteConfig> _003C_003E7__wrap3;

		RoarRouteConfig System.Collections.Generic.IEnumerator<RoarRouteConfig>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CMatchUpDataFormats_003Ed__13(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if ((uint)(num - -5) > 2u && num != 1)
			{
				return;
			}
			try
			{
				if ((uint)(num - -5) > 1u && num != 1)
				{
					return;
				}
				try
				{
					if (num != -5 && num != 1)
					{
						return;
					}
					try
					{
					}
					finally
					{
						_003C_003Em__Finally3();
					}
				}
				finally
				{
					_003C_003Em__Finally2();
				}
			}
			finally
			{
				_003C_003Em__Finally1();
			}
		}

		private bool MoveNext()
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				int num = _003C_003E1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -5;
					goto IL_0148;
				}
				_003C_003E1__state = -1;
				_003C_003E8__1 = new _003C_003Ec__DisplayClass13_0();
				_003C_003E8__1.format = format;
				_003C_003E7__wrap1 = trainersWithDataFormats.GetEnumerator();
				_003C_003E1__state = -3;
				goto IL_01ac;
				IL_01ac:
				if (_003C_003E7__wrap1.MoveNext())
				{
					_003C_003E8__2 = new _003C_003Ec__DisplayClass13_1();
					_003C_003E8__2.CS_0024_003C_003E8__locals1 = _003C_003E8__1;
					_003C_003E8__2.trainer = _003C_003E7__wrap1.Current;
					_003C_003E7__wrap2 = _003C_003E8__2.trainer.TxChannels.GetEnumerator();
					_003C_003E1__state = -4;
					goto IL_0183;
				}
				_003C_003Em__Finally1();
				_003C_003E7__wrap1 = default(Enumerator<FormatMatching>);
				return false;
				IL_0107:
				System.Collections.Generic.IEnumerable<RoarRouteConfig> enumerable;
				_003C_003E7__wrap3 = enumerable.GetEnumerator();
				_003C_003E1__state = -5;
				goto IL_0148;
				IL_0183:
				while (_003C_003E7__wrap2.MoveNext())
				{
					_003C_003Ec__DisplayClass13_2 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass13_2
					{
						CS_0024_003C_003E8__locals2 = _003C_003E8__2,
						txChannel = _003C_003E7__wrap2.Current
					};
					enumerable = Enumerable.Select<FormatMatching, RoarRouteConfig>(Enumerable.Where<FormatMatching>((System.Collections.Generic.IEnumerable<FormatMatching>)trainersWithDataFormats, (Func<FormatMatching, bool>)((FormatMatching td) => td != CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals2.trainer && td.RxChannels.ContainsKey(CS_0024_003C_003E8__locals0.txChannel.Key))), (Func<FormatMatching, RoarRouteConfig>)((FormatMatching match) => new RoarRouteConfig
					{
						ActualFormat = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.format,
						SourceEndpoint = CS_0024_003C_003E8__locals0.CS_0024_003C_003E8__locals2.trainer.TrainerData.EndpointName.IdentStringChecked,
						SourceChannel = CS_0024_003C_003E8__locals0.txChannel.Value,
						DestinationEndpoint = match.TrainerData.EndpointName.IdentStringChecked,
						DestinationChannel = match.RxChannels[CS_0024_003C_003E8__locals0.txChannel.Key]
					}));
					if (!Enumerable.Any<RoarRouteConfig>(enumerable))
					{
						Logger.Log(LogLevel.Debug, "'{0}' was not matched to another trainer.", CS_0024_003C_003E8__locals0.txChannel.Key);
						continue;
					}
					goto IL_0107;
				}
				_003C_003Em__Finally2();
				_003C_003E7__wrap2 = default(Enumerator<string, int>);
				_003C_003E8__2 = null;
				goto IL_01ac;
				IL_0148:
				if (((System.Collections.IEnumerator)_003C_003E7__wrap3).MoveNext())
				{
					RoarRouteConfig current = _003C_003E7__wrap3.Current;
					_003C_003E2__current = current;
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003Em__Finally3();
				_003C_003E7__wrap3 = null;
				goto IL_0183;
			}
			catch
			{
				//try-fault
				((System.IDisposable)this).Dispose();
				throw;
			}
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			((System.IDisposable)_003C_003E7__wrap1).Dispose();
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -3;
			((System.IDisposable)_003C_003E7__wrap2).Dispose();
		}

		private void _003C_003Em__Finally3()
		{
			_003C_003E1__state = -4;
			if (_003C_003E7__wrap3 != null)
			{
				((System.IDisposable)_003C_003E7__wrap3).Dispose();
			}
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<RoarRouteConfig> System.Collections.Generic.IEnumerable<RoarRouteConfig>.GetEnumerator()
		{
			_003CMatchUpDataFormats_003Ed__13 _003CMatchUpDataFormats_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CMatchUpDataFormats_003Ed__ = this;
			}
			else
			{
				_003CMatchUpDataFormats_003Ed__ = new _003CMatchUpDataFormats_003Ed__13(0);
			}
			_003CMatchUpDataFormats_003Ed__.trainersWithDataFormats = _003C_003E3__trainersWithDataFormats;
			_003CMatchUpDataFormats_003Ed__.format = _003C_003E3__format;
			return _003CMatchUpDataFormats_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<RoarRouteConfig>)this).GetEnumerator();
		}
	}

	private static readonly Regex INSTANCED_DATA_FORMAT_PATTERN = new Regex("_{\\d+}$");

	public IReadOnlyJsonDataSource<RoarUdpOnlyConfiguration> CreateRouteTable(ISessionEnvironment sessionEnvironment)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		string text = "Launcher";
		RoarUdpOnlyConfiguration roarUdpOnlyConfiguration = new RoarUdpOnlyConfiguration
		{
			Endpoints = Enumerable.ToDictionary<TrainerSessionData, string, RoarUdpEndpointConfig>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, string>)((TrainerSessionData tsd) => ((object)tsd.EndpointName).ToString()), (Func<TrainerSessionData, RoarUdpEndpointConfig>)((TrainerSessionData tsd) => new RoarUdpEndpointConfig())),
			Routes = Enumerable.Concat<KeyValuePair<string, RoarRouteConfig>>(Enumerable.Concat<KeyValuePair<string, RoarRouteConfig>>(BuildHSDBRoutes(sessionEnvironment), BuildRoutes(sessionEnvironment, ROAR_prtcl_fmt_type.ROAR_A429_FMT)), BuildRoutes(sessionEnvironment, ROAR_prtcl_fmt_type.ROAR_RS232_FMT)).ToDictionary()
		};
		List<KeyValuePair<string, RoarRouteConfig>> val = BuildRdmpRoutes(sessionEnvironment, text);
		if (val.Count > 0)
		{
			roarUdpOnlyConfiguration.Endpoints.Add(text, new RoarUdpEndpointConfig());
			Enumerator<KeyValuePair<string, RoarRouteConfig>> enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, RoarRouteConfig> current = enumerator.Current;
					roarUdpOnlyConfiguration.Routes.Add(current.Key, current.Value);
				}
			}
			finally
			{
				((System.IDisposable)enumerator).Dispose();
			}
		}
		return ReadOnlyJsonDataSource.Create(roarUdpOnlyConfiguration);
	}

	private static Data_formats GetBaseDataFormats(TrainerSessionData td)
	{
		return td.PackageInfo.BaseInfo.Manifest.CopyDataFormats();
	}

	private static Data_formats GetModelDataFormats(TrainerSessionData td)
	{
		return td.PackageInfo.BaseInfo.Manifest.CopyModelDataFormats(td.PackageInfo.ModelInfo.ModelId);
	}

	private static Data_formats ChooseDataFormat(Data_formats baseDataFormats, Data_formats modelDataFormats)
	{
		if (modelDataFormats != null)
		{
			return modelDataFormats;
		}
		return baseDataFormats;
	}

	private static Data_formats? MoveAllIntoInstances(Data_formats startingFormats, int maxInstances)
	{
		if (startingFormats == null || startingFormats.Instances == null)
		{
			return null;
		}
		if (!startingFormats.Instances.AdditionalProperties.ContainsKey("all"))
		{
			return startingFormats;
		}
		System.Collections.Generic.IEnumerable<KeyValuePair<string, JToken>> enumerable = Enumerable.Where<KeyValuePair<string, JToken>>((System.Collections.Generic.IEnumerable<KeyValuePair<string, JToken>>)startingFormats.Instances.AdditionalProperties, (Func<KeyValuePair<string, JToken>, bool>)((KeyValuePair<string, JToken> kv) => Regex.IsMatch(kv.Key, "^\\d+$")));
		Dictionary<string, JToken> val = Enumerable.ToDictionary<KeyValuePair<string, JToken>, string, JToken>(Enumerable.Concat<KeyValuePair<string, JToken>>(enumerable, Enumerable.Select<int, KeyValuePair<string, JToken>>(Enumerable.Except<int>(Enumerable.Range(1, maxInstances), Enumerable.Select<KeyValuePair<string, JToken>, int>(enumerable, (Func<KeyValuePair<string, JToken>, int>)((KeyValuePair<string, JToken> kv) => int.Parse(kv.Key, (IFormatProvider)(object)CultureInfo.InvariantCulture)))), (Func<int, KeyValuePair<string, JToken>>)((int missingInstance) => new KeyValuePair<string, JToken>(missingInstance.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture), (JToken)JObject.FromObject(new Data_format_directions(), SerializationUtil.JSON_SERIALIZER.Value))))), (Func<KeyValuePair<string, JToken>, string>)((KeyValuePair<string, JToken> kv) => kv.Key), (Func<KeyValuePair<string, JToken>, JToken>)((KeyValuePair<string, JToken> kv) => kv.Value.DeepClone()));
		JObject all = (JObject)startingFormats.Instances.AdditionalProperties["all"];
		System.Collections.Generic.IEnumerator<JObject> enumerator = Enumerable.Cast<JObject>((System.Collections.IEnumerable)val.Values).GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				JObject current = enumerator.Current;
				RecursiveMergeAllIntoInstance(all, current);
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
		return new Data_formats
		{
			Instances = new Instances
			{
				AdditionalProperties = (IDictionary<string, JToken>)(object)val
			}
		};
	}

	private static void RecursiveMergeAllIntoInstance(JObject all, JObject instance)
	{
		System.Collections.Generic.IEnumerator<JProperty> enumerator = all.Properties().GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				JProperty current = enumerator.Current;
				JToken jToken = instance.SelectToken(current.Name);
				if (jToken == null)
				{
					instance.Add(current.Name, current.Value);
				}
				else if (jToken is JObject instance2)
				{
					RecursiveMergeAllIntoInstance((JObject)current.Value, instance2);
				}
				else
				{
					instance[current.Name] = current.Value;
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private static Data_formats? GetDataFormats(TrainerSessionData td)
	{
		return MoveAllIntoInstances(ChooseDataFormat(GetBaseDataFormats(td), GetModelDataFormats(td)), td.PackageInfo.BaseInfo.MaxInstances);
	}

	private static System.Collections.Generic.IEnumerable<Tuple<HsdbNode, HsdbNode>> CreateHSDBChainRoutes(System.Collections.Generic.IList<HsdbNode> individualHsdbNodes)
	{
		System.Collections.Generic.IEnumerable<HsdbNode> enumerable = Enumerable.Where<HsdbNode>((System.Collections.Generic.IEnumerable<HsdbNode>)individualHsdbNodes, (Func<HsdbNode, bool>)((HsdbNode n) => n.NumberOfHsdbPorts > 1));
		System.Collections.Generic.IEnumerable<HsdbNode> enumerable2 = ((!enumerable.HasAtLeast(3)) ? enumerable : Enumerable.Concat<HsdbNode>(enumerable, Enumerable.Take<HsdbNode>(enumerable, 1)));
		return Enumerable.Zip<HsdbNode, HsdbNode, Tuple<HsdbNode, HsdbNode>>(enumerable2, Enumerable.Skip<HsdbNode>(enumerable2, 1), (Func<HsdbNode, HsdbNode, Tuple<HsdbNode, HsdbNode>>)((HsdbNode source, HsdbNode dest) => source.ConnectToNode(dest)));
	}

	private static System.Collections.Generic.IEnumerable<Tuple<HsdbNode, HsdbNode>> CreateHSDBSpikeRoutes(System.Collections.Generic.IList<HsdbNode> individualHsdbNodes)
	{
		System.Collections.Generic.IList<HsdbNode> individualHsdbNodes2 = individualHsdbNodes;
		return Enumerable.Select<ValueTuple<HsdbNode, HsdbNode>, Tuple<HsdbNode, HsdbNode>>(Enumerable.Where<ValueTuple<HsdbNode, HsdbNode>>(Enumerable.Select<HsdbNode, ValueTuple<HsdbNode, HsdbNode>>(Enumerable.Where<HsdbNode>((System.Collections.Generic.IEnumerable<HsdbNode>)individualHsdbNodes2, (Func<HsdbNode, bool>)((HsdbNode n) => n.NumberOfHsdbPorts == 1)), (Func<HsdbNode, ValueTuple<HsdbNode, HsdbNode>>)((HsdbNode sn) => new ValueTuple<HsdbNode, HsdbNode>(sn, FindNextNodeWithOpenPort(individualHsdbNodes2, sn)))), (Func<ValueTuple<HsdbNode, HsdbNode>, bool>)((ValueTuple<HsdbNode, HsdbNode> pair) => HsdbNode.IsValidNode(pair.Item2))), (Func<ValueTuple<HsdbNode, HsdbNode>, Tuple<HsdbNode, HsdbNode>>)((ValueTuple<HsdbNode, HsdbNode> pair) => pair.Item1.ConnectToNode(pair.Item2)));
	}

	private static HsdbNode? FindNextNodeWithOpenPort(System.Collections.Generic.IList<HsdbNode> individualHsdbNodes, params HsdbNode[] exceptThese)
	{
		return Enumerable.FirstOrDefault<HsdbNode>(Enumerable.Except<HsdbNode>((System.Collections.Generic.IEnumerable<HsdbNode>)individualHsdbNodes, (System.Collections.Generic.IEnumerable<HsdbNode>)exceptThese), (Func<HsdbNode, bool>)((HsdbNode n) => n.IsValidPort(n.NextOpenPort())));
	}

	private static string FormatDataFormatForInstance(string value, int instanceId)
	{
		if (INSTANCED_DATA_FORMAT_PATTERN.IsMatch(value))
		{
			return value;
		}
		return value + "_{" + instanceId.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture) + "}";
	}

	private static FormatException CreateNullDataFormatException(TrainerSessionData td)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		return new FormatException("Data format instance: " + td.InstallSlot.InstanceString + " is null.");
	}

	[IteratorStateMachine(typeof(_003CMatchUpDataFormats_003Ed__13))]
	private static System.Collections.Generic.IEnumerable<RoarRouteConfig> MatchUpDataFormats(List<FormatMatching> trainersWithDataFormats, ROAR_prtcl_fmt_type format)
	{
		Enumerator<FormatMatching> enumerator = trainersWithDataFormats.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				FormatMatching trainer = enumerator.Current;
				Enumerator<string, int> enumerator2 = trainer.TxChannels.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, int> txChannel = enumerator2.Current;
						System.Collections.Generic.IEnumerable<RoarRouteConfig> enumerable = Enumerable.Select<FormatMatching, RoarRouteConfig>(Enumerable.Where<FormatMatching>((System.Collections.Generic.IEnumerable<FormatMatching>)trainersWithDataFormats, (Func<FormatMatching, bool>)((FormatMatching td) => td != trainer && td.RxChannels.ContainsKey(txChannel.Key))), (Func<FormatMatching, RoarRouteConfig>)((FormatMatching match) => new RoarRouteConfig
						{
							ActualFormat = format,
							SourceEndpoint = trainer.TrainerData.EndpointName.IdentStringChecked,
							SourceChannel = txChannel.Value,
							DestinationEndpoint = match.TrainerData.EndpointName.IdentStringChecked,
							DestinationChannel = match.RxChannels[txChannel.Key]
						}));
						if (Enumerable.Any<RoarRouteConfig>(enumerable))
						{
							System.Collections.Generic.IEnumerator<RoarRouteConfig> enumerator3 = enumerable.GetEnumerator();
							try
							{
								while (((System.Collections.IEnumerator)enumerator3).MoveNext())
								{
									yield return enumerator3.Current;
								}
							}
							finally
							{
								((System.IDisposable)enumerator3)?.Dispose();
							}
						}
						else
						{
							Logger.Log(LogLevel.Debug, "'{0}' was not matched to another trainer.", txChannel.Key);
						}
					}
				}
				finally
				{
					((System.IDisposable)enumerator2).Dispose();
				}
			}
		}
		finally
		{
			((System.IDisposable)enumerator).Dispose();
		}
	}

	private static string ConvertFormatToPropertyName(ROAR_prtcl_fmt_type format)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		return format switch
		{
			ROAR_prtcl_fmt_type.ROAR_A429_FMT => "A429", 
			ROAR_prtcl_fmt_type.ROAR_RS232_FMT => "RS232", 
			ROAR_prtcl_fmt_type.ROAR_SIM_GENERIC_FMT => "GENERIC", 
			_ => throw new NotSupportedException(((object)format).ToString() + " is not supported for data formats."), 
		};
	}

	private static Dictionary<string, int> BuildChannelLookup(TrainerSessionData td, ROAR_prtcl_fmt_type format, string direction)
	{
		Data_formats dataFormats = GetDataFormats(td);
		if (dataFormats != null)
		{
			int instanceId = td.InstallSlot.InstanceId;
			JToken jToken = dataFormats.Instances.AdditionalProperties[instanceId.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture)].SelectToken(direction + "." + ConvertFormatToPropertyName(format));
			if (jToken != null)
			{
				Dictionary<string, string> val = jToken.ToObject<Dictionary<string, string>>();
				if (val != null)
				{
					return Enumerable.ToDictionary<KeyValuePair<string, string>, string, int>((System.Collections.Generic.IEnumerable<KeyValuePair<string, string>>)val, (Func<KeyValuePair<string, string>, string>)((KeyValuePair<string, string> kv) => FormatDataFormatForInstance(kv.Value, instanceId)), (Func<KeyValuePair<string, string>, int>)((KeyValuePair<string, string> kv) => int.Parse(kv.Key, (IFormatProvider)(object)CultureInfo.InvariantCulture)));
				}
			}
		}
		return new Dictionary<string, int>();
	}

	private static System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarRouteConfig>> BuildRoutes(ISessionEnvironment sessionEnvironment, ROAR_prtcl_fmt_type format)
	{
		return Enumerable.Select<RoarRouteConfig, KeyValuePair<string, RoarRouteConfig>>(MatchUpDataFormats(Enumerable.ToList<FormatMatching>(Enumerable.Select<TrainerSessionData, FormatMatching>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, FormatMatching>)((TrainerSessionData td) => new FormatMatching(td, BuildChannelLookup(td, format, "OUT"), BuildChannelLookup(td, format, "IN"))))), format), (Func<RoarRouteConfig, int, KeyValuePair<string, RoarRouteConfig>>)((RoarRouteConfig rr, int i) => new KeyValuePair<string, RoarRouteConfig>(rr.Format + "_" + i.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture), rr)));
	}

	private static List<KeyValuePair<string, RoarRouteConfig>> BuildRdmpRoutes(ISessionEnvironment sessionEnvironment, string launcherEndpointName)
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<string, RoarRouteConfig>> val = new List<KeyValuePair<string, RoarRouteConfig>>();
		int num = 0;
		string text = "RDMP";
		System.Collections.Generic.IEnumerator<TrainerSessionData> enumerator = ((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)sessionEnvironment.Data.TrainerData).Values.GetEnumerator();
		try
		{
			while (((System.Collections.IEnumerator)enumerator).MoveNext())
			{
				TrainerSessionData current = enumerator.Current;
				Data_formats dataFormats = GetDataFormats(current);
				if (dataFormats != null)
				{
					int instanceId = current.InstallSlot.InstanceId;
					Data_format_directions? obj = dataFormats.Instances.AdditionalProperties[instanceId.ToString((IFormatProvider)(object)CultureInfo.InvariantCulture)].ToObject<Data_format_directions>() ?? throw CreateNullDataFormatException(current);
					Dictionary<string, string> gENERIC = obj.OUT.GENERIC;
					Dictionary<string, string> gENERIC2 = obj.IN.GENERIC;
					if (!gENERIC.ContainsValue(text) || !gENERIC2.ContainsValue(text))
					{
						Logger.Log(LogLevel.Info, "IN or OUT GENERIC data format not defined. Install key: {0}", current.InstallData.InstallationKey.SerializedKey);
						continue;
					}
					RoarRouteConfig roarRouteConfig = new RoarRouteConfig
					{
						ActualFormat = ROAR_prtcl_fmt_type.ROAR_SIM_GENERIC_FMT,
						DestinationChannel = num,
						DestinationEndpoint = launcherEndpointName,
						SourceChannel = 0,
						SourceEndpoint = current.EndpointName.IdentStringChecked
					};
					RoarRouteConfig roarRouteConfig2 = roarRouteConfig.ReverseRoute();
					num++;
					val.Add(new KeyValuePair<string, RoarRouteConfig>(((object)roarRouteConfig).ToString(), roarRouteConfig));
					val.Add(new KeyValuePair<string, RoarRouteConfig>(((object)roarRouteConfig2).ToString(), roarRouteConfig2));
				}
			}
			return val;
		}
		finally
		{
			((System.IDisposable)enumerator)?.Dispose();
		}
	}

	private static System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarRouteConfig>> BuildHSDBRoutes(ISessionEnvironment sessionEnvironment)
	{
		List<HsdbNode> val = Enumerable.ToList<HsdbNode>((System.Collections.Generic.IEnumerable<HsdbNode>)Enumerable.ThenBy<HsdbNode, int>(Enumerable.OrderBy<HsdbNode, AvionicsType>(Enumerable.Select<TrainerSessionData, HsdbNode>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, HsdbNode>)((TrainerSessionData td) => new HsdbNode(new HsdbMatching(td)))), (Func<HsdbNode, AvionicsType>)((HsdbNode td) => td.HSDBInfo.InstallSlot.Category)), (Func<HsdbNode, int>)((HsdbNode td) => td.HSDBInfo.InstallSlot.InstanceId)));
		if (val.Count > 1)
		{
			return Enumerable.SelectMany<Tuple<HsdbNode, HsdbNode>, KeyValuePair<string, RoarRouteConfig>>(Enumerable.Concat<Tuple<HsdbNode, HsdbNode>>(CreateHSDBChainRoutes((System.Collections.Generic.IList<HsdbNode>)val), CreateHSDBSpikeRoutes((System.Collections.Generic.IList<HsdbNode>)val)), (Func<Tuple<HsdbNode, HsdbNode>, System.Collections.Generic.IEnumerable<KeyValuePair<string, RoarRouteConfig>>>)HsdbNode.CreateTwoWayHSDBRoute);
		}
		return Enumerable.Empty<KeyValuePair<string, RoarRouteConfig>>();
	}
}
