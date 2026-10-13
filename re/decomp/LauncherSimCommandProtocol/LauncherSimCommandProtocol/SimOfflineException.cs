using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

[Serializable]
public class SimOfflineException : IOException
{
	[field: CompilerGenerated]
	public SimInstallKey OfflineInstallKey
	{
		[CompilerGenerated]
		get;
	}

	public SimOfflineException(SimInstallKey offlineInstallKey, string message)
		: base(message)
	{
		OfflineInstallKey = offlineInstallKey;
	}

	public SimOfflineException(SimInstallKey offlineInstallKey, string message, System.Exception innerException)
		: base(message, innerException)
	{
		OfflineInstallKey = offlineInstallKey;
	}

	protected SimOfflineException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		OfflineInstallKey = (SimInstallKey)info.GetValue("OfflineInstallKey", typeof(SimInstallKey));
	}

	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((System.Exception)this).GetObjectData(info, context);
		info.AddValue("OfflineInstallKey", (object)OfflineInstallKey);
	}
}
