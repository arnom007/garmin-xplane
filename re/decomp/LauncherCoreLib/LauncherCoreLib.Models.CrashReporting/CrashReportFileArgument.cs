using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.CrashReporting;

public class CrashReportFileArgument : CrashReportStreamArgument
{
	[field: CompilerGenerated]
	public string Filepath
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
			return Path.GetFileName(Filepath);
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException("The filename is pulled from the actual file.");
		}
	}

	public override bool IsValid
	{
		get
		{
			if (base.IsValid)
			{
				return File.Exists(Filepath ?? string.Empty);
			}
			return false;
		}
	}

	protected override Stream OpenStream()
	{
		return (Stream)(object)File.OpenRead(Filepath);
	}
}
