using System;
using System.Runtime.Serialization;

namespace LauncherCoreLib.Models.TrainerPackages;

[Serializable]
public class BreakingChangeException : System.Exception
{
	public BreakingChangeException()
	{
	}

	public BreakingChangeException(string message)
		: base(message)
	{
	}

	public BreakingChangeException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}

	protected BreakingChangeException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
