using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.CrashReporting;

public class CrashReportStreamHolderArgument : CrashReportStreamArgument
{
	[field: CompilerGenerated]
	public bool DisposeAfterUse
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public Stream ExistingStream
	{
		[CompilerGenerated]
		get;
	}

	public CrashReportStreamHolderArgument(Stream existingStream)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		DisposeAfterUse = true;
		ExistingStream = existingStream ?? throw new ArgumentNullException("existingStream");
	}

	protected override Stream OpenStream()
	{
		return ExistingStream;
	}

	protected override bool DisposeAfterSent(Stream stream)
	{
		return DisposeAfterUse;
	}
}
