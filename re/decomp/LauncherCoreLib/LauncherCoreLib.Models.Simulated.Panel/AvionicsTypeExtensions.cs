using System;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Panel;

internal static class AvionicsTypeExtensions
{
	private static readonly char[] REMOVE_S = new char[2] { ' ', 's' };

	public static AvionicsType ToAvionicEnum(this string avtypeString)
	{
		AvionicsType result = default(AvionicsType);
		if (avtypeString != null && System.Enum.TryParse<AvionicsType>(avtypeString.Trim(REMOVE_S), ref result))
		{
			return result;
		}
		return AvionicsType.Unknown;
	}
}
