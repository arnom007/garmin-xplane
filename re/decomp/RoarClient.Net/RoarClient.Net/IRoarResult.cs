using System;

namespace RoarClient.Net;

public interface IRoarResult
{
	ROAR_err_type NativeError { get; }

	Exception BuildException();
}
