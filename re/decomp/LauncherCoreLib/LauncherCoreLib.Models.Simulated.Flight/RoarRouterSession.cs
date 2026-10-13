using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using LOLA.Processes;
using LauncherCoreLib.Models.Simulated.Flight.Serialized;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class RoarRouterSession : IDataRouterSession, System.IDisposable
{
	private readonly IRoarClient _clientLibrary;

	private readonly IRouterCommandConnectionFactory _commandConnectionFactory;

	private readonly IRouterEndpointFactory _endpointFactory;

	private readonly List<IRouterCommandConnection> _commandConnections = new List<IRouterCommandConnection>();

	private readonly object _commandConnectionMutex = new object();

	private readonly Dictionary<string, IRoarEndpointInternal> _openEndpoints = new Dictionary<string, IRoarEndpointInternal>();

	private readonly object _openEndpointMutex = new object();

	private bool _disposed;

	[field: CompilerGenerated]
	public IProcessSession RouterProcess
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public IRoarRouteCollection RouteCollection
	{
		[CompilerGenerated]
		get;
	}

	public RoarRouterSession(IProcessSession roarProcess, IRoarClient client, IRouterCommandConnectionFactory commandConnectionFactory, IRouterEndpointFactory endpointFactory)
	{
		RouterProcess = roarProcess;
		_clientLibrary = client;
		_commandConnectionFactory = commandConnectionFactory;
		_endpointFactory = endpointFactory;
		RouterProcess.CloseOnDispose = true;
		RouteCollection = new RoarRouteCollection((System.Collections.Generic.IReadOnlyCollection<RoarRouteConfig>)new List<RoarRouteConfig>());
	}

	public IRouterCommandConnection CreateCommandConnection()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (_disposed)
		{
			throw new ObjectDisposedException("RoarRouterSession");
		}
		IRouterCommandConnection routerCommandConnection = _commandConnectionFactory.CreateCommandConnection();
		lock (_commandConnectionMutex)
		{
			_commandConnections.Add(routerCommandConnection);
			return routerCommandConnection;
		}
	}

	public IRoarEndpoint GetOrOpenEndpoint(string endpointName)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		string endpointName2 = endpointName;
		lock (_openEndpointMutex)
		{
			IRoarEndpointInternal roarEndpointInternal = default(IRoarEndpointInternal);
			if (!_openEndpoints.TryGetValue(endpointName2, ref roarEndpointInternal))
			{
				roarEndpointInternal = _endpointFactory.OpenEndpoint(endpointName2, Disposable.Create((Action)delegate
				{
					lock (_openEndpointMutex)
					{
						_openEndpoints.Remove(endpointName2);
					}
				}));
				_openEndpoints.Add(endpointName2, roarEndpointInternal);
			}
			return roarEndpointInternal;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize((object)this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		_disposed = true;
		if (disposing)
		{
			((System.IDisposable)RouterProcess).Dispose();
			IRouterCommandConnection[] array;
			lock (_commandConnectionMutex)
			{
				array = _commandConnections.ToArray();
				_commandConnections.Clear();
			}
			IRouterCommandConnection[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				((System.IDisposable)array2[i]).Dispose();
			}
			IRoarEndpointInternal[] array3;
			lock (_openEndpointMutex)
			{
				array3 = Enumerable.ToArray<IRoarEndpointInternal>((System.Collections.Generic.IEnumerable<IRoarEndpointInternal>)_openEndpoints.Values);
				_openEndpoints.Clear();
			}
			IRoarEndpointInternal[] array4 = array3;
			for (int i = 0; i < array4.Length; i++)
			{
				((System.IDisposable)array4[i]).Dispose();
			}
			((System.IDisposable)_clientLibrary).Dispose();
		}
	}
}
