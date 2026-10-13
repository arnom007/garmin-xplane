using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GTI.Net;
using LOLA.Processes;
using LOLA.Tasks;

namespace LauncherCoreLib.Models.TrainerPackages;

public sealed class GtiProcessWrapper : IGtiProcessExecution
{
	[field: CompilerGenerated]
	public IGtiProcessExecution Inner
	{
		[CompilerGenerated]
		get;
	}

	public IProcessSession Process => Inner.Process;

	public IGtiOutputProcessing Output => Inner.Output;

	[field: CompilerGenerated]
	public System.Threading.Tasks.Task<GtiProcessResult> ProcessResult
	{
		[CompilerGenerated]
		get;
	}

	public GtiProcessWrapper(IGtiProcessExecution inner, Func<GtiProcessResult, GtiProcessResult> onProcessEnd)
	{
		Inner = inner;
		ProcessResult = inner.ProcessResult.Then<GtiProcessResult, GtiProcessResult>(onProcessEnd);
	}

	public IGtiProcessExecution IgnoreStandardOutput()
	{
		Inner.IgnoreStandardOutput();
		return this;
	}

	public System.Threading.Tasks.Task<bool> SendCancelSignal()
	{
		return Inner.SendCancelSignal();
	}
}
