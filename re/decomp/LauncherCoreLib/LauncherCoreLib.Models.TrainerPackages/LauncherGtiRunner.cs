using System;
using System.Runtime.CompilerServices;
using System.Threading;
using GTI.Net;
using LOLA.Logging;

namespace LauncherCoreLib.Models.TrainerPackages;

public class LauncherGtiRunner : GtiRunnerWrapper, ILauncherGtiRunner, IGtiRunner
{
	[CompilerGenerated]
	private EventHandler? m_MutationCommandCompleted;

	public event EventHandler MutationCommandCompleted
	{
		[CompilerGenerated]
		add
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			EventHandler val = this.m_MutationCommandCompleted;
			EventHandler val2;
			do
			{
				val2 = val;
				EventHandler val3 = (EventHandler)System.Delegate.Combine((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler>(ref this.m_MutationCommandCompleted, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			EventHandler val = this.m_MutationCommandCompleted;
			EventHandler val2;
			do
			{
				val2 = val;
				EventHandler val3 = (EventHandler)System.Delegate.Remove((System.Delegate)(object)val2, (System.Delegate)(object)value);
				val = Interlocked.CompareExchange<EventHandler>(ref this.m_MutationCommandCompleted, val3, val2);
			}
			while (val != val2);
		}
	}

	public LauncherGtiRunner(IGtiRunner runner)
		: base(runner)
	{
	}

	public override IGtiProcessExecution Execute(params string[] commandLineArgs)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		IGtiProcessExecution gtiProcessExecution = base.Execute(commandLineArgs);
		if (gtiProcessExecution != null && commandLineArgs != null && commandLineArgs.Length != 0 && IsMutationCommand(commandLineArgs[0]))
		{
			return new GtiProcessWrapper(gtiProcessExecution, [CompilerGenerated] (GtiProcessResult pr) =>
			{
				if (pr.Succeeded)
				{
					try
					{
						EventHandler? mutationCommandCompleted = this.MutationCommandCompleted;
						if (mutationCommandCompleted != null)
						{
							mutationCommandCompleted.Invoke((object)this, EventArgs.Empty);
						}
					}
					catch (System.Exception e)
					{
						Logger.LogFormattedException(e, "Mutation complete event handler");
						throw;
					}
				}
				return pr;
			});
		}
		return gtiProcessExecution ?? throw new InvalidOperationException("GTI runner created null execution object.");
	}

	private static bool IsMutationCommand(string v)
	{
		v = v.Trim();
		if (!string.Equals(v, "list", (StringComparison)5))
		{
			return !string.Equals(v, "help", (StringComparison)5);
		}
		return false;
	}
}
