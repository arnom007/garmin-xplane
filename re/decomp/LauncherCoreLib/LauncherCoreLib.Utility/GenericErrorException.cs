using System;
using System.Runtime.Serialization;

namespace LauncherCoreLib.Utility;

[Serializable]
public class GenericErrorException : System.Exception
{
	public GenericErrorException(string message)
		: base(message)
	{
	}

	public GenericErrorException(string message, System.Exception? innerException)
		: base(message, innerException)
	{
	}

	public GenericErrorException()
	{
	}

	protected GenericErrorException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
