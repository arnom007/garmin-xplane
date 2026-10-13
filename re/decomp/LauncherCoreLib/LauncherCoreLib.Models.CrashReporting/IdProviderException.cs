using System;
using System.Runtime.Serialization;

namespace LauncherCoreLib.Models.CrashReporting;

[Serializable]
public class IdProviderException : System.Exception
{
	public IdProviderException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}

	public IdProviderException()
	{
	}

	public IdProviderException(string message)
		: base(message)
	{
	}

	protected IdProviderException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
