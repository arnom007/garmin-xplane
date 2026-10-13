using System;
using System.Runtime.CompilerServices;
using RoarClient.Net;

namespace LauncherCoreLib.Models.Simulated.Flight;

public class RoarClientProvider : IRoarClientProvider
{
	[field: CompilerGenerated]
	public IRoarClient Roar
	{
		[CompilerGenerated]
		get;
	}

	public RoarClientProvider(IRoarClient client)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Roar = client ?? throw new ArgumentNullException("client");
	}
}
