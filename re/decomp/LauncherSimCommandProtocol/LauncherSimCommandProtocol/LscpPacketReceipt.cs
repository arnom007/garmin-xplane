using System;
using System.Runtime.CompilerServices;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

public class LscpPacketReceipt
{
	[field: CompilerGenerated]
	public LauncherSimCommandPacket CommandPacket
	{
		[CompilerGenerated]
		get;
	}

	[field: CompilerGenerated]
	public SimInstallKey InstallKey
	{
		[CompilerGenerated]
		get;
	}

	public LscpPacketReceipt(LauncherSimCommandPacket packet, SimInstallKey installKey)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		CommandPacket = packet ?? throw new ArgumentNullException("packet");
		InstallKey = installKey ?? throw new ArgumentNullException("installKey");
	}
}
