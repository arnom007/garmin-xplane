using System;
using RoarClient.Net.Native;
using RoarClient.Net.Native.Adapters;

namespace RoarClient.Net;

internal class RoarCommandConnection : IRoarCommandConnection, IDisposable
{
	private readonly INativeCommandConnectionAdapter _library;

	private readonly IHumanErrorLookup _errorLookup;

	private readonly object _mutex = new object();

	private ROAR_hndl _hndl;

	private bool _disposed;

	public bool IsOpen => _library.ConvertHandleToError(_hndl) == ROAR_err_type.ROAR_ERR_NO_ERR;

	public RoarCommandConnection(INativeCommandConnectionAdapter library, IHumanErrorLookup errorLookup)
	{
		_library = library ?? throw new ArgumentNullException("library");
		_errorLookup = errorLookup ?? throw new ArgumentNullException("errorLookup");
	}

	~RoarCommandConnection()
	{
		Dispose(disposing: false);
	}

	public bool Open()
	{
		if (!IsOpen)
		{
			lock (_mutex)
			{
				if (!IsOpen)
				{
					_hndl = _library.OpenCommandConnection();
				}
			}
		}
		return IsOpen;
	}

	public void Close()
	{
		lock (_mutex)
		{
			_library.CloseHandle(_hndl);
			_hndl = ROAR_hndl.INVALID;
		}
	}

	public RoarUdpEndpoint GetOrAddUdpEndpoint(string ep_name, ushort client_port = 0, ushort master_port = 0)
	{
		ThrowIfConnectionIsNotOpen();
		IRoarResult result = CreateCombinedResult(_library.AddUdpEndpoint(_hndl, ep_name, client_port, master_port));
		if (result.IsNegativeErrorCode())
		{
			result.ThrowIfNot();
		}
		EndpointPortsResult endpointPorts = _library.GetEndpointPorts(_hndl, ep_name);
		CreateCombinedResult(endpointPorts.Error).ThrowIfNotTypicalSuccess();
		return new RoarUdpEndpoint(result.AsInteger(), ep_name, endpointPorts.Client, endpointPorts.Master);
	}

	public bool GetEndpointId(string ep_name, out int endpointId)
	{
		ThrowIfConnectionIsNotOpen();
		IRoarResult result = CreateCombinedResult(_library.GetEndpointId(_hndl, ep_name));
		if (result.IsNegativeErrorCode())
		{
			endpointId = -1;
			return false;
		}
		endpointId = result.AsInteger();
		return true;
	}

	public IRoarResult AddRoute(RoarRoute route)
	{
		ThrowIfConnectionIsNotOpen();
		if (route == null)
		{
			throw new ArgumentNullException("route");
		}
		using ROAR_route_args src = route.CreateSourceRouteArgs();
		using ROAR_route_args dest = route.CreateDestinationRouteArgs();
		ROAR_err_type error = _library.AddRoute(_hndl, src, dest, route.Format, two_way: false);
		return CreateCombinedResult(error);
	}

	public void StartRouting()
	{
		ThrowIfConnectionIsNotOpen();
		CreateCombinedResult(_library.StartRouting(_hndl)).ThrowIfNot(ROAR_err_type.ROAR_ERR_NO_ERR, ROAR_err_type.ROAR_ERR_INVLD_CALL);
	}

	public void StopRouting()
	{
		ThrowIfConnectionIsNotOpen();
		CreateCombinedResult(_library.StopRouting(_hndl)).ThrowIfNot(ROAR_err_type.ROAR_ERR_NO_ERR, ROAR_err_type.ROAR_ERR_INVLD_CALL);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			_disposed = true;
			_library.CloseHandleSafe(_hndl, disposing);
		}
	}

	private IRoarResult CreateCombinedResult(ROAR_err_type error)
	{
		return RoarResult.CreateClientServerResult(error, _library, _errorLookup, _hndl);
	}

	private void ThrowIfConnectionIsNotOpen()
	{
		if (!IsOpen)
		{
			throw new InvalidOperationException();
		}
	}
}
