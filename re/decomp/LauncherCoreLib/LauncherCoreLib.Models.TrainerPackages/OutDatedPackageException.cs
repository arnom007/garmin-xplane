using System;
using System.Runtime.Serialization;

namespace LauncherCoreLib.Models.TrainerPackages;

[Serializable]
public class OutDatedPackageException : System.Exception
{
	public OutDatedPackageException()
	{
	}

	public OutDatedPackageException(string message)
		: base(message)
	{
	}

	public OutDatedPackageException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}

	protected OutDatedPackageException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		: base(serializationInfo, streamingContext)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
