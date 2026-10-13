using System;
using System.Runtime.CompilerServices;

namespace LauncherSimCommandProtocol;

public static class ParsedLscpCommandResponse
{
	public static ParsedLscpCommandResponse<T> Create<T>(LscpCommandResponse genericResponse, T payload) where T : class
	{
		return new ParsedLscpCommandResponse<T>(genericResponse, payload);
	}

	public static UnsafeParsedLscpCommandResponse<T> CreateUnsafe<T>(LscpCommandResponse genericResponse, T? payload) where T : class
	{
		return new UnsafeParsedLscpCommandResponse<T>(genericResponse, payload);
	}

	public static UnsafeParsedLscpCommandResponse<T> CreateUnsafeNull<T>(LscpCommandResponse genericResponse) where T : class
	{
		return new UnsafeParsedLscpCommandResponse<T>(genericResponse, null);
	}
}
public class ParsedLscpCommandResponse<T> where T : class
{
	public LscpCommandResponse GenericResponse
	{
		[CompilerGenerated]
		get;
	}

	public T Payload
	{
		[CompilerGenerated]
		get;
	}

	public ParsedLscpCommandResponse(LscpCommandResponse genericResponse, T payload)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		GenericResponse = genericResponse ?? throw new ArgumentNullException("genericResponse");
		Payload = payload ?? throw new ArgumentNullException("payload");
	}
}
