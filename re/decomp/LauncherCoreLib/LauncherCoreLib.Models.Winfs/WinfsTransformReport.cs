using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Utility;

namespace LauncherCoreLib.Models.Winfs;

public sealed class WinfsTransformReport
{
	private static readonly System.Exception UNKNOWN_ERROR = new GenericErrorException("Unknown Error");

	[field: CompilerGenerated]
	public WinfsTransformRequest Request
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public WinfsOperationResult Result
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public System.Exception? Error
	{
		[CompilerGenerated]
		get;
	}

	public string FormattedReport => FormatReport();

	public WinfsTransformReport(WinfsTransformRequest request, WinfsOperationResult result)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Request = request ?? throw new ArgumentNullException("request");
		Result = result;
		Error = ((result == WinfsOperationResult.Failed) ? UNKNOWN_ERROR : null);
	}

	public WinfsTransformReport(WinfsTransformRequest request, System.Exception error)
		: this(request, WinfsOperationResult.Failed)
	{
		Error = error ?? UNKNOWN_ERROR;
	}

	public string ToString()
	{
		return FormattedReport;
	}

	private string FormatReport()
	{
		return string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0}: {1}", (object)Request, (object)Result);
	}
}
