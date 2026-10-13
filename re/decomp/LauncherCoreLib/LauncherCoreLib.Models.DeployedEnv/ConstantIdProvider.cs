using System;
using System.Runtime.CompilerServices;

namespace LauncherCoreLib.Models.DeployedEnv;

public class ConstantIdProvider : IUserIdProvider
{
	[field: CompilerGenerated]
	public string UniqueUserId
	{
		[CompilerGenerated]
		get;
	}

	public ConstantIdProvider(string id)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		UniqueUserId = id ?? throw new ArgumentNullException("id");
	}
}
