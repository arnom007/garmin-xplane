using System;
using RoarClient.Net.Native;
using RoarClient.Net.Native.Adapters;
using RoarClient.Net.Native.Discovery;

namespace RoarClient.Net;

public sealed class ClientContainer : IRoarClient, IDisposable
{
	private class RxAdapter : IEndpointRx
	{
		public event EventHandler<RoarMessageEventArgs>? MessageReceived;

		public void RxReceived(ROAR_hndl hndl, ref ROAR_client_io_type pkt)
		{
			this.MessageReceived?.Invoke(this, new RoarMessageEventArgs(hndl, pkt));
		}
	}

	private readonly IDynamicNativeLibraryLoader _lib;

	private readonly IClientInitAdapter _init;

	private readonly EndpointIoMediator _endpoints;

	private readonly LoggerV1Mediator? _logger;

	private readonly IHumanErrorLookup _errorLookup;

	private readonly RxAdapter _rxAdapter;

	private readonly ROAR_data_rx_callback _nativeFuncStorage;

	private bool _disposed;

	public Version NativeClientVersion { get; }

	public ClientContainer(IDynamicNativeLibraryLoader lib, Version nativeClientVersion)
	{
		_lib = lib ?? throw new ArgumentNullException("lib");
		NativeClientVersion = nativeClientVersion ?? throw new ArgumentNullException("nativeClientVersion");
		_init = InitDiscovery.GetInitAdapter(lib);
		_rxAdapter = new RxAdapter();
		_nativeFuncStorage = SafeRxCallback;
		_errorLookup = ErrorLookupDiscovery.GetErrorAdapter(_lib);
		_endpoints = new EndpointIoMediator(_lib, _rxAdapter, _errorLookup);
		_logger = null;
		if (ClientLoggingV1.IsSupportedByLibrary(_lib))
		{
			_logger = new LoggerV1Mediator(ClientLoggingV1.BindToLibrary(_lib), _errorLookup);
		}
		ROARException.ThrowIfErr(_init.InitClient(_nativeFuncStorage));
	}

	public IRoarIOEndpoint OpenEndpoint(string endpointName)
	{
		return _endpoints.OpenEndpoint(endpointName);
	}

	public IRoarCommandConnection CreateCommandConn()
	{
		INativeCommandConnectionAdapter library;
		if (CommandConnectionV2.IsSupportedByLibrary(_lib))
		{
			library = new CC2Adapter(CommandConnectionV2.BindToLibrary(_lib), HandlesV1.BindToLibrary(_lib));
		}
		else
		{
			if (!CommandConnectionV1.IsSupportedByLibrary(_lib))
			{
				throw new NotSupportedException("The bound native client library does not support known command connections.");
			}
			library = new CC1Adapter(CommandConnectionV1.BindToLibrary(_lib), HandlesV1.BindToLibrary(_lib));
		}
		return new RoarCommandConnection(library, _errorLookup);
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_disposed = true;
			_endpoints.Dispose();
			_logger?.Dispose();
			_init.CloseClient();
			_lib.Dispose();
		}
		GC.SuppressFinalize(this);
	}

	public IDisposable RegisterLogger(IRoarClientLogger logger)
	{
		if (_logger == null)
		{
			throw new NotSupportedException("The bound native client does not support logging.");
		}
		return _logger.RegisterLogger(logger);
	}

	private void SafeRxCallback(ROAR_hndl hndl, ref ROAR_client_io_type pkt)
	{
		try
		{
			_rxAdapter.RxReceived(hndl, ref pkt);
		}
		catch (Exception ex)
		{
			Exception e = ex;
			Util.CatchAll(delegate
			{
				_logger?.LogManagedError(e);
			});
		}
	}
}
