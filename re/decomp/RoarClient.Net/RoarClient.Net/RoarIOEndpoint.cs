using System;
using RoarClient.Net.Native;
using RoarClient.Net.Native.Adapters;

namespace RoarClient.Net;

internal class RoarIOEndpoint : IRoarIOEndpoint, IDisposable
{
	private readonly IEndpointIoAdapter _lib;

	private readonly IHumanErrorLookup _errorLookup;

	private readonly IDisposable _clientCleanup;

	private ROAR_hndl _hndl;

	private bool _disposed;

	public string EndpointName { get; }

	public ROAR_hndl Handle => _hndl;

	public event EventHandler<RoarMessageEventArgs>? OnReceiveMessage;

	public RoarIOEndpoint(ROAR_hndl hndl, string endpointName, IEndpointIoAdapter endpoints, IHumanErrorLookup errorLookup, IDisposable clientCleanup)
	{
		if (hndl == ROAR_hndl.INVALID)
		{
			throw new ArgumentException("Invalid handle", "hndl");
		}
		if (string.IsNullOrEmpty(endpointName))
		{
			throw new ArgumentNullException("endpointName");
		}
		_hndl = hndl;
		EndpointName = endpointName;
		_lib = endpoints ?? throw new ArgumentNullException("endpoints");
		_errorLookup = errorLookup ?? throw new ArgumentNullException("errorLookup");
		_clientCleanup = clientCleanup ?? throw new ArgumentNullException("clientCleanup");
	}

	public void WriteEndpoint(ROAR_prtcl_fmt_type format, ROAR_rte_chnl_type channel, byte[] payload)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("RoarIOEndpoint", "This endpoint has already been closed");
		}
		RoarResult.Create(_lib.WriteToEndpoint(new RoarMessageEventArgs(_hndl, format, channel, payload)), _errorLookup).ThrowIfNotTypicalSuccess();
	}

	public void RaiseReceiveMessage(RoarMessageEventArgs eventArgs)
	{
		RaiseReceiveEvent(eventArgs);
	}

	protected void RaiseReceiveEvent(RoarMessageEventArgs eventArgs)
	{
		this.OnReceiveMessage?.Invoke(this, eventArgs);
	}

	~RoarIOEndpoint()
	{
		Dispose(disposing: false);
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
			if (disposing)
			{
				_clientCleanup.Dispose();
				this.OnReceiveMessage = null;
			}
			if (_hndl != ROAR_hndl.INVALID)
			{
				_lib.CloseHandleSafe(_hndl, disposing);
				_hndl = ROAR_hndl.INVALID;
			}
		}
	}
}
