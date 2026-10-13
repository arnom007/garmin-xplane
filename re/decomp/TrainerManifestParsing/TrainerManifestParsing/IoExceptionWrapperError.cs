using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace TrainerManifestParsing;

public class IoExceptionWrapperError : JsonParseError
{
	[field: CompilerGenerated]
	public IOException ExternalReaderError
	{
		[CompilerGenerated]
		get;
	}

	public override string ErrorMessage => ((System.Exception)(object)ExternalReaderError).Message;

	public IoExceptionWrapperError(IOException externalReaderError)
	{
		ExternalReaderError = externalReaderError;
	}
}
