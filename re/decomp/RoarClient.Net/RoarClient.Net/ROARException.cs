using System;
using System.Runtime.Serialization;

namespace RoarClient.Net;

[Serializable]
public class ROARException : Exception
{
	private const string USE_ERROR_CODE_OVERLOADS = "Use the overloads with error code parameter";

	public ROAR_err_type ErrorCode { get; }

	public ROARException(ROAR_err_type error)
		: this(string.Empty, error)
	{
	}

	[Obsolete("Use the overloads with error code parameter")]
	public ROARException()
	{
		ErrorCode = ROAR_err_type.ROAR_ERR_UNKN_ERR;
	}

	public ROARException(string message, ROAR_err_type error)
		: this(message, error, null)
	{
	}

	public ROARException(string message, ROAR_err_type error, Exception innerException)
		: base(message, innerException)
	{
		ErrorCode = error;
	}

	[Obsolete("Use the overloads with error code parameter")]
	public ROARException(string message)
		: base(message)
	{
	}

	[Obsolete("Use the overloads with error code parameter")]
	public ROARException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected ROARException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		ErrorCode = (ROAR_err_type)info.GetInt32("ErrorCode");
	}

	public static void ThrowIfErr(ROAR_err_type error, string message = "")
	{
		if (error != 0)
		{
			throw new ROARException(message, error);
		}
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("ErrorCode", (int)ErrorCode);
		base.GetObjectData(info, context);
	}
}
