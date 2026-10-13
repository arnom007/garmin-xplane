using System;

namespace LauncherCoreLib.Models.Winfs;

public static class BatchTransformSourceExtensions
{
	public static bool AppliesToAllSessions(this BatchTransformSource source)
	{
		if (source != BatchTransformSource.Essentials)
		{
			return source == BatchTransformSource.Databases;
		}
		return true;
	}

	[Obsolete("Model specific transforms are done via constraints. This is always false.")]
	public static bool IsModelSpecific(this BatchTransformSource source)
	{
		return false;
	}

	[Obsolete("Unconfigured transforms are done via constraints. This is always false.")]
	public static bool IsUnconfigured(this BatchTransformSource source)
	{
		return false;
	}
}
