using System.Collections.Generic;

namespace LauncherCoreLib.Models.Simulated.Panel;

internal static class SimArguments
{
	public static string BuildLaunchArgString(System.Collections.Generic.IEnumerable<string> simSwitches)
	{
		return string.Join(" ", simSwitches);
	}
}
