using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace LauncherCoreLib.Models.CrashReporting;

public class CrashReportStringArgument : CrashReportStreamArgument
{
	[field: CompilerGenerated]
	public string? StringContent
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	}

	[field: CompilerGenerated]
	public string FileName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	public override string FileNameHint
	{
		get
		{
			return FileName;
		}
		set
		{
			FileName = value;
		}
	}

	public override bool IsValid
	{
		get
		{
			if (base.IsValid)
			{
				return StringContent != null;
			}
			return false;
		}
	}

	protected override Stream OpenStream()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		MemoryStream val = new MemoryStream();
		StreamWriter val2 = new StreamWriter((Stream)(object)val, Encoding.UTF8, 4096, true);
		try
		{
			((TextWriter)val2).Write(StringContent);
			((TextWriter)val2).Flush();
			((Stream)val).Position = 0L;
			return (Stream)(object)val;
		}
		finally
		{
			((System.IDisposable)val2)?.Dispose();
		}
	}
}
