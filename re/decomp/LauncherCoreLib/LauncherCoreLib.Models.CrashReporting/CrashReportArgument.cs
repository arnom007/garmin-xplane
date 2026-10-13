using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.CrashReporting;

public abstract class CrashReportArgument
{
	protected sealed class NoopDispose : System.IDisposable
	{
		public void Dispose()
		{
		}
	}

	protected static readonly System.IDisposable NOOP_DISPOSE = new NoopDispose();

	[field: CompilerGenerated]
	public string VariableName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = string.Empty;


	public abstract bool IsValid { get; }

	public virtual System.IDisposable AddArgumentToForm(MultipartFormDataContent form)
	{
		return NOOP_DISPOSE;
	}
}
