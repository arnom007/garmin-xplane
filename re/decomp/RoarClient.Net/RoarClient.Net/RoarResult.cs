using System;
using RoarClient.Net.Native;
using RoarClient.Net.Native.Adapters;

namespace RoarClient.Net;

internal class RoarResult : IRoarResult
{
	private readonly IHumanErrorLookup _errorLookup;

	public ROAR_err_type NativeError { get; }

	internal RoarResult(ROAR_err_type error, IHumanErrorLookup errorLookup)
	{
		NativeError = error;
		_errorLookup = errorLookup ?? throw new ArgumentNullException("errorLookup");
	}

	public override string ToString()
	{
		return _errorLookup.GetHumanReadableError(NativeError);
	}

	public Exception BuildException()
	{
		return new ROARException(ToString(), NativeError);
	}

	internal static IRoarResult Create(ROAR_err_type error, IHumanErrorLookup errorLookup)
	{
		return new RoarResult(error, errorLookup);
	}

	internal static IRoarResult CreateClientServerResult(ROAR_err_type error, INativeCommandConnectionAdapter library, IHumanErrorLookup errorLookup, ROAR_hndl cmdHandle)
	{
		return new ClientServerResult(Create(error, errorLookup), Create(library.GetLastRouterResponseError(cmdHandle), errorLookup));
	}
}
