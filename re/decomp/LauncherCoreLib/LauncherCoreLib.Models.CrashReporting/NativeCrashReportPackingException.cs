using System;
using System.Runtime.Serialization;

namespace LauncherCoreLib.Models.CrashReporting;

[Serializable]
public class NativeCrashReportPackingException : System.Exception
{
	public NativeCrashReportPackingException(string message)
		: base(message)
	{
	}

	public NativeCrashReportPackingException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}

	public NativeCrashReportPackingException()
	{
	}

	protected NativeCrashReportPackingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
