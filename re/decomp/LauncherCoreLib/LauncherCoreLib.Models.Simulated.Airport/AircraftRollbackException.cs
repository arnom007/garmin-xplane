using System;
using System.Runtime.Serialization;

namespace LauncherCoreLib.Models.Simulated.Airport;

[Serializable]
public class AircraftRollbackException : System.Exception
{
	public AircraftRollbackException()
	{
	}

	public AircraftRollbackException(string message)
		: base(message)
	{
	}

	public AircraftRollbackException(string message, System.Exception innerException)
		: base(message, innerException)
	{
	}

	protected AircraftRollbackException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)

}
