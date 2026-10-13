using System;
using System.Runtime.CompilerServices;

namespace LauncherSimCommandProtocol;

public class UnsafeParsedLscpCommandResponse<T> where T : class
{
	public LscpCommandResponse GenericResponse
	{
		[CompilerGenerated]
		get;
	}

	public T? Payload
	{
		[CompilerGenerated]
		get;
	}

	public UnsafeParsedLscpCommandResponse(LscpCommandResponse genericResponse, T? payload)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		GenericResponse = genericResponse ?? throw new ArgumentNullException("genericResponse");
		Payload = payload;
	}
}
