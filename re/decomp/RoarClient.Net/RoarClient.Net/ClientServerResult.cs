using System;

namespace RoarClient.Net;

public class ClientServerResult : IRoarResult
{
	public IRoarResult Client { get; }

	public IRoarResult Server { get; }

	public ROAR_err_type NativeError
	{
		get
		{
			if (ClientFailedAndServerFailed())
			{
				return Server.NativeError;
			}
			return Client.NativeError;
		}
	}

	public ClientServerResult(IRoarResult client, IRoarResult server)
	{
		Client = client ?? throw new ArgumentNullException("client");
		Server = server ?? throw new ArgumentNullException("server");
	}

	public override string ToString()
	{
		return $"Client: {Client}, Server: {Server}";
	}

	public Exception BuildException()
	{
		if (ClientFailedAndServerFailed())
		{
			return new ROARCommandConnectionException(ToString(), Client.NativeError, Server.NativeError);
		}
		return Client.BuildException();
	}

	private bool ClientFailedAndServerFailed()
	{
		if (Client.NativeError != 0)
		{
			return Server.NativeError != ROAR_err_type.ROAR_ERR_NO_ERR;
		}
		return false;
	}
}
