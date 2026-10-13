using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using AdvancedProcessCreation;
using LOLA.Processes;

namespace LauncherCoreLib.Models.Simulated.Flight;

public record DataRouterProcessApi(IOsProcessOperations OsProcessApi, ForkProcess<IAdvancedProcessStartupInfo> Fork, GetProcessExePath GetExePath)
{
	protected virtual System.Type EqualityContract
	{
		[CompilerGenerated]
		get
		{
			return typeof(DataRouterProcessApi);
		}
	}

	[CompilerGenerated]
	public override string ToString()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		StringBuilder val = new StringBuilder();
		val.Append("DataRouterProcessApi");
		val.Append(" { ");
		if (PrintMembers(val))
		{
			val.Append(' ');
		}
		val.Append('}');
		return ((object)val).ToString();
	}

	[CompilerGenerated]
	protected virtual bool PrintMembers(StringBuilder builder)
	{
		RuntimeHelpers.EnsureSufficientExecutionStack();
		builder.Append("OsProcessApi = ");
		builder.Append((object)OsProcessApi);
		builder.Append(", Fork = ");
		builder.Append((object)Fork);
		builder.Append(", GetExePath = ");
		builder.Append((object)GetExePath);
		return true;
	}

	[CompilerGenerated]
	public virtual bool Equals(DataRouterProcessApi? other)
	{
		if ((object)this != other)
		{
			if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<IOsProcessOperations>.Default.Equals(OsProcessApi, other.OsProcessApi) && EqualityComparer<ForkProcess<IAdvancedProcessStartupInfo>>.Default.Equals(Fork, other.Fork))
			{
				return EqualityComparer<GetProcessExePath>.Default.Equals(GetExePath, other.GetExePath);
			}
			return false;
		}
		return true;
	}
}
