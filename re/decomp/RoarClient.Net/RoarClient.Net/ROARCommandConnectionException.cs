using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace RoarClient.Net;

[Serializable]
public class ROARCommandConnectionException : ROARException
{
	private const string USE_MULTIPLE_ERROR_CODE_OVERLOADS = "Use the overloads with multiple error codes.";

	public ROAR_err_type LastRouterErrorCode { get; }

	[Obsolete("Use the overloads with multiple error codes.")]
	public ROARCommandConnectionException(ROAR_err_type clientError)
		: base(clientError)
	{
		LastRouterErrorCode = ROAR_err_type.ROAR_ERR_UNKN_ERR;
	}

	public ROARCommandConnectionException()
		: base(ROAR_err_type.ROAR_ERR_UNKN_ERR)
	{
		LastRouterErrorCode = ROAR_err_type.ROAR_ERR_UNKN_ERR;
	}

	public ROARCommandConnectionException(ROAR_err_type clientError, ROAR_err_type routerError)
		: base(clientError)
	{
		LastRouterErrorCode = routerError;
	}

	public ROARCommandConnectionException(string message, ROAR_err_type clientError, ROAR_err_type routerError)
		: base(message, clientError)
	{
		LastRouterErrorCode = routerError;
	}

	public ROARCommandConnectionException(string message, ROAR_err_type error, ROAR_err_type routerError, Exception innerException)
		: base(message, error, innerException)
	{
		LastRouterErrorCode = routerError;
	}

	[Obsolete("Use the overloads with multiple error codes.")]
	public ROARCommandConnectionException(string message, ROAR_err_type error)
		: base(message, error)
	{
	}

	[Obsolete("Use the overloads with multiple error codes.")]
	public ROARCommandConnectionException(string message, ROAR_err_type error, Exception innerException)
		: base(message, error, innerException)
	{
	}

	[Obsolete("Use the overloads with multiple error codes.")]
	public ROARCommandConnectionException(string message)
		: base(message)
	{
	}

	[Obsolete("Use the overloads with multiple error codes.")]
	public ROARCommandConnectionException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ROARCommandConnectionException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		LastRouterErrorCode = (ROAR_err_type)info.GetInt32("LastRouterErrorCode");
	}

	public static void ThrowIfErr(ROAR_err_type clientError, Func<ROAR_err_type> getLastRouterError, string message = "")
	{
		if (getLastRouterError == null)
		{
			throw new ArgumentNullException("getLastRouterError");
		}
		if (clientError != 0)
		{
			throw new ROARCommandConnectionException(message, clientError, getLastRouterError());
		}
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("LastRouterErrorCode", LastRouterErrorCode);
		base.GetObjectData(info, context);
	}
}
