using System;
using System.Collections.Generic;
using System.Linq;
using RoarClient.Net.Native;
using RoarClient.Net.Native.Adapters;
using RoarClient.Net.Native.Discovery;

namespace RoarClient.Net;

public class EndpointIoMediator : IDisposable
{
	private class RoarClientEndpointCleanup : IDisposable
	{
		private readonly Dictionary<ROAR_hndl, RoarIOEndpoint> _createdEndpoints;

		private readonly ROAR_hndl _hndl;

		private bool _disposed;

		public RoarClientEndpointCleanup(ROAR_hndl hndl, Dictionary<ROAR_hndl, RoarIOEndpoint> createdEndpoints)
		{
			if (hndl == ROAR_hndl.INVALID)
			{
				throw new ArgumentException("Invalid handle.", "hndl");
			}
			_createdEndpoints = createdEndpoints ?? throw new ArgumentNullException("createdEndpoints");
			_hndl = hndl;
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			if (_hndl != ROAR_hndl.INVALID)
			{
				lock (_createdEndpoints)
				{
					_createdEndpoints.Remove(_hndl);
				}
			}
		}
	}

	private readonly IEndpointIoAdapter _endpoints;

	private readonly IEndpointRx _rx;

	private readonly IHumanErrorLookup _errorLookup;

	private readonly Dictionary<ROAR_hndl, RoarIOEndpoint> _createdEndpoints = new Dictionary<ROAR_hndl, RoarIOEndpoint>();

	private bool _disposed;

	public EndpointIoMediator(IDynamicNativeLibraryLoader library, IEndpointRx rx, IHumanErrorLookup errorLookup)
		: this(EndpointIoDiscovery.GetIoAdapter(library), rx, errorLookup)
	{
	}

	public EndpointIoMediator(IEndpointIoAdapter endpoints, IEndpointRx rx, IHumanErrorLookup errorLookup)
	{
		_endpoints = endpoints ?? throw new ArgumentNullException("endpoints");
		_rx = rx ?? throw new ArgumentNullException("rx");
		_errorLookup = errorLookup ?? throw new ArgumentNullException("errorLookup");
		_rx.MessageReceived += OnMessageReceived;
	}

	public IRoarIOEndpoint OpenEndpoint(string endpointName)
	{
		if (string.IsNullOrEmpty(endpointName))
		{
			throw new ArgumentNullException("endpointName");
		}
		ROAR_hndl rOAR_hndl = _endpoints.OpenEndpoint(endpointName);
		RoarResult.Create(_endpoints.ConvertHandleToError(rOAR_hndl), _errorLookup).ThrowIfNotTypicalSuccess();
		lock (_createdEndpoints)
		{
			if (_createdEndpoints.ContainsKey(rOAR_hndl))
			{
				throw new InvalidOperationException("Endpoint with name \"" + endpointName + "\" is already open.");
			}
			IDisposable disposable = new RoarClientEndpointCleanup(rOAR_hndl, _createdEndpoints);
			RoarIOEndpoint roarIOEndpoint;
			try
			{
				roarIOEndpoint = new RoarIOEndpoint(rOAR_hndl, endpointName, _endpoints, _errorLookup, disposable);
				disposable = null;
			}
			finally
			{
				disposable?.Dispose();
			}
			_createdEndpoints.Add(rOAR_hndl, roarIOEndpoint);
			return roarIOEndpoint;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		_disposed = true;
		if (!disposing)
		{
			return;
		}
		_rx.MessageReceived -= OnMessageReceived;
		IEnumerable<IDisposable> enumerable;
		lock (_createdEndpoints)
		{
			enumerable = _createdEndpoints.Values.ToList();
			_createdEndpoints.Clear();
		}
		foreach (IDisposable item in enumerable)
		{
			item.Dispose();
		}
	}

	private void OnMessageReceived(object sender, RoarMessageEventArgs message)
	{
		lock (_createdEndpoints)
		{
			if (_createdEndpoints.TryGetValue(message.Handle, out RoarIOEndpoint value))
			{
				value.RaiseReceiveMessage(message);
			}
		}
	}
}
