using System;
using System.Collections.Generic;
using System.Linq;
using RoarClient.Net.Native;
using RoarClient.Net.Native.Adapters;

namespace RoarClient.Net;

public class LoggerV1Mediator : IDisposable
{
	private class LoggerUnregister : IDisposable
	{
		private readonly LoggerV1Mediator _client;

		private readonly IRoarClientLogger _logger;

		private bool _disposed;

		public LoggerUnregister(LoggerV1Mediator mediator, IRoarClientLogger logger)
		{
			_client = mediator ?? throw new ArgumentNullException("mediator");
			_logger = logger ?? throw new ArgumentNullException("logger");
		}

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}
			_disposed = true;
			lock (_client._loggers)
			{
				if (_client._loggers.ContainsKey(_logger) && _client._loggers[_logger]-- <= 0)
				{
					_client._loggers.Remove(_logger);
				}
				_client.RefreshManagedCallbackWithLock();
				try
				{
					_client.CheckNativeRegistrationWithLock();
				}
				catch (ROARException)
				{
				}
			}
		}
	}

	private readonly ClientLoggingV1 _logging;

	private readonly IHumanErrorLookup _errorLookup;

	private readonly Dictionary<IRoarClientLogger, int> _loggers = new Dictionary<IRoarClientLogger, int>();

	private readonly ROAR_log_callback _callback_container;

	private Action<string, bool>? _managedCallback;

	private ROAR_log_callback? _current_callback;

	private bool _disposed;

	public LoggerV1Mediator(ClientLoggingV1 logging, IHumanErrorLookup errorLookup)
	{
		_logging = logging ?? throw new ArgumentNullException("logging");
		_errorLookup = errorLookup ?? throw new ArgumentNullException("errorLookup");
		_callback_container = NativeLoggingCallback;
	}

	~LoggerV1Mediator()
	{
		Dispose(disposing: false);
	}

	public IDisposable RegisterLogger(IRoarClientLogger logger)
	{
		if (logger == null)
		{
			throw new ArgumentNullException("logger");
		}
		if (_disposed)
		{
			throw new ObjectDisposedException(GetType().Name);
		}
		lock (_loggers)
		{
			if (!_loggers.ContainsKey(logger))
			{
				_loggers[logger] = 0;
			}
			_loggers[logger]++;
			RefreshManagedCallbackWithLock();
			try
			{
				CheckNativeRegistrationWithLock();
			}
			catch (Exception)
			{
				_loggers[logger]--;
				CheckNativeRegistrationWithLock();
				throw;
			}
			return new LoggerUnregister(this, logger);
		}
	}

	public void LogManagedError(Exception error)
	{
		if (error == null)
		{
			throw new ArgumentNullException("error");
		}
		_managedCallback?.Invoke("[" + error.GetType().Name + "] " + error.Message + "\n" + error.StackTrace, arg2: true);
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
		if (disposing)
		{
			lock (_loggers)
			{
				_loggers.Clear();
				RefreshManagedCallbackWithLock();
				CheckNativeRegistrationWithLock();
			}
		}
		_logging.ROAR_adv_callback_register_log(null);
	}

	private static Action<string, bool>? CreateManagedCallback(IReadOnlyCollection<IRoarClientLogger> loggers)
	{
		IReadOnlyCollection<IRoarClientLogger> loggers2 = loggers;
		if (loggers2.Count == 0)
		{
			return null;
		}
		return delegate(string message, bool isError)
		{
			string message2 = message;
			if (isError)
			{
				foreach (IRoarClientLogger logger2 in loggers2)
				{
					Util.CatchAll(delegate
					{
						logger2.LogError(message2);
					});
				}
				return;
			}
			foreach (IRoarClientLogger logger in loggers2)
			{
				Util.CatchAll(delegate
				{
					logger.LogInfo(message2);
				});
			}
		};
	}

	private void RefreshManagedCallbackWithLock()
	{
		List<IRoarClientLogger> loggers = (from kv in _loggers
			where kv.Value > 0
			select kv.Key).ToList();
		_managedCallback = CreateManagedCallback(loggers);
	}

	private void NativeLoggingCallback(string message, int message_sz, byte is_error)
	{
		string message2 = message;
		Util.CatchAll(delegate
		{
			_managedCallback?.Invoke(message2, is_error != 0);
		});
	}

	private void CheckNativeRegistrationWithLock()
	{
		if (_current_callback == null)
		{
			if (_loggers.Values.Any((int l) => l > 0))
			{
				SetCallbackAndThrowIfError(_callback_container);
			}
		}
		else if (_loggers.Count == 0 || _loggers.Values.All((int refCount) => refCount == 0))
		{
			SetCallbackAndThrowIfError(null);
		}
	}

	private void SetCallbackAndThrowIfError(ROAR_log_callback? cb)
	{
		ROAR_err_type rOAR_err_type = _logging.ROAR_adv_callback_register_log(cb);
		if (rOAR_err_type == ROAR_err_type.ROAR_ERR_NO_ERR)
		{
			_current_callback = cb;
			return;
		}
		throw new ROARException(_errorLookup.GetHumanReadableError(rOAR_err_type), rOAR_err_type);
	}
}
