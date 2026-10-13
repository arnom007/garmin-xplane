using System;
using System.Collections;
using System.Linq;
using LOLA.Certificates;
using LOLA.Logging;

namespace LauncherCoreLib.Utility;

public static class LayeredFileTrustExtensions
{
	public static IFileTrustLayer LogDistrustReasons(this IFileTrustLayer trust)
	{
		if (!trust.IsTrusted)
		{
			string text = string.Join("; ", Enumerable.Select<DistrustedFileTrustLayer, string>(Enumerable.OfType<DistrustedFileTrustLayer>((System.Collections.IEnumerable)trust.EnumerateLayers()), (Func<DistrustedFileTrustLayer, string>)((DistrustedFileTrustLayer distrusted) => distrusted.DistrustReason)));
			Logger.Log(LogLevel.Error, $"The file {trust.FilePath} is not trusted. Reasons: [{text}]");
		}
		return trust;
	}
}
