using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using LOLA.StringHelpers.Paths;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Simulated.Flight;

public static class SessionConfigurationNameFormatter
{
	public static string Format(string aircraftDisplayName, ITrainerPackage primaryPackage)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (aircraftDisplayName == null)
		{
			throw new ArgumentNullException("aircraftDisplayName");
		}
		if (primaryPackage == null)
		{
			throw new ArgumentNullException("primaryPackage");
		}
		return ReplaceInvalidPathCharacters(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "{0}_{1}_v{2}", (object)aircraftDisplayName, (object)primaryPackage.BaseInfo.DisplayName.Replace(" ", "_", (StringComparison)4), (object)primaryPackage.BaseInfo.VersionString.Replace(" ", "_", (StringComparison)4)));
		[CompilerGenerated]
		static string ReplaceInvalidPathCharacters(string str)
		{
			System.Collections.Generic.IEnumerator<char> enumerator = ((System.Collections.Generic.IEnumerable<char>)PathHelpers.GetInvalidFileNameCharactersIn(str)).GetEnumerator();
			try
			{
				while (((System.Collections.IEnumerator)enumerator).MoveNext())
				{
					char current = enumerator.Current;
					str = str.Replace(current, '_');
				}
				return str;
			}
			finally
			{
				((System.IDisposable)enumerator)?.Dispose();
			}
		}
	}
}
